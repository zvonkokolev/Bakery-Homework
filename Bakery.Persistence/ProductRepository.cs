using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bakery.Persistence
{
  public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetCountAsync()
            => await _dbContext.Products.CountAsync()
            ;
        public async Task AddRangeAsync(IEnumerable<Product> products)
            => await _dbContext.Products.AddRangeAsync(products)
            ;
        public async Task<Product[]> GetAllProductsAsync()
            => await _dbContext.Products
            .OrderBy(p => p.Name)
            .ToArrayAsync()
            ;
        public async Task<ProductDto[]> GetAllProductDtosAsync()
            => await _dbContext.Products
            .Include(oi => oi.OrderItems)
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(p))
            .ToArrayAsync()
            ;

        public async Task<ProductDto[]> GetFilteredProduct(double From, double To)
        {
            var a = await GetAllProductDtosAsync();
            return a
                .Where(p => p.Price >= From && p.Price <= To)
                .ToArray()
                ;
        }
    }
}
