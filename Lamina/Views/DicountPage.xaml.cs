using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class DiscountPage : Page
{
    public DiscountPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(OriginalPriceTextBox.Text, out double originalPrice) &&
            double.TryParse(DiscountPercentageTextBox.Text, out double discountPercentage))
        {
            if (discountPercentage > 100 || discountPercentage < 0)
            {
                DiscountAmountTextBlock.Text = "INVALID PERCENTAGE".ToUpper();
                FinalPriceTextBlock.Text = "CAN'T CALCULATE RPICE".ToUpper();
                return;
            }

            double discountAmount = originalPrice * (discountPercentage / 100);
            double finalPrice = originalPrice - discountAmount;

            DiscountAmountTextBlock.Text = $"DISCOUNT AMOUNT: {discountAmount:F2}".ToUpper();
            FinalPriceTextBlock.Text = $"FINAL PRICE: {finalPrice:F2}".ToUpper();
        }
        else
        {
            DiscountAmountTextBlock.Text = "INVALID INPUT".ToUpper();
            FinalPriceTextBlock.Text = "INVALID INPUT".ToUpper();
        }
    }
}