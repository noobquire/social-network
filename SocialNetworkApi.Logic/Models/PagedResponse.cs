using SocialNetworkApi.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkApi.Services.Models
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
        }

        public static PagedResponse<T> CreatePagedResponse(IEnumerable<T> allData, PaginationFilter filter)
        {
            var allDataArray = allData.ToArray();
            var pagedMessages =
                allDataArray.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);
            var totalPages = (int)Math.Ceiling((double)allDataArray.Length / filter.PageSize);

            var pageNumber = filter.PageNumber > totalPages ? totalPages : filter.PageNumber;
            var response = new PagedResponse<T>(pagedMessages, pageNumber, filter.PageSize)
            {
                TotalPages = totalPages,
                TotalRecords = allDataArray.Length
            };
            return response;
        }
    }
}
