using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NCalc;

namespace Lamina.ViewModels;

public partial class AdvancedCalculatorViewModel : ObservableObject
{
    private string _displayText = "";    
    private string _operationText = "";
    private bool _isInverse;
    private string _angleModeText = "DEG";

    public ObservableCollection<string> CalculationHistory { get; } = new();

    public AdvancedCalculatorViewModel()
    {
        InputNumberCommand = new RelayCommand<string>(InputNumber);
        SetOperatorCommand = new RelayCommand<string>(SetOperator);
        CalculateCommand = new RelayCommand(Calculate);
        ClearAllCommand = new RelayCommand(ClearAll);
        BackspaceCommand = new RelayCommand(Backspace);
        InputDecimalCommand = new RelayCommand(InputDecimal);
        InputCommaCommand = new RelayCommand(InputComma);
        CycleAngleModeCommand = new RelayCommand(CycleAngleMode);
        NegateCommand = new RelayCommand(Negate);
        ReciprocalCommand = new RelayCommand(Reciprocal);
        PasteCommand = new RelayCommand<string>(Paste);
    }

    #region Properties

    public string DisplayText
    {
        get => _displayText;
        set => SetProperty(ref _displayText, value);
    }

    public string OperationText
    {
        get => _operationText;
        set => SetProperty(ref _operationText, value);
    }

    public string AngleModeText
    {
        get => _angleModeText;
        set => SetProperty(ref _angleModeText, value);
    }

    public bool IsInverse
    {
        get => _isInverse;
        set
        {
            if (SetProperty(ref _isInverse, value))
            {
                OnPropertyChanged(nameof(SinLabel));
                OnPropertyChanged(nameof(CosLabel));
                OnPropertyChanged(nameof(TanLabel));
                OnPropertyChanged(nameof(SinDisplay));
                OnPropertyChanged(nameof(CosDisplay));
                OnPropertyChanged(nameof(TanDisplay));
            }
        }
    }

    public string SinLabel => IsInverse ? "Asin(" : "Sin(";
    public string CosLabel => IsInverse ? "Acos(" : "Cos(";
    public string TanLabel => IsInverse ? "Atan(" : "Tan(";
    
    public string SinDisplay => IsInverse ? "sin⁻¹" : "sin";
    public string CosDisplay => IsInverse ? "cos⁻¹" : "cos";
    public string TanDisplay => IsInverse ? "tan⁻¹" : "tan";

    #endregion

    #region Commands

    public ICommand InputNumberCommand { get; }
    public ICommand SetOperatorCommand { get; }
    public ICommand CalculateCommand { get; }
    public ICommand ClearAllCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand InputDecimalCommand { get; }
    public ICommand InputCommaCommand { get; }
    public ICommand CycleAngleModeCommand { get; }
    public ICommand NegateCommand { get; }
    public ICommand ReciprocalCommand { get; }
    public ICommand PasteCommand { get; }

    #endregion

    #region Logic

    private void InputNumber(string input)
    {
        if (DisplayText == "" || DisplayText == "Error")
            DisplayText = input;
        else
            DisplayText += input;
    }

    private void SetOperator(string op)
    {
        string nCalcOp = op switch
        {
            "×" => "*",
            "÷" => "/",
            "^" => "^",
            _ => op
        };

        if ((DisplayText == "" || DisplayText == "Error") && nCalcOp == "-")
            DisplayText = "-";
        else
            DisplayText += nCalcOp;
    }

    private void InputDecimal()
    {
        var parts = DisplayText.Split(new[] { '+', '-', '*', '/', '(', ')', ',' });
        var lastPart = parts.LastOrDefault();
        if (lastPart != null && !lastPart.Contains("."))
            DisplayText += ".";
    }

    private void Backspace()
    {
        if (DisplayText == "Error")
        {
            DisplayText = "";
            return;
        }

        if (DisplayText.Length > 0)
        {
            DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            if (string.IsNullOrEmpty(DisplayText)) DisplayText = "";
        }
    }

    private void ClearAll()
    {
        DisplayText = "";
        OperationText = "";
    }

    private void Negate()
    {
        if (DisplayText == "" || DisplayText == "Error") return;

        // Wrap the existing expression in a negative bracket
        if (DisplayText.StartsWith("-(") && DisplayText.EndsWith(")"))
            DisplayText = DisplayText.Substring(2, DisplayText.Length - 3);
        else
            DisplayText = $"-({DisplayText})";
    }

    private void Reciprocal()
    {
        if (DisplayText == "" || DisplayText == "Error") return;
        DisplayText = $"1/({DisplayText})";
    }

    private void InputComma()
    {
        if (DisplayText != "" && !DisplayText.EndsWith(","))
            DisplayText += ",";
    }

    private void CycleAngleMode()
    {
        AngleModeText = AngleModeText == "DEG" ? "RAD" : "DEG";
    }

    private void Paste(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        
        // Try to parse as a number first
        if (double.TryParse(text, out _))
        {
            DisplayText = text;
        }
        else
        {
            // If not a single number, append the text (allows pasting expressions)
            if (DisplayText == "" || DisplayText == "Error")
                DisplayText = text;
            else
                DisplayText += text;
        }
    }

