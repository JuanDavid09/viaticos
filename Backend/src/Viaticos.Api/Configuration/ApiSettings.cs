namespace Viaticos.Api.Configuration;

public class CorsSettings
{
    public const string SectionName = "Cors";
    public const string PolicyName = "ViaticosCors";

    public string[] AllowedOrigins { get; set; } = [];
}

public class ApiSettings
{
    public const string SectionName = "Api";

    public bool UseHttpsRedirection { get; set; } = true;
}
