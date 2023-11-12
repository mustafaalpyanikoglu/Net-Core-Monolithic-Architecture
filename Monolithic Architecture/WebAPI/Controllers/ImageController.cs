using BusinessLayer.Features.Images.Dtos;
using BusinessLayer.Services.ImageService;
using WebAPI.CrossCuttingConcerns;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CrossCuttingConcerns.Exceptions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : BaseController
    {
        private readonly ImageServiceBase _imageServiceBase;

        public ImageController(ImageServiceBase imageServiceBase)
        {
            _imageServiceBase = imageServiceBase;
        }

        //[HttpPost("add")]
        //public async Task<IActionResult> UploadImage([FromForm(Name = "Image")] IFormFile file)
        //{
        //    string result = await _imageServiceBase.UploadAsync(file);
        //    return Ok(result);
        //}

        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteImage([FromBody] ImageDto deletedImageDto)
        {
            await _imageServiceBase.DeleteAsync(deletedImageDto.ImageUrl);
            return Ok();
        }

        [ProducesResponseType(typeof(ImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDynamicResponseModel), StatusCodes.Status500InternalServerError)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateImage([FromForm] UpdatedImageDto model)
        {
            model.ImageUrl = await _imageServiceBase.UpdateAsync(model.Image, model.ImageUrl);
            return Ok(new ImageDto() { ImageUrl = model.ImageUrl});
        }
    }
}
