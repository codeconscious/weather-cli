using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Spectre.Console;

namespace WeatherCLI;

internal static class Program
{
    private static HttpClient _client { get; set; } = new();
    private const string _keyFile = "openweathermap.apikey";
    private const string _units = "metric";

    static async Task Main(string[] args)
    {
        if (!File.Exists(_keyFile))
        {
            AnsiConsole.WriteLine($"Cannot find \"{_keyFile}\", so aborting.");
            return;
        }

        AnsiConsole.WriteLine($"Reading API key from \"{_keyFile}\"");
        var apiKey = File.ReadAllText(_keyFile);
        AnsiConsole.WriteLine("API key retrieved");

        const string lat = "35.1815";
        const string lon = "136.9066";

        var result = await _client.GetStringAsync(
            $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&units={_units}&appid={apiKey}");
        AnsiConsole.WriteLine("Response received.");

        var forecast = JsonSerializer.Deserialize<Forecast.Root>(result);

        if (forecast is null)
        {
            AnsiConsole.WriteLine("No data was received. Aborting.");
            return;
        }

        var table = new Table();
        table.AddColumn("Date");
        table.AddColumn("Temp");
        table.AddColumn("Rain");
        table.AddColumn("Wind");

        AnsiConsole.WriteLine($"Weather for {forecast.Lat}, {forecast.Lon}");
        AnsiConsole.WriteLine($"The current temperature is {forecast.Current.Temp} degrees, but it feels like {forecast.Current.FeelsLike} degrees.");
        foreach (var d in forecast.Daily)
        {
            var dateTime = DateTime.UnixEpoch.AddSeconds(d.Dt).ToShortDateString();
            var temp = d.Temp.Min.ToString("0") + " to " + d.Temp.Max.ToString("0");
            var rain = d.Rain is null ? "--" : d.Rain.Value.ToString("0") + "%";
            var wind = $"{d.WindSpeed:0} (up to {d.WindGust:0})";

            table.AddRow(dateTime, temp, rain, wind);
        }

        AnsiConsole.Write(table);
    }
}