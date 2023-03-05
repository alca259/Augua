using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Newtonsoft.Json;

/// <summary>
/// Extensiones de Json
/// </summary>
public static class JsonExtensions
{
    static readonly JsonSerializerSettings _jsonSettings = new()
    {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        ContractResolver = new MyContractResolver()
    };

    internal static JsonSerializerSettings Copy(this JsonSerializerSettings options)
    {
        return new JsonSerializerSettings(options);
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

    #region FromJson
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

        using StringReader stringReader = new StringReader(value);
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
    #endregion

    private sealed class MyContractResolver : DefaultContractResolver
    {
        public MyContractResolver() : base()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
