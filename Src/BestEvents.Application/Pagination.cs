
using BestEvents.Domain.Exceptions;

namespace BestEvents.Application
{
    /// <summary>
    /// Класс для пагинации данных
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    public class Pagination<T> 
    {
        /// <summary>
        /// Метод для получения результатов пагинации. Принимает на вход массив данных, номер страницы и количество элементов на странице.
        /// </summary>
        /// <param name="data">Массив для пагинации</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="size">Количество элементов на странице</param>
        /// <returns></returns>
        public PaginatedResult<T> GetResult(IQueryable<T>? data, int page, int size)
        {
            
            if (page <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongPageForPagination, page));
            if (size <= 0)
                throw new EventWrongParameterException(string.Format(Messages_ru.WrongSizeForPagination, size));
            if (data == null)
                return new PaginatedResult<T>([], page, 0);
            int totalCount = data.Count();
            if (totalCount == 0)
                return new PaginatedResult<T>([], page, 0);
            var result = data.Skip((page - 1)*size).Take(size).ToList();
            return new PaginatedResult<T>(result, page, totalCount);
        }
    }
}
