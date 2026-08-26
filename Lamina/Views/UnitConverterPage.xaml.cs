using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;

namespace Lamina.Views;

public sealed partial class UnitConverterPage : Page
{
    private string lastResult = "";

    // Units categorized by their full names for internal logic
    private Dictionary<string, List<string>> unitsByCategory = new Dictionary<string, List<string>>()
    {
        {"Length", new List<string>{"Meters", "Feet", "Inches", "Kilometers", "Miles", "Centimeters", "Millimeters", "Yards", "Nautical Miles"}},
        {"Weight (Mass)", new List<string>{"Kilograms", "Pounds", "Grams", "Ounces", "Milligrams", "Stones", "Tons (metric)", "Tons (US)"}},
        {"Volume", new List<string>{"Liters", "Gallons (US)", "Gallons (UK)", "Cubic Meters", "Cubic Feet", "Cubic Inches", "Milliliters", "Cups (US)", "Pints (US)", "Quarts (US)"}},
        {"Temperature", new List<string>{"Celsius", "Fahrenheit", "Kelvin"}},
        {"Area", new List<string>{"Square Meters", "Square Feet", "Square Inches", "Square Kilometers", "Square Miles", "Hectares", "Acres"}},
        {"Time", new List<string>{"Seconds", "Minutes", "Hours", "Days", "Weeks", "Years"}},
        {"Speed", new List<string>{"Meters per second", "Kilometers per hour", "Miles per hour", "Knots"}},
        {"Energy", new List<string>{"Joules", "Kilojoules", "Calories", "Kilocalories", "British Thermal Units"}},
        {"Pressure", new List<string>{"Pascals", "Kilopascals", "Bar", "Pounds per square inch", "Atmospheres"}},
        {"Power", new List<string>{"Watts", "Kilowatts", "Horsepower"}},
        {"Data (Digital Storage)", new List<string>{"Bits", "Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes"}},
        {"Angle", new List<string>{"Degrees", "Radians"}}
    };

    // Mapping for the UI to show shortened notations
    private Dictionary<string, string> unitSymbols = new Dictionary<string, string>()
    {
        {"Meters", "m"}, {"Feet", "ft"}, {"Inches", "in"}, {"Kilometers", "km"}, {"Miles", "mi"}, {"Centimeters", "cm"}, {"Millimeters", "mm"}, {"Yards", "yd"}, {"Nautical Miles", "nmi"},
        {"Kilograms", "kg"}, {"Pounds", "lb"}, {"Grams", "g"}, {"Ounces", "oz"}, {"Milligrams", "mg"}, {"Stones", "st"}, {"Tons (metric)", "t"}, {"Tons (US)", "ton (US)"},
        {"Liters", "L"}, {"Gallons (US)", "gal (US)"}, {"Gallons (UK)", "gal (UK)"}, {"Cubic Meters", "m³"}, {"Cubic Feet", "ft³"}, {"Cubic Inches", "in³"}, {"Milliliters", "mL"}, {"Cups (US)", "cup (US)"}, {"Pints (US)", "pt (US)"}, {"Quarts (US)", "qt (US)"},
        {"Celsius", "°C"}, {"Fahrenheit", "°F"}, {"Kelvin", "K"},
        {"Square Meters", "m²"}, {"Square Feet", "ft²"}, {"Square Inches", "in²"}, {"Square Kilometers", "km²"}, {"Square Miles", "mi²"}, {"Hectares", "ha"}, {"Acres", "ac"},
        {"Seconds", "s"}, {"Minutes", "min"}, {"Hours", "h"}, {"Days", "d"}, {"Weeks", "wk"}, {"Years", "yr"},
        {"Meters per second", "m/s"}, {"Kilometers per hour", "km/h"}, {"Miles per hour", "mph"}, {"Knots", "kn"},
        {"Joules", "J"}, {"Kilojoules", "kJ"}, {"Calories", "cal"}, {"Kilocalories", "kcal"}, {"British Thermal Units", "BTU"},
        {"Pascals", "Pa"}, {"Kilopascals", "kPa"}, {"Bar", "bar"}, {"Pounds per square inch", "psi"}, {"Atmospheres", "atm"},
        {"Watts", "W"}, {"Kilowatts", "kW"}, {"Horsepower", "hp"},
        {"Bits", "bit"}, {"Bytes", "B"}, {"Kilobytes", "KB"}, {"Megabytes", "MB"}, {"Gigabytes", "GB"}, {"Terabytes", "TB"},
        {"Degrees", "°"}, {"Radians", "rad"}
    };

