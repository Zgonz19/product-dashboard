﻿namespace ProductAPI.DTOs
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
