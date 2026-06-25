using FinTech.API.DTOs.Requests;
using FinTech.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost("simulate")]
    [AllowAnonymous]
    public IActionResult Simulate([FromBody] SimulateLoanRequest request)
    {
        var result = _loanService.SimulateAsync(request);
        return Ok(result.Result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanRequest request)
    {
        try
        {
            var result = await _loanService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? userId)
    {
        var loans = await _loanService.GetAllAsync(userId);
        return Ok(loans);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var loan = await _loanService.GetByIdAsync(id);
        if (loan is null)
            return NotFound(new { error = "Préstamo no encontrado" });
        return Ok(loan);
    }

    [HttpGet("{id}/schedule")]
    public async Task<IActionResult> GetSchedule(Guid id)
    {
        try
        {
            var schedule = await _loanService.GetScheduleAsync(id);
            return Ok(schedule);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        try
        {
            var loan = await _loanService.ApproveAsync(id);
            return Ok(loan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        try
        {
            var loan = await _loanService.RejectAsync(id);
            return Ok(loan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
