using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Spectre.Console;

namespace WeatherCLI
{
    internal class Program
    {
        private static HttpClient _client { get; set; } = new();
        private const string _keyFile = "openweathermap.apikey";
        private static string _apiKey { get; set; }

        static async Task Main(string[] args)
        {
            if (!File.Exists(_keyFile))
            {
                AnsiConsole.WriteLine($"Cannot find \"{_keyFile}\", so aborting.");
                return;
            }

            AnsiConsole.WriteLine($"Reading API key from \"{_keyFile}\"");
            _apiKey = File.ReadAllText(_keyFile);
            AnsiConsole.WriteLine("API key retrieved");

            const string lat = "35.1815";
            const string lon = "136.9066";

            var result = await _client.GetStringAsync($"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&appid={_apiKey}");

            AnsiConsole.WriteLine("JSON data retrieved:");
            AnsiConsole.WriteLine(result);
        }
    }
}