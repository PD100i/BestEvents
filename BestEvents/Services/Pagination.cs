using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;

namespace BestEvents
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
        public PaginatedResult<T> GetResult(IEnumerable<T> data, int page, int size)
        {
            int totalCount = data.Count();
            var result = data.Skip(page - 1).Take(size);
            return new PaginatedResult<T>(result.ToList(), page, totalCount);
        }
    }
}
