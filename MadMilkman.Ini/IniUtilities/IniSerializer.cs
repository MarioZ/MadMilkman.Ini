using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace MadMilkman.Ini
{
    internal static class IniSerializer
    {
        private static readonly Predicate<Type> PropertyTypeVerifier = IniKey.IsSupportedValueType;
        
        public static void Serialize<T>(T source, IniSection section) where T : class, new()
        {
            foreach (var propertyPair in GetPropertyPairs(typeof(T)))
            {
                var key = section.Keys.Add(propertyPair.Key);
                if (key.ParentCollectionCore != null)
                    SetKeyValue(propertyPair.Value, source, key);
            }
        }

        public static T Deserialize<T>(IniSection section) where T : class, new()
        {
            T destination = new T();

            foreach (var propertyPair in GetPropertyPairs(typeof(T)))
            {
                var key = section.Keys[propertyPair.Key];
                if (key != null)
                    SetPropertyValue(propertyPair.Value, destination, key);
            }

            return destination;
        }

        private static IEnumerable<KeyValuePair<string, PropertyInfo>> GetPropertyPairs(Type type)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!IniSerializer.PropertyTypeVerifier(property.PropertyType))
                    continue;

                string propertyName = null;
                var attributes = property.GetCustomAttributes(typeof(IniSerializationAttribute), false);

                if (attributes.Length > 0)
                {
                    var attribute = (IniSerializationAttribute)attributes[0];
                    if (attribute.Ignore)
                        continue;

                    propertyName = attribute.Alias;
                }

                yield return new KeyValuePair<string, PropertyInfo>((propertyName) ?? property.Name, property);
            }
        }

        private static void SetKeyValue(PropertyInfo property, object source, IniKey key)
        {
            object propertyValue = property.GetValue(source, null);

            if (propertyValue == null)
                return;

            if (property.PropertyType.IsArray || property.PropertyType.GetInterface(typeof(IList).Name) != null)
            {
                var values = new List<string>();

                /* MZ(2016-01-02): Fixed issue with null items in array and list. */
                foreach (var item in (IEnumerable)propertyValue)
                    values.Add(item != null ? item.ToString() : null);

                key.Values = values.ToArray();
            }
            else
                key.Value = propertyValue.ToString();
        }

        private static void SetPropertyValue(PropertyInfo property, object destination, IniKey key)
        {
            var propertyType = property.PropertyType;

            if (propertyType.IsArray)
            {
                /* MZ(2016-01-02): Fixed issue with null array and list. */
                if (!key.IsValueArray)
                    return;

                var values = key.Values;
                var itemType = propertyType.GetElementType();
                var array = Array.CreateInstance(itemType, values.Length);

                for (int i = 0; i < values.Length; i++)
                    array.SetValue(
                        TypeDescriptor.GetConverter(itemType).ConvertFromInvariantString(values[i]),
                        i);

                property.SetValue(destination, array, null);
            }

            else if (propertyType.GetInterface(typeof(IList).Name) != null)
            {
                /* MZ(2016-01-02): Fixed issue with null array and list. */
                if (!key.IsValueArray)
                    return;

                var itemType = propertyType.GetGenericArguments()[0];
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

                var values = key.Values;
                if (!(values.Length == 1 && string.IsNullOrEmpty(values[0])))
                    foreach (var value in values)
                        list.Add(
                            TypeDescriptor.GetConverter(itemType).ConvertFromInvariantString(value));

                property.SetValue(destination, list, null);
            }

            else
                property.SetValue(
                    destination,
                    TypeDescriptor.GetConverter(propertyType).ConvertFromInvariantString(key.Value),
                    null);
        }
    }
}
