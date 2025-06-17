using Microsoft.AspNetCore.Http;

namespace JuniorSteps.ViewModels
{
    public class PostCreateViewModel
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int? CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; } 
        public string? ImagePath { get; set; }  
    }
}