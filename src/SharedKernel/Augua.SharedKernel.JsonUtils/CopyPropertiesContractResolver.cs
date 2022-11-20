using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace System.Reflection
{
	/// <summary>
	/// Resolvedor Json para copia de propiedades
	/// </summary>
	internal sealed class CopyPropertiesContractResolver : DefaultContractResolver
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
				.Select(p => base.CreateProperty(p, memberSerialization))
				.Union(fields
				.Select(f => base.CreateProperty(f, memberSerialization)))
				.ToList();

			foreach (JsonProperty prop in props)
			{
				var shouldSerialize = prop.ShouldSerialize;
				prop.ShouldSerialize = obj => Options.CopyByDepth() && (shouldSerialize == null || shouldSerialize(obj));

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
}