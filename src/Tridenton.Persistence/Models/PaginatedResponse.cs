namespace Tridenton.Persistence;

public class PaginatedResponse<TEntity> : TridentonResponse, IEnumerable<TEntity> where TEntity : class
{
    /// <summary>
    ///     Gets or sets maximum amount of items in current result
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    ///     Gets or sets index of page. By default - 1
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    ///     Gets or sets total amount of elements in the database 
    /// </summary>
    public ulong TotalRecordsCount { get; set; }

    /// <summary>
    ///     Gets or sets total amount of elements in the database, which correspond to filtering conditions 
    /// </summary>
    public ulong TotalFilteredRecordsCount { get; set; }

    /// <summary>
    ///     Get the size of <see cref="Items"/> collection
    /// </summary>
    [JsonInclude]
    public int ItemsCount => Items.Count;

    /// <summary>
    ///     Gets an amount of pages due to total elements count and page size
    /// </summary>
    [JsonInclude]
    public uint Pages
    {
        get
        {
            try
            {
                return (uint)Math.Ceiling(TotalRecordsCount / (double)Size);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    /// <summary>
    ///     Gets or sets index of first item of current page collection within the query. Default value is 1, if collection is empty - 0
    /// </summary>
    [JsonInclude]
    public long StartRowIndex
    {
        get
        {
            long index = (Page - 1) * Size;

            if (Any())
            {
                index++;
            }

            return index;
        }
    }

    /// <summary>
    ///     Index of last item of current page`s collection within the query
    /// </summary>
    [JsonInclude]
    public ulong EndRowIndex => HasNextPage ? (ulong)(Page * Size) : TotalRecordsCount;

    /// <summary>
    ///     Defines whether current page index is more than 1
    /// </summary>
    [JsonInclude]
    public bool HasPreviousPage => Page > PaginationConstants.DefaultPageIndex;

    /// <summary>
    ///     Defines whether current page index is less than total amount of pages
    /// </summary>
    [JsonInclude]
    public bool HasNextPage => Page < Pages;

    /// <summary>
    ///     Gets or sets results collection of current page
    /// </summary>
    public List<TEntity> Items { get; set; }

    /// <summary>
    ///     Initializes new instance of <see cref="PaginatedResponse{TEntity}"/>
    /// </summary>
    public PaginatedResponse() : base()
    {
        Size = 0;
        Page = 0;
        TotalRecordsCount = 0;

        Items = new();
    }

    /// <summary>
    ///     Defines whether <see cref="Items"/> collection is not empty
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="Items"/> collection is not empty; otherwise, <see langword="false" />
    /// </returns>
    public bool Any() => ItemsCount != 0;

    public IEnumerator<TEntity> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
}