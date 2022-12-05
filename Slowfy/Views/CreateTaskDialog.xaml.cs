using Microsoft.UI.Xaml.Controls;

namespace XamlBrewer.WinUI3.Navigation.Sample.Views;
public sealed partial class CreateTaskDialog : ContentDialog
{
    public CreateTaskDialog()
    {
        this.InitializeComponent();
    }

    string newTask;

    private void newTaskCreate_TextChanged(object sender, TextChangedEventArgs e)
    {
        newTask = newTaskCreate.Text;
        this.Tag = newTask;
    }
}