using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using Traveling.Dtos;
using Traveling.Models;
using Traveling.Services;

namespace Traveling.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;

        public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository ??
                throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        // 5-9 【应用】获取嵌套对象关系型数据
        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (! await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
                return NotFound($"The touristRoute:{touristRouteId} not found");

            var pictureFromRepo = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);

            if (pictureFromRepo == null || pictureFromRepo.Count() <= 0)
                return NotFound("No picture found");

            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(pictureFromRepo));
        }

        // 5-10 【应用】单独获取子资源 
        [HttpGet("{pictureId}", Name = "GetPicture")]
        public async Task<IActionResult> GetPicture(Guid touristRouteId, int pictureId) 
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
                return NotFound($"touristRoute id: {touristRouteId} not Found");

            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);

            if (pictureFromRepo == null)
                return NotFound("No picture found");

            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }

        // 7-3 【应用】创建子资源：旅游路线图片
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto) 
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
                return NotFound($"TouristRoute: {touristRouteId} not found");

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();

            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute(
                "GetPicture",
                new
                {
                    touristRouteId = pictureModel.TouristRouteId,
                    pictureId = pictureModel.Id
                },
                pictureToReturn   
           );
        }

        // 9-2 【应用】删除嵌套子资源
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{pictureId}")]
        public async Task<IActionResult> DeletPicture(
            [FromRoute] Guid touristRouteId,
            [FromRoute] int pictureId
        )
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
                return NotFound($"Tourist Route: {touristRouteId} not found");

            var picture = await _touristRouteRepository.GetPictureAsync(pictureId);
            _touristRouteRepository.DeleteTouristRoutePicture(picture);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
    }
}
