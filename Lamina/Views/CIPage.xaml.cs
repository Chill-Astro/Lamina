using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views
{
    public sealed partial class CIPage : Page
    {
        public CIPage()
        {
            this.InitializeComponent();
        }

        private void CalculateCIButton_Click(object sender, RoutedEventArgs e)
        {
            string principalString = CIPrincipalTextBox.Text;
            string rateString = CIRateTextBox.Text;
            string timeString = CITimeTextBox.Text;
            string compoundingFrequencyString = CompoundingFrequencyTextBox.Text;

            double principal, rate, time, compoundingFrequency, compoundInterest, amount;
            CompoundInterestTextBlock.Text = ""; // Clear previous result

            if (double.TryParse(principalString, out principal) &&
                double.TryParse(rateString, out rate) &&
                double.TryParse(timeString, out time) &&
                double.TryParse(compoundingFrequencyString, out compoundingFrequency))
            {
                if (principal < 0 || rate < 0 || time < 0 || compoundingFrequency <= 0) // Values should be non-negative, frequency positive
                {
                    CompoundInterestTextBlock.Text = "VALUES MUST BE +VE";
                }
                else if (compoundingFrequency == 0)
                {
                    CompoundInterestTextBlock.Text = "FREQUENCY MUST BE > 0";
                }
                else
                {
                    rate = rate / 100.0; // Rate from percentage to decimal
                    amount = principal * Math.Pow((1 + (rate / compoundingFrequency)), (compoundingFrequency * time)); // CI Amount Formula
                    compoundInterest = amount - principal; // Compound Interest

                    CompoundInterestTextBlock.Text = compoundInterest.ToString("C"); // Display CI in Currency format
                }
            }
            else
            {
                CompoundInterestTextBlock.Text = "INVALID INPUT";
            }
        }
    }
}