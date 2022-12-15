# Digillect Common Libraries: Result and Error

Implementation of the `Result<T>` type that is used to return either successful
or unsuccessful result from methods.

Inspired by Louthy's implementation of `Fin<A>`, but the `Error` does not contain
any members and is solely used as a marker class.

## Usage

### Returning successful results

To return a successful result you can either call `Result.Success` passing in return value or
just return this value, that will be converted into a `Result`.

```csharp
Result<int> Multiply(int v1, int v2) => Result.Success(v1 * v2);

Result<int> Add(int v1, int v2) => v1 + v2;
```

### Returning errors

The common scenario imply creation of the specific `Error`-derived class(es) to report the
problem in the program flow, for example:

```csharp
class DivisionByZeroError : Error
{
    public override string ToString() => "Division by zero is prohibited.";
}

Result<double> Divide(int numerator, int denominator)
{
    if (denominator == 0)
    {
        return new DivizionByZeroError();
    }
    
    return numerator/denominator;
} 
```

If you only need to report that the problem has happened (and provide an error message), a predefined
`UnspecifiedError` class can be used either by creating and returning an instance of that class or by the calling
`Result<T>.Error()` passing in the error message:

```csharp
Result<double> Divide(int numerator, int denominator)
{
    if (denominator == 0)
    {
        return Result<double>.Error("Division by zero is prohibited.");
    }
 
    // Manually construct the result instead of casting, just for example.   
    return Result.Success(numerator/denominator);
}
```

### Handling results

There are many ways to deal with the result, but the main one is to use `Match` method and provide
either functions or actions to handle the corresponding result:

```csharp
void DivideAndPrintResult(int v1, int v2)
{
    DivideTwoIntegers(v1, v2).Match(
        result => Console.WriteLine($"Division result is {result}"),
        error => Console.WriteLine($"Error: {error}"));
}

double DivideAndReturnZeroOnError(int v1, int v2)
{
    return Divide(v1, v2).Match(result => result, error => 0);
}
```

There are also shortcuts for the latter case:

```csharp
double DivideAndReturnZeroOnError(int v1, int v2) => Divide(v1, v2).IfFailure(0);
```

`Result` can be queried for the Success or Failure state by using `IsSuccess` and `IsFailure`. Sometimes,
when you've already ensured that result is either successful or failed and need to access the corresponding
value or an error you can cast that result to `T` or to `Error`. If the guard check has not been performed,
and you query a value from the faulted result or an error from the successful result, one of the
`ResultIsNotSuccessException` or `ResultIsNotFailureException` will be thrown.

You can also use the result in the `switch` expression by accessing the `Case` property:

```csharp
double DivideAndReturnZeroOnError(int v1, int v2) => Divide(v1, v2).Case switch {
    double result => result,
    Error error => 0,
}
```
