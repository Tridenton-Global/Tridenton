﻿namespace Tridenton.Core.Util;

public readonly struct ReflectionManager
{
    /// <summary>
    ///     Retrieves value from <paramref name="item"/>`s <paramref name="property"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="item">Item</param>
    /// <param name="property">Property</param>
    /// <returns>
    ///     Value from <paramref name="item"/>`s <paramref name="property"/>
    /// </returns>
    public static object? GetItemValue<TItem>(TItem? item, PropertyInfo property) where TItem : class => GetItemValue(item, property.Name);

    /// <summary>
    ///     Retrieves value from <paramref name="item"/>`s property, defined by <paramref name="propertyName"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="item">Item</param>
    /// <param name="propertyName">Property name</param>
    /// <returns>
    ///     Value from <paramref name="item"/>`s property, defined by <paramref name="propertyName"/>
    /// </returns>
    public static object? GetItemValue<TItem>(TItem? item, string propertyName) where TItem : class
    {
        try
        {
            return item?.GetType()?.GetProperty(propertyName)?.GetValue(item, null);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    ///     Sets <paramref name="value"/> to <paramref name="item"/>`s property, defined by <paramref name="propertyName"/>.
    ///     If property does not have a setter method, <paramref name="value"/> will be set into corresponding backing private field
    ///     (this approach works only with <b>autogenerated private fields</b>).
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="item">Item</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public static void SetItemValue<TItem>(TItem? item, string propertyName, object? value) where TItem : class
    {
        var property = typeof(TItem).GetProperty(propertyName);

        if (property is null) return;

        SetItemValue(item, property, value);
    }

    /// <summary>
    ///     Sets <paramref name="value"/> to <paramref name="item"/>`s <paramref name="property"/>.
    ///     If property does not have a setter method, <paramref name="value"/> will be set into corresponding backing private field
    ///     (this approach works only with <b>autogenerated private fields</b>).
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="item">Item</param>
    /// <param name="property">Property</param>
    /// <param name="value">Value</param>
    public static void SetItemValue<TItem>(TItem? item, PropertyInfo property, object? value) where TItem : class
    {
        if (!property.CanWrite)
        {
            var backingFieldName = $"<{property.Name}>k__BackingField";

            var field = typeof(TItem).GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field is null) return;

            field.SetValue(item, value);

            return;
        }

        property.SetValue(item, value);
    }
}