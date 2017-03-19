using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Querying
{
    /// <summary>
    /// Represents that the implemented classes are the collection
    /// that contains a specific page of the records along with
    /// the pagination information.
    /// </summary>
    public interface IPagedResult
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of total records.
        /// </summary>
        /// <value>
        /// The number of total records.
        /// </value>
        long TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        long TotalPages { get; set; }
    }
}
