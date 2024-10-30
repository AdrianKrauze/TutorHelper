namespace TutorHelper.Models
{
    public class SearchQuery
    {
        public int PageNumber { get; set; } = 1; // Domyślna strona to 1
        public int PageSize { get; set; } = 10; // Domyślny rozmiar strony to 10
    }
}