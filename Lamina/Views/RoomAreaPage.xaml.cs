using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class RoomAreaPage : Page
{
    public RoomAreaPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(LengthTextBox.Text, out double length) &&
            double.TryParse(BreadthTextBox.Text, out double breadth) &&
            double.TryParse(HeightTextBox.Text, out double height))
        {
            double area = 2 * height * (length + breadth);
            ResultTextBlock.Text = $"AREA: {area:F2} units²".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}