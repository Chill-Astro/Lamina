using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class RTCheckPage : Page
{
    public RTCheckPage()
    {
        InitializeComponent();
    }

    private void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(SideATextBox.Text, out double a) &&
            double.TryParse(SideBTextBox.Text, out double b) &&
            double.TryParse(SideCTextBox.Text, out double c))
        {
            if (a <= 0 || b <= 0 || c <= 0)
            {
                ResultTextBlock.Text = "SIDES MUST BE +VE".ToUpper();
            }
            else if (Math.Pow(a, 2) + Math.Pow(b, 2) == Math.Pow(c, 2) ||
                     Math.Pow(a, 2) + Math.Pow(c, 2) == Math.Pow(b, 2) ||
                     Math.Pow(b, 2) + Math.Pow(c, 2) == Math.Pow(a, 2))
            {
                ResultTextBlock.Text = "IT IS A RIGHT TRIANGLE".ToUpper();
            }
            else
            {
                ResultTextBlock.Text = "NOT A RIGHT TRIANGLE.".ToUpper();
            }
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}