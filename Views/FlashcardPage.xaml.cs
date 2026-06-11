using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using N5StudyApp.ViewModels;

namespace N5StudyApp.Views
{
    public partial class FlashcardPage : ContentPage
    {
        private readonly FlashcardViewModel _viewModel;
        private bool _isShowingFront = true;

        public FlashcardPage(FlashcardViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDeckAsync();
        }

        // Custom Back Navigation (Returns to Home Screen)
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        // Tap to Flip Card (Premium Spring-Loaded Animation)
        private async void OnCardTapped(object sender, EventArgs e)
        {
            // 1. "Press down" effect
            await MainCard.ScaleTo(0.95, 100, Easing.CubicOut);

            // 2. Squish horizontally
            await MainCard.ScaleXTo(0, 150, Easing.CubicIn);

            // 3. Swap content
            if (_isShowingFront)
            {
                FrontLayout.Opacity = 0;
                BackLayout.Opacity = 1;
            }
            else
            {
                FrontLayout.Opacity = 1;
                BackLayout.Opacity = 0;
            }

            _isShowingFront = !_isShowingFront;

            // 4. Stretch horizontally
            await MainCard.ScaleXTo(1, 150, Easing.CubicOut);

            // 5. "Pop up" effect
            await MainCard.ScaleTo(1, 250, Easing.SpringOut);
        }

        // 2D Drag and Physics Engine
        private async void OnCardPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    MainCard.TranslationX = e.TotalX;
                    MainCard.TranslationY = e.TotalY;
                    MainCard.Rotation = e.TotalX * 0.05;

                    double progress = Math.Min(1.0, Math.Abs(e.TotalX) / 150.0);
                    
                    if (e.TotalX > 0) // Dragging Right
                    {
                        RightSwipeIndicator.Opacity = progress;
                        RightSwipeIndicator.Scale = 0.8 + (0.2 * progress);
                        LeftSwipeIndicator.Opacity = 0;
                    }
                    else // Dragging Left
                    {
                        LeftSwipeIndicator.Opacity = progress;
                        LeftSwipeIndicator.Scale = 0.8 + (0.2 * progress);
                        RightSwipeIndicator.Opacity = 0;
                    }
                    break;

                case GestureStatus.Completed:
                    double swipeThreshold = 120;

                    if (MainCard.TranslationX > swipeThreshold)
                    {
                        // Swipe Right Confirmed
                        _ = RightSwipeIndicator.FadeTo(0, 200);
                        await MainCard.TranslateTo(600, MainCard.TranslationY, 200, Easing.CubicIn);
                        _viewModel.ProcessSwipe(wasCorrect: true);
                        ResetCardPosition();
                    }
                    else if (MainCard.TranslationX < -swipeThreshold)
                    {
                        // Swipe Left Confirmed
                        _ = LeftSwipeIndicator.FadeTo(0, 200);
                        await MainCard.TranslateTo(-600, MainCard.TranslationY, 200, Easing.CubicIn);
                        _viewModel.ProcessSwipe(wasCorrect: false);
                        ResetCardPosition();
                    }
                    else
                    {
                        // Spring Snapback
                        await Task.WhenAll(
                            MainCard.TranslateTo(0, 0, 250, Easing.SpringOut),
                            MainCard.RotateTo(0, 250, Easing.SpringOut),
                            RightSwipeIndicator.FadeTo(0, 250),
                            LeftSwipeIndicator.FadeTo(0, 250)
                        );
                    }
                    break;
            }
        }

        private void ResetCardPosition()
        {
            MainCard.TranslationX = 0;
            MainCard.TranslationY = 0;
            MainCard.Rotation = 0;
            MainCard.Scale = 1; // Ensures the scale resets if tapped repeatedly
            
            RightSwipeIndicator.Opacity = 0;
            LeftSwipeIndicator.Opacity = 0;
            RightSwipeIndicator.Scale = 0.8;
            LeftSwipeIndicator.Scale = 0.8;

            _isShowingFront = true;
            FrontLayout.Opacity = 1;
            BackLayout.Opacity = 0;
        }
    }
}