# Using Session Pattern
A pattern for managing resource sessions using the `using` statement in C#

This pattern aims to reduce the lines of code, and potential errors, when a class behaves similarly, but not equal to an `IDisposable` implementation.
This pattern helps reduce the amount of lines of code and minimize errors when a class behaves similarly to, but does not implement, `IDisposable` or should be put in a `try .. finally` block.

## IDbConnection
Consider the `IDbConnection`, which has `Open()` and `Close()` methods that should be enclosed in a `try...finally` block for proper resource management:

````
var connection = this.CreateConnection();
connection.Open();
try
{
	using var cmd = connection.CreateCommand();
	cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
	cmd.ExecuteNonQuery();
}
finally
{
	connection.Close();
}
````

With this pattern, the code can be streamlined using an `IDisposable` class named `DbConnectionOpenSession`, which automatically opens the connection when created and closes it when disposed.

````
var connection = this.CreateConnection();
using (connection.OpenSession())
{
	using var cmd = connection.CreateCommand();
	cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
	cmd.ExecuteNonQuery();
}
````

Note that in this case, the `IDbConnection` itself is not disposed; instead, it's the `DbConnectionOpenSession` that is disposed, which allows you to reuse the `IDbConnection` later.  
Additionally, since the `DbConnectionOpenSession` created by the `OpenSession` extension method is effectively "useless", you can omit the variable declaration, letting the using statement to Dispose it when its scope ends.

## IDbTransaction
The pattern becomes even more useful when dealing with more complex scenarios, such as adding a `IDbTransaction` to the mix. Consider the following code:

````
var connection = this.CreateConnection();
connection.Open();
try
{
	var transaction = connection.BeginTransaction();
	try
	{
		using var cmd = connection.CreateCommand();
		cmd.Transaction = transaction;
		cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
		cmd.ExecuteNonQuery();
		transaction.Commit();
	}
	catch
	{
		transaction.Rollback();
		throw;
	}
}
finally
{
	connection.Close();
}
````

With the **Using Session Pattern** the code becomes more concise:

````
var connection = this.CreateConnection();
using (connection.OpenSession())
{
	using var transaction = connection.BeginTransactionSession();
	using var cmd = connection.CreateCommand();
	cmd.Transaction = transaction.Transaction;
	cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
	cmd.ExecuteNonQuery();
	transaction.Commit();
}
````
Here, you no longer need to manually handle `Rollback`. If `Commit` is not called before the `DbTransactionSession` is disposed, the transaction will be automatically rolled back in the `Dispose()` method.

If you are using older version of .NET, like .NET Framework or .NET Standard 2.0, or if you don't like the `using` variable declarations, the amount of lines does not change at all:

````
var connection = this.CreateConnection();
using (connection.OpenSession())
using (var transaction = connection.BeginTransactionSession())
using (var cmd = connection.CreateCommand())
{
	cmd.Transaction = transaction.Transaction;
	cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
	cmd.ExecuteNonQuery();
	transaction.Commit();
}
````

## IUsingSession
The pattern is built around the simple `IUsingSession` interface:
````
public interface IUsingSession : IDisposable
{

	bool IsSessionEnded { get; }

	void EndSession();

}
````
## UsingSessionBase
The base class `UsingSessionBase` provides a simple implementation for managing session lifecycles:
````
public abstract class UsingSessionBase : DisposableBase, IUsingSession
{

	public bool IsSessionEnded { get; private set; } = false;

	public void EndSession()
	{
		if (!this.IsSessionEnded)
		{
			this.IsSessionEnded = true;
			this.DoEndSession();
		}
	}

	protected abstract void DoEndSession();

	protected override void Disposing(bool disposing)
	{
		base.Disposing(disposing);
		this.EndSession();
	}

}
````
This abstract class ensures that resources are cleaned up by overriding the `Dispose()` method of `DisposableBase`, which is an implementation of the `IDisposable` pattern.  
The `DoEndSession()` method is abstract and can be implemented in derived classes to perform specific resource cleanup.

If the `EndSession` method has not been explicitly called, the `IDisposable.Dispose` will call it.

## DbConnectionOpenSession
The `DbConnectionOpenSession` class implements the `UsingSessionBase` abstract class:
````
public class DbConnectionOpenSession : UsingSessionBase
{

	internal DbConnectionOpenSession(IDbConnection connection)
	{
		connection?.Open();
		this.Connection = connection;
	}

	public IDbConnection Connection { get; }

	protected override void DoEndSession()
	{
		this.Connection?.Close();
	}

}
````
It opens the connection and assigns it to the local property, then closes it in the `DoEndSession` method.
The constructor is declared with `internal` access modifier for ease of use with extensions methods

## IDbConnection.OpenSession
The extension method allows the creation of an 'Open Session' without using the constructor, that would look as an awful way of coding:
````
public static class SystemDataExtensions
{

	public static DbConnectionOpenSession OpenSession(this IDbConnection connection)
	{
		return new DbConnectionOpenSession(connection);
	}

}
````

## Conclusions
This pattern could be used for almost everything that could be nested in a `try .. finally` block and should do very simple but repetitive tasks
