using System.Text.Json;
using WebAppTemplate.Shared.Http;

namespace WebAppTemplate.Frontend;

public static class Constants
{
    public static JsonSerializerOptions SerializerOptions
    {
        get
        {
            if (InternalOptions != null)
                return InternalOptions;
            
            InternalOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };
                
            // Add source generated options from shared project
            InternalOptions.TypeInfoResolverChain.Add(SerializationContext.Default);

            return InternalOptions;
        }
    }

    private static JsonSerializerOptions? InternalOptions;
}