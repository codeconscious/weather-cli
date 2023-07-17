using System;

namespace WeatherCLI;

public sealed record class Options
{
    public string Latitude { get; init; }

    public string Longitude { get; init; }

    public string Language { get; init; } = "en";

    public Options(string latitude, string longitude)
    {
        this.Latitude = IsStringNumericWithinRange(latitude, -90, 90)
            ? latitude
            : throw new ArgumentException($"An invalid latitude ({latitude}) was provided.");

        this.Longitude = IsStringNumericWithinRange(longitude, -180, 180)
            ? longitude
            : throw new ArgumentException($"An invalid longitude ({longitude}) was provided.");

        static bool IsStringNumericWithinRange(string text, float minAllowed, float maxAllowed)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   float.TryParse(text, out var textAsFloat) &&
                   textAsFloat >= minAllowed &&
                   textAsFloat <= maxAllowed;
        }
    }
}
