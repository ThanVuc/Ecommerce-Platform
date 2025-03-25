using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EPlatform_API.Helper
{
    public class PageList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItem { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPage;

        public PageList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalItem = count;
            PageSize = pageSize;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize); // Move this line up
            CurrentPage = pageNumber;
            Console.WriteLine($"TotalItem: {TotalItem}, PageSize: {PageSize}, TotalPage: {TotalPage}, CurrentPage: {CurrentPage}");

            if (CurrentPage > TotalPage)
            {
                CurrentPage = TotalPage;
            }

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            
            AddRange(items);
        }

        public static PageList<T> ToPageList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            // I think it shoulb be a Queryable type so that it will not be execute the query in database
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(items, count, pageNumber, pageSize);
        }

        public void AddPagingInfoToHeader(HttpResponse response)
        {
            var metaData = new
            {
                CurrentPage,
                PageSize,
                TotalItem,
                TotalPage,
                HasNext,
                HasPrevious
            };
            response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metaData));
        }
    }
}