using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using BestEvents.Exceptions;

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
            if (data == null)
                return new PaginatedResult<T>([], 1, 0, 0);
            if (page <= 0)
                throw new PaginationWromgParameterException($"Попытка пагинации с недопустимым значением номера страницы (page={page})");
            if (size <= 0)
                throw new PaginationWromgParameterException($"Попытка пагинации с недопустимым значением размера выборки на странице (size={size})");
            int totalCount = data.Count();
            var result = data.Skip(page - 1).Take(size);
            return new PaginatedResult<T>([.. result], page, result.Count(), totalCount);
        }
    }
}
