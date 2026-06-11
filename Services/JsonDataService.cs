using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using N5StudyApp.Models;

namespace N5StudyApp.Services
{
    public class JsonDataService : IDataService
    {
        public async Task<List<Flashcard>> LoadLessonSetAsync(string fileName)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var flashcards = JsonSerializer.Deserialize<List<Flashcard>>(json);
                return flashcards ?? new List<Flashcard>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading JSON: {ex.Message}");
                return new List<Flashcard>();
            }
        }
    }
}