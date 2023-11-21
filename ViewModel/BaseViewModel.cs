
using MvvmHelpers;
using System.Windows.Input;
using EMICalculator.Model;
using Microcharts;
using SkiaSharp.Views.Maui;
using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;


namespace EMICalculator.ViewModel
{
    public partial class BaseViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {

        [ObservableProperty]
        public RadioButton optYear;

        [ObservableProperty]
        public RadioButton optMonth;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private double loanAmount;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private double interestRate;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private int tenure;

        [ObservableProperty]
        private double monthlyEMI;

        [ObservableProperty]
        private double totalAmountPayable;

        [ObservableProperty]
        private double totalInterestPayable;

        [ObservableProperty]
        public ObservableRangeCollection<Loan> items = new();

        [ObservableProperty]
        public ObservableRangeCollection<Loan> loanList = new();

        [ObservableProperty]
        public List<Loan> itList = new();

        [ObservableProperty]
        public List<Loan> itemList = new();

        [ObservableProperty]
        private bool isReady = false;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private bool isRunning = false;

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private bool isShow = false;

        [ObservableProperty]
        private bool isRefreshing = false;

        [ObservableProperty]
        private char tenureType;

        private int batchSize = 40;
        private PieChart pieChart;
        public PieChart PieChart
        {
            get => pieChart;
            set => SetProperty(ref pieChart, value);
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            IsRunning = true;
            await Task.Delay(2000);
            if (Items.Count == 0)
                await LoadData();
            IsShow = true;
            IsLoading = false;
            IsRunning = false;
        }

        private bool CanSave =>
            (LoanAmount.ToString() != "0" && LoanAmount.ToString() != "") && (InterestRate.ToString() != "0" && InterestRate.ToString() != "") && (Tenure.ToString() != "0" && Tenure.ToString() != "");

        private async Task LoadData()
        {
            try
            {
                double intRate = InterestRate / 12 / 100;
                double noOfMonths = Tenure * 12;
                double powerValue = Math.Pow(1 + intRate, noOfMonths);

                MonthlyEMI = (LoanAmount * intRate * powerValue) / (powerValue - 1);
                TotalAmountPayable = MonthlyEMI * noOfMonths;
                TotalInterestPayable = TotalAmountPayable - LoanAmount;

                MonthlyEMI = MonthlyEMI;
                TotalAmountPayable = Math.Round(TotalAmountPayable);
                TotalInterestPayable = Math.Round(TotalInterestPayable);

                this.GraphChart((int)TotalInterestPayable, (int)LoanAmount);

                double principleAmt = 0.0;
                double interestAmt = 0.0;
                double loanBalanceAmt = LoanAmount;
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                int numMonths = 0;

                numMonths = (TenureType == 'M') ? Tenure : Tenure * 12;

                int countMonths = 0;
                int cMths;
                Items.Clear();

                for (int x = 1; x <= 1; x++)
                {
                    for (int i = currentMonth; i <= currentMonth + 11; i++)
                    {
                        cMths = i;
                        Loan sln = new Loan();
                        double firstPrincipalAmt = MonthlyEMI - (loanBalanceAmt * ((InterestRate / 100) / 12));
                        double firstInterestAmt = ((InterestRate / 100) / 12) * loanBalanceAmt;
                        loanBalanceAmt = loanBalanceAmt - firstPrincipalAmt;
                        principleAmt = principleAmt + firstPrincipalAmt;
                        interestAmt = interestAmt + firstInterestAmt;

                        DateTime date = new(currentYear, cMths, 1);

                        sln.Month = date.ToString("MMM");
                        sln.Year = currentYear;
                        sln.Principal = Math.Round(firstPrincipalAmt);
                        sln.Interest = Math.Round(firstInterestAmt);
                        sln.TotalPayment = Math.Round(firstPrincipalAmt + firstInterestAmt);
                        sln.Balance = Math.Round(loanBalanceAmt);
                        ItemList.Add(sln);
                        countMonths++;
                        if (i == 12 && countMonths != numMonths)
                        {
                            Loan lns = new();
                            lns.Year = currentYear;
                            lns.Month = currentYear.ToString();
                            lns.Principal = Math.Round(principleAmt);
                            lns.Interest = Math.Round(interestAmt);
                            lns.TotalPayment = Math.Round(principleAmt + interestAmt);
                            lns.Balance = Math.Round(loanBalanceAmt);

                            ItemList.Add(lns);

                            principleAmt = 0.0;
                            interestAmt = 0.0;
                            i = 0;
                            currentYear++;
                        }
                        if (countMonths == numMonths)
                            break;
                    }

                    Loan ln = new();
                    ln.Year = currentYear;
                    ln.Month = currentYear.ToString();
                    ln.Principal = Math.Round(principleAmt);
                    ln.Interest = Math.Round(interestAmt);
                    ln.TotalPayment = Math.Round(principleAmt + interestAmt);
                    ln.Balance = Math.Round(loanBalanceAmt);

                    ItemList.Add(ln);

                    principleAmt = 0.0;
                    interestAmt = 0.0;
                    currentMonth = 1;
                    currentYear++;
                }

                MonthlyEMI = Math.Round(MonthlyEMI);

                Items.ReplaceRange(ItemList.Take(batchSize).ToList());
                IsLoading = false;
                IsReady = true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.StackTrace.ToString(), "OK");
            }
        }

        private void GraphChart(int TotalInterestPayable, int PrincipalAmount)
        {
            try
            {
                PieChart = new PieChart
                {
                    LabelTextSize = 34,
                    LabelMode = LabelMode.LeftAndRight,
                    Entries = new List<ChartEntry>
                    {
                        new ChartEntry(PrincipalAmount)
                        {
                            Color = Colors.Green.ToSKColor(),
                            Label = "Principal Loan Amount",
                            ValueLabel = PrincipalAmount.ToString(),
                            ValueLabelColor = Colors.Green.ToSKColor(),
                        },
                        new ChartEntry(TotalInterestPayable)
                        {
                            Color = Colors.DarkOrange.ToSKColor(),
                            ValueLabelColor = Colors.DarkOrange.ToSKColor(),
                            Label = "Total Interest",
                            ValueLabel = TotalInterestPayable.ToString(),
                        },
                    }
                };
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", ex.StackTrace.ToString(), "OK");
            }
        }
    }
}