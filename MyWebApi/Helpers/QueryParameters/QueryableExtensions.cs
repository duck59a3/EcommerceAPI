using System.Linq.Expressions;
using System.Reflection;

namespace MyWebApi.Helpers.QueryParameters
{
    public static class QueryableExtensions
    {
        //Phân trang
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        //Sắp xếp class T theo ....
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;

            var propertyInfo = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = sortOrder?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        //Tìm kiếm theo
        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string? searchTerm, params string[] searchProperties)
        {
            if (string.IsNullOrEmpty(searchTerm) || !searchProperties.Any())
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var propertyName in searchProperties)
            {
                var property = Expression.Property(parameter, propertyName);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(property, containsMethod, Expression.Constant(searchTerm));

                combinedExpression = combinedExpression == null
                    ? containsExpression
                    : Expression.OrElse(combinedExpression, containsExpression);
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }
    }
}
