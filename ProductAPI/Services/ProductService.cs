using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Data.Models;
using ProductAPI.DTOs;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetActiveProducts();
        Task<ProductDto?> GetProductById(int id);
        Task<ProductDto> CreateProduct(CreateProductDto dto);
        Task<ProductDto?> UpdateProduct(int id, UpdateProductDto dto);
        Task<bool> DeleteProductById(int id);
        Task<PagedResult<ProductDto>> SearchProducts(ProductSearchQuery query);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly DtoMapper _mapper;
        public ProductService(AppDbContext db, DtoMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetActiveProducts()
        {
            var products = await _db.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            return products.Select(p => _mapper.ToDto(p));
        }

        public async Task<ProductDto?> GetProductById(int id)
        {
            var product = await _db.Products
                .Where(p => p.Id == id && p.IsActive)
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return product is null ? null : _mapper.ToDto(product);
        }

        public async Task<ProductDto> CreateProduct(CreateProductDto dto)
        {
            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if (category is null)
                throw new ArgumentException("Invalid category ID");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = category.Id,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return _mapper.ToDto(product);
        }

        public async Task<ProductDto?> UpdateProduct(int id, UpdateProductDto dto)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product is null)
                return null;

            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if (category is null)
                throw new ArgumentException("Invalid category ID");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = category.Id;
            product.StockQuantity = dto.StockQuantity;
            await _db.SaveChangesAsync();

            return _mapper.ToDto(product);
        }

        public async Task<bool> DeleteProductById(int id)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
            if (product is null)
                return false;

            product.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<ProductDto>> SearchProducts(ProductSearchQuery query)
        {
            var products = _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            if (query.CategoryId.HasValue)
                products = products.Where(p => p.CategoryId == query.CategoryId);
            if (query.MinPrice.HasValue)
                products = products.Where(p => p.Price >= query.MinPrice);
            if (query.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= query.MaxPrice);
            if (query.InStock.HasValue)
                products = products.Where(p => query.InStock.Value ? p.StockQuantity > 0 : p.StockQuantity == 0);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var terms = query.SearchTerm
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                products = terms.Aggregate(products, (current, term) =>
                current.Where(p =>
                    EF.Functions.Like(p.Name, $"%{term}%") ||
                    EF.Functions.Like(p.Description ?? "", $"%{term}%")));
            }

            var sortBy = query.SortBy?.ToLower() ?? "name";
            var sortOrder = query.SortOrder?.ToLower() ?? "asc";
            products = (sortBy, sortOrder) switch
            {
                ("name", "asc") => products.OrderBy(p => p.Name),
                ("name", "desc") => products.OrderByDescending(p => p.Name),
                ("price", "asc") => products.OrderBy(p => (double)p.Price),
                ("price", "desc") => products.OrderByDescending(p => (double)p.Price),
                ("stock", "asc") => products.OrderBy(p => p.StockQuantity),
                ("stock", "desc") => products.OrderByDescending(p => p.StockQuantity),
                _ => products.OrderBy(p => p.Name)
            };

            var skip = (query.PageNumber - 1) * query.PageSize;
            var take = query.PageSize;
            var items = await products
                .Skip(skip)
                .Take(take)
                .Select(p => _mapper.ToDto(p))
                .ToListAsync();

            var totalCount = await products.CountAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}
