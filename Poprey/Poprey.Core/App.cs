using Acr.UserDialogs;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;
using Poprey.Core.ViewModels;

namespace Poprey.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterDependencies();

            // register the appstart object
            RegisterCustomAppStart<AppStart>();
        }

        private void RegisterDependencies()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            CreatableTypes()
                .EndingWith("Client")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            CreatableTypes()
                .EndingWith("ViewModel")
                .AsTypes()
                .RegisterAsDynamic();

            CreatableTypes()
                .EndingWith("Singleton")
                .AsTypes()
                .RegisterAsLazySingleton();

          
            Mvx.IoCProvider.RegisterSingleton<IUserDialogs>(() => UserDialogs.Instance);
            Mvx.IoCProvider.RegisterType<MessageTokenHelper>();

            CreatableTypes()
                .EndingWith("Singleton")
                .AsTypes()
                .RegisterAsLazySingleton();

        }
    }
}
