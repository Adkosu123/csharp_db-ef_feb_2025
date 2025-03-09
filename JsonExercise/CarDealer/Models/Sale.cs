namespace CarDealer.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public decimal Discount { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; } = null!;    

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!; 

		public decimal FinalPrice
		{
			get
			{
	
				decimal carPrice = Car.TotalPrice;

				decimal finalDiscount = Discount;

				if (Customer.IsYoungDriver)
				{
					finalDiscount += 5;
				}

				return carPrice * (1 - finalDiscount / 100);
			}
		}
	}
}

