using OrderProcessing.Domain.Models;
using System.Threading.Tasks;

namespace OrderProcessing.Domain.Repositories
{
    public interface IProductsRepository
    {
        Task<bool> ProductExistsAsync(ProductCode code);
        Task<Price> GetProductPriceAsync(ProductCode code);
    }
}
