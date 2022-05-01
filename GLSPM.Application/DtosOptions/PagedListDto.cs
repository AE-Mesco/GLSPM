using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos
{
    public class PagedListDto<TData> : ListResultDto<TData>, IPagedResult<TData>
    {
        public PagedListDto()
        {

        }
        public PagedListDto(int totalCount, IReadOnlyList<TData> items):base(items)
        {
            TotalCount = totalCount;
            Items = items;
        }
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Error { get; set; }
        public int StatusCode { get; set; }
    }
}
