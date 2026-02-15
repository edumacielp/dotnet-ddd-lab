using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Repositories;
using LibraryManagement.Infrastructure.Persistence;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IMongoCollection<Member> _members;

    public MemberRepository(MongoDbContext context)
    {
        _members = context.GetCollection<Member>("Members");
    }

    public async Task<Member?> GetByIdAsync(string id)
        => await _members.Find(m => m.Id == id).FirstOrDefaultAsync();

    public async Task<Member?> GetByEmailAsync(string email)
        => await _members.Find(m => m.Email.Value == email).FirstOrDefaultAsync();

    public async Task<IEnumerable<Member>> GetAllAsync()
        => await _members.Find(_ => true).ToListAsync();

    public async Task<IEnumerable<Member>> GetActiveMembersAsync()
        => await _members.Find(m => m.Status == MembershipStatus.Active).ToListAsync();

    public async Task<IEnumerable<Member>> SearchByNameAsync(string name)
    {
        var filter = Builders<Member>.Filter.Regex(m => m.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
        return await _members.Find(filter).ToListAsync();
    }

    public async Task AddAsync(Member member)
        => await _members.InsertOneAsync(member);

    public async Task UpdateAsync(Member member)
        => await _members.ReplaceOneAsync(m => m.Id == member.Id, member);

    public async Task DeleteAsync(string id)
        => await _members.DeleteOneAsync(m => m.Id == id);
}
