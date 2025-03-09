using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ProductShop.DTOs.Import
{
    public class ImportUserDto
    {
		[JsonProperty("firstName")]
		public string? FirstName { get; set; }

		[JsonProperty("lastName")]
		[Required]
		public string LastName { get; set; } = null!;

		[JsonProperty("age")]
		public string? Age { get; set; }
	}
}
