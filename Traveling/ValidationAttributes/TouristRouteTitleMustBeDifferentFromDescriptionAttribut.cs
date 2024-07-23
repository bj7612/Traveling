using System.ComponentModel.DataAnnotations;
using Traveling.Dtos;

namespace Traveling.ValidationAttributes
{
    public class TouristRouteTitleMustBeDifferentFromDescriptionAttribut: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var touristRouteDto = (TouristRouteForManipulationDto)validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult(
                        "Route title can not be same as description",
                        new[] { "TouristRouteForCreationDto" }
                    );
            }
            return ValidationResult.Success;
        }
    }
}
