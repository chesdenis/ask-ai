namespace AITextWriter.Model;

public class ApiRequestSettings
{
    public string ApiKey { get; set; }
    public string Model { get; set; }
    public string Endpoint { get; set; }
    public double TimeoutMinutes { get; set; }
}