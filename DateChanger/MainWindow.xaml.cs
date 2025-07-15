using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DateChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _includeFolder = false;
        private DateTime? _newDate = null;
        private DirectoryInfo _directoryInfo = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeFileDate()
        {
            foreach (FileInfo _file in _directoryInfo.GetFiles())
            {
                try
                {
                    _file.LastWriteTime = _newDate.Value;
                    _file.CreationTime = _newDate.Value;
                    _file.LastAccessTime = _newDate.Value;
                    if (_includeFolder)
                    {
                        _file.Directory.LastWriteTime = _newDate.Value;
                        _file.Directory.CreationTime = _newDate.Value;
                        _file.Directory.LastAccessTime = _newDate.Value;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas zmiany daty pliku: " + _file.Name + "\n" + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Czyszczenie listy plików po inicjalizacji kontrolki.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lbx_FilesList_Initialized(object sender, EventArgs e)
        {
            Lbx_FilesList.Items.Clear();
        }

        /// <summary>
        /// W zależności od zaznaczenia checkboxa, ustawiamy flagę _includeFolder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbx_IncludeFolder_Checked(object sender, RoutedEventArgs e)
        {
            _includeFolder = true;
        }

        /// <summary>
        /// W zależności od odznaczenia checkboxa, ustawiamy flagę _includeFolder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbx_IncludeFolder_Unchecked(object sender, RoutedEventArgs e)
        {
            _includeFolder = false;
        }


        /// <summary>
        /// Pozyskuje informacje o plikach w danym folderze.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            string __path = string.Empty;
            OpenFolderDialog openFolderDialog = new()
            {
                Title = "Wybierz folder, którego zawartość będzie edytowana (pliki są niewidoczne)"
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                __path = openFolderDialog.FolderName;
            }
            if (__path == string.Empty)
            {
                MessageBox.Show("Nie wybrano folderu!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                _directoryInfo = new DirectoryInfo(__path);
                Txb_DirectoryName.Text = _directoryInfo.FullName;
                foreach (FileInfo _fi in _directoryInfo.GetFiles())
                {
                    Lbx_FilesList.Items.Add(_fi.Name+" ("+_fi.LastWriteTime+")");
                }
            }
        }

        /// <summary>
        /// Reakcja na kliknięcie przycisku "Zmień datę plików".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_BeginWork_Click(object sender, RoutedEventArgs e)
        {
            ChangeFileDate();
        }

        /// <summary>
        /// Pobiera datę z kontrolki DatePicker i ustawia ją jako nową datę do zmiany.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dtp_NewDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dtp_NewDate.SelectedDate.HasValue)
            {
                _newDate = new DateTime(
                    Dtp_NewDate.SelectedDate.Value.Year,
                    Dtp_NewDate.SelectedDate.Value.Month,
                    Dtp_NewDate.SelectedDate.Value.Day,
                    8, 0, 0);
            }
            else
            {
                _newDate = null;
            }
            Txb_NewDate.Text = _newDate.HasValue ? _newDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Nie wybrano daty";
        }
    }
}