using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Tridenton.Extensions.Persistence;

public static class PaginationExtensions
{
    /// <summary>
    ///     Asynchronously filters, sorts and divides <paramref name="source"/> into paginated response
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="request">Request instance</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the <see cref="PaginatedResponse{TEntity}"/>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidExpressionOperatorException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public static async ValueTask<PaginatedResponse<TEntity>> ToPaginatedResponseAsync<TEntity>(this IQueryable<TEntity> source, PaginatedRequest<TEntity> request, CancellationToken cancellationToken = default) where TEntity : class
    {
        var totalRecords = await source.LongCountAsync(cancellationToken);

        source = source.FilterQuery(request).OrderQuery(request);

        var totalFilteredRecords = await source.LongCountAsync(cancellationToken);

        source = await DivideQueryIntoPagesAsync(source, request, cancellationToken);

        var items = await source.ToListAsync(cancellationToken);

        var response = new PaginatedResponse<TEntity>
        {
            Items = items,
            TotalRecordsCount = (ulong)totalRecords,
            TotalFilteredRecordsCount = (ulong)totalFilteredRecords,
            Page = request.Page,
            Size = request.Size,
        };

        return response;
    }

    /// <summary>
    ///     Filters <paramref name="source"/> by specified conditions
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="request">Pagination request</param>
    /// <returns>
    ///     A filtered <see cref="IQueryable{T}"/>
    /// </returns>
    /// <exception cref="InvalidExpressionOperatorException"></exception>
    public static IQueryable<TEntity> FilterQuery<TEntity>(this IQueryable<TEntity> source, PaginatedRequest<TEntity> request) where TEntity : class
    {
        var filters = request.Filtering.Where(f => !f.IsEmpty()).ToArray();

        if (!filters.Any()) return source;

        var entityType = typeof(TEntity);

        var predicateParts = new List<string>();
        var predicateValues = new List<object>();

        var filtersGroup = filters.GroupBy(f => f.Property).ToArray();

        var currentFilterIndex = 0;

        for (var groupIndex = 0; groupIndex < filtersGroup.Length; groupIndex++)
        {
            var filterGroup = filtersGroup[groupIndex];

            filters = filterGroup.Select(fg => fg).ToArray();

            var property = entityType.GetProperty(filterGroup.Key);

            ArgumentNullException.ThrowIfNull(property, $"Property {filterGroup.Key} does not exist");

            var filtersCount = filters.LongLength;

            for (long i = 0; i < filtersCount; i++)
            {
                var predicatePart = "(";

                var filter = filters[i];

                if (property.PropertyType.BaseType == typeof(Enumeration))
                {
                    var config = new ParsingConfig(property.PropertyType, filter, ExpressionOperator.GetEnumOperators());

                    var values = ParseEnum(config);

                    predicatePart += string.Join(PaginationConstants.Or, values.Select(v =>
                    {
                        var predicate = filter.Operator.FormatExpression(filter.Property, currentFilterIndex);
                        currentFilterIndex++;

                        return predicate;
                    }));

                    predicateValues.AddRange(values);
                }
                else
                {
                    if (property.PropertyType == typeof(string))
                    {
                        if (!ExpressionOperator.GetStringOperators().Contains(filter.Operator)) throw new InvalidExpressionOperatorException(filter);

                        if (filter.Operator == ExpressionOperator.Contains)
                        {
                            var values = filter.GetValues(false);

                            predicatePart += string.Join(PaginationConstants.Or, values.Select(v =>
                            {
                                var predicate = filter.Operator.FormatExpression(filter.Property, currentFilterIndex);
                                currentFilterIndex++;

                                return predicate;
                            }));

                            predicateValues.AddRange(values);
                        }
                        else
                        {
                            var values = filter.GetValues();

                            predicatePart += filter.Operator.FormatExpression(property.Name, Enumerable.Range(currentFilterIndex, values.Length).ToArray());
                            currentFilterIndex += values.Length;

                            predicateValues.AddRange(values);
                        }
                    }
                    else
                    {
                        var typeCode = Type.GetTypeCode(property.PropertyType);

                        if (_typeHandlers.TryGetValue(typeCode, out var handler))
                        {
                            var values = handler.Invoke(property.PropertyType, filter);

                            if (filter.Operator == ExpressionOperator.Between)
                            {
                                predicatePart += filter.Operator.FormatBetweenExpression(filter.Property, currentFilterIndex, currentFilterIndex + 1);
                                currentFilterIndex += filter.Operator.ParamsCount;
                            }
                            else
                            {
                                predicatePart += string.Join(PaginationConstants.And, values.Select(v =>
                                {
                                    var predicate = filter.Operator.FormatExpression(filter.Property, currentFilterIndex);
                                    currentFilterIndex++;

                                    return predicate;
                                }));
                            }

                            predicateValues.AddRange(values);
                        }
                    }
                }

                predicatePart += ")";

                predicateParts.Add(predicatePart);
            }
        }

        var predicate = string.Join(PaginationConstants.And, predicateParts);

        source = source.Where(predicate, predicateValues.ToArray());

        return source;
    }

    /// <summary>
    ///     Orders <paramref name="source"/> by specified property and direction
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="request">Pagination request</param>
    /// <returns>
    ///     A re-ordered <see cref="IQueryable{T}"/>
    /// </returns>
    public static IQueryable<TEntity> OrderQuery<TEntity>(this IQueryable<TEntity> source, PaginatedRequest<TEntity> request) where TEntity : class
    {
        if (request.Ordering is null || request.Ordering.IsEmpty()) return source;

        source = source.OrderBy(request.Ordering.OrderBy);

        return request.Ordering.Direction == OrderingDirection.Descending ? source.Reverse() : source;
    }

