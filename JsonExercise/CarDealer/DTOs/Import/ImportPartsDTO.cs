using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportPartsDTO
    {
		[JsonProperty("name")]
		public string Name { get; set; } = null!;

		[JsonProperty("price")]
		public string Price { get; set; } = null!;

		[JsonProperty("quantity")]
		public string Quantity { get; set; } = null!;

		[JsonProperty("supplierId")]
		public string SupplierId { get; set; } = null!;
	}
}
