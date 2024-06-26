﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel_Website_System_API.Models;
using Travel_Website_System_API_.UnitWork;
using Microsoft.Extensions.Options;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Travel_Website_System_API_.DTO.PaymentClasses;

namespace Travel_Website_System_API_.Controllers
{
    // for payment i installed (PayPalCheckoutSdk) library v 1.4
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        UnitOFWork unitOFWork;
        private readonly PayPalHttpClient _client;// from paypal extension

        public PaymentController(UnitOFWork unitOFWork, IOptions<PayPalSettings> payPalSettings)
        {
            this.unitOFWork = unitOFWork;
            // Initialize PayPal HTTP client using business account credentials
            var environment = new SandboxEnvironment(payPalSettings.Value.ClientId, payPalSettings.Value.ClientSecret);
            _client = new PayPalHttpClient(environment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] PaymentRequest paymentRequest)
        {
            // Create an order using business account credentials

            // Construct order request
            var orderRequest = new OrdersCreateRequest();
            orderRequest.Prefer("return=representation");
            orderRequest.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest// payment request
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = paymentRequest.Currency,  // Use currency from request
                    Value = paymentRequest.Amount.ToString()
                }
            }
        },
                ApplicationContext = new ApplicationContext
                {
                    // Replace with your actual return and cancel URLs
                    ReturnUrl = "http://localhost:5200/api/Payment/capture",// approval url
                    CancelUrl = "http://localhost:5200/api/Payment/cancel"
                }
            });

            try
            {
                // Execute order creation request using business client
                var response = await _client.Execute(orderRequest);
                var result = response.Result<Order>();

                if (result.Status == "CREATED")
                {
                    var approvalLink = result.Links?.Find(link => link.Rel == "approve")?.Href;
                    var cancelLink = result.Links?.Find(link => link.Rel == "self")?.Href;

                    if (approvalLink != null)
                    {
                        // Save initial payment details to the database
                        var payment = new Payment
                        {
                            Amount = paymentRequest.Amount,
                            PaymentDate = DateTime.Now,
                            Method = "PayPal",
                            PaymentStatus = "Created",
                            PayPalOrderId = result.Id,
                            BookingPackageId = paymentRequest.BookingPackageId
                        };
                        unitOFWork.PaymentRepo.Add(payment);
                        unitOFWork.save();
                        // Return approval URL and cancel URL to client
                        return Ok(new { success = true, approvalUrl = approvalLink, cancelUrl = cancelLink });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Approval URL not found." });
                    }
                }
                else
                {
                    return BadRequest(new { success = false, message = $"Order creation failed. Status: {result.Status}" });
                }
            }
            catch (HttpException httpException)
            {
                // Log and return error message
                Console.WriteLine($"HttpException: {httpException.Message}");
                Console.WriteLine($"Status Code: {httpException.StatusCode}");
                return BadRequest(new { success = false, message = $"Payment failed: {httpException.Message}" });
            }
            catch (Exception ex)
            {
                // Log and return generic error message
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred. Please try again later." });
         
            }
        }
        // this will be done after approval
        // this will be sent to paypall api
        [HttpGet("capture")]
        public async Task<IActionResult> CaptureOrder([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { success = false, message = "Token is required." });
            }

            var captureRequest = new OrdersCaptureRequest(token);
            captureRequest.RequestBody(new OrderActionRequest());

            try
            {
                var captureResponse = await _client.Execute(captureRequest);
                var captureResult = captureResponse.Result<Order>();

                if (captureResult.Status == "COMPLETED")
                {
                    // Update the payment record in the database
                    var payment = unitOFWork.PaymentRepo.GetAll().FirstOrDefault(p=>p.PayPalOrderId == captureResult.Id);
                    if (payment != null)
                    {
                        payment.PaymentStatus = "Completed";
                        payment.PaymentDate = DateTime.Now;

                        unitOFWork.PaymentRepo.Update(payment);
                        unitOFWork.save();
                    }

                    return Ok(new { success = true, orderId = captureResult.Id });
                }
                else
                {
                    return BadRequest(new { success = false, message = $"Payment not completed. Status: {captureResult.Status}" });
                }
            }
            catch (HttpException httpException)
            {
                Console.WriteLine($"HttpException: {httpException.Message}");
                return BadRequest(new { success = false, message = $"Payment failed: {httpException.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }
        [HttpGet("cancel")]
        public IActionResult CancelPayment([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { success = false, message = "Token is required." });
            }

            try
            {
                // Retrieve the payment record based on the PayPal order ID (token)
                var payment = unitOFWork.PaymentRepo.GetAll().FirstOrDefault(p => p.PayPalOrderId == token);
                if (payment != null)
                {
                    // Update the payment status to "Cancelled"
                    payment.PaymentStatus = "Cancelled";
                    payment.PaymentDate = DateTime.Now;

                    unitOFWork.PaymentRepo.Update(payment);
                    unitOFWork.save();
                }
                else
                {
                    return NotFound(new { success = false, message = "Payment not found." });
                }

                return Ok(new { success = true, message = "Payment cancelled successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred. Please try again later." });
            }
        }

    }
}
            