    /// <summary>
    ///     Asynchronously calculates the amount of pages into which the <paramref name="source"/> will be divided according to <paramref name="request"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="request">Pagination request</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the amount of pages into which the <paramref name="source"/> will be divided according to <paramref name="request"/>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public static async ValueTask<int> GetQueryPagesAsync<TEntity>(this IQueryable<TEntity> source, PaginatedRequest<TEntity> request, CancellationToken cancellationToken = default) where TEntity : class
    {
        var recordsCount = await source.LongCountAsync(cancellationToken);

        var pages = (int)Math.Ceiling(recordsCount / (double)request.Size);

        return pages < 1 ? 1 : pages;
    }

    /// <summary>
    ///     Asynchronously retrieves a specific amount of items at specific page, specified at <paramref name="request"/>, from <paramref name="source"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source">Input source</param>
    /// <param name="request">Pagination request</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an <see cref="IQueryable{T}"/> of specific amount of items at specific page, specified at <paramref name="request"/>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public static async ValueTask<IQueryable<TEntity>> DivideQueryIntoPagesAsync<TEntity>(IQueryable<TEntity> source, PaginatedRequest<TEntity> request, CancellationToken cancellationToken = default) where TEntity : class
    {
        var pages = await source.GetQueryPagesAsync(request, cancellationToken);

        request.Page = pages < request.Page ? pages : request.Page;

        source = source.Skip((request.Page - 1) * request.Size).Take(request.Size);

        return source;
    }

    #region Private members

    private static readonly Dictionary<TypeCode, Func<Type, FilteringExpression, object[]>> _typeHandlers = new()
    {
        { TypeCode.Boolean,  (property, expression) => ParseBool(new (property, expression, ExpressionOperator.GetBooleanOperators()))        },
        { TypeCode.Byte,     (property, expression) => Parse<byte>(new (property, expression, ExpressionOperator.GetNumericOperators()))      },
        { TypeCode.Char,     (property, expression) => Parse<char>(new (property, expression, ExpressionOperator.GetEnumOperators()))         },
        { TypeCode.DateTime, (property, expression) => Parse<DateTime>(new (property, expression, ExpressionOperator.GetDateTimeOperators())) },
        { TypeCode.Decimal,  (property, expression) => Parse<decimal>(new (property, expression, ExpressionOperator.GetNumericOperators()))   },
        { TypeCode.Double,   (property, expression) => Parse<double>(new (property, expression, ExpressionOperator.GetNumericOperators()))    },
        { TypeCode.Int16,    (property, expression) => Parse<short>(new (property, expression, ExpressionOperator.GetNumericOperators()))     },
        { TypeCode.Int32,    (property, expression) => Parse<int>(new (property, expression, ExpressionOperator.GetNumericOperators()))       },
        { TypeCode.Int64,    (property, expression) => Parse<long>(new (property, expression, ExpressionOperator.GetNumericOperators()))      },
        { TypeCode.SByte,    (property, expression) => Parse<sbyte>(new (property, expression, ExpressionOperator.GetNumericOperators()))     },
        { TypeCode.Single,   (property, expression) => Parse<float>(new (property, expression, ExpressionOperator.GetNumericOperators()))     },
        { TypeCode.UInt16,   (property, expression) => Parse<ushort>(new (property, expression, ExpressionOperator.GetNumericOperators()))    },
        { TypeCode.UInt32,   (property, expression) => Parse<uint>(new (property, expression, ExpressionOperator.GetNumericOperators()))      },
        { TypeCode.UInt64,   (property, expression) => Parse<ulong>(new (property, expression, ExpressionOperator.GetNumericOperators()))     },
    };

    private static object[] Parse<TValue>(ParsingConfig config) where TValue : IParsable<TValue>
    {
        var values = config.GetValues();

        var result = new List<object>();

        for (long i = 0; i < values.LongLength; i++)
        {
            var value = values[i];

            if (!TValue.TryParse(value, CultureInfo.InvariantCulture, out TValue? parsed)) throw new InvalidFilterArgumentException(value, config.Expression);

            result.Add(parsed);
        }

        return result.ToArray();
    }

    private static object[] ParseBool(ParsingConfig config)
    {
        var values = config.GetValues();

        var result = new List<object>();

        for (long i = 0; i < values.LongLength; i++)
        {
            var value = values[i];

            if (!bool.TryParse(value, out bool parsed)) throw new InvalidFilterArgumentException(value, config.Expression);

            result.Add(parsed);
        }

        return result.ToArray();
    }

    private static object[] ParseEnum(ParsingConfig config)
    {
        var values = config.GetValues(false);

        var result = new List<object>();

        for (long i = 0; i < values.LongLength; i++)
        {
            var value = values[i];

            var enumeration = Enumeration.GetValue(config.Property, value);

            if (enumeration is null) throw new InvalidFilterArgumentException(value, config.Expression);

            result.Add(enumeration);
        }

        return result.ToArray();
    }

    record ParsingConfig(Type Property, FilteringExpression Expression, ExpressionOperator[] ValidOperations)
    {
        internal string[] GetValues(bool verifyParametersCount = true)
        {
            if (!ValidOperations.Contains(Expression.Operator)) throw new InvalidExpressionOperatorException(Expression);

            return Expression.GetValues(verifyParametersCount);
        }
    }

    #endregion
}