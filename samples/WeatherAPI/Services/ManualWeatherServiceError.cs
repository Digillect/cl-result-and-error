using Digillect;

namespace WeatherAPI.Services;

public abstract class ManualWeatherServiceError : Error
{
	public static ManualWeatherServiceError InvalidCount(int count) => new InvalidCountError {
		Count = count,
	};

	public static Error TooFarAway() => TooFarAwayError.Instance;

	public sealed class InvalidCountError : ManualWeatherServiceError
	{
		public int Count { get; init; }

		public override string ToString() => $"Requested number of days ({Count}) is not valid";
	}

	public sealed class TooFarAwayError : ManualWeatherServiceError
	{
		public static readonly TooFarAwayError Instance = new();
	}
}
