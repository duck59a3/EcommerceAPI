using MyWebApi.Logs;
using MyWebApi.Repository.IRepository;
using MyWebApi.Responses;
using MyWebApi.Services.IService;
using System.Linq.Expressions;

namespace MyWebApi.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<T> _repository;
        public GenericService(IUnitOfWork unitOfWork, IRepository<T> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<Response> CreateAsync(T entity)
        {
            try
            {   
                await _unitOfWork.Repository<T>().AddAsync(entity);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Thêm thành công");
            }
            catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, $"Thêm thất bại ,{ex.Message} ");
            }

        }

        public async Task<Response> DeleteAsync(T entity)
        {
            try
            {
                var entityexist = await _repository.FindByAsync(x => x.Equals(entity));
                if (entityexist is null)
                {
                    return new Response(false, "Không tìm thấy đối tượng để xóa");
                }
                 await _unitOfWork.Repository<T>().RemoveAsync(entity);
                await _unitOfWork.SaveAsync();
                return new Response(true, "Xóa thành công");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Xóa thất bại");
            }
        }

        public async Task<T> FindByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<T>().GetByIdAsync(id);
                return entity is not null ? entity : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Lỗi khi tìm kiếm đối tượng theo ID");
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try 
            {
                var list = await _unitOfWork.Repository<T>().GetAllAsync();
                return list is not null ? list : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Lỗi khi lấy tất cả đối tượng");
            }
        }

        //public async Task<T> GetByAsync(Expression<Func<T, bool>> predicate)
        //{
            
        //}

        //public Task<T> GetByIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
