using Digillect;

namespace WeatherAPI.Services;

public abstract partial class PublicAbstractPartialError : Error
{
	public static partial PublicAbstractPartialError Internal();
}
