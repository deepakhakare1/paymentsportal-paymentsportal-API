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

    /// <summary>
    /// Get all payments
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _service.GetAllAsync();
        var dto = payments.Select(PaymentResponse.FromModel);
        return Ok(dto);
    }

    /// <summary>
    /// Update payment (amount, currency).
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreatePaymentRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(PaymentResponse.FromModel(updated));
        }
        catch (ArgumentException aex)
        {
            return BadRequest(new { error = aex.Message });
        }
    }

    /// <summary>
    /// Delete payment
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    // <summary>
    /// Get id specific payments
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var payments = await _service.GetByIdAsync(id);
        var dto = payments;
        return Ok(dto);
    }
}
