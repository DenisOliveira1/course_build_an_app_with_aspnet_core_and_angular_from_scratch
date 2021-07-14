using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set;} // Página atual
        public int TotalPages { get; set;} // Total de páginas
        public int PageSize { get; set;} // Itens por pagina
        public int TotalCount { get; set;} // Total de itens

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count/(double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber-1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}