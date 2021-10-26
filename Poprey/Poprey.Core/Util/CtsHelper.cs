using System.Collections.Generic;
using System.Threading;

namespace Poprey.Core.Util
{
    public class CtsHelper
    {
        private readonly List<CancellationTokenSource> _cancellationSources;

        public CtsHelper()
        {
            _cancellationSources = new List<CancellationTokenSource>();
        }

        public CancellationTokenSource CreateCts()
        {
            return RegisterCancellationSource(new CancellationTokenSource());
        }

        public CancellationTokenSource RegisterCancellationSource(CancellationTokenSource cts)
        {
            _cancellationSources.Add(cts);
            return cts;
        }

        public void UnregisterCancellationSource(CancellationTokenSource cts)
        {
            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
            }

            _cancellationSources.Remove(cts);
        }

        public void CancelAllTokens()
        {
            foreach (var tokenSource in _cancellationSources)
            {
                if (!tokenSource.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }
            }

            _cancellationSources.Clear();
        }
    }
}