# Using Session Pattern
Pattern for resource sessions enclosed in a using c# statement

This pattern aims to reduce the lines of code, and potential errors, when a class behaves similarly, but not equal to an `IDisposable` implementation.

For example: the `IDbConnection` has `Open()` and `Close` methods that should be enclosed in a `try .. catch` statement to be consistent:

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
