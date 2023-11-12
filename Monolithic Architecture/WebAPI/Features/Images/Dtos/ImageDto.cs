using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Images.Dtos
{
    public class ImageDto : IDto
    {
        public string ImageUrl { get; set; }
    }
}
