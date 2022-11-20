using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace System.Data
{
	public static class DataTableExtensions
	{
		/// <summary>
		/// Aplica el método Select a un DataTable pero devuelve un DataTable con la misma estructura que el original
		///    en lugar del DataRow[] que devuelve el método normal.
		/// </summary>
		/// <param name="input">Tabla de entrada</param>
		/// <param name="filter">Filtro Select a aplicar</param>
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

		public static DataTable AddColumn<T>(this DataTable dt, string name, string caption = "", bool allowDBNull = true)
		{
			var col = dt.Columns.Add(name, typeof(T));
			col.AllowDBNull = allowDBNull;
			col.Caption = caption;

			return dt;
		}

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

			return default;
		}

		/// <summary>
		/// Obtiene un objeto tipado de una colección de datos (de forma segura, sin lanzar excepción si falla) realizando comparación con el valor Nulo de BBDD
		/// Útil para objetos leídos de bbdd, de los que sabemos cual es su tipo esperado.
		/// Si no consigue la transformación del tipo o es un DBNull, devuelve el valor por defecto del tipo indicado
		/// </summary>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="source"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static TTarget ConvertFieldToType<TTarget>(this DataRow source, string fieldName)
		{
			if (source.Table.Columns.Contains(fieldName) && source[fieldName] != null)
			{
				return source[fieldName].ConvertFromDB<TTarget>();
			}

			return default;
		}

		/// <summary>
		/// Convierte una lista de objetos a DataTable. Si se indica "_readonly", TODAS las celdas se marcan como readonly
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="_readonly"></param>
		/// <returns></returns>
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
		/// Convierte un IEnumerable en un DataTable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static DataTable ToDataTable<T>(this IEnumerable<T> data) where T : class
		{
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
		/// Copia los datos en finalTable (mapeando columnas por nombre). Las columnas que no existen en final, no se copian
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="finalTable">tabla destino (con la estructura de columnas que queramos tener)</param>
		/// <param name="preserveChangesOnfinalTable">indica si se quieren preservar los cambios en finalTable</param>
		public static void CloneAllSameColumns(this DataTable dt, ref DataTable finalTable, bool preserveChangesOnfinalTable = false)
		{
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
		/// Pone el nombre de la tabla y devuelve la tabla.
		/// Si DataTable es null, devuelve null.
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static DataTable SetTableName(this DataTable dt, string name)
		{
			if (dt == null) return null;
			dt.TableName = name;
			return dt;
		}

		/// <summary>
		/// Pone el nombre de la columna clave (sólo 1)
		/// Si DataTable es null, devuelve null.
		/// </summary>
		/// <returns></returns>
		public static DataTable SetPrimaryKey(this DataTable dt, string colname)
		{
			if (dt == null) return dt;
			if (string.IsNullOrEmpty(colname)) return dt;
			dt.PrimaryKey = new[] { dt.Columns[colname] };
			return dt;
		}

		/// <summary>
		/// Devuelve el nombre de la columna clave (sólo 1)
		/// Si DataTable es null, devuelve null.
		/// </summary>
		/// <returns></returns>
		public static string GetPrimaryKey(this DataTable dt)
		{
			string ret = null;
			if (dt != null && dt.PrimaryKey != null && dt.PrimaryKey.Length > 0)
			{
				ret = dt.PrimaryKey[0].ColumnName;
			}
			return ret;
		}

		/// <summary>
		/// Devuelve un string con el contenido de la tabla formateado para mostrar por consola/log
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static string ToStringConsole(this DataTable dt)
		{
			var ret = new StringBuilder();

			if (dt != null)
			{
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
			}

			return ret.ToString();
		}

		/// <summary>
		/// Devuelve un string con el contenido de la fila formateado para mostrar por consola/log
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static string ToStringConsole(this DataRow row)
		{
			var ret = new StringBuilder();

			if (row != null)
			{
				foreach (var col in row.ItemArray)
				{
					ret.Append($"| {col} |");
				}
			}

			return ret.ToString();
		}
	}
}
