namespace Tridenton.Persistence;

internal readonly struct PaginationConstants
{
    internal const int DefaultPageIndex = 1;
    internal const int DefaultPageSize = 25;
    internal const char FilteringValuesDelimiter = ',';
    internal const string Or = " OR ";
    internal const string And = " AND ";
}

public class PaginatedRequest<TEntity> : PaginatedRequest<PaginatedResponse<TEntity>, TEntity> where TEntity : class { }

public class PaginatedRequest<TResponse, TEntity> : TridentonRequest<TResponse> where TResponse : PaginatedResponse<TEntity> where TEntity : class
{
    /// <summary>
    ///     Index of page to display
    /// </summary>
    [TridentonProperty(Required = true, Min = PaginationConstants.DefaultPageIndex, Max = int.MaxValue, ErrorMessage = $"Page is required and should be between 1 and 2147483647")]
    public int Page { get; set; }

    /// <summary>
    ///     Amount of elements to display per one page
    /// </summary>
    [TridentonProperty(Required = true, Min = 1, Max = int.MaxValue, ErrorMessage = $"Size is required and should be between 1 and 2147483647")]
    public int Size { get; set; }

    /// <summary>
    ///     Ordering specification
    /// </summary>
    public Ordering Ordering { get; set; }

    /// <summary>
    ///     Filtering expressions
    /// </summary>
    public FilteringExpression[] Filtering { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="PaginatedRequest{TResponse, TEntity}"/>
    /// </summary>
    public PaginatedRequest()
    {
        Page = PaginationConstants.DefaultPageIndex;
        Size = PaginationConstants.DefaultPageSize;

        Ordering = new();
        Filtering = Array.Empty<FilteringExpression>();
    }
}

/// <summary>
///     Ordering specification
/// </summary>
public sealed class Ordering
{
    /// <summary>
    ///     Name of property to order by
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    ///     Ordering direction. Default value - <see cref="OrderingDirection.Ascending"/>
    /// </summary>
    public OrderingDirection Direction { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="Ordering"/>
    /// </summary>
    public Ordering()
    {
        OrderBy = string.Empty;
        Direction = OrderingDirection.Ascending;
    }

    internal bool IsEmpty() => OrderBy.IsEmpty();
}

/// <summary>
///     Filtering expression
/// </summary>
public sealed class FilteringExpression
{
    /// <summary>
    ///     Name of property
    /// </summary>
    public string Property { get; set; }

    /// <summary>
    ///     Filtering value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Expression operator
    /// </summary>
    public ExpressionOperator Operator { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="FilteringExpression"/>
    /// </summary>
    public FilteringExpression()
    {
        Property = Value = string.Empty;
        Operator = ExpressionOperator.None;
    }

    internal bool IsEmpty() => Property.IsEmpty() || Value.IsEmpty() || Operator == ExpressionOperator.None;

    internal string[] GetValues(bool verifyParametersCount = true)
    {
        var values = Value.Split(PaginationConstants.FilteringValuesDelimiter);

        if (verifyParametersCount && !values.Length.Equals(Operator.ParamsCount)) throw new ArgumentException($"Invalid amount of parameters for {Property}");

        return values;
    }

    public override string ToString() => $"{Property} {Operator} '{Value}'";
}

[TypeConverter(typeof(EnumerationTypeConverter<OrderingDirection>))]
[JsonConverter(typeof(EnumerationJsonConverter<OrderingDirection>))]
public record OrderingDirection : Enumeration
{
    protected OrderingDirection(string value) : base(value) { }

    public static readonly OrderingDirection Ascending = new("Ascending");
    public static readonly OrderingDirection Descending = new("Descending");
}

[TypeConverter(typeof(EnumerationTypeConverter<ExpressionOperator>))]
[JsonConverter(typeof(EnumerationJsonConverter<ExpressionOperator>))]
public sealed record ExpressionOperator : Enumeration
{
    internal readonly string Expression;
    internal readonly int ParamsCount;

    internal ExpressionOperator(string value, string expression, int paramsCount) : base(value)
    {
        Expression = expression;
        ParamsCount = paramsCount;
    }

    public static readonly ExpressionOperator None = new(nameof(None), string.Empty, 0);
    public static readonly ExpressionOperator Is = new(nameof(Is), "{0} = @{1}", 1);
    public static readonly ExpressionOperator Not = new(nameof(Not), "{0} != @{1}", 1);
    public static readonly ExpressionOperator Equal = new(nameof(Equal), "{0} = @{1}", 1);
    public static readonly ExpressionOperator NotEqual = new(nameof(NotEqual), "{0} != @{1}", 1);
    public static readonly ExpressionOperator Greater = new(nameof(Greater), "{0} > @{1}", 1);
    public static readonly ExpressionOperator GreaterOrEqual = new(nameof(GreaterOrEqual), "{0} >= @{1}", 1);
    public static readonly ExpressionOperator Less = new(nameof(Less), "{0} < @{1}", 1);
    public static readonly ExpressionOperator LessOrEqual = new(nameof(LessOrEqual), "{0} <= @{1}", 1);
    public static readonly ExpressionOperator Contains = new(nameof(Contains), "{0}.ToLower().Contains(@{1}.ToLower())", 1);
    public static readonly ExpressionOperator Between = new(nameof(Between), "{0} >= @{1} AND {0} <= @{2}", 2);

    internal string FormatExpression(string property, object value) => string.Format(Expression, property, value);
    internal string FormatBetweenExpression(string property, object min, object max) => string.Format(Expression, property, min, max);

    internal static ExpressionOperator[] GetBooleanOperators()
    {
        return new[]
        {
            Is,
            Not,
        };
    }

    internal static ExpressionOperator[] GetStringOperators()
    {
        return new[]
        {
            Equal,
            NotEqual,
            Contains,
        };
    }

    internal static ExpressionOperator[] GetEnumOperators()
    {
        return new[]
        {
            Equal,
            NotEqual,
            //Contains,
        };
    }

    internal static ExpressionOperator[] GetNumericOperators()
    {
        return new[]
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual,
            Between,
        };
    }

    internal static ExpressionOperator[] GetDateTimeOperators()
    {
        return new[]
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual,
            Between,
        };
    }
}