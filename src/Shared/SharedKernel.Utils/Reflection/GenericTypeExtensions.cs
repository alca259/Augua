namespace System.Reflection;

/// <summary>
/// Extensiones de tipos genéricos
/// </summary>
public static class GenericTypeExtensions
{
	public static string GetGenericTypeName(this Type type)
	{
		string typeName = string.Empty;

		if (type.IsGenericType)
		{
			var args = type.GetGenericArguments();
			var genericAll = new List<string>();
			var genericClass = args.Where(w => w.IsClass).Select(t => t.GetClassName(string.Empty)).ToArray();
			var genericOthers = args.Where(w => !w.IsClass).Select(t => t.Name).ToArray();

			genericAll.AddRange(genericClass);
			genericAll.AddRange(genericOthers);

			var genericTypes = string.Join(",", genericAll);
			typeName = type.GetClassName(string.Empty);
			typeName = $"{typeName.Remove(typeName.IndexOf('`'))}<{genericTypes}>";
		}
		else
		{
			typeName = type.GetClassName(string.Empty);
		}

		return typeName;
	}

	public static string GetGenericTypeName(this object @object)
	{
		return @object.GetType().GetGenericTypeName();
	}

	private static string GetClassName(this Type type, string value)
	{
		value = string.IsNullOrEmpty(value) ? type.Name : $"{type.Name}.{value}";

		if (!type.IsNested || type.DeclaringType == null)
		{
			return value;
		}

		return type.DeclaringType.GetClassName(value);
	}

	public static List<KeyValuePair<string, T>> GetAllPublicConstantKeyValues<T>(this Type type)
	{
		return type
			.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
			.Select(x => new KeyValuePair<string, T>(x.Name, (T)x.GetRawConstantValue()))
			.ToList();
	}

	public static List<KeyValuePair<int, string>> GetAllPublicConstantKeyValues(this Type type)
	{
		return type
			.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			.Where(fi => fi.IsLiteral && !fi.IsInitOnly)
			.Select(x => new KeyValuePair<int, string>((int)x.GetRawConstantValue(), x.Name))
			.ToList();
	}
}
