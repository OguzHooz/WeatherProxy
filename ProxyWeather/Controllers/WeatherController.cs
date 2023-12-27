using Microsoft.AspNetCore.Mvc;
using ProxyWeather.Models;
using System.Text.Json;

namespace ProxyWeather.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        IConfiguration _config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        [HttpGet("{city}")]
        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> GetWeather(string city)
        {
            var OpenWeatherKey = _config["OpenWeatherKey"];

            string geoURL = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&appid={OpenWeatherKey}";
            
            HttpClient client = new HttpClient();

            string geoJson = await client.GetStringAsync(geoURL);

            var geoData = JsonSerializer.Deserialize<List<GeoLocation>>(geoJson);

            if (geoJson != null && geoData.Count > 0)
            {
                double lat = geoData[0].lat;
                double lon = geoData[0].lon;
                
                string WeatherURL = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&units=metric&appid={OpenWeatherKey}";

                string weatherJson = await client.GetStringAsync(WeatherURL);

                RootWeather weatherData = JsonSerializer.Deserialize<RootWeather>(weatherJson);

                return Ok(weatherData);
            }
            return NotFound();
        }
    }
}
