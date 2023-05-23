namespace Tridenton.Core.Attributes;

public sealed class TridentonPropertyAttribute : ValidationAttribute
{
    public bool Required { get; set; }

    private bool _isMinSet;
    private bool _isMaxSet;
    private bool _isRegularExpressionSet;

    private long _min;
    public long Min
    {
        get => _isMinSet ? _min : long.MinValue;
        set
        {
            _isMinSet = true;
            _min = value;
        }
    }

    private long _max;
    public long Max
    {
        get => _isMaxSet ? _max : long.MaxValue;
        set
        {
            _isMaxSet = true;
            _max = value;
        }
    }

    private string _regularExpression = string.Empty;
    public string RegularExpression
    {
        get => _isRegularExpressionSet ? _regularExpression : string.Empty;
        set
        {
            if (!value.IsEmpty())
            {
                _isRegularExpressionSet = true;
                _regularExpression = value;
            }
        }
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var validationResult = new ValidationResult(ErrorMessage);

        if (Required && value is null) return validationResult;

        if (Required)
        {
            switch (value)
            {
                case string valueStr when valueStr.IsEmpty():
                case Guid guid when guid == Guid.Empty:
                case DoubleGuid doubleGuid when doubleGuid == DoubleGuid.Empty:
                case Enumeration enumeration when Enumeration.GetValue(enumeration.GetType(), enumeration.Value) is null:
                case IEnumerable enumerable when !enumerable.GetEnumerator().MoveNext():
                case IList list when list.Count == 0:
                case ICollection collection when collection.Count == 0:
                case IDictionary dictionary when dictionary.Count == 0:
                    return validationResult;

                default:
                    break;
            }
        }

        if (_isMinSet)
        {
            if (value is null) return validationResult;

            switch (value)
            {
                case string valueStr when valueStr.Length < Min:
                    return validationResult;

                case int valueInt when valueInt < Min:
                case long valueLong when valueLong < Min:
                case double valueDouble when valueDouble < Min:
                case decimal valueDecimal when valueDecimal < Min:
                case IEnumerable enumerable when enumerable.Count() < Min:
                case IList list when list.Count < Min:
                case ICollection collection when collection.Count < Min:
                case IDictionary dictionary when dictionary.Count < Min:
                    return validationResult;

                default:
                    break;
            }
        }

        if (_isMaxSet)
        {
            if (value is null) return validationResult;

            switch (value)
            {
                case string valueStr when valueStr.Length > Max:
                    return validationResult;

                case int valueInt when valueInt > Max:
                case long valueLong when valueLong > Max:
                case double valueDouble when valueDouble > Max:
                case decimal valueDecimal when valueDecimal > Max:
                case IEnumerable enumerable when enumerable.Count() > Max:
                case IList list when list.Count > Max:
                case ICollection collection when collection.Count > Max:
                case IDictionary dictionary when dictionary.Count > Max:
                    return validationResult;

                default:
                    break;
            }
        }

        if (_isRegularExpressionSet && value is string str && !str.MatchesRegex(RegularExpression)) return validationResult;

        return ValidationResult.Success;
    }

    public override bool IsValid(object? value) => IsValid(value, out _);

    public bool IsValid(object? value, out ValidationResult? validationResult)
    {
        validationResult = IsValid(value, new ValidationContext(value ?? new()));

        return validationResult == ValidationResult.Success;
    }
}