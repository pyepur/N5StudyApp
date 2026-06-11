namespace N5StudyApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("FlashcardPage", typeof(Views.FlashcardPage));
	}
}
