using Microsoft.AspNetCore.Mvc;
using PaymentsApi.Dtos;
using PaymentsApi.Services;
namespace PaymentsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
    {
    private readonly IPaymentService _service;
    private readonly ILogger<PaymentsController> _logger;
    public PaymentsController(IPaymentService service, ILogger<PaymentsController> logger)
    {
        _service = service;
        _logger = logger;
    }
    /// <summary>
    /// Create payment (idempotent by clientRequestId).
    /// If a payment with the same clientRequestId exists, returns the existing payment.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var payment = await _service.CreatePaymentAsync(request);
            var response = PaymentResponse.FromModel(payment);
            return Ok(response);
        }
        catch (ArgumentException aex)
        {
            _logger.LogWarning(aex, "Validation failed for create payment");
            return BadRequest(new { error = aex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating payment");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
