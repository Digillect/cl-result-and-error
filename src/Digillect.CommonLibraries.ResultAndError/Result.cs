using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Digillect
{
	/// <summary>
	/// Result of some operation that can either be a value or an <see cref="Error"/>.
	/// </summary>
	/// <typeparam name="T">Type of the successful result value.</typeparam>
	public readonly struct Result<T> : IEquatable<Result<T>>
	{
		/// <summary>
		/// Result value.
		/// </summary>
		private readonly T _value;

		/// <summary>
		/// An error.
		/// </summary>
		private readonly Error _error;

		/// <summary>
		/// <c>true</c> if this result represents a success.
		/// </summary>
		public readonly bool IsSuccess;

		/// <summary>
		/// Initializes new successful result.
		/// </summary>
		/// <param name="value">Result value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Result(in T value)
		{
			_value = value;
			_error = default;
			IsSuccess = true;
		}

		/// <summary>
		/// Initializes new failed result.
		/// </summary>
		/// <param name="error">An error.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Result(in Error error)
		{
			_value = default;
			_error = error;
			IsSuccess = false;
		}

		/// <summary>
		/// <c>true</c> if this result represents an error.
		/// </summary>
		public bool IsFailure
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !IsSuccess;
		}

		/// <summary>
		/// Successful result or an error to be used in the switch expression.
		/// </summary>
		[Pure]
		public object Case => IsSuccess ? (object) _value : _error;

		/// <summary>
		/// Returns value of the successful result.
		/// </summary>
		/// <param name="result">Result to operate with.</param>
		/// <returns>Result value.</returns>
		/// <exception cref="ResultIsNotSuccessException">When result is not in the Success state.</exception>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator T(Result<T> result) => result.IsSuccess ? result._value! : throw new ResultIsNotSuccessException();

		/// <summary>
		/// Returns an error of the failed result.
		/// </summary>
		/// <param name="result">Result to operate with.</param>
		/// <returns>Result error.</returns>
		/// <exception cref="ResultIsNotFailureException">When result is not in the Failure state.</exception>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Error(Result<T> result) => result.IsFailure ? result._error! : throw new ResultIsNotFailureException();

		/// <summary>
		/// Creates successful result.
		/// </summary>
		/// <param name="value">Result value.</param>
		/// <returns>Successful result.</returns>
		/// <exception cref="ArgumentNullException">If passed value is <c>null</c>.</exception>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Success(T value) => value != null ? new Result<T>(value) : throw new ArgumentNullException(nameof(value));

		/// <summary>
		/// Creates failed result from the specified error.
		/// </summary>
		/// <param name="error">Error that caused a failure.</param>
		/// <returns>Failed result.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Failure(Error error) => new Result<T>(in error);

		/// <summary>
		/// Creates failed result based on the <see cref="UnspecifiedError"/> with the provided error message.
		/// </summary>
		/// <param name="message">Message that describes the cause of the failure.</param>
		/// <returns>Failed result.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Result<T> Failure(string message) => new Result<T>(Error.New(message));

		/// <summary>
		/// Implicitly converts value of proper type to the successful result.
		/// </summary>
		/// <param name="value">Result value.</param>
		/// <returns>Successful result.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T>(T value) => Success(value);

		/// <summary>
		/// Implicitly converts an error to the failed result.
		/// </summary>
		/// <param name="error">An error.</param>
		/// <returns>Failed result.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Result<T>(Error error) => Failure(error);

		/// <summary>
		/// Returns <c>true</c> to indicate that the result is a successful one.
		/// </summary>
		/// <param name="result">Result to test.</param>
		/// <returns><c>true</c> if the result is successful, otherwise <c>false</c>.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator true(Result<T> result) => result.IsSuccess;

		/// <summary>
		/// Returns <c>true</c> to indicate that the result is a failure.
		/// </summary>
		/// <param name="result">Result to test.</param>
		/// <returns><c>true</c> if the result is a failure, otherwise <c>false</c>.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator false(Result<T> result) => !result.IsSuccess;

		/// <summary>
		/// Projects result to the new form depending on the result state.
		/// </summary>
		/// <param name="success">A transform function to project a successful result to the target type.</param>
		/// <param name="failure">A transform function to project an error to the target type.</param>
		/// <typeparam name="TResult">Target type.</typeparam>
		/// <returns>Projected value.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TResult Match<TResult>(Func<T, TResult> success, Func<Error, TResult> failure) => IsSuccess ? success(_value!) : failure(_error!);

		/// <summary>
		/// Calls an appropriate action depending on the result state.
		/// </summary>
		/// <param name="success">An action to call for the successful result.</param>
		/// <param name="failure">An action to call for the error.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Match(Action<T> success, Action<Error> failure)
		{
			if (IsSuccess)
			{
				success(_value!);
			}
			else
			{
				failure(_error!);
			}
		}

		/// <summary>
		/// Returns a successful value, if any, or transforms an error to the type of the successful result.
		/// </summary>
		/// <param name="failure">An transform function to project an error to the successful result.</param>
		/// <returns>Successful value or transformed error.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T IfFailure(Func<Error, T> failure) => IsSuccess ? _value! : failure(_error!);

		/// <summary>
		/// Returns a successful value or provided alternative if result is a failure.
		/// </summary>
		/// <param name="alternative">A value to return if result is a failure.</param>
		/// <returns>Successful value or a the provided alternative.</returns>
		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T IfFailure(in T alternative) => IsSuccess ? _value! : alternative;

		/// <summary>
		/// Executes an action if the result is a failure.
		/// </summary>
		/// <param name="failure">An action to execute.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void IfFailure(Action<Error> failure)
		{
			if (!IsSuccess)
			{
				failure(_error!);
			}
		}

		/// <summary>
		/// Executes an action if the result is a success.
		/// </summary>
		/// <param name="success">An action to execute.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void IfSuccess(Action<T> success)
		{
			if (IsSuccess)
			{
				success(_value!);
			}
		}

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(Result<T> other) =>
			(IsSuccess && other.IsSuccess && EqualityComparer<T>.Default.Equals(_value, other._value)) || (!IsSuccess && !other.IsSuccess);

		/// <inheritdoc cref="ValueType.Equals(object)"/>
		public override bool Equals(object obj) => obj is Result<T> other && Equals(other);

		/// <inheritdoc cref="ValueType.GetHashCode"/>
		public override int GetHashCode() => HashCode.Combine(IsSuccess, _value, _error);

		/// <summary>
		/// Compares two results for being equal.
		/// </summary>
		/// <param name="left">First result.</param>
		/// <param name="right">Second result</param>
		/// <returns><c>true</c> if two results are equal, otherwise <c>false</c>.</returns>
		public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

		/// <summary>
		/// Compares two results for not being equal.
		/// </summary>
		/// <param name="left">First result.</param>
		/// <param name="right">Second result</param>
		/// <returns><c>true</c> if two results are not equal, otherwise <c>false</c>.</returns>
		public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
	}

	/// <summary>
	/// Class to simplify creation of the instances of <see cref="Result{T}"/> class.
	/// </summary>
	public static class Result
	{
		/// <summary>
		/// Creates the <see cref="Result{T}"/> in <b>Success</b> state with the specified result value.
		/// </summary>
		/// <param name="value">Result value.</param>
		/// <typeparam name="T">Type of the result value.</typeparam>
		/// <returns>Successful <see cref="Result{T}"/>.</returns>
		public static Result<T> Success<T>(in T value)
			where T : notnull =>
			Result<T>.Success(value);
	}
}
