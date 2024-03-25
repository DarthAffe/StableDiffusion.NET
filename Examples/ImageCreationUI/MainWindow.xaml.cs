using System.Windows;
using System.Windows.Controls;

namespace ImageCreationUI;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs args) => (sender as TextBox)?.ScrollToEnd();
}