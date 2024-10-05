using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System;
using Newtonsoft.Json;

namespace CINEMA_BE.Utils
{
    public class ApiContext<T> where T : class
    {
        public static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private IQueryable<T> _query;
        private int _totalItem;

        public ApiContext(DbSet<T> dbSet)
        {
            _query = dbSet;
        }
        public int TotalItem()
        {
            _totalItem = _query.Count();
            return _totalItem;
        }

        public ApiContext<T> Filter(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                _query = _query.Where(predicate);
            }
            return this;
        }

        public ApiContext<T> Pagination(int page = 1,int pageSize = 10)
        {
            if (pageSize < 1) pageSize = 10;
            if (page < 1) page = 1;

            int totalPage =  (int)Math.Ceiling((double)_totalItem / pageSize);

            if (page > totalPage && totalPage > 0) // Kiểm tra nếu trang yêu cầu lớn hơn tổng số trang
            {
                throw new ArgumentException($"Page {page} exceeds the total number of pages {totalPage}.");
            }

            int skip = (page - 1) * pageSize;
            _query = _query.Skip(skip).Take(pageSize).AsQueryable();
            return this;
        }

        public ApiContext<T> SortBy<TKey>(Expression<Func<T, TKey>> keySelector, bool descending = false)
        {
            _query = descending ? _query.OrderByDescending(keySelector) : _query.OrderBy(keySelector);
            return this;
        }

        public IEnumerable<TResult> SelectProperties<TResult>(Func<T, TResult> selector)
        {
            return _query.Select(selector);
        }

        // Trả về danh sách kết quả
        public List<T> ToList()
        {
            return _query.ToList();
        }
    }
}
