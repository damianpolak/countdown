/* Damian Polak @ Mezc4L
   version: 2.0 @ 2016
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MahApps.Metro.Controls;

using Rect = System.Drawing.Rectangle;
using Screen = System.Windows.Forms.Screen;

namespace CountDown_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private windowFullScreen wfs;
        private DispatcherTimer dtCountDown;
        private DispatcherTimer dtBlinking;
        private DispatcherTimer dtClock;

        private Settings mainSettings;
        private Logs logs;

        private System.Media.SoundPlayer sound;

        // Booleans
        private bool bStart = false;
        private bool bHide = false;
        private bool bMin = true;
        private bool bBlinking = true;
        private bool bClockMode = false;
        private bool bTransparentMode = false;

        private double toptemp, lefttemp;

        // Ścieżka do pliku z ustawieniami
        private string settingsFilePath = "settings.bin";
        private SolidColorBrush timeColor;

        private int Hour = 0, Minute = 15, Second = 0;
        private int clockHour, clockMinute, clockSecond;

        private int BeepSeconds = 5;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            mainSettings = new Settings();
            logs = new Logs();

            
            wfs = new windowFullScreen();
            

            sound = new System.Media.SoundPlayer("beep.wav");
            timeColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 255, 31));

            // DispatcherTimer
            // Odliczanie czasu
            dtCountDown = new DispatcherTimer();
            dtCountDown.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dtCountDown.IsEnabled = true;
            dtCountDown.Stop();
            dtCountDown.Tick += dtCountDown_Tick;

            // DispatcherTimer
            // Ostatnie minuty i ostatnie sekundy - miganie
            dtBlinking = new DispatcherTimer();
            dtBlinking.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dtBlinking.IsEnabled = true;
            dtBlinking.Stop();
            dtBlinking.Tick += dtBlinking_Tick;

            // DispatcherTimer
            // Clock
            dtClock = new DispatcherTimer();
            dtClock.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dtClock.IsEnabled = true;
            dtClock.Tick += DtClock_Tick;
            dtClock.Stop();

            for (int i = 0; i <= 23; i++)
            {
                cbHour.Items.Add(new ComboBoxItem
                {
                    Content = i.ToString("D2")
                });
            }

            for (int i = 0; i <= 59; i++)
            {
                cbMinute.Items.Add(new ComboBoxItem
                {
                    Content = i.ToString("D2")
                });

                cbSecond.Items.Add(new ComboBoxItem
                {
                    Content = i.ToString("D2")
                });
            }

            for (int i = 0; i <= 59; i++)
            {
                cbSecondsBefore.Items.Add(new ComboBoxItem
                {
                    Content = i.ToString("D2")
                });
            }


            cbHour.SelectedIndex = Hour;
            cbMinute.SelectedIndex = Minute;
            cbSecond.SelectedIndex = Second;

            cbSecondsBefore.SelectedIndex = BeepSeconds;

            btStartStop.Background = Brushes.LightGreen;

            UpdateTimeValue();


            ShowHideChangeState();
            MaxMinChangeState();

            // Sprawdzenie istnienia pliku z ustawieniami
            // Jeśli plik istnieje wczytanie zawartości
            if (File.Exists(settingsFilePath))
            {
                mainSettings = LoadSettingsFromFile(settingsFilePath);
                UpdateSettings();

            }

            GetDesktops();
        }

        private void DtClock_Tick(object sender, EventArgs e)
        {
            clockHour = DateTime.Now.Hour;
            clockMinute = DateTime.Now.Minute;
            clockSecond = DateTime.Now.Second;

            lbViewTime.Content = clockHour.ToString("D2") + ":" +
                                clockMinute.ToString("D2") + ":" +
                                clockSecond.ToString("D2");

            wfs.lbMain.Content = lbViewTime.Content;
            this.Title = "CountDown Timer Console " + wfs.lbMain.Content.ToString();
        }

        private void ClockMode()
        {
            if(bClockMode == false)
            {

            } else
            {

            }
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            wfs.Disactive += new EventHandler(WindowFullScreen_Disactive);
        }

        private void WindowFullScreen_Disactive(object sender, EventArgs e)
        {
            cobDesktops.SelectedItem = null;
            
        }

        private void dtBlinking_Tick(object sender, EventArgs e)
        {
            if (bBlinking == true)
            {
                bBlinking = false;
                wfs.lbMain.Foreground = Brushes.Red;
            }
            else
            {
                bBlinking = true;
                wfs.lbMain.Foreground = timeColor;
            }
        }

        private void GetDesktops()
        {
            cobDesktops.Items.Clear();
            Screen[] screens = Screen.AllScreens;
            int i = 0;
            try
            {
                foreach (Screen item in screens)
                {

                    cobDesktops.Items.Add("Desktop " + i + " [" + item.WorkingArea.Width + "x" + item.WorkingArea.Height + "] " + (item.Primary == true ? "Primary" : "Secondary"));
                    cobDesktops.SelectedIndex = 0;
                    i++;
                }
            } catch (Exception ex)
            {
                logs.Message(ex.Message);
            }

        }

        private int UpdateSettings()
        {
            try
            {
                this.Top = mainSettings.consoleTop;
                this.Left = mainSettings.consoleLeft;

                //bHide = mainSettings.mainHide;
                //bMin = mainSettings.mainMin;

                wfs.Top = mainSettings.mainTop;
                wfs.Left = mainSettings.mainLeft;
                wfs.Width = mainSettings.mainWidth;
                wfs.Height = mainSettings.mainHeight;

                if (mainSettings.mainResizeMode == true)
                {
                    wfs.ResizeMode = ResizeMode.NoResize;
                    wfs.ChangeResizeMode();
                }
                else
                {
                    wfs.ResizeMode = ResizeMode.CanResize;
                    wfs.ChangeResizeMode();
                }
                
                Hour = mainSettings.Hour;
                Minute = mainSettings.Minute;
                Second = mainSettings.Second;
                cbHour.SelectedIndex = Hour;
                cbMinute.SelectedIndex = Minute;
                cbSecond.SelectedIndex = Second;

                UpdateTimeValue();

                if (mainSettings.Sound == true)
                {
                    cbSound.IsChecked = true;
                    this.cbSecondsBefore.IsEnabled = true;
                }
                else
                {
                    cbSound.IsChecked = false;
                    this.cbSecondsBefore.IsEnabled = false;
                }

                /*
                if(mainSettings.Transparent == true)
                {
                    cbTransparentMode.IsChecked = true;
                    wfs.AllowsTransparency = true;
                    wfs.Background = Brushes.Transparent;
                } else
                {
                    cbTransparentMode.IsChecked = false;
                    wfs.AllowsTransparency = false;
                    wfs.Background = Brushes.Black;

                }*/

                BeepSeconds = mainSettings.BeepSeconds;
                cbSecondsBefore.SelectedIndex = BeepSeconds;

                return 0;
            }
            catch (Exception e)
            {
                logs.Message(e.Message);
                return 1;
            }
        }

        private void SaveSettingsToFile(string filePath)
        {

            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, mainSettings);
                    
                }
            }
            catch (IOException e)
            {
                //MessageBox.Show(e.ToString(), "SaveSettingsToFile()");
                logs.Message(e.Message);
            }
        }

        private Settings LoadSettingsFromFile(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    Settings st = (Settings)bin.Deserialize(stream);
                    return st;
                }
            }
            catch (IOException e)
            {
                logs.Message(e.Message);
                return null;
            }
        }
        private void dtCountDown_Tick(object sender, EventArgs e)
        {
            if (bStart == true)
            {
                // Odliczanie
                if (Hour != 0 || Minute != 0 || Second != 0)
                {

                    Second = Second - 1;
                    if (Second == -1)
                    {
                        Second = 59;
                        Minute = Minute - 1;
                    }
                    if (Minute == -1)
                    {
                        Minute = 59;
                        Hour = Hour - 1;
                    }

                    // Dźwięk on/off
                    if (BeepSeconds != 0 && cbSound.IsChecked == true)
                    {
                        if (Hour == 0 && Minute == 0 && Second <= BeepSeconds)
                        {
                            try
                            {
                                sound.Play();
                            }
                            catch (Exception ex)
                            {
                                logs.Message(ex.Message);

                            }



                        }
                    }

                    // Ostatnie 30 sekund zmiana kolorow
                    if (Hour == 0 && Minute == 0 && Second <= int.Parse(cbSecondsBefore.Text.ToString()))
                    {
                        dtBlinking.Start();
                    }

                    cbClockMode.IsEnabled = false;
                }
                else // W przypadku gdy licznik osiągnie zero
                {
                    cbClockMode.IsEnabled = true;
                    StartStopChangeState();
                    dtBlinking.Stop();

                }

                UpdateTimeValue();
            }

        }

        private void SetTimeValue()
        {
            Hour = cbHour.SelectedIndex;
            Minute = cbMinute.SelectedIndex;
            Second = cbSecond.SelectedIndex;
        }

        private void UpdateTimeValue()
        {

            lbViewTime.Content = Hour.ToString("D2") + ":" +
                                Minute.ToString("D2") + ":" +
                                Second.ToString("D2");

            wfs.lbMain.Content = lbViewTime.Content;
            this.Title = "CountDown Timer Console " + wfs.lbMain.Content.ToString();


        }

        private void StartStopChangeState()
        {
            if (bStart == true)
            {
                bStart = false;
                btStartStop.Background = Brushes.LightGreen;
                btStartStop.Content = "START";

                dtCountDown.Stop();
                dtBlinking.Stop();
                dtBlinking.IsEnabled = false;
                wfs.lbMain.Foreground = timeColor;
                cbClockMode.IsEnabled = true;
            }
            else
            {
                bStart = true;
                btStartStop.Background = Brushes.LightCoral;
                btStartStop.Content = "STOP";

                dtCountDown.Start();
                cbClockMode.IsEnabled = false;
            }
        }

        private void ClockModeChangeState()
        {
            if (bClockMode == true)
            {
                btSet.IsEnabled = false;
                btStartStop.IsEnabled = false;
                cbSecond.IsEnabled = false;
                cbMinute.IsEnabled = false;
                cbHour.IsEnabled = false;
                cbSecond.IsEnabled = false;
                cbSecondsBefore.IsEnabled = false;
                cbSound.IsEnabled = false;

                dtClock.Start();

            }
            else
            {
                btSet.IsEnabled = true;
                btStartStop.IsEnabled = true;
                cbSecond.IsEnabled = true;
                cbMinute.IsEnabled = true;
                cbHour.IsEnabled = true;
                cbSecond.IsEnabled = true;
                cbSecondsBefore.IsEnabled = true;
                cbSound.IsEnabled = false;

                dtClock.Stop();
                UpdateTimeValue();
            }
        }
        private void ShowHideChangeState()
        {
            if (bHide == false)
            {
                bHide = true;
                btShowHide.Content = "HIDE";

                wfs.Show();
            }
            else
            {
                bHide = false;
                btShowHide.Content = "SHOW";

                wfs.Hide();
            }
        }

        private void MaxMinChangeState()
        {
            
            if (bMin == false)
            {
                bMin = true;
                btMinMax.Content = "RESTORE";

                //wfs.WindowState = WindowState.Maximized;
                if(cobDesktops.SelectedItem != null)
                {
                    Screen s2 = Screen.AllScreens[cobDesktops.SelectedIndex];
                    Rect r2 = s2.WorkingArea;

                    toptemp = wfs.Top;
                    lefttemp = wfs.Left;
                    wfs.Top = r2.Top;
                    wfs.Left = r2.Left;
                }

                wfs.WindowState = WindowState.Maximized;
            }
            else
            {
                bMin = false;
                btMinMax.Content = "MAXIMIZE";

                wfs.WindowState = WindowState.Normal;
                if (cobDesktops.SelectedItem != null)
                {
                    wfs.Top = toptemp;
                    wfs.Left = lefttemp;
                }

            }
        }

        private void btStartStop_Click(object sender, RoutedEventArgs e)
        {
            StartStopChangeState();
        }

        private void btShowHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideChangeState();
        }

        private void btMinMax_Click(object sender, RoutedEventArgs e)
        {
            MaxMinChangeState();
        }

        private void btSet_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Are you sure?",
                                "",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bStart = true;
                StartStopChangeState();

                SetTimeValue();
                UpdateTimeValue();
            }

        }

        // Zdarzenie podczas zamykania tego okna aplikacji
        private void windowConsole_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainSettings.consoleTop = this.Top;
            mainSettings.consoleLeft = this.Left;

            mainSettings.mainTop = wfs.Top;
            mainSettings.mainLeft = wfs.Left;
            mainSettings.mainWidth = wfs.Width;
            mainSettings.mainHeight = wfs.Height;

            if (wfs.ResizeMode == ResizeMode.NoResize)
            {
                mainSettings.mainResizeMode = false;
            }
            else if (wfs.ResizeMode == ResizeMode.CanResize)
            {
                mainSettings.mainResizeMode = true;
            }

            mainSettings.Hour = cbHour.SelectedIndex;
            mainSettings.Minute = cbMinute.SelectedIndex;
            mainSettings.Second = cbSecond.SelectedIndex;

            if (cbSound.IsChecked == true)
            {
                mainSettings.Sound = true;
            }
            else
            {
                mainSettings.Sound = false;
            }

            mainSettings.BeepSeconds = cbSecondsBefore.SelectedIndex;
            mainSettings.Transparent = bTransparentMode;

            SaveSettingsToFile(settingsFilePath);
         // Wymuszenie zamknięcia użyte z powodu okna "fullScreenWindow"
         // Okno, którego uchwyt nigdy nie zostaje zniszczony
         // Uchwyt jest tworzony raz, a okno podczas zamknięcia X,
         // zostaje jedynie ukryte
         //windowFullScreen
            dtCountDown.Stop();
            dtBlinking.Stop();
            wfs.Close();
            Application.Current.Shutdown();
            System.Environment.Exit(777);

        }

        private void cbTransparentMode_Click(object sender, RoutedEventArgs e)
        {
            if (cbTransparentMode.IsChecked == true)
            {
                bTransparentMode = true;
                //wfs.lbMain.Background = Brushes.Transparent;
            }
            else
            {
                bTransparentMode = false;
                //wfs.lbMain.Background = Brushes.Black;
            }
        }

        private void cbSecondsBefore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BeepSeconds = cbSecondsBefore.SelectedIndex;
        }

        private void cbClockMode_Click(object sender, RoutedEventArgs e)
        {
            if (cbClockMode.IsChecked == true)
            {
                bClockMode = true;
                ClockModeChangeState();
                
            }
            else
            {
                bClockMode = false;
                ClockModeChangeState();
            }
        }

        private void cobDesktops_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GetDesktops();
        }

        private void cbSound_Click(object sender, RoutedEventArgs e)
        {
            if (cbSound.IsChecked == true)
            {
                cbSecondsBefore.IsEnabled = true;
            }
            else
            {
                cbSecondsBefore.IsEnabled = false;
            }
        }

        private void btload_Click(object sender, RoutedEventArgs e)
        {
            mainSettings = LoadSettingsFromFile(settingsFilePath);
            UpdateSettings();
            cbHour.SelectedIndex = Hour;
            cbMinute.SelectedIndex = Minute;
            cbSecond.SelectedIndex = Second;
        }

    }
}
