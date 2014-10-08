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
To easily inject this real life abstractioned behavior, we can map such attrocity to the regular fellow using the __`SurrogatesContainer`__. Here's how:
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
Same or can be infered from the original class | __s_instance__      | It contains a pointer to the instance of the original class 
`System.Delegate` or the equivalent in either `System.Action<T, ...>` or `System.Func<T...>` | **s_method** or the __s___ + **same** name of the original, __in any case__ | It contains a pointer to the original method. For mor information on how to use this argument, [click here](#methodParameter)

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

- a __`System.Delegate`__    

Making use of this type, to call the method, you can use the `DynamicInvoke` method. It accepts an array of objects and returns an object. As it calls a late-bound call, it can give a small overhead to regular call.

- the equivalent in __`System.Action<>`__ or __`System.Func<>`__      

Please, keep in mind that this is simplest and fastest, with no overheading, in relation to a native call.     
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