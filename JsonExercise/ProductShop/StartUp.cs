﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext dbContext = new ProductShopContext();
            dbContext.Database.Migrate();
			
			string jsonString = File.ReadAllText("../../../Datasets/categories-products.json");
			string result = GetSoldProducts(dbContext);
			Console.WriteLine(result);
		} 

        public static string ImportUsers(ProductShopContext context, string inputJson) 
        {
			string result = string.Empty;
			ImportUserDto[]? userDtos = JsonConvert
				.DeserializeObject<ImportUserDto[]>(inputJson);

			if (userDtos != null) 
			{
				ICollection<User> usersToAdd = new List<User>();
				foreach (ImportUserDto userDto in userDtos)
				{
					if (!IsValid(userDto)) 
					{
						continue;
					}
					int? userAge = null;
					if (userDto.Age != null) 
					{
						bool isAgeValid = int.TryParse(userDto.Age, out int parsedAge);
						if (!isAgeValid) 
						{
							continue;
						}

						userAge = parsedAge;
					}

					User user = new User
					{
						FirstName = userDto.FirstName,
						LastName = userDto.LastName,
						Age = userAge
					};

					usersToAdd.Add(user);
				}

				context.Users.AddRange(usersToAdd);
				context.SaveChanges();
			result = $"Successfully imported {usersToAdd.Count}";
			}
			return result;
		}

		public static string ImportProducts(ProductShopContext context, string inputJson) 
		{
			string result = string.Empty;

			ImportProductDto[]? productDtos = JsonConvert
				.DeserializeObject<ImportProductDto[]>(inputJson);
			if (productDtos != null) 
			{
				ICollection<int> dbUsers = context
					.Users
					.Select(u => u.Id)
					.ToArray();

				ICollection<Product> validProducts = new List<Product>();
				
				foreach (ImportProductDto productDto in productDtos)
				{
					if (!IsValid(productDto))
					{
						continue;
					}

					bool isPriceValid = decimal
						.TryParse(productDto.Price, out decimal productPrice);

					bool isSellerValid = int
						.TryParse(productDto.SellerId, out int sellerId);
					
					if ((!isPriceValid) || (!isSellerValid)) 
					{
						continue;	
					}

					int? buyerId = null;
					
					if (productDto.BuyerId != null) 
					{
						bool isBuyerValid = int
							.TryParse(productDto.BuyerId, out int parsedBuyerId);
						if (!isBuyerValid)
						{
							continue;
						}
						/*if (!dbUsers.Contains(parsedBuyerId)) 
						{
							continue;
						}*/
						buyerId = parsedBuyerId;
					}

					/*if (!dbUsers.Contains(sellerId))
					{
						continue;
					}*/
					Product product = new Product
					
					{
						Name = productDto.Name,
						Price = productPrice,
						SellerId = sellerId,
						BuyerId = buyerId
					};

					validProducts.Add(product);
				}

				context.Products.AddRange(validProducts);
				context.SaveChanges();


				result = $"Successfully imported {validProducts.Count}";
			}
			return result;
		}

		public static string ImportCategories(ProductShopContext context, string inputJson)
		{
			string result = string.Empty;

			ImportCategoryDto[]? categoryDtos = JsonConvert
				.DeserializeObject<ImportCategoryDto[]>(inputJson);

			if (categoryDtos != null)
			{
			     ICollection<Category> validCategories = new List<Category>();
				
				foreach (ImportCategoryDto categoryDto in categoryDtos)
				{
					if (!IsValid(categoryDto)) 
					{
						continue;
					}
					Category category = new Category()
					{
						Name = categoryDto.Name
					};

					validCategories.Add(category);
				}
				context.Categories.AddRange(validCategories);
				context.SaveChanges();

				result = $"Successfully imported {validCategories.Count}";
			}
			return result;
		}

		public static string ImportCategoryProducts(ProductShopContext context, string inputJson) 
		{
			string result = string.Empty;

			ImportCategoryProductDto[]? catProdDtos = JsonConvert
				.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

			if (catProdDtos != null)
			{
				ICollection<int> dbProducts = context
					.Products
					.Select(p => p.Id)
					.ToArray();
				
				ICollection<int> dbCategories = context
					.Categories
					.Select(c => c.Id)
					.ToArray();
				
				ICollection<CategoryProduct> validCatProd = new List<CategoryProduct>();
				foreach (ImportCategoryProductDto catProdDto in catProdDtos)
				{
					if (!IsValid(catProdDto))
					{
						continue;
					}

					bool isProductIdValid = int
						.TryParse(catProdDto.ProductId, out int productId);
					bool isCategoryIdValid = int
						.TryParse(catProdDto.CategoryId, out int categoryId);

					if ((!isProductIdValid) || (!isCategoryIdValid)) 
					{
						continue;
					}

					CategoryProduct catProd = new CategoryProduct()
					{
						ProductId = productId,
						CategoryId = categoryId
					};

					validCatProd.Add(catProd);
				}
				context.CategoriesProducts.AddRange(validCatProd);
				context.SaveChanges();
				result = $"Successfully imported {validCatProd.Count}";
			}
			return result;
		}

		public static string GetProductsInRange(ProductShopContext context) 
		{
			var products = context
				.Products
				.Where(p => p.Price >= 500 && p.Price <= 1000)
				.Select(p => new
				{
					p.Name,
					p.Price,
					Seller = p.Seller.FirstName + " " + p.Seller.LastName
				})
				.OrderBy(p => p.Price)
				.ToArray();

			DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			};

			string jsonResult = JsonConvert
				.SerializeObject(products, Formatting.Indented, new JsonSerializerSettings()
				{
					ContractResolver = camelCaseResolver
				});

			return jsonResult;
		}

		public static string GetSoldProducts(ProductShopContext context) 
		{
			var users = context
				.Users
				.Where(u => u.ProductsSold
				.Any(p => p.BuyerId.HasValue))
				.Select(u => new
				{
					u.FirstName,
					u.LastName,
					SoldProducts = u.ProductsSold
					.Where(p => p.BuyerId.HasValue)
						.Select(p => new
						{
						
								p.Name,
								p.Price,
								BuyerFirstName = p.Buyer!.FirstName,
								BuyerLastName = p.Buyer!.LastName
							})
							.ToArray()
						
				})
				.OrderBy(u => u.LastName)
				.ThenBy(u => u.FirstName)
				.ToArray();
			DefaultContractResolver camelCaseResolver = new DefaultContractResolver()
			{
				NamingStrategy = new CamelCaseNamingStrategy()
			};
			string jsonResult = JsonConvert
				.SerializeObject(users, Formatting.Indented, new JsonSerializerSettings()
				{
					ContractResolver = camelCaseResolver
				});
			return jsonResult;
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