using Serilog.Events;
using Serilog.Formatting;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace DF_PA_API.Models
{
    public class CustomJsonFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            logEvent.Properties.TryGetValue("Method", out LogEventPropertyValue methodNameVal);
            logEvent.Properties.TryGetValue("Class", out LogEventPropertyValue classnameVal);          
            
            var logObject = new
            {
                Timestamp = logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                Level = logEvent.Level.ToString(),
                Class = classnameVal?.ToString()?.Trim('"'),
                Method = methodNameVal?.ToString()?.Trim('"'),
                Message = logEvent.MessageTemplate.Text.Contains("{Path}")
                ? logEvent.MessageTemplate.Text.Replace("{Path}", GetPathValue(logEvent))
                : Regex.Replace(logEvent.RenderMessage(), "\"([A-Za-z0-9_]+)\"", "$1"),
                Exception = logEvent.Exception?.ToString(),
            };

            var options = new JsonSerializerOptions
            {                
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(logObject, options);
            output.WriteLine(json);
            output.WriteLine();
        }
        private string GetPathValue(LogEvent logEvent)
        {
            return logEvent.Properties.TryGetValue("Path", out var value)
                ? value.ToString().Trim('"') // Removes the quotes from the string
                : string.Empty;
        }

    }
}
