using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Numerics;

namespace Lamina.Views;

public sealed partial class FactorialPage : Page
{
    public FactorialPage()
    {
        InitializeComponent();
    }

    private void CalculateFactButton_Click(object sender, RoutedEventArgs e)
    {
        string input = FactTextBox.Text;
        FectorialTextBlock.Text = "OUTPUT AREA"; // Reset output

        if (string.IsNullOrWhiteSpace(input))
        {
            FectorialTextBlock.Text = "INVALID INPUT";
            return;
        }

        if (!int.TryParse(input, out int n))
        {
            FectorialTextBlock.Text = "INVALID INPUT";
            return;
        }

        if (n < 0)
        {
            FectorialTextBlock.Text = "ONLY +VE ACCEPTED";
            return;
        }

        BigInteger factorial = 1;
        for (int i = 1; i <= n; i++)
        {
            factorial *= i;
        }

        FectorialTextBlock.Text = factorial.ToString(); // Display only the factorial result
    }
}