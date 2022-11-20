using Newtonsoft.Json;

namespace System.Reflection
{
	internal sealed class CopyPropertiesJsonTextWriter : JsonTextWriter
	{
		public CopyPropertiesJsonTextWriter(TextWriter textWriter) : base(textWriter)
		{
		}

		public int CurrentDepth { get; private set; }

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
}