using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportSupplierDTO
    {
		[JsonProperty("name")]
		public string Name { get; set; } = null!;

		[JsonProperty("isImporter")]
		[Required]
		public bool IsImporter { get; set; }
	}
}
