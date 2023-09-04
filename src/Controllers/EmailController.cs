
using EmailService.ProcessEvent;
using EmitService.AsyncDataServices;
using EmitService.Interfaces.Repositories;
using EmitService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmitService.Controllers
{
  [Route("v1/api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly IEmailRepository _emailRepository;
    private readonly IMessageBusClient _messageBusClient;
    private readonly IProcessEvent _processEvent;

    public EmailController(
      IEmailRepository emailRepository,
      IMessageBusClient messageBusClient, IProcessEvent processEvent)
    {
      _emailRepository = emailRepository;
      _messageBusClient = messageBusClient;
      _processEvent = processEvent;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Email>>> GetAllAsync()
    {
      //System.Guid.NewGuid().ToString();
      try
      {
        var result = _emailRepository.GetAllAsync();
        return Ok(result);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Erro: {ex.Message}");
        throw;
      }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Email email)
    {
      await _emailRepository.CreateAsync(email);
      //_processEvent.ProcessEventSend("Email_Published");
      try
      {
        //var emailPublished = email;

        _messageBusClient.PublishNewEmailDocument(email);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not send: {ex.Message}");
      }

      //return Ok(emailPublished);
      return Ok(email);
    }
  }

}
