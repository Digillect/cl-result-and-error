namespace Digillect
{
	/// <summary>
	/// Error with no particular meaning, but with the message that explains the cause of the error.
	/// </summary>
	public sealed class UnspecifiedError : Error
	{
		/// <summary>
		/// Initializes a new instance of <see cref="UnspecifiedError"/> class with the specified error message.
		/// </summary>
		/// <param name="message">Message that describes the cause of the failure.</param>
		public UnspecifiedError(string message)
		{
			Message = message;
		}

		/// <summary>
		/// Message that describes the cause of the failure.
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// Returns error message.
		/// </summary>
		public override string ToString() => Message;
	}
}
