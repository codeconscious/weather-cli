﻿using System;
using System.IO;
using System.Linq;
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

        var forecast = GetForecast();

        PrintCurrent(forecast);
        PrintDailyForecast(forecast);
    }

    private static Forecast.Root GetForecast()
    {
        return AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Arc)
            .SpinnerStyle(Style.Parse("green bold"))
            .Start<Forecast.Root>("Getting weather data...", ctx =>
            {
                ctx.Status($"Reading API key from \"{_keyFile}\"...");
                var apiKey = File.ReadAllText(_keyFile);
                AnsiConsole.WriteLine($"API key retrieved from \"{_keyFile}\"");

                ctx.Status("Contacting the weather service...");
                const string lat = "35.1815";
                const string lon = "136.9066";
                const string lang = "en";
                var result = _client.GetStringAsync(
                    "https://api.openweathermap.org/data/2.5/onecall?" +
                    $"lat={lat}&lon={lon}&units={_units}&lang={lang}&appid={apiKey}").Result;

                if (string.IsNullOrWhiteSpace(result))
                {
                    AnsiConsole.WriteLine("No data was received. Aborting.");
                    return null;
                }

                AnsiConsole.WriteLine("Response received");

                ctx.Status("Parsing the data...");
                var parsedForecast = JsonSerializer.Deserialize<Forecast.Root>(result);
                AnsiConsole.WriteLine("Data parsed OK");

                return parsedForecast;
            });
    }

    private static void PrintCurrent(Forecast.Root forecast)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        var table = new Table
        {
            Border = TableBorder.None
        };
        table.AddColumn("Info");
        table.HideHeaders();
        table.AddRow($"Weather for {forecast.Lat} @ {forecast.Lon}");
        table.AddRow($"Temperature is {forecast.Current.Temp} degrees, feeling like {forecast.Current.FeelsLike}");
        table.AddRow($"Humidity is {forecast.Current.Humidity}%");

        if (forecast.Alerts?.Any() == true)
        {
            foreach (var alert in forecast.Alerts)
                table.AddRow($"ALERT: {alert}");
        }

        var panel = new Panel(table)
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("Current conditions", Justify.Left)
        };
        AnsiConsole.Write(panel);
    }

    private static void PrintDailyForecast(Forecast.Root forecast)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        var table = new Table();
        table.AddColumn("Date");
        table.AddColumn("Temp");
        table.AddColumn("Humidity");
        table.AddColumn("Rain");
        table.AddColumn("Wind");
        table.AddColumn("Sun");

        foreach (var d in forecast.Daily)
        {
            var dateTime = DateTime.UnixEpoch.AddSeconds(d.Dt).ToLocalTime().ToString("ddd MMM d");
            var temp = d.Temp.Min.ToString("0") + " / " + d.Temp.Max.ToString("0");
            var humidity = d.Humidity + "%";
            var rain = d.Rain is null ? "--" : d.Rain.Value.ToString("0") + "%";
            var wind = $"{d.WindSpeed:0} (up to {d.WindGust:0})";
            var sunrise = $"{DateTime.UnixEpoch.AddSeconds(d.Sunrise).ToLocalTime():HH:mm} / {DateTime.UnixEpoch.AddSeconds(d.Sunset).ToLocalTime():HH:mm}";

            table.AddRow(dateTime, temp, humidity, rain, wind, sunrise);
        }

        AnsiConsole.Write(table);
    }
}