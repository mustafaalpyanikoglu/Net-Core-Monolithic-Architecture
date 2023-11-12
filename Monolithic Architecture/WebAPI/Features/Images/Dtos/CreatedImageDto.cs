using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Images.Dtos
{
    public class CreatedImageDto : IDto
    {
        public int UserID { get; set; }
        public IFormFile Image { get; set; }
    }
}
