using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class CuboidSAPage : Page
{
    public CuboidSAPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(LengthTextBox.Text, out double length) &&
            double.TryParse(BreadthTextBox.Text, out double breadth) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double surfaceArea = 2 * (length * breadth + breadth * height + height * length);
            ResultTextBlock.Text = (surfaceArea.ToString("F2") + " sq. units").ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}