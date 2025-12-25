using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Lamina.Views;

public sealed partial class CSurfaceAreaPage : Page
{
    public CSurfaceAreaPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Load the Cylinder CSA page by default
        ContentFrame.Navigate(typeof(CylinderSAPage));
    }

    private void ShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ShapeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string shape = selectedItem.Content.ToString();

            if (ContentFrame == null)
            {
                System.Diagnostics.Debug.WriteLine("ContentFrame is null.");
                return;
            }

            switch (shape)
            {
                case "Cylinder":
                    ContentFrame.Navigate(typeof(CylinderCSAPage));
                    break;
                case "Cone":
                    ContentFrame.Navigate(typeof(ConeCSAPage));
                    break;
                case "Sphere":
                    ContentFrame.Navigate(typeof(SphereCSAPage));
                    break;
            }
        }
    }
}