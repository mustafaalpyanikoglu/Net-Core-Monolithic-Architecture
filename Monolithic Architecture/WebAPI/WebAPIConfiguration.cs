namespace WebAPI;

public class WebAPIConfiguration
{
    public string ApiDomain { get; set; }
    public string[] AllowedOrigins { get; set; }

    public WebAPIConfiguration()
    {
        ApiDomain = string.Empty;
        AllowedOrigins = Array.Empty<string>();
    }

    public WebAPIConfiguration(string apiDomain, string[] allowedOrigins)
    {
        ApiDomain = apiDomain;
        AllowedOrigins = allowedOrigins;
    }
}