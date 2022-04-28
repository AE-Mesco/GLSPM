using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos
{
    public class GetListDto
    {
        public string Filter { get; set; }
        public string Sorting { get; set; }
        public int? SkipCount { get; set; } = 0;
        public int? MaxResults { get; set; } = 100;
    }
}
