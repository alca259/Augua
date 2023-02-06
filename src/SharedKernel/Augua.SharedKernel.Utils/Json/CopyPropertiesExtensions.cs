using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace System.Reflection;

/// <summary>
/// Extensiones Json para copia de propiedades
/// </summary>
public static class CopyPropertiesExtensions
{
    /// <summary>
    /// Copia las propiedades de un objeto cualquiera a otro por nomenclatura y tipo de dato
    /// </summary>
    /// <param name="target">Objeto destino</param>
    /// <param name="source">Objeto origen</param>
    /// <param name="optionsSetup">Opciones de copia</param>
    /// <exception cref="ArgumentException">Al intentar copiar listados o enumerados directamente, sin iterar</exception>
    public static T CopyPropertiesFrom<T>(this T target, T source, Action<CopyPropertiesOptions> optionsSetup = null) where T : class
    {
        CopyPropertiesOptions options = new();
        optionsSetup?.Invoke(options);

        target.CopyPropertiesJsonInternal(source, options);
        return target;
    }

    /// <summary>
    /// Copia las propiedades de un objeto cualquiera a otro por nomenclatura y tipo de dato
    /// </summary>
    /// <param name="target">Objeto destino</param>
    /// <param name="source">Objeto origen</param>
    /// <param name="optionsSetup">Opciones de copia</param>
    /// <exception cref="ArgumentException">Al intentar copiar listados o enumerados directamente, sin iterar</exception>
    public static object CopyPropertiesFrom(this object target, object source, Action<CopyPropertiesOptions> optionsSetup = null)
    {
        CopyPropertiesOptions options = new();
        optionsSetup?.Invoke(options);

        target.CopyPropertiesJsonInternal(source, options);
        return target;
    }

    private static void CopyPropertiesJsonInternal(this object target, object source, CopyPropertiesOptions options)
    {
        var localJsonSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            FloatFormatHandling = FloatFormatHandling.DefaultValue,
            NullValueHandling = NullValueHandling.Include,
            MaxDepth = 64, // Máximo que vamos a permitir independientemente del valor de options.MaxCopyDepth
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
            string result;

            using StringWriter sw = new();
            if (jsonSettings.MaxDepth != null)
            {
                using var jsonWriter = new CopyPropertiesJsonTextWriter(sw);
                options.CopyByDepth = (bool isPredefinedType) =>
                {
                    // Si es tipo predefinido, ignoraremos profundidad ya que solo debe aplicar a clases y estructuras
                    if (isPredefinedType) return true;
                    var copy = jsonWriter.CurrentDepth <= options.MaxCopyDepth;
                    return copy;
                };

                var serializer = JsonSerializer.Create(jsonSettings);
                serializer.Serialize(jsonWriter, data);
                result = sw.ToString();
                return result;
            }

            jsonSerializer.Serialize(sw, data);
            result = sw.ToString();
            return result;
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

    /// <summary>
    /// Escritor JSON para detectar el nivel de profundidad al que nos encontramos.
    /// </summary>
    private sealed class CopyPropertiesJsonTextWriter : JsonTextWriter
    {
        public CopyPropertiesJsonTextWriter(TextWriter textWriter) : base(textWriter)
        {
        }

        public int CurrentDepth { get; private set; } = 0;

        public override void WriteStartObject()
        {
            CurrentDepth++;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth--;
            base.WriteEndObject();
        }
    }

    /// <summary>
    /// Resolvedor Json para copia de propiedades
    /// </summary>
    private sealed class CopyPropertiesContractResolver : DefaultContractResolver
    {
        private CopyPropertiesOptions Options { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public CopyPropertiesContractResolver(CopyPropertiesOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MemberInfo[] fields = objectType.GetFields(flags);
            return fields
                .Concat(objectType.GetProperties(flags).Where(propInfo => propInfo.CanWrite))
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(w => w.SetMethod != null)
                .ToList();
            var fields = type.GetFields(BindingFlags.Public |/* BindingFlags.NonPublic |*/ BindingFlags.Instance)
                .ToList();

            var props = properties
                .Select(p => CreateProperty(p, memberSerialization))
                .Union(fields
                .Select(f => CreateProperty(f, memberSerialization)))
                .ToList();

            foreach (JsonProperty prop in props)
            {
                var shouldSerialize = prop.ShouldSerialize;
                prop.ShouldSerialize = obj =>
                {
                    var copyByDepth = Options.CopyByDepth(PredefinedTypes.Contains(prop.PropertyType));
                    var copyBySerialize = shouldSerialize == null || shouldSerialize(obj);
                    return copyByDepth && copyBySerialize;
                };

                prop.Writable = true;
                prop.Readable = true;

                if (Options.ExcludePropertiesNames.Contains(prop.PropertyName, StringComparer.InvariantCultureIgnoreCase))
                {
                    prop.Ignored = true;
                    prop.Writable = false;
                    prop.Readable = false;
                    prop.Required = Required.Default;
                    continue;
                }

                if (Options.ExcludeClassPropertiesNamePrefix.Any(prefix => prop.PropertyName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    prop.Ignored = true;
                    prop.Writable = false;
                    prop.Readable = false;
                    prop.Required = Required.Default;
                    continue;
                }

                if (Options.ExcludeClassPropertiesNameSuffix.Any(suffix => prop.PropertyName.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    prop.Ignored = true;
                    prop.Writable = false;
                    prop.Readable = false;
                    prop.Required = Required.Default;
                }
            }

            return props;
        }
    }

    internal static readonly Type[] PredefinedTypes = {
        typeof(Object),
        typeof(Boolean),
        typeof(Char),
        typeof(String),
        typeof(SByte),
        typeof(Byte),
        typeof(Int16),
        typeof(UInt16),
        typeof(Int32),
        typeof(UInt32),
        typeof(Int64),
        typeof(UInt64),
        typeof(Single),
        typeof(Double),
        typeof(Decimal),
        typeof(DateTime),
        typeof(TimeSpan),
        typeof(Guid),
        typeof(Math),
        typeof(Convert)
    };
}