    private void Calculate()
    {
        try
        {
            string expressionToEvaluate = DisplayText;

            if (string.IsNullOrWhiteSpace(expressionToEvaluate))
                return;

            // Replace operators with NCALC-compatible format
            expressionToEvaluate = expressionToEvaluate
                .Replace("×", "*")
                .Replace("÷", "/")
                .Replace("^", "^");

            // Replace constant names with NCALC parameter references
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bpi\b", "[Pi]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = expressionToEvaluate.Replace("π", "[Pi]");
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\be\b", "[E]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Replace our function names with custom names to avoid NCALC conflicts (case-insensitive)
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bSin\(", "TrigSin(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bCos\(", "TrigCos(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bTan\(", "TrigTan(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bAsin\(", "TrigAsin(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bAcos\(", "TrigAcos(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bAtan\(", "TrigAtan(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bSqrt\(", "TrigSqrt(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bLog\(", "TrigLog(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"\bLog10\(", "TrigLog10(", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Handle Cube Root symbol replacement
            if (expressionToEvaluate.Contains("³√("))
            {
                expressionToEvaluate = expressionToEvaluate.Replace("³√(", "Cbrt(");
            }

            // Auto-close parentheses for NCalc safety
            int openBrackets = expressionToEvaluate.Count(f => f == '(');
            int closeBrackets = expressionToEvaluate.Count(f => f == ')');
            for (int i = 0; i < (openBrackets - closeBrackets); i++)
                expressionToEvaluate += ")";

            // Handle Factorial replacement - handle expressions ending with !
            // This handles both simple numbers like "5!" and expressions like "(5+3)!"
            expressionToEvaluate = System.Text.RegularExpressions.Regex.Replace(
                expressionToEvaluate, @"([0-9.\(\)\+\-\*\/]+)!", "Fact($1)");

            var expression = new Expression(expressionToEvaluate);

            expression.Parameters["Pi"] = Math.PI;
            expression.Parameters["E"] = Math.E;

            bool isDeg = AngleModeText == "DEG";
            expression.EvaluateFunction += (name, args) =>
            {
                try
                {
                    var parameters = args.EvaluateParameters();
                    if (parameters.Length > 0)
                    {
                        double val = Convert.ToDouble(parameters[0]);

                        switch (name)
                        {
                            case "TrigSin":
                                args.Result = Math.Sin(isDeg ? val * Math.PI / 180.0 : val);
                                break;
                            case "TrigCos":
                                args.Result = Math.Cos(isDeg ? val * Math.PI / 180.0 : val);
                                break;
                            case "TrigTan":
                                // Tan is undefined at 90° and 270° (π/2 and 3π/2 in radians)
                                double tanAngle = isDeg ? val * Math.PI / 180.0 : val;
                                double tanCheck = Math.Abs(tanAngle % (Math.PI / 2));
                                if (tanCheck < 1e-10 || Math.Abs(tanCheck - Math.PI / 2) < 1e-10)
                                    args.Result = double.NaN;
                                else
                                    args.Result = Math.Tan(tanAngle);
                                break;
                            case "TrigAsin":
                                if (val < -1 || val > 1)
                                    args.Result = double.NaN;
                                else
                                {
                                    double asin = Math.Asin(val);
                                    args.Result = isDeg ? asin * 180.0 / Math.PI : asin;
                                }
                                break;
                            case "TrigAcos":
                                if (val < -1 || val > 1)
                                    args.Result = double.NaN;
                                else
                                {
                                    double acos = Math.Acos(val);
                                    args.Result = isDeg ? acos * 180.0 / Math.PI : acos;
                                }
                                break;
                            case "TrigAtan":
                                double atan = Math.Atan(val);
                                args.Result = isDeg ? atan * 180.0 / Math.PI : atan;
                                break;
                            case "TrigSqrt":
                                if (val < 0)
                                    args.Result = double.NaN;
                                else
                                    args.Result = Math.Sqrt(val);
                                break;
                            case "TrigLog":
                                if (val <= 0)
                                    args.Result = double.NaN;
                                else
                                    args.Result = Math.Log(val);
                                break;
                            case "TrigLog10":
                                if (val <= 0)
                                    args.Result = double.NaN;
                                else
                                    args.Result = Math.Log10(val);
                                break;
                            case "Fact":
                                args.Result = GetFactorial(val);
                                break;
                            case "Cbrt":
                                args.Result = Math.Pow(val, 1.0 / 3.0);
                                break;
                        }
                    }
                }
                catch
                {
                    args.Result = double.NaN;
                }
            };

            var result = expression.Evaluate();
            
            if (result != null)
            {
                double numericResult;
                if (double.TryParse(result.ToString(), out numericResult))
                {
                    if (double.IsNaN(numericResult) || double.IsInfinity(numericResult))
                    {
                        DisplayText = "Error";
                    }
                    else
                    {
                        OperationText = DisplayText + " =";
                        DisplayText = numericResult.ToString("G15");
                        CalculationHistory.Add($"{OperationText} {DisplayText}");
                    }
                }
                else
                {
                    DisplayText = "Error";
                }
            }
            else
            {
                DisplayText = "Error";
            }
        }
        catch (DivideByZeroException)
        {
            DisplayText = "Division by zero";
        }
        catch (Exception ex)
        {
            DisplayText = "Error";
            System.Diagnostics.Debug.WriteLine($"Calculation Error: {ex.Message}");
        }
    }

    private double GetFactorial(double n)
    {
        if (n < 0 || double.IsInfinity(n) || double.IsNaN(n))
            return double.NaN;
        
        if (n != Math.Floor(n))
            return double.NaN;
            
        if (n > 170) // Prevent stack overflow and overflow
            return double.PositiveInfinity;
            
        long factorial = 1;
        for (long i = 2; i <= (long)n; i++)
        {
            try
            {
                checked
                {
                    factorial *= i;
                }
            }
            catch (OverflowException)
            {
                return double.PositiveInfinity;
            }
        }
        return factorial;
    }

    #endregion
}