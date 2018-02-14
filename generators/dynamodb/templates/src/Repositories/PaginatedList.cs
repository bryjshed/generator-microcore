using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace <%= namespace %>
{
    /// <summary>
    /// Provides pagination functionality for EF record sets.
    /// </summary>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Gets or sets the next key.
        /// </summary>
        /// <returns>An integer.</returns>
        public string NextKey { get; set; }

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
        /// Gets or sets if there is a next page relative to the current page number.
        /// </summary>
        /// <returns>True if there is a next page; false otherwise.</returns>
        public bool HasNextPage => !string.IsNullOrWhiteSpace(NextKey);

        /// <summary>
        /// Gets or sets the Insurances of the Patient.
        /// </summary>
        public List<T> Results { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="PaginatedList{T}"/> class.
        /// </summary>
        /// <param name="items">The list containing the items on the current page.</param>
        /// <param name="lastKey">The current page number.</param>
        /// <param name="pageSize">The maximum number of records to return.</param>
        /// <param name="totalRecords">The total number of records to return.</param>
        /// <param name="filterCount">The filtered number of records to return.</param>
        public PaginatedList(List<T> items, string lastKey, int pageSize, int totalRecords, int filterCount)
        {
            NextKey = lastKey;
            TotalPages = (int)Math.Ceiling(filterCount / (double)pageSize);
            TotalRecords = totalRecords;
            TotalFilterdRecords = filterCount;
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
        /// <param name="lastKey">The current page number.</param>
        /// <param name="pageSize">The maximum number of records to return.</param>
        /// <param name="totalCount">The total number of patient records.</param>
        /// <param name="filterCount">The filtered number of records to return.</param>
        /// <returns>The paginated list.</returns>
        public static PaginatedList<T> CreateAsync(List<T> source, string lastKey, int pageSize, int totalCount, int filterCount)
        {
            return new PaginatedList<T>(source, lastKey, pageSize, totalCount, filterCount);
        }
    }
}