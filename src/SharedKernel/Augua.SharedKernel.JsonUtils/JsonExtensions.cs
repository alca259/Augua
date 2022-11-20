using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace System.Reflection
{
	public static class JsonExtensions
	{
		static readonly JsonSerializerSettings _jsonSettings = new()
		{
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			ContractResolver = new AppContractResolver()
		};

		#region Default ToJson
		/// <summary>
		/// Devuelve una cadena con el objeto serializado en formato JSON.
		/// </summary>
		/// <param name="data">El objeto para serializar en JSON</param>
		/// <param name="enumAsString">Al serializar un dato de tipo enum, usar el valor de cadena en lugar del valor numérico</param>
		/// <param name="customJsonSettings">Opciones personalizadas</param>
		/// <returns></returns>
		public static string ToJson(this object data, bool enumAsString = false, JsonSerializerSettings customJsonSettings = null)
		{
			try
			{
				if (data == null) return "[]";

				if (enumAsString)
					return JsonConvert.SerializeObject(data, Formatting.None, new StringEnumConverter());
				else
					return JsonConvert.SerializeObject(data, Formatting.None, customJsonSettings ?? _jsonSettings);
			}
			catch (Exception)
			{
				return "[]";
			}
		}

		/// <summary>
		/// Devuelve una cadena con el objeto serializado en formato JSON
		/// </summary>
		/// <param name="data">El objeto para serializar en JSON</param>
		/// <param name="jsonSettings">La configuración de json</param>
		/// <param name="prettyPrint">Hacer más legible el json para las personas</param>
		/// <returns></returns>
		public static string ToJson(this object data, JsonSerializerSettings jsonSettings, bool prettyPrint = false)
		{
			try
			{
				return JsonConvert.SerializeObject(data, prettyPrint ? Formatting.Indented : Formatting.None, jsonSettings ?? _jsonSettings);
			}
			catch (Exception)
			{
				return "{}"; // La cadena vacía en formato json
			}
		}
		#endregion

		#region Default FromJson
		/// <summary>
		/// Devuelve un objeto .NET a partir de una cadena en formato JSON.
		/// </summary>
		/// <typeparam name="T">El tipo de objeto .NET que será devuelto</typeparam>
		/// <param name="data">La cadena JSON que representa el objeto serializado</param>
		/// <param name="customJsonSettings">Opciones personalizadas</param>
		/// <returns></returns>
		public static T FromJson<T>(this string data, JsonSerializerSettings customJsonSettings = null)
		{
			try
			{
				return (T)JsonConvert.DeserializeObject(data, typeof(T), customJsonSettings ?? _jsonSettings);
			}
			catch (Exception)
			{
				return default;
			}
		}

		/// <summary>
		/// Devuelve un objeto .NET a partir de una cadena en formato JSON.
		/// </summary>
		/// <typeparam name="T">El tipo de objeto .NET que será devuelto</typeparam>
		/// <param name="data">La cadena JSON que representa el objeto serializado</param>
		/// <param name="customJsonSettings">Opciones personalizadas</param>
		/// <returns></returns>
		public static T FromJson<T>(this object data, JsonSerializerSettings customJsonSettings = null) where T : class
		{
			if (data == null) return null;
			return JsonConvert.DeserializeObject<T>(data.ToString(), customJsonSettings ?? _jsonSettings);
		}

		/// <summary>
		/// Rellena el objeto .NET a partir de una cadena en formato JSON.
		/// </summary>
		/// <param name="target">El objeto .NET que será rellenado</param>
		/// <param name="value">La cadena JSON que representa el objeto serializado</param>
		/// <param name="customJsonSettings">Opciones personalizadas</param>
		/// <returns></returns>
		public static object FromJson(this string value, object target, JsonSerializerSettings customJsonSettings = null)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(customJsonSettings ?? _jsonSettings);

			using StringReader stringReader = new(value);
			using JsonReader jsonReader = new JsonTextReader(stringReader);

			jsonSerializer.Populate(jsonReader, target);

			if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
				throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");

			return target;
		}


		/// <summary>
		/// Rellena el objeto .NET a partir de una cadena en formato JSON.
		/// </summary>
		/// <param name="targetType">El tipo del objeto .NET que será rellenado</param>
		/// <param name="value">La cadena JSON que representa el objeto serializado</param>
		/// <param name="customJsonSettings">Opciones personalizadas</param>
		/// <returns></returns>
		public static object FromJson(this string value, Type targetType, JsonSerializerSettings customJsonSettings = null)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(customJsonSettings ?? _jsonSettings);
			var target = Activator.CreateInstance(targetType);
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(value)))
			{
				jsonSerializer.Populate(jsonReader, target);

				if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
					throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
			}

			return target;
		}

		/// <summary>
		/// Indica si la cadena json representa un objeto vacío [] o {}
		/// </summary>
		/// <param name="json">La cadena JSON</param>
		/// <returns></returns>
		public static bool IsJsonEmpty(this string json)
		{
			return string.IsNullOrWhiteSpace(json) || json == "[]" || json == "{}";
		}
		#endregion

		#region Copy properties from
		/// <summary>
		/// Copia las propiedades de un objeto especifico a otro por nomenclatura y tipo de dato, y lo devuelve
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">Objeto destino</param>
		/// <param name="source">Objeto origen</param>
		/// <param name="optionsSetup">Opciones de copia</param>
		/// <returns></returns>
		public static T CopyPropertiesFrom<T>(this T target, T source, Action<CopyPropertiesOptions> optionsSetup = null)
		{
			target.CopyPropertiesFrom((object)source, optionsSetup);
			return target;
		}

		/// <summary>
		/// Copia las propiedades de un objeto cualquiera a otro por nomenclatura y tipo de dato
		/// </summary>
		/// <param name="target">Objeto destino</param>
		/// <param name="source">Objeto origen</param>
		/// <param name="optionsSetup">Opciones de copia</param>
		public static void CopyPropertiesFrom(this object target, object source, Action<CopyPropertiesOptions> optionsSetup = null)
		{
			CopyPropertiesOptions options = new();
			optionsSetup?.Invoke(options);

			var localJsonSettings = new JsonSerializerSettings
			{
				PreserveReferencesHandling = PreserveReferencesHandling.None,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				FloatFormatHandling = FloatFormatHandling.DefaultValue,
				NullValueHandling = NullValueHandling.Include,
				MaxDepth = 64, // Default value
				ContractResolver = new CopyPropertiesContractResolver(options)
			};

			var jsonSource = ToJsonCopyProperties(source, localJsonSettings, options);
			FromJsonCopyProperties(jsonSource, target, localJsonSettings);
		}

		private static string ToJsonCopyProperties(object data, JsonSerializerSettings jsonSettings, CopyPropertiesOptions options)
		{
			try
			{
				JsonSerializer jsonSerializer = JsonSerializer.Create(jsonSettings);

				using StringWriter sw = new();
				if (jsonSettings.MaxDepth != null)
				{
					using var jsonWriter = new CopyPropertiesJsonTextWriter(sw);
					options.CopyByDepth = () => jsonWriter.CurrentDepth <= options.MaxCopyDepth;

					var serializer = JsonSerializer.Create(jsonSettings);
					serializer.Serialize(jsonWriter, data);
					return sw.ToString();
				}

				jsonSerializer.Serialize(sw, data);
				return sw.ToString();
			}
			catch (Exception)
			{
				return "{}"; // La cadena vacía en formato json
			}
		}

		private static void FromJsonCopyProperties(string value, object target, JsonSerializerSettings jsonSettings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create(jsonSettings);

			using StringReader stringReader = new(value);
			using JsonReader jsonReader = new JsonTextReader(stringReader);

			jsonSerializer.Populate(jsonReader, target);

			if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
				throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
		}
		#endregion
	}
}