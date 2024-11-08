using System;
using System.Collections.Generic;
using System.Text;

namespace UsingSessionPattern
{

	/// <summary>
	/// Base implementation o <see cref="IDisposable"/> interface
	/// </summary>
	public class DisposableBase : IDisposable
	{

		#region IDisposable Support

		/// <summary>
		/// Obtains a value that indicates if <see cref="IDisposable.Dispose"/> has already been called
		/// </summary>
		public bool IsDisposed { get; private set; } = false;

		private void Dispose(bool disposing)
		{
			if (!this.IsDisposed)
			{
				this.Disposing(disposing);
				this.IsDisposed = true;
			}
		}

		/// <summary>
		/// Performs the cleaning operations necessary for <see cref="IDisposable.Dispose"/>
		/// </summary>
		/// <param name="disposing">If true, it also deletes managed objects, otherwise only free unmanaged resources and set large fields to null</param>
		protected virtual void Disposing(bool disposing)
		{
		}

		/// <summary/>
		~DisposableBase()
		{
			this.Dispose(false);
		}

		/// <inheritdoc cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

	}

}
