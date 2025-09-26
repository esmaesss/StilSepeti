using StilSepetiApp.DTO;

namespace StilSepetiApp.Services
{
    public interface IProductService
    {
        
        Task<Productdto> CreateProductAsync(ProductCreatedto dto);
        Task<Productdto?> GetProductByIdAsync(int id);
        Task<ServiceResult> UpdateProductAsync(int id, ProductUpdatedto dto, int sellerId);
        Task<ServiceResult> DeleteProductAsync(int id, int sellerId);

        
        Task<PagedResult<Productdto>> GetProductsAsync(ProductFilter filter);
        Task<PagedResult<Productdto>> GetSellerProductsAsync(int sellerId, int pageNumber = 1, int pageSize = 20);

        
        Task<List<string>> GetBrandsAsync();
        Task<List<string>> GetCategoriesAsync();

        
        Task<ServiceResult> UpdateStockAsync(int productId, int newStock, int sellerId);
    }
}