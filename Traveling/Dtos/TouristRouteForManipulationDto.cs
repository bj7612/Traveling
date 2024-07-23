using System.ComponentModel.DataAnnotations;
using Traveling.ValidationAttributes;

namespace Traveling.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescriptionAttribut] // 7-8 【应用】类级别数据验证
    public abstract class TouristRouteForManipulationDto // : IValidatableObject
    {
        [Required(ErrorMessage = "title 不可为空")]  // Using the anotation to do the data validation in the dto stage before it go into model
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
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
    }
}
