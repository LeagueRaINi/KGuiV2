using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace KGuiV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Creates a new <see cref="MainWindow"/> instance.
        /// </summary>
        public MainWindow()
            => InitializeComponent();

        /// <summary>
        /// TODO!: this will be removed later
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = sender is TextBox && !char.IsDigit(e.Text, e.Text.Length - 1);

        /// <summary>
        /// TODO!: this will be removed later
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
            => e.Handled =
               e.Command == ApplicationCommands.Copy
            || e.Command == ApplicationCommands.Cut
            || e.Command == ApplicationCommands.Paste;
    }
}
