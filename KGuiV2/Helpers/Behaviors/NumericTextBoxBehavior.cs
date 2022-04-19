using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace KGuiV2.Helpers.Behaviors
{
    // TODO!: handle pasting events
    internal class NumericTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Occurs when this element gets text in a device-independent manner.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = sender is TextBox && !char.IsDigit(e.Text, e.Text.Length - 1);

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            AssociatedObject.PreviewTextInput += AssociatedObject_PreviewTextInput;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewTextInput -= AssociatedObject_PreviewTextInput;
        }
    }
}
