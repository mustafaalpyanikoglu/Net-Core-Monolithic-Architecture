using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Images.Dtos
{
    public class UpdatedImageDto : IDto
    {
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
