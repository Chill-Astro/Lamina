// File: Views/UnitConverterPage.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace Lamina.Views;

public sealed partial class UnitConverterPage : Page
{
    // Dictionary to store units by category (full names)
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

    // Dictionary to map full unit names to symbols for output display
    private Dictionary<string, string> unitSymbols = new Dictionary<string, string>()
    {
        // Length
        {"Meters", "m"},
        {"Feet", "ft"},
        {"Inches", "in"},
        {"Kilometers", "km"},
        {"Miles", "mi"},
        {"Centimeters", "cm"},
        {"Millimeters", "mm"},
        {"Yards", "yd"},
        {"Nautical Miles", "nmi"},

        // Weight (Mass)
        {"Kilograms", "kg"},
        {"Pounds", "lb"},
        {"Grams", "g"},
        {"Ounces", "oz"},
        {"Milligrams", "mg"},
        {"Stones", "st"},
        {"Tons (metric)", "t"}, // or "tonne"
        {"Tons (US)", "ton (US)"},

        // Volume
        {"Liters", "L"}, // or "l"
        {"Gallons (US)", "gal (US)"},
        {"Gallons (UK)", "gal (UK)"},
        {"Cubic Meters", "m³"},
        {"Cubic Feet", "ft³"},
        {"Cubic Inches", "in³"},
        {"Milliliters", "mL"}, // or "ml"
        {"Cups (US)", "cup (US)"}, // Common abbreviation
        {"Pints (US)", "pt (US)"},
        {"Quarts (US)", "qt (US)"},

        // Temperature
        {"Celsius", "°C"},
        {"Fahrenheit", "°F"},
        {"Kelvin", "K"},

        // Area
        {"Square Meters", "m²"},
        {"Square Feet", "ft²"},
        {"Square Inches", "in²"},
        {"Square Kilometers", "km²"},
        {"Square Miles", "mi²"},
        {"Hectares", "ha"},
        {"Acres", "ac"},

        // Time
        {"Seconds", "s"},
        {"Minutes", "min"},
        {"Hours", "h"},
        {"Days", "d"},
        {"Weeks", "wk"},
        {"Years", "yr"},

        // Speed
        {"Meters per second", "m/s"},
        {"Kilometers per hour", "km/h"},
        {"Miles per hour", "mph"},
        {"Knots", "kn"},

        // Energy
        {"Joules", "J"},
        {"Kilojoules", "kJ"},
        {"Calories", "cal"},
        {"Kilocalories", "kcal"},
        {"British Thermal Units", "BTU"},

        // Pressure
        {"Pascals", "Pa"},
        {"Kilopascals", "kPa"},
        {"Bar", "bar"},
        {"Pounds per square inch", "psi"},
        {"Atmospheres", "atm"},

        // Power
        {"Watts", "W"},
        {"Kilowatts", "kW"},
        {"Horsepower", "hp"},

        // Data (Digital Storage)
        {"Bits", "bit"},
        {"Bytes", "B"},
        {"Kilobytes", "KB"}, // or KiB for kibibyte
        {"Megabytes", "MB"}, // or MiB for mebibyte
        {"Gigabytes", "GB"}, // or GiB for gibibyte
        {"Terabytes", "TB"}, // or TiB for tebibyte

        // Angle
        {"Degrees", "°"},
        {"Radians", "rad"}
    };


