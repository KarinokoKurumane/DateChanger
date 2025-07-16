using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.IO;

namespace DateChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string label_basic_folder = "Zmień datę modyfikacji folderu: ";
        private const string label_root_folder = "Zmień datę modyfikacji folderu nadrzędnego: ";

        private string folder_path = string.Empty;
        private bool include_folder = false;
        private bool include_folder_root = false;
        private DateTime? new_date = null;
        private DirectoryInfo? directory_info = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Logika zmiany daty plików w wybranym folderze.
        /// </summary>
        private void ChangeFileDateLogic()
        {
            if (directory_info == null)
            {
                MessageBox.Show("Nie wybrano folderu!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (new_date == null)
            {
                MessageBox.Show("Nie wybrano daty!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (FileInfo _file in directory_info.GetFiles())
            {
                try
                {
                    _file.LastWriteTime = new_date.Value;
                    _file.CreationTime = new_date.Value;
                    _file.LastAccessTime = new_date.Value;
                    if (include_folder)
                    {
                        ChangeFolderDate(_file.Directory, new_date.Value);
                    }
                    if (include_folder_root && _file.Directory.Parent != null)
                    {
                        ChangeFolderDate(_file.Directory.Parent, new_date.Value);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas zmiany daty pliku: {_file.Name}\n{ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Zmienia datę modyfikacji folderu.
        /// </summary>
        /// <param name="_di"></param>
        /// <param name="_dt"></param>
        private void ChangeFolderDate(DirectoryInfo _di, DateTime _dt)
        {
            try
            {
                _di.LastWriteTime = _dt;
                _di.CreationTime = _dt;
                _di.LastAccessTime = _dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zmiany daty folderu: {_di.Name}\n{ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Główna logika aplikacji, która jest wywoływana po wyborze folderu.
        /// </summary>
        private void BaseLogic()
        {
            if (folder_path == string.Empty)
            {
                MessageBox.Show("Nie wybrano folderu!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                directory_info = new DirectoryInfo(folder_path);
                try
                {
                    Txb_DirectoryName.Text = directory_info.FullName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas odczytu folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                finally
                {
                    PrepareCheckboxes(directory_info);
                    PrepareFilesList();
                }
            }
        }

        /// <summary>
        /// Przygotowuje checkboxy z informacjami o folderze i folderze nadrzędnym.
        /// </summary>
        /// <param name="_di"></param>
        private void PrepareCheckboxes(DirectoryInfo _di)
        {
            if (_di != null)
            {
                Cbx_IncludeFolder.Content = label_basic_folder + _di.Name;
                Cbx_IncludeFolder.IsEnabled = true;
            }
            else
            {
                Cbx_IncludeFolder.Content = label_basic_folder + "Brak folderu";
                Cbx_IncludeFolder.IsEnabled = false;
                return; // Jeśli nie ma folderu, nie ustawiamy checkboxa dla folderu nadrzędnego
            }
            if (_di.Parent != null)
            {
                Cbx_IncludeFolderRoot.Content = label_root_folder + _di.Parent.Name;
                Cbx_IncludeFolderRoot.IsEnabled = true;
            }
            else
            {
                Cbx_IncludeFolderRoot.Content = label_root_folder + "Brak folderu nadrzędnego";
                Cbx_IncludeFolderRoot.IsEnabled = false;
            }
        }

        /// <summary>
        /// Przygotowuje listę plików w wybranym folderze.
        /// </summary>
        private void PrepareFilesList()
        {
            List<FileItem> files = [];
            if (directory_info == null)
            {
                MessageBox.Show("Nie wybrano folderu!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            foreach (FileInfo fi in directory_info.GetFiles())
            {
                files.Add(new FileItem
                {
                    Name = fi.Name,
                    LastModified = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                });
            }

            Lvw_FilesList.ItemsSource = files;
        }

        /// <summary>
        /// Pozyskuje informacje o folderze i uruchamia logikę podstawową.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFolderDialog _openFolderDialog = new()
                {
                    Title = "Wybierz folder",
                    ValidateNames = false
                };

                if (_openFolderDialog.ShowDialog() == true)
                {
                    folder_path = _openFolderDialog.FolderName;
                    BaseLogic();
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Brak uprawnień do wybranego folderu. Wybierz inny folder.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wyboru folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reakcja na kliknięcie przycisku "Zmień datę plików".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_BeginWork_Click(object sender, RoutedEventArgs e) => ChangeFileDateLogic();

        /// <summary>
        /// Pobiera datę z kontrolki DatePicker i ustawia ją jako nową datę do zmiany.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dtp_NewDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Dtp_NewDate.SelectedDate.HasValue)
            {
                new_date = new DateTime(
                    Dtp_NewDate.SelectedDate.Value.Year,
                    Dtp_NewDate.SelectedDate.Value.Month,
                    Dtp_NewDate.SelectedDate.Value.Day,
                    8, 0, 0);
            }
            else
            {
                new_date = null;
            }
            Txb_NewDate.Text = new_date.HasValue ? new_date.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Nie wybrano daty";
        }

        /// <summary>
        /// Czyszczenie listy plików po inicjalizacji kontrolki ListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lvw_FilesList_Initialized(object sender, EventArgs e) => Lvw_FilesList.Items.Clear();

        /// <summary>
        /// W zależności od zaznaczenia checkboxa, ustawiamy flagę _includeFolder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbx_IncludeFolder_Checked(object sender, RoutedEventArgs e) => include_folder = true;

        /// <summary>
        /// W zależności od odznaczenia checkboxa, ustawiamy flagę _includeFolder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbx_IncludeFolder_Unchecked(object sender, RoutedEventArgs e) => include_folder = false;

        /// <summary>
        /// W zależności od zaznaczenia checkboxa, ustawiamy flagę _includeFolderRoot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbx_IncludeFolderRoot_Checked(object sender, RoutedEventArgs e) => include_folder_root = true;

        /// <summary>
        /// W zależności od odznaczenia checkboxa, ustawiamy flagę _includeFolderRoot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbx_IncludeFolderRoot_Unchecked(object sender, RoutedEventArgs e) => include_folder_root = false;
    }
}