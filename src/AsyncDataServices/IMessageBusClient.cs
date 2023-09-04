using EmitService.Models;

namespace EmitService.AsyncDataServices
{
  public interface IMessageBusClient
  {
    //   void PublishNewTest(Test Test);
    //   void PublishNewUser(User user);
    void PublishNewEmailDocument(Email email);
  }
}
