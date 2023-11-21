using EMICalculator.Model;
using EMICalculator.ViewModel;
using MvvmHelpers;
using System.Web;

namespace EMICalculator.View;

public partial class LoanAmorPage : ContentPage
{
    HomeLoanViewModel viewModel = new();
    public LoanAmorPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}