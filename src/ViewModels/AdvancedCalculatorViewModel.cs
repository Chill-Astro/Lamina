using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NCalc;
using Windows.Storage;

namespace Lamina.ViewModels;

public partial class AdvancedCalculatorViewModel : ObservableObject
{
    private string _displayText = "";
    private string _operationText = "";
    private bool _isInverse;
    private string _angleModeText = "DEG";
    private int _cursorPosition;
    private bool _isEditing;

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

    #region History

    private async void LoadHistoryAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var historyFile =
                await localFolder.TryGetItemAsync(HistoryFileName) as StorageFile;

            if (historyFile == null)
                return;

            var historyText = await FileIO.ReadTextAsync(historyFile);

            var historyLines = historyText.Split(
                new[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in historyLines)
            {
                CalculationHistory.Add(line);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error loading history: {ex.Message}");
        }
    }

    private async void SaveHistoryAsync()
    {
        try
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var historyFile = await localFolder.CreateFileAsync(
                HistoryFileName,
                CreationCollisionOption.ReplaceExisting);

            var historyText = string.Join("\n", CalculationHistory);

            await FileIO.WriteTextAsync(historyFile, historyText);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error saving history: {ex.Message}");
        }
    }

    #endregion

    #region Properties

    public string DisplayText
    {
        get => _displayText;
        set
        {
            if (SetProperty(ref _displayText, value))
            {
                if (!_isEditing && _cursorPosition > value.Length)
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

    public string SinDisplay => IsInverse ? "Asin(" : "Sin(";
    public string CosDisplay => IsInverse ? "Acos(" : "Cos(";
    public string TanDisplay => IsInverse ? "Atan(" : "Tan(";

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

    #region Input

    private void InputNumber(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        bool isFunction =
            input.StartsWith("Sin", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Cos", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Tan", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Asin", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Acos", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Atan", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Sqrt", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Ln", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Log10", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("Pow", StringComparison.OrdinalIgnoreCase) ||
            input == "(" ||
            input == ")" ||
            input == "E" ||
            input == "Pi" ||
            input == "π";

        if (_isEditing && !isFunction)
            return;

        if (DisplayText == "Error" || DisplayText == "Division by zero")
        {
            DisplayText = input;
            CursorPosition = input.Length;
            return;
        }

        int position = Math.Clamp(
            CursorPosition,
            0,
            DisplayText.Length);

        DisplayText = DisplayText.Insert(position, input);
        CursorPosition = position + input.Length;
    }

    private void SetOperator(string op)
    {
        if (string.IsNullOrEmpty(op))
            return;

        string nCalcOp = op switch
        {
            "×" => "*",
            "÷" => "/",
            _ => op
        };

        if (DisplayText == "Error" || DisplayText == "Division by zero")
        {
            DisplayText = nCalcOp;
            CursorPosition = nCalcOp.Length;
            return;
        }

        if (DisplayText == "" && nCalcOp == "-")
        {
            DisplayText = "-";
            CursorPosition = 1;
            return;
        }

        int position = Math.Clamp(
            CursorPosition,
            0,
            DisplayText.Length);

        DisplayText = DisplayText.Insert(position, nCalcOp);
        CursorPosition = position + nCalcOp.Length;
    }

    private void InputDecimal()
    {
        if (_isEditing)
            return;

        if (DisplayText == "Error" || DisplayText == "Division by zero")
        {
            DisplayText = ".";
            CursorPosition = 1;
            return;
        }

        int position = Math.Clamp(
            CursorPosition,
            0,
            DisplayText.Length);

        string beforeCursor = DisplayText.Substring(0, position);

        int lastSeparator = beforeCursor.LastIndexOfAny(
            new[] { '+', '-', '*', '/', '(', ')', ',' });

        string currentNumber = lastSeparator < 0
            ? beforeCursor
            : beforeCursor.Substring(lastSeparator + 1);

        if (!currentNumber.Contains("."))
        {
            DisplayText = DisplayText.Insert(position, ".");
            CursorPosition = position + 1;
        }
    }

    private void InputComma()
    {
        if (_isEditing || string.IsNullOrEmpty(DisplayText))
            return;

        int position = Math.Clamp(
            CursorPosition,
            0,
            DisplayText.Length);

        if (position > 0 && DisplayText[position - 1] != ',')
        {
            DisplayText = DisplayText.Insert(position, ",");
            CursorPosition = position + 1;
        }
    }

    private void Paste(string text)
    {
        if (_isEditing || string.IsNullOrWhiteSpace(text))
            return;

        if (DisplayText == "Error" || DisplayText == "Division by zero")
        {
            DisplayText = text;
            CursorPosition = text.Length;
            return;
        }

        int position = Math.Clamp(
            CursorPosition,
            0,
            DisplayText.Length);

        DisplayText = DisplayText.Insert(position, text);
        CursorPosition = position + text.Length;
    }

    #endregion

    #region Editing

    private void Backspace()
    {
        if (_isEditing)
            return;

        if (DisplayText == "Error" || DisplayText == "Division by zero")
        {
            DisplayText = "";
            CursorPosition = 0;
            return;
        }

        if (string.IsNullOrEmpty(DisplayText))
            return;

        int position = Math.Clamp(
            CursorPosition,
            0,
            DisplayText.Length);

        if (position > 0)
        {
            DisplayText = DisplayText.Remove(position - 1, 1);
            CursorPosition = position - 1;
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
        if (_isEditing ||
            string.IsNullOrEmpty(DisplayText) ||
            DisplayText == "Error")
        {
            return;
        }

        if (DisplayText.StartsWith("-(") &&
            DisplayText.EndsWith(")"))
        {
            DisplayText = DisplayText.Substring(
                2,
                DisplayText.Length - 3);
        }
        else
        {
            DisplayText = $"-({DisplayText})";
        }

        CursorPosition = DisplayText.Length;
    }

    private void Reciprocal()
    {
        if (_isEditing ||
            string.IsNullOrEmpty(DisplayText) ||
            DisplayText == "Error")
        {
            return;
        }

        DisplayText = $"1/({DisplayText})";
        CursorPosition = DisplayText.Length;
    }

    private void MoveCursorLeft()
    {
        CursorPosition = Math.Max(0, CursorPosition - 1);
    }

    private void MoveCursorRight()
    {
        CursorPosition = Math.Min(
            DisplayText.Length,
            CursorPosition + 1);
    }

    private void MoveCursorToPosition(int position)
    {
        CursorPosition = Math.Clamp(
            position,
            0,
            DisplayText.Length);
    }

    private void CycleAngleMode()
    {
        AngleModeText = AngleModeText == "DEG"
            ? "RAD"
            : "DEG";
    }

    #endregion

    #region Calculation

    private void Calculate()
    {
        try
        {
            string originalExpression = DisplayText;

            if (string.IsNullOrWhiteSpace(originalExpression))
                return;

            string expressionToEvaluate = PrepareExpression(originalExpression);

            var expression = new Expression(expressionToEvaluate);

            expression.Parameters["Pi"] = Math.PI;
            expression.Parameters["E"] = Math.E;

            bool isDeg = AngleModeText == "DEG";

            /*
             * NCalc 7.1 custom functions.
             *
             * IMPORTANT:
             * args.Evaluate(0) is the correct API for evaluating
             * the first argument.
             */

            expression.Functions["TrigSin"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));
                double angle = isDeg ? DegreesToRadians(value) : value;
                return Math.Sin(angle);
            };

            expression.Functions["TrigCos"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));
                double angle = isDeg ? DegreesToRadians(value) : value;
                return Math.Cos(angle);
            };

            expression.Functions["TrigTan"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));
                double angle = isDeg ? DegreesToRadians(value) : value;

                if (Math.Abs(Math.Cos(angle)) < 1e-12)
                    return double.NaN;

                return Math.Tan(angle);
            };

            expression.Functions["TrigAsin"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));

                if (value < -1 || value > 1)
                    return double.NaN;

                double result = Math.Asin(value);

                return isDeg
                    ? RadiansToDegrees(result)
                    : result;
            };

            expression.Functions["TrigAcos"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));

                if (value < -1 || value > 1)
                    return double.NaN;

                double result = Math.Acos(value);

                return isDeg
                    ? RadiansToDegrees(result)
                    : result;
            };

            expression.Functions["TrigAtan"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));

                double result = Math.Atan(value);

                return isDeg
                    ? RadiansToDegrees(result)
                    : result;
            };

            expression.Functions["TrigSqrt"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));

                return value < 0
                    ? double.NaN
                    : Math.Sqrt(value);
            };

            expression.Functions["TrigLog"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));

                return value <= 0
                    ? double.NaN
                    : Math.Log(value);
            };

            expression.Functions["TrigLog10"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));

                return value <= 0
                    ? double.NaN
                    : Math.Log10(value);
            };

            expression.Functions["Fact"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));
                return GetFactorial(value);
            };

            expression.Functions["Cbrt"] = args =>
            {
                double value = Convert.ToDouble(args.Evaluate(0));
                return Math.Cbrt(value);
            };

            object result = expression.Evaluate();

            if (result == null)
            {
                DisplayText = "Error";
                return;
            }

            double numericResult = Convert.ToDouble(result);

            if (double.IsNaN(numericResult) ||
                double.IsInfinity(numericResult))
            {
                DisplayText = "Error";
                return;
            }

            OperationText = originalExpression + " =";
            DisplayText = numericResult.ToString("G15");
            CursorPosition = DisplayText.Length;

            CalculationHistory.Add(
                $"{OperationText} {DisplayText}");

            SaveHistoryAsync();
        }
        catch (DivideByZeroException)
        {
            DisplayText = "Division by zero";
            CursorPosition = DisplayText.Length;
        }
        catch (Exception ex)
        {
            DisplayText = "Error";
            CursorPosition = DisplayText.Length;

            System.Diagnostics.Debug.WriteLine(
                $"Calculation Error: {ex.Message}");
        }
    }

    private string PrepareExpression(string input)
    {
        string result = input
            .Replace("×", "*")
            .Replace("÷", "/")
            .Trim();

        // Constants.
        result = Regex.Replace(
            result,
            @"\bpi\b",
            "[Pi]",
            RegexOptions.IgnoreCase);

        result = result.Replace("π", "[Pi]");

        // Use word boundaries so scientific notation such as 1e3
        // is not accidentally modified.
        result = Regex.Replace(
            result,
            @"(?<![0-9.])\be\b",
            "[E]",
            RegexOptions.IgnoreCase);

        // Replace longer function names before shorter ones.
        result = Regex.Replace(
            result,
            @"\bLog10\s*\(",
            "TrigLog10(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bAsin\s*\(",
            "TrigAsin(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bAcos\s*\(",
            "TrigAcos(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bAtan\s*\(",
            "TrigAtan(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bSin\s*\(",
            "TrigSin(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bCos\s*\(",
            "TrigCos(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bTan\s*\(",
            "TrigTan(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bSqrt\s*\(",
            "TrigSqrt(",
            RegexOptions.IgnoreCase);

        result = Regex.Replace(
            result,
            @"\bLog\s*\(",
            "TrigLog(",
            RegexOptions.IgnoreCase);

        // Convert simple factorial operands:
        // 5!      -> Fact(5)
        // (2+3)!  -> Fact((2+3))
        result = ReplaceFactorials(result);

        // Auto-close missing parentheses.
        int openBrackets = result.Count(c => c == '(');
        int closeBrackets = result.Count(c => c == ')');

        if (openBrackets > closeBrackets)
        {
            result += new string(
                ')',
                openBrackets - closeBrackets);
        }

        return result;
    }

    private string ReplaceFactorials(string expression)
    {
        while (expression.Contains('!'))
        {
            int factorialIndex = expression.IndexOf('!');

            if (factorialIndex <= 0)
                break;

            int startIndex;

            if (expression[factorialIndex - 1] == ')')
            {
                int depth = 0;
                startIndex = -1;

                for (int i = factorialIndex - 1; i >= 0; i--)
                {
                    if (expression[i] == ')')
                        depth++;
                    else if (expression[i] == '(')
                    {
                        depth--;

                        if (depth == 0)
                        {
                            startIndex = i;
                            break;
                        }
                    }
                }

                if (startIndex < 0)
                    break;
            }
            else
            {
                startIndex = factorialIndex - 1;

                while (startIndex > 0 &&
                       (char.IsDigit(expression[startIndex - 1]) ||
                        expression[startIndex - 1] == '.'))
                {
                    startIndex--;
                }
            }

            string operand = expression.Substring(
                startIndex,
                factorialIndex - startIndex);

            expression =
                expression.Substring(0, startIndex) +
                $"Fact({operand})" +
                expression.Substring(factorialIndex + 1);
        }

        return expression;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static double RadiansToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }

    private double GetFactorial(double n)
    {
        if (n < 0 ||
            double.IsNaN(n) ||
            double.IsInfinity(n))
        {
            return double.NaN;
        }

        if (n != Math.Floor(n))
            return double.NaN;

        if (n > 170)
            return double.PositiveInfinity;

        double factorial = 1;

        for (long i = 2; i <= (long)n; i++)
        {
            factorial *= i;
        }

        return factorial;
    }

    #endregion
}