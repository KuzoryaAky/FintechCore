using FintechCore.Application.DTOs;
using FintechCore.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FintechCore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransfersController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    private readonly ITransferService _transferService;
    private readonly IOutboxRepository _outboxRepository;

    public TransfersController(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IOutboxRepository outboxRepository, ITransferService transferService)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _outboxRepository = outboxRepository;
        _transferService = transferService;
    }

    [HttpPost]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
    {
        var result = await _transferService.TransferAsync(dto.FromAccountId, dto.ToAccountId, dto.Amount);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result);
    }
}
