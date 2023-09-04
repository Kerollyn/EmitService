using EmailService.Extension;
using EmailService.ProcessEvent;
using EmitService.AsyncDataServices;
using EmitService.Database.Interface;
using EmitService.Database.Repository;
using EmitService.Interfaces.Repositories;
using EmitService.Repositories;
using Microsoft.AspNetCore.Http.Json;
using Serilog;

try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.Services.Configure<JsonOptions>(opt =>
  {
    opt.SerializerOptions.WriteIndented = true;
  });

  SerilogExtension.AddSerilogApi(builder.Configuration);
  builder.Host.UseSerilog(Log.Logger);
  builder.Host.UseSerilog((context, config) =>
  {
    var connectionString = context.Configuration.GetConnectionString("logging");

    config.WriteTo.PostgreSQL(connectionString, "LogsSystem", needAutoCreateTable: true)
     .MinimumLevel.Error();

    if (context.HostingEnvironment.IsProduction() == false)
    {
      config.WriteTo.Console().MinimumLevel.Information();
    }

  });


  builder.Services.AddSingleton<IDatabase, PostgresDatabase>();
  builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
  builder.Services.AddScoped<IEmailRepository, EmailRepository>();
  builder.Services.AddSingleton<IProcessEvent, ProcessEvent>();

  builder.Services.AddControllers();
  builder.Services.AddSwaggerGen(c =>
  {
    c.SwaggerDoc("v1", new() { Title = "EmitService API", Version = "v1" });
  });

  var app = builder.Build();

  if (builder.Environment.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmitService API v1"));
  }

  // app.MapControllers();
  app.UseRouting();
  app.UseAuthorization();


  app.UseEndpoints(endpoints =>
  {
    endpoints.MapControllers();
    //endpoints.MapGrpcService<GrpcEmailService>();

    endpoints.MapGet("/protos/emails.proto", async context =>
    {
      await context.Response.WriteAsync(File.ReadAllText("Protos/emails.proto"));
    });
  });

  app.Run();

}
catch (Exception ex)
{
  Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
  Log.Information("Server Shutting down...");
  Log.CloseAndFlush();
}
