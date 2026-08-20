using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lamina.ViewModels;

public partial class AdvancedCalculatorViewModel : ObservableRecipient
{
    // [BoringProperty]
    [ObservableProperty]
    private string _displayText = "0";

    // [BoringProperty]
    [ObservableProperty]
    private string _operationText = "";

    // Even more Boring Stuff
    private double _currentNumber;
    private double _previousNumber;
    private double _secondNumber;
    private string _currentOperator = "";
    private bool _isNewNumberInput = true;
    private bool _divisionByZeroOccurred = false;
    private bool _lastOperationWasEquals = false;

    public ObservableCollection<string> CalculationHistory { get; } = new();

    public AdvancedCalculatorViewModel()
    {
    }

    [RelayCommand]
    private void InputNumber(string number)
    {
        if (_lastOperationWasEquals)
        {
            ClearAll(); // Do that to your Reels Feed Pls 🙏
            _lastOperationWasEquals = false;
        }

        if (_isNewNumberInput || DisplayText == "0")
        {
            DisplayText = number;
            _isNewNumberInput = false;
        }
        else if (DisplayText.Length < 16)
        {
            DisplayText += number;
        }

        UpdateCurrentNumber();
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void InputDecimal()
    {
        if (_lastOperationWasEquals)
        {
            ClearAll();
            _lastOperationWasEquals = false;
        }

        if (!DisplayText.Contains("."))
        {
            DisplayText += ".";
            _isNewNumberInput = false;
        }
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void SetOperator(string op)
    {
        if (_lastOperationWasEquals)
        {
            _previousNumber = _currentNumber;
            _lastOperationWasEquals = false;
        }
        else if (!_isNewNumberInput)
        {
            if (!string.IsNullOrEmpty(_currentOperator))
                CalculateIntermediate();
            else
                _previousNumber = _currentNumber;
        }

        _currentOperator = op;
        OperationText = $"{_previousNumber} {op} ";
        _isNewNumberInput = true;
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void Calculate()
    {
        if (string.IsNullOrEmpty(_currentOperator) || _divisionByZeroOccurred) return;

        if (!_lastOperationWasEquals)
            _secondNumber = _currentNumber;
        else
            _previousNumber = _currentNumber;

        double result = PerformMath(_previousNumber, _secondNumber, _currentOperator);

        if (!_divisionByZeroOccurred)
        {
            OperationText = $"{_previousNumber} {_currentOperator} {_secondNumber} =";
            DisplayText = result.ToString("G15");
            CalculationHistory.Add($"{OperationText} {DisplayText}");
            _currentNumber = result;
            _lastOperationWasEquals = true;
            _isNewNumberInput = true;
        }
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void ClearAll()
    {
        DisplayText = "0";
        OperationText = "";
        _currentNumber = 0;
        _previousNumber = 0;
        _currentOperator = "";
        _isNewNumberInput = true;
        _divisionByZeroOccurred = false;
        _lastOperationWasEquals = false;
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void Backspace()
    {
        if (_isNewNumberInput || DisplayText == "0") return;

        DisplayText = DisplayText.Length > 1 ? DisplayText[..^1] : "0";
        if (DisplayText == "-") DisplayText = "0";

        UpdateCurrentNumber();
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void Percent()
    {
        if (!double.TryParse(DisplayText.Replace(",", ""), out double value)) return;

        if (!string.IsNullOrEmpty(_currentOperator))
        {
            // Behind the Scenes of % Button :
            // For Addition and Subtraction: Calculate percentage relative to the first number            
            if (_currentOperator == "+" || _currentOperator == "-")
            {
                _currentNumber = _previousNumber * (value / 100.0);
            }
            // For Multiplication and Division: Just treat it as a decimal            
            else if (_currentOperator == "×" || _currentOperator == "÷")
            {
                _currentNumber = value / 100.0;
            }

            // Update the display and operation string to show the calculated value
            DisplayText = _currentNumber.ToString("G15");
            OperationText = $"{_previousNumber} {_currentOperator} {DisplayText}";
        }
        else
        {
            // If no operator is active, just turn the current number into its decimal form            
            _currentNumber = value / 100.0;
            DisplayText = _currentNumber.ToString("G15");
            OperationText = $"{value}%";
        }

        _isNewNumberInput = true; // Ready for the next number or Equals
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void ClearEntry() => DisplayText = "0";

    // [BoilerPlateCommand]
    [RelayCommand]
    private void Square()
    {
        if (double.TryParse(DisplayText, out double value))
        {
            double result = value * value;
            OperationText = $"sqr({value})";
            DisplayText = result.ToString("G15");
            _currentNumber = result;
            _isNewNumberInput = true;
        }
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void SquareRoot()
    {
        if (double.TryParse(DisplayText, out double value))
        {
            if (value < 0)
            {
                DisplayText = "Invalid Input";
                OperationText = $"sqrt({value})";
                return;
            }
            double result = Math.Sqrt(value);
            OperationText = $"sqrt({value})";
            DisplayText = result.ToString("G15");
            _currentNumber = result;
            _isNewNumberInput = true;
        }
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void CubeRoot()
    {
        if (double.TryParse(DisplayText, out double value))
        {
            double result = Math.Pow(value, 1.0 / 3.0);
            OperationText = $"cbrt({value})";
            DisplayText = result.ToString("G15");
            _currentNumber = result;
            _isNewNumberInput = true;
        }
    }

    // [BoilerPlateCommand]
    [RelayCommand]
    private void Paste(string n)
    {
        // TryParse to make sure it's actually a number
        if (double.TryParse(n, out double value))
        {
            // Use the Property (DisplayText), not the field (_displayText)
            DisplayText = value.ToString("G15");
            OperationText = "";

            // Also update the internal state so further math works
            _currentNumber = value;
            _isNewNumberInput = true; // Set to true so the next digit typed clears the paste
            _lastOperationWasEquals = false;
            _divisionByZeroOccurred = false;
        }
    }

    private void UpdateCurrentNumber()
    {
        if (double.TryParse(DisplayText.Replace(",", ""), out double val))
        {
            _currentNumber = val;
            if (!DisplayText.Contains("."))
                DisplayText = _currentNumber.ToString("N0");
        }
    }

    private void CalculateIntermediate()
    {
        _currentNumber = PerformMath(_previousNumber, _currentNumber, _currentOperator);
        _previousNumber = _currentNumber;
        DisplayText = _currentNumber.ToString("G15");
        _isNewNumberInput = true;
    }

    private double PerformMath(double n1, double n2, string op)
    {
        _divisionByZeroOccurred = false;
        double res = op switch
        {
            "+" => n1 + n2,
            "-" => n1 - n2,
            "×" => n1 * n2,
            "÷" => n2 == 0 ? TriggerDivError() : n1 / n2,
            "^" => Math.Pow(n1, n2),
            _ => n2
        };
        return _divisionByZeroOccurred ? 0 : Math.Round(res, 15);
    }

    private double TriggerDivError() // U suck at Math. Once a Legend said 0/0 is NaN.
    {
        _divisionByZeroOccurred = true;
        DisplayText = "Division By 0 Not Defined";
        OperationText = "NaN is NaN and Not a Number ❌"; // Education isn't Optional, Kids.
        return 0;
    }
}