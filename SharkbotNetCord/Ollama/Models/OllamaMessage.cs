using System.Text.Json.Serialization;

namespace SharkbotNetCord.Ollama.Models
{
    public class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
