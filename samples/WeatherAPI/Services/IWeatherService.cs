using Digillect;

namespace WeatherAPI.Services;

public interface IWeatherService
{
	Result<IReadOnlyList<WeatherForecast>> GetWeatherForecastWithManualErrors(int offsetInDays, int count);
	Result<IReadOnlyList<WeatherForecast>> GetWeatherForecastWithGeneratedErrors(int offsetInDays, int count);
}

internal class WeatherService : IWeatherService
{
	private static readonly string[] _summaries = {
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	public Result<IReadOnlyList<WeatherForecast>> GetWeatherForecastWithManualErrors(int offsetInDays, int count)
	{
		if (offsetInDays > 7)
		{
			return ManualWeatherServiceError.TooFarAway();
		}

		if (count < 1)
		{
			return ManualWeatherServiceError.InvalidCount(count);
		}

		return Enumerable.Range(1, count).Select(index => new WeatherForecast {
				Date = DateTime.Now.AddDays(offsetInDays + index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = _summaries[Random.Shared.Next(_summaries.Length)]
			})
			.ToList();
	}

	public Result<IReadOnlyList<WeatherForecast>> GetWeatherForecastWithGeneratedErrors(int offsetInDays, int count)
	{
		if (offsetInDays > 7)
		{
			return GeneratedWeatherServiceError.TooFarAway();
		}

		if (count < 1)
		{
			return GeneratedWeatherServiceError.InvalidCount(count);
		}

		return Enumerable.Range(1, count).Select(index => new WeatherForecast {
				Date = DateTime.Now.AddDays(offsetInDays + index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = _summaries[Random.Shared.Next(_summaries.Length)]
			})
			.ToList();
	}
}
