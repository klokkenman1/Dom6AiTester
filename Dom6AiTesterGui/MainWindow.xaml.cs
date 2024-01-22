using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Dom6AiTesterGui.Properties;
using Microsoft.Win32;
using System.Linq;

namespace Dom6AiTesterGui
{
    public class Data : INotifyPropertyChanged
    {
        private string _dom6ExeLocation = Settings.Default.Dom6ExeLocation;
        // Will create a folder called Game in %appdata%/Dominions6/savedgames
        private string _gameName = "Game";
        // Make sure the map has a UW place for the omni to spawn and spawns for the AIs you want
        private string _map = Settings.Default.Map;
        private string _aiMod = Settings.Default.AiMod;

        public string Dom6ExeLocation
        {
            get => _dom6ExeLocation;
            set
            {
                if (_dom6ExeLocation != value)
                {
                    _dom6ExeLocation = value;
                    Settings.Default.Dom6ExeLocation = _dom6ExeLocation;
                    Settings.Default.Save();
                    NotifyPropertyChanged(nameof(Dom6ExeLocation));
                }
            }
        }

        public string GameName
        {
            get => _gameName;
            set
            {
                if (_gameName != value)
                {
                    _gameName = value;
                    NotifyPropertyChanged(nameof(GameName));
                }
            }
        }

        public string Map
        {
            get => _map;
            set
            {
                if (_map != value)
                {
                    _map = value;
                    Settings.Default.Map = _map;
                    Settings.Default.Save();
                    NotifyPropertyChanged(nameof(Map));
                }
            }
        }

        public string AiMod
        {
            get => _aiMod;
            set
            {
                if (_aiMod != value)
                {
                    _aiMod = value;
                    Settings.Default.AiMod = _aiMod;
                    Settings.Default.Save();
                    NotifyPropertyChanged(nameof(AiMod));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data _data = new Data();

        private Dictionary<string, int> _nationNameToNbrMap = new Dictionary<string, int>()
        {
            {"Arcoscephale", 95},
            {"Phlegra", 96},
            {"Pangaea", 97},
            {"Pythium", 98},
            {"Lemuria", 99},
            {"Man", 100},
            {"Ulm", 101},
            {"Agartha", 102},
            {"Marignon", 103},
            {"Abysia", 104},
            {"Ragha", 105},
            {"Caelum", 106},
            {"Gath", 107},
            {"Patala", 108},
            {"T'ien Ch'i", 109},
            {"Jomon", 110},
            {"Mictlan", 111},
            {"Xibalba", 112},
            {"C'tis", 113},
            {"Midgård", 115},
            {"Bogarus", 116},
            {"Utgård", 117},
            {"Vaettiheim", 118},
            {"Feminie", 119},
            {"Piconye", 120},
            {"Andramania", 121},
            {"Erytheia", 125},
            {"Atlantis", 126},
        };

        private StringCollection _aiNames = Settings.Default.Ais ?? [];

        private CancellationTokenSource? _cancellationTokenSource;
        private bool _autoTurnsOn = false;
        private Task _turnTask = Task.CompletedTask;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = _data;

            foreach (var ai in _nationNameToNbrMap)
            {
                var checkbox = new CheckBox
                {
                    Content = ai.Key,
                    IsChecked = _aiNames.Contains(ai.Key),
                    MinWidth = 150
                };
                checkbox.Checked += CheckBox_Checked;
                checkbox.Unchecked += Checkbox_Unchecked;
                Wrap.Children.Add(checkbox);
            }

        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            Dom6AiTester.Dom6AiTester.StartServer(_data.Dom6ExeLocation, _data.GameName, _data.Map, _data.AiMod, _aiNames.Cast<string>().Select(x => _nationNameToNbrMap[x]));
        }

        private void StartClient_Click(object sender, RoutedEventArgs e)
        {
            Dom6AiTester.Dom6AiTester.StartClient(_data.Dom6ExeLocation);
        }

        private async void StartAutoTurns_Click(object sender, RoutedEventArgs e)
        {
            if (_autoTurnsOn)
            {
                _cancellationTokenSource?.Cancel();
                await _turnTask;
                ((Button)sender).Content = "Start Auto Turn";
            }
            else
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _turnTask = Dom6AiTester.Dom6AiTester.StartTurns(_data.GameName, _cancellationTokenSource.Token);
                ((Button)sender).Content = "Stop Auto Turn";
            }

            _autoTurnsOn = !_autoTurnsOn;
        }

        private void SelectDomExe_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dominions6";
            dialog.FileName = "Dominions6";
            dialog.DefaultExt = ".exe";
            dialog.Filter = "Executable (.exe)|*.exe";

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                _data.Dom6ExeLocation = dialog.FileName;
            }
        }

        private void SelectMap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Dominions6\\maps";
            dialog.DefaultExt = ".map";
            dialog.Filter = "Map (.map)|*.map";

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                _data.Map = Path.GetFileName(dialog.FileName);
            }
        }

        private void SelectAiMod_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Dominions6\\mods";
            dialog.DefaultExt = ".dm";
            dialog.Filter = "Mod (.dm)|*.dm";

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                _data.AiMod = Path.GetFileName(Path.GetDirectoryName(dialog.FileName)) + "\\" + Path.GetFileName(dialog.FileName);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            _aiNames.Add(checkBox?.Content.ToString());
            Settings.Default.Ais = _aiNames;
            Settings.Default.Save();
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            _aiNames.Remove(checkBox?.Content.ToString());
            Settings.Default.Ais = _aiNames;
            Settings.Default.Save();
        }
    }
}
