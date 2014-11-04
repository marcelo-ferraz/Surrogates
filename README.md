#Surrogates
A very simple and versatile object masher, for C# developers.    
It hooks on your regular OOP and ables you to add highly abstracted features that, otherwise, would imply in many replicated lines of code, helpers and maintenance.     
Leave your code readable,  by doing exactly what it says. Then, to a end user, provide a very sophisticated façade, that will have quite separated concerns and responsibilities. And those features can be reused for many other porpoises.     

##Table of Contents
- [Surrogates](#user-content-surrogates)
	- [Table of Contents](#user-content-table-of-contents)
		- [API](#user-content-api)
			- [Parameter Rules](#user-content-parameter-rules)
			- [Special Parameters](#user-content-special-parameters)
			- [Restrictions](#user-content-restrictions)
		- [Usages](#user-content-usages)
			- [A simple example](#user-content-a-simple-example)
			- [Depency Injection](#user-content-depency-injection)
			- [Disabling a method](#user-content-disabling-a-method)
			- [Adding instrumentation](#user-content-adding-instrumentation)
			- [Lazy loading](#user-content-lazy-loading)
			- [Adding logs](#user-content-adding-logs)
			- [Intercepting specific methods](#user-content-intercepting-specific-methods)
			- [Multi-dispatch delegate behaviour](#user-content-multi-dispatch-delegate-behaviour)


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


