using System.Collections.Generic;
using System.Threading.Tasks;
using N5StudyApp.Models;

namespace N5StudyApp.Services
{
    public interface IDataService
    {
        Task<List<Flashcard>> LoadLessonSetAsync(string fileName);
    }
}