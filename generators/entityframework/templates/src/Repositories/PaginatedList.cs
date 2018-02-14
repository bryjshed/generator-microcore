using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace <%= namespace %>
{
    /// <summary>
    /// Provides pagination functionality for EF record sets.
    /// </summary>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Gets or sets the current page number of the record set being returned.
        /// </summary>
        /// <returns>An integer.</returns>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of pages in the record set.
        /// </summary>
        /// <returns>An integer.</returns>
        public int TotalPages { get; set; }


        /// <summary>
        /// Gets or sets the number of records set.
        /// </summary>
        /// <returns>An integer.</returns>
        public int TotalRecords { get; set; }


        /// <summary>
        /// Gets or sets the number of records set.
        /// </summary>
        /// <returns>An integer.</returns>
        public int TotalFilterdRecords { get; set; }

        /// <summary>
        /// Gets or sets if there is a previous page relative to the current page number.
        /// </summary>
        /// <returns>True if there is a previous page; false otherwise.</returns>
        public bool HasPreviousPage => (PageNumber > 1);

        /// <summary>
        /// Gets or sets if there is a next page relative to the current page number.
        /// </summary>
        /// <returns>True if there is a next page; false otherwise.</returns>
        public bool HasNextPage => (PageNumber < TotalPages);

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public List<T> Results { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="PaginatedList{T}"/> class.
        /// </summary>
        /// <param name="items">The list containing the items on the current page.</param>
        /// <param name="count">The total number of records in the set prior to applying pagination.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The maximum number of records to return.</param>
        /// <param name="totalRecords">The total number of records to return.</param>
        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize, int totalRecords)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalRecords = totalRecords;
            TotalFilterdRecords = count;
            Results = items;
        }

        /// <summary>
        /// Creates an instance of <see cref="PaginatedList{T}"/> class.
        /// </summary>
        public PaginatedList()
        {

        }

        /// <summary>
        /// Creates an instance of the <see cref="PaginatedList{T}"/> class around the record set being passed in.
        /// </summary>
        /// <param name="source">The source record set on which pagination is applied.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The maximum number of records to return.</param>
        /// <param name="totalCount">The total number of records.</param>
        /// <returns>The paginated list.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, int totalCount)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageNumber, pageSize, totalCount);
        }
    }
}