using JuniorSteps.Models;

namespace JuniorSteps.ViewModels
{
    public class PostIndexViewModel
    {
        public List<Post> Posts { get; set; }
        public List<CategoryCountViewModel> CategoryCounts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
