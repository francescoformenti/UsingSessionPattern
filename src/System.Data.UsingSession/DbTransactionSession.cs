using System;
using System.Collections.Generic;
using System.Text;
using UsingSessionPattern;

namespace System.Data.UsingSession
{

	/// <summary>
	/// Manages a <see cref="IDbTransaction"/> with the Using Session Pattern.
	/// </summary>
	/// <remarks>
	/// <para/>When the session ends calls:
	/// <list type="bullet">
	/// <item><see cref="IDbTransaction.Commit"/> if <see cref="Commit"/> has been called</item>
	/// <item><see cref="IDbTransaction.Rollback"/> otherwise</item>
	/// </list>
	/// </remarks>
	public class DbTransactionSession : UsingSessionBase
	{

		/// <summary>
		/// Initiates a new <see cref="IDbConnection.BeginTransaction()"/> session that should be enclosed in a <c>using</c> clause.
		/// </summary>
		/// <param name="connection">Connection on which <see cref="IDbConnection.Open"/> is called</param>
		/// <inheritdoc cref="DbTransactionSession"/>
		internal DbTransactionSession(IDbConnection connection)
		{
			this.Transaction = connection.BeginTransaction();
		}

		/// <summary>
		/// Initiates a new <see cref="IDbConnection.BeginTransaction(IsolationLevel)"/> session that should be enclosed in a <c>using</c> clause.
		/// </summary>
		/// <param name="connection">Connection on which <see cref="IDbConnection.Open"/> is called</param>
		/// <inheritdoc cref="DbTransactionSession"/>
		/// <inheritdoc cref="IDbConnection.BeginTransaction(IsolationLevel)"/>
		/// <param name="il"></param>
		internal DbTransactionSession(IDbConnection connection, IsolationLevel il)
		{
			this.Transaction = connection.BeginTransaction(il);
		}

		#region Properties

		/// <summary>
		/// Transaction managed by the instance.
		/// </summary>
		/// <remarks>
		/// The <see cref="IDbTransaction.Commit"/> must not be called directly in this pattern.
		/// <para/>You should instead use <see cref="Commit"/> in this instance.
		/// </remarks>
		public IDbTransaction Transaction { get; }

		/// <summary>
		/// Obtains a value that indicates if <see cref="Commit"/> has been called.
		/// </summary>
		public bool IsCommitted { get; private set; } = false;

		#endregion

		#region Public methods

		/// <summary>
		/// Performs <see cref="IDbTransaction.Commit"/> and sets <see cref="IsCommitted"/> to true
		/// </summary>
		public void Commit()
		{
			this.Transaction.Commit();
			this.IsCommitted = true;
		}

		#endregion

		#region Override methods

		/// <inheritdoc/>
		protected override void DoEndSession()
		{
			if (!this.IsCommitted)
			{
				this.Transaction?.Rollback();
			}
		}

		#endregion

	}

}
