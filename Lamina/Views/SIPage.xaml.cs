using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views
{
    public sealed partial class SIPage : Page
    {
        public SIPage()
        {
            this.InitializeComponent();
        }

        private void CalculateSIButton_Click(object sender, RoutedEventArgs e)
        {
            string principalString = PrincipalTextBox.Text;
            string rateString = RateTextBox.Text;
            string timeString = TimeTextBox.Text;

            double principal, rate, time, simpleInterest;
            SimpleInterestTextBlock.Text = ""; // Clear previous result

            if (double.TryParse(principalString, out principal) &&
                double.TryParse(rateString, out rate) &&
                double.TryParse(timeString, out time))
            {
                if (principal < 0 || rate < 0 || time < 0) // Principal, Rate, Time should be non-negative
                {
                    SimpleInterestTextBlock.Text = "VALUES MUST BE +VE";
                }
                else
                {
                    simpleInterest = (principal * rate * time) / 100.0; // SI Formula

                    SimpleInterestTextBlock.Text = simpleInterest.ToString("C"); // Display SI in Currency format
                }
            }
            else
            {
                SimpleInterestTextBlock.Text = "INVALID INPUTS";
            }
        }
    }
}