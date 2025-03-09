using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext dbContext = new CarDealerContext();

            string jsonString = File.ReadAllText("../../../Datasets/cars.json");

           string result = ImportParts(dbContext, jsonString);

			Console.WriteLine(result);
		}

        public static string ImportSuppliers(CarDealerContext context, string inputJson) 
        {
			var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson);

			context.Suppliers.AddRange(suppliers);
			context.SaveChanges();

            string result = $"Successfully imported {suppliers.Count}.";

            return result;

		}

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;
            ImportPartsDTO[]? partsDTOs = JsonConvert
                .DeserializeObject<ImportPartsDTO[]>(inputJson);

            if (partsDTOs != null)
            {
                ICollection<Part> partsToAdd = new List<Part>();
				var validSupplierIds = context.Suppliers.Select(s => s.Id).ToList();
				foreach (ImportPartsDTO partDTO in partsDTOs)
                {
                    if (!IsValid(partDTO))
                    {
                        continue;
                    }

                    decimal partPrice = 0;
                    int partQuantity = 0;
                    int partSupplierId = 0;
                    if (partDTO.Price != null)
                    {
                        bool isPriceValid = Decimal.TryParse(partDTO.Price, out decimal parsedPrice);
                        if (!isPriceValid)
                        {
                            continue;
                        }

                        partPrice = parsedPrice;
                    }
                    if (partDTO.Quantity != null)
                    {
                        bool isQuantityValid = int.TryParse(partDTO.Quantity, out int parsedQuantity);
                        if (!isQuantityValid)
                        {
                            continue;
                        }
                        partQuantity = parsedQuantity;
                    }
                    if (partDTO.SupplierId != null)
                    {
                        bool isSupplierIdValid = int.TryParse(partDTO.SupplierId, out int parsedSupplierId);
                        if (!isSupplierIdValid)
                        {
                            continue;
                        }
                        partSupplierId = parsedSupplierId;
                    }
					if (!validSupplierIds.Contains(partSupplierId))
					{
						continue;
					}

					Part part = new Part()
                    {
                        Name = partDTO.Name,
                        Price = partPrice,
                        Quantity = partQuantity,
						SupplierId = partSupplierId
					};

                    partsToAdd.Add(part);
				}
                context.Parts.AddRange(partsToAdd);
				context.SaveChanges();
                result = $"Successfully imported {context.Parts.Count()}.";
			}
                return result;
		}

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCarsDto[]? carsDTOs
                = JsonConvert.DeserializeObject<ImportCarsDto[]>(inputJson);

            if (carsDTOs != null)
            {
                ICollection<Car> carsToAdd = new List<Car>();
                foreach (ImportCarsDto carDTO in carsDTOs)
                {
                    if (!IsValid(carDTO))
                    {
                        continue;
                    }

                    int carTravlledDistance = 0;
                    int[] partsId = new int[0];

                    if (carDTO.TravelledDistance != null) 
                    {
						bool isTravelledDistanceValid = int.TryParse(carDTO.TravelledDistance, out int travelledDistance);
						if (!isTravelledDistanceValid)
						{
							continue;
						}

						carTravlledDistance = travelledDistance;
					}

					if (carDTO.PartsId != null)
					{
						bool isPartsIdValid = int.TryParse(carDTO.PartsId, out int parsedPartsId);
						if (!isPartsIdValid)
						{
							continue;
						}

                        int[] arrayParsedPartsId = new int[] { parsedPartsId };
						partsId = arrayParsedPartsId;
					}

                    Car car = new Car()
                    {
                        Make = carDTO.Make,
                        Model = carDTO.Model,
                        TraveledDistance = carTravlledDistance,

                    };
				}
            }
        }

		private static bool IsValid(object dto)
		{
			var validateContext = new ValidationContext(dto);
			var validationResults = new List<ValidationResult>();

			bool isValid = Validator
				.TryValidateObject(dto, validateContext, validationResults, true);

			return isValid;
		}

	}
}