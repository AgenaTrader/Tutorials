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
/// Version: 1.0
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This indicator provides entry and exit signals for a SMA crossover.
/// Long  Signal when SMA20 crosses SMA50 above. Plot is set to  1
/// Short Signal wenn SMA20 crosses SMA50 below. Plot is set to -1
/// You can use this indicator also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{

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
    }

    [Description("Basic indicator example for SMA crossover")]
    public class Example_Indicator_SMA_CrossOver_Advanced : UserIndicator
    {

        //input
        private Color _plot0color = Color.Green;
        private int _plot0width = 1;
        private DashStyle _plot0dashstyle = DashStyle.Solid;
        private Color _plot1color = Color.Red;
        private int _plot1width = 1;
        private DashStyle _plot1dashstyle = DashStyle.Solid;
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;


        //output

        //internal


        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            //Define the plots and its color which is displayed underneath the chart
            Add(new Plot(this.Plot0Color, "SMA_CrossOver_Entry"));
            Add(new Plot(this.Plot1Color, "SMA_CrossOver_Exit"));

            //Define if the OnBarUpdate method should be triggered only on BarClose (=end of period)
            //or with each price update
            CalculateOnBarClose = true;

            this.BarsRequired = 50;
        }

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
            base.InitRequirements();
        }


        /// <summary>
        /// Is called on startup.
        /// </summary>
        protected override void OnStartUp()
        {
            //Print("OnStartUp");
            base.OnStartUp();

        }

        /// <summary>
        /// Called on each update of the bar.
        /// </summary>
        protected override void OnBarUpdate()
        {

            //Check if peridocity is valid for this script
            if (!DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": Periodicity of your data feed is suboptimal for this indicator!", InfoLogLevel.AlertLog);
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = this.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

            //If the calculate method was not finished we need to stop and show an alert message to the user.
            if (returnvalue.ErrorOccured)
            {
                Log(this.DisplayName + ": A problem has occured during the calculation method!", InfoLogLevel.AlertLog);
                return;
            }

            //Entry
            if (returnvalue.Entry.HasValue)
            {
                switch (returnvalue.Entry)
                {
                    case OrderAction.Buy:
                        //DrawDot("ArrowLong_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, this.Plot0Color);
                        this.Indicator_Curve_Entry.Set(1);
                        break;
                    case OrderAction.SellShort:
                        //DrawDiamond("ArrowShort_Entry" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, this.Plot0Color);
                        this.Indicator_Curve_Entry.Set(-1);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                this.Indicator_Curve_Entry.Set(0);
            }

            //Exit
            if (returnvalue.Exit.HasValue)
            {
                switch (returnvalue.Exit)
                {
                    case OrderAction.BuyToCover:
                        //DrawDiamond("ArrowShort_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, this.Plot1Color);
                        this.Indicator_Curve_Exit.Set(-0.5);
                        break;
                    case OrderAction.Sell:
                        //DrawDot("ArrowLong_Exit" + Bars[0].Time.Ticks, true, Bars[0].Time, Bars[0].Open, this.Plot1Color);
                        this.Indicator_Curve_Exit.Set(0.5);
                        break;
                }
            }
            else
            {
                //Value was null so nothing to do.
                this.Indicator_Curve_Exit.Set(0);
            }
        }

        /// <summary>
        /// Is called if the indicator stops.
        /// </summary>
        protected override void OnTermination()
        {
            //Print("OnTermination");
        }


        /// <summary>
        /// In this method we do all the work and return the object with all data like the OrderActions.
        /// This method can be called from any other script like strategies, indicators or conditions.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultValue_Example_Indicator_SMA_CrossOver_Advanced calculate(IBar data, bool islongenabled, bool isshortenabled)
        {
            //Create a return object
            ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = new ResultValue_Example_Indicator_SMA_CrossOver_Advanced();

            //try catch block with all calculations
            try
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
                    if (islongenabled)
                    {
                        returnvalue.Entry = OrderAction.Buy;
                    }
                    if (isshortenabled)
                    {
                        returnvalue.Exit = OrderAction.SellShort;
                    }
                    returnvalue.Price = data.Close;
                }
                //the internal function CrossBelow checks if the two values are crossing below
                else if (CrossBelow(SMA(20), SMA(50), 0) == true)
                {
                    //set the value of the plot to "-1" to inidcate a short signal
                    if (islongenabled)
                    {
                        returnvalue.Entry = OrderAction.Sell;
                    }
                    if (isshortenabled)
                    {
                        returnvalue.Exit = OrderAction.BuyToCover;
                    }
                    returnvalue.Price = data.Close;
                }

            }
            catch (Exception)
            {
                //If this method is called via a strategy or a condition we need to log the error.
                returnvalue.ErrorOccured = true;
            }

            //return the result object
            return returnvalue;
        }




        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader chart window)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Example SMA CrossOver Advanced";
        }

        /// <summary>
        /// defines display name of indicator (e.g. in AgenaTrader indicator selection window)
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Example SMA CrossOver Advanced";
            }
        }


        /// <summary>
        /// True if the periodicity of the data feed is correct for this indicator.
        /// </summary>
        /// <returns></returns>
        public bool DatafeedPeriodicityIsValid(ITimeFrame timeframe)
        {
            TimeFrame tf = (TimeFrame)timeframe;
            if (tf.Periodicity == DatafeedHistoryPeriodicity.Day ||  tf.Periodicity == DatafeedHistoryPeriodicity.Hour || tf.Periodicity == DatafeedHistoryPeriodicity.Minute)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region Properties



        #region Input


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create long positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Long")]
        public bool IsLongEnabled
        {
            get { return _IsLongEnabled; }
            set { _IsLongEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("If true it is allowed to create short positions.")]
        [Category("Parameters")]
        [DisplayName("Allow Short")]
        public bool IsShortEnabled
        {
            get { return _IsShortEnabled; }
            set { _IsShortEnabled = value; }
        }


        /// <summary>
        /// </summary>
        [Description("Select Color for the long indicator.")]
            [Category("Plots")]
            [DisplayName("Color long")]
            public Color Plot0Color
            {
                get { return _plot0color; }
                set { _plot0color = value; }
            }
            // Serialize Color object
            [Browsable(false)]
            public string Plot0ColorSerialize
            {
                get { return SerializableColor.ToString(_plot0color); }
                set { _plot0color = SerializableColor.FromString(value); }
            }

            /// <summary>
            /// </summary>
            [Description("Line width for long indicator.")]
            [Category("Plots")]
            [DisplayName("Line width long")]
            public int Plot0Width
            {
                get { return _plot0width; }
                set { _plot0width = Math.Max(1, value); }
            }

            /// <summary>
            /// </summary>
            [Description("DashStyle for long indicator.")]
            [Category("Plots")]
            [DisplayName("DashStyle long")]
            public DashStyle Dash0Style
            {
                get { return _plot0dashstyle; }
                set { _plot0dashstyle = value; }
            }


        /// <summary>
        /// </summary>
        [Description("Select Color for the short indicator.")]
        [Category("Plots")]
        [DisplayName("Color short")]
        public Color Plot1Color
        {
            get { return _plot1color; }
            set { _plot1color = value; }
        }
        // Serialize Color object
        [Browsable(false)]
        public string Plot1ColorSerialize
        {
            get { return SerializableColor.ToString(_plot1color); }
            set { _plot1color = SerializableColor.FromString(value); }
        }

        /// <summary>
        /// </summary>
        [Description("Line width for short indicator.")]
        [Category("Plots")]
        [DisplayName("Line width short")]
        public int Plot1Width
        {
            get { return _plot1width; }
            set { _plot1width = Math.Max(1, value); }
        }

        /// <summary>
        /// </summary>
        [Description("DashStyle for short indicator.")]
        [Category("Plots")]
        [DisplayName("DashStyle short")]
        public DashStyle Dash1Style
        {
            get { return _plot1dashstyle; }
            set { _plot1dashstyle = value; }
        }

        #endregion


        #region Output

        [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Indicator_Curve_Entry
            {
                get { return Values[0]; }
            }

            [Browsable(false)]
            [XmlIgnore()]
            public DataSeries Indicator_Curve_Exit
            {
                get { return Values[1]; }
            }

            #endregion




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
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
        {
			return Example_Indicator_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Example_Indicator_SMA_CrossOver_Advanced>(input, i => i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled);

			if (indicator != null)
				return indicator;

			indicator = new Example_Indicator_SMA_CrossOver_Advanced
						{
							BarsRequired = BarsRequired,
							CalculateOnBarClose = CalculateOnBarClose,
							Input = input,
							IsLongEnabled = isLongEnabled,
							IsShortEnabled = isShortEnabled
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
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Example_Indicator_SMA_CrossOver_Advanced(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Advanced(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Indicator_SMA_CrossOver_Advanced Example_Indicator_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Indicator_SMA_CrossOver_Advanced(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

}

#endregion
