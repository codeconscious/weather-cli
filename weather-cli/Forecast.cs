using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WeatherCLI;

public sealed record class Forecast
{
    public sealed record class Root(
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lon")] double Lon,
        [property: JsonPropertyName("timezone")] string Timezone,
        [property: JsonPropertyName("timezone_offset")] int TimezoneOffset,
        [property: JsonPropertyName("current")] Current Current,
        [property: JsonPropertyName("minutely")] IReadOnlyList<Minutely> Minutely,
        [property: JsonPropertyName("hourly")] IReadOnlyList<Hourly> Hourly,
        [property: JsonPropertyName("daily")] IReadOnlyList<Daily> Daily,
        [property: JsonPropertyName("alerts")] IReadOnlyList<Alert> Alerts
    );

    public sealed record class Weather(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("main")] string Main,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("icon")] string Icon
    );

    public sealed record class Current(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("sunrise")] int Sunrise,
        [property: JsonPropertyName("sunset")] int Sunset,
        [property: JsonPropertyName("temp")] double Temp,
        [property: JsonPropertyName("feels_like")] double FeelsLike,
        [property: JsonPropertyName("pressure")] int Pressure,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("dew_point")] double DewPoint,
        [property: JsonPropertyName("uvi")] double Uvi,
        [property: JsonPropertyName("clouds")] int Clouds,
        [property: JsonPropertyName("visibility")] int Visibility,
        [property: JsonPropertyName("wind_speed")] double WindSpeed,
        [property: JsonPropertyName("wind_deg")] int WindDeg,
        [property: JsonPropertyName("wind_gust")] double WindGust,
        [property: JsonPropertyName("weather")] IReadOnlyList<Weather> Weather
    );

    public sealed record class Minutely(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("precipitation")] double Precipitation
    );

    public sealed record class Hourly(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("temp")] double Temp,
        [property: JsonPropertyName("feels_like")] double FeelsLike,
        [property: JsonPropertyName("pressure")] int Pressure,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("dew_point")] double DewPoint,
        [property: JsonPropertyName("uvi")] double Uvi,
        [property: JsonPropertyName("clouds")] int Clouds,
        [property: JsonPropertyName("visibility")] int Visibility,
        [property: JsonPropertyName("wind_speed")] double WindSpeed,
        [property: JsonPropertyName("wind_deg")] int WindDeg,
        [property: JsonPropertyName("wind_gust")] double WindGust,
        [property: JsonPropertyName("weather")] IReadOnlyList<Weather> Weather,
        /// <summary>
        /// Probability of precipitation
        /// </summary>
        [property: JsonPropertyName("pop")] double Pop
    );

    public sealed record class Temp(
        [property: JsonPropertyName("day")] double Day,
        [property: JsonPropertyName("min")] double Min,
        [property: JsonPropertyName("max")] double Max,
        [property: JsonPropertyName("night")] double Night,
        [property: JsonPropertyName("eve")] double Eve,
        [property: JsonPropertyName("morn")] double Morn
    );

    public sealed record class FeelsLike(
        [property: JsonPropertyName("day")] double Day,
        [property: JsonPropertyName("night")] double Night,
        [property: JsonPropertyName("eve")] double Eve,
        [property: JsonPropertyName("morn")] double Morn
    );

    public sealed record class Daily(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("sunrise")] int Sunrise,
        [property: JsonPropertyName("sunset")] int Sunset,
        [property: JsonPropertyName("moonrise")] int Moonrise,
        [property: JsonPropertyName("moonset")] int Moonset,
        [property: JsonPropertyName("moon_phase")] double MoonPhase,
        [property: JsonPropertyName("temp")] Temp Temp,
        [property: JsonPropertyName("feels_like")] FeelsLike FeelsLike,
        [property: JsonPropertyName("pressure")] int Pressure,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("dew_point")] double DewPoint,
        [property: JsonPropertyName("wind_speed")] double WindSpeed,
        [property: JsonPropertyName("wind_deg")] int WindDeg,
        [property: JsonPropertyName("wind_gust")] double WindGust,
        [property: JsonPropertyName("weather")] IReadOnlyList<Weather> Weather,
        [property: JsonPropertyName("clouds")] int Clouds,
        [property: JsonPropertyName("pop")] double Pop,
        [property: JsonPropertyName("uvi")] double Uvi,
        [property: JsonPropertyName("rain")] double? Rain
    );

    public sealed record class Alert(
        [property: JsonPropertyName("sender_name")] string SenderName,
        [property: JsonPropertyName("event")] string Event,
        [property: JsonPropertyName("start")] int Start,
        [property: JsonPropertyName("end")] int End,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("tags")] IReadOnlyList<string> Tags
    );
}
