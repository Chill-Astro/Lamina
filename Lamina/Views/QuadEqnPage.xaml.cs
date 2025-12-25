// File: Views/QuadEqnPage.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System; // Required for Math.Sqrt and Math.Pow

namespace Lamina.Views;

public sealed partial class QuadEqnPage : Page
{
    public QuadEqnPage()
    {
        InitializeComponent();
        // Optionally clear the fields when the page is initialized
        ClearFields();
    }

    // Event handler for the Calculate Roots button click
    private void CalculateRootsButton_Click(object sender, RoutedEventArgs e)
    {
        // Clear previous output and errors
        // We'll set the final output text at the end, so no need to set "Calculating..." here
        double a, b, c;

        // Attempt to parse input values from TextBoxes
        // Use double.TryParse for safer conversion
        bool parseA = double.TryParse(CoefficientATextBox.Text, out a);
        bool parseB = double.TryParse(CoefficientBTextBox.Text, out b);
        bool parseC = double.TryParse(CoefficientCTextBox.Text, out c);

        // Check if all inputs are valid numbers
        if (!parseA || !parseB || !parseC)
        {
            RootsTextBlock.Text = "Invalid input. Please enter valid numbers for coefficients a, b, and c.";
            return; // Stop execution if input is invalid
        }

        // Handle the case where 'a' is 0 (linear equation)
        if (a == 0)
        {
            if (b == 0)
            {
                // If a=0 and b=0, the equation is c = 0
                if (c == 0)
                {
                    RootsTextBlock.Text = "Equation is 0 = 0. Infinite solutions.";
                }
                else
                {
                    RootsTextBlock.Text = $"Equation is {c} = 0. No solution.";
                }
            }
            else
            {
                // If a=0 and b!=0, it's a linear equation: bx + c = 0
                double root = -c / b;
                RootsTextBlock.Text = $"This is a linear equation. Root: x = {root}";
            }
            return; // Stop execution after handling linear or constant cases
        }

        // Calculate the discriminant (Delta)
        double discriminant = (b * b) - (4 * a * c);
        string resultText;

        // Determine the nature of the roots based on the discriminant and format output
        if (discriminant > 0)
        {
            // Two distinct real roots
            double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
            resultText = $"TWO REAL ROOTS : {root1} or {root2}";
        }
        else if (discriminant == 0)
        {
            // One real root (or two equal real roots)
            double root = -b / (2 * a);
            resultText = $"TWO EQUAL ROOTS : {root} or {root}"; // Display the same root twice as per example
        }
        else
        {
            // Two complex conjugate roots
            // As per the requested format for complex roots, display "NO REAL ROOTS"
            resultText = "NO REAL ROOTS";
            // If you wanted to display complex roots, you could use:
            // double realPart = -b / (2 * a);
            // double imaginaryPart = Math.Sqrt(Math.Abs(discriminant)) / (2 * a);
            // resultText = $"Two complex conjugate roots:\nRoot 1 = {realPart} + {imaginaryPart}i\nRoot 2 = {realPart} - {imaginaryPart}i";
        }

        // Display the formatted result in the TextBlock
        RootsTextBlock.Text = resultText;
    }

    // Event handler for the Clear button click (assuming you still want a clear function,
    // even if the button was removed from the simplified XAML. You might re-add it later
    // or trigger ClearFields from another event if needed).
    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        ClearFields();
    }

    // Helper method to clear input and output fields
    private void ClearFields()
    {
        CoefficientATextBox.Text = "";
        CoefficientBTextBox.Text = "";
        CoefficientCTextBox.Text = "";
        RootsTextBlock.Text = "OUTPUT AREA"; // Reset output text
    }
}
