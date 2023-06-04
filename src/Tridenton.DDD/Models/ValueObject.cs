﻿namespace Tridenton.DDD;

public abstract record ValueObject<TValue>(TValue Value);

#region v2

//public abstract record ValueObject : IEquatable<ValueObject>
//{
//    public abstract IEnumerable<object> GetAtomicValues();

//    public override int GetHashCode()
//    {
//        return GetAtomicValues().Aggregate(default(int), HashCode.Combine);
//    }

//    private bool ValuesAreEqual(ValueObject other)
//    {
//        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
//    }
//}

#endregion

#region v1

//public abstract class ValueObject : IEquatable<ValueObject>
//{
//    public abstract IEnumerable<object> GetAtomicValues();

//    public bool Equals(ValueObject? other)
//    {
//        return other is not null && ValuesAreEqual(other);
//    }

//    public override bool Equals(object? obj)
//    {
//        return obj is ValueObject other && ValuesAreEqual(other);
//    }

//    public override int GetHashCode()
//    {
//        return GetAtomicValues().Aggregate(default(int), HashCode.Combine);
//    }

//    private bool ValuesAreEqual(ValueObject other)
//    {
//        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
//    }
//}

#endregion