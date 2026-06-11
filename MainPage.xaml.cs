using Microsoft.Maui.Controls;
using System;

namespace N5StudyApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnDeckButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string selectedFile = button.CommandParameter.ToString();

            // Navigate to the Flashcard page, attaching the file name as a variable!
            await Shell.Current.GoToAsync($"FlashcardPage?PassedFileName={selectedFile}");
        }
    }
}