using System;

namespace WeatherCLI;

public sealed record class Options
{
    public string Latitude { get; init; }

    public string Longitude { get; init; }

    public string Language { get; init; } = "en";

    public Options(string[] args)
    {
        if (args is null || args.Length < 2 || args.Length > 3)
            throw new ArgumentException("There are an invalid count of arguments.");

        string latitude = args[0];
        string longitude = args[1];

        Latitude = IsStringNumericWithinRange(latitude, -90, 90)
            ? latitude
            : throw new ArgumentException($"An invalid latitude ({latitude}) was provided.");

        Longitude = IsStringNumericWithinRange(longitude, -180, 180)
            ? longitude
            : throw new ArgumentException($"An invalid longitude ({longitude}) was provided.");

        if (args.Length != 3)
            return;

        string language = args[2];
        Language = string.IsNullOrWhiteSpace(language) || language.Length > 5
            ? throw new ArgumentException($"An invalid language ({language}) was provided.")
            : language;

        static bool IsStringNumericWithinRange(string text, float minAllowed, float maxAllowed)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   float.TryParse(text, out var textAsFloat) &&
                   textAsFloat >= minAllowed &&
                   textAsFloat <= maxAllowed;
        }
    }
}
