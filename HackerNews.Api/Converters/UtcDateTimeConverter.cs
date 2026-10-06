using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace HackerNews.Api.Converters;
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
 public override DateTime Read(ref Utf8JsonReader reader,Type type,JsonSerializerOptions options) => DateTimeOffset.Parse(reader.GetString()!,CultureInfo.InvariantCulture).UtcDateTime;
 public override void Write(Utf8JsonWriter writer,DateTime value,JsonSerializerOptions options) => writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'+00:00'",CultureInfo.InvariantCulture));
}
