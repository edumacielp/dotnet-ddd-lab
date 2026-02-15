using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
    {
        var loans = await _loanService.GetAllAsync();
        return Ok(loans);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanDto>> GetById(string id)
    {
        var loan = await _loanService.GetByIdAsync(id);
        if (loan == null)
            return NotFound();

        return Ok(loan);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetActive()
    {
        var loans = await _loanService.GetActiveLoansAsync();
        return Ok(loans);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdue()
    {
        var loans = await _loanService.GetOverdueLoansAsync();
        return Ok(loans);
    }

    [HttpGet("member/{memberId}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByMember(string memberId)
    {
        var loans = await _loanService.GetLoansByMemberAsync(memberId);
        return Ok(loans);
    }

    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetByBook(string bookId)
    {
        var loans = await _loanService.GetLoansByBookAsync(bookId);
        return Ok(loans);
    }

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto)
    {
        try
        {
            var loan = await _loanService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<LoanDto>> Return(string id)
    {
        try
        {
            var loan = await _loanService.ReturnBookAsync(id);
            return Ok(loan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/renew")]
    public async Task<ActionResult<LoanDto>> Renew(string id, [FromQuery] int additionalDays = 14)
    {
        try
        {
            var loan = await _loanService.RenewLoanAsync(id, additionalDays);
            return Ok(loan);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}