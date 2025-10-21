using Company.App.Application.Dto;
using Company.App.Application.Interface.UseCases;
using Company.App.Cross.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Company.App.Service.WebApi.Controllers.v1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Policy-Transaction")]
    public class TransactionsController: ControllerBase
    {
        private readonly ITransactionApplication _transactionApplication;
        private readonly ILogger<TransactionsController> _logger;
        public TransactionsController(ITransactionApplication transactionApplication, ILogger<TransactionsController> logger)
        {
            _transactionApplication = transactionApplication;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new financial transaction.
        /// </summary>
        /// <param name="transactionDto">Transaction data to be inserted.</param>
        /// <returns>Returns the created transaction ID.</returns>
        /// 
        [HttpPost("InsertTransaction")]
        [ProducesResponseType(typeof(ResponseT<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseT<int>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseT<int>>> InsertTransaction([FromBody] TransactionDto transactionDto)    
        {
            _logger.LogInformation("InsertTransaction called with InsertTransaction");

            var response = await _transactionApplication.InsertTransactionAsync(transactionDto);

            if (!response.IsSuccess)
                return BadRequest(response);

            return response;
        }
        [EnableRateLimiting("GetSlidingWindowLimiter")]
        [HttpPost("GetTransaction")]
        public async Task<ActionResult<ResponseT<TransactionResponseDto>>> GetTransaction([FromBody] TransactionRequestDto request)
        {
            _logger.LogInformation("InsertTransaction called with GetTransaction");
            var response = await _transactionApplication.GetTransactionAsync(request);

            if (!response.IsSuccess)
                return NotFound(response);

            return Ok(response);
        }

    }
}
