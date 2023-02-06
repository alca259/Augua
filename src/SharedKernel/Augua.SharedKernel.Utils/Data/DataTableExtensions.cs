using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace System.Data;

/// <summary>
/// Extensiones de DataTable
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// Aplica el método Select a un DataTable pero devuelve un DataTable con la misma estructura que el método original,
    /// en lugar del DataRow[] que devuelve el método normal.
    /// </summary>
    /// <param name="input" cref="DataTable">Tabla de entrada</param>
    /// <param name="filter" cref="string">Filtro Select a aplicar</param>
    /// <returns>Tabla conteniendo las filas que complen el criterio <paramref name="filter"/>. Devuelve null si no encuentra resultado.</returns>
    public static DataTable SelectDataTable(this DataTable input, string filter)
	{
		try
		{
			DataTable dtResult = new();

			// copy table structure
			dtResult = input.Clone();

			// filter data
			DataRow[] rows = input.Select(filter);

			// fill dtNew with selected rows
			foreach (DataRow dr in rows)
			{
				dtResult.ImportRow(dr);
			}

			// return filtered dt
			return dtResult;
		}
		catch (Exception)
		{
			return null;
		}
	}

    /// <summary>
    /// Añade una columna a una tabla.
    /// </summary>
    /// <typeparam name="T">Tipo de dato. (bool, int, string...)</typeparam>
    /// <param name="dt" cref="DataTable"></param>
    /// <param name="name" cref="string">Nombre interno de la columna.</param>
    /// <param name="caption" cref="string">Nombre a mostrar en la columna.</param>
    /// <param name="allowDBNull" cref="bool">Si se permite null.</param>
    /// <returns cref="DataTable">Si <paramref name="dt"/> es null, devuelve null.</returns>
    public static DataTable AddColumn<T>(this DataTable dt, string name, string caption = "", bool allowDBNull = true)
	{
		if (dt == null) return null;

		var col = dt.Columns.AddExtended(name, typeof(T), caption);
		col.AllowDBNull = allowDBNull;

		return dt;
	}

	/// <summary>
	/// Añade una columna a una colección de columnas.
	/// </summary>
	/// <param name="columnCollection" cref="DataColumnCollection">Colección</param>
	/// <param name="name" cref="string">Nombre interno de la columna</param>
	/// <param name="type" cref="Type">Tipo de dato. (bool, int, string...)</param>
	/// <param name="caption" cref="string">Nombre a mostrar en la columna.</param>
	/// <returns cref="DataColumn">La columna creada si la tabla no la contiene, si la contiene devuelve esa.</returns>
	public static DataColumn AddExtended(this DataColumnCollection columnCollection, string name, Type type, string caption = "")
	{
		DataColumn dataColumn = new()
		{
			ColumnName = name,
			DataType = type,
			Caption = caption
		};

		if (!columnCollection.Contains(name))
		{
			columnCollection.Add(dataColumn);
			return dataColumn;
		}

		return columnCollection[name];
	}

    /// <summary>
    /// Obtiene un objeto tipado de una colección de datos (de forma segura, sin lanzar excepción si falla) realizando comparación con el valor Nulo de BBDD
    /// </summary>
    /// <remarks>
    /// Útil para objetos leídos de bbdd, de los que sabemos cual es su tipo esperado.
    /// Si no consigue la transformación del tipo o es un DBNull, devuelve el valor por defecto del tipo indicado.
    /// </remarks>
    /// <typeparam name="TTarget">Tipo de dato a devolver.</typeparam>
    /// <param name="source" cref="DataRow">Fila de origen.</param>
    /// <param name="fieldName" cref="string">Nombre interno de la columna.</param>
    /// <returns>Devuelve el tipo especificado en el genérico <typeparamref name="TTarget"/>.</returns>
    public static TTarget ConvertFieldToType<TTarget>(this DataRow source, string fieldName)
	{
		if (source.Table.Columns.Contains(fieldName) && source[fieldName] != null)
		{
			return source[fieldName].ConvertFromDB<TTarget>();
		}

		return default;
	}

    /// <summary>
    /// Convierte una lista de objetos a DataTable.
    /// </summary>
	/// <typeparam name="T" cref="class">Tipo de dato del IEnumerable</typeparam>
	/// <param name="data" cref="IList{T}">Datos</param>
    /// <param name="_readonly" cref="bool">Si se activa, todas las celdas se marcarán como solo lectura.</param>
    /// <returns cref="DataTable">Si <paramref name="data"/> es null, devolverá null.</returns>
    public static DataTable ToDataTable<T>(this IList<T> data, bool _readonly = false)
	{
		PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
		DataTable table = new();

		foreach (PropertyDescriptor prop in properties)
			table.Columns.Add(new DataColumn(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) { ReadOnly = _readonly });

		foreach (T item in data)
		{
			DataRow row = table.NewRow();
			foreach (PropertyDescriptor prop in properties)
				row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
			table.Rows.Add(row);
		}
		return table;
	}

	/// <summary>
	/// Convierte un IEnumerable de objetos en un DataTable.
	/// </summary>
	/// <typeparam name="T" cref="class">Tipo de dato del IEnumerable</typeparam>
	/// <param name="data" cref="IEnumerable{T}">Datos</param>
	/// <returns cref="DataTable">Si <paramref name="data"/> es null, devolverá null.</returns>
	public static DataTable ToDataTable<T>(this IEnumerable<T> data) where T : class
	{
		if (data == null) return null;

		PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
		DataTable table = new();

		int countProperties = 0;

		for (int i = 0; i < props.Count; i++)
		{
			PropertyDescriptor prop = props[i];
			if (prop.PropertyType.Name.Equals(typeof(Enum).Name))
			{
				table.Columns.Add(!string.IsNullOrEmpty(prop.Description) ? prop.Description : prop.Name, typeof(string));
			}
			else if (prop.PropertyType.Name.Contains("Nullable"))
			{
				if (prop.PropertyType.GenericTypeArguments.Length > 0)
				{
					table.Columns.Add(!string.IsNullOrEmpty(prop.Description) ? prop.Description : prop.Name, prop.PropertyType.GenericTypeArguments[0]);
				}
				else
				{
					table.Columns.Add(!string.IsNullOrEmpty(prop.Description) ? prop.Description : prop.Name, typeof(string));
				}
			}
			else
			{
				table.Columns.Add(!string.IsNullOrEmpty(prop.Description) ? prop.Description : prop.Name, prop.PropertyType);
			}
		}

		countProperties += props.Count;

		object[] values = new object[countProperties];
		foreach (T item in data)
		{
			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor prop = props[i];
				if (prop.PropertyType.Name.Equals(typeof(Enum).Name))
				{
					values[i] = ((Enum)props[i].GetValue(item)).ToString();
				}
				else
				{
					values[i] = props[i].GetValue(item);
				}
			}
			table.Rows.Add(values);
		}
		return table;
	}

    /// <summary>
    /// Copia los datos en finalTable (mapeando columnas por nombre). Las columnas que no existen en la tabla de destino, no se copian.
    /// </summary>
	/// <remarks>
	/// Si <paramref name="dt"/> o <paramref name="finalTable"/> son null, no se realizará ninguna acción.
	/// </remarks>
    /// <param name="dt" cref="DataTable">Tabla de origen</param>
    /// <param name="finalTable" cref="DataTable">Tabla destino (con la estructura de columnas que queramos tener)</param>
    /// <param name="preserveChangesOnfinalTable" cref="bool">Indica si se quieren preservar los cambios en <paramref name="finalTable"/></param>
    public static void CloneAllSameColumns(this DataTable dt, ref DataTable finalTable, bool preserveChangesOnfinalTable = false)
	{
		if (dt == null || finalTable == null) return;

		// Eliminamos las columnas que no aparecen en finalTable
		for (int colIndex = dt.Columns.Count - 1; colIndex >= 0; colIndex--)
		{
			DataColumn col = dt.Columns[colIndex];

			if (!finalTable.Columns.Contains(col.ColumnName))
			{
				dt.Columns.Remove(col);
			}
		}

		dt.AcceptChanges();
		finalTable.Merge(dt, preserveChangesOnfinalTable);
	}

    /// <summary>
    /// Establece el nombre de la tabla.
    /// </summary>
    /// <param name="dt" cref="DataTable">Tabla</param>
    /// <param name="name" cref="string">Nombre de la tabla</param>
    /// <returns cref="DataTable">Si <paramref name="dt"/> es null, devuelve null.</returns>
    public static DataTable SetTableName(this DataTable dt, string name)
	{
		if (dt == null) return null;
		dt.TableName = name;
		return dt;
	}

    /// <summary>
    /// Establece la clave primaria de la tabla (sólo 1)
    /// </summary>
    /// <param name="dt" cref="DataTable">Tabla</param>
    /// <param name="colname" cref="string">Nombre interno de la columna</param>
    /// <returns cref="DataTable">Si <paramref name="dt"/> es null, devuelve null.</returns>
    public static DataTable SetPrimaryKey(this DataTable dt, string colname)
	{
		if (dt == null) return null;
		if (string.IsNullOrEmpty(colname)) return dt;
		dt.PrimaryKey = new[] { dt.Columns[colname] };
		return dt;
	}

    /// <summary>
    /// Devuelve el nombre de la columna clave (sólo 1)
    /// </summary>
    /// <param name="dt" cref="DataTable">Tabla</param>
    /// <returns cref="DataTable">Si <paramref name="dt"/> es null, devuelve null.</returns>
    public static string GetPrimaryKey(this DataTable dt)
	{
        if (dt == null) return null;

        string ret = null;
		if (dt.PrimaryKey != null && dt.PrimaryKey.Length > 0)
		{
			ret = dt.PrimaryKey[0].ColumnName;
		}

		return ret;
	}

    /// <summary>
    /// Devuelve un string con el contenido de la tabla formateado para mostrar por consola/log
    /// </summary>
    /// <param name="dt" cref="DataTable">Tabla</param>
    /// <returns cref="string">Si <paramref name="dt"/> es null, devuelve string.Empty.</returns>
    public static string ToStringConsole(this DataTable dt)
    {
        if (dt == null)
        {
            return string.Empty;
        }

        var ret = new StringBuilder();

        // Cabecera
        foreach (var col in dt.Columns)
        {
            ret.Append($"| {col} |");
        }
        ret.AppendLine();

        // Datos
        foreach (DataRow row in dt.Rows)
        {
            ret.Append(row.ToStringConsole());
            ret.AppendLine();
        }

        return ret.ToString();
    }

    /// <summary>
    /// Devuelve un string con el contenido de la fila formateado para mostrar por consola/log
    /// </summary>
    /// <param name="row" cref="DataRow">Fila</param>
    /// <returns cref="string">Si <paramref name="row"/> es null, devuelve string.Empty.</returns>
    public static string ToStringConsole(this DataRow row)
    {
        if (row == null)
        {
            return string.Empty;
        }

        var ret = new StringBuilder();
        foreach (var col in row.ItemArray)
        {
            ret.Append($"| {col} |");
        }
        return ret.ToString();
    }
}
