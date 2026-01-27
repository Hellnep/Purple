using System.Collections;
using System.Reflection;

namespace PurpleBackendService.Infrastucture.Utility
{
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
            T1 entity = new();

            // Метод, который получает свойства и тут же их сохраняет в кеш,
            // для последующей работы с ними.
            var (entityProperties, dtoProperties) = GetProperties(typeof(T1), typeof(T2));

            foreach (var dtoProperty in dtoProperties)
            {
                var entityProperty = Array.Find(entityProperties, p =>
                    p.Name.Equals(dtoProperty.Name, StringComparison.OrdinalIgnoreCase) &&
                    p.CanWrite);

                if (entityProperty is not null)
                {
                    var dtoValue = dtoProperty.GetValue(inputType);
                    var entityType = entityProperty.PropertyType;

                    if (dtoValue is null)
                    {
                        continue;
                    }

                    if (IsCollectionType(dtoProperty.PropertyType) &&
                        IsCollectionType(entityProperty.PropertyType))
                    {
                        // If these are collections, call the collection generatiuon method.
                        // Если это коллекции, вызвается метод конструкции
                        // коллекций и передачи данных для них
                        GenerateCollection(entityProperty, dtoProperty, dtoValue, entity);
                    }
                    // Checking for collection types
                    // Проверка на примитивные типы данных для их конвертации
                    else if (!entityType.IsPrimitive &&
                        entityType != typeof(string) &&
                        entityType != typeof(DateTime) &&
                        entityType != typeof(DateTime?) &&
                        entityType != typeof(DateOnly) &&
                        entityType != typeof(DateOnly?))
                    {
                        // Если это не коллекции, но и не примитивные данные,
                        // вызывается метод конструкции этих классов с последующей передачей данных.
                        GenerateClass(entityProperty, dtoProperty, dtoValue, entity);
                    }
                    else
                    {
                        // If they are primitive, call a simple data mapping
                        // Если примитивные данные, просто передаём данные
                        if (entityProperty.PropertyType.IsAssignableFrom(dtoProperty.PropertyType))
                            entityProperty.SetValue(entity, dtoValue);
                    }
                }
            }

            return entity;
        }


        /// <summary>
        /// Matching data from collections of incoming types
        /// </summary>
        /// <typeparam name="T">Primary class</typeparam>
        /// <param name="entityProperty">Own object property</param>
        /// <param name="dtoProperty">Child object property</param>
        /// <param name="value">Input values</param>
        /// <param name="entity">The mapping class for</param>
        private static void GenerateCollection<T> (
            PropertyInfo entityProperty,
            PropertyInfo dtoProperty,
            object value,
            T entity
        )
        {
            var sourceCollection = (IEnumerable?)value;

            if (sourceCollection is not null)
            {
                var targetCollection = CreateCollectionInstance(entityProperty.PropertyType);

                // Getting information about collection types
                // Получаем информацию о типах коллекий
                var sourceElementType = dtoProperty.PropertyType
                    .GetGenericArguments()[0];

                var targetElementType = entityProperty.PropertyType
                    .GetGenericArguments()[0];

                foreach (var item in sourceCollection)
                {
                    var method = GenerateMethod(targetElementType, sourceElementType);
                    var mappedItem = method.Invoke(null, [item]);

                    if (targetCollection is IList localList)
                    {
                        localList.Add(mappedItem);
                    }
                }

                if (entityProperty.PropertyType.IsArray &&
                    targetCollection is IList list)
                {
                    var array = Array.CreateInstance(entityProperty.PropertyType
                        .GetElementType()!,
                        list.Count
                    );

                    list.CopyTo(array, 0);
                    entityProperty.SetValue(entity, array);
                }
                else
                {
                    entityProperty.SetValue(entity, targetCollection);
                }
            }
        }

        /// <summary>
        /// Creating a new collection of input data type
        /// </summary>
        /// <param name="collectionType">Incoming of data type</param>
        /// <returns>New collection</returns>
        private static object CreateCollectionInstance(Type collectionType)
        {
            if (collectionType.IsArray)
            {
                var elementTypeLocal = collectionType
                    .GetElementType()!;

                var listTypeLocal = typeof(List<>)
                    .MakeGenericType(elementTypeLocal);

                return Activator.CreateInstance(listTypeLocal)!;
            }
            else if (collectionType.IsGenericType)
            {
                if (collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    collectionType.GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    return Activator.CreateInstance(collectionType)!;
                }
            }

            var elementType = collectionType
                .GetGenericArguments()[0];

            var listType = typeof(List<>)
                .MakeGenericType(elementType);

            return Activator.CreateInstance(listType)!;
        }


        /// <summary>
        /// Checking the data type on collections
        /// </summary>
        /// <param name="type">Type of data</param>
        /// <returns></returns>
        private static bool IsCollectionType(Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Generation method of mapping for an undefined class
        /// </summary>
        /// <typeparam name="T">Primary class</typeparam>
        /// <param name="entityProperty">Own object property</param>
        /// <param name="dtoProperty">Child object property</param>
        /// <param name="value">Input values</param>
        /// <param name="entity">The mapping class for</param>
        private static void GenerateClass<T> (
            PropertyInfo entityProperty,
            PropertyInfo dtoProperty,
            object value,
            T entity
        )
        {
            var method = GenerateMethod(entityProperty.PropertyType, dtoProperty.PropertyType);
            var nestedValue = method.Invoke(null, new object?[] { value });

            entityProperty.SetValue(entity, nestedValue);
        }

        /// <summary>
        /// Generating a new method recursively
        /// </summary>
        /// <param name="targetType">Target data type</param>
        /// <param name="sourceType">Source data type</param>
        /// <returns></returns>
        private static MethodInfo GenerateMethod(Type targetType, Type sourceType) => typeof(Mapping)
                .GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static)!
                .MakeGenericMethod(targetType, sourceType);

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
        private static (PropertyInfo[], PropertyInfo[]) GetProperties(
            Type outputType,
            Type inputType
        )
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
}