    public UnitConverterPage()
    {
        this.InitializeComponent();
        CategoryMenu.SelectedIndex = 0;
    }

    private void CategoryMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoryMenu.SelectedItem is ComboBoxItem selectedItem)
        {
            string category = selectedItem.Content.ToString();
            if (unitsByCategory.ContainsKey(category))
            {
                var shortUnits = new List<string>();
                foreach (var fullName in unitsByCategory[category])
                {
                    shortUnits.Add(unitSymbols.ContainsKey(fullName) ? unitSymbols[fullName] : fullName);
                }
                FromUnit.ItemsSource = shortUnits;
                ToUnit.ItemsSource = shortUnits;
                FromUnit.SelectedIndex = 0;
                ToUnit.SelectedIndex = 1;
            }
        }
    }

    private string GetFullNameFromSymbol(string symbol)
    {
        foreach (var pair in unitSymbols)
        {
            if (pair.Value == symbol) return pair.Key;
        }
        return symbol;
    }

    // Convertion Fluff
    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(ValueToConvert.Text, out double val))
        {
            await ShowResultPopup("Input Issue:", "Please enter a valid number.", false);
            return;
        }

        if (FromUnit.SelectedItem is string fromSym &&
            ToUnit.SelectedItem is string toSym &&
            CategoryMenu.SelectedItem is ComboBoxItem catItem)
        {
            string category = catItem.Content.ToString().Trim();
            string fromUnit = GetFullNameFromSymbol(fromSym);
            string toUnit = GetFullNameFromSymbol(toSym);

            double result = 0;
            switch (category)
            {
                case "Length": result = ConvertLength(val, fromUnit, toUnit); break;
                case "Weight (Mass)": result = ConvertWeight(val, fromUnit, toUnit); break;
                case "Volume": result = ConvertVolume(val, fromUnit, toUnit); break;
                case "Temperature": result = ConvertTemperature(val, fromUnit, toUnit); break;
                case "Area": result = ConvertArea(val, fromUnit, toUnit); break;
                case "Time": result = ConvertTime(val, fromUnit, toUnit); break;
                case "Speed": result = ConvertSpeed(val, fromUnit, toUnit); break;
                case "Energy": result = ConvertEnergy(val, fromUnit, toUnit); break;
                case "Pressure": result = ConvertPressure(val, fromUnit, toUnit); break;
                case "Power": result = ConvertPower(val, fromUnit, toUnit); break;
                case "Data (Digital Storage)": result = ConvertData(val, fromUnit, toUnit); break;
                case "Angle": result = ConvertAngle(val, fromUnit, toUnit); break;
            }

            lastResult = result > 1000000 ? $"{result:G6} {toSym}" : $"{result:N4} {toSym}";
            await ShowResultPopup($"{val} {fromSym} =", lastResult, true);
        }
        else
        {
            await ShowResultPopup("Selection Issue :", "Please Select Both Units", false);
        }
    }

    private async Task ShowResultPopup(string label, string value, bool isSuccess)
    {
        ResultLabel.Text = label;
        ResultValue.Text = value;
        CopyButton.Visibility = isSuccess ? Visibility.Visible : Visibility.Collapsed;
        ResultValue.Foreground = isSuccess
            ? (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
            : (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];

        ResultDialog.XamlRoot = this.Content.XamlRoot;
        await ResultDialog.ShowAsync();
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        var dp = new DataPackage();
        dp.SetText(lastResult);
        Clipboard.SetContent(dp);
    }    

    // More Convertion Fluff
    private double ConvertLength(double value, string from, string to)
    {
        if (from == to) return value;
        double m = from switch
        {
            "Meters" => value,
            "Feet" => value * 0.3048,
            "Inches" => value * 0.0254,
            "Kilometers" => value * 1000,
            "Miles" => value * 1609.34,
            "Centimeters" => value * 0.01,
            "Millimeters" => value * 0.001,
            "Yards" => value * 0.9144,
            "Nautical Miles" => value * 1852,
            _ => 0
        };
        return to switch
        {
            "Meters" => m,
            "Feet" => m / 0.3048,
            "Inches" => m / 0.0254,
            "Kilometers" => m / 1000,
            "Miles" => m / 1609.34,
            "Centimeters" => m / 0.01,
            "Millimeters" => m / 0.001,
            "Yards" => m / 0.9144,
            "Nautical Miles" => m / 1852,
            _ => 0
        };
    }

    private double ConvertWeight(double value, string from, string to)
    {
        if (from == to) return value;
        double kg = from switch
        {
            "Kilograms" => value,
            "Pounds" => value * 0.453592,
            "Grams" => value / 1000,
            "Ounces" => value * 0.0283495,
            "Milligrams" => value / 1000000,
            "Stones" => value * 6.35029,
            "Tons (metric)" => value * 1000,
            "Tons (US)" => value * 907.185,
            _ => 0
        };
        return to switch
        {
            "Kilograms" => kg,
            "Pounds" => kg / 0.453592,
            "Grams" => kg * 1000,
            "Ounces" => kg / 0.0283495,
            "Milligrams" => kg * 1000000,
            "Stones" => kg / 6.35029,
            "Tons (metric)" => kg / 1000,
            "Tons (US)" => kg / 907.185,
            _ => 0
        };
    }

    private double ConvertVolume(double value, string from, string to)
    {
        if (from == to) return value;
        double l = from switch
        {
            "Liters" => value,
            "Gallons (US)" => value * 3.78541,
            "Gallons (UK)" => value * 4.54609,
            "Cubic Meters" => value * 1000,
            "Cubic Feet" => value * 28.3168,
            "Cubic Inches" => value * 0.0163871,
            "Milliliters" => value / 1000,
            "Cups (US)" => value * 0.24,
            "Pints (US)" => value * 0.473176,
            "Quarts (US)" => value * 0.946353,
            _ => 0
        };
        return to switch
        {
            "Liters" => l,
            "Gallons (US)" => l / 3.78541,
            "Gallons (UK)" => l / 4.54609,
            "Cubic Meters" => l / 1000,
            "Cubic Feet" => l / 28.3168,
            "Cubic Inches" => l / 0.0163871,
            "Milliliters" => l * 1000,
            "Cups (US)" => l / 0.24,
            "Pints (US)" => l / 0.473176,
            "Quarts (US)" => l / 0.946353,
            _ => 0
        };
    }

    private double ConvertTemperature(double value, string from, string to)
    {
        if (from == to) return value;
        double celsius = from switch { "Celsius" => value, "Fahrenheit" => (value - 32) * 5 / 9, "Kelvin" => value - 273.15, _ => 0 };
        return to switch { "Celsius" => celsius, "Fahrenheit" => (celsius * 9 / 5) + 32, "Kelvin" => celsius + 273.15, _ => 0 };
    }

    private double ConvertArea(double value, string from, string to)
    {
        if (from == to) return value;
        double m2 = from switch
        {
            "Square Meters" => value,
            "Square Feet" => value * 0.092903,
            "Square Inches" => value * 0.00064516,
            "Square Kilometers" => value * 1000000,
            "Square Miles" => value * 2589988.11,
            "Hectares" => value * 10000,
            "Acres" => value * 4046.86,
            _ => 0
        };
        return to switch
        {
            "Square Meters" => m2,
            "Square Feet" => m2 / 0.092903,
            "Square Inches" => m2 / 0.00064516,
            "Square Kilometers" => m2 / 1000000,
            "Square Miles" => m2 / 2589988.11,
            "Hectares" => m2 / 10000,
            "Acres" => m2 / 4046.86,
            _ => 0
        };
    }

    private double ConvertTime(double value, string from, string to)
    {
        if (from == to) return value;
        double s = from switch { "Seconds" => value, "Minutes" => value * 60, "Hours" => value * 3600, "Days" => value * 86400, "Weeks" => value * 604800, "Years" => value * 31536000, _ => 0 };
        return to switch { "Seconds" => s, "Minutes" => s / 60, "Hours" => s / 3600, "Days" => s / 86400, "Weeks" => s / 604800, "Years" => s / 31536000, _ => 0 };
    }

    private double ConvertSpeed(double value, string from, string to)
    {
        if (from == to) return value;
        double mps = from switch { "Meters per second" => value, "Kilometers per hour" => value / 3.6, "Miles per hour" => value * 0.44704, "Knots" => value * 0.514444, _ => 0 };
        return to switch { "Meters per second" => mps, "Kilometers per hour" => mps * 3.6, "Miles per hour" => mps / 0.44704, "Knots" => mps / 0.514444, _ => 0 };
    }

    private double ConvertEnergy(double value, string from, string to)
    {
        if (from == to) return value;
        double j = from switch { "Joules" => value, "Kilojoules" => value * 1000, "Calories" => value * 4.184, "Kilocalories" => value * 4184, "British Thermal Units" => value * 1055.06, _ => 0 };
        return to switch { "Joules" => j, "Kilojoules" => j / 1000, "Calories" => j / 4.184, "Kilocalories" => j / 4184, "British Thermal Units" => j / 1055.06, _ => 0 };
    }

    private double ConvertPressure(double value, string from, string to)
    {
        if (from == to) return value;
        double pa = from switch { "Pascals" => value, "Kilopascals" => value * 1000, "Bar" => value * 100000, "Pounds per square inch" => value * 6894.76, "Atmospheres" => value * 101325, _ => 0 };
        return to switch { "Pascals" => pa, "Kilopascals" => pa / 1000, "Bar" => pa / 100000, "Pounds per square inch" => pa / 6894.76, "Atmospheres" => pa / 101325, _ => 0 };
    }

    private double ConvertPower(double value, string from, string to)
    {
        if (from == to) return value;
        double w = from switch { "Watts" => value, "Kilowatts" => value * 1000, "Horsepower" => value * 745.7, _ => 0 };
        return to switch { "Watts" => w, "Kilowatts" => w / 1000, "Horsepower" => w / 745.7, _ => 0 };
    }

    private double ConvertData(double value, string from, string to)
    {
        if (from == to) return value;
        double b = from switch { "Bits" => value / 8, "Bytes" => value, "Kilobytes" => value * 1024, "Megabytes" => value * 1024 * 1024, "Gigabytes" => value * 1024.0 * 1024 * 1024, "Terabytes" => value * 1024.0 * 1024 * 1024 * 1024, _ => 0 };
        return to switch { "Bits" => b * 8, "Bytes" => b, "Kilobytes" => b / 1024, "Megabytes" => b / (1024 * 1024), "Gigabytes" => b / (1024.0 * 1024 * 1024), "Terabytes" => b / (1024.0 * 1024 * 1024 * 1024), _ => 0 };
    }

    private double ConvertAngle(double value, string from, string to)
    {
        if (from == to) return value;
        return from == "Degrees" ? value * Math.PI / 180 : value * 180 / Math.PI;
    }
    private void SwapButton_Click(object sender, RoutedEventArgs e)
    {
        // Swap the selected indices
        int tempIndex = FromUnit.SelectedIndex;
        FromUnit.SelectedIndex = ToUnit.SelectedIndex;
        ToUnit.SelectedIndex = tempIndex;        
    }
}