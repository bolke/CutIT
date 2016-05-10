using CutIT.GRBL;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace CutITGui.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<TcpGrblClient>();
            SimpleIoc.Default.Register<GrblSettings>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ActiveStatusViewModel>();
            SimpleIoc.Default.Register<ConsoleViewModel>();
            SimpleIoc.Default.Register<ConnectViewModel>();
            SimpleIoc.Default.Register<ScreenTabViewModel>();
            SimpleIoc.Default.Register<FileViewModel>();
            SimpleIoc.Default.Register<MessageViewModel>(true);
        }
        
        public static ConsoleViewModel ConsoleViewModel
        {
            get { return SimpleIoc.Default.GetInstance<ConsoleViewModel>(); }
        }

        public static TcpGrblClient TcpGrblClient
        {
            get { return SimpleIoc.Default.GetInstance<TcpGrblClient>(); }
        }

        public static ConnectViewModel ConnectViewModel
        {
            get { return SimpleIoc.Default.GetInstance<ConnectViewModel>(); }
        }

        public static GrblSettings GrblSettings
        {
            get { return SimpleIoc.Default.GetInstance<GrblSettings>(); }
        }

        public static ActiveStatusViewModel ActiveStatusViewModel
        {
            get { return SimpleIoc.Default.GetInstance<ActiveStatusViewModel>(); }
        }

        public static MessageViewModel MessageViewModel
        {
            get { return SimpleIoc.Default.GetInstance<MessageViewModel>(); }
        }

        public static ScreenTabViewModel ScreenTabViewModel
        {
            get { return SimpleIoc.Default.GetInstance<ScreenTabViewModel>(); }
        }

        public static FileViewModel FileViewModel
        {
            get { return SimpleIoc.Default.GetInstance<FileViewModel>(); }
        }
        public static void Cleanup()
        {
        }
    }
}