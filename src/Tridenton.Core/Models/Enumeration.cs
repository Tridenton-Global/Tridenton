namespace Tridenton.Core.Models;

public abstract class Enumeration
{
    public readonly string Value;

    protected Enumeration(string value)
    {
        Value = value;
    }

    public static Enumeration[] GetValues(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Select(f => f.GetValue(null)).Cast<Enumeration>().ToArray();
    }

    public static TEnumeration[] GetValues<TEnumeration>() where TEnumeration : Enumeration
    {
        return GetValues(typeof(TEnumeration)).Cast<TEnumeration>().ToArray();
    }

    public static Enumeration? GetValue(Type type, string value)
    {
        return GetValues(type).FirstOrDefault(v => v.Equals(value));
    }

    public static TEnumeration? GetValue<TEnumeration>(string value) where TEnumeration : Enumeration
    {
        return GetValue(typeof(TEnumeration), value) as TEnumeration;
    }

    public override string ToString() => Value;

    public override int GetHashCode() => ToString().GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is Enumeration enumeration) return Value.Equals(enumeration.Value);
        if (obj is string str) return Value.Equals(str);

        return base.Equals(obj);
    }

    public static implicit operator string(Enumeration value) => value.Value;

    public static bool operator ==(Enumeration? a, Enumeration? b)
    {
        if (ReferenceEquals(a, b)) return true;

        return a is not null && a.Equals(b);
    }

    public static bool operator !=(Enumeration? a, Enumeration? b) => !(a == b);

    public static bool operator ==(Enumeration a, string b)
    {
        if (a is null && b == null) return true;

        return a is not null && a.Equals(b);
    }

    public static bool operator !=(Enumeration a, string b) => !(a.Value == b);

    public static bool operator ==(string a, Enumeration b) => b.Value == a;

    public static bool operator !=(string a, Enumeration b) => !(a == b.Value);
}