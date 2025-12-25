using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class ApproxPage : Page
{
    public ApproxPage()
    {
        InitializeComponent();
    }

    private void RoundButton_Click(object sender, RoutedEventArgs e)
    {
        string input = NumberTextBox.Text;
        RoundedTextBlock.Text = "ROUNDED VALUE"; // Reset output

        if (string.IsNullOrWhiteSpace(input))
        {
            RoundedTextBlock.Text = "INVALID INPUT";
            return;
        }

        if (double.TryParse(input, out double number))
        {
            RoundedTextBlock.Text = Math.Round(number).ToString();
        }
        else
        {
            RoundedTextBlock.Text = "INVALID INPUT";
        }
    }
}