using Digillect;

namespace WeatherAPI.Services;

public partial class PublicPartialError : Error
{
	public static partial PublicPartialError Internal();
	public static partial Error CodeAndMessage(int code, string message);
	public static partial Error CodeAndMessageWithDefaults(int code = 400, string? message = null);
}
