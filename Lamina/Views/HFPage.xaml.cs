using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views
{
    public sealed partial class HFPage : Page
    {
        public HFPage()
        {
            this.InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            string sideAString = SideATextBox.Text;
            string sideBString = SideBTextBox.Text;
            string sideCString = SideCTextBox.Text;

            double sideA, sideB, sideC, area;
            AreaTextBlock.Text = "";

            if (double.TryParse(sideAString, out sideA) &&
                double.TryParse(sideBString, out sideB) &&
                double.TryParse(sideCString, out sideC))
            {
                if (sideA <= 0 || sideB <= 0 || sideC <= 0)
                {
                    AreaTextBlock.Text = "SIDES MUST BE +ve";
                }
                else if (sideA + sideB <= sideC || sideA + sideC <= sideB || sideB + sideC <= sideA)
                {
                    AreaTextBlock.Text = "INVALID SIDES";
                }
                else
                {
                    double s = (sideA + sideB + sideC) / 2.0;
                    area = Math.Sqrt(s * (s - sideA) * (s - sideB) * (s - sideC));
                    AreaTextBlock.Text = area.ToString("F2") + " sq. units";
                }
            }
            else
            {
                AreaTextBlock.Text = "INVALID INPUT";
            }
        }
    }
}