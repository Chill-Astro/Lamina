using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class PrimeCheckPage : Page
{
    public PrimeCheckPage()
    {
        InitializeComponent();
    }

    private void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(NumberTextBox.Text, out int number))
        {
            if (number <= 1)
            {
                ResultTextBlock.Text = "NOT A PRIME NUMBER".ToUpper();
                return;
            }
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    ResultTextBlock.Text = "NOT A PRIME NUMBER".ToUpper();
                    return;
                }
            }
            ResultTextBlock.Text = "IS A PRIME NUMBER".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}