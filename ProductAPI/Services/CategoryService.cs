using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Data.Models;
using ProductAPI.DTOs;

namespace ProductAPI.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetActiveCategories();
        Task<CategoryDto> CreateCategory(CreateCategoryDto dto);
    }

    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;
        private readonly DtoMapper _mapper;
        public CategoryService(AppDbContext db, DtoMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategories()
        {
            var categories = await _db.Categories
                .Where(c => c.IsActive)
                .AsNoTracking()
                .ToListAsync();

            return categories.Select(c => _mapper.ToDto(c));
        }

        public async Task<CategoryDto> CreateCategory(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(category);
        }
    }
}
