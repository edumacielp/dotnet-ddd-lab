using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetById(string id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetActive()
    {
        var members = await _memberService.GetActiveMembersAsync();
        return Ok(members);
    }

    [HttpGet("search/{name}")]
    public async Task<ActionResult<IEnumerable<MemberDto>>> SearchByName(string name)
    {
        var members = await _memberService.SearchByNameAsync(name);
        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
    {
        try
        {
            var member = await _memberService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> Update(string id, [FromBody] UpdateMemberDto dto)
    {
        try
        {
            var member = await _memberService.UpdateAsync(id, dto);
            return Ok(member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/suspend")]
    public async Task<ActionResult<MemberDto>> Suspend(string id)
    {
        try
        {
            var member = await _memberService.SuspendAsync(id);
            return Ok(member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/reactivate")]
    public async Task<ActionResult<MemberDto>> Reactivate(string id)
    {
        try
        {
            var member = await _memberService.ReactivateAsync(id);
            return Ok(member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _memberService.DeleteAsync(id);
        return NoContent();
    }
}