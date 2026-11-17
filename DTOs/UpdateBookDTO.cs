using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.DTOs
{
    public class UpdateBookDTO: IValidatableObject
    {
        [Required]
        [MinLength(2)]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        
        public int PublishYear { get; set; }

        public bool IsAvailable { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PublishYear <1000 || PublishYear > DateTime.Now.Year)
            {
                yield return new ValidationResult(
                    $"Publish year must be between 1 and {DateTime.Now.Year}.",
                    new[] { nameof(PublishYear) });
            }
        }
    }
}
