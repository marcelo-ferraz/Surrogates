#Welcome to Surrogates
Leave your code simple and direct, by doing exactly what it says. Then, to a client, provide a very sophisticated façade, that will have separated concerns and responsibilities. 
Those features can be reused for many other porpoises.      

><i class="icon-quote-left"></i>*"Keep your business code **simple**"*<i class="icon-quote-right"></i>

Surrogates is a direct and versatile object masher, which injects aspects and functionalities into any code.
It hooks on your regular OOP and ables you to add highly abstracted features that, that otherwise, would imply in many replicated lines of code, inheritance, helpers and maintenance.     
><i class="icon-quote-left"></i>*"Apply any immediate, or temporary, changes to a problematic legacy code"*<i class="icon-quote-right"></i>

____
## Table of Contents

____
## API
### Mapping
Mapping is responsible for creating a bind between the base type and its interceptor type.
#### Expressions
The mapping through __Expressions__ offers a sugar-like fluent synthax, that provides an easy to use and straight foward approach to the binding.    
the syntax is supposed to be read as a sentence or a phrase:
```c#
_container.Map(m => 
	m.From<RegularJoe>()
	 .Replace
	 .This(r => (Func<int>) r.GetAge)
	 .Using<TwoKids>("NewMethod"));    
```
It supports multiple operations for a single expression, to create a very complex Surrogated type.
This expression API has support for methods and properties.
>**<i class="icon-attention"></i> Important notes**: 

> - Each expression made will be used to create one new proxy.     
> - In order to create multiple proxies, from a same type, You have to give it a different name.

Examples: 
<i class="icon-pencil"></i> Syntax for replacing:
```c#
_container.Map(m => m
	.From<Dummy>()
	.Replace
	.This(d => d.AccessItWillThrowException)
	.Accessors(a =>
	{
		Set4Property.OneSimpleGetter(a);
		Set4Property.OneSimpleSetter(a);
	}));    
```
 <i class="icon-coffee"></i>Syntax for visiting:
```c#
_container.Map(m => m
	.From<Dummy>()
	.Visit
	.This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
	.Using<InterferenceObject>(r => (Action) r.AccomplishNothing));   
```
<i class="icon-trash"></i> Syntax for disabling:
```c#
_container.Map(m => m
	.From<Dummy>()
	.Disable
	.Method("SetPropText_simple"));
```
#### Surrogates Command Language (*SCL*)
### Intercepting
Intercepting
There are three basic kinds of interception: __Replace__, __Visit__ and __Disable__. Each will intercept and act differently withn the base method or property. 

