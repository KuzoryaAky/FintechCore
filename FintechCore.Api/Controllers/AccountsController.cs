using FintechCore.Application.DTOs;
using FintechCore.Domain.Entities;
using FintechCore.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FintechCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        IAccountRepository _accountRepository;
        public AccountsController(IAccountRepository account)
        {
            _accountRepository = account;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            var account = new Account
            {
                UserId = dto.UserId,
                Balance = dto.InitialBalance,
                Status = "Active"
            };

            await _accountRepository.AddAsync(account);

            return Ok(account);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account is null) return NotFound("Счёт не найден");

            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount([FromBody] UpdateAccountDto dto, int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account is null) return NotFound();

            account.Balance = dto.Balance;
            account.Status = dto.Status;

            await _accountRepository.UpdateAsync(account);

            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account is null) return NotFound();
            
            await _accountRepository.DeleteAsync(account);

            return NoContent();
        }

        
    }
}
