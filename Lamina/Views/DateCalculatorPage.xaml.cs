using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Lamina.Views;

public sealed partial class DateCalculatorPage : Page
{
    public DateCalculatorPage()
    {
        this.InitializeComponent();
        // Initialize with today's date
        var today = DateTimeOffset.Now;
        FromDateDiff.Date = ToDateDiff.Date = StartDateAdd.Date = today;        
        FromDateDiff.DateChanged += (s, e) => LiveUpdateDiff();
        ToDateDiff.DateChanged += (s, e) => LiveUpdateDiff();
        StartDateAdd.DateChanged += (s, e) => LiveUpdateAdd(null, null);

        LiveUpdateDiff();
        LiveUpdateAdd(null, null);
    }

    private void ModeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DifferenceSection == null || AddSubtractSection == null) return;
        bool isDiffMode = ModeSelector.SelectedIndex == 0;
        DifferenceSection.Visibility = isDiffMode ? Visibility.Visible : Visibility.Collapsed;
        AddSubtractSection.Visibility = isDiffMode ? Visibility.Collapsed : Visibility.Visible;
    }

    // Yeah for saving your Hand from Clicking 100 times!
    private void LiveUpdateDiff()
    {
        if (!FromDateDiff.Date.HasValue || !ToDateDiff.Date.HasValue) return;

        DateTime start = FromDateDiff.Date.Value.DateTime;
        DateTime end = ToDateDiff.Date.Value.DateTime;

        if (start == end) { DiffResultText.Text = "Same Dates"; return; }

        // Ensure we always calculate from earlier to later
        if (start > end) { var temp = start; start = end; end = temp; }

        int years = end.Year - start.Year;
        int months = end.Month - start.Month;
        int days = end.Day - start.Day;

        if (days < 0)
        {
            months--;
            days += DateTime.DaysInMonth(start.Year, start.Month);
        }
        if (months < 0)
        {
            years--;
            months += 12;
        }

        string result = "";
        if (years > 0) result += $"{years} year{(years > 1 ? "s" : "")}, ";
        if (months > 0) result += $"{months} month{(months > 1 ? "s" : "")}, ";
        if (days > 0) result += $"{days} day{(days > 1 ? "s" : "")}";

        DiffResultText.Text = result.TrimEnd(',', ' ');
    }

    private void LiveUpdateAdd(object sender, object e)
    {
        if (StartDateAdd == null || !StartDateAdd.Date.HasValue) return;

        DateTime start = StartDateAdd.Date.Value.DateTime;
        int y = (int)YearsInput.Value;
        int m = (int)MonthsInput.Value;
        int d = (int)DaysInput.Value;

        int direction = (SubtractRadio.IsChecked == true) ? -1 : 1;

        try
        {
            DateTime result = start.AddYears(y * direction).AddMonths(m * direction).AddDays(d * direction);
            AddResultText.Text = result.ToString("dddd, MMMM d, yyyy");
        }
        catch { AddResultText.Text = "Out of Range"; }
    }
}