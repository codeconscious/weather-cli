using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
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
        PrintHourly(forecast);
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
        table.AddColumn("Humid", t => t.Alignment = Justify.Right);
        table.AddColumn("Rain", t => t.Alignment = Justify.Right);
        table.AddColumn("Wind");
        table.AddColumn("Sun");

        foreach (var d in forecast.Daily)
        {
            var fullMoon = d.MoonPhase == 0.5 ? " 🌕" : string.Empty;
            var dateTime = ConvertIntToLocalDateTime(d.Dt).ToString("ddd MMM d") + fullMoon;
            var temp = d.Temp.Min.ToString("0") + " [gray]/[/] " + d.Temp.Max.ToString("0");
            var humidity = d.Humidity + "%";

            var rainPct = (d.Pop * 100).ToString("0") + "%";
            var rainMm = d.Rain is null ? "--" : d.Rain.Value.ToString("0") + "mm";
            var rain = $"{rainMm} @ {rainPct}";

            var wind = $"{d.WindSpeed:0} (up to {d.WindGust:0})";

            var sunrise = ConvertIntToLocalDateTime(d.Sunrise);
            var sunset = ConvertIntToLocalDateTime(d.Sunset);
            var sunHours = $"{sunrise:HH:mm} – {sunset:HH:mm}" +
                           $" ({(sunset - sunrise).ToString(@"h\:mm")})";

            table.AddRow(dateTime, temp, humidity, rain, wind, sunHours);
        }

        AnsiConsole.Write(table);
    }

    private static void PrintHourly(Forecast.Root forecast)
    {
        ArgumentNullException.ThrowIfNull(forecast);

        var table = new Table();
        table.AddColumn("Date", t => t.Alignment = Justify.Right);
        table.AddColumn("Temp", t => t.Alignment = Justify.Right);
        table.AddColumn("Humid", t => t.Alignment = Justify.Right);
        table.AddColumn("Rain", t => t.Alignment = Justify.Right);
        table.AddColumn("Wind");
        table.AddColumn("Summary");
        table.AddColumn("Cloud", t => t.Alignment = Justify.Right);
        table.AddColumn("Vis.", t => t.Alignment = Justify.Right);
        table.AddColumn("UV");

        foreach (var h in forecast.Hourly.Where(ShouldProcessHourly))
        {
            var dt = ConvertIntToLocalDateTime(h.Dt);
            var formattedDt = dt.Hour == 0
                ? dt.ToString("MMM d @ HH")
                : dt.ToString("HH");
            var temp = h.Temp.ToString("0");
            var humidity = h.Humidity + "%";
            var rain = (h.Pop * 100).ToString("0") + "%";
            var wind = $"{h.WindSpeed:0} [gray]/[/] {h.WindGust:0}";
            var desc = string.Join(Environment.NewLine, h.Weather.Select(w => w.Description));
            var cloudPct = h.Clouds.ToString() + "%";
            var visibility = (h.Visibility / 1000).ToString("0") + "km";
            var uv = h.Uvi.ToString("0");

            table.AddRow(formattedDt, temp, humidity, rain, wind, desc, cloudPct, visibility, uv);
        }

        AnsiConsole.Write(table);

        static bool ShouldProcessHourly(Forecast.Hourly hourly)
        {
            var earliest = DateTime.Now.AddHours(-1);
            var last = earliest.Date.AddDays(1).Add(new TimeSpan(23, 0, 0));

            var dtLocalTime = DateTime.UnixEpoch.AddSeconds(hourly.Dt).ToLocalTime();

            return earliest <= dtLocalTime && last >= dtLocalTime;
        }
    }

    private static DateTime ConvertIntToLocalDateTime(int dtInt) =>
        DateTime.UnixEpoch.AddSeconds(dtInt).ToLocalTime();
}