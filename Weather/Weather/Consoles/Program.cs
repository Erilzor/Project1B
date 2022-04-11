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
    //Your can move your Console application Main here. Rename Main to myMain and make it NOT static and async


    class Program
    {

        //Register the event


        #region used by the Console
        Views.ConsolePage theConsole;
        StringBuilder theConsoleString;
        public Program(Views.ConsolePage myConsole)
        {
            //used for the Console
            theConsole = myConsole;
            theConsoleString = new StringBuilder();
        }
        #endregion

        #region Console Demo program
        //This is the method you replace with your async method renamed and NON static Main
        public async Task myMain()
        {
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += ReportWeatherDataAvailable;

            Task<Forecast> t1 = null, t2 = null, t3 = null, t4 = null;
            //List<Forecast> forecasts = new List<Forecast>();
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;
                /*forecasts.Add(await service.GetForecastAsync(latitude, longitude));
                theConsole.WriteLine(forecasts[0].City);*/

                //Create the two tasks and wait for comletion
                await service.GetForecastAsync(latitude, longitude);
                await service.GetForecastAsync("Uppsala");
                t1 = service.GetForecastAsync(latitude, longitude);
                t2 = service.GetForecastAsync("Uppsala");
                Task.WaitAll(t1, t2);



            }
            catch (Exception ex)
            {
                //if exception write the message later
                exception = ex;
                theConsole.WriteLine(ex.Message);
            }

            theConsoleString.AppendLine("-----------------");

            if (t1?.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t1.Result;
                theConsoleString.AppendLine($"Weather forecast for {forecast.City}");
                var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                foreach (var group in GroupedList)
                {
                    theConsoleString.AppendLine(group.Key.Date.ToShortDateString());
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

            Console.WriteLine("-----------------");
            if (t2.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t2.Result;
                theConsoleString.AppendLine($"Weather forecast for {forecast.City}");
                var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                foreach (var group in GroupedList)
                {
                    theConsoleString.AppendLine(group.Key.Date.ToShortDateString());
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
            /*foreach (var forecast in forecasts)
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
                }*/
        }
        void ReportWeatherDataAvailable(object sender, string message)
        {
            theConsole.WriteLine($"Event message from weather service: {message}");
        }


        #endregion


    }
}
