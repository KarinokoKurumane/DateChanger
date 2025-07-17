using System.IO;
using System.Windows;

namespace DateChanger
{
    internal class BasicBehaviour
    {
        /// <summary>
        /// Zmienia daty modyfikacji folderu.
        /// </summary>
        /// <param name="_di"></param>
        /// <param name="_dt"></param>
        public static void ChangeFolderDate(DirectoryInfo _di, DateTime _dt)
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
        /// Zmienia daty modyfikacji pliku.
        /// Posiada zabezpieczenie przed błędami, które mogą wystąpić podczas zmiany daty. Pojedynczy błąd w trakcie zmiany daty nie powinien przerwać całego procesu zmiany dat dla wszystkich plików.
        /// </summary>
        /// <param name="_fi"></param>
        /// <param name="_dt"></param>
        public static void ChangeFileDate(FileInfo _fi, DateTime _dt)
        {
            try
            {
                _fi.LastWriteTime = _dt;
                _fi.CreationTime = _dt;
                _fi.LastAccessTime = _dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zmiany daty pliku: {_fi.Name}\n{ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<FileItem>? GetFilesList(DirectoryInfo _di)
        {
            List<FileItem> _f = [];
            if (_di == null)
            {
                MessageBox.Show("Nie wybrano folderu!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            foreach (FileInfo _fi in _di.GetFiles())
            {
                _f.Add(new FileItem
                {
                    Name = _fi.Name,
                    LastModified = _fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return _f;
        }
    }
}
