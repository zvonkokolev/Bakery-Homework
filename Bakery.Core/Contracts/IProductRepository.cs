using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Bakery.Core.Contracts
{
    public interface IProductRepository
    {
        Task<int> GetCountAsync();
        Task AddRangeAsync(IEnumerable<Product> products);
        Task<Product[]> GetAllProductsAsync();
        Task<ProductDto[]> GetAllProductDtosAsync();
        Task<ProductDto[]> GetFilteredProduct(double From, double To);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product selectedProduct);
    }
}