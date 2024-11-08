# UsingSessionPattern
Pattern for resource sessions enclosed in a using c# statement

This pattern aims to reduce the lines of code, and potential errors, when a class behaves similarly, but not equal to an `IDisposable` implementation.

For example: the `IDbConnection` has `Open()` and `Close` methods that should be enclosed in a `try .. catch` statement to be consistent:

````
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
using (connection.OpenSession())
{
	using var cmd = connection.CreateCommand();
	cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
	cmd.ExecuteNonQuery();
}
````

Note that the `IDbConnection` itself is not disposed, as it is the `DbConnectionOpenSession` that is disposed, so the `IDbConnection` can be used and opened again later
