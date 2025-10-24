using Riok.Mapperly.Abstractions;
using ProductAPI.Data.Models;

namespace ProductAPI.DTOs
{


    [Mapper]
    public partial class DtoMapper
    {
        public partial ProductDto ToDto(Product product);
        public partial CategoryDto ToDto(Category category);
    }

}
