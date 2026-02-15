using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(string id);
    Task<Member?> GetByEmailAsync(string email);
    Task<IEnumerable<Member>> GetAllAsync();
    Task<IEnumerable<Member>> GetActiveMembersAsync();
    Task<IEnumerable<Member>> SearchByNameAsync(string name);
    Task AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(string id);
}
