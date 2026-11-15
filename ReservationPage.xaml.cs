namespace SunshineTailwinds.Waitlist.MAUI;

public partial class ReservationPage : ContentPage
{
    public TimeSpan? SelectedTime { get; private set; }

    private readonly TaskCompletionSource<bool> _tcs =
        new();

    public ReservationPage()
    {
        InitializeComponent();
    }

    public Task WaitForCloseAsync()
    {
        return _tcs.Task;
    }

    private async void Save_Clicked(
        object? sender,
        EventArgs e)
    {
        SelectedTime =
            ReservationTimePicker.Time;

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