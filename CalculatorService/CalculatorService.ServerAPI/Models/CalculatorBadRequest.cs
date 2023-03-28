﻿using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CalculatorService.ServerAPI.Models
{
	public class CalculatorBadRequest
	{
		public string? ErrorCode { get; set; }
		public int? ErrorStatus { get; set; }
		public string? ErrorMessage { get; set; }

		public CalculatorBadRequest(ModelStateDictionary modelState)
		{
			ErrorCode = "InvalidInput";
			ErrorStatus = 400;

			var modelStateValues = modelState.Values;
			var message = "";
			foreach (ModelStateEntry values in modelStateValues)
			{
				foreach (ModelError error in values.Errors)
				{
					if (!string.IsNullOrEmpty(error.ErrorMessage))
					{
						message += error.ErrorMessage + " ";
					}
				}
			}
				ErrorMessage = "Unable to process request: "+message.Trim();
		}

	}
}
