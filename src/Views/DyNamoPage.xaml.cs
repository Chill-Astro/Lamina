using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NCalc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lamina.Views;

public sealed partial class DyNamoPage : Page
{
    private LaminaScript _currentScript;
    private readonly List<NumberBox> _activeInputs = new();

    public DyNamoPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(
        Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is string path &&
            !string.IsNullOrWhiteSpace(path))
        {
            LoadModule(path);
        }
    }

    private void LoadModule(string path)
    {
        try
        {
            _activeInputs.Clear();

            string json = File.ReadAllText(path);

            _currentScript =
                JsonSerializer.Deserialize<LaminaScript>(json);

            if (_currentScript == null)
            {
                Debug.WriteLine(
                    "DyNamo Load Error: Could not deserialize module.");

                return;
            }

            // Formula Bar
            if (!string.IsNullOrWhiteSpace(_currentScript.UI?.Formula))
            {
                FormulaBar.Message = _currentScript.UI.Formula;
                FormulaBar.Visibility = Visibility.Visible;
                FormulaBar.IsOpen = true;
            }
            else
            {
                FormulaBar.Visibility = Visibility.Collapsed;
                FormulaBar.IsOpen = false;
            }

            // Dynamic Inputs
            InputList.ItemsSource = _currentScript.UI?.Inputs;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"DyNamo Load Error: {ex}");
        }
    }

    private void NumberBox_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not NumberBox numberBox)
            return;

        if (!_activeInputs.Contains(numberBox))
        {
            _activeInputs.Add(numberBox);

            Debug.WriteLine(
                $"DyNamo NumberBox registered: {numberBox.Tag}");
        }
    }

    private async void Calculate_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_currentScript?.Logic == null ||
            string.IsNullOrWhiteSpace(_currentScript.Logic.Output))
        {
            await ShowResult(
                _currentScript?.Logic?.Error ??
                "Calculation Error");

            return;
        }

        try
        {
            // Build the parameter dictionary once.
            var parameters = GetInputParameters();

            Debug.WriteLine(
                $"DyNamo Output: {_currentScript.Logic.Output}");

            foreach (var parameter in parameters)
            {
                Debug.WriteLine(
                    $"DyNamo Parameter: [{parameter.Key}] = {parameter.Value}");
            }

            // Evaluate the Lamina output.
            string result = EvaluateLaminaOutput(
                _currentScript.Logic.Output,
                parameters);

            await ShowResult(result);
        }
        catch (Exception ex)
        {
            // This now shows the REAL error in Visual Studio Debug Output.
            Debug.WriteLine(
                $"DyNamo Calculation Error: {ex}");

            await ShowResult(
                _currentScript.Logic.Error ??
                "Calculation Error");
        }
    }

    private Dictionary<string, object> GetInputParameters()
    {
        var parameters =
            new Dictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var nb in _activeInputs)
        {
            if (nb.Tag is not string key ||
                string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            double value = double.IsNaN(nb.Value)
                ? 0.0
                : nb.Value;

            parameters[key] = value;
        }

        return parameters;
    }

    private static string EvaluateLaminaOutput(
        string output,
        Dictionary<string, object> parameters)
    {
        // Split only on + operators that are OUTSIDE:
        // - quoted strings
        // - parentheses
        //
        // Example:
        //
        // 'GST: ' + ([price] * [tax_rate] / 100)
        //
        // becomes:
        //
        // 1. 'GST: '
        // 2. ([price] * [tax_rate] / 100)
        //
        var parts = SplitTopLevelAddition(output);

        var finalResult = new StringBuilder();

        foreach (string rawPart in parts)
        {
            string part = rawPart.Trim();

            if (string.IsNullOrWhiteSpace(part))
                continue;

            // Quoted text:
            //
            // 'Hello'
            //
            // becomes:
            //
            // Hello
            if (IsQuotedString(part))
            {
                finalResult.Append(
                    UnquoteString(part));

                continue;
            }

            // Mathematical expression.
            var expression = new Expression(part);

            foreach (var parameter in parameters)
            {
                expression.Parameters[parameter.Key] =
                    parameter.Value;
            }

            object result = expression.Evaluate();

            if (result != null)
            {
                finalResult.Append(
                    Convert.ToString(
                        result,
                        CultureInfo.CurrentCulture));
            }
        }

        return finalResult.ToString();
    }

    private static List<string> SplitTopLevelAddition(
        string expression)
    {
        var parts = new List<string>();
        var current = new StringBuilder();

        bool insideString = false;
        int parenthesesDepth = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            // Toggle quoted-string mode.
            if (c == '\'')
            {
                insideString = !insideString;
                current.Append(c);
                continue;
            }

            if (!insideString)
            {
                if (c == '(')
                {
                    parenthesesDepth++;
                    current.Append(c);
                    continue;
                }

                if (c == ')')
                {
                    parenthesesDepth--;

                    if (parenthesesDepth < 0)
                    {
                        throw new InvalidOperationException(
                            "Invalid expression: unmatched closing parenthesis.");
                    }

                    current.Append(c);
                    continue;
                }

                // Only split on + when it is at the top level.
                //
                // This means:
                //
                // [price] + 18
                //
                // inside parentheses is NOT split.
                if (c == '+' && parenthesesDepth == 0)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                    continue;
                }
            }

            current.Append(c);
        }

        if (insideString)
        {
            throw new InvalidOperationException(
                "Invalid expression: unterminated string.");
        }

        if (parenthesesDepth != 0)
        {
            throw new InvalidOperationException(
                "Invalid expression: unmatched parentheses.");
        }

        parts.Add(current.ToString());

        return parts;
    }

    private static bool IsQuotedString(string value)
    {
        if (value.Length < 2)
            return false;

        return value[0] == '\'' &&
               value[value.Length - 1] == '\'';
    }

    private static string UnquoteString(string value)
    {
        return value.Substring(
            1,
            value.Length - 2);
    }

    private async Task ShowResult(string value)
    {
        ResultLabel.Text = "Value Returned = ";
        ResultValueText.Text = value;

        ResultDialog.XamlRoot = this.XamlRoot;

        await ResultDialog.ShowAsync();
    }
}