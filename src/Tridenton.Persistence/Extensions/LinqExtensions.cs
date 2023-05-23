using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Tridenton.Extensions.Persistence;

public static class LinqExtensions
{
    /// <summary>
    ///     Orders <paramref name="source"/> by specified property
    /// </summary>
    /// <param name="source">Input source</param>
    /// <param name="ordering">Ordering property</param>
    /// <param name="args">Optional arguments</param>
    /// <returns>
    ///     An re-ordered <see cref="IOrderedQueryable"/>
    /// </returns>
    public static IOrderedQueryable OrderBy(this IQueryable source, string ordering, params object[] args)
    {
        return DynamicQueryableExtensions.OrderBy(source, ordering, args);
    }

    /// <summary>
    ///     Orders <paramref name="source"/> by specified property
    /// </summary>
    /// <param name="source">Input source</param>
    /// <param name="ordering">Ordering property</param>
    /// <param name="args">Optional arguments</param>
    /// <returns>
    ///     <see cref="IOrderedQueryable"/>
    /// </returns>
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string ordering, params object[] args) where TEntity : class
    {
        return DynamicQueryableExtensions.OrderBy(source, ordering, args);
    }

    /// <summary>
    ///     Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">Input source</param>
    /// <returns>
    ///     Default if source is empty; otherwise, the first element in source
    /// </returns>
    public static dynamic FirstOrDefault(this IQueryable source)
    {
        return DynamicQueryableExtensions.FirstOrDefault(source);
    }

    /// <summary>
    ///     Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">Input source</param>
    /// <returns>
    ///     Default if source is empty; otherwise, the last element in source
    /// </returns>
    public static dynamic LastOrDefault(this IQueryable source)
    {
        return DynamicQueryableExtensions.LastOrDefault(source);
    }

    /// <summary>
    ///     Filters <paramref name="source"/> by specified <paramref name="predicate"/> and arguments
    /// </summary>
    /// <param name="source">Input source</param>
    /// <param name="predicate">Filtering expression predicate</param>
    /// <param name="args">Filtering values</param>
    /// <returns>
    ///     A filtered <see cref="IQueryable" />
    /// </returns>
    public static IQueryable Where(this IQueryable source, string predicate, params object?[] args)
    {
        return DynamicQueryableExtensions.Where(source, predicate, args);
    }

    /// <summary>
    ///     Filters <paramref name="source"/> by specified <paramref name="predicate"/> and arguments
    /// </summary>
    /// <param name="source">Input source</param>
    /// <param name="predicate">Filtering expression predicate</param>
    /// <param name="args">Filtering values</param>
    /// <returns>
    ///     A filtered <see cref="IQueryable{TEntity}" />
    /// </returns>
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, string predicate, params object?[] args) where TEntity : class
    {
        return DynamicQueryableExtensions.Where(source, predicate, args);
    }

    /// <summary>
    ///     Inverts the order of the elements in a sequence
    /// </summary>
    /// <param name="source">Input source</param>
    /// <returns>
    ///     A <see cref="IQueryable"/> whose elements correspond to those of the input sequence in reverse order.
    /// </returns>
    public static IQueryable Reverse(this IQueryable source)
    {
        return DynamicQueryableExtensions.Reverse(source);
    }

    /// <summary>
    ///     Inverts the order of the elements in a sequence
    /// </summary>
    /// <param name="source">Input source</param>
    /// <returns>
    ///     A <see cref="IQueryable{TEntity}"/> whose elements correspond to those of the input sequence in reverse order.
    /// </returns>
    public static IQueryable<TEntity> Reverse<TEntity>(this IQueryable<TEntity> source)
    {
        return source.Reverse<TEntity>();
    }

    /// <summary>
    ///     Counts the total amount of elements in <paramref name="source"/>
    /// </summary>
    /// <param name="source">Input source</param>
    /// <returns>
    ///     Total amount of elements in <paramref name="source"/>.
    /// </returns>
    public static int Count(this IQueryable source)
    {
        return DynamicQueryableExtensions.Count(source);
    }

    /// <summary>
    ///     Asynchronously returns a <see langword="long"/> that represents the total number of elements in a sequence.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the number of elements in the input sequence.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<int> CountAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.CountAsync(source, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                return source.Count();
            }

            throw;
        }
    }

    /// <summary>
    ///     Returns a <see langword="long"/> that represents the total number of elements in a sequence.
    /// </summary>
    /// <param name="source">Input source</param>
    /// <returns>
    ///     The number of elements in the input sequence.
    /// </returns>
    public static long LongCount(this IQueryable source)
    {
        return DynamicQueryableExtensions.LongCount(source);
    }

    /// <summary>
    ///     Asynchronously returns a <see langword="long"/> that represents the total number of elements in a sequence.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the number of elements in the input sequence.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<long> LongCountAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.LongCountAsync(source, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.LongCount();
            }

            throw;
        }
    }

    /// <summary>
    ///     Asynchronously creates a <see cref="List{T}"/> from an <see cref="IQueryable{T}"/> by enumerating it asynchronously.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a <see cref="List{T}"/> that contains elements from the input sequence.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<List<TEntity>> ToListAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.ToListAsync(source, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.ToList();
            }

            throw;
        }
    }

    /// <summary>
    ///     Asynchronously creates an array from an <see cref="IQueryable{T}"/> by enumerating it asynchronously.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an array that contains elements from the input sequence.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<TEntity[]> ToArrayAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.ToArrayAsync(source, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.ToArray();
            }

            throw;
        }
    }

    /// <summary>
    ///     Asynchronously determines whether a sequence contains any elements.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the source sequence contains any elements; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<bool> AnyAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.AnyAsync(source, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.Any();
            }

            throw;
        }
    }

    /// <summary>
    ///     Asynchronously determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains <see langword="true"/> if any elements in the source sequence pass the test in the specified predicate; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<bool> AnyAsync<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.AnyAsync(source, predicate, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.Any(predicate);
            }

            throw;
        }
    }

    /// <summary>
    ///     Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains <see langword="default"/> (<typeparamref name="TEntity"/>) if <paramref name="source"/> is empty; otherwise, the first element in <paramref name="source"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<TEntity?> FirstOrDefaultAsync<TEntity>(this IQueryable<TEntity> source, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(source, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.FirstOrDefault();
            }

            throw;
        }
    }

    /// <summary>
    ///     Asynchronously returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains <see langword="default"/> (<typeparamref name="TEntity"/>) if <paramref name="source"/> is empty or if no element passes the test specified by <paramref name="predicate"/>,
    ///     otherwise, the first element in <paramref name="source"/> that passes the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="OperationCanceledException" />
    public static async ValueTask<TEntity?> FirstOrDefaultAsync<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
    {
        try
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(source, predicate, cancellationToken);
        }
        catch (Exception exception)
        {
            if (IsNotImplementedOrNotSupported(exception))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return source.FirstOrDefault(predicate);
            }

            throw;
        }
    }

    #region Private members

    private static bool IsNotImplementedOrNotSupported(Exception exception) => exception is NotImplementedException or NotSupportedException or InvalidOperationException;

    #endregion
}