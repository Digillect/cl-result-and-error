using Digillect;

namespace WeatherAPI.Services;

internal abstract partial class InternalAbstractPartialError : Error
{
	public static partial InternalAbstractPartialError Internal();
}
