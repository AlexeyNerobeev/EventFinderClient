using EventFinderClient.ViewModels;

namespace EventFinderClient.Views;

[QueryProperty(nameof(EventId), "id")]
public partial class EventDetailsPage : ContentPage
{
    private readonly EventDetailsViewModel _viewModel;

    public EventDetailsPage(EventDetailsViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    private int _eventId;
    public int EventId
    {
        get => _eventId;
        set
        {
            _eventId = value;

            if (_viewModel != null && value > 0)
            {
                _viewModel.EventId = value;
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}