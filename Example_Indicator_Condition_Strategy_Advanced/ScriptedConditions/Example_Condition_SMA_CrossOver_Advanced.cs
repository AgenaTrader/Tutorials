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
/// Version: 1.1.2
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This scripted condition provides entry and exit signals for a SMA crossover.
/// Long  Signal when SMA20 crosses SMA50 above. Plot is set to  1
/// Short Signal wenn SMA20 crosses SMA50 below. Plot is set to -1
/// You can use this scripted condition also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Basic indicator example for SMA crossover")]
    [IsEntryAttribute(true)]
    [IsStopAttribute(false)]
    [IsTargetAttribute(false)]
    [OverrulePreviousStopPrice(false)]
    public class Example_Condition_SMA_CrossOver_Advanced : UserScriptedCondition
    {

        //input
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;

        //output

        //internal
        private Example_Indicator_SMA_CrossOver_Advanced _Example_Indicator_SMA_CrossOver_Advanced = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

        private int _myvalue = 1;

        protected override void Initialize()
        {
            //Print("Initialize");
            IsEntry = true;
            IsStop = false;
            IsTarget = false;
            Add(new Plot(Color.Orange, "Occurred"));
            Add(new Plot(Color.Gray, "Entry"));

            Overlay = false;
            CalculateOnBarClose = true;

            //Because of Backtesting reasons if we use the advanced mode we need at least two bars
            this.BarsRequired = 2;
        }

        protected override void InitRequirements()
        {
            //Print("InitRequirements");
            base.InitRequirements();
        }


        protected override void OnStartUp()
        {
            //Print("OnStartUp");
            base.OnStartUp();

            //Init our indicator to get code access to the calculate method
            this._Example_Indicator_SMA_CrossOver_Advanced = new Example_Indicator_SMA_CrossOver_Advanced();
        }


        protected override void OnBarUpdate()
        {
            //Check if peridocity is valid for this script
            if (!this._Example_Indicator_SMA_CrossOver_Advanced.DatafeedPeriodicityIsValid(Bars.TimeFrame))
            {
                Log(this.DisplayName + ": Periodicity of your data feed is suboptimal for this indicator!", InfoLogLevel.AlertLog);
                return;
            }

            //Lets call the calculate method and save the result with the trade action
            ResultValue_Example_Indicator_SMA_CrossOver_Advanced returnvalue = this._Example_Indicator_SMA_CrossOver_Advanced.calculate(Bars[0], this.IsLongEnabled, this.IsShortEnabled);

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
                        //Long Signal
                        Occurred.Set(1);
                        //Entry.Set(Close[0]);
                        break;
                    case OrderAction.SellShort:
                        //Short Signal
                        Occurred.Set(-1);
                        //Entry.Set(Close[0]);
                        break;
                }
            }
            else
            {
                //No Signal
                Occurred.Set(0);
                //Entry.Set(Close[0]);
            }

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


        #endregion

        #region Output


        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Occurred
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore()]
        public DataSeries Entry
        {
            get { return Values[1]; }
        }

        public override IList<DataSeries> GetEntries()
        {
            return new[] { Entry };
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
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
        {
			return Example_Condition_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			var indicator = CachedCalculationUnits.GetCachedIndicator<Example_Condition_SMA_CrossOver_Advanced>(input, i => i.IsLongEnabled == isLongEnabled && i.IsShortEnabled == isShortEnabled);

			if (indicator != null)
				return indicator;

			indicator = new Example_Condition_SMA_CrossOver_Advanced
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
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Condition_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			if (InInitialize && input == null)
				throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

			return LeadIndicator.Example_Condition_SMA_CrossOver_Advanced(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Column

	public partial class UserColumn
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Condition_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Condition_SMA_CrossOver_Advanced(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

	#region Scripted Condition

	public partial class UserScriptedCondition
	{
		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Condition_SMA_CrossOver_Advanced(Input, isLongEnabled, isShortEnabled);
		}

		/// <summary>
		/// Basic indicator example for SMA crossover
		/// </summary>
		public Example_Condition_SMA_CrossOver_Advanced Example_Condition_SMA_CrossOver_Advanced(IDataSeries input, System.Boolean isLongEnabled, System.Boolean isShortEnabled)
		{
			return LeadIndicator.Example_Condition_SMA_CrossOver_Advanced(input, isLongEnabled, isShortEnabled);
		}
	}

	#endregion

}

#endregion
