using System;
using System.Threading;
using System.Threading.Tasks;


namespace Poprey.Core.Util
{
    internal sealed class Timer : CancellationTokenSource, IDisposable
    {
        internal Timer(Action callback, int milliseconds)
        {
            Task.Run(async () =>
                {
                    await Task.Delay(milliseconds);

                    if(!IsCancellationRequested)
                    {
                        callback?.Invoke();
                    }
                }
            );
        }

        public new void Dispose() { Cancel(); }
    }
}
