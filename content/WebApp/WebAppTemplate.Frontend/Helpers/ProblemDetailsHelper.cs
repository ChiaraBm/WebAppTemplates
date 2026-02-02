using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using WebAppTemplate.Shared.Http.Responses;

namespace WebAppTemplate.Frontend.Helpers;

public static class ProblemDetailsHelper
{
    public static async Task HandleProblemDetailsAsync(HttpResponseMessage response, object model, ValidationMessageStore validationMessageStore)
    {
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        if (problemDetails == null)
            response.EnsureSuccessStatusCode(); // Trigger exception when unable to parse
        else
        {
            if(!string.IsNullOrEmpty(problemDetails.Detail))
                validationMessageStore.Add(new FieldIdentifier(model, string.Empty), problemDetails.Detail);

            if (problemDetails.Errors != null)
            {
                foreach (var error in problemDetails.Errors)
                {
                    foreach (var message in error.Value)
                        validationMessageStore.Add(new FieldIdentifier(model, error.Key), message);
                }
            }
        }
    }
}