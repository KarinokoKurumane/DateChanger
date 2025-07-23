using DataChanger.Animations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DateChanger.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy TitleBar.xaml
    /// Co w kontrolce pozostaje w kontrolce.
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pobiera okno nadrzędne, do którego należy ta kontrolka.
        /// </summary>
        /// <returns></returns>
        private Window? GetParentWindow() => Window.GetWindow(this);

        /// <summary>
        /// Pozwala przeciągać okno, gdy kliknięto na pasek tytułowy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                GetParentWindow()?.DragMove();
        }

        /// <summary>
        /// Minimalizuje okno, gdy kliknięto przycisk minimalizacji.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            GetParentWindow()!.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Powiększa lub przywraca okno do normalnego rozmiaru, gdy kliknięto przycisk maksymalizacji.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            var win = GetParentWindow();
            if (win == null) return;

            win.WindowState = win.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            (sender as Button)!.Content = win.WindowState == WindowState.Maximized ? "❐" : "⬜";
        }

        /// <summary>
        /// Zamyka okno, gdy kliknięto przycisk zamykania. Z animacją zmniejszenia skali i zanikania. Takie pro.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Animations.CloseWithAnimation(GetParentWindow());
        }

        /// <summary>
        /// Reakcja na kliknięcie przycisku logowania.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Config_Click(object sender, RoutedEventArgs e)
        {
            // TODO: otwórz okno logowania
        }

        /// <summary>
        /// Reakcja na kliknięcie przycisku pomocy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            // TODO: otwórz okno pomocy / link
        }

    }
}
