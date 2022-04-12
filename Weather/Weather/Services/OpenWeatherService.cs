using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Weather.Models;

namespace Weather.Services
{
    public delegate void WeatherForecastAvailableHandler(object sender, string message);
    public class OpenWeatherService
    {
        ConcurrentDictionary<(string, string), Forecast> _forecastCacheCity = new ConcurrentDictionary<(string, string), Forecast>();
        ConcurrentDictionary<(double, double, string), Forecast> _forecastCacheLongLat = new ConcurrentDictionary<(double, double, string), Forecast>();
        public event WeatherForecastAvailableHandler WeatherForecastAvailable;
        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "01e1d7002da561ca5aca0dac28fbae18"; 

        public async Task<Forecast> GetForecastAsync(string City)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            var cities = City;
            var date = DateTime.Now.ToString("yyyy-MM-dd: HH:mm");
            var key = (cities, date);

            if (!_forecastCacheCity.TryGetValue(key, out var forecast))
            {

                forecast = await ReadWebApiAsync(uri);
                _forecastCacheCity[key] = forecast;
                WeatherForecastAvailable?.Invoke(forecast, $"New weather forecast for {City} avalible");

            }
            else

                WeatherForecastAvailable?.Invoke(forecast, $"Cached weather forecast for {City} avalible");
            return forecast;
        }

        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            var longit = longitude;
            var latit = latitude;
            var date = DateTime.Now.ToString("yyyy-MM-dd: HH:mm");
            var key = (longit, latit, date);

            if (!_forecastCacheLongLat.TryGetValue(key, out var forecast))
            {

                forecast = await ReadWebApiAsync(uri);
                _forecastCacheLongLat[key] = forecast;
                WeatherForecastAvailable.Invoke(forecast, $"New weather forecast for ({latitude},{longitude}) avalible");
            }

            else
                WeatherForecastAvailable.Invoke(forecast, $"Cached weather forecast for ({latitude},{longitude}) avalible");

            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            var forecast = new Forecast
            {

                City = wd.city.name,
                Items = wd.list.Select(x => new ForecastItem
                {

                    Temperature = x.main.temp,
                    WindSpeed = x.wind.speed,
                    Description = x.weather[0].description,
                    Icon = x.weather[0].icon,
                    DateTime = UnixTimeStampToDateTime(x.dt)
                }).ToList()
            };
            return forecast;
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}