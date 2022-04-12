using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weather.Models;
using Weather.Services;

namespace Weather.Consoles
{
    class Program
    {
        #region used by the Console
        Views.ConsolePage theConsole;
        StringBuilder theConsoleString;
        public Program(Views.ConsolePage myConsole)
        {
            theConsole = myConsole;
            theConsoleString = new StringBuilder();
        }
        #endregion

        #region Console Demo program
        public async Task myMain()
        {
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += ReportWeatherDataAvailable;

 
            List<Forecast> forecasts = new List<Forecast>();
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;
                forecasts.Add(await service.GetForecastAsync(latitude, longitude));
                theConsole.WriteLine(forecasts[0].City);

            }
            catch (Exception ex)
            {
                exception = ex;
                theConsole.WriteLine(ex.Message);
            }
            
            foreach (var forecast in forecasts)
            {
                theConsoleString.AppendLine(".....");
                if (forecast != null)
                {
                    theConsoleString.AppendLine($"Weather forecast for {forecast.City}");
                    var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                    foreach (var group in GroupedList)
                    {
                        theConsole.WriteLine(group.Key.Date.ToShortDateString());
                        foreach (var item in group)
                        {

                            theConsole.WriteLine($"   - {item.DateTime.ToShortTimeString()}: {item.Description}, temperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s");
                        }
                    }
                }
                else
                {
                    theConsoleString.AppendLine($"Geolocation weather service error.");
                    theConsoleString.AppendLine($"Error: {exception.Message}");
                }
            }
            void ReportWeatherDataAvailable(object sender, string message)
            {
                theConsole.WriteLine($"Event message from weather service: {message}");
            }
            #endregion
        }
    }
}
