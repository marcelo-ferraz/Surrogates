Surrogates
==========
A very simple and versatile object masher. 
## Overview
By making use of this container, you can overwrite (, condiotionate [to be implemented]) or visit both properties and/or methods.

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
	

#####The documentation is still under development.    


[nHibernate]:http://nhforge.org/