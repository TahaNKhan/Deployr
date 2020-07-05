
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;

namespace Deployr.Web.SwaggerHelpers
{
	public class SwaggerFileOperationFilter : IOperationFilter
	{
		private static readonly Type TypeOfIFormFile = typeof(IFormFile);

		private const string FormDataMimeType = "multipart/form-data";
		private const string ParameterSourceFormData = "formFile";
		private const string ParameterTypeFile = "file";
		private const string ParameterTypeFormat = "binary";

		/// <inheritdoc />
		public virtual void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			//if (operation.Parameters == null || operation.Parameters.Count == 0) return;

			//// get all parameters that use the IFormFile type
			//var formFileParameters = context
			//	.ApiDescription
			//	.ActionDescriptor
			//	.Parameters
			//	.Where(_ => _.ParameterType == TypeOfIFormFile)
			//	.ToDictionary(_ => _.Name, _ => _);

			//if (formFileParameters.Count == 0) return;

			//var operationParameters = operation.Parameters.ToDictionary(_ => _.Name, _ => _);

			//foreach (var formFileParameter in formFileParameters.Values)
			//{
			//	// remove the incorrect parameter specification
			//	if (operationParameters.TryGetValue(formFileParameter.Name, out var parameterToRemove))
			//	{
			//		operation.Parameters.Remove(parameterToRemove);
			//	}

			//	var uploadFileMediaType = new OpenApiMediaType()
			//	{
			//		Schema = new OpenApiSchema()
			//		{
			//			Type = "object",
			//			Properties =
			//			{
			//				[ParameterSourceFormData] = new OpenApiSchema()
			//				{
			//					Type = ParameterTypeFile,
			//					Format = ParameterTypeFormat
			//				}
			//			},
			//			Required = new HashSet<string>()
			//			{
			//				ParameterSourceFormData
			//			}
			//		}
			//	};
			//	operation.RequestBody = new OpenApiRequestBody
			//	{
			//		Content = new Dictionary<string, OpenApiMediaType>
			//		{
			//			[FormDataMimeType] = uploadFileMediaType
			//		}
			//	};
			//}
		}
	}
}
