namespace System.Reflection;

/// <summary>
/// Extensiones genéricas de objetos.
/// </summary>
public static class ObjectExtensions
{
	/// <summary>
	/// Transforma un objeto en otro tipo (de forma segura, sin lanzar excepción si falla) realizando comparación con el valor Nulo de BBDD
	/// Útil para objetos leídos de bbdd, de los que sabemos cual es su tipo esperado.
	/// Si no consigue la transformación del tipo o es un DBNull, devuelve el valor por defecto del tipo indicado
	/// </summary>
	/// <typeparam name="TTarget">Tipo al que debemos convertir el objeto</typeparam>
	/// <param name="source">Objeto que deseamos tipar </param>
	/// <returns>Objeto convertido al tipo indicado o bien el valor por defecto si esto no ha sido posible</returns>
	public static TTarget ConvertFromDB<TTarget>(this object source)
	{
		if (source == null || source == DBNull.Value)
		{
			return default;
		}

		if (source is TTarget sourceTarget) return sourceTarget;

		// como el objeto no es del tipo que esperamos, intentamos convertirlo...
		try
		{
			object convertedSource = Convert.ChangeType(source, typeof(TTarget), Globalization.CultureInfo.InvariantCulture);

			if (convertedSource is TTarget target) return target;

			return default;
		}
		catch (InvalidCastException) { }
		catch (FormatException) { }
		catch (OverflowException) { }
		catch (ArgumentException) { }

		return default;
	}

	/// <summary>
	/// Returns a _private_ Property Value from a given Object. Uses Reflection.
	/// Throws a ArgumentOutOfRangeException if the Property is not found.
	/// </summary>
	/// <typeparam name="T">Type of the Property</typeparam>
	/// <param name="obj">Object from where the Property Value is returned</param>
	/// <param name="propName">Propertyname as string.</param>
	/// <returns>PropertyValue</returns>
	public static T GetPrivatePropertyValue<T>(this object obj, string propName)
	{
		if (obj == null) return default;
		PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		if (pi == null) return default;
		return (T)pi.GetValue(obj, null);
	}

	/// <summary>
	/// Sets a _private_ Property Value from a given Object. Uses Reflection.
	/// Throws a ArgumentOutOfRangeException if the Property is not found.
	/// </summary>
	/// <typeparam name="T">Type of the Property</typeparam>
	/// <param name="obj">Object from where the Property Value is set</param>
	/// <param name="propName">Propertyname as string.</param>
	/// <param name="val">Value to set.</param>
	/// <returns>PropertyValue</returns>
	public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
	{
		Type t = obj.GetType();
		var prop = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		if (prop == null)
			return;

		prop.DeclaringType.GetProperty(propName);
		prop.GetSetMethod(true).Invoke(obj, new object[] { val });
	}

	/// <summary>
	/// Returns a private Property Value from a given Object. Uses Reflection.
	/// Throws a ArgumentOutOfRangeException if the Property is not found.
	/// </summary>
	/// <typeparam name="T">Type of the Property</typeparam>
	/// <param name="obj">Object from where the Property Value is returned</param>
	/// <param name="fieldName">FieldName as string.</param>
	/// <returns>PropertyValue</returns>
	public static T GetPrivateFieldValue<T>(this object obj, string fieldName)
	{
		if (obj == null) throw new ArgumentNullException("obj");
		Type t = obj.GetType();
		FieldInfo fi = null;
		while (fi == null && t != null)
		{
			fi = t.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			t = t.BaseType;
		}
		if (fi == null) return default;
		return (T)fi.GetValue(obj);
	}

	/// <summary>
	/// Set a private Property Value on a given Object. Uses Reflection.
	/// </summary>
	/// <typeparam name="T">Type of the Property</typeparam>
	/// <param name="obj">Object from where the Property Value is returned</param>
	/// <param name="fieldName">FieldName as string.</param>
	/// <param name="val">the value to set</param>
	/// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
	public static void SetPrivateFieldValue<T>(this object obj, string fieldName, T val)
	{
		if (obj == null) throw new ArgumentNullException("obj");
		Type t = obj.GetType();
		FieldInfo fi = null;
		while (fi == null && t != null)
		{
			fi = t.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			t = t.BaseType;
		}
		if (fi == null) return;
		fi.SetValue(obj, val);
	}
}
