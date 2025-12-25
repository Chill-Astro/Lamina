// File: Views/CalculatorPage.xaml.cs
using Lamina.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Microsoft.UI.Xaml.Automation.Peers;
using System.Collections.Generic;
using System;
using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media; // Added for SolidColorBrush

namespace Lamina.Views;

public sealed partial class CalculatorPage : Page
{
    public CalculatorPage()
    {
        ViewModel = App.GetService<CalculatorViewModel>();
        InitializeComponent();
        ClearCalculator();
        this.Focus(FocusState.Programmatic);
        this.Loaded += CalculatorPage_Loaded;
        this.KeyDown += CalculatorPage_KeyDown;
    }

    public CalculatorViewModel ViewModel
    {
        get;
    }

    private double _currentNumber;
    private double _previousNumber;
    private double _secondNumber;
    private string _currentOperator = "";
    private bool _isNewNumberInput = true;
    private string _operationString = "";
    private bool _divisionByZeroOccurred = false;
    private List<string> _calculationHistory = new List<string>();
    private bool _lastOperationWasEquals = false; // Flag to track if the last operation was equals

    private void ClearCalculator()
    {
        DisplayTextBlock.Text = "0";
        OperationTextBlock.Text = "";
        _currentNumber = 0;
        _previousNumber = 0;
        _secondNumber = 0;
        _currentOperator = "";
        _isNewNumberInput = true;
        _operationString = "";
        _divisionByZeroOccurred = false;
        _lastOperationWasEquals = false; // Reset flag
    }

    private void NumberButton_Click(object sender, RoutedEventArgs e)
    {
        Button clickedButton = (Button)sender;
        string buttonContent = clickedButton.Content.ToString();

        // If the last operation was equals and a number is clicked, start a new calculation
        if (_lastOperationWasEquals)
        {
            ClearCalculator(); // Clear everything to start fresh
            DisplayTextBlock.Text = buttonContent;
            _isNewNumberInput = false;
            _lastOperationWasEquals = false; // Reset flag
            if (double.TryParse(DisplayTextBlock.Text, out double initialNumber))
            {
                _currentNumber = initialNumber;
            }
            return; // Exit the method after starting a new calculation
        }

        if (_isNewNumberInput || DisplayTextBlock.Text == "0")
        {
            DisplayTextBlock.Text = buttonContent;
            _isNewNumberInput = false;
        }
        else
        {
            if (DisplayTextBlock.Text.Length < 16) // Limit display length
            {
                DisplayTextBlock.Text += buttonContent;
            }
        }

        // Only update _currentNumber and format if there is no decimal point
        if (!DisplayTextBlock.Text.Contains("."))
        {
            if (DisplayTextBlock.Text.Length > 0 && DisplayTextBlock.Text != "N/A" && DisplayTextBlock.Text != "Not Defined")
            {
                if (double.TryParse(DisplayTextBlock.Text, out double currentNumber))
                {
                    _currentNumber = currentNumber;
                    DisplayTextBlock.Text = currentNumber.ToString("N0"); // Format with commas for integers
                }
            }
        }
        else
        {
            // If there is a decimal, just parse for calculation but do not format the display
            if (double.TryParse(DisplayTextBlock.Text, out double currentNumber))
            {
                _currentNumber = currentNumber;
            }
        }
    }

    private void DecimalButton_Click(object sender, RoutedEventArgs e)
    {
        // If the last operation was equals and decimal is clicked, start with "0."
        if (_lastOperationWasEquals)
        {
            ClearCalculator();
            DisplayTextBlock.Text = "0.";
            _isNewNumberInput = false;
            _lastOperationWasEquals = false;
            return;
        }

        if (!DisplayTextBlock.Text.Contains("."))
        {
            if (DisplayTextBlock.Text.Length < 16) // Limit display length
            {
                DisplayTextBlock.Text += ".";
            }
        }
        _isNewNumberInput = false;
    }

