namespace SunshineTailwinds.Waitlist.MAUI;

public partial class GuestActionPage : ContentPage
{
    private TaskCompletionSource<bool> _tcs = new();

    public string SelectedAction { get; private set; } = "";

    public GuestActionPage()
    {
        InitializeComponent();

        SeatGuestButton.Clicked += Action_Clicked;
        OrdersInButton.Clicked += Action_Clicked;
        ClosePartyButton.Clicked += Action_Clicked;
        ReturnWaitingButton.Clicked += Action_Clicked;
        EditButton.Clicked += Action_Clicked;
        PreSeatButton.Clicked += Action_Clicked;
        MoveUpButton.Clicked += Action_Clicked;
        MoveDownButton.Clicked += Action_Clicked;
        RemoveButton.Clicked += Action_Clicked;

        CancelButton.Clicked += async (s, e) =>
        {
            _tcs.TrySetResult(true);
            await Navigation.PopModalAsync();
        };
    }

    public Task WaitForCloseAsync()
    {
        return _tcs.Task;
    }

    private async void Action_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            SelectedAction = button.Text;

            _tcs.TrySetResult(true);

            await Navigation.PopModalAsync();
        }
    }
}