using System.ComponentModel.DataAnnotations;
using Traveling.Models;
using Traveling.ValidationAttributes;

namespace Traveling.Dtos
{
    //[TouristRouteTitleMustBeDifferentFromDescriptionAttribut] // 7-8 【应用】类级别数据验证
    public class TouristRouteForCreationDto : TouristRouteForManipulationDto 
    {
        /*
        [Required(ErrorMessage = "title can not be empty")]   // Using the anotation to do the data validation in the dto stage before it go into model 
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }
            // 计算方式：原价 * 折扣
            public decimal Price { get; set; }
            //public decimal OriginalPrice { get; set; }
            //public double? DiscountPresent { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime? UpdateTime { get; set; }
            public DateTime? DepartureTime { get; set; }
            public string? Features { get; set; }
            public string? Fees { get; set; }
            public string? Notes { get; set; }
            public double? Rating { get; set; }
            public string? TravelDays { get; set; }
            public string? TripType { get; set; }
            public string? DepartureCity { get; set; }
            public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
                = new List<TouristRoutePictureForCreationDto>();
        */

        // 7-7 【应用】属性级别数据验证
        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == Description)
            {
                yield return new ValidationResult(
                        "Route title can not be same as description",
                        new[] { "TouristRouteForCreationDto" }
                    );
            }
        }
        */
    }
}
