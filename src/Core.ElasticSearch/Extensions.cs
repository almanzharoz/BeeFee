using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.ElasticSearch
{
	internal static class Extensions
	{
		public static bool HasBaseType<T>(this Type type) => typeof(T).IsAssignableFrom(type);
		public static bool HasBaseType(this Type type, Type basetype) => basetype.IsAssignableFrom(type);

		public static string[] GetFieldsNames(this Type type, string lastName = null)
		{
			var fields = new List<string>();
			foreach (var property in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name != "Id" && x.Name != "Version" && x.Name != "Parent"))
			{
				if (property.PropertyType.IsValueType && !property.PropertyType.IsPrimitive && !property.PropertyType.IsAutoLayout /*|| property.PropertyType.HasBaseType<IEntity>() && property.GetCustomAttribute<KeywordAttribute>() == null*/)
					fields.AddRange(GetFieldsNames(property.PropertyType, String.Join(".", new[] { lastName, property.Name.ToLower() }.Where(x => x != null))));
				else
					fields.Add(String.Join(".", new[] { lastName, property.Name.ToLower() }.Where(x => x != null)));
			}
			return fields.ToArray();
		}

		public static PropertyInfo[] GetProperties(this Type type) =>
			type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(x => x.Name != "Id" && x.Name != "Version" && x.Name != "Parent")
				.ToArray();
	}
}