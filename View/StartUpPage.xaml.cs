using EMICalculator.ViewModel;

namespace EMICalculator.View;

public partial class StartUpPage : ContentPage
{
	private readonly HomeLoanViewModel viewModel = new();
    double width = DeviceDisplay.Current.MainDisplayInfo.Width / 4;
    public StartUpPage()
	{
		InitializeComponent();
		BindingContext = viewModel;
		btnHomeLoan.WidthRequest = width;
		btnCarLoan.WidthRequest = width;
		btnPersonalLoan.WidthRequest = width;
	}

}