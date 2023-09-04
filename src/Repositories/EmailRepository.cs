using System.Text;
using EmitService.Database.Interface;
using EmitService.Extension;
using EmitService.Interfaces.Repositories;
using EmitService.Models;
using Dapper;

namespace EmitService.Repositories
{
  public class EmailRepository : IEmailRepository
  {
    private readonly IDatabase _database;

    public EmailRepository(
      IDatabase database)
    {
      _database = database;
    }

    public async Task CreateAsync(Email email)
    {
      using (var cn = _database.GetDbConnection())
      {
        var sql = new StringBuilder();
        sql.Append(" INSERT INTO Email(mailTo,cc,Subject,Body,EmailType,Caller,emailuiid,Status) ");
        sql.Append(" VALUES(@mailTo, @cc, @Subject, @Body, @EmailType, @Caller, @emailuiid, @Status) ");

        var filePathEmailModel = Path.Combine("StaticFiles", "newDocument.html");
        var htmlBody = System.IO.File.ReadAllText(filePathEmailModel);
        htmlBody = htmlBody.Replace("{{USERNAME}}", email.mailTo);
        htmlBody = htmlBody.Replace("{{EMAIL}}", email.mailTo);


        await cn.InsertPostgresAsync(sql.ToString(), new
        {
          mailTo = email.mailTo,
          cc = email.cc,
          Subject = email.Subject,
          Body = email.Body = htmlBody,
          EmailType = email.EmailType,
          Caller = email.Caller,
          emailuiid = email.emailuiid,
          Status = email.Status
        });
      }
    }

    public IEnumerable<Email> GetAllAsync()
    {
      using (var cn = _database.GetDbConnection())
      {
        var sql = new StringBuilder();
        sql.Append(" SELECT * FROM email; ");

        return cn.Query<Email>(sql.ToString());
      }
    }
  }
}
