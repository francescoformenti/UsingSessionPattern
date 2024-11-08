using System;

namespace System.Data.UsingSession.Examples.NET8
{

	/// <summary>
	/// Examples of <see cref="DbConnectionOpenSession"/> for .NET 8.0
	/// </summary>
	public static class DbConnectionOpenExamplesNET8
	{

		#region Public methods

		/// <summary>
		/// Example with Classic <see cref="IDbConnection.Open"/>
		/// </summary>
		/// <param name="connection">Connection</param>
		public static void ClassicOpenExample(IDbConnection connection)
		{
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
		}

		/// <summary>
		/// Example with Using Session Pattern <see cref="IDbConnection.Open"/>
		/// </summary>
		/// <param name="connection">Connection</param>
		public static void UsingSessionOpenExample(IDbConnection connection)
		{
			using (connection.OpenSession())
			{
				using var cmd = connection.CreateCommand();
				cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
				cmd.ExecuteNonQuery();
			}
		}

		#endregion

	}

}
