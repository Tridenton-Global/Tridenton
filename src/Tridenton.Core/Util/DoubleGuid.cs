namespace Tridenton.Core.Util;

public struct DoubleGuid : IEquatable<DoubleGuid>, IParsable<DoubleGuid>
{
    private static int _guidLength => Guid.Empty.ToString().Length;

    private Guid _first;
    private Guid _second;

    public DoubleGuid()
    {
        _first = _second = Guid.Empty;
    }

    public override string ToString() => $"{_first.ToString().ToLower()}-{_second.ToString().ToLower()}";

    public override int GetHashCode() => ToString().GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is DoubleGuid guid) return Equals(guid);

        if (obj is string str) return TryParse(str, out var another) && Equals(another);

        return base.Equals(obj);
    }

    public bool Equals(DoubleGuid other) => other.ToString().Equals(ToString(), StringComparison.OrdinalIgnoreCase);

    public static DoubleGuid Empty => new();

    public static DoubleGuid NewGuid() => new()
    {
        _first = Guid.NewGuid(),
        _second = Guid.NewGuid(),
    };

    public static DoubleGuid Parse(string? value) => Parse(value, null);

    public static DoubleGuid Parse(string? value, IFormatProvider? provider)
    {
        if (value.IsEmpty()) throw new ArgumentException("String value is empty");

        if (value!.Length != _guidLength * 2 + 1) throw new MalformedDoubleGuidException();
        if (value.Count(c => c.Equals('-')) != 9) throw new MalformedDoubleGuidException();
        if (value[_guidLength] != '-') throw new MalformedDoubleGuidException();

        char[] firstGuidStr = value.Take(_guidLength).ToArray();
        if (!Guid.TryParse(firstGuidStr, out Guid firstGuid)) throw new MalformedDoubleGuidException();

        char[] secondGuidStr = value.Skip(_guidLength + 1).Take(_guidLength).ToArray();
        if (!Guid.TryParse(secondGuidStr, out Guid secondGuid)) throw new MalformedDoubleGuidException();

        return new()
        {
            _first = firstGuid,
            _second = secondGuid
        };
    }

    public static bool TryParse(string? value, out DoubleGuid result) => TryParse(value, null, out result);

    public static bool TryParse(string? value, IFormatProvider? provider, out DoubleGuid result)
    {
        try
        {
            result = Parse(value!, provider);
            return true;
        }
        catch (Exception)
        {
            result = Empty;
            return false;
        }
    }

    public static bool operator ==(DoubleGuid left, DoubleGuid right) => left.Equals(right);

    public static bool operator !=(DoubleGuid left, DoubleGuid right) => !(left == right);
}