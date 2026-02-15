using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Repositories;
using LibraryManagement.Core.ValueObjects;

namespace LibraryManagement.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<MemberDto?> GetByIdAsync(string id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        return member == null ? null : MapToDto(member);
    }

    public async Task<IEnumerable<MemberDto>> GetAllAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Select(MapToDto);
    }

    public async Task<IEnumerable<MemberDto>> GetActiveMembersAsync()
    {
        var members = await _memberRepository.GetActiveMembersAsync();
        return members.Select(MapToDto);
    }

    public async Task<IEnumerable<MemberDto>> SearchByNameAsync(string name)
    {
        var members = await _memberRepository.SearchByNameAsync(name);
        return members.Select(MapToDto);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
    {
        var existingMember = await _memberRepository.GetByEmailAsync(dto.Email);
        if (existingMember != null)
            throw new InvalidOperationException("A member with this email already exists");

        var email = new Email(dto.Email);
        var member = new Member(dto.Name, email, dto.PhoneNumber);

        await _memberRepository.AddAsync(member);
        return MapToDto(member);
    }

    public async Task<MemberDto> UpdateAsync(string id, UpdateMemberDto dto)
    {
        var member = await _memberRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException("Member not found");

        member.UpdateContactInfo(dto.PhoneNumber);
        await _memberRepository.UpdateAsync(member);

        return MapToDto(member);
    }

    public async Task<MemberDto> SuspendAsync(string id)
    {
        var member = await _memberRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException("Member not found");

        member.Suspend();
        await _memberRepository.UpdateAsync(member);

        return MapToDto(member);
    }

    public async Task<MemberDto> ReactivateAsync(string id)
    {
        var member = await _memberRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException("Member not found");

        member.Reactivate();
        await _memberRepository.UpdateAsync(member);

        return MapToDto(member);
    }

    public async Task DeleteAsync(string id)
        => await _memberRepository.DeleteAsync(id);

    private static MemberDto MapToDto(Member member)
        => new(
            member.Id,
            member.Name,
            member.Email.Value,
            member.PhoneNumber,
            member.MembershipDate,
            member.Status.ToString(),
            member.BorrowedBookIds.Count,
            member.CreatedAt,
            member.UpdatedAt
        );
}