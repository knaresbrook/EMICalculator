using CommunityToolkit.Maui.Views;

namespace EMICalculator.View;

public partial class PopupPage : Popup
{
	public PopupPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked_Yes(object sender, EventArgs e)
    {
        await this.CloseAsync(txtEmailAddress.Text);
    }

    private async void Button_Clicked_No(object sender, EventArgs e)
    {
        await this.CloseAsync(true);
    }
}