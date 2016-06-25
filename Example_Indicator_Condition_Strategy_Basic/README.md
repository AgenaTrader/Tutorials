#Template for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you a basic template for indicators, conditions and strategies.

#General
##Bars required
Because of backtesting reasons if we use the advanced mode we need at least two bars, but in our case we are using SMA50 so we need at least 50 bars. We set this in the Initialize() method.
```C#
this.BarsRequired = 50;
```

#Indicator
In many cases we are starting with indicators because indicators are the best place to start on script development. 
You will be able to get pretty quick an indication if your trading idea is working and of course you are able to screen instruments visual and verify if your trading idea will be profitable.

##OnBarUpdate
Our main logic will be inside of the OnBarUpdate() method. In our example we are using SMA to get long and short signals. If the SMA20 is crossing above the SMA50 we get a long signal. If the SMA20 is crossing below the SMA50 we create a short signal.
```C#
protected override void OnBarUpdate()
{
            //the value of SMA20 is calculated with the statemend "SMA(20)"
            //the value of SMA50 is calculated with the statemend "SMA(50)"
            //the internal function CrossAbove checks if the two values are crossing above
            //CrossAbove takes the following parameters for the calculation and returns either "true" or "false":
            //CrossAbove(double value, IDataSeries series1, int lookBackPeriod)
            //lookBackPeriod defines the number of previous bars which should be considered 
            //if lookBackPeriod is 0, it just checks for the current bar
            if (CrossAbove(SMA(20), SMA(50), 0) == true)
            {
                //set the value of the plot to "1" to inidcate a long signal
                this.SMA_CrossOver.Set(1);
            }
            //the internal function CrossBelow checks if the two values are crossing below
            else if (CrossBelow(SMA(20), SMA(50), 0) == true)
            {
                //set the value of the plot to "-1" to inidcate a short signal
                this.SMA_CrossOver.Set(-1);
            }
            else
            {
             //set the value of the plot to "0" to inidcate a flat signal
                this.SMA_CrossOver.Set(0);
            }
}
```

#Condition
##OnBarUpdate
Also in this case the main logic is inside of the OnBarUpdate() method. Because our main logic is inside of the indicator itself we need to create an instance of this indicator. So we are able to get the data from the indicator and set our Occured object.
```C#
protected override void OnBarUpdate()
{
            //get the indicator
            Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic = LeadIndicator.Example_Indicator_SMA_CrossOver_Basic();

            //get the value
            double returnvalue = Example_Indicator_SMA_CrossOver_Basic[0];

            //set the value
            Occurred.Set(returnvalue);
}
```

#Strategy
##Default time frame
If you start the strategy via the strategy-escort you need to set the default time frame. In this case you override the zero default time frame in strategy escort. If you start the strategy on a chart the time frame is automatically set, this will lead to a better usability in both cases.
```C#
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Minute, 1);
            }
```
##OnBarUpdate
Same procedure as in the condition. We create a fresh instance of the indicator and save the return value into a variable and we call the methods to create orders.

```C#
            DummyOneMinuteEvenOdd_Indicator _DummyOneMinuteEvenOdd_Indicator = LeadIndicator.DummyOneMinuteEvenOdd_Indicator();
            double returnvalue = _DummyOneMinuteEvenOdd_Indicator[0];
            if (returnvalue == 1)
            {
                this.DoEnterLong();
            }
            else if (returnvalue == -1)
            {
                this.DoEnterShort();
            }
	}
```

#Miscellaneous
##Filenames and Class names
To import all scripts into AgenaTrader without any error we add _indicator, _strategy, _condition or _alert to the filename and also to the c# class name.

##DisplayName and ToString()
In each script we override the ToString() method and the DisplayName to provide a readable string in AgenaTrader. So we do see a readable string instead of the class name in AgenaTrader.
```C#

        public override string ToString()
        {
            return "Dummy even/odd (S)";
        }

        public override string DisplayName
        {
            get
            {
                return "Dummy even/odd (S)";
            }
        }
```

#Files
[Indicator](https://github.com/simonpucher/AgenaTrader/blob/master/Indicator/DummyOneMinuteEvenOdd_Indicator.cs)

[Condition](https://github.com/simonpucher/AgenaTrader/blob/master/Condition/DummyOneMinuteEvenOdd_Condition.cs)

[Strategy](https://github.com/simonpucher/AgenaTrader/blob/master/Strategy/DummyOneMinuteEvenOdd_Strategy.cs)
