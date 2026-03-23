using System.ComponentModel.DataAnnotations;

namespace YouthParliamentApp.ViewModels
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Введите название")]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public decimal ComplexityWeight { get; set; } = 1.0m;
        public int CategoryId { get; set; }

        // Призы (для простоты на хакатоне берем один базовый приз-баллы и один бонус)
        public int BasePoints { get; set; }
        public string BonusTitle { get; set; } // Например: "Мерч", "Встреча с депутатом"
    }
}
