using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DateChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LABEL_BASIC_FOLDER = "Zmień datę modyfikacji folderu: ";
        private const string LABEL_PARENT_FOLDER = "Zmień datę modyfikacji folderu nadrzędnego: ";
        private const string BACKUP_FOLDER_NAME = "Backups";

        /// <summary>
        /// Ścieżka do folderu, w którym znajdują się pliki do zmiany daty.
        /// </summary>
        private string folder_path = string.Empty;

        /// <summary>
        /// Ścieżka do folderu aplikacji, gdzie mogą być przechowywane backupy lub inne pliki.
        /// </summary>
        private string app_folder_path = string.Empty;

        /// <summary>
        /// Ścieżka do folderu, w którym będą przechowywane backupy plików w zip.
        /// </summary>
        private string backup_folder_path = string.Empty;

        private bool include_folder = false;
        private bool include_folder_root = false;
        private bool aslways_backup = false;

        private DateTime? new_date = null;
        private DirectoryInfo? directory_info = null;

        public MainWindow()
        {
            InitializeComponent();
            ResetMiniLog();
        }

        /// <summary>
        /// Logika zmiany daty plików w wybranym folderze.
        /// </summary>
        private void ChangeFileDateLogic()
        {
            var __inc = 0;
            foreach (FileInfo _file in directory_info.GetFiles())
            {
                try
                {
                    BasicBehaviour.ChangeFileDate(_file, new_date.Value);
                    __inc++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas zmiany daty pliku: {_file.Name}\n{ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    Tbx_MiniLog.Text = $"Wystąpił błąd podczas zmiany daty pliku";
                    return;
                }
            }
            if (include_folder)
            {
                BasicBehaviour.ChangeFolderDate(directory_info, new_date.Value);
            }
            if (include_folder_root && directory_info.Parent != null)
            {
                BasicBehaviour.ChangeFolderDate(directory_info.Parent, new_date.Value);
            }
            Tbx_MiniLog.Text = $"Zmieniono datę modyfikacji {__inc} plików.";
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
                Cbx_IncludeFolder.Content = LABEL_BASIC_FOLDER + _di.Name;
                Cbx_IncludeFolder.IsEnabled = true;
            }
            else
            {
                Cbx_IncludeFolder.Content = LABEL_BASIC_FOLDER + "Brak folderu";
                Cbx_IncludeFolder.IsEnabled = false;
                return; // Jeśli nie ma folderu, nie ustawiamy checkboxa dla folderu nadrzędnego
            }
            if (_di.Parent != null)
            {
                Cbx_IncludeFolderRoot.Content = LABEL_PARENT_FOLDER + _di.Parent.Name;
                Cbx_IncludeFolderRoot.IsEnabled = true;
            }
            else
            {
                Cbx_IncludeFolderRoot.Content = LABEL_PARENT_FOLDER + "Brak folderu nadrzędnego";
                Cbx_IncludeFolderRoot.IsEnabled = false;
            }
        }

        /// <summary>
        /// Przygotowuje listę plików w wybranym folderze.
        /// </summary>
        private void PrepareFilesList()
        {
            if (directory_info != null)
            {
                Lvw_FilesList.ItemsSource = BasicBehaviour.GetFilesList(directory_info);
            }
        }

        /// <summary>
        /// Pozyskuje informacje o folderze i uruchamia logikę podstawową.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            ResetMiniLog();
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
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wyboru folderu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Btn_CreateBackup.IsEnabled = false;
                return;
            }
            finally
            {
                Btn_CreateBackup.IsEnabled = true;
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

        /// <summary>
        /// Resetuje pole tekstowe mini logu, aby było puste (no kto by się spodziewał).
        /// </summary>
        private void ResetMiniLog() => Tbx_MiniLog.Text = string.Empty;

        /// <summary>
        /// Wywołuje metodę tworzenia kopii zapasowej folderu w formie pliku ZIP.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_CreateBackup_Click(object sender, RoutedEventArgs e) => BackupBehaviour.CreateBackupZip(new DirectoryInfo(folder_path).FullName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BACKUP_FOLDER_NAME));

        /// <summary>
        /// Otwiera folder kopii zapasowych w eksploratorze plików.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_BackupFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "/select,"+Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BACKUP_FOLDER_NAME));
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 2) // ERROR_FILE_NOT_FOUND
            {
                MessageBox.Show("Folder kopii zapasowej nie istnieje. Proszę najpierw utworzyć kopię zapasową.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas otwierania folderu kopii zapasowej: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}