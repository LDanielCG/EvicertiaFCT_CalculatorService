﻿using CalculatorService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace CalculatorService.ServerAPI.Controllers
{
	// ipaddress/Calculator
	[Route("[controller]")]
	[ApiController]
	public class CalculatorController : ControllerBase
	{
		string trackingId = "";
		string operation = "";
		string calculation = "";
		const string trackingIdName = "X-Evi-Tracking-Id";

		// ipaddress/Calculator/add
		[HttpPost("add")]
		public ActionResult<AdditionResponse> Add(AdditionRequest request)
		{
			if (request.IsValid())
			{
				var result = request.Calculate();
				trackingId = HttpContext.Request.Headers[trackingIdName];
				operation = "add";
				calculation = String.Join('+', request.Addends.ToArray()) + "=" + result;
				// save to journal
				Journal.AddRecord(trackingId, operation, calculation);
				// Return result to client
				return AdditionResponse.FromAddition(result);
			}
			else
			{
				var errorKey = "addends";
				var errorMessage = "Invalid input parameters. Make sure you entered at least two numbers and they don't have more than nine digits.";
				return BadRequest(ThrowBadRequest400(errorKey, errorMessage));
			}

			// Change to something like this whenever is ready:
			/*
			 * if (request.notNull())
			 * {
			 *		if(request.IsValid())
			 *		{
			 *			var result = request.Calculate();
			 *			return AdditionResponse.FromAddition(result);
			 *		}
			 *		else
			 *		{
			 *			var modelState = new ModelStateDictionary();
			 *			var errorKey = "addends";
			 *			var errorMessage = "Invalid input parameters. Make sure you entered at least two numbers and they don't have more than nine digits.";
			 *			modelState.AddModelError(errorKey, errorMessage);
			 *
			 *			var badRequest = new CalculatorBadRequest(modelState);
			 *			return BadRequest(badRequest);
			 *		}
			 * }
			 * else
			 * {
			 *		// Error 500
			 * }
			 */


		}
		// ipaddress/Calculator/sub
		[HttpPost("sub")]
		public ActionResult<SubtractionResponse> Subtract(SubtractionRequest request)
		{
			if (request.IsValid())
			{
				var result = request.Calculate();
				trackingId = HttpContext.Request.Headers[trackingIdName];
				operation = "sub";
				calculation = request.Minuend + "-" + request.Subtrahend + "=" + result;
				// save to journal
				Journal.AddRecord(trackingId, operation, calculation);
				// Return result to client
				return SubtractionResponse.FromSubtraction(result);
			}
			else
			{
				var errorKey = "subtraction";
				var errorMessage = "Invalid input parameters. Make sure you entered only numbers and not other characters.";
				return BadRequest(ThrowBadRequest400(errorKey, errorMessage));
			}
		}
		// ipaddress/Calculator/mult
		[HttpPost("mult")]
		public ActionResult<MultiplicationResponse> Multiply(MultiplicationRequest request)
		{
			if (request.IsValid())
			{
				var result = request.Calculate();
				trackingId = HttpContext.Request.Headers[trackingIdName];
				operation = "mult";
				calculation = String.Join('*', request.Factors.ToArray()) + "=" + result;
				// Save to journal
				Journal.AddRecord(trackingId, operation, calculation);
				// Return result to client
				return MultiplicationResponse.FromMultiplication(result);
			}
			else
			{
				var errorKey = "multiplication";
				var errorMessage = "Invalid input parameters. Make sure you entered at least two numbers and they don't have more than nine digits.";
				return BadRequest(ThrowBadRequest400(errorKey, errorMessage));
			}
		}
		// ipaddress/Calculator/div
		[HttpPost("div")]
		public ActionResult<DivisionResponse> Division(DivisionRequest request)
		{
			if(request.IsValid())
			{
				var result = request.Calculate();
				trackingId = HttpContext.Request.Headers[trackingIdName];
				operation = "div";
				calculation = request.Dividend + "/" + request.Divisor + "=" + " { Quotient:" + result.ToArray()[0] + ", Remainder: " + result.ToArray()[1] + " }";
				// Save to journal
				Journal.AddRecord(trackingId, operation, calculation);
				// Return result to client
				return DivisionResponse.FromDivision(result.First(), result.Last());
			}
			else
			{
				var errorKey = "division";
				var errorMessage = "Invalid input parameters. Make sure you didn't enter a zero as the divisor!.";
				return BadRequest(ThrowBadRequest400(errorKey, errorMessage));
			}
		}
		// ipaddress/Calculator/sqrt
		[HttpPost("sqrt")]
		public ActionResult<SquareRootResponse> SquareRoot(SquareRootRequest request)
		{
			if(request.IsValid()) 
			{
				var result = request.Calculate();
				trackingId = HttpContext.Request.Headers[trackingIdName];
				operation = "sqrt";
				calculation = "√" + request.Number + "=" + result;
				// Save to journal
				Journal.AddRecord(trackingId, operation, calculation);
				// Return result to client
				return SquareRootResponse.FromSquareRoot(result);
			}
			else
			{
				var errorKey = "squareRoot";
				var errorMessage = "Invalid input parameters. Make sure you entered only numbers and not other characters.";
				return BadRequest(ThrowBadRequest400(errorKey, errorMessage));
			}
		}
		// ipaddress/Calculator/journal/query
		[HttpPost("journal/query")]
		public ActionResult<JournalResponse> JournalQuery(JournalRequest request)
		{
			if (request.HasTrackingId())
			{
				var records = Journal.GetRecordsForTrackingId(request.Id);
				return JournalResponse.FromJournal(records);
			}

			return null;
		}
		public static CalculatorBadRequest ThrowBadRequest400(string errorKey, string errorMessage)
		{
			var modelState = new ModelStateDictionary();
			modelState.AddModelError(errorKey, errorMessage);

			return new CalculatorBadRequest(modelState);
		}
	}
}
