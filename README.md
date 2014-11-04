#Surrogates
A very simple and versatile object masher, for C# developers.    
It hooks on your regular OOP and ables you to add highly abstracted features that, otherwise, would imply in many replicated lines of code, helpers and maintenance.     
Leave your code readable,  by doing exactly what it says. Then, to a end user, provide a very sophisticated façade, that will have quite separated concerns and responsibilities. And those features can be reused for many other porpoises.     
____
### Table of Contents
- [API](#user-content-api)
	- [Parameter Rules](#user-content-parameter-rules)
	- [Mapping](#user-content-mapping)
	- [Special Parameters](#user-content-special-parameters)
		- [Special Parameters for Properties](#user-content-Special-Parameters-for-Properties)
		- [Special Parameters for Methods](#user-content-Special-Parameters-for-Methods)
- [Usages](#user-content-usages)
	- [A simple example](#user-content-a-simple-example)
	- [Depency Injection](#user-content-depency-injection)
	- [Disabling a method](#user-content-disabling-a-method)
	- [Adding instrumentation](#user-content-adding-instrumentation)
	- [Lazy loading](#user-content-lazy-loading)
	- [Adding logs](#user-content-adding-logs)
	- [Intercepting specific methods](#user-content-intercepting-specific-methods)
	- [Multi-dispatch delegate behaviour](#user-content-multi-dispatch-delegate-behaviour)

____
### API
This framework can be divided in: __Mapping__, __Method Rules__ and __Parameter Rules__. The first is responsible for creating a bind between the base code and the interceptor code.     
To make use of the original class instance, or original method's parameters or information, there are some rules that have to be followed. And for that matter, this document divides into rules for methods (, properties) and those method's parameters.

#### Mapping
_to_be_documented_
#### Method Rules
_to_be_documented_
#### Parameter Rules
Every single parameter from the original can be passed on, as long as it respects these rules:      

+   Same exact name
+   Same or assignable type
+   The order does not matter.

#### Special Parameters 
For all that it matters, just being able to execute an action in a method, inside another just seem too limited, so for I've made some special parameters to be used, hopefully, a lot. 
Those special parameters are different for Methods and for Properties. 

####Special Parameters for Properties:

Type       | Parameter     | Contents
--------   |---------------| -------------
`System.String`   | __s_name__    | it contains the original property's name
Same or can be inferred from the original parameter | __s_value__     | it contains the value of the value set to the property. This only works for the setter, otherwise, it will be passed as `default(type)` 
Same or can be inferred from the original class | __s_instance__      | It contains a pointer to the instance of the original class 

####Special Parameters for methods:

Type       | Parameter     | Contents
--------   |---------------| -------------
`System.Object[]` | __s_arguments__     | It contains the value of all arguments of that original method 
`System.String`   | __s_name__    | it contains the original method's name
Same or can be inferred from the original class | __s_instance__      | It contains a pointer to the instance of the original class 
`System.Delegate` or the equivalent in either `System.Action<T, ...>` or `System.Func<T...>` | **s_method** or the __s___ + **same** name of the original, __in any case__ | It contains a pointer to the original method. For more information on how to use this argument, [click here](#methodParameter)

<a id="methodParameter" title="methodParameter" class="toc-item"></a>
####The special s_method parameter
When passing the method as parameter, there are some restrictions, a few rules and a helper, in to ease the use. it can only be a protected or public instance method. 
#####How to name it:
You can make use of the original method name, in any letter case, led by "__s___".    
Per example, if you have the original method named __GetCommand__, any of the following is acceptable:

 - __s_GetCommand__,
 - __s_getCommand__,
 - __s_getcommand__      
 
Or you can just simply name it __s_method__.
#####How to type it:
The original method will be exposed through delegate or a derivated type, like an ``System.Action`` or a ``System.Func<>``. 

- a __`System.Delegate`__  : Making use of this type, to call the method, you can use the `DynamicInvoke` method. It accepts an array of objects and returns an object. As it calls a late-bound call, it can give a small overhead to regular call.

- the equivalent in __`System.Action<>`__ or __`System.Func<>`__, keep in mind that this is simplest and fastest, with no overheading, and no need for boxing and unboxing, in comparison to a native call, this call it will have the same perfomance.     
The relation between a method and a Action or a function is this:
 - a method that returns __`void`__, is an __Action__, if it returns something, it is a __Func__,
  - the order of parameters dictate the type of such delegate:    
  - `void Get(string s, int i)`, turns into __`System.Action<string, int>`__,  
  - `long Get(object obj, DateTime i)`, turns into __`System.Func<object, DateTime, log>`__, 
   
____

### Usages
#### A simple example
Take a simple factory:
```c#
public IService GetService(string key)
{
	var srv = (IService) 
		Activator.CreateInstace(_dictionary[key]);
	srv.Init();
	return srv;
}
```
In order to have a log, your method would have to: 
```c#
public IService GetService(string key)
{
	Iservice service = null;
	Log.Debug("Getting: {0}", key);
	try
	{
		 service = (IService) Activator.CreateInstace(_dictionary[key]);
		 service.Init();
	}
	catch(Exception ex)
	{
		Log.Exception(ex);
	}
	return service;
}
```
If you want to add a pool behaviour to it, the method would grow to a much larger and verbose syntax. In order to have it in more than one, it would imply in either replication of code, the creation of a super|base class with many pre-set features or static tools or mixins, which would smudge a little the reading of it;
Surrogates proposes this:    
With a PoolInterceptor, you could simply add, to any instances, of any different types the same high end feature logic, and turn off when needed:    
```c#
public IService Intercept(Func<string, IService> s_method, string key)
{
	if(string.IsNullOrEmpty(key))
	{ throw new ArgumentException("key"); } 
	
	IService srv = null;
	
	Log.Debug("Getting: {0}", key);		
	if(!_pool.TryGetFrom(key, out srv)) 
	{
		Log.Debug("Instance not found on Pool, storing a new instance");
		try
		{
			srv = (IService) _pool.Store(s_method(key)).Clone();
		}
		catch(Exception ex)
		{
			Log.Exception(ex);
		}	 
	}	
	return srv;
} 
```
Any class that has a method (public or protected) that returns an IService, can have their pool and a static log. Just by mapping it:
```c#
container.Map(m => 
	m.Throughout<SrvFactory>().Replace.ThisMethod("GetService").Using<PoolInterceptor>().ThisMethod("Intercept"));
```
Or for a property, 
```c#
container.Map(m => 
	m.Throughout<Legacy>().Replace.ThisProperty(l => l.Service).Accessors(p => p.Getter.Using<PoolInterceptor>().ThisMethod("Intercept"));
```
When this behaviour is not desired, just delete the map.

Sometimes a legacy class or system, has to have an instrumentation logic, because some method is too expensive, or a page is bloated, or just not working. You can add those behaviours to any class that is not static.    
Or you need to temporarily add some triggers to a project, so you can run a new one in parallel.      
The list goes on and on. Access logic in memory, disable partially an website, create a fast and simple depedency injection logic, etc.     
>As this is a non-linear line of thought, **it can turn, very easily, into a big mess**. So please, **use it wisely and document it as much as you might think you will need**.

#### Depency Injection
_to_be_documented_
#### Disabling a method
_to_be_documented_
#### Adding instrumentation
_to_be_documented_
#### Lazy loading
_to_be_documented_
#### Adding logs
_to_be_documented_
#### Intercepting specific methods
_to_be_documented_
#### Multi-dispatch delegate behaviour
_to_be_documented_