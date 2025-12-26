using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Invoicing.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OrderProcessing.Invoicing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private static readonly ConcurrentDictionary<Guid, Invoice> _invoices = new();
        private static int _invoiceCounter = 1000;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generate invoice from order placed event
        /// </summary>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status201Created)]
        public ActionResult<Invoice> GenerateInvoice([FromBody] OrderPlacedEvent orderEvent)
        {
            try
            {
                var invoiceId = Guid.NewGuid();
                var invoiceNumber = $"INV-{DateTime.Now.Year}-{Interlocked.Increment(ref _invoiceCounter):D6}";

                var invoice = new Invoice
                {
                    InvoiceId = invoiceId,
                    OrderId = orderEvent.OrderId,
                    InvoiceNumber = invoiceNumber,
                    CustomerName = orderEvent.CustomerName,
                    CustomerEmail = orderEvent.CustomerEmail,
                    LineItems = orderEvent.OrderLines.Select(ol => new InvoiceLineItem
                    {
                        ProductCode = ol.ProductCode,
                        Quantity = ol.Quantity,
                        UnitPrice = ol.Price,
                        LineTotal = ol.LineTotal
                    }).ToList(),
                    Subtotal = orderEvent.TotalAmount,
                    Tax = orderEvent.TotalAmount * 0.19m, // 19% TVA
                    Total = orderEvent.TotalAmount * 1.19m,
                    GeneratedAt = DateTime.Now,
                    Status = InvoiceStatus.Generated
                };

                _invoices.TryAdd(invoiceId, invoice);

                _logger.LogInformation($"Invoice {invoiceNumber} generated for Order {orderEvent.OrderId}");

                return CreatedAtAction(nameof(GetInvoice), new { id = invoiceId }, invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get invoice by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Invoice> GetInvoice(Guid id)
        {
            if (_invoices.TryGetValue(id, out var invoice))
            {
                return Ok(invoice);
            }
            return NotFound(new { error = $"Invoice {id} not found" });
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Invoice>), StatusCodes.Status200OK)]
        public ActionResult<List<Invoice>> GetAllInvoices()
        {
            return Ok(_invoices.Values.OrderByDescending(i => i.GeneratedAt).ToList());
        }

        /// <summary>
        /// Get invoices by Order ID
        /// </summary>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(List<Invoice>), StatusCodes.Status200OK)]
        public ActionResult<List<Invoice>> GetInvoicesByOrderId(Guid orderId)
        {
            var invoices = _invoices.Values
                .Where(i => i.OrderId == orderId)
                .OrderByDescending(i => i.GeneratedAt)
                .ToList();

            return Ok(invoices);
        }

        /// <summary>
        /// Update invoice status
        /// </summary>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Invoice> UpdateInvoiceStatus(Guid id, [FromBody] InvoiceStatus newStatus)
        {
            if (_invoices.TryGetValue(id, out var invoice))
            {
                invoice.Status = newStatus;
                _logger.LogInformation($"Invoice {invoice.InvoiceNumber} status updated to {newStatus}");
                return Ok(invoice);
            }
            return NotFound(new { error = $"Invoice {id} not found" });
        }
    }
}