#### <i class="icon-pencil"></i> Replacing 
To replace a method means that the new code will be called instead of the original one. You still can call the original method, using the parameter [s_method](#user-content-the-special-s_method-parameter), with wich you can conditionate or alter the its outcome, per example.    

>**About the return**: If the new method has a return, and that return is either the same type of the original return or some type that can be deduced from the original, it will be returned. Otherwise, the return will be discarded, and the original will return a default value.    

#### <i class="icon-coffee"></i>Visiting
To visit a method means that your new code will be called before the original method. _If the new method has a return, this result will be discarded.     
>**About the return**: If the new code does not throws an exception, it will not interrupt the original flow.     

#### <i class="icon-trash"></i> Disabling
It is meant to disable any method, or property. 
>**About the return**: By disable, you should read: *it will be only returned Null for a reference type and the default for value type.*
      
### Rules

#### Rules for Methods and Properties
The methods or properties in order to be intercepted, have to be non-static and marked as virtual.
Can come from a super (inherited) type. 
Either protected and public modifiers are acceptable. 
*So, to sum it up, any static, non-virtual, internal or private will not work out.*

#### Rules for Parameters
Every single parameter from the original can be passed on, as long as it respects these rules:      

+   Same exact name,
+   Same type or one that is assignable from the original type,     

_(The order does not matter.)_
   
### <i class="icon-exchange"> </i>Special Parameters
For all that it matters, just being able to execute an action in a method, inside another just seem too limited, so this framework  made some special parameters to be used, hopefully, a lot. 
Those special parameters are different for Methods and for Properties.      

#### Special Parameters for Properties
Type       | Parameter     | Contents
--------   |---------------| -------------
`System.String`   | __s_name__    | it contains the original property's name
Same or one that can be inferred from the original parameter | __s_value__     | it contains the value of the value set to the property. This only works for the setter, otherwise, it will be passed as `default(type)` 
Same or can be inferred from the original class | __s_instance__      | It contains a pointer to the instance of the original class     

#### Special Parameters for methods
Type       | Parameter     | Contents
--------   |---------------| -------------
`System.Object[]` | __s_arguments__     | It contains the value of all arguments of that original method 
`System.String`   | __s_name__    | It contains the original method's name
Same or one that can be inferred from the original class | __s_instance__      | It contains a pointer to the instance of the original class 
`System.Delegate` or the equivalent in either `System.Action<T, ...>` or `System.Func<T...>` | **s_method** or the __s___ + **same name** of the original, __in any case__ | It contains a pointer to the original method. For more information on how to use this argument, [click here](#methodParameter)

<a id="methodParameter" title="methodParameter" class="toc-item"></a>

#### <i class="icon-rocket"></i>The special s_method parameter
When passing the method as parameter, there are some restrictions and a few rules, to ease its use. It can only be a protected or public instance method. 
#####How to name it:
You can make use of the original method name, in any letter case, led by "__s___".    
Per example, if you have the original method named __GetCommand__, any of the following is acceptable:

 - __s_GetCommand__,
 - __s_getCommand__,
 - __s_getcommand__      
 
_(You can just simply name it __s_method__, which serves for any method.)_
#####**How to type it**:
The original method will be exposed through delegate or a derived type, like an ``System.Action`` or a ``System.Func<>``:   

- A __`System.Delegate`__  : Making use of this type, to call the method, you can use the `DynamicInvoke` method. It accepts an array of objects and returns an object. As it calls a late-bound call as it demands boxing and unboxing, it may give a small overhead compared to regular call. 

<i class="icon-plus-circled"></i> Example:  ```s_method.DynamicInvoke(object)```

- The equivalent in __`System.Action<>`__ or __`System.Func<>`__, keep in mind that this is simplest and fastest, with no over heading, and no need for boxing and unboxing, in comparison to a native call, this call it will have the a very close performance.     
The relation between a method and a Action or a function is this:
 - a method that returns __`void`__, is an __Action__, if it returns something, it is a __Func__,
  - the order of parameters dictate the type of such delegate:    
  - `void Get(string s, int i)`, turns into __`System.Action<string, int>`__,  
  - `long Get(object obj, DateTime i)`, turns into __`System.Func<object, DateTime, log>`__, 

<i class="icon-plus-circled"></i> Examples:  
```c#
// a simple void method, (it will ask for the right parameters, and can return the right type)
s_method(/*right parameters*/ (...));

// a simple method that returns a value 
SomeType result = (SomeType) s_method.DynamicInvoke(object);

// another example of invoke (it will ask for the right parameters, and can return the right type):
s_method.Invoke(/*right parameters*/ (...));

// it will call this method asynchronously, and with the return, you can wait the result 
var asyncResult = s_method.BeginInvoke(object);
// this will make your thread wait for the result
asyncResult.AsyncWaitHandle.WaitOne();
```

____

### <i class="icon-align-left"></i> Usages
#### Creating a pool aspect
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
	m.Throughout<SrvFactory>()
	 .Replace
	 .Method("GetService")
	 .Using<PoolInterceptor>("Intercept"));
```
Or for a property, 
```c#
container.Map(m => 
	m.Throughout<Legacy>()
	 .Replace
	 .This(l => l.Service)
	 .Accessors(p => 
		 p.Getter.Using<PoolInterceptor>("Intercept"));
```
When this behaviour is not desired, just delete the map.

Sometimes a legacy class or system, has to have an instrumentation logic, because some method is too expensive, or a page is bloated, or just not working. You can add those behaviors to any class that is not static.    
Or you need to temporarily add some triggers to a project, so you can run a new one in parallel.      
The list goes on and on. Access logic in memory, disable partially an website, create a fast and simple dependency injection logic, etc.     
><i class="icon-info-circled"></i> As this is a non-linear line of thought, **it can turn, very easily, into a big mess**. So please, **use it wisely** and **document it as much as you might think you will need**.

#### Depency Injection
_to_be_documented_
#### Disabling a method
_to_be_documented_
#### Adding instrumentation
_to_be_documented_
#### Adding Lazy loading Aspect 
One of the most admirable features of [Nhibernate](http://nhforge.org/) has to be the lazy loading feature. To be able to create a domain model clean, without a single infrastructure feature, and it will have, even then, a high abstractioned feature, it is something to be inspired.     
Following you will find one example on how to do it.       
The model:
```c#
    public class SimpleModel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int OutterId { get; set; }
    }
```

The loader class:
```c#
    public class IdLazyLoader
    {
        private int _value;
        private bool _isDirty = false;

        private MockedRepository _repository = new MockedRepository();

        public int Load(string propertyName)
        {
            return object.Equals(_value, default(int)) ?
                (_value = _repository.Get<int>(propertyName)) :
                _value;
        }

        public void MarkAsDirty(int value)
        {
            _isDirty = true;
            _value = value;
        }
    }
```
The class map: 
```c#
    public void Map()
    {
        _container.Map(m => m
            .From<SimpleModel>()
            .Replace
            .This(d => d.Id)
            .Accessors(a => a
                .Getter.Using<IdLazyLoader>("idLoader", l => (Func<string, int>)l.Load)
                .And
                .Setter.Using<IdLazyLoader>("idLoader", (Action<int>)l => l.MarkAsDirty));
    }
```


And the usage:
```c#
   public void Test()
        {
            var model = 
                _container.Invoke<SimpleModel>();

            try
            {
                var id = model.Id;
                Assert.Fail();
            }
            catch (NotImplementedException)
            {
                Assert.Pass(); 
            }
        }
``` 
#### Adding logs
_to_be_documented_
#### Intercepting specific methods
_to_be_documented_
#### Multi-dispatch delegate behaviour
_to_be_documented_
