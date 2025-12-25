using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace Lamina.Views;

public sealed partial class BaseConverterPage : Page
{
    public BaseConverterPage()
    {
        InitializeComponent();
    }

    private void ConvertButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(InputTextBox.Text) || FromBaseComboBox.SelectedItem == null || ToBaseComboBox.SelectedItem == null)
        {
            ResultTextBlock.Text = "Please enter a number and select both bases.";
            return;
        }

        if (!(FromBaseComboBox.SelectedItem is ComboBoxItem fromBaseItem) || !(ToBaseComboBox.SelectedItem is ComboBoxItem toBaseItem))
        {
            ResultTextBlock.Text = "Error in base selection.";
            return;
        }

        // Extract the base number from the ComboBoxItem content (assuming format like "Binary (2)")
        // Added null checks and improved parsing robustness slightly
        int fromBase;
        string fromBaseContent = fromBaseItem.Content?.ToString();
        if (string.IsNullOrEmpty(fromBaseContent) || !TryExtractBase(fromBaseContent, out fromBase))
        {
            ResultTextBlock.Text = "Invalid 'From' base format.";
            return;
        }

        int toBase;
        string toBaseContent = toBaseItem.Content?.ToString();
        if (string.IsNullOrEmpty(toBaseContent) || !TryExtractBase(toBaseContent, out toBase))
        {
            ResultTextBlock.Text = "Invalid 'To' base format.";
            return;
        }


        string inputNumber = InputTextBox.Text.ToUpper(); // Handle hexadecimal A-F

        try
        {
            // Use Convert.ToInt64 to convert from the source base to decimal (long)
            long decimalValue = Convert.ToInt64(inputNumber, fromBase);
            // Use Convert.ToString to convert from decimal (long) to the target base
            string result = Convert.ToString(decimalValue, toBase).ToUpper();
            ResultTextBlock.Text = result;
        }
        catch (FormatException)
        {
            // Catch FormatException specifically for invalid input characters for the given base
            ResultTextBlock.Text = $"Invalid input '{InputTextBox.Text}' for base {fromBase}.";
        }
        catch (OverflowException)
        {
            // Catch OverflowException if the number is too large for long
            ResultTextBlock.Text = $"Input number is too large for conversion.";
        }
        catch (ArgumentOutOfRangeException)
        {
            // Catch ArgumentOutOfRangeException if base is less than 2 or greater than 36
            ResultTextBlock.Text = $"Invalid base value. Bases must be between 2 and 36.";
        }
        catch (Exception ex)
        {
            // Catch any other unexpected errors
            ResultTextBlock.Text = $"An unexpected error occurred: {ex.Message}";
        }
    }

    // Helper method to safely extract the base number from the ComboBoxItem content
    private bool TryExtractBase(string content, out int baseValue)
    {
        baseValue = 0;
        try
        {
            int startIndex = content.LastIndexOf('(');
            int endIndex = content.LastIndexOf(')');
            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                string baseString = content.Substring(startIndex + 1, endIndex - startIndex - 1);
                return int.TryParse(baseString, out baseValue);
            }
        }
        catch (Exception)
        {
            // Ignore exceptions during parsing, just return false
        }
        return false;
    }
}
