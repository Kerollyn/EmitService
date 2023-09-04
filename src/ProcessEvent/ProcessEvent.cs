using System.Text.Json;
using EmailService.Models;
using EmitService.Interfaces.Repositories;
using EmitService.Models;

namespace EmailService.ProcessEvent
{
  public class ProcessEvent : IProcessEvent
  {
    private readonly IServiceScopeFactory _scopeFactory;

    public ProcessEvent(IServiceScopeFactory scopeFactory)
    {
      _scopeFactory = scopeFactory;
    }

    public void ProcessEventSend(string message)
    {
      var eventType = DetermineEvent(message);

      switch (eventType)
      {
        case EventType.EmailPublished:
          eventNewEmailDocument(message);
          break;
        case EventType.UserPublished:
          eventNewUser(message);
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notifcationMessage)
    {
      Console.WriteLine("--> Determining Event");

      var eventType = JsonSerializer.Deserialize<Email>(notifcationMessage);
      Console.WriteLine($"{eventType}");
      switch (eventType.EmailType)
      {
        case "Email_Published":
          Console.WriteLine("--> Email Published Event Detected");
          return EventType.EmailPublished;
        case "User_Published":
          Console.WriteLine("--> User Published Event Detected");
          return EventType.UserPublished;
        default:
          Console.WriteLine("--> Could not determine the event type");
          return EventType.Undetermined;
      }
    }

    private void eventNewUser(string notification)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var user = JsonSerializer.Deserialize<User>(notification);
        var repository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();

        try
        {
          var filePathUserModel = Path.Combine("StaticFiles", "newuser.html");
          var htmlBody = System.IO.File.ReadAllText(filePathUserModel);
          htmlBody = htmlBody.Replace("{{USERNAME}}", user.Name);
          htmlBody = htmlBody.Replace("{{EMAIL}}", user.Email);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> {ex.Message}");
        }
      }
    }

    private void eventNewEmailDocument(string notification)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var email = JsonSerializer.Deserialize<Email>(notification);
        var repository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();

        try
        {
          var filePathEmailModel = Path.Combine("StaticFiles", "newdocument.html");
          Console.WriteLine($"File path: {filePathEmailModel}");
          var htmlBody = System.IO.File.ReadAllText(filePathEmailModel);
          htmlBody = htmlBody.Replace("{{CorpoEmail}}", email.Body);
          htmlBody = htmlBody.Replace("{{EMAIL}}", email.mailTo);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> {ex.Message}");

        }
      }
    }


    enum EventType
    {
      EmailPublished,
      UserPublished,
      Undetermined
    }
  }
}
