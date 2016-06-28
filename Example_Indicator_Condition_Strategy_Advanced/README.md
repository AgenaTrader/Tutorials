#Template for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you a templates for indicators, conditions, strategies and give you the ability to communicate between these scripts. This will lead you to more code transparency and reduces your programming time. 

##Why do we want this?
AgenaTrader provides you the ability to create indicators, conditions, alerts and strategies in c# and use them during trading. 
Of course you can start creating an indicator and copy the code afterwards into a condition, a strategy or an alert.
Programming by using "copy & paste" is easy but on the other hand there are many disadvantages like lack of testing reasons, no single point for bug fixing and low maintainability. 

#Indicator
In many cases we are starting with indicators because indicators are the best place to start on script development. 
You will be able to get pretty quick an indication if your trading idea is working and of course you are able to screen instruments visual and verify if your trading idea will be profitable.

##Result value
The result value object will holds all result data from the calculate method so we know what to do. In a strategy we create long or short orders, in a condition we set the Occured object, and so on. In our example we use our global result value object, of course you can use your own class if you need more properties.
```C#
    /// <summary>
    /// Class which holds all important data like the OrderAction. 
    /// We use this object as a global default return object for the calculate method in indicators.
    /// </summary>
    public class ResultValue_Example_Indicator_SMA_CrossOver_Advanced
    {
        public bool ErrorOccured = false;
        public OrderAction? Entry = null;
        public OrderAction? Exit = null;
        public double Price = 0.0;
        public double Slow = 0.0;
        public double Fast = 0.0;
    }
```

##Method calculate
We want to capsulate the main logic into one main methods in the indicator. In our case we do this using the following public method in the indicator.

```C#
 public ResultValue_Example_Indicator_SMA_CrossOver_Advanced calculate(IDataSeries data, int fastsma, int slowsma, bool islongenabled, bool isshortenabled) {
   /* 
   * Here we do all the smart work and in the end we return our result object.
   * So the calling scripts knows what to do (e.g. a strategy will create an order in the market, the condition will create a signal, and so on).
   */
}
```

So it is possible that other scripts just need to call the calculate method of the indicator and get a decision what to do. 
In our case the calculate method return an object which holds all important information what has to be done. 
If we get the OrderAction.Buy as a Entry result, we need to start a long order in a strategy or we set the condition value to 1.

#Condition
We have finished our indicator so we can start now to work on our condition. 
Because we already have added our trading concept in the calculate methode in the indicator we just need a reference to our indicator and we are almost done.
```C#
//internal
private Example_Indicator_SMA_CrossOver_Advanced _Example_Indicator_SMA_CrossOver_Advanced = null;
```

We need to initalize this variable in our OnStartUp() method:
```C#
protected override void OnStartUp()
{
     base.OnStartUp();

     //Init our indicator to get code access to the calculate method
     this._Example_Indicator_SMA_CrossOver_Advanced = new Example_Indicator_SMA_CrossOver_Advanced();
}
```

Now we are ready to use the calculate method of the indicator in our OnBarUpdate() method of the condition:
```C#
//Lets call the calculate method and save the result with the trade action
ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = this._Example_Indicator_SMA_CrossOver_Advanced.calculate(this.Input, this.FastSma, this.SlowSma, this.IsLongEnabled, this.IsShortEnabled);
```

In the code snippet above we see that the return value of the calculate method is our result object from the beginning of this tutorial. So we just need to evaluate this object.
```C#
//Entry
if (returnvalue.Entry.HasValue)
{
   switch (returnvalue.Entry)
    {
        case OrderAction.Buy:
            //Long Signal
            Occurred.Set(1);
            Entry.Set(1);
            break;
        case OrderAction.SellShort:
            //Short Signal
            Occurred.Set(-1);
            Entry.Set(-1);
            break;
    }
}
else
{
   //No Signal
   Occurred.Set(0);
   Entry.Set(0);
}
```

#Strategy
Of course we are following the same procedure as in our condition. We create a variable of the indicator class, we initalize this variable during the OnStartUp() method and we use the object in our OnBarUpdate() method.
Please pay attention because of backtesting reasons if we use the advanced mode we need at least two bars!
```C#
this.BarsRequired = 2;
```
We use IsAutomated = true to decide if the strategy will do all work fully automated. In this case the strategy can be used in strategy escort and will create entry & exit orders automatically.

In the end of the strategy file there are four methods: DoEnterLong(), DoEnterShort(), DoExitLong() and DoExitShort()
In these methods we implement all rules for the creation of orders.

#Miscellaneous
##Filenames and Class names
To import all scripts into AgenaTrader without any error we add _indicator, _strategy, _condition or _alert to the filename and also to the c# class name.

##DisplayName and ToString()
In each script we override the ToString() method and the DisplayName property to provide a readable string in AgenaTrader. So we do see a readable string instead of the class name in AgenaTrader. In parentheses we add and C for Condition, I for Indicator, A for Alert and S for Strategy to ensure that we can distinguish between the scripts (e.g. if we are editing on indicator and condition in charts).
```C#
public override string ToString()
{
   return "Dummy one minute even/odd (C)";
}

public override string DisplayName
{
get
{
   return "Dummy one minute even/odd (C)";
}
}
```

#Files
[Indicator](https://github.com/AgenaTrader/Tutorials/blob/master/Example_Indicator_Condition_Strategy_Advanced/Indicators/Example_Indicator_SMA_CrossOver_Advanced.cs)

[Condition](https://github.com/AgenaTrader/Tutorials/blob/master/Example_Indicator_Condition_Strategy_Advanced/ScriptedConditions/Example_Condition_SMA_CrossOver_Advanced.cs)

[Strategy](https://github.com/AgenaTrader/Tutorials/blob/master/Example_Indicator_Condition_Strategy_Advanced/Strategies/Example_Strategy_SMA_CrossOver_Advanced.cs)