    private void OperatorButton_Click(object sender, RoutedEventArgs e)
    {
        var clickedButton = (Button)sender;
        var operatorContent = clickedButton.Content.ToString();
        var standardOperator = "";

        // Map button content to standard operator symbols
        if (operatorContent == "\uE94A" || operatorContent == "÷") // Division symbol (handle both if needed)
        {
            standardOperator = "÷";
        }
        else if (operatorContent == "\uE947" || operatorContent == "×") // Multiplication symbol
        {
            standardOperator = "×";
        }
        else if (operatorContent == "\uE949" || operatorContent == "-") // Subtraction symbol
        {
            standardOperator = "-";
        }
        else if (operatorContent == "\uE948" || operatorContent == "+") // Addition symbol
        {
            standardOperator = "+";
        }
        else if (operatorContent == "^") // Power symbol
        {
            standardOperator = "^";
        }
        // Add other operators if necessary

        var operatorSymbolForDisplay = GetOperatorSymbol(standardOperator);


        // If the last operation was equals, the current displayed number is the result
        if (_lastOperationWasEquals)
        {
            if (double.TryParse(DisplayTextBlock.Text, out double result))
            {
                _previousNumber = result; // The result becomes the first number for the next operation
                _currentOperator = standardOperator;
                // Removed the redundant lines that were causing issues around line 160
                _isNewNumberInput = true;
                _lastOperationWasEquals = false; // Reset flag
                DisplayTextBlock.Text = "0"; // Clear display for new number input
            }
            // Removed return here to allow the code after the if block to execute
        }
        // Added else if condition to prevent unintended execution after the if block when returning
        else if (!_isNewNumberInput)
        {
            if (_currentOperator != "" && _currentOperator != "=") // Perform intermediate calculation if there was a pending operation
            {
                CalculateIntermediateResult();
                // _previousNumber is updated in CalculateIntermediateResult
            }
            else // If no pending operator or it was equals, set the current number as the previous
            {
                _previousNumber = _currentNumber;
            }
        }
        // If _isNewNumberInput is true and an operator is clicked, it means the user might be changing the operator
        else if (_operationString.EndsWith(" ")) // Check if the operation string ends with an operator followed by a space
        {
            // Replace the last operator in the operation string
            int lastOperatorIndex = _operationString.TrimEnd().LastIndexOf(' ');
            if (lastOperatorIndex != -1)
            {
                _operationString = _operationString.Substring(0, lastOperatorIndex + 1) + GetOperatorSymbol(standardOperator) + " ";
                OperationTextBlock.Text = _operationString;
                _currentOperator = standardOperator;
                return; // Operator changed, no further action needed
            }
        }
        else // This case handles clicking an operator as the very first input after clearing or launching
        {
            _previousNumber = _currentNumber; // Set the current number (which is likely 0) as the previous
        }

        // These lines now correctly update the operation string and display after handling all scenarios
        _operationString = $"{_previousNumber} {operatorSymbolForDisplay} ";
        OperationTextBlock.Text = _operationString;

        _currentOperator = standardOperator;
        _isNewNumberInput = true;
        DisplayTextBlock.Text = "0"; // Clear display for the next number
        _lastOperationWasEquals = false; // Ensure flag is false after an operator is clicked
    }


