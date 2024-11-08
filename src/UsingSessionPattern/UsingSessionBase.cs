using System;
using System.Collections.Generic;
using System.Text;

namespace UsingSessionPattern
{

	/// <summary>
	/// Base class to use for a <see cref="IUsingSession"/> to be included in a <c>using</c> clause
	/// </summary>
	/// <inheritdoc cref="IUsingSession"/>
	public abstract class UsingSessionBase : DisposableBase, IUsingSession
	{

		/// <summary>
		/// Nuova istanza della classe
		/// </summary>
		protected UsingSessionBase()
		{
		}

		#region Properties

		/// <inheritdoc cref="IUsingSession.IsSessionEnded"/>
		public bool IsSessionEnded { get; private set; } = false;

		#endregion

		#region Public methods

		/// <inheritdoc cref="IUsingSession"/>
		public void EndSession()
		{
			if (!this.IsSessionEnded)
			{
				this.IsSessionEnded = true;
				this.DoEndSession();
			}
		}

		#endregion

		#region Protected methods

		/// <inheritdoc cref="EndSession"/>
		protected abstract void DoEndSession();

		#endregion

		#region Override methods

		/// <inheritdoc/>
		protected override void Disposing(bool disposing)
		{
			base.Disposing(disposing);
			this.EndSession();
		}

		#endregion

	}

}
