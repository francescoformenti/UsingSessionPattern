using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.UsingSession.Examples.NET8
{

	/// <summary>
	/// Examples of <see cref="DbTransactionSession"/> for .NET 8.0
	/// </summary>
	public static class DbTransactionExamplesNET8
	{

		#region Example methods

		/// <summary>
		/// Example with Classic <see cref="IDbConnection.BeginTransaction()"/> without catch statement
		/// </summary>
		/// <param name="connection"><inheritdoc cref="IDbConnection" path="/summary"/></param>
		public static void ClassicOpenAndTransactionExampleWithoudCatch(IDbConnection connection)
		{
			connection.Open();
			try
			{
				bool executed = false;
				var transaction = connection.BeginTransaction();
				try
				{
					using (var cmd = connection.CreateCommand())
					{
						cmd.Transaction = transaction;
						cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
						cmd.ExecuteNonQuery();
					}
					executed = true;
				}
				finally
				{
					if (executed)
					{
						transaction.Commit();
					}
					else
					{
						transaction.Rollback();
					}
				}
			}
			finally
			{
				connection.Close();
			}
		}

		/// <summary>
		/// Example with Classic <see cref="IDbConnection.BeginTransaction()"/> with catch statement
		/// </summary>
		/// <param name="connection"><inheritdoc cref="IDbConnection" path="/summary"/></param>
		public static void ClassicOpenAndTransactionWithCatch(IDbConnection connection)
		{
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
		}

		/// <summary>
		/// Example with Using Session Pattern <see cref="IDbConnection.BeginTransaction()"/>
		/// </summary>
		/// <param name="connection"><inheritdoc cref="IDbConnection" path="/summary"/></param>
		public static void UsingSessionOpenAndTransactionExample(IDbConnection connection)
		{
			using (connection.OpenSession())
			{
				using var transaction = connection.BeginTransactionSession();
				using var cmd = connection.CreateCommand();
				cmd.Transaction = transaction.Transaction;
				cmd.CommandText = "UPDATE Persons SET Name='Foo' WHERE ID=1";
				cmd.ExecuteNonQuery();
				transaction.Commit();
			}
		}

		#endregion

	}

}
