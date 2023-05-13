using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHomeServer.Models;

namespace SmartHomeServer
{
    public class DeviceConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return typeof(Device).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      JObject jsonObject = JObject.Load(reader);
      JToken typeToken = jsonObject.GetValue("Type", StringComparison.OrdinalIgnoreCase);
      string typeName = "SmartHomeServer.Models." + typeToken.ToString() + "Device";
      Type type = Type.GetType(typeName);
      if (type == null)
      {
        throw new ArgumentException($"Invalid type name '{typeName}'.");
      }

      object target = Activator.CreateInstance(type);
      serializer.Populate(jsonObject.CreateReader(), target);
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }

}
