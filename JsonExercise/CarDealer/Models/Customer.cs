namespace CarDealer.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime BirthDate { get; set; }

		public bool IsYoungDriver
		{
			get
			{
				var age = DateTime.Now.Year - BirthDate.Year;
				if (DateTime.Now < BirthDate.AddYears(age)) age--;
				return age < 2; 
			}
		}

		public virtual ICollection<Sale> Sales { get; set; } 
                   = new HashSet<Sale>(); 
    }
}