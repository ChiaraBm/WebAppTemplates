namespace WebAppTemplate.Api.Configuration;

public class OidcOptions
{
    public string Authority { get; set; }
    public bool RequireHttpsMetadata { get; set; } = true;
    public string ResponseType { get; set; } = "code";
    public string[]? Scopes { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}