using EMICalculator.ViewModel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Image = iTextSharp.text.Image;
using CommunityToolkit.Maui.Core.Platform;
using Microcharts;
using System.ComponentModel;
using Color = iTextSharp.text.Color;
using Font = iTextSharp.text.Font;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Networking;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using System;
using CommunityToolkit.Maui.Views;

namespace EMICalculator.View;
public partial class HomeLoanPage : ContentPage
{
    private readonly HomeLoanViewModel viewModel = new();
    public HomeLoanPage()
    {
        BindingContext = viewModel;
        InitializeComponent();
    }

    private HomeLoanViewModel VM => BindingContext as HomeLoanViewModel;
    private async void PrintToPdfBtn_Clicked(object sender, EventArgs e)
    {
        var promptResult = await this.ShowPopupAsync(new PopupPage());
        if (!string.IsNullOrEmpty(promptResult as string))
        {
            string pdfpath = Path.Combine(FileSystem.Current.AppDataDirectory, "EMICalculator.pdf");
            string imagepath = Path.Combine(FileSystem.Current.AppDataDirectory, "EMICalculator.jpg");

            var result = await HomeLoanPages.CaptureAsync();
            using MemoryStream memoryStream = new();
            await result.CopyToAsync(memoryStream);
            await File.WriteAllBytesAsync(imagepath, memoryStream.ToArray());

            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(pdfpath, FileMode.Create));
            doc.Open();
            doc.Add(new Paragraph("JPG"));
            Image jpg = Image.GetInstance(imagepath);
            jpg.ScaleToFit(250f, 250f);
            jpg.Border = Rectangle.BOX;
            jpg.BorderWidth = 5f;
            doc.Add(jpg);

            //Add detail rows
            PdfPTable table = new PdfPTable(5);
            //actual width of table in points
            table.TotalWidth = 550f;
            //fix the absolute width of the table
            table.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3
            //float[] widths = new float[] { 1f, 2f };
            //table.SetWidths(widths);
            table.HorizontalAlignment = 0;

            //leave a gap before and after the table
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            PdfPCell cell = new PdfPCell(new Phrase("Loan Amortization Schedule", new Font(Font.HELVETICA, 12f, Font.NORMAL,Color.YELLOW)));
            cell.BackgroundColor = new Color(255, 140, 0);
            cell.Colspan = 5;
            cell.Border = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            PdfPCell cellMonth = new PdfPCell(new Phrase("Month", new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.YELLOW)));
            cell.BackgroundColor = new Color(255, 140, 0);
            cell.Border = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cellMonth);

            PdfPCell cellPrinciple = new PdfPCell(new Phrase("Principle", new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.YELLOW)));
            cell.BackgroundColor = new Color(255, 140, 0);
            cell.Border = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cellPrinciple);

            PdfPCell cellInterest = new PdfPCell(new Phrase("Interest", new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.YELLOW)));
            cell.BackgroundColor = new Color(255, 140, 0);
            cell.Border = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cellInterest);

            PdfPCell cellPayment = new PdfPCell(new Phrase("Payment", new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.YELLOW)));
            cell.BackgroundColor = new Color(255, 140, 0);
            cell.Border = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cellPayment);

            PdfPCell cellBalance = new PdfPCell(new Phrase("Balance", new Font(Font.HELVETICA, 8f, Font.NORMAL, Color.YELLOW)));
            cell.BackgroundColor = new Color(255, 140, 0);
            cell.Border = 0;
            cell.HorizontalAlignment = 1;
            table.AddCell(cellBalance);

            //Add detail rows.
            foreach (var row in VM.ItemList)
            {
                table.AddCell(row.Month);
                table.AddCell(row.Principal.ToString());
                table.AddCell(row.Interest.ToString());
                table.AddCell(row.TotalPayment.ToString());
                table.AddCell(row.Balance.ToString());
            }
            doc.Add(table);
            doc.Close();
            await Shell.Current.DisplayAlert("Path", pdfpath, "OK");
            SendEmail(pdfpath, promptResult as string);
        }
    }

    private async void SendEmail(string fileName, string emailAddress)
    {
        string subject = "EMI Calculator!";
        string body = "Please find attached the Loan Amortization Schedule.";
        string[] recipients = new[] { emailAddress };

        var message = new EmailMessage
        {
            Subject = subject,
            Body = body,
            BodyFormat = EmailBodyFormat.PlainText,
            To = new List<string>(recipients)

        };

        message.Attachments.Add(new EmailAttachment(fileName));
        await Email.Default.ComposeAsync(message);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        txtLoanAmount.Loaded += delegate
        {
            VM.OptYear = opt1;
            VM.OptMonth = opt2;
            txtLoanAmount.Focus();
        };
        txtLoanAmount.Focused += delegate
        {
            if (txtLoanAmount.Text == "" || txtLoanAmount.Text == "0")
                txtLoanAmount.Text = "";
        };
        txtInterestRate.Focused += delegate
        {
            if (txtInterestRate.Text == "" || txtInterestRate.Text == "0")
                txtInterestRate.Text = "";
        };
        txtTenure.Focused += delegate
        {
            if (txtTenure.Text == "" || txtTenure.Text == "0")
                txtTenure.Text = "";
        };
    }

    private void txtTenure_Completed(object sender, EventArgs e)
    {
        string text = ((Entry)sender).Text;
        txtTenure.IsEnabled = false;
        txtTenure.IsEnabled = true;
        btnCalculate.Focus();
    }

    private void btnClear_Clicked(object sender, EventArgs e)
    {
        VM.IsReady = false;
        VM.IsShow = false;
        VM.Items = new();
        VM.ItemList = new();
        VM.LoanAmount = 0;
        VM.InterestRate = 0;
        VM.Tenure = 0;
        VM.MonthlyEMI = 0;
        VM.TotalInterestPayable = 0;
        VM.TotalAmountPayable = 0;
        VM.IsRunning = false;
        VM.PieChart = null;
        controlsCheck();
    }

    private void controlsCheck()
    {
        txtLoanAmount.Focus();
        txtLoanAmount.Text = "";
        txtInterestRate.Text = "";
        txtTenure.Text = "";
    }
}