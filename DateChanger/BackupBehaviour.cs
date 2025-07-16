using System.IO;
using System.IO.Compression;
using System.Windows;

namespace DateChanger
{
    internal class BackupBehaviour
    {
        /// <summary>
        /// Tworzy kopię zapasową folderu źródłowego w formie pliku ZIP w folderze docelowym.
        /// </summary>
        /// <param name="_sourceFolder"></param>
        /// <param name="_backupFolder"></param>
        public static void CreateBackupZip(string _sourceFolder, string _backupFolder)
        {
            try
            {
                if (!Directory.Exists(_sourceFolder))
                {
                    MessageBox.Show("Folder źródłowy nie istnieje!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Sprawdzenie, czy folder docelowy istnieje, jeśli nie, to go tworzymy, a to konieczne bo się sra
                if (!Directory.Exists(_backupFolder))
                {
                    Directory.CreateDirectory(_backupFolder);
                }

                ZipFile.CreateFromDirectory(_sourceFolder, Path.Combine(_backupFolder, new DirectoryInfo(_sourceFolder).Name + ".zip"), CompressionLevel.Fastest, true); // folder powinien zostać stworzony przez CreateFromDirectory
                MessageBox.Show("Kopia zapasowa została utworzona.", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia kopii zapasowej: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
