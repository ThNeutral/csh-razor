using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace razor {
    public class JSON {
        public static JsonSerializerOptions options = new() { Converters = { new Vector2JsonConverter() } };
        public static string Stringify(object obj) {
            return JsonSerializer.Serialize(obj, options);
        }
        public static T? Parse<T>(string str) {
            return JsonSerializer.Deserialize<T>(str, options);
        }
    }
    public class Vector2JsonConverter : JsonConverter<Vector2> {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
            float x = jsonObject.GetProperty("X").GetSingle();
            float y = jsonObject.GetProperty("Y").GetSingle();
            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options) {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }
}