using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using HaniApi.Data;

namespace HaniApi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;
		private readonly TestContext _context; 

		public WeatherForecastController(
			ILogger<WeatherForecastController> logger,
			TestContext context)
		{
			_logger = logger;
			_context = context;
		}

		[HttpPost("GetWeather")]
		public void GetWeather(GetWeatherForecast info)
		{
			string appId = "8760a47cac26ab2fc5ac27c174056cac";

			string url = $"http://api.openweathermap.org/data/2.5/weather?lat={info.lat}&lon={info.lon}&appid={appId}&units=metric";

			using (WebClient client = new WebClient())
			{
				string json = client.DownloadString(url);

				RootObject weatherInfo = JsonConvert.DeserializeObject<RootObject>(json);

				if (weatherInfo.main.temp >= 14)
				{
					_context.WeatherForecasts.Add(new WeatherForecast
					{
						City = weatherInfo.name,
						TemperatureC = weatherInfo.main.temp,
						Date = DateTime.Now
					});
					_context.SaveChanges();
				}
			}
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = rng.Next(-20, 55),
				Summary = Summaries[rng.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}
