using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class Category : EntityBase
    {
        public required string CategoryName { get; set; }

        public Category()
        {
            
        }

        public Category(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}
