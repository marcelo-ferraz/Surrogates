#Surrogates
A very simple and versatile object masher, for C# developers.    
It hooks on your regular OOP and ables you to add highly abstracted features that, otherwise, would imply in many replicated lines of code, helpers and maintenance.     
Leave your code readable,  by doing exactly what it says. Then, to a end user, provide a very sophisticated façade, that will have quite separated concerns and responsibilities. And those features can be reused for many other porpoises.     


##Table of Contents
- API
	- Mapping
 	- Restrictions    
 	- Parameters
 		- Parameter Rules
 		- Special Parameters 
- Usages	
 	- A Simple Example
 	- Depency Injection
 	- Disabling a method
 	- Adding instrumentation
 	- Lazy loading
 	- Adding logs
 	- Intercepting specific methods
 	- Multi-dispatch delegate behaviour


### API
#### Parameter Rules
#### Special Parameters 
#### Restrictions

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
>As this is a non-linear line of thought, **it can turn, very easily, into a big mess**. So please, **use it wisely and document it as much as you need it**.

#### Depency Injection
#### Disabling a method
#### Adding instrumentation
#### Lazy loading
#### Adding logs
#### Intercepting specific methods
#### Multi-dispatch delegate behaviour

<div style="display:none">
old doc


Surrogates
==========
A very simple and versatile object masher. 
## Overview
By making use of this container, you can overwrite or visit both properties and/or methods. [click here](#method-parameter)

###Examples:

#### Example 1: Lazy loading
We all love [nHibernate], right? I certainly do!    
One of the thing I do admire is the whole lazy loading feature. We can have our domain model all clean and neat, without a single infrastructure feature.     
Ain't it nice?      
But you see, our applications, usually, cannot feature such high abstraction, either because the difficulty, because the time, or just because.     

Would it be swell to be able to have it applied to our most dangerous desires?      
Oh jolly, now you can! (and with ease):    

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
            .Throughout<SimpleModel>()
            .Replace
            .ThisProperty(d => d.Id)
            .Accessors(a => a
                .Getter.Using<IdLazyLoader>("idLoader").ThisMethod<string, int>(l => l.Load)
                .And
                .Setter.Using<IdLazyLoader>("idLoader").ThisMethod<int>(l => l.MarkAsDirty))
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
    
#### Example 2: A simple extension
What if, whilst having a domain model like `RegularJoe` that we'd want to change the behaviour of the property `Age`, to express what really happens to you when you get kids.     
So basically, we would want something slightly like:

```c#
    public class RegularJoeWithKids
    {
        private Kids _kids;

        public MarriedNeder()
        {
            _kids = new Kids();
        }
        
        public int Age
        {
            get { return _kids.AddTo(_age); }
            set { _age = value; }
        }
    }
```
The regular joe fellow model:
```c#
    public class RegularJoe
    {
        public int Age { get; set; }
    }
```
The reason of having white hair:
```c#
    public class TwoKids
    {
        public int Quantity { get; set; }
     	
        public TwoKids()
        {
        	Quantity = 2;
        }
        
        public int AddTo(int value)
        {
            return value + 10 * Quantity;
        }
    }
```
To easily inject this real life abstracted behaviour, we can map such atrocity to the regular fellow using the __`SurrogatesContainer`__. Here's how:
```c#
    public void Map()
    {
    	_container.Map(m => m
        	.Throughout<RegularJoe>()
            .Replace
            .ThisProperty(d => d.Age)
            .Accessors(a =>
            	a.Getter.Using<TwoKids>().ThisMethod<int, int>(d => d.AddTo)));
    }
```
And then, when we call the regular joe fellow, it will gives us back a broken man, filled with joy and happiness:
```c#
	public void SomeAct()
    {
    	// some processing here
        
        RegularJoe joeWithKids = 
        	_container.Invoke<RegularJoe>();
        
        joeWithKids.Age = 18;
                
        // his age will be 38
        var age = 
        	joeWithKids.Age;
        
        // some processing here
    }
```

##The Parameter rule
Every single parameter from the original can be passed on, as long as it respects these rules:      

+   Same exact name
+   Same or assignable type
+   The order does not matter.

##The Special Parameters
For all that matter, just being able to execute an action in a method, inside another just seem too limited, so for I've made some special parameters to be used, hopefully, a lot. 
They are different for Methods and for Properties.    

####1.   For methods:

Type       | Parameter     | Contents
--------   |---------------| -------------
`System.Object[]` | __s_arguments__     | It contains the value of all arguments of that original method 
`System.String`   | __s_name__    | it contains the original method's name
Same or can be inferred from the original class | __s_instance__      | It contains a pointer to the instance of the original class 
`System.Delegate` or the equivalent in either `System.Action<T, ...>` or `System.Func<T...>` | **s_method** or the __s___ + **same** name of the original, __in any case__ | It contains a pointer to the original method. For more information on how to use this argument, [click here](#methodParameter)

####2.   For Properties:

Type       | Parameter     | Contents
--------   |---------------| -------------
`System.String`   | __s_name__    | it contains the original property's name
Same or can be inferred from the original parameter | __s_value__     | it contains the value of the value set to the property. This only works for the setter, otherwise, it will be passed as `default(type)` 
Same or can be inferred from the original class | __s_instance__      | It contains a pointer to the instance of the original class 

   
<a id="methodParameter" title="methodParameter" class="toc-item"></a>
###The special s_method parameter
When passing the method as parameter, there are some restrictions, a few rules and a helper, in to ease the use.
#####Rules
Can only be an protected or public method.
#####Naming 
The name can be either the same name of the original method, in any case (per example: if the orginal method is named __GetCommand__,   __s_GetCommand__, __s_getCommand__ and __s_getcommand__ are all equivalent), or just simply put __s_method__.
#####Typing
The type can be either:    

- a __`System.Delegate`__  : Making use of this type, to call the method, you can use the `DynamicInvoke` method. It accepts an array of objects and returns an object. As it calls a late-bound call, it can give a small overhead to regular call.

- the equivalent in __`System.Action<>`__ or __`System.Func<>`__ : Please, keep in mind that this is simplest and fastest, with no overheading, in relation to a native call.     
The relation between a method and a Action and a function is this:
 - a method that returns __`void`__, is an __Action__, if it returns something, it is a __Func__,
 - the order of parameters dictate the type of such delegate:    
 - `void Get(string s, int i)`, turns into __`System.Action<string, int>`__,  
 - `Datetime Get(string s, int i)`, turns into __`System.Func<string, int, DateTime>`__,     
 
<br />
<br />
<br />


#####The documentation is still under development. 
[nHibernate]:http://nhforge.org/
</div>
