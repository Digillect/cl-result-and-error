using System;

namespace Digillect
{
	/// <summary>
	/// The exception that is thrown when an operation of <see cref="Result{T}"/> can't be fulfilled due to the
	/// wrong state of the object.
	/// </summary>
	public abstract class InvalidResultStateException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidResultStateException"/> class with the specified
		/// error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		protected InvalidResultStateException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// The exception that is thrown when an operation of <see cref="Result{T}"/> can't be fulfilled because
	/// result is not in Success state.
	/// </summary>
	public class ResultIsNotSuccessException : InvalidResultStateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResultIsNotSuccessException"/> class.
		/// </summary>
		public ResultIsNotSuccessException()
			: base("Result is not in the Success state")
		{
		}
	}

	/// <summary>
	/// The exception that is thrown when an operation of <see cref="Result{T}"/> can't be fulfilled because
	/// result is not in Failure state.
	/// </summary>
	public class ResultIsNotFailureException : InvalidResultStateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResultIsNotFailureException"/> class.
		/// </summary>
		public ResultIsNotFailureException()
			: base("Result is not in the Failure state")
		{
		}
	}
}
