using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Model
{
    public class NewsType
    {
        string Id { get; set; }

        string ParentId { get; set; }

        string stringName { get; set; }

        List<NewsType> children { get; set; }
    }
}
