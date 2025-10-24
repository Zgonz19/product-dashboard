﻿namespace ProductAPI.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
