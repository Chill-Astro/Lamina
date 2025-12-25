using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CubeVolumePage : Page
{
    public CubeVolumePage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(SideTextBox.Text, out double side))
        {
            double volume = Math.Pow(side, 3);
            ResultTextBlock.Text = (volume.ToString("F2") + " c. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}