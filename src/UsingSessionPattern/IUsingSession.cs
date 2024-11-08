using System;

namespace UsingSessionPattern
{

	/// <summary>
	/// Interface for a session object to be used in a <c>using</c> clause
	/// </summary>
	public interface IUsingSession : IDisposable
	{

		#region Properties

		/// <summary>
		/// Obtains a value that indicates if <see cref="EndSession"/> has been already called at least once on this instance
		/// </summary>
		bool IsSessionEnded { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Closes the session.
		/// <para/>This method should be called in the <see cref="IDisposable.Dispose"/> by the interface implementation; an explicit call is therefore not necessary if the instance is wrapped in a <c>using</c>
		/// </summary>
		void EndSession();

		#endregion

	}

}
