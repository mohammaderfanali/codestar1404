using System.Text.Json.Serialization;

namespace project.Plugins.Pluginmodels;

public class DatabaseRederModel
{
    [JsonPropertyName("Host")]
    public required string Host { get; set; }

    [JsonPropertyName("Port")]
    public int Port { get; set; } = 5432;

    [JsonPropertyName("Username")]
    public required string Username { get; set; }

    [JsonPropertyName("Password")]
    public required string Password { get; set; }

    [JsonPropertyName("Database")]
    public required string Database { get; set; }

    [JsonPropertyName("UseSSL")]
    public bool UseSsl { get; set; } = false;
    
    [JsonPropertyName("Tablename")] 
    public required string Tablename { get; set; }
}