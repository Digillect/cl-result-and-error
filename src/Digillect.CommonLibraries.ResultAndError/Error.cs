namespace Digillect
{
	/// <summary>
	/// Base class for result failures.
	/// </summary>
	public abstract class Error
	{
		/// <summary>
		/// Creates and returns new <see cref="UnspecifiedError"/> with the provided message.
		/// </summary>
		/// <param name="message">error message.</param>
		/// <returns>Created error.</returns>
		public static Error New(string message) => new UnspecifiedError(message);
	}
}
