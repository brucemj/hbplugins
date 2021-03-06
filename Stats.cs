﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Threading;
using log4net;
using Triton.Bot;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Game;
using Triton.Bot.Logic.Bots.DefaultBot;
using Logger = Triton.Common.LogUtilities.Logger;

namespace Stats
{
    public class Stats : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private bool _enabled;
        
        private bool _findNewQuest = true;
		private bool _tickstop = false;
	
		private bool _isNewgameing = false;
		private bool _isStartingNewGame = false;
		private int _numtick = 0;
		private int _numStartingNewGame = 0;
		private bool _isUnlocking = false;
		private Triton.Game.Mapping.TAG_CLASS unlockDeck = TritonHs.BasicHeroTagClasses[1];

		//private var _rngDeck1 = TritonHs.BasicHeroTagClasses[2];
		//private var _rngDeck2 = TritonHs.BasicHeroTagClasses[2];

        #region Implementation of IPlugin

        /// <summary> The name of the plugin. </summary>
        public string Name
        {
            get { return "Stats"; }
        }

        /// <summary> The description of the plugin. </summary>
        public string Description
        {
            get { return "A plugin that provides basic stats for Hearthbuddy."; }
        }

        /// <summary>The author of the plugin.</summary>
        public string Author
        {
            get { return "Bossland GmbH"; }
        }

        /// <summary>The version of the plugin.</summary>
        public string Version
        {
            get { return "0.0.1.1"; }
        }

        /// <summary>Initializes this object. This is called when the object is loaded into the bot.</summary>
        public void Initialize()
        {
            Log.DebugFormat("[Stats] Initialize");
            TritonHs.OnGuiTick += TritonHsOnOnGuiTick;
        }

        /// <summary> The plugin start callback. Do any initialization here. </summary>
        public void Start()
        {
            Log.DebugFormat("[Stats] Start");
            
            GameEventManager.StartingNewGame += GameEventManagerOnStartingNewGame;
            GameEventManager.QuestUpdate += GameEventManagerOnQuestUpdate;
            GameEventManager.GameOver += GameEventManagerOnGameOver;
			GameEventManager.NewGame += GameEventManagerOnNewGame ;
			_tickstop = false ;
        }
		
		public void GameEventManagerOnNewGame(object sender, NewGameEventArgs newGameEventArgs)
		{
			DateTime baseTime = Convert.ToDateTime("1970-1-1 8:00:00");
			TimeSpan ts = DateTime.Now - baseTime;
			long intervel = (long)ts.TotalSeconds;
			StatsSettings.Instance.Newtime = intervel;
			UpdateMainGuiStats();
			_isNewgameing = true;
			_isStartingNewGame = false;
			_numStartingNewGame = 0;
			Log.DebugFormat("[Stats] GameEventManagerOnNewGame");
		}

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The plugin stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            Log.DebugFormat("[Stats] Stop");
			_tickstop = true ;
			
