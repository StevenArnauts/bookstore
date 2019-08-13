using System;
using System.Linq;
using Newtonsoft.Json;

namespace Utilities.Mvc {

	public class GuidConverter : JsonConverter {

		private readonly Type[] convertableTypes = { typeof (Guid), typeof (Guid?) };

		public override bool CanRead => false;

		public override bool CanWrite => true;

		public override bool CanConvert(Type objectType) {
			return this.convertableTypes.Contains(objectType);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			if (value == null)
			{
				return;
			}
			Guid guid = value as Guid? ?? ((Guid?)value).Value;
			writer.WriteValue(guid.ToString("N").ToUpper());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			throw new NotImplementedException();
		}

	}

}