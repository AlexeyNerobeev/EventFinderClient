using EventFinderClient.ViewModels;

namespace EventFinderClient.Views;

[QueryProperty("Id", "id")]
public partial class EventDetailsPage : ContentPage
{
    private readonly EventDetailsViewModel _viewModel;

    public int Id
    {
        set
        {
            if (_viewModel != null)
            {
                _viewModel.EventId = value;
            }
        }
    }

    public EventDetailsPage(EventDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
}