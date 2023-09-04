namespace EmailService.ProcessEvent
{
  public interface IProcessEvent
  {
    void ProcessEventSend(string message);
  }
}
