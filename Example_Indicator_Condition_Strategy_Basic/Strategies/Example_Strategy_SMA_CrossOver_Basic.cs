using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using AgenaTrader.API;
using AgenaTrader.Custom;
using AgenaTrader.Plugins;
using AgenaTrader.Helper;


/// <summary>
/// Version: 1.0.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This strategy provides entry signals and order management for a SMA crossover.
/// Long  Signal when SMA20 crosses SMA50 above. Plot is set to  1
/// Short Signal wenn SMA20 crosses SMA50 below. Plot is set to -1
/// StopLoss and Target is set for 1%
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Basic strategy example for SMA crossover")]
	public class Example_Strategy_SMA_CrossOver_Basic : UserStrategy
	{
		protected override void Initialize()
		{
			CalculateOnBarClose = true;
		}

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

        //defines display name of indicator (e.g. in AgenaTrader chart window)
        public override string ToString()
        {
            return "Example SMA CrossOver Basic";
        }

        //defines display name of indicator (in AgenaTrader indicator selection window)
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Basic";
            }
        }

	}
}