            GameEventManager.StartingNewGame -= GameEventManagerOnStartingNewGame;
            GameEventManager.QuestUpdate -= GameEventManagerOnQuestUpdate;
            GameEventManager.GameOver -= GameEventManagerOnGameOver;
			GameEventManager.NewGame -= GameEventManagerOnNewGame ;
			_isNewgameing = false;
			//TritonHs.OnGuiTick -= TritonHsOnOnGuiTick;
        }

        public JsonSettings Settings
        {
            get { return StatsSettings.Instance; }
        }

        private UserControl _control;

        /// <summary> The plugin's settings control. This will be added to the Hearthbuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                if (_control != null)
                {
                    return _control;
                }

                using (var fs = new FileStream(@"Plugins\Stats\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl) XamlReader.Load(fs);

                    // Your settings binding here.

                    if (!Wpf.SetupTextBoxBinding(root, "WinsTextBox", "Wins",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'WinsTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (!Wpf.SetupTextBoxBinding(root, "LossesTextBox", "Losses",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'LossesTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (!Wpf.SetupTextBoxBinding(root, "ConcedesTextBox", "Concedes",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'ConcedesTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (!Wpf.SetupTextBoxBinding(root, "QuestsTextBox", "Quests",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'QuestsTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
					
					if (!Wpf.SetupTextBoxBinding(root, "NewtimeTextBox", "Newtime",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'NewtimeTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
					
					if (!Wpf.SetupTextBoxBinding(root, "TicktimeTextBox", "Ticktime",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'TicktimeTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
					
					if (!Wpf.SetupTextBoxBinding(root, "DWinsTextBox", "DWins",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'DWinsTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
					
					if (!Wpf.SetupTextBoxBinding(root, "DLossesTextBox", "DLosses",
                        BindingMode.TwoWay, StatsSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'DLossesTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }


                    // Your settings event handlers here.

                    var resetButton = Wpf.FindControlByName<Button>(root, "ResetButton");
                    resetButton.Click += ResetButtonOnClick;

                    var addWinButton = Wpf.FindControlByName<Button>(root, "AddWinButton");
                    addWinButton.Click += AddWinButtonOnClick;

                    var addLossButton = Wpf.FindControlByName<Button>(root, "AddLossButton");
                    addLossButton.Click += AddLossButtonOnClick;

                    var addConcedeButton = Wpf.FindControlByName<Button>(root, "AddConcedeButton");
                    addConcedeButton.Click += AddConcedeButtonOnClick;

                    var removeWinButton = Wpf.FindControlByName<Button>(root, "RemoveWinButton");
                    removeWinButton.Click += RemoveWinButtonOnClick;

                    var removeLossButton = Wpf.FindControlByName<Button>(root, "RemoveLossButton");
                    removeLossButton.Click += RemoveLossButtonOnClick;

                    var removeConcedeButton = Wpf.FindControlByName<Button>(root, "RemoveConcedeButton");
                    removeConcedeButton.Click += RemoveConcedeButtonOnClick;

                    UpdateMainGuiStats();

                    _control = root;
                }

                return _control;
            }
        }

        /// <summary>Is this plugin currently enabled?</summary>
        public bool IsEnabled
        {
            get { return _enabled; }
        }

        /// <summary> The plugin is being enabled.</summary>
        public void Enable()
        {
            Log.DebugFormat("[Stats] Enable");
            _enabled = true;
            
			_isUnlocking = false;
            Log.DebugFormat("Hello this is custom Stats! --------");
			NormalPlayConf();
			DateTime dtDateTime = Convert.ToDateTime("1970-1-1 00:00:00");
			dtDateTime = dtDateTime.AddSeconds( StatsSettings.Instance.Newtime ).ToLocalTime();
			// 在新的一天，清0 DWins 和 DLosses
			if( DateTime.Now.DayOfYear != dtDateTime.DayOfYear ){
				StatsSettings.Instance.DWins = 0;
				StatsSettings.Instance.DLosses = 0;
			}
			
			DateTime baseTime = Convert.ToDateTime("1970-1-1 8:00:00");
			
			TimeSpan ts = DateTime.Now - baseTime;
			long intervel = (long)ts.TotalSeconds;
			StatsSettings.Instance.Newtime = intervel;
			StatsSettings.Instance.Ticktime = intervel;
			//UpdateMainGuiStats();
            BotManager.Start();
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            Log.DebugFormat("[Stats] Disable");
            _enabled = false;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>Deinitializes this object. This is called when the object is being unloaded from the bot.</summary>
        public void Deinitialize()
        {
            TritonHs.OnGuiTick -= TritonHsOnOnGuiTick;
        }

        #endregion

        #region Override of Object

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        #endregion

        private void GameEventManagerOnGameOver(object sender, GameOverEventArgs gameOverEventArgs)
        {
			_isNewgameing = false;
			_isStartingNewGame = false;
			NormalPlayConf();
			if( _isUnlocking ){
				StatsSettings.Instance.Concedes++;
			}
            StatsSettings.Instance.Quests = TritonHs.CurrentQuests.Count ;
            if (gameOverEventArgs.Result == GameOverFlag.Victory)
            {
				if( !_isUnlocking ){
					StatsSettings.Instance.Wins++;
					StatsSettings.Instance.DWins++;
				}
				_isUnlocking = false;
                UpdateMainGuiStats();
            }
            else if (gameOverEventArgs.Result == GameOverFlag.Defeat)
            {
                if (gameOverEventArgs.Conceded)
                {
                    StatsSettings.Instance.Concedes++;
                }
                else
                {
					if(!_isUnlocking){
						StatsSettings.Instance.Losses++;
						StatsSettings.Instance.DLosses++;
					}
                }
                UpdateMainGuiStats();
            }
        }

        private void ResetButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            StatsSettings.Instance.Wins = 0;
            StatsSettings.Instance.Losses = 0;
            StatsSettings.Instance.Concedes = 0;
            UpdateMainGuiStats();
        }

        private void AddWinButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            StatsSettings.Instance.Wins++;
            UpdateMainGuiStats();
        }

        private void AddLossButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            StatsSettings.Instance.Losses++;
            UpdateMainGuiStats();
        }

        private void AddConcedeButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            StatsSettings.Instance.Concedes++;
            UpdateMainGuiStats();
        }

        private void RemoveWinButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (StatsSettings.Instance.Wins > 0)
            {
                StatsSettings.Instance.Wins--;
                UpdateMainGuiStats();
            }
        }

        private void RemoveLossButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (StatsSettings.Instance.Losses > 0)
            {
                StatsSettings.Instance.Losses--;
                UpdateMainGuiStats();
            }
        }

        private void RemoveConcedeButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (StatsSettings.Instance.Concedes > 0)
            {
                StatsSettings.Instance.Concedes--;
                UpdateMainGuiStats();
            }
        }

        private void UpdateMainGuiStats()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var leftControl = Wpf.FindControlByName<Label>(Application.Current.MainWindow, "StatusBarLeftLabel");
                var rightControl = Wpf.FindControlByName<Label>(Application.Current.MainWindow, "StatusBarRightLabel");

                if (StatsSettings.Instance.Wins + StatsSettings.Instance.Losses == 0)
                {
                    if (StatsSettings.Instance.Concedes == 0)
                    {
                        rightControl.Content = string.Format("(No Games)");
                    }
                    else
                    {
                        rightControl.Content = string.Format("[{0} concedes]", StatsSettings.Instance.Concedes);
                    }
                }
                else
                {
                    rightControl.Content = string.Format("{0} / {1} ({2:0.00} %) [{3} concedes]",
                        StatsSettings.Instance.Wins,
                        StatsSettings.Instance.Wins + StatsSettings.Instance.Losses,
                        100.0f*(float) StatsSettings.Instance.Wins/
                        (float) (StatsSettings.Instance.Wins + StatsSettings.Instance.Losses),
                        StatsSettings.Instance.Concedes);

                    Log.InfoFormat("[Stats] Summary: {0}", rightControl.Content);
                }

                // Force a save all.
                Configuration.Instance.SaveAll();
            }));
        }

        private void TritonHsOnOnGuiTick(object sender, GuiTickEventArgs guiTickEventArgs)
        {
            // Only update if we're actually enabled.
            if (IsEnabled)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var leftControl = Wpf.FindControlByName<Label>(Application.Current.MainWindow, "StatusBarLeftLabel");
                    leftControl.Content = string.Format("Runtime: {0}", TritonHs.Runtime.Elapsed.ToString("h'h 'm'm 's's'"));
                }));
				if(!_isNewgameing && _isStartingNewGame)
				{
					_numStartingNewGame++ ;
					if ( _numStartingNewGame >= 75)
					{
						Log.DebugFormat("StartingNewGame is hang 50s , so Unlock this Hero");
						_numStartingNewGame = 0 ;
						_isUnlocking = true;
						UnlockHeroConf();
					}
				}
				if(_tickstop){
					Log.InfoFormat("The stats plugin is stop, Ticktime not update");
				}else{
					if(_numtick >=60){
						DateTime baseTime = Convert.ToDateTime("1970-1-1 8:00:00");
						TimeSpan ts = DateTime.Now - baseTime;
						long intervel = (long)ts.TotalSeconds;
						StatsSettings.Instance.Ticktime = intervel;
						_numtick = 0 ;
						UpdateMainGuiStats();
					}else{
						_numtick ++ ;
					}
				}
            }	
        }
		
		private void UnlockHeroConf()
		{
			Log.DebugFormat("Use UnlockHeroConf ......");
			DefaultBotSettings.Instance.GameMode = GameMode.Practice	;
            DefaultBotSettings.Instance.AutoGreet = false	;
			DefaultBotSettings.Instance.NeedsToCacheQuests = false;
            DefaultBotSettings.Instance.NeedsToCacheCustomDecks = false;
			
            DefaultBotSettings.Instance.PracticeDifficulty = PracticeDifficulty.Normal;
            DefaultBotSettings.Instance.PracticeDeckType = DeckType.Basic;
			
			// 0-DRUID; 1-HUNTER; 2-MAGE; 3-PALADIN; 4-PRIEST;
            // 5-ROGUE; 6-SHAMAN; 7-WARLOCK; 8-WARRIOR; 
			
			var rngDeck = TritonHs.BasicHeroTagClasses[2];
			DefaultBotSettings.Instance.PracticeBasicDeck = rngDeck;
			
			//var opponent = TritonHs.BasicHeroTagClasses[8];
			DefaultBotSettings.Instance.PracticeOpponentClass = unlockDeck;
		}
		
		private void NormalPlayConf()
		{
			Log.DebugFormat("Use NormalPlayConf ......");
			DefaultBotSettings.Instance.GameMode = GameMode.Constructed	;
            DefaultBotSettings.Instance.AutoGreet = false	;
            DefaultBotSettings.Instance.ConstructedMode = ConstructedMode.Casual;  
            DefaultBotSettings.Instance.ConstructedDeckType = DeckType.Basic;
            // DefaultBotSettings.Instance.ConstructedCustomDeck = "123";
            DefaultBotSettings.Instance.NeedsToCacheQuests = false;
            DefaultBotSettings.Instance.NeedsToCacheCustomDecks = false;
            //var rngDeck = TritonHs.BasicHeroTagClasses[Client.Random.Next(0, TritonHs.BasicHeroTagClasses.Length)];
            var rngDeck = TritonHs.BasicHeroTagClasses[2];
            // 0-DRUID; 1-HUNTER; 2-MAGE; 3-PALADIN; 4-PRIEST;
            // 5-ROGUE; 6-SHAMAN; 7-WARLOCK; 8-WARRIOR; 
            DefaultBotSettings.Instance.ConstructedBasicDeck = rngDeck;
            //StatsSettings.Instance.Quests = TritonHs.CurrentQuests.Count ;
			//StatsSettings.Instance.Quests = 3 ;
            //UpdateMainGuiStats();
        }
		
        private void GameEventManagerOnStartingNewGame(object sender, StartingNewGameEventArgs startingNewGameEventArgs)
        {
			_isStartingNewGame = true;
			Log.DebugFormat("GameEventManagerOnStartingNewGame");

            if (!_isUnlocking && _findNewQuest)
            {
                var foundQuest = false;
                var quests = TritonHs.CurrentQuests;
				StatsSettings.Instance.Quests = TritonHs.CurrentQuests.Count ;
                
                // Choose a basic deck to complete the quests.
                foreach (var quest in quests)
                {
                    // Loop through for each each class.
                    foreach (var @class in TritonHs.BasicHeroTagClasses)
                    {
                        // If this is a class specific quest, find a suitable deck. Otherwise,
                        // just use a random custom deck.
                        if (TritonHs.IsQuestForSpecificClass(quest.Id))
                        {
                            // If this quest is a win quest for this class.
							
								if (TritonHs.IsQuestForClass(quest.Id, @class))
								{
									Log.InfoFormat(
										"[Quest] Now choosing the basic hero class to complete 要求职业的任务： [{0}] ,任务ID: [{1}] with.",
										quest.Name, quest.Id);

									DefaultBotSettings.Instance.ConstructedDeckType = DeckType.Basic;
									DefaultBotSettings.Instance.ConstructedBasicDeck = @class;
									unlockDeck = @class;
									foundQuest = true;
									break;
								}
								
                        }
                    }

                    // If we found a quest and changed bot settings, we're done.
                    if (foundQuest)
                        break;

                    // Make sure we have a non-class quest to choose a random deck for.
                    if (!TritonHs.IsQuestForSpecificClass(quest.Id)) //不要求职业的任务
                    {
                        var decks = TritonHs.BasicHeroTagClasses;

                        // //Choose a random deck. We can add more logic for selection later...
                        var deck = decks[Client.Random.Next(0, decks.Length)];

                         Log.InfoFormat("[Quest] 预留功能，开发卡牌组,不要求职业的任务： [{0}] ,任务ID: [{1}]", quest.Name, quest.Id);

                        DefaultBotSettings.Instance.ConstructedDeckType = DeckType.Basic;
                        DefaultBotSettings.Instance.ConstructedBasicDeck = deck;

                        foundQuest = true;
                        break;
                    }
                }
            }
        }
 
        private void GameEventManagerOnQuestUpdate(object sender, QuestUpdateEventArgs questUpdateEventArgs)
        {
            _findNewQuest = true;
        }
    }
}
