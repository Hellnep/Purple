using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Purple.Common.Database.Mapping;

/// <summary>
/// A custom class for mapping data objects
/// to their representation
/// </summary>
public static class Mapping
{
    private static readonly Dictionary<(Type, Type), (PropertyInfo[], PropertyInfo[])> _cache = new();

    /// <summary>
    /// Perform type matching and return 
    /// the required data type
    /// </summary>
    /// <typeparam name="T1">The type of data to be returned</typeparam>
    /// <typeparam name="T2">The type of data to match</typeparam>
    /// <param name="inputType">Input data</param>
    /// <returns>
    /// Return the received data
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Called if empty data is received
    /// </exception>
    public static T1 Get<T1, T2>(T2 inputType) where T1 : new()
    {
        if (inputType is null)
            throw new ArgumentNullException(nameof(T2), "Types cannot be nullable");

        T1 entity = new();

        var (entityProperties, dtoProperties) = GetProperties(typeof(T1), typeof(T2));

        foreach (PropertyInfo dtoProperty in dtoProperties)
        {
            PropertyInfo? entityProperty = Array.Find(entityProperties, p =>
                p.Name.Equals(dtoProperty.Name, StringComparison.OrdinalIgnoreCase) &&
                p.CanWrite);

            if (entityProperty != null &&
                entityProperty.PropertyType.IsAssignableFrom(dtoProperty.PropertyType))
            {
                object? value = dtoProperty.GetValue(inputType);
                entityProperty.SetValue(entity, value);
            }
        }

        return entity;
    }

    /// <summary>
    /// Retrieving type properties and then storing
    /// them in the cache.
    /// </summary>
    /// <param name="outputType">
    /// The type of data those property you want to get
    /// </param>
    /// <param name="inputType">
    /// The type of data where the data will be taken from
    /// </param>
    /// <returns>Returns properties corresponding to the types</returns>
    private static (PropertyInfo[], PropertyInfo[]) GetProperties(Type outputType, Type inputType)
    {
        var key = (outputType, inputType);

        if (!_cache.TryGetValue(key, out var properties))
        {
            properties = (outputType.GetProperties(), inputType.GetProperties());
            _cache[key] = properties;
        }

        return properties;
    }
}
