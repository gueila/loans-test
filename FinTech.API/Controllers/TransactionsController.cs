using FinTech.API.DTOs.Requests;
using FinTech.API.Enums;
using FinTech.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTech.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var result = await _transactionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] TransactionType? type,
        [FromQuery] TransactionStatus? status)
    {
        var transactions = await _transactionService.GetAllAsync(type, status);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction is null)
            return NotFound(new { error = "Transacción no encontrada" });
        return Ok(transaction);
    }
}
