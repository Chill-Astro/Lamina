using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NCalc;
using Windows.Storage;
using System.Text.RegularExpressions;

namespace Lamina.ViewModels;

public partial class AdvancedCalculatorViewModel : ObservableObject
{
    private string _displayText = "";    
    private string _operationText = "";
    private bool _isInverse;
    private string _angleModeText = "DEG";
    private int _cursorPosition = 0;
    private bool _isEditing = false;

    public ObservableCollection<string> CalculationHistory { get; } = new();
    private const string HistoryFileName = "advanced_calculator_history.json";

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
        MoveCursorLeftCommand = new RelayCommand(MoveCursorLeft);
        MoveCursorRightCommand = new RelayCommand(MoveCursorRight);
        MoveCursorToPositionCommand = new RelayCommand<int>(MoveCursorToPosition);
        LoadHistoryAsync();
    }

    private async void LoadHistoryAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var historyFile = await localFolder.TryGetItemAsync(HistoryFileName) as StorageFile;
            
            if (historyFile != null)
            {
                var historyText = await FileIO.ReadTextAsync(historyFile);
                var historyLines = historyText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in historyLines)
                {
                    CalculationHistory.Add(line);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
        }
    }

    private async void SaveHistoryAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var historyFile = await localFolder.CreateFileAsync(HistoryFileName, CreationCollisionOption.ReplaceExisting);
            var historyText = string.Join("\n", CalculationHistory);
            await FileIO.WriteTextAsync(historyFile, historyText);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving history: {ex.Message}");
        }
    }

    #region Properties

    public string DisplayText
    {
        get => _displayText;
        set
        {
            if (SetProperty(ref _displayText, value))
            {
                // Adjust cursor position if text changed from button input
                if (value != null && _cursorPosition > value.Length && !_isEditing)
                {
                    _cursorPosition = value.Length;
                    OnPropertyChanged(nameof(CursorPosition));
                }
            }
        }
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

    public int CursorPosition
    {
        get => _cursorPosition;
        set => SetProperty(ref _cursorPosition, value);
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

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
    public ICommand MoveCursorLeftCommand { get; }
    public ICommand MoveCursorRightCommand { get; }
    public ICommand MoveCursorToPositionCommand { get; }

    #endregion

    #region Logic

    private void InputNumber(string input)
    {
        // Allow function names (Sin, Cos, etc.) even when editing
        bool isFunction = input.StartsWith("Sin") || input.StartsWith("Cos") || input.StartsWith("Tan") ||
                         input.StartsWith("Asin") || input.StartsWith("Acos") || input.StartsWith("Atan") ||
                         input.StartsWith("Sqrt") || input.StartsWith("Log") || input.StartsWith("Log10") ||
                         input.StartsWith("Pow") || input == "(" || input == ")" || input == "E" || input == "Pi";
        
        if (_isEditing && !isFunction) return; // Disable number/operator input when editing, but allow functions
        
        if (DisplayText == "Error")
        {
            DisplayText = input;
            CursorPosition = input.Length;
            return;
        }

        if (DisplayText == "")
        {
            DisplayText = input;
            CursorPosition = input.Length;
        }
        else
        {
            int pos = Math.Clamp(CursorPosition, 0, DisplayText.Length);
            DisplayText = DisplayText.Insert(pos, input);
            CursorPosition = pos + input.Length;
        }
    }

    private void SetOperator(string op)
    {
        // Allow operators when editing for expression typing
        // if (_isEditing) return; // Disable button input when editing
        
        string nCalcOp = op switch
        {
            "×" => "*",
            "÷" => "/",
            _ => op
        };

        if (DisplayText == "Error")
        {
            DisplayText = nCalcOp;
            CursorPosition = nCalcOp.Length;
            return;
        }

        if ((DisplayText == "") && nCalcOp == "-")
        {
            DisplayText = "-";
            CursorPosition = 1;
        }
        else
        {
            int pos = Math.Clamp(CursorPosition, 0, DisplayText.Length);
            DisplayText = DisplayText.Insert(pos, nCalcOp);
            CursorPosition = pos + nCalcOp.Length;
        }
    }

    private void InputDecimal()
    {
        if (_isEditing) return; // Disable button input when editing
        
        if (DisplayText == "Error")
        {
            DisplayText = ".";
            CursorPosition = 1;
            return;
        }

        if (DisplayText == "")
        {
            DisplayText = ".";
            CursorPosition = 1;
            return;
        }

        int pos = Math.Clamp(CursorPosition, 0, DisplayText.Length);
        
        // Check if the current number segment already has a decimal
        var parts = DisplayText.Split(new[] { '+', '-', '*', '/', '(', ')', ',' });
        var currentSegment = DisplayText.Substring(0, pos);
        var lastSegmentIndex = currentSegment.LastIndexOfAny(new[] { '+', '-', '*', '/', '(', ')', ',' });
        var segmentToCheck = lastSegmentIndex < 0 ? currentSegment : currentSegment.Substring(lastSegmentIndex + 1);
        
        if (!segmentToCheck.Contains("."))
        {
            DisplayText = DisplayText.Insert(pos, ".");
            CursorPosition = pos + 1;
        }
    }

    private void Backspace()
    {
        if (_isEditing) return; // Disable button input when editing
        
        if (DisplayText == "Error")
        {
            DisplayText = "";
            CursorPosition = 0;
            return;
        }

        if (DisplayText.Length > 0)
        {
            int pos = Math.Clamp(CursorPosition, 0, DisplayText.Length);
            if (pos > 0)
            {
                DisplayText = DisplayText.Remove(pos - 1, 1);
                CursorPosition = pos - 1;
            }
            else
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
                CursorPosition = DisplayText.Length;
            }
            
            if (string.IsNullOrEmpty(DisplayText))
            {
                DisplayText = "";
                CursorPosition = 0;
            }
        }
    }

    private void ClearAll()
    {
        DisplayText = "";
        OperationText = "";
        CursorPosition = 0;
    }

    private void Negate()
    {
        if (_isEditing) return; // Disable button input when editing
        if (DisplayText == "" || DisplayText == "Error") return;

        // Wrap the existing expression in a negative bracket
        if (DisplayText.StartsWith("-(") && DisplayText.EndsWith(")"))
            DisplayText = DisplayText.Substring(2, DisplayText.Length - 3);
        else
            DisplayText = $"-({DisplayText})";
    }

    private void Reciprocal()
    {
        if (_isEditing) return; // Disable button input when editing
        if (DisplayText == "" || DisplayText == "Error") return;
        DisplayText = $"1/({DisplayText})";
    }

    private void InputComma()
    {
        if (_isEditing) return; // Disable button input when editing
        if (DisplayText != "" && !DisplayText.EndsWith(","))
            DisplayText += ",";
    }

    private void CycleAngleMode()
    {
        AngleModeText = AngleModeText == "DEG" ? "RAD" : "DEG";
    }

    private void Paste(string text)
    {
        if (_isEditing) return; // Disable button input when editing
        if (string.IsNullOrWhiteSpace(text)) return;
        
        if (DisplayText == "Error")
        {
            DisplayText = text;
            CursorPosition = text.Length;
            return;
        }

        // Try to parse as a number first
        if (double.TryParse(text, out _))
        {
            int pos = Math.Clamp(CursorPosition, 0, DisplayText.Length);
            DisplayText = DisplayText.Insert(pos, text);
            CursorPosition = pos + text.Length;
        }
        else
        {
            // If not a single number, append the text (allows pasting expressions)
            if (DisplayText == "")
            {
                DisplayText = text;
                CursorPosition = text.Length;
            }
            else
            {
                int pos = Math.Clamp(CursorPosition, 0, DisplayText.Length);
                DisplayText = DisplayText.Insert(pos, text);
                CursorPosition = pos + text.Length;
            }
        }
    }

    private void MoveCursorLeft()
    {
        CursorPosition = Math.Max(0, CursorPosition - 1);
    }

    private void MoveCursorRight()
    {
        CursorPosition = Math.Min(DisplayText?.Length ?? 0, CursorPosition + 1);
    }

    private void MoveCursorToPosition(int position)
    {
        CursorPosition = Math.Clamp(position, 0, DisplayText?.Length ?? 0);
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
                        SaveHistoryAsync();
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