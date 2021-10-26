using MvvmCross;
using MvvmCross.Logging;

namespace Poprey.Core
{
    public static class Logs
    {
        public static IMvxLog Instance { get; } = Mvx.IoCProvider.Resolve<IMvxLogProvider>().GetLogFor("Poprey");
    }
}
