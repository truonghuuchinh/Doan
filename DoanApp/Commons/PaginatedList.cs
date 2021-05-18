using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class PaginatedList<T>:List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
        public PaginatedList(List<T> items,int count,int pageIndex,int pageSize)
        {
            PageIndex = pageIndex;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }
        public bool PreviousPage {
            get
            {
                return PageIndex > 1;
            }
        }
        public bool NextPage
        {
            get
            {
                return (PageIndex < TotalPage);
            }
        }
        public static  PaginatedList<T> CreateAsync(IQueryable<T> source,int pageIndex,int pageSize)
        {
            var count =  source.Count();
            var item =  source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return   new  PaginatedList<T>(item, count, pageIndex, pageSize);
        }
    }
}
