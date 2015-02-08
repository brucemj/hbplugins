﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using log4net;
using Newtonsoft.Json;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Game.Mapping;
using Logger = Triton.Common.LogUtilities.Logger;

namespace Stats
{
    /// <summary>Settings for the Stats plugin. </summary>
    public class StatsSettings : JsonSettings
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static StatsSettings _instance;

        /// <summary>The current instance for this class. </summary>
        public static StatsSettings Instance
        {
            get { return _instance ?? (_instance = new StatsSettings()); }
        }

        /// <summary>The default ctor. Will use the settings path "Stats".</summary>
        public StatsSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "Stats")))
        {
        }

        private int _wins;
        private int _losses;
        private int _concedes;
        private int _quests;
		private long _newtime;
		private long _ticktime;
		private int _dwins;
		private int _dlosses;

        /// <summary>Current stored wins.</summary>
        [DefaultValue(0)]
        public int Wins
        {
            get { return _wins; }
            set
            {
                if (value.Equals(_wins))
                {
                    return;
                }
                _wins = value;
                NotifyPropertyChanged(() => Wins);
            }
        }

        /// <summary>Current stored losses.</summary>
        [DefaultValue(0)]
        public int Losses
        {
            get { return _losses; }
            set
            {
                if (value.Equals(_losses))
                {
                    return;
                }
                _losses = value;
                NotifyPropertyChanged(() => Losses);
            }
        }

        /// <summary>Current stored concedes.</summary>
        [DefaultValue(0)]
        public int Concedes
        {
            get { return _concedes; }
            set
            {
                if (value.Equals(_concedes))
                {
                    return;
                }
                _concedes = value;
                NotifyPropertyChanged(() => Concedes);
            }
        }

        /// <summary>Current quests concedes.</summary>
        [DefaultValue(3)]
        public int Quests
        {
            get { return _quests; }
            set
            {
                if (value.Equals(_quests))
                {
                    return;
                }
                _quests = value;
                NotifyPropertyChanged(() => Quests);
            }
        }
		
		/// <summary>new game start time .</summary>
        [DefaultValue(0)]
        public long Newtime
        {
            get { return _newtime; }
            set
            {
                if (value.Equals(_newtime))
                {
                    return;
                }
                _newtime = value;
                NotifyPropertyChanged(() => Newtime);
            }
        }
		
		/// <summary>new game start time .</summary>
        [DefaultValue(0)]
        public long Ticktime
        {
            get { return _ticktime; }
            set
            {
                if (value.Equals(_ticktime))
                {
                    return;
                }
                _ticktime = value;
                NotifyPropertyChanged(() => Ticktime);
            }
        }
		
		/// <summary>wins day .</summary>
        [DefaultValue(0)]
        public int DWins
        {
            get { return _dwins; }
            set
            {
                if (value.Equals(_dwins))
                {
                    return;
                }
                _dwins = value;
                NotifyPropertyChanged(() => DWins);
            }
        }
		
		/// <summary>losses day .</summary>
        [DefaultValue(0)]
        public int DLosses
        {
            get { return _dlosses; }
            set
            {
                if (value.Equals(_dlosses))
                {
                    return;
                }
                _dlosses = value;
                NotifyPropertyChanged(() => DLosses);
            }
        }
		
    }
}
