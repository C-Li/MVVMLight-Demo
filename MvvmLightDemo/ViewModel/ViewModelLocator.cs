using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MvvmLightDemo.Model;

namespace MvvmLightDemo.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// （这个类包含了这个应用中所有ViewModel的静态引用并提供了绑定的进入点。）
    /// 
    /// App.xaml中将ViewModelLocator作为资源添加到了全局的Application.Resources里
    /// 
    /// 在这个类中，完成了为View创建ViewModel示例的工作。
    /// 
    /// 使用ViewModelLocator的好处：
    /// 1.View和ViewModel之间不再直接引用，而是通过ViewModelLocator关联。
    /// 2.储存在ViewModelLocator里的ViewModel类似与单例的存在，可以在全局引用绑定。
    /// 3.避免了某些情况下频繁创建ViewModel，却未做好资源释放造成的内存泄漏。（并不是说所有ViewModel都必须放到ViewModelLocator里）
    /// 
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        public const string SecondPageKey = "SecondPage";   //MainViewModel中导航到第二页的时候用到

        /// <summary>
        /// This property can be used to force the application to run with design time data.
        /// （这个属性能被用来强制应用和设计时数据一起运行）
        /// </summary>
        public static bool UseDesignTimeData
        {
            get
            {
                return false;
            }
        }

        static ViewModelLocator()
        {
            //
            //SimpleIoc:    一个非常简单的依赖注入容器。 Ioc（控制反转）
            //SimpleIoc.Default：    SimpleIoc的默认实例

            //使用方法：
            //1.将自己的类注册到SimpleIoc
            //  SimpleIoc.Default.Register(()=>new MyClass());
            //
            //2.把MainViewModel也注册到SimpleIoc
            //  SimpleIoc.Default.Register<MainViewModel>();
            //
            //3.在Main属性中通过ServiceLocator.Current.GetInstance()方法获取实例
            //  public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
            //
            //4.在MainViewModel的构造函数中匹配MyClass类
            //  public MainViewModel(MyClass data)
            //  {
            //      WelcomeTitle = data.Name;
            //  }

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);     //设置用来检索当前实例的委托

            var nav = new NavigationService();
            nav.Configure(SecondPageKey, typeof(SecondPage));               //添加一个"键/页对"到导航服务中
            SimpleIoc.Default.Register<INavigationService>(() => nav);      //注册一个NavigationService实例（MainViewModel构造参数中匹配类型获取这个实例）

            SimpleIoc.Default.Register<IDialogService, DialogService>();    //注册DialogService实例（MainViewModel构造参数中匹配类型获取这个实例）

            if (ViewModelBase.IsInDesignModeStatic
                    || UseDesignTimeData)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();   //注册DesignDataService实例（设计时（设计窗口里的界面））
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();    //注册DataService实例（运行时）
            }

            SimpleIoc.Default.Register<MainViewModel>();    //注册MainViewModel
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();   //获取MainViewModel的实例(依赖注入)
        //上一行等效于：
        //public MainViewModel Main
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<MainViewModel>();
        //    }
        //}
        
        
        
        
    }
}
