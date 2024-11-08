# Using Session Pattern
Pattern for resource sessions enclosed in a using c# statement

This pattern aims to reduce the lines of code, and potential errors, when a class behaves similarly, but not equal to an `IDisposable` implementation.

## IDbConnection
For example: the `IDbConnection` has `Open()` and `Close()` methods that should be enclosed in a `try .. finally` statement to be consistent:

````
var connection = this.CreateConnection();
connection.Open();
try
{
	using (var cmd = connection.CreateCommand())
	{
		cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
		cmd.ExecuteNonQuery();
	}
}
finally
{
	connection.Close();
}
````

With this pattern, the code can be reduced with the help of a `IDisposable` class named `DbConnectionOpenSession` that opens the connection when created, and closes it when disposed.

````
var connection = this.CreateConnection();
using (connection.OpenSession())
{
	using var cmd = connection.CreateCommand();
	cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
	cmd.ExecuteNonQuery();
}
````

Note that the `IDbConnection` itself is not disposed, as it is the `DbConnectionOpenSession` that is disposed, so the `IDbConnection` can be used and opened again later.  
In addition, since the `DbConnectionOpenSession` created by the `OpenSession` extension method is quite useless, you can avoid declaring a variable name for it, letting the using statement to Dispose it when its scope ends.

## IDbTransaction
The pattern becomes more useful as the complexity increases, for example adding a `IDbTransaction` to the previous code snippet:

````
var connection = this.CreateConnection();
connection.Open();
try
{
	var transaction = connection.BeginTransaction();
	try
	{
		using (var cmd = connection.CreateCommand())
		{
			cmd.Transaction = transaction;
			cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
			cmd.ExecuteNonQuery();
		}
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

With the **Using Session Pattern** the amount of lines decreases significantly:

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
You don't have to worry about the `Rollback`, that would be called automatically if `Commit` is not called before the `DbTransactionSession` instance is disposed; for example if an exception occurs before the call to `Commit`.


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
The pattern is quite simple: it is based on the `IUsingSession` interface:
````
public interface IUsingSession : IDisposable
{

	bool IsSessionEnded { get; }

	void EndSession();

}
````
## UsingSessionBase
`IUsingSession` base implementation `UsingSessionBase` is quite simple too:
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
It is an abstract class on which there is the abstract method `DoEndSession` that can be overridden in derived classes to perform the release of the resource.  
The `DisposableBase` class simply implements the `IDisposable` pattern.

If the `EndSession` method has not been explicitly called, the `IDisposable.Dispose` will call it.

## DbConnectionOpenSession
This class derives and implements `UsingSessionBase` abstract class:
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
