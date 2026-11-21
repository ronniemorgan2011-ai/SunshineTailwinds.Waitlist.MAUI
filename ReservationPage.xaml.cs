namespace SunshineTailwinds.Waitlist.MAUI;

public partial class ReservationPage : ContentPage
{
    public TimeSpan? SelectedTime { get; private set; }

    private readonly TaskCompletionSource<bool> _tcs =
        new();

    public ReservationPage()
    {
        InitializeComponent();

        PopulateTimes();
    }

    private void PopulateTimes()
    {
        var startTime = new TimeSpan(6, 0, 0);
        var endTime = new TimeSpan(14, 0, 0);

        for (var time = startTime;
             time <= endTime;
             time = time.Add(TimeSpan.FromMinutes(15)))
        {
            ReservationTimePicker.Items.Add(
                DateTime.Today
                    .Add(time)
                    .ToString("h:mm tt"));
        }

        ReservationTimePicker.SelectedIndex = 0;
    }

    public Task WaitForCloseAsync()
    {
        return _tcs.Task;
    }

    private async void Save_Clicked(
        object? sender,
        EventArgs e)
    {
        if (ReservationTimePicker.SelectedIndex < 0)
        {
            await DisplayAlertAsync(
                "Reservation Time",
                "Please select a reservation time.",
                "OK");

            return;
        }

        string selectedTime =
            ReservationTimePicker
                .Items[
                    ReservationTimePicker.SelectedIndex];

        DateTime parsedTime =
            DateTime.Parse(selectedTime);

        SelectedTime = parsedTime.TimeOfDay;

        _tcs.TrySetResult(true);

        await Navigation.PopModalAsync();
    }

    private async void Cancel_Clicked(
        object? sender,
        EventArgs e)
    {
        _tcs.TrySetResult(false);

        await Navigation.PopModalAsync();
    }
}