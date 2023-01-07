using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	[HttpGet("manual")]
	public IActionResult GetManual([FromServices] IWeatherService weatherService, int offsetInDays = 0, int count = 5)
	{
		return weatherService.GetWeatherForecastWithManualErrors(offsetInDays, count).Match<IActionResult>(
			Ok,
			error => error switch {
				ManualWeatherServiceError.InvalidCountError e => BadRequest(e.ToString()),
				_ => StatusCode(StatusCodes.Status500InternalServerError)
			});
	}

	[HttpGet("generated")]
	public IActionResult Get([FromServices] IWeatherService weatherService, int offsetInDays = 0, int count = 5)
	{
		return weatherService.GetWeatherForecastWithGeneratedErrors(offsetInDays, count).Case switch {
			IReadOnlyList<WeatherForecast> results => Ok(results),
			GeneratedWeatherServiceError.InvalidCountError e => BadRequest(e.ToString()),
			_ => StatusCode(StatusCodes.Status500InternalServerError)
		};

		// return weatherService.GetWeatherForecastWithGeneratedErrors(offsetInDays, count).Match<IActionResult>(
		// 	Ok,
		// 	error => error switch {
		// 		GeneratedWeatherServiceError.InvalidCountError e => BadRequest(e.ToString()),
		// 		_ => StatusCode(StatusCodes.Status500InternalServerError)
		// 	});
	}
}
