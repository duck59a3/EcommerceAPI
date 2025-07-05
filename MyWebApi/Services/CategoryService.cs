using Microsoft.AspNetCore.Http.HttpResults;
using MyWebApi.Logs;
using MyWebApi.Models;
using MyWebApi.Repository;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System.Threading.Tasks;

namespace MyWebApi.Services
{
    public class CategoryService : GenericService<Category>, ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _repository;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository repository) : base(unitOfWork, repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<Response> UpdateAsync(Category category)
        {
            try
            {
                var existingCate = await _repository.GetByIdAsync(category.Id);
                if (existingCate == null)
                {
                    return new Response(false, $"Không tìm thấy loại hàng để cập nhật");
                }

                existingCate.Name = category.Name;
                existingCate.Description = category.Description;

                await _repository.UpdateAsync(existingCate);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Product updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $"Cập nhật thất bại vì {ex.Message}");
            }
        }
    }
}
