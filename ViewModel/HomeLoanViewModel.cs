
using EMICalculator.View;
using Microcharts;
using Microsoft.Maui.Controls;
using SkiaSharp.Views.Maui;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace EMICalculator.ViewModel
{
    [QueryProperty(nameof(Items), nameof(Items))]
    [QueryProperty(nameof(ItemList), nameof(ItemList))]
    public partial class HomeLoanViewModel : BaseViewModel
    {
        int batchSize = 40;

        public HomeLoanViewModel()
        {
            IsRunning = false;
        }

        public void Clear()
        {
            IsShow = false;
            Items = new();
            ItemList = new();
            LoanAmount = 0;
            InterestRate = 0;
            Tenure = 0;
            MonthlyEMI = 0;
            TotalInterestPayable = 0;
            TotalAmountPayable = 0;
            IsRunning = false;
            PieChart = null;
        }

        [RelayCommand]
        public void TenureChanged(object content)
        {
            var entry = content as Entry;

            if (entry.Text != "" && entry.Text != "0")
            {
                if (Convert.ToInt32(entry.Text) > 40 && OptYear.Value.ToString() == "Yr" && OptYear.IsChecked)
                    Tenure = 40;
                if (Convert.ToInt32(entry.Text) > 480 && OptMonth.Value.ToString() == "Mt" && OptMonth.IsChecked)
                    Tenure = 480;
            }
        }

        [RelayCommand]
        public void RadioChanged(object content)
        {
            var rb = content as RadioButton;
            int exTenure = Tenure;

            if (rb.Value.ToString() == "Yr" && rb.IsChecked)
            {
                if (Tenure > 40)
                    Tenure = 40;
                else
                {
                    Tenure = exTenure / 12;
                    TenureType = 'Y';
                }
            }
            else if (rb.Value.ToString() == "Mt" && rb.IsChecked)
            {
                if (Tenure > 480)
                    Tenure = 480;
                else
                {
                    Tenure = exTenure * 12;
                    TenureType = 'M';
                }
            }
        }


        [RelayCommand]
        Task Navigate() => Shell.Current.GoToAsync($"{nameof(LoanAmorPage)}?", new Dictionary<string, object> { ["Items"] = Items, ["ItemList"] = ItemList });

        [RelayCommand]
        private void Reload()
        {
            IsRefreshing = false;
        }

        [RelayCommand]
        private void LoadMoreItems()
        {
            IsRunning = false;
            IsBusy = false;
            IsLoading = false;
            if (IsLoading)
                return;

            IsLoading = true;
            if (ItemList.Count > 0)
            {
                IsRefreshing = true;
                IsBusy = true;
                IsRunning = true;
                IsLoading = true;
                Items.AddRange(ItemList.Skip(Items.Count).Take(batchSize).ToList());
                IsLoading = false;
                IsBusy = false;
                IsRunning = false;
            }
        }

    }
}