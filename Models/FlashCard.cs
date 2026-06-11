using System;
using System.Text.Json.Serialization;

namespace N5StudyApp.Models
{
    public class Flashcard
    {
        [JsonPropertyName("Id")]
        public int? Id { get; set; }

        [JsonPropertyName("SetName")]
        public string? SetName { get; set; }

        [JsonPropertyName("Kanji")]
        public string? Kanji { get; set; }

        [JsonPropertyName("Reading")]
        public string? Reading { get; set; }

        [JsonPropertyName("English")]
        public string? English { get; set; }

        [JsonPropertyName("MasteryLevel")]
        public int MasteryLevel { get; set; } = 0;

        [JsonPropertyName("NextReviewDate")]
        public DateTime NextReviewDate { get; set; } = DateTime.UtcNow;

        public void UpdateProgress(bool wasCorrect)
        {
            if (wasCorrect)
            {
                MasteryLevel++;
                int daysToAdd = (int)Math.Pow(2, MasteryLevel - 1);
                NextReviewDate = DateTime.UtcNow.AddDays(daysToAdd);
            }
            else
            {
                MasteryLevel = 0;
                NextReviewDate = DateTime.UtcNow;
            }
        }
    }
}