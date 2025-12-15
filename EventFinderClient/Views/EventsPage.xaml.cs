using EventFinderClient.ViewModels;

namespace EventFinderClient.Views;

public partial class EventsPage : ContentPage
{
    public EventsPage(EventsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}