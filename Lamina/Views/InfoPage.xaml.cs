using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace Lamina.Views
{
    public sealed partial class InfoPage : Page
    {
        public class ButtonInfo
        {
            public string Label { get; set; }
            public string Description { get; set; }
        }

        public InfoPage()
        {
            this.InitializeComponent();
            LoadButtonInfo();
        }

        private void LoadButtonInfo()
        {
            var buttonInfoList = new List<ButtonInfo>
            {
                new ButtonInfo { Label = "Sqrt()", Description = "> Square root" },
                new ButtonInfo { Label = "Pi", Description = "> Mathematical constant π" },
                new ButtonInfo { Label = "!", Description = "> Factorial" },
                new ButtonInfo { Label = "e", Description = "> Euler's Constant" },
                new ButtonInfo { Label = "DEG/RAD", Description = "> Degree/Radian mode toggle" },
                new ButtonInfo { Label = "Sin()", Description = "> Sine function" },
                new ButtonInfo { Label = "Cos()", Description = "> Cosine function" },
                new ButtonInfo { Label = "Tan()", Description = "> Tangent function" },
                new ButtonInfo { Label = "Inv", Description = "> Inverse trigonometric toggle" },
                new ButtonInfo { Label = "Ln()", Description = "> Natural logarithm" },
                new ButtonInfo { Label = "Log10()", Description = "> Common logarithm" },
                new ButtonInfo { Label = "Pow()", Description = "> Power function" },
                new ButtonInfo { Label = "1/x", Description = "> Reciprocal" },
                new ButtonInfo { Label = "+/-", Description = "> Negate" },
                new ButtonInfo { Label = "Cbrt()", Description = "> Cube root" },
                new ButtonInfo { Label = "AC", Description = "> Clear Screen" },
                new ButtonInfo { Label = "(", Description = "> Left parenthesis" },
                new ButtonInfo { Label = ")", Description = "> Right parenthesis" },
                new ButtonInfo { Label = ",", Description = "> Argument separator" }
            };

            ButtonInfoList.ItemsSource = buttonInfoList;
        }
    }
}
