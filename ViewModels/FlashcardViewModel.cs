using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using N5StudyApp.Models;
using N5StudyApp.Services;
using Microsoft.Maui.Storage; 
using System.Text.Json;
using System.Windows.Input; 
using Microsoft.Maui.Controls;
namespace N5StudyApp.ViewModels
{
    [QueryProperty(nameof(SelectedFileName), "PassedFileName")]
        public class FlashcardViewModel : BaseViewModel
    {
        public string SelectedFileName { get; set; } = "n5_set_01.json";
        private readonly IDataService _dataService;
        private Flashcard? _currentCard;

        public ObservableCollection<Flashcard> Deck { get; set; } = new();

        public Flashcard? CurrentCard
        {
            get => _currentCard;
            set
            {
                SetProperty(ref _currentCard, value);
                OnPropertyChanged(nameof(HasCard)); 
            }
        }

        public bool HasCard => CurrentCard != null;
        private List<Flashcard> _activeDeck = new();

        // THE COMMAND FOR THE RESET BUTTON
        public ICommand ResetProgressCommand { get; }

        public FlashcardViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            // Wire up the button click to the reset method
            ResetProgressCommand = new Command(async () => await ResetProgressAsync());
        }
// 1. THE WEIGHTED SHUFFLE ENGINE (SRS)
        public async Task LoadDeckAsync()
        {
            var fullDeck = await GetFullDeckFromJsonAsync(); 

            // Load our advanced Dictionary memory: <Kanji, MasteryLevel>
            string savedData = Preferences.Default.Get("MasteryProgress", "{}");
            Dictionary<string, int> progress = JsonSerializer.Deserialize<Dictionary<string, int>>(savedData) ?? new();

            List<Flashcard> sessionDeck = new();

            // Multiply cards based on how bad the user is at them
            foreach(var card in fullDeck)
            {
                // Sync the JSON card with the device memory (Default to 0 if it's a new word)
                card.MasteryLevel = progress.ContainsKey(card.Kanji) ? progress[card.Kanji] : 0;

                if (card.MasteryLevel == 0)
                {
                    sessionDeck.Add(card);
                    sessionDeck.Add(card);
                    sessionDeck.Add(card); // New/Hard: Appears 3 times
                }
                else if (card.MasteryLevel == 1)
                {
                    sessionDeck.Add(card);
                    sessionDeck.Add(card); // Familiar: Appears 2 times
                }
                else 
                {
                    sessionDeck.Add(card); // Mastered (Level 2+): ALWAYS appears exactly 1 time
                }
            }

            // Shuffle the deck randomly so the duplicates are spread out
            var random = new Random();
            _activeDeck = sessionDeck.OrderBy(x => random.Next()).ToList();

            NextCard();
        }

   // 2. THE REWARD/PENALTY ENGINE
        public void ProcessSwipe(bool wasCorrect)
        {
            if (CurrentCard == null) return;

            // Load the advanced Dictionary memory
            string savedData = Preferences.Default.Get("MasteryProgress", "{}");
            Dictionary<string, int> progress = JsonSerializer.Deserialize<Dictionary<string, int>>(savedData) ?? new();

            // Get the current score (Default to 0 if it's not in the dictionary yet)
            int currentMastery = progress.ContainsKey(CurrentCard.Kanji) ? progress[CurrentCard.Kanji] : 0;

            if (wasCorrect)
            {
                // REWARD: Level up!
                currentMastery++;
                
                // Instantly purge any remaining duplicates of this card from the active queue so they don't see it again this session
                _activeDeck.RemoveAll(c => c.Kanji == CurrentCard.Kanji);
            }
            else
            {
                // PENALTY: Drop mastery by 1 (but don't let it go below 0)
                currentMastery = Math.Max(0, currentMastery - 1);
                
                // Add it to the back of the queue so they have to review it again before finishing the session
                _activeDeck.Add(CurrentCard);
            }

            // Save the new score back to the device memory
            progress[CurrentCard.Kanji] = currentMastery;
            Preferences.Default.Set("MasteryProgress", JsonSerializer.Serialize(progress));

            NextCard();
        }
        private void NextCard()
        {
            if (_activeDeck.Count > 0)
            {
                CurrentCard = _activeDeck[0];
                _activeDeck.RemoveAt(0);
            }
            else
            {
                CurrentCard = null; 
            }
        }

 private async Task<List<Flashcard>> GetFullDeckFromJsonAsync() 
        { 
            if (SelectedFileName == "ALL")
            {
                // The Master Vocab Deck
                var deck1 = await _dataService.LoadLessonSetAsync("n5_set_01.json") ?? new List<Flashcard>();
                var deck2 = await _dataService.LoadLessonSetAsync("n5_set_02.json") ?? new List<Flashcard>();
                return deck1.Concat(deck2).ToList(); 
            }
            else if (SelectedFileName == "KANJI_ALL")
            {
                // The Master Kanji Deck
                var kanji1 = await _dataService.LoadLessonSetAsync("n5_kanji_set_01.json") ?? new List<Flashcard>();
                var kanji2 = await _dataService.LoadLessonSetAsync("n5_kanji_set_02.json") ?? new List<Flashcard>();
                var kanji3 = await _dataService.LoadLessonSetAsync("n5_kanji_set_03.json") ?? new List<Flashcard>();
                
                return kanji1.Concat(kanji2).Concat(kanji3).ToList();
            }
            else
            {
                // Single Deck Mode
                var cards = await _dataService.LoadLessonSetAsync(SelectedFileName); 
                return cards?.ToList() ?? new List<Flashcard>(); 
            }
        }
       // 3. UPDATED RESET BUTTON
        private async Task ResetProgressAsync()
        {
            // Wipe the new advanced memory key
            Preferences.Default.Remove("MasteryProgress");
            
            // Reload the deck from scratch
            await LoadDeckAsync();
        }
    }
}