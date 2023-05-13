using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using SmartHomeServer;
using SmartHomeServer.Models;
using SmartHomeServer.Services;
using SmartHomeServer.Settings;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
var configuration = builder.Configuration;
configuration.AddEnvironmentVariables();
// Register OrderDatabaseSettings

var orderDbSettingsSection = configuration.GetSection(nameof(DatabaseSettings));
services.Configure<DatabaseSettings>(orderDbSettingsSection);

services.AddSingleton<IDatabaseSettings>(sp =>
  sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

// Register MongoClient
services.AddSingleton<IMongoClient>(serviceProvider => {
  var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
  return new MongoClient(settings.ConnectionString);
});
services.AddScoped<IDeviceService, DeviceService>();
services.AddCors(options =>
{
  options.AddPolicy("CorsPolicy",
    builder => builder.AllowAnyMethod()
      .AllowAnyHeader()
      .SetIsOriginAllowed(origin => true) // allow any origin
      .AllowCredentials());
});
services.AddAuthentication();

services.AddControllers();

services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "DeviceService", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Device API V1");
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
});
var factory = new MqttFactory();
var client = factory.CreateMqttClient();

var mqttHost = configuration["Settings:MqttHost"];
var mqttPort = int.Parse(configuration["Settings:MqttPort"]);
Console.WriteLine($"mqttHost - {mqttHost}");
Console.WriteLine($"mqttPort - {mqttPort}");
var options = new MqttClientOptionsBuilder()
  .WithTcpServer(mqttHost, mqttPort) // Change to the IP address and port of your Mosquitto server
  .Build();

await client.ConnectAsync(options);

Console.WriteLine($"IsConnected - {client.IsConnected}");
var topicFilter = new MqttTopicFilterBuilder()
  .WithTopic("smart-home/devices/#") 
  .Build();
ServiceProvider serviceProvider = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(services);
var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
var databaseSettings = serviceProvider.GetRequiredService<IDatabaseSettings>();
client.ApplicationMessageReceivedAsync += async (e) =>
{
  await HandleMessage(e);
};
await client.SubscribeAsync(topicFilter);

app.Run();

async Task HandleMessage(MqttApplicationMessageReceivedEventArgs e)
{
  Console.WriteLine($"IsConnected - {client.IsConnected}");
  // Convert the payload to a string
  var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

  // Output the topic and message payload
  Console.WriteLine($"Received message from topic '{e.ApplicationMessage.Topic}': {payload}");
  Device? device = JsonConvert.DeserializeObject<Device>(payload, new JsonSerializerSettings
  {
    TypeNameHandling = TypeNameHandling.Auto,
    Converters = { new DeviceConverter() }
  });

  if (device != null)
    switch (device.Type)
    {
      case DeviceType.AC:
        ACDeviceService acDeviceService = new ACDeviceService(mongoClient, databaseSettings);
        await acDeviceService.Process(device);
        break;
      case DeviceType.Alarm:
        AlarmDeviceService alarmDeviceService = new AlarmDeviceService(mongoClient, databaseSettings);
        await alarmDeviceService.Process(device);
        break;
      case DeviceType.Camera:
        CameraDeviceService cameraDeviceService = new CameraDeviceService(mongoClient, databaseSettings);
        await cameraDeviceService.Process(device);
        break;
      case DeviceType.Light:
        LightDeviceService lightDeviceService = new LightDeviceService(mongoClient, databaseSettings);
        await lightDeviceService.Process(device);
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
}