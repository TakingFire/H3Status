using System;
using Fleck2.Interfaces;
using System.Threading;

namespace Fleck2
{
    public class SocketCancellationToken : ICancellationToken
    {

        #region Variables
        /// <summary>
        ///
        /// </summary>
        private readonly object _syncLock = new object();
        #endregion

        #region Fields
        /// <summary>
        ///
        /// </summary>
        public readonly Guid Token = new Guid();

        /// <summary>
        ///
        /// </summary>
        private bool _isCancellationRequested;
        /// <summary>
        ///
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                Monitor.Enter(_syncLock);
                try
                {
                    return _isCancellationRequested;
                }
                finally
                {
                    Monitor.Exit(_syncLock);
                }
            }
            private set
            {
                Monitor.Enter(_syncLock);
                try
                {
                    _isCancellationRequested = value;
                }
                finally
                {
                    Monitor.Exit(_syncLock);
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        ///
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            if (IsCancellationRequested)
            {
                throw new SocketCancellationTokenException(this);
            }
        }
        /// <summary>
        ///
        /// </summary>
        public void Cancel()
        {
            IsCancellationRequested = true;
        }
        #endregion

    }
}
