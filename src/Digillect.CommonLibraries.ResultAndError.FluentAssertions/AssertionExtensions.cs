using Digillect;

namespace FluentAssertions
{
	/// <summary>
	/// Contains extension methods for custom assertions in unit tests.
	/// </summary>
	public static class AssertionExtensions
	{
		/// <summary>
		/// Returns an <see cref="ResultAssertions{T}"/> object that can be used to assert the
		/// current <see cref="Result{T}"/>.
		/// </summary>
		public static ResultAssertions<T> Should<T>(this Result<T> result)
			where T : notnull
		{
			return new ResultAssertions<T>(result);
		}
	}
}
