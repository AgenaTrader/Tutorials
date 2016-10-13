#Download files
[Advanced Indicator](./Indicators/Example_Indicator_SMA_CrossOver_Advanced.cs)

[Advanced Condition](./ScriptedConditions/Example_Condition_SMA_CrossOver_Advanced.cs)

[Advanced Strategy](./Strategies/Example_Strategy_SMA_CrossOver_Advanced.cs)

#Advanced Example for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you an advanced example for indicators, conditions, strategies and give you the ability to communicate between these scripts. This will lead you to more code transparency and reduces your programming time.

##Why do we want this?
AgenaTrader provides you the ability to create indicators, conditions, alerts and strategies in c# and use them during trading.
Of course, you can start creating an indicator and copy the code afterwards into a condition, a strategy or an alert.
Programming by using "copy & paste" is easy but on the other hand there are many disadvantages like lack of testing reasons, no single point for bug fixing and low maintainability.

#Indicator
In many cases we are starting with indicators because indicators are the easiest place to start on script development. You will get a quick indication if your trading idea is working and addionally you can screen your instruments of choice visually and verify if your trading idea will be profitable.

##Result value
The "ResultValue" object will hold all result data from the "calculate" method. Based on this result data, the next steps will be determined. In a strategy we create long or short orders, in a condition we set the "Occured" object, and so on. In our example, we use our global "ResultValue" object, of course you can use your own class if you need more properties.

```cs
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
We want to encapsulate the main logic into one main method in the indicator. In our case we do this using the following public method in the indicator.

```cs
public ResultValue_Example_Indicator_SMA_CrossOver_Advanced calculate(IDataSeries data, int fastsma, int slowsma, bool islongenabled, bool isshortenabled) {
/*
* Here we do all the smart work and in the end we return our result object.
* So the calling scripts knows what to do (e.g. a strategy will create an order in the market, the condition will create a signal, and so on).
*/
}
```

So it is possible that other scripts just call the "calculate" method of the indicator and get a decision of what to do.
In our case the "calculate" method returns an object which holds all important information what has to be done.
If we get the "OrderAction.Buy" as an "Entry" result, we need to start a long order in a strategy or we set the condition value to 1.

#Condition
As we have finished our indicator, we can start working on our condition.
Because we already have added our trading concept in the "calculate" method in the indicator, we just need a reference to our indicator and we are almost done.

```cs
private Example_Indicator_SMA_CrossOver_Advanced _Example_Indicator_SMA_CrossOver_Advanced = null;
```

We need to initalize this variable in our OnStartUp() method:

```cs
protected override void OnStartUp()
{
     base.OnStartUp();

     //Init our indicator to get code access to the calculate method
     this._Example_Indicator_SMA_CrossOver_Advanced = new Example_Indicator_SMA_CrossOver_Advanced();
}
```

Now we are ready to use the "calculate" method of the indicator in our OnBarUpdate() method of the condition:

```cs
//Lets call the calculate method and save the result with the trade action
ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = this._Example_Indicator_SMA_CrossOver_Advanced.calculate(this.Input, this.FastSma, this.SlowSma, this.IsLongEnabled, this.IsShortEnabled);
```

In the code snippet above we see that the return value of the "calculate" method is our "ResultObject" from the beginning of this tutorial. So we just need to evaluate this object.

```cs
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
Please pay attention while backtesting with the parameter "Orders Handling Mode = Advanced", in this case we need at least two bars!

```cs
//Because of backtesting reasons if we use the advanced mode we need at least two bars!
//In this case we are using SMA50, so we need at least 50 bars.
this.BarsRequired = 50;
```

If you start the strategy on a chart the TimeFrame is automatically set. If you start this strategy within the "Strategy Escort", it would be a smart idea to set a default TimeFrame, this will lead to a better usability. We do this by adding the default TimeFrame in the Initialize() method.

```cs
if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
{
    this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
}
```

We use IsAutomated = true to decide if the strategy will do all work fully automated. In this case the strategy can be used within the "Strategy Escort" and will create entry & exit orders automatically.

In the end of the strategy file there are four methods: DoEnterLong(), DoEnterShort(), DoExitLong() and DoExitShort()
In these methods we implement all rules for the creation of orders.

#Miscellaneous
##Filenames and Class names
To import all scripts into AgenaTrader without any error we add "indicator", "strategy", "condition" or "alert" to the filename and also to the c# class name. This is important because if you like to use all files in your AgenaTrader the names must be different. It is not possible to have an indicator and condition with the same name, e.g. "SMA_CrossOver". They must have unique names like "SMA_CrossOver_indicator" and "SMA_CrossOver_condition"!

##Color and drawing style
If the user has changed the color or the drawing style of the script (indicator or condition) we need to change the setting during the OnBarUpdate() method.

```cs
//Set the drawing style, if the user has changed it.
PlotColors[0][0] = this.Plot0Color;
Plots[0].PenStyle = this.Dash0Style;
Plots[0].Pen.Width = this.Plot0Width;
```

##DisplayName and ToString()
In each script we override the ToString() method and the DisplayName property to provide a readable string in AgenaTrader. So we do see a readable string instead of the class name in AgenaTrader. In parentheses we add and C for Condition, I for Indicator, A for Alert and S for Strategy to ensure that we can distinguish between the scripts (e.g. if we are editing on indicators or conditions in charts).

```cs
        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Advanced (I)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Advanced (I)";
            }
        }
```

**Autors**

|     :---:     |     :---:      |
| ![Simon Pucher](../images/user_simon_pucher_100.jpeg) | ![Christian Kovar](../images/user_christian_kovar_100.jpg) |
| [Twitter Simon](https://twitter.com/SimonPucher) |  [Twitter Christian](https://twitter.com/ckovar82) |
| Simon Pucher | Christian Kovar |
