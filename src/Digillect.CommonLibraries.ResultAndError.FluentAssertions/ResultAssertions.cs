using Digillect;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace FluentAssertions
{
	/// <summary>
	/// Contains a number of methods to assert that a <see cref="Result{T}"/> is in the expected state.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ResultAssertions<T> : ReferenceTypeAssertions<Result<T>, ResultAssertions<T>>
		where T : notnull
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResultAssertions{T}"/> class.
		/// </summary>
		/// <param name="subject">Result to assert.</param>
		public ResultAssertions(Result<T> subject)
			: base(subject)
		{
		}

		/// <inheritdoc cref="ReferenceTypeAssertions{TSubject,TAssertions}.Identifier"/>
		protected override string Identifier => "result";

		/// <summary>
		/// Asserts that result is in the <c>Success</c> state.
		/// </summary>
		/// <param name="because">
		/// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
		/// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
		/// </param>
		/// <param name="becauseArgs">
		/// Zero or more objects to format using the placeholders in <paramref name="because" />.
		/// </param>
		public AndWhichConstraint<ResultAssertions<T>, T> BeSuccess(string because = "", params object[] becauseArgs)
		{
			Execute.Assertion
				.BecauseOf(because, becauseArgs)
				.WithExpectation("Expected {context:result} to be Success{reason}, ")
				.Given(() => Subject)
				.ForCondition(result => result.IsSuccess)
				.FailWith("but found to be not.");

			// Casting Subject to T is safe, because it has been guarded by the check of Subject.IsSuccess
			// in the assertions above
			return new AndWhichConstraint<ResultAssertions<T>, T>(this, (T) Subject);
		}

		/// <summary>
		/// Asserts that result is in the <c>Failure</c> state.
		/// </summary>
		/// <param name="because">
		/// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
		/// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
		/// </param>
		/// <param name="becauseArgs">
		/// Zero or more objects to format using the placeholders in <paramref name="because" />.
		/// </param>
		public AndWhichConstraint<ResultAssertions<T>, Error> BeFailure(string because = "", params object[] becauseArgs)
		{
			Execute.Assertion
				.BecauseOf(because, becauseArgs)
				.WithExpectation("Expected {context:result} to be Failure{reason}, ")
				.Given(() => Subject)
				.ForCondition(result => result.IsFailure)
				.FailWith("but found to be not.");

			// Casting Subject to Error is safe, because it has been guarded by the check of Subject.IsFailure
			// in the assertions above
			return new AndWhichConstraint<ResultAssertions<T>, Error>(this, (Error) Subject);
		}

		/// <summary>
		/// Asserts that result is in the <c>Failure</c> state and the error message is the same as <paramref name="expectedMessage"/>.
		/// </summary>
		/// <param name="expectedMessage">Message to compare an error message to.</param>
		/// <param name="because">
		/// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
		/// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
		/// </param>
		/// <param name="becauseArgs">
		/// Zero or more objects to format using the placeholders in <paramref name="because" />.
		/// </param>
		public AndConstraint<ResultAssertions<T>> BeFailureWithMessage(string expectedMessage, string because = "", params object[] becauseArgs)
		{
			Execute.Assertion
				.BecauseOf(because, becauseArgs)
				.ForCondition(Subject.IsFailure)
				.FailWith("Expected {context:result} to be Failure{reason}, but found to be not.")
				.Then
				.Given(() => ((Error) Subject).ToString())
				.ForCondition(errorMessage => string.Equals(expectedMessage, errorMessage))
				.FailWith(
					"Expected {context:result} to be Failure with message {0}{reason}, but message is {1}",
					_ => expectedMessage,
					errorMessage => errorMessage);

			return new AndConstraint<ResultAssertions<T>>(this);
		}

		/// <summary>
		/// Asserts that result is in the <c>Failure</c> state and represented by the <typeparamref name="TError"/> type.
		/// </summary>
		/// <param name="because">
		/// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
		/// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
		/// </param>
		/// <param name="becauseArgs">
		/// Zero or more objects to format using the placeholders in <paramref name="because" />.
		/// </param>
		/// <typeparam name="TError">Type of the error.</typeparam>
		public AndWhichConstraint<ResultAssertions<T>, TError> BeFailure<TError>(string because = "", params object[] becauseArgs)
			where TError : Error
		{
			Execute.Assertion
				.BecauseOf(because, becauseArgs)
				.WithExpectation("Expected {context:result} to be an unspecified error{reason}, ")
				.Given(() => Subject)
				.ForCondition(result => result.IsFailure)
				.FailWith("but found to be not.")
				.Then
				.Given(result => (Error) result as TError)
				.ForCondition(error => error != null)
				.FailWith("but is of different type.");

			// Casting Subject to Error is safe, because it has been guarded by the check of Subject.IsFailure
			// in the assertions above
			return new AndWhichConstraint<ResultAssertions<T>, TError>(this, (TError) (Error) Subject);
		}
	}
}
