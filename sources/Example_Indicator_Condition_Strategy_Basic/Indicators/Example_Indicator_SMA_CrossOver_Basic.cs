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
/// Version: 1.0.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry signals for a SMA crossover.
/// Long  Signal when SMA20 crosses SMA50 above. Plot is set to  1
/// Short Signal wenn SMA20 crosses SMA50 below. Plot is set to -1
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
	[Description("Basic indicator example for SMA crossover")]
	public class Example_Indicator_SMA_CrossOver_Basic : UserIndicator
	{
		protected override void OnInit()
		{
            //Define the plot and its color which is displayed underneath the chart
			Add(new Plot(Color.Orange, "SMA_CrossOver"));

            //Define if the OnBarUpdate method should be triggered only on BarClose (=end of period)
            //or with each price update
			CalculateOnClosedBar = true;

            this.RequiredBarsCount = 50;
        }

		protected override void OnCalculate()
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

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Basic (I)";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Basic (I)";
            }
        }


		#region Properties
        //output data for plot
		[Browsable(false)]
		[XmlIgnore()]
        public DataSeries SMA_CrossOver
        {
			get { return Outputs[0]; }
		}

		#endregion
	}
}

#region AgenaTrader Automaticaly Generated Code. Do not change it manualy

namespace AgenaTrader.UserCode
{
	#region Indicator

	public partial class UserIndicator
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic()
        {
			return Example_Indicator_SMA_CrossOver_Basic(InSeries);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic(IDataSeries input)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Example_Indicator_SMA_CrossOver_Basic>(input);

			if (indicator != null)
				return indicator;

			indicator = new Example_Indicator_SMA_CrossOver_Basic
						{
							RequiredBarsCount = RequiredBarsCount,
							CalculateOnClosedBar = CalculateOnClosedBar,
							InSeries = input
						};
			indicator.SetUp();

			CachedCalculationUnits.AddIndicator2Cache(indicator);

			return indicator;
		}
	}

	#endregion

	#region Strategy

	public partial class UserStrategy
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic()
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Basic(InSeries);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic(IDataSeries input)
		{
			if (IsInInit && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'OnInit()' method");

			return LeadIndicator.Example_Indicator_SMA_CrossOver_Basic(input);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic()
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Basic(InSeries);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic(IDataSeries input)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Basic(input);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic()
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Basic(InSeries);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Basic Example_Indicator_SMA_CrossOver_Basic(IDataSeries input)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Basic(input);
		}
	}

	#endregion

}

#endregion
