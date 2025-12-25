using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class DateDifferencePage : Page
{
    public DateDifferencePage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
        {
            DateTime startDate = StartDatePicker.SelectedDate.Value.Date;
            DateTime endDate = EndDatePicker.SelectedDate.Value.Date;

            TimeSpan difference = endDate - startDate;
            int daysDifference = (int)difference.TotalDays;

            if (daysDifference > 365)
            {
                string formattedDifference = CalculateAgeDifference(startDate, endDate);
                ResultTextBlock.Text = $"Difference: {formattedDifference}".ToUpper();
            }
            else if (daysDifference > 30)
            {
                int months = (int)(daysDifference / 30.44); // Average days in a month
                int remainingDays = daysDifference % 30;
                ResultTextBlock.Text = $"Difference: {months} Months {remainingDays} Days".ToUpper();
            }
            else
            {
                ResultTextBlock.Text = $"Difference: {daysDifference} Days".ToUpper();
            }
        }
        else
        {
            ResultTextBlock.Text = "PLEASE SELECT BOTH START AND END DATES".ToUpper();
        }
    }

    private string CalculateAgeDifference(DateTime startDate, DateTime endDate)
    {
        int years = endDate.Year - startDate.Year;
        int months = endDate.Month - startDate.Month;
        int days = endDate.Day - startDate.Day;

        if (days < 0)
        {
            months--;
            days += DateTime.DaysInMonth(endDate.Year, endDate.Month == 1 ? 12 : endDate.Month - 1);
        }

        if (months < 0)
        {
            years--;
            months += 12;
        }

        return $"{years} Years {months} Months {days} Days";
    }
}