using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Lamina.Views;

public sealed partial class AgeCalcPage : Page
{
    public AgeCalcPage()
    {
        InitializeComponent();
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (BirthDatePicker.SelectedDate.HasValue)
        {
            DateTime birthDate = BirthDatePicker.SelectedDate.Value.Date;
            DateTime currentDate = DateTime.Now.Date;

            string ageDifference = CalculateAgeDifference(birthDate, currentDate);
            ResultTextBlock.Text = $"AGE : {ageDifference}".ToUpper();
        }
        else
        {
            ResultTextBlock.Text = "PLEASE SELECT A DATE OF BIRTH".ToUpper();
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