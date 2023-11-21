using EMICalculator.View;

namespace EMICalculator
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoanAmorPage), typeof(LoanAmorPage));
        }
    }
}