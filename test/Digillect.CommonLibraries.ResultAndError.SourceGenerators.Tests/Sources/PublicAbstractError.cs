using Digillect;

namespace WeatherAPI.Services;

public abstract class PublicAbstractError : Error
{
	public static partial PublicAbstractError Internal();
}
