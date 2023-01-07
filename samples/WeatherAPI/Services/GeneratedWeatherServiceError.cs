using Digillect;

namespace WeatherAPI.Services;

public abstract partial class GeneratedWeatherServiceError : Error
{
	public static partial GeneratedWeatherServiceError InvalidCount(int count);
	public static partial Error TooFarAway();

	public partial class InvalidCountError
	{
		public override string ToString() => $"Requested number of days ({Count}) is not valid";
	}
}