    private void EqualsButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentOperator != "" && _currentOperator != "=" && !_divisionByZeroOccurred)
        {
            _secondNumber = _currentNumber; // The current displayed number is the second number
            CalculateFinalResult();
            _lastOperationWasEquals = true; // Set the flag after equals
        }
        else if (_currentOperator == "=" && !_divisionByZeroOccurred) // Handle pressing equals multiple times after a calculation
        {
            // If equals is pressed again, repeat the last operation with the result and the second number
            double result = PerformOperation(_previousNumber, _secondNumber, GetOperatorSymbol(_currentOperator)); // Use the last operator
            string operatorSymbol = GetOperatorSymbol(_currentOperator);
            string calculationString = $"{_previousNumber} {operatorSymbol} {_secondNumber} = {result}";

            if (!_divisionByZeroOccurred)
            {
                OperationTextBlock.Text = $"{_previousNumber} {operatorSymbol} {_secondNumber} =";
                // Consider using a consistent format specifier, e.g., "G" for general or remove it
                DisplayTextBlock.Text = result.ToString(); // Display the new result
                _calculationHistory.Add(calculationString); // Add to history
                _previousNumber = result; // The new result becomes the previous number for potential further equals presses
            }
            else
            {
                OperationTextBlock.Text = ""; // Clear operation text on error
            }
            _lastOperationWasEquals = true; // Keep flag true
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        ClearCalculator();
    }

    private void ClearEntryButton_Click(object sender, RoutedEventArgs e)
    {
        DisplayTextBlock.Text = "0";
        _currentNumber = 0;
        _isNewNumberInput = true;
        // Do not reset _previousNumber or _currentOperator here if you want CE to only clear the current input
    }

    private void BackspaceButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isNewNumberInput || DisplayTextBlock.Text == "0") return; // Cannot backspace an empty or initial display

        if (DisplayTextBlock.Text.Length > 1)
        {
            DisplayTextBlock.Text = DisplayTextBlock.Text.Substring(0, DisplayTextBlock.Text.Length - 1);
        }
        else
        {
            DisplayTextBlock.Text = "0";
            _isNewNumberInput = true; // If only one character was left, now input is new
        }
        // Handle the case if the remaining text is just "-"
        if (DisplayTextBlock.Text == "-") DisplayTextBlock.Text = "0";

        // Update _currentNumber after backspace
        if (double.TryParse(DisplayTextBlock.Text, out double currentNumber))
        {
            _currentNumber = currentNumber;
            // DisplayTextBlock.Text = currentNumber.ToString(""); // Avoid reformatting here
        }
        else // Handle cases where parsing fails (e.g., just a decimal point left)
        {
            _currentNumber = 0; // Or handle as needed
        }
    }

    private void CalculateIntermediateResult()
    {
        if (_currentOperator != "" && _currentOperator != "=" && !_divisionByZeroOccurred)
        {
            _currentNumber = PerformOperation(_previousNumber, _currentNumber, _currentOperator);
            // DisplayTextBlock.Text = _currentNumber.ToString("N0"); // Update display with intermediate result
            _previousNumber = _currentNumber; // The result becomes the new previous number for the next operation
            _isNewNumberInput = true; // Ready for a new number input
            // The operation string is updated in OperatorButton_Click
        }
    }

    private void CalculateFinalResult()
    {
        if (_currentOperator != "" && _currentOperator != "=" && !_divisionByZeroOccurred)
        {
            _secondNumber = _currentNumber; // The current displayed number is the second number
            double result = PerformOperation(_previousNumber, _secondNumber, _currentOperator);
            string operatorSymbol = GetOperatorSymbol(_currentOperator);
            string calculationString = $"{_previousNumber} {operatorSymbol} {_secondNumber} = {result}";

            if (!_divisionByZeroOccurred)
            {
                OperationTextBlock.Text = $"{_previousNumber} {operatorSymbol} {_secondNumber} =";
                // Consider using a consistent format specifier, e.g., "G" for general or remove it
                DisplayTextBlock.Text = result.ToString();
                _calculationHistory.Add(calculationString);

                // _previousNumber is updated to the result in EqualsButton_Click
            }
            else
            {
                // OperationTextBlock.Text is cleared in PerformOperation on division by zero
                // DisplayTextBlock.Text is set to "Not Defined" in PerformOperation on division by zero
                _currentOperator = ""; // Reset operator on error
            }

            // _currentOperator is set to "=" in EqualsButton_Click
            // _previousNumber is updated to the result in EqualsButton_Click
            _isNewNumberInput = true; // Ready for a new number input
        }
    }

    private double PerformOperation(double num1, double num2, string operatorSymbol)
    {
        _divisionByZeroOccurred = false; // Reset flag at the start of each operation

        double result;
        switch (operatorSymbol)
        {
            case "+":
                result = num1 + num2;
                break;
            case "-":
                result = num1 - num2;
                break;
            case "×":
                result = num1 * num2;
                break;
            case "÷":
                if (num2 == 0)
                {
                    OperationTextBlock.Text = "";
                    DisplayTextBlock.Text = "Not Defined";
                    _divisionByZeroOccurred = true;
                    _currentOperator = ""; // Reset operator on error
                    _isNewNumberInput = true; // Ready for new input
                    _previousNumber = 0; // Reset previous number on error
                    return double.NaN; // Return NaN or another indicator for error
                }
                result = num1 / num2;
                break;
            case "^":
                result = Math.Pow(num1, num2);
                break;
            default:
                result = num2; // Should not happen if operator is handled
                break;
        }

        // Round the result to 15 decimal places to avoid floating-point precision issues
        return Math.Round(result, 15);
    }


    private string GetOperatorSymbol(string operatorContent)
    {
        // This method seems redundant if operatorContent is already the standard symbol
        // but keeping it for consistency if it's used elsewhere for mapping.
        return operatorContent; // Assuming operatorContent is already the correct symbol
    }

    private void PercentButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentOperator != "")
        {
            if (_currentOperator == "+" || _currentOperator == "-")
            {
                _currentNumber = _previousNumber * (_currentNumber / 100.0); // Ensure division by 100.0 for precision
            }
            else if (_currentOperator == "×" || _currentOperator == "÷")
            {
                _currentNumber /= 100.0; // Use compound assignment for clarity
            }

            DisplayTextBlock.Text = _currentNumber.ToString("N0");
            OperationTextBlock.Text = _operationString + _currentNumber.ToString("N0");
            _isNewNumberInput = true;
        }
        else
        {
            _currentNumber /= 100.0; // Use compound assignment for clarity
            DisplayTextBlock.Text = _currentNumber.ToString("N0");
            OperationTextBlock.Text = $"{_currentNumber * 100}%";
            _isNewNumberInput = true;
        }
    }
    private void SquareButton_Click(object sender, RoutedEventArgs e)
    {
        OperationTextBlock.Text = $"sqr({_currentNumber})";
        _currentNumber = Math.Pow(_currentNumber, 2);
        DisplayTextBlock.Text = _currentNumber.ToString("N0");
        _isNewNumberInput = true;
    }
    private void PowerButton_Click(object sender, RoutedEventArgs e)
    {
        _isNewNumberInput = true;
        _previousNumber = _currentNumber;
        _currentOperator = "^";
        _operationString = $"{_previousNumber}^";
        OperationTextBlock.Text = _operationString;
        DisplayTextBlock.Text = "0";
    }
    private void SquareRootButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentNumber < 0)
        {
            DisplayTextBlock.Text = "Invalid input for √";
        }
        else
        {
            OperationTextBlock.Text = $"√({_currentNumber})";
            _currentNumber = Math.Sqrt(_currentNumber);
            DisplayTextBlock.Text = _currentNumber.ToString(); // No format specifier
        }
        _isNewNumberInput = true;
    }
    private void CubeRootButton_Click(object sender, RoutedEventArgs e)
    {
        OperationTextBlock.Text = $"³√({_currentNumber})";
        _currentNumber = Math.Cbrt(_currentNumber);
        DisplayTextBlock.Text = _currentNumber.ToString(); // No format specifier
        _isNewNumberInput = true;
    }

    private void CalculatorPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        var isControlPressed = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
        var isAltPressed = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down);
        var isShiftPressed = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);

        // Handle Enter key to behave like the Equals button
        if (e.Key == VirtualKey.Enter && !isControlPressed && !isAltPressed && !isShiftPressed)
        {
            EqualsButton_Click(this, null); // Call the Equals button click handler
            e.Handled = true; // Mark as handled
            return;
        }
        // Handle Shift + 5 (e.g., for the '%' operator)
        if (e.Key == VirtualKey.Number5 && isShiftPressed)
        {
            PercentButton_Click(this, null); // Call the Percent button click handler
            e.Handled = true; // Mark as handled
            return;
        }

        // Handle Shift + 8 (e.g., for the multiplication operator '×')
        if (e.Key == VirtualKey.Number8 && isShiftPressed)
        {
            OperatorButton_Click(FindButtonByContent("\uE947"), null); // Call the multiplication button click handler
            e.Handled = true; // Mark as handled
            return;
        }

        // Handle other key combinations
        if (isControlPressed && e.Key == VirtualKey.H)
        {
            HistoryButton_Click(this, new RoutedEventArgs());
            e.Handled = true; // Mark as handled
            return;
        }

        if (e.Key == VirtualKey.C)
        {
            ClearButton_Click(this, null);
            e.Handled = true; // Mark as handled
            return;
        }

        if (e.Key == VirtualKey.Back)
        {
            BackspaceButton_Click(this, null);
            e.Handled = true; // Mark as handled
            return;
        }

        if (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9)
        {
            string number = (e.Key - VirtualKey.Number0).ToString();
            NumberButton_Click(FindButtonByContent(number), null);
            e.Handled = true; // Mark as handled
            return;
        }
        else if (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9)
        {
            string number = (e.Key - VirtualKey.NumberPad0).ToString();
            NumberButton_Click(FindButtonByContent(number), null);
            e.Handled = true; // Mark as handled
            return;
        }

        if (e.Key == (VirtualKey)0xBE)
        {
            DecimalButton_Click(this, null);
            e.Handled = true; // Mark as handled
            return;
        }

        // Handle Enter key only when no modifiers are pressed
        if (e.Key == (VirtualKey)0xBB)
        {
            EqualsButton_Click(this, null);
            e.Handled = true;
            return;
        }
        if (e.Key == (VirtualKey)0xBB)
        {
            OperatorButton_Click(FindButtonByContent("\uE948"), null);
            e.Handled = true;
            return;
        }
        if (e.Key == (VirtualKey)0xBD)
        {
            OperatorButton_Click(FindButtonByContent("\uE949"), null);
            e.Handled = true;
            return;
        }
        if (e.Key == VirtualKey.Multiply || (e.Key == VirtualKey.Number8 && isShiftPressed))
        {
            OperatorButton_Click(FindButtonByContent("×"), null);
            e.Handled = true;
            return;
        }
        if (e.Key == (VirtualKey)0xBF)
        {
            OperatorButton_Click(FindButtonByContent("\uE94A"), null);
            e.Handled = true;
            return;
        }
    }


    private Button FindButtonByContent(string content)
    {
        foreach (var element in FindVisualChildren<Button>(this))
        {
            if (element.Content?.ToString() == content)
            {
                return element;
            }
        }
        return null; // Return null if no matching button is found
    }

    // Helper method to find visual children (keep this as is)
    private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }
                // Recursively search children of children
                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
    private void CalculatorPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Focus(FocusState.Programmatic); // Ensure the page has focus
        ContentGrid.Focus(FocusState.Programmatic); // Optionally focus the ContentGrid
    }
    private async void HistoryButton_Click(object sender, RoutedEventArgs e)
    {
        // Check if a ContentDialog is already open by tracking it manually
        if (_isDialogOpen)
        {
            return; // Prevent opening another dialog
        }

        _isDialogOpen = true; // Set the flag to indicate a dialog is open

        try
        {
            // Set the calculation history after creating the HistoryPage
            var historyPage = new HistoryPage();
            historyPage.SetHistory(_calculationHistory); // Assuming a method to set history exists

            // Create a ContentDialog to display the HistoryPage as a pop-up
            var dialog = new ContentDialog
            {
                Content = historyPage,
                CloseButtonText = "Close",
                FullSizeDesired = false,
                XamlRoot = App.MainWindow.Content.XamlRoot, // Ensure XamlRoot is set to the main window
                RequestedTheme = this.RequestedTheme, // Match the theme of the current page
            };

            // Center the dialog explicitly
            dialog.HorizontalAlignment = HorizontalAlignment.Center;
            dialog.VerticalAlignment = VerticalAlignment.Center;
            dialog.MaxHeight = App.MainWindow.Bounds.Height * 0.8; // Set max height to 80% of the window height
            dialog.MinHeight = App.MainWindow.Bounds.Height * 0.5; // Set min height to 50% of the window height
            dialog.MinWidth = App.MainWindow.Bounds.Width * 0.8; // Set a minimum width for the dialog

            // Show the dialog
            await dialog.ShowAsync();
        }
        finally
        {
            _isDialogOpen = false; // Reset the flag when the dialog is closed
        }
    }

    // Add a private field to track if a dialog is open
    private bool _isDialogOpen = false;

}