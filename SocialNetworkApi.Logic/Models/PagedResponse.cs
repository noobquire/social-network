using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetworkApi.Data.Models;

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
            var response = new PagedResponse<T>(pagedMessages, filter.PageNumber, filter.PageSize)
            {
                TotalPages = (int)Math.Ceiling((double)allDataArray.Count() / filter.PageSize),
                TotalRecords = allDataArray.Count()
            };
            return response;
        }
    }
}
