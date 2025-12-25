using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Lamina.Views;

public sealed partial class VolumePage : Page
{
    public VolumePage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Load the Cube volume page by default
        ContentFrame.Navigate(typeof(CubeVolumePage));
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
                    ContentFrame.Navigate(typeof(CubeVolumePage));
                    break;
                case "Cuboid":
                    ContentFrame.Navigate(typeof(CuboidVolumePage));
                    break;
                case "Cylinder":
                    ContentFrame.Navigate(typeof(CylinderVolumePage));
                    break;
                case "Cone":
                    ContentFrame.Navigate(typeof(ConeVolumePage));
                    break;
                case "Sphere":
                    // User mentioned they named it Sphere.xaml
                    ContentFrame.Navigate(typeof(SpherePage));
                    break;
            }
        }
    }
}