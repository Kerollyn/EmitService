using EmitService.Models;

namespace EmitService.Interfaces.Repositories
{
  public interface IEmailRepository
  {
    Task CreateAsync(Email email);
    IEnumerable<Email> GetAllAsync();
  }
}
