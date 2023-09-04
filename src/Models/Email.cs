using EmailService.Models;

namespace EmitService.Models
{
  public class Email
  {
    public int Id { get; set; }
    public string mailTo { get; set; }
    public string cc { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string EmailType { get; set; }
    public string Caller { get; set; }
    public string emailuiid { get; set; }
    public string Status => EmailStatus.ToString();
    private EmailStatus EmailStatus { get; set; }

    // public Email(Email email)
    // {
    //   mailTo = email.mailTo;
    //   cc = email.cc;
    //   Subject = email.Subject;
    //   Body = email.Body;
    //   Caller = email.Caller;
    //   emailuiid = email.emailuiid;
    //   EmailStatus = EmailStatus.Processing;
    // }
  }
}
