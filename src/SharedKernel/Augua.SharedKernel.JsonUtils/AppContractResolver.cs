using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace System.Reflection
{
	internal sealed class AppContractResolver : DefaultContractResolver
	{
		public AppContractResolver()
		{
		}

		// Para evitar el error "Cannot be deserialized because the property does not have a public setter"
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

		protected override List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			MemberInfo[] fields = objectType.GetFields(flags);
			return fields
				.Concat(objectType.GetProperties(flags).Where(propInfo => propInfo.CanWrite))
				.ToList();
		}
	}
}