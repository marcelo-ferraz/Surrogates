# Welcome to Surrogates

Leave your code simple and direct, by doing exactly what it says. Then, to a client, provide a very sophisticated facade that will have separated concerns and responsibilities.
Those features can be reused for many other porpoises.

> <i class="icon-quote-left"></i>_"Keep your business code **simple**"_<i class="icon-quote-right"></i>

Surrogates is a direct and versatile object masher, which injects aspects and features into any code.
It hooks on your regular OOP and enables you to add highly abstracted features that otherwise would imply in many replicated lines of code, inheritance, helpers and maintenance.

> <i class="icon-quote-left"></i>_"Apply any immediate, or temporary, changes to a problematic legacy code"_<i class="icon-quote-right"></i>
> Sometimes a legacy class or system, has to have an instrumentation logic, because some method is too expensive, or a page is bloated, or just not working. You can add those behaviors to any class that is not static.
> Or you need to temporarily add some triggers to a project, so you can run a new one in parallel.

The list goes on and on. Access logic in memory, disable partially an website, create a fast and simple dependency injection logic, etc.

> :exclamation: As this is a non-linear line of thought, **it can turn, very easily, into a big mess**. So please, **use it wisely** and **document it as much as you might think you will need**.

You can find the latest version of this project at [Nuget](#www.nuget.org/packages/Surrogates/)'s site. Or the nuget in Visual Studio. It is your call.

> _The new [**.Applications**](#surrogatesapplications) project is now available! Please go the [its section](#surrogatesapplications) and check their premade aspects._

---

## Table of Contents

- [Welcome to Surrogates](#welcome-to-surrogates)

- [Mapping](#mapping)
- [Expressions](#expressions)
- [Surrogates Command Language (SCL)](#surrogates-command-language-scl)
- [Intercepting](#intercepting)
- [ Replacing](#-replacing)
- [Visiting](#-visiting)
- [ Disabling](#-disabling)
- [Rules](#rules)
- [Rules for Methods and Properties](#rules-for-methods-and-properties)
- [Rules for Parameters](#rules-for-parameters)
- [Special Parameters](#special-parameters)
- [Special Parameters for Properties](#special-parameters-for-properties)
- [Special Parameters for methods](#special-parameters-for-methods)
- [The special s_method parameter](#squirrel-the-special-s_method-parameter)
- [How to name it](#how-to-name-it)
- [How to type it](#how-to-type-it)
- [How to use it](#how-to-use-it)
- [ Usages](#-usages)
- [Creating a pool aspect](#creating-a-pool-aspect)
- [Depency Injection](#depency-injection)
- [Disabling a method](#disabling-a-method)
- [Adding instrumentation](#adding-instrumentation)
- [Adding Lazy loading Aspect](#adding-lazy-loading-aspect)
- [Adding logs](#adding-logs)
- [Intercepting specific methods](#intercepting-specific-methods)
- [Multi-dispatch delegate behaviour](#multi-dispatch-delegate-behaviour)

---

# Mapping

Mapping is responsible for creating a bind between the base type and its interceptor type.

## Expressions

The mapping through **Expressions** offers a sugar-like fluent synthax, that provides an easy to use and straight foward approach to the binding.

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

> :exclamation:**Important notes**:

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

<i class="icon-coffee"></i> Syntax for visiting:

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

## Surrogates Command Language (_SCL_)

A more _magic_ approach to mapping is by using a loose comand language. It was created to be very friendly. But it has nothing to lose when compared to an [expression](#expressions) as both providers are parsed into a strategy. Strategy that will be used inside the same engine.

It supports the same three basic operations [to replace](#replacing), [to visit](#visiting) and [to disable](#disabling). Its syntax resambles very much the [expressions mapping](#expressions) api.

<i class="icon-pencil"></i> Syntax for replacing:

```c#
_container.Map<Dummy, InterferenceObject>(
	"as d, i replace d.AccessItWillThrowException accessors getter = i.AccomplishNothing and setter = i.AccomplishNothing");
```

<i class="icon-coffee"></i> Syntax for visiting:

```c#
_container.Map<Dummy, InterferenceObject>(
	"as d, i visit d.Call_SetPropText_simple_Return_1 with i.AccomplishNothing");
```

<i class="icon-trash"></i> Syntax for disabling:

```c#
_container.Map<Dummy>(
	"as d Disable d.SetPropText_simple");
```

# Intercepting

There are three basic kinds of interception: [replace](#replacing), [visit](#visiting) and [disable](#disabling). Each will intercept and act differently withn the base method or property.

## <i class="icon-pencil"></i> Replacing

To replace a method means that the new code will be called instead of the original one. You still can call the original method, using the parameter [s_method](#user-content-the-special-s_method-parameter), with wich you can conditionate or alter the its outcome, per example.

> **About the return**: If the new method has a return, and that return is either the same type of the original return or some type that can be deduced from the original, it will be returned. Otherwise, the return will be discarded, and the original will return a default value.

## <i class="icon-coffee"></i> Visiting

To visit a method means that your new code will be called before the original method. \_If the new method has a return, this result will be discarded.

> **About the return**: If the new code does not throws an exception, it will not interrupt the original flow.

## <i class="icon-trash"></i> Disabling

It is meant to disable any method, or property.

> **About the return**: By disable, you should read: _it will be only returned Null for a reference type and the default for value type._

# Rules

In order to use the surrogacy at its best, one should know the rules which mandates the behavior of this API.

### Rules for Methods and Properties

The methods or properties in order to be intercepted, have to be non-static and marked as virtual.

Can come from a super (inherited) type.

Either protected and public modifiers are acceptable.

_So, to sum it up, any static, non-virtual, internal or private will not work out._

## Rules for Parameters

Every single parameter from the original can be passed on, as long as it respects these rules:

- Same exact name,

- Same type or one that is assignable from the original type,

_(The order does not matter.)_

# Special Parameters

When creating the interceptors, you might need to have access to the original base instance, things like the original instance, functions or properties. They are generated at build time.
Those special parameters can be different for methods and for properties.

| Type                                                                                         | Parameter                                                                  | Contents                                                                                                                       |
| -------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| `System.Object[]`                                                                            | **s_arguments**, **s_args**                                                | It contains the value of all arguments of that original method                                                                 |
| Same, can be inferred from the original class or `object`                                    | **s_instance**                                                             | It contains a pointer to the instance of the original class                                                                    |
| `System.Delegate` or the equivalent in either `System.Action<T, ...>` or `System.Func<T...>` | **s_method** or the **s\_** + **same name** of the original, _in any case_ | It contains a pointer to the original method. For more information on how to use this argument, [click here](#methodParameter) |
| Same, can be inferred from the original class or `object`                                    | **f\_** + original field name, _it has to be the same case_                | It contains a either the value or the reference of a given field                                                               |
| Same, can be inferred from the original class or `object`                                    | **p\_** + original field name, _it has to be the same case_                | It contains a either the value or the reference of a given property                                                            |

## Special Parameters only for properties

These are the parameters only available to property's interception.

| Type                                                                   | Parameter   | Contents                                                                                                                                |
| ---------------------------------------------------------------------- | ----------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| `System.String`                                                        | **s_name**  | it contains the original property's name                                                                                                |
| Same, one that can be inferred from the original parameter or `object` | **s_value** | it contains the value of the value set to the property. This only works for the setter, otherwise, it will be passed as `default(type)` |

## Special Parameters only for methods

These are the parameters only available to method's interception.

| Type            | Parameter  | Contents                               |
| --------------- | ---------- | -------------------------------------- |
| `System.String` | **s_name** | It contains the original method's name |

## The dynamic underscore Parameter, or simply \_

In order to maintain some level sanity, and to diminish the quantity of extra parameters you might want to add to your interceptor.

- There is no reflection involved in the construction of this parameter, is just a generated subtype of your proxy,
- This parameter obeys the rules of access (`Surrogates.Model.Entities.Access`), declared in the mapping of the type, meaning:
- If it contains `Access.Container`, it will have property named `Container`,
- If it contains `Access.StateBag`, it will have property named `Bag`,
- If it contains `Access.Instance`, it will have property named `Holder`,

Those are the \_'s properties:

| Type                      | Property   | Description                                                                 |
| ------------------------- | ---------- | --------------------------------------------------------------------------- |
| `BaseContainer4Surrogacy` | Container  | An instance of the container that created this type                         |
| `dynamic`                 | Bag        | A state bag, which holds whatever the developer might want, set on instance |
| `string`                  | HolderName | The name of the type that holds this interceptor                            |
| `object`                  | Holder     | The instance that holds this interceptor                                    |
| `string`                  | CallerName | The name of the method that called this method                              |
| `Delegate`                | Caller     | The method that called this method                                          |
| `object[]`                | Arguments  | All arguments from the caller                                               |

<a id="methodParameter" title="methodParameter" class="toc-item"></a>Special Parameters

## :squirrel: The special s_method parameter and how to call any method

When passing the method as parameter, there are some restrictions and a few rules. This passed method can only be used for a protected or public instance.

### How to reference the caller method:

You can make use of the original caller method, by naming a parameter as the name of that method. Its case insensitive, led by "**s\_**".

Per example, if you have the original method named **GetCommand**, any of the following is acceptable:

- **s_GetCommand**,
- **s_getCommand**,
- **s_getcommand**

_(Or, you can just simply name it `s_method`, and that will serve for any caller method.)_

### How to reference any method:

You can make use of any method, by naming the parameter as `m_` + methods name.

Per example, the method **GetCommand** shall be named `m_GetCommand`

### **How to type it**:

The method as parameter has to be exposed through a delegate or a derived type, such as `System.Action` or `System.Func<>`.

The type of this parameter has to be either:

- A **`System.Delegate`** : Making use of this type, to call the method, you can use the `DynamicInvoke` method. It accepts an array of objects and returns an object.
- The equivalent in **`System.Action<>`** or **`System.Func<>`**. The relation between a method and a `Action` or a `Func` is tightly related to the signature of the method:
- A method that returns **`void`**, is an [Action](#msdn.microsoft.com/en-us/library/system.action%28v=vs.90%29.aspx), if it returns something, it is a [Func<>](#msdn.microsoft.com/en-us/library/bb534960%28v=vs.90%29.aspx),
- The order of parameters dictate the type of such delegate:
- `void Get(string s, int i)`, turns into **`System.Action<string, int>`**,
- `long Get(object obj, DateTime i)`, turns into **`System.Func<object, DateTime, log>`**,

### **How to use it**:

Each type will change slightly the way a method is called.

| Type                  | How to call                                                                                                                                            |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `Delegate`            | `s_method.DynamicInvoke(object)`                                                                                                                       |
| `Action<string, int>` | `s_method("something", 1)`, `s_method.DynamicInvoke("str", 1)`, `s_method.DynamicInvoke(new object[] { "str", 1 })`, `s_method.Invoke("something", 1)` |
| `Func<string, int>`   | `s_method("something")`, `s_method.DynamicInvoke("str")`, `s_method.Invoke("something", 1)`                                                            |

When using `Func<>` or `Action`, there is the possibility of asynchronously call the method.

For more info, please read [this](<#msdn.microsoft.com/en-us/library/22t547yb(v=vs.90).aspx>) and [this](#msdn.microsoft.com/en-us/library/2e08f6yc.aspx).

```c#
// this will call this method asynchronously,
//and with the return, you can wait for it to finish
var asyncResult =
	s_method.BeginInvoke(null, null);

// this will make your thread wait for the result
asyncResult.AsyncWaitHandle.WaitOne();
```

---

#<i class="icon-align-left"></i> Usages

## Creating a pool aspect

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
		service = (IService) Activator
			.CreateInstace(_dictionary[key]);
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

## Adding instrumentation

_to_be_documented_

## Adding logs

_to_be_documented_

# Surrogates.Aspects

Surrogates comes with an open door for developers to create and use their own aspects. To help their endeavors, this project comes with a couple of useful extensions to the original project.

## The Apply property

By making use of it, anyone can extend and improve the capabilities of the Surrogates project. There are a couple of useful helpers when developing their own extensions.

### The Pass class

The pass class is used to expose some very needed core features of the framework.

| Method                                                                        | description                                                                                                                                                                                                    |
| ----------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `On<T>(ApplyExpression<T>, IExtension<T>)`, `On<T, TExt>(ApplyExpression<T>)` | Accepting the apply expression, you can expose the original container, their strategies and the `FactoryExpression<T>`, which is used to continue the original expression                                      |
| `Current<T>`                                                                  | Accepting an expression, you can expose the current strategy of that portion, being able to manipulate, or iterate by, their base methods, base properties, new properties, change the type builder and so on. |

## Adding a Cache Aspect

The model:

```c#
public class Simpleton
{
	Random _rnd = new Random();

	public virtual int GetRandom()
	{
		return _rnd.Next();
	}

	public virtual object GetNewObject()
	{
		return new object();
	}
}
```

The mapping syntax:

```c#
Container.Map(m => m
	.From<Simpleton>("SimpleCachedTest")
	.Apply
	.Cache(s => (Func<int>)s.GetRandom, timeout: TimeSpan.FromMilliseconds(500)));

```

## Adding a Contract Pre Validator Aspect

_to_be_documented_

## Adding an Execute Elsewhere Aspect

It enables a given method to be executed in other context. Up to now, its implemented to send the call to another thread, and to send the call to another domain.

### Executing in another Thread

Sends the calling of the method to another thread, and either waiting for it to finish, or send it and forget.

| Parameter | Type | Description                       |
| --------- | ---- | --------------------------------- |
| andForget | bool | Turn the call asynchronous or not |

Usages:

```c#
public class Simpleton
{
	public virtual string GetThreadName()
	{
		return Thread.CurrentThread.Name;
	}
}
```

The mapping syntax

```c#
Container.Map(m =>
	m.From<Simpleton>()
		.Apply
		.Calls(s => (Func<string>)s.GetThreadName).InOtherThread(andForget: true));
```

To obtain the task for that method:

```c#
var proxy = Container.Invoke<Simpleton>();

var handle =
	((Func<string>) proxy.GetThreadName).Method.MethodHandle.Value;
proxy.GetThreadName();

//.. do some work in here

// and call either a wait
((IHasTasks)proxy)[handle].Wait();

//or straight away result (it will make it wait)
var name = (string) (proxy as IHasTasks).Tasks[handle].Result;

```

### Executing in another Domain

The model:

```c#
public class Simpleton
{
	public virtual string GetDomainName()
	{
		return AppDomain.CurrentDomain.FriendlyName;
	}
}
```

The mapping syntax:

```c#
Container.Map(m =>
	m.From<Simpleton>()
		.Apply
		.Calls(s => (Func<string>)s.GetDomainName).InOtherDomain());

Container.Save();
```

## Adding an Interlocking Aspect

The model:

```c#
public class Simpleton
{
	public virtual int GetFromList(int index)
	{
		return List[index];
	}

	public virtual void Add2List(int val)
	{
		List.Add(val);
	}
}
```

The mapping syntax

```C#
Container.Map(m =>
	m.From<Simpleton>()
		.Apply
		.ReadAndWrite(s => (Func<int, int>)s.GetFromList, s => (Action<int>)s.Add2List));
```

## Adding an Inversion of Control Aspect

The Inversion of Control (IoC) and Dependency Injection (DI) patterns are all about removing dependencies from your code, and injecting such dependencies at runtime.

The model:

```c#
public class Simpleton
{
	public virtual List<int> List { get; set; }
}
```

The inject type

```c#
public class InjectedList<T>: List<T>
{
	public int Value { get; set; }
}
```

The mapping syntax:

```c#
Container.Map(m => m
 	.From<Simpleton>()
	.Apply
 	.IoCFor(s => s.List)
 	.Implying<InjectedList<int>>());
```

## Adding a Lazy Loading Aspect

One of the most admirable features of [Nhibernate](http://nhforge.org/) has to be the lazy loading feature. To be able to create a domain model clean, without a single infrastructure feature, and it will have, even then, a high level feature, it is something to be inspired.

Following you will find one example on how to do it.

The model:

```c#
public class Simpleton
{
 	public virtual string Text { get; set; }
}
```

The mapping syntax:

```c#
Container.Map(m =>
 	m.From<Simpleton>()
		.Apply
	 	.LazyLoading(s => s.Text, loader: Load));
```

To recover the "**dirty**" properties you can box it as `IContainsLazyLoadings`, and through the `LazyLoadingInterceptor` property you can retrieve all properties:

```c#
var holder = Container.Invoke<Simpleton>() as IContainsLazyLoadings;

var intProperties = holder
 	.LazyLoadingInterceptor
 	.Properties;
```

## Adding a Notify Property Change Aspect

Sometimes, inside a collection, it is needed to know whether some object had its values changed or not. With this aspect, you can keep track of those changes, by mapping all the properties you might need.

The model:

```c#
public class SimpletonList : IList<Simpleton>
{
 	// Just a plain implementation of a list
 	//(...)
}
```

The mapping syntax:

```c#
Container.Map(m => m
 	.From<SimpletonList>()
 	.Apply
 	.NotifyChanges<SimpletonList, Simpleton>(after: (l, i, v) => TextsAreEqual(i, v)));
```