    public UnitConverterPage()
    {
        InitializeComponent();
        CategoryComboBox.SelectedIndex = 0; // Default to Length
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string category = selectedItem.Content.ToString();
            if (unitsByCategory.ContainsKey(category))
            {
                FromUnitComboBox.ItemsSource = unitsByCategory[category];
                ToUnitComboBox.ItemsSource = unitsByCategory[category];
                FromUnitComboBox.SelectedIndex = 0;
                ToUnitComboBox.SelectedIndex = 1; // Default to a different unit
            }
        }
    }

    private void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(ValueToConvertTextBox.Text, out double valueToConvert) &&
            FromUnitComboBox.SelectedItem is string fromUnit &&
            ToUnitComboBox.SelectedItem is string toUnit &&
            CategoryComboBox.SelectedItem is ComboBoxItem selectedCategory)
        {
            string category = selectedCategory.Content.ToString();
            double result = 0;

            // Perform conversion logic here based on category and units
            switch (category)
            {
                case "Length":
                    result = ConvertLength(valueToConvert, fromUnit, toUnit);
                    break;
                case "Weight (Mass)":
                    result = ConvertWeight(valueToConvert, fromUnit, toUnit);
                    break;
                case "Volume":
                    result = ConvertVolume(valueToConvert, fromUnit, toUnit);
                    break;
                case "Temperature":
                    result = ConvertTemperature(valueToConvert, fromUnit, toUnit);
                    break;
                case "Area":
                    result = ConvertArea(valueToConvert, fromUnit, toUnit);
                    break;
                case "Time":
                    result = ConvertTime(valueToConvert, fromUnit, toUnit);
                    break;
                case "Speed":
                    result = ConvertSpeed(valueToConvert, fromUnit, toUnit);
                    break;
                case "Energy":
                    result = ConvertEnergy(valueToConvert, fromUnit, toUnit);
                    break;
                case "Pressure":
                    result = ConvertPressure(valueToConvert, fromUnit, toUnit);
                    break;
                case "Power":
                    result = ConvertPower(valueToConvert, fromUnit, toUnit);
                    break;
                case "Data (Digital Storage)":
                    result = ConvertData(valueToConvert, fromUnit, toUnit);
                    break;
                case "Angle":
                    result = ConvertAngle(valueToConvert, fromUnit, toUnit);
                    break;
            }

            // Get the symbols for the selected units
            string fromUnitSymbol = unitSymbols.ContainsKey(fromUnit) ? unitSymbols[fromUnit] : fromUnit;
            string toUnitSymbol = unitSymbols.ContainsKey(toUnit) ? unitSymbols[toUnit] : toUnit;

            // Format the output string using the symbols
            ResultTextBlock.Text = $"{valueToConvert} {fromUnitSymbol} = {result:F2} {toUnitSymbol}".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "INVALID INPUT OR UNIT SELECTION".ToUpper();
        }
    }

    // Conversion logic functions (kept as they are, only output formatting changes)
    private double ConvertLength(double value, string from, string to)
    {
        if (from == to) return value;
        double meters = 0;

        // Convert to meters (base unit)
        switch (from)
        {
            case "Meters": meters = value; break;
            case "Feet": meters = value * 0.3048; break;
            case "Inches": meters = value * 0.0254; break;
            case "Kilometers": meters = value * 1000; break;
            case "Miles": meters = value * 1609.34; break;
            case "Centimeters": meters = value * 0.01; break;
            case "Millimeters": meters = value * 0.001; break;
            case "Yards": meters = value * 0.9144; break;
            case "Nautical Miles": meters = value * 1852; break;
        }

        // Convert from meters to the target unit
        switch (to)
        {
            case "Meters": return meters;
            case "Feet": return meters / 0.3048;
            case "Inches": return meters / 0.0254;
            case "Kilometers": return meters / 1000;
            case "Miles": return meters / 1609.34;
            case "Centimeters": return meters / 0.01;
            case "Millimeters": return meters / 0.001;
            case "Yards": return meters / 0.9144;
            case "Nautical Miles": return meters / 1852;
            default: return 0;
        }
    }

    private double ConvertWeight(double value, string from, string to)
    {
        if (from == to) return value;
        double kilograms = 0;

        switch (from)
        {
            case "Kilograms": kilograms = value; break;
            case "Pounds": kilograms = value * 0.453592; break;
            case "Grams": kilograms = value / 1000; break;
            case "Ounces": kilograms = value * 0.0283495; break;
            case "Milligrams": kilograms = value / 1000000; break;
            case "Stones": kilograms = value * 6.35029; break;
            case "Tons (metric)": kilograms = value * 1000; break;
            case "Tons (US)": kilograms = value * 907.185; break;
        }

        switch (to)
        {
            case "Kilograms": return kilograms;
            case "Pounds": return kilograms / 0.453592;
            case "Grams": return kilograms * 1000;
            case "Ounces": return kilograms / 0.0283495;
            case "Milligrams": return kilograms * 1000000;
            case "Stones": return kilograms / 6.35029;
            case "Tons (metric)": return kilograms / 1000;
            case "Tons (US)": return kilograms / 907.185;
            default: return 0;
        }
    }

    private double ConvertVolume(double value, string from, string to)
    {
        if (from == to) return value;
        double liters = 0;

        switch (from)
        {
            case "Liters": liters = value; break;
            case "Gallons (US)": liters = value * 3.78541; break;
            case "Gallons (UK)": liters = value * 4.54609; break;
            case "Cubic Meters": liters = value * 1000; break;
            case "Cubic Feet": liters = value * 28.3168; break;
            case "Cubic Inches": liters = value * 0.0163871; break;
            case "Milliliters": liters = value / 1000; break;
            case "Cups (US)": liters = value * 0.240; break; // Approximation
            case "Pints (US)": liters = value * 0.473176; break;
            case "Quarts (US)": liters = value * 0.946353; break;
        }

        switch (to)
        {
            case "Liters": return liters;
            case "Gallons (US)": return liters / 3.78541;
            case "Gallons (UK)": return liters / 4.54609;
            case "Cubic Meters": return liters / 1000;
            case "Cubic Feet": return liters / 28.3168;
            case "Cubic Inches": return liters / 0.0163871;
            case "Milliliters": return liters * 1000;
            case "Cups (US)": return liters / 0.240; // Approximation
            case "Pints (US)": return liters / 0.473176;
            case "Quarts (US)": return liters / 0.946353;
            default: return 0;
        }
    }

    private double ConvertTemperature(double value, string from, string to)
    {
        if (from == to) return value;
        if (from == "Celsius" && to == "Fahrenheit") return (value * 9 / 5) + 32;
        if (from == "Fahrenheit" && to == "Celsius") return (value - 32) * 5 / 9;
        if (from == "Celsius" && to == "Kelvin") return value + 273.15;
        if (from == "Kelvin" && to == "Celsius") return value - 273.15;
        if (from == "Fahrenheit" && to == "Kelvin") return (value - 32) * 5 / 9 + 273.15;
        if (from == "Kelvin" && to == "Fahrenheit") return (value - 273.15) * 9 / 5 + 32;
        return 0;
    }

    private double ConvertArea(double value, string from, string to)
    {
        if (from == to) return value;
        double squareMeters = 0;

        switch (from)
        {
            case "Square Meters": squareMeters = value; break;
            case "Square Feet": squareMeters = value * 0.092903; break;
            case "Square Inches": squareMeters = value * 0.00064516; break;
            case "Square Kilometers": squareMeters = value * 1000000; break;
            case "Square Miles": squareMeters = value * 2589988.11; break;
            case "Hectares": squareMeters = value * 10000; break;
            case "Acres": squareMeters = value * 4046.86; break;
        }

        switch (to)
        {
            case "Square Meters": return squareMeters;
            case "Square Feet": return squareMeters / 0.092903;
            case "Square Inches": return squareMeters / 0.00064516;
            case "Square Kilometers": return squareMeters / 1000000;
            case "Square Miles": return squareMeters / 2589988.11;
            case "Hectares": return squareMeters / 10000;
            case "Acres": return squareMeters / 4046.86;
            default: return 0;
        }
    }

    private double ConvertTime(double value, string from, string to)
    {
        if (from == to) return value;
        double seconds = 0;

        switch (from)
        {
            case "Seconds": seconds = value; break;
            case "Minutes": seconds = value * 60; break;
            case "Hours": seconds = value * 3600; break;
            case "Days": seconds = value * 86400; break;
            case "Weeks": seconds = value * 604800; break;
            case "Years": seconds = value * 31536000; break; // Assuming 365 days
        }

        switch (to)
        {
            case "Seconds": return seconds;
            case "Minutes": return seconds / 60;
            case "Hours": return seconds / 3600;
            case "Days": return seconds / 86400;
            case "Weeks": return seconds / 604800;
            case "Years": return seconds / 31536000;
            default: return 0;
        }
    }

    private double ConvertSpeed(double value, string from, string to)
    {
        if (from == to) return value;
        double metersPerSecond = 0;

        switch (from)
        {
            case "Meters per second": metersPerSecond = value; break;
            case "Kilometers per hour": metersPerSecond = value * 1000 / 3600; break;
            case "Miles per hour": metersPerSecond = value * 1609.34 / 3600; break;
            case "Knots": metersPerSecond = value * 1.852 / 3.6; break; // Corrected conversion for knots to m/s
        }

        switch (to)
        {
            case "Meters per second": return metersPerSecond;
            case "Kilometers per hour": return metersPerSecond * 3600 / 1000;
            case "Miles per hour": return metersPerSecond * 3600 / 1609.34;
            case "Knots": return metersPerSecond * 3.6 / 1.852; // Corrected conversion for m/s to knots
            default: return 0;
        }
    }

    private double ConvertEnergy(double value, string from, string to)
    {
        if (from == to) return value;
        double joules = 0;

        switch (from)
        {
            case "Joules": joules = value; break;
            case "Kilojoules": joules = value * 1000; break;
            case "Calories": joules = value * 4.184; break;
            case "Kilocalories": joules = value * 4184; break;
            case "British Thermal Units": joules = value * 1055.06; break;
        }

        switch (to)
        {
            case "Joules": return joules;
            case "Kilojoules": return joules / 1000;
            case "Calories": return joules / 4.184;
            case "Kilocalories": return joules / 4184;
            case "British Thermal Units": return joules / 1055.06;
            default: return 0;
        }
    }

    private double ConvertPressure(double value, string from, string to)
    {
        if (from == to) return value;
        double pascals = 0;

        switch (from)
        {
            case "Pascals": pascals = value; break;
            case "Kilopascals": pascals = value * 1000; break;
            case "Bar": pascals = value * 100000; break;
            case "Pounds per square inch": pascals = value * 6894.76; break;
            case "Atmospheres": pascals = value * 101325; break;
        }

        switch (to)
        {
            case "Pascals": return pascals;
            case "Kilopascals": return pascals / 1000;
            case "Bar": return pascals / 100000;
            case "Pounds per square inch": return pascals / 6894.76;
            case "Atmospheres": return pascals / 101325;
            default: return 0;
        }
    }

    private double ConvertPower(double value, string from, string to)
    {
        if (from == to) return value;
        double watts = 0;

        switch (from)
        {
            case "Watts": watts = value; break;
            case "Kilowatts": watts = value * 1000; break;
            case "Horsepower": watts = value * 745.7; break;
        }

        switch (to)
        {
            case "Watts": return watts;
            case "Kilowatts": return watts / 1000;
            case "Horsepower": return watts / 745.7;
            default: return 0;
        }
    }

    private double ConvertData(double value, string from, string to)
    {
        if (from == to) return value;
        double bytes = 0;

        switch (from)
        {
            case "Bits": bytes = value / 8; break;
            case "Bytes": bytes = value; break;
            case "Kilobytes": bytes = value * 1024; break;
            case "Megabytes": bytes = value * 1024 * 1024; break;
            case "Gigabytes": bytes = value * 1024 * 1024 * 1024; break;
            case "Terabytes": bytes = value * 1024 * 1024 * 1024 * 1024; break;
        }

        switch (to)
        {
            case "Bits": return bytes * 8;
            case "Bytes": return bytes;
            case "Kilobytes": return bytes / 1024;
            case "Megabytes": return bytes / (1024 * 1024);
            case "Gigabytes": return bytes / (1024 * 1024 * 1024);
            case "Terabytes": return bytes / (1024 * 1024 * 1024 * 1024L);
            default: return 0;
        }
    }

    private double ConvertAngle(double value, string from, string to)
    {
        if (from == to) return value;
        if (from == "Degrees" && to == "Radians") return value * Math.PI / 180;
        if (from == "Radians" && to == "Degrees") return value * 180 / Math.PI;
        return 0;
    }
}
