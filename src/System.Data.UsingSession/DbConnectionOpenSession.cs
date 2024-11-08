using System;
using UsingSessionPattern;

namespace System.Data.UsingSession
{

	/// <summary>
	/// Manages the <see cref="IDbConnection.Open"/> with the Using Session Pattern.
	/// </summary>
	/// <remarks>
	/// When the session is disposed, <see cref="IDbConnection.Close"/> is called.
	/// </remarks>
	public class DbConnectionOpenSession : UsingSessionBase
	{

		/// <summary>
		/// Initiates a new <see cref="IDbConnection.Open"/> session that should be enclosed in a <c>using</c> clause.
		/// </summary>
		/// <param name="connection">Connection on which <see cref="IDbConnection.Open"/> is called</param>
		/// <inheritdoc cref="DbConnectionOpenSession"/>
		internal DbConnectionOpenSession(IDbConnection connection)
		{
			connection?.Open();
			this.Connection = connection;
		}

		#region Properties

		/// <summary>
		/// Connection managed by the session
		/// </summary>
		public IDbConnection Connection { get; }

		#endregion

		#region Override methods

		/// <inheritdoc/>
		protected override void DoEndSession()
		{
			this.Connection?.Close();
		}

		#endregion

	}

}
