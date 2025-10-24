using System.Text.Json;
using ProductAPI.Data.Models;

namespace ProductAPI.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!db.Categories.Any())
            {
                var categoryJson = File.ReadAllText("Data/SeedData/categories.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(categoryJson);
                if (categories is not null)
                {
                    await db.Categories.AddRangeAsync(categories);
                    await db.SaveChangesAsync();
                }
            }

            if (!db.Products.Any())
            {
                var productJson = File.ReadAllText("Data/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productJson);
                if (products is not null)
                {
                    await db.Products.AddRangeAsync(products);
                    await db.SaveChangesAsync();
                }
            }
        }

    }
}
