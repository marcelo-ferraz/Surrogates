Surrogates
==========
A very simple and versatile object masher by creation of proxies.
## Overview
By making use of this container, you can overwrite (, condiotionate [to be implemented]) or visit both properties and/or methods.

#####One example:

From:
```c#
    public class RegularJoe
    {
        public int Age { get; set; }
    }
```
Keeping in mind that:
```c#
    public class Kids
    {
    	public int Quantity { get; set; }
     	
        public Kids()
        {
        	Quantity = 2;
        }
        
        public int AddTo(int value)
        {
            return value + 10 * Quantity;
        }
    }
```
We would want to change the behaviour of the property `Age`, from the class `RegularJoe`, by expressing what happens when you get kids, so we would want something slightly like:
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

To easily inject this real life abstractioned behavior, we make can use of the __`SurrogatesContainer`__. Here's how:
```c#
    public MarriedNeder()
    {
    	_container.Map(m => m
        	.Throughout<RegularJoe>()
            .Replace
            .ThisProperty(d => d.Age)
            .Accessors(a =>
            	a.Getter.Using<Kids>().ThisMethod<int, int>(d => d.AddTo)));
    }
```
And call it with:
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


