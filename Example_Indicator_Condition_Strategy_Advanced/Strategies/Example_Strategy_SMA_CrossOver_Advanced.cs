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
/// Version: 1.1.3
/// -------------------------------------------------------------------------
/// Simon Pucher 2016
/// Christian Kovar 2016
/// -------------------------------------------------------------------------
/// This strategy provides provides entry and exit signals for a SMA crossover.
/// Long  Signal when SMA20 crosses SMA50 above. Plot is set to  1
/// Short Signal wenn SMA20 crosses SMA50 below. Plot is set to -1
/// StopLoss and Target is set for 1%
/// You can use this strategy also as a template for further script development.
/// -------------------------------------------------------------------------
/// Namespace holds all indicators and is required. Do not change it.
/// </summary>
namespace AgenaTrader.UserCode
{
    [Description("Basic indicator example for SMA crossover")]
    public class Example_Strategy_SMA_CrossOver_Advanced : UserStrategy
    {

        //input
        private bool _autopilot = true;
        private bool _IsLongEnabled = true;
        private bool _IsShortEnabled = true;

        //output

        //internal
        private Example_Indicator_SMA_CrossOver_Advanced _Example_Indicator_SMA_CrossOver_Advanced = null;
        private IOrder _orderenterlong;
        private IOrder _orderentershort;

		protected override void Initialize()
		{
            CalculateOnBarClose = true;

            //Set the default time frame if you start the strategy via the strategy-escort
            //if you start the strategy on a chart the TimeFrame is automatically set, this will lead to a better usability
            if (this.TimeFrame == null || this.TimeFrame.PeriodicityValue == 0)
            {
                this.TimeFrame = new TimeFrame(DatafeedHistoryPeriodicity.Day, 1);
            }

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
            base.OnStartUp();

            //Init our indicator to get code access to the calculate method
            this._Example_Indicator_SMA_CrossOver_Advanced = new Example_Indicator_SMA_CrossOver_Advanced();

        }

		protected override void OnBarUpdate()
		{
            //Set Autopilot
            this.IsAutomated = this.Autopilot;

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
                        this.DoEnterLong();
                        break;
                    case OrderAction.SellShort:
                        this.DoEnterShort();
                        break;
                }
            }

            //Exit
            if (returnvalue.Exit.HasValue)
            {
                switch (returnvalue.Exit)
                {
                    case OrderAction.BuyToCover:
                        this.DoExitShort();
                        break;
                    case OrderAction.Sell:
                        this.DoExitLong();
                        break;
                }
            }
		}


        /// <summary>
        /// Create LONG order.
        /// </summary>
        private void DoEnterLong()
        {
            if (_orderenterlong == null)
            {
                _orderenterlong = EnterLong(this.DefaultQuantity, this.DisplayName + "_" + OrderAction.Buy + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                //SetStopLoss(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.RangeLow, false);
                //SetProfitTarget(_orderenterlong.Name, CalculationMode.Price, this._orb_indicator.TargetLong); 
            }
        }

        /// <summary>
        /// Create SHORT order.
        /// </summary>
        private void DoEnterShort()
        {
            if (_orderentershort == null)
            {
                _orderentershort = EnterShort(this.DefaultQuantity, this.DisplayName + "_" + OrderAction.SellShort + "_" + this.Instrument.Symbol + "_" + Bars[0].Time.Ticks.ToString(), this.Instrument, this.TimeFrame);
                //SetStopLoss(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.RangeHigh, false);
                //SetProfitTarget(_orderentershort.Name, CalculationMode.Price, this._orb_indicator.TargetShort);
            }
        }

        /// <summary>
        /// Exit the LONG position.
        /// </summary>
        private void DoExitLong()
        {
            if (_orderenterlong != null)
            {
                 ExitLong(this._orderenterlong.Name);
                 this._orderenterlong = null;
            }
        }

        /// <summary>
        /// Fill the SHORT position.
        /// </summary>
        private void DoExitShort()
        {
            if (_orderentershort != null)
            {
                ExitShort(this._orderentershort.Name);
                this._orderentershort = null;
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

        [Description("If true the strategy will handle everything. It will create buy orders, sell orders, stop loss orders, targets fully automatically")]
        [Category("Safety first!")]
        [DisplayName("Autopilot")]
        public bool Autopilot
        {
            get { return _autopilot; }
            set { _autopilot = value; }
        }

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

        #endregion

    }
}