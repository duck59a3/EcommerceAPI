using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyWebApi.Helpers
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFileParameter = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile) ||
                     p.ParameterType == typeof(IFormFileCollection) ||
                     p.ParameterType == typeof(IEnumerable<IFormFile>) ||
                     p.ParameterType == typeof(List<IFormFile>));

            if (!hasFileParameter) return;

            // Clear existing parameters
            operation.Parameters.Clear();

            // Set request body for multipart/form-data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>()
                        }
                    }
                }
            };

            var formDataSchema = operation.RequestBody.Content["multipart/form-data"].Schema;

            // Add file parameters
            foreach (var parameter in context.MethodInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(IFormFile))
                {
                    formDataSchema.Properties[parameter.Name] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    };
                }
                else if (parameter.ParameterType == typeof(IFormFileCollection) ||
                         parameter.ParameterType == typeof(IEnumerable<IFormFile>) ||
                         parameter.ParameterType == typeof(List<IFormFile>))
                {
                    formDataSchema.Properties[parameter.Name] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    };
                }
                else if (parameter.ParameterType == typeof(string))
                {
                    // Add string parameters (like productId)
                    formDataSchema.Properties[parameter.Name] = new OpenApiSchema
                    {
                        Type = "string"
                    };
                }
            }
        }
    }
}
