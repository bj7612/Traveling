using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Traveling.Dtos;
using Traveling.Help;
using Traveling.Models;
using Traveling.ResoureParameters;
using Traveling.Services;

namespace Traveling.Controllers
{
    [Route("api/[controller]")] // api/touristroute
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteRepository _routeRepository;
        private readonly IMapper _mapper;

        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
        )
        {
            _routeRepository = touristRouteRepository;
            _mapper = mapper;
        }

        // api/touristRoutes?keyword=传入的参数
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            // reducing the coupling between url parameters and controller by encapsolation of the parameters
            [FromQuery] TouristRouteResourceParameters parameters
            // [FromQuery] string keyword,
            // [FromQuery] string rating // 小于lessThan, 大于largerThan, 等于equalTo, lessThan3, largerThan2, equalTo5
            )
        {
            var touristRoutesFromRepo = await _routeRepository.GetTouristRoutesAsync(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);

            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0) {
                return NotFound("No tourist route found");
            }

            var touristRoutesDot = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            return Ok(touristRoutesDot);
        }

        // api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId) {
            var touristRouteFromRepo = await _routeRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"Tourist Route: {touristRouteId} not found");
            }

            //var touristRouteDto = new TouristRouteDto()
            //{
            //    Id = touristRouteFromRepo.Id,
            //    Title = touristRouteFromRepo.Title,
            //    Description = touristRouteFromRepo.Description,
            //    Price = touristRouteFromRepo.OriginalPrice * (decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
            //    CreateTime = touristRouteFromRepo.CreateTime,
            //    UpdateTime = touristRouteFromRepo.UpdateTime,
            //    Features = touristRouteFromRepo.Features,
            //    Fees = touristRouteFromRepo.Fees,
            //    Notes = touristRouteFromRepo.Notes,
            //    Rating = touristRouteFromRepo.Rating,
            //    TravelDays = touristRouteFromRepo.TravelDays.ToString(),
            //    TripType = touristRouteFromRepo.TripType.ToString(),
            //    DepartureCity = touristRouteFromRepo.DepartureCity.ToString()

            //};
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);
            return Ok(touristRouteDto);
        }

        // 7-2 【应用】创建旅游路线资源
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _routeRepository.AddTouristRoute(touristRouteModel);
            await _routeRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            // reture status code 201 with body and header
            return CreatedAtRoute(
                "GetTouristRouteById", // API route name which will contait in the return header
                new { touristRouteId = touristRouteToReturn.Id }, // route value 
                touristRouteToReturn
                );
        }

        // 8-2 【应用】使用put请求更新资源
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{touristRouteId}")]
        public async Task<IActionResult> UpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
            )
        {
            if (!(await _routeRepository.TouristRouteExistsAsync(touristRouteId)))
                return NotFound($"Tourist Route: {touristRouteId} not found");
            var touristRouteFromRepo = await _routeRepository.GetTouristRouteAsync(touristRouteId);

            // using map() to do 1. 映射 dto 2. 更新dto 3. 映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            await _routeRepository.SaveAsync();

            return NoContent();
        }

        // 8-5 【应用】使用PATCH部分更新资源
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpPatch("{touristRouteId}")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
        )
        {
            if (!(await _routeRepository.TouristRouteExistsAsync(touristRouteId)))
                return NotFound($"Tourist Route: {touristRouteId} not found");

            var touristRouteFromRepo = await _routeRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPath = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);

            patchDocument.ApplyTo(touristRouteToPath, ModelState);

            // 8-6 【应用】PATCH请求的数据验证
            if (!TryValidateModel(touristRouteToPath))
                return ValidationProblem(ModelState);


            _mapper.Map(touristRouteToPath, touristRouteFromRepo);
            await _routeRepository.SaveAsync();

            return NoContent();
        }

        // 9-1 【应用】删除资源
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{touristRouteId}")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!(await _routeRepository.TouristRouteExistsAsync(touristRouteId)))
                return NotFound($"Tourist Route: {touristRouteId} not found");

            var touristRoute = await _routeRepository.GetTouristRouteAsync(touristRouteId);
            _routeRepository.DeleteTouristRoute(touristRoute);
            await _routeRepository.SaveAsync();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("({touristIDs})")]
        public async Task<IActionResult> DeleteTouristRoutesByIDs(
            [ModelBinder(BinderType = typeof(ArrayModelBlinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if (touristIDs == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = await _routeRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _routeRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _routeRepository.SaveAsync();

            return NoContent();
        }
    }
}
