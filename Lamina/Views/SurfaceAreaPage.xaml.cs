using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Lamina.Views;

public sealed partial class SurfaceAreaPage : Page
{
    public SurfaceAreaPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Load the Cube surface area page by default
        ContentFrame.Navigate(typeof(CubeSAPage));
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
                case "Cube":
                    ContentFrame.Navigate(typeof(CubeSAPage));
                    break;
                case "Cuboid":
                    ContentFrame.Navigate(typeof(CuboidSAPage));
                    break;
                case "Cylinder":
                    ContentFrame.Navigate(typeof(CylinderSAPage));
                    break;
                case "Cone":
                    ContentFrame.Navigate(typeof(ConeSAPage));
                    break;
                case "Sphere":
                    ContentFrame.Navigate(typeof(SphereSAPage));
                    break;
            }
        }
    }
}