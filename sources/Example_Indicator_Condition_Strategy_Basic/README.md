#Download files
[Basic Indicator](./Indicators/Example_Indicator_SMA_CrossOver_Basic.cs)

[Basic Condition](./ScriptedConditions/Example_Condition_SMA_CrossOver_Basic.cs)

[Basic Strategy](./Strategies/Example_Strategy_SMA_CrossOver_Basic.cs)

#Basic Example for Indicator, Condition and Strategy
[Originally posted as a question in the Agenatrader forum](http://www.tradeescort.com/phpbb_de/viewtopic.php?f=18&t=2680&p=11739)

This tutorial will show you a basic example for indicators, conditions and strategies.

#Indicator
In many cases we are starting with indicators because indicators are the easiest place to start on script development.
You will get a quick indication if your trading idea is working and addionally you can screen your instruments of choice visually and verify if your trading idea will be profitable.

##OnBarUpdate
Our main logic will be inside the OnBarUpdate() method. In our example, we are using the SMA to get long and short signals. If the SMA20 is crossing above the SMA50 we get a long signal. If the SMA20 is crossing below the SMA50 we create a short signal.

```cs
protected override void OnBarUpdate()
{
            //the internal function CrossAbove checks if the two values are crossing above
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
Like in the indicator, the main logic is inside of the OnBarUpdate() method. Because our main logic is inside the indicator itself, we need to create an instance of this very indicator. So we are able to get the data from the indicator and set our "Occured" object.

```cs
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
##OnBarUpdate
Same procedure as in the condition. We create a fresh instance of the indicator and save the return value into a variable. Based on the return value we call the methods to create orders.

```cs
protected override void OnBarUpdate()
{
            string uniqueOrderName;

            //get the indicator
            Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic = LeadIndicator.Example_Indicator_SMA_CrossOver_Basic();

            //get the value
            double returnvalue = Example_Indicator_SMA_CrossOver_Basic[0];

            //Entry
            if (returnvalue == 1)
            {   
                //define a unique name for the order. in this example the current bars timestamp
                uniqueOrderName = "Long_SMA_CrossOver" + Bars[0].Time.ToString();

                //create the long order with quantity "1" and our unique OrderName
                IOrder _orderenterlong = EnterLong(1, uniqueOrderName);

                //set a stop loss for our order. we set it 1% below the current price
                SetStopLoss(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 0.99, false);

                //set a target for our order. we set it 1% above the current price
                SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, Bars[0].Close * 1.01);


            }
            else if (returnvalue == -1)
            {
                //define a unique name for the order. in this example the current bars timestamp
                uniqueOrderName = "Short_SMA_CrossOver" + Bars[0].Time.ToString();

                //create the short order with quantity "1" and our unique OrderName
                IOrder _orderentershort = EnterShort(1, uniqueOrderName);

                //set a stop loss for our order. we set it 1% above the current price
                SetStopLoss(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 1.01, false);

                //set a target for our order. we set it 1% below the current price
                SetProfitTarget(_orderentershort.Name, CalculationMode.Price, Bars[0].Close * 0.99);
            }
}
```

#Miscellaneous
##Bars required
Because of backtesting reasons: If we use the advanced mode we need at least two bars, but in our case we are using SMA50 so we need at least 50 bars. We set this in the Initialize() method.

```cs
this.BarsRequired = 50;
```

##Filenames and Class names
To import all scripts into AgenaTrader without any error we add "indicator", "strategy", "condition" or "alert" to the filename and also to the c# class name. This is important because if you like to use all files in your AgenaTrader the names must be different. It is not possible to have an indicator and condition with the same name, e.g. "SMA_CrossOver". They must have unique names like "SMA_CrossOver_indicator" and "SMA_CrossOver_condition"!

##DisplayName and ToString()
In each script we override the ToString() method and the DisplayName to provide a readable string in AgenaTrader. So we do see a readable string instead of the class name in AgenaTrader.

```cs
        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Basic";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Basic";
            }
        }
```

**Autors**

|               |                 |
|     :---:     |      :---:      |
| ![Simon Pucher](../images/user_simon_pucher_100.jpeg) | ![Christian Kovar](../images/user_christian_kovar_100.jpg) |
| Simon Pucher | Christian Kovar |
| [Twitter](https://twitter.com/SimonPucher) |  [Twitter](https://twitter.com/ckovar82) |
