using System;
using System.Collections.Generic;
using System.Text;
using UsingSessionPattern;

namespace System.Data.UsingSession
{

	/// <summary>
	/// Static class with extension methods for Using Session Partern
	/// </summary>
	public static class SystemDataExtensions
	{

		#region Extension methods

		/// <returns>New <see cref="DbConnectionOpenSession"/> instance</returns>
		/// <inheritdoc cref="DbConnectionOpenSession(IDbConnection)"/>
		public static DbConnectionOpenSession OpenSession(this IDbConnection connection)
		{
			return new DbConnectionOpenSession(connection);
		}

		/// <returns>New <see cref="DbTransactionSession"/> instance</returns>
		/// <inheritdoc cref="DbTransactionSession(IDbConnection)"/>
		public static DbTransactionSession BeginTransactionSession(this IDbConnection connection)
		{
			return new DbTransactionSession(connection);
		}

		/// <returns>New <see cref="DbTransactionSession"/> instance</returns>
		/// <inheritdoc cref="DbTransactionSession(IDbConnection, IsolationLevel)"/>
		public static DbTransactionSession BeginTransactionSession(this IDbConnection connection, IsolationLevel il)
		{
			return new DbTransactionSession(connection, il);
		}

		#endregion

	}

}
