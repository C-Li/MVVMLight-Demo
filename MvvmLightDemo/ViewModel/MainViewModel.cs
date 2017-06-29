using System;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;   //最顶层的命名空间，包含了MvvmLight的主体,最核心的功能都在这里。
                            //1.ICleanup            接口。实现该接口的ViewModel需要在Cleanup方法中释放资源，特别是-=event
                            //2.ObservableObject    该类实现了INotifyPropertyChanged接口，定义了一个可通知的对象基类，供ViewModelBase继承
                            //3.ViewModelBase       继承自ObsevableObject,ICleanup。将作为MvvmLight框架下使用的ViewModel的基类。主要提供Set和RaisePropertyChanged供外部使用。同时会在Cleanup方法里，Unregister该实例的所有MvvmLight Messager（在GalaSoft.MvvmLight.Messaging命名空间内定义）

using GalaSoft.MvvmLight.Command;   //1.RelayCommand   提供了一个ICommand接口的实现
                                    //2.RelayCommand<T>   提供了ICommand接口的泛型实现

using GalaSoft.MvvmLight.Messaging; //消息类命名空间，提供全局的消息通知。

using GalaSoft.MvvmLight.Threading; //DispatcherHelper  非UI线程操作UI线程时用到的帮助类，已针对各平台不同的写法做了封装。

using GalaSoft.MvvmLight.Views; //和View结合较紧密，ViewModel通过依赖该命名空间下的类，来避免直接引用View,用以解耦代码对具体的平台的依赖。
                                //1.IDialogService      对系统弹框消息的抽象。针对具体平台会在GalaSoft.MvvmLight.Platform程序集里分别实现
                                //2.INavigationService  对页面导航的抽象，不同平台会有不同实现。

using Microsoft.Practices.ServiceLocation;
using MvvmLightDemo.Model;

namespace MvvmLightDemo.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// （这个类包含可以被main View绑定的属性）
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase  //ViewModelBase继承了INotifyPropertyChanged接口并提供了一个Set方法来给属性赋值。
                                                //（也就是不用自己在VeiwModel实现INotifyProperrtyChanged,然后在属性赋值时通知了，当然也可以手动通知）
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private string _clock = "Starting...";
        private int _counter;
        private RelayCommand _incrementCommand;
        private RelayCommand<string> _navigateCommand;
        private bool _runClock;
        private RelayCommand _sendMessageCommand;
        private RelayCommand _showDialogCommand;
        private string _welcomeTitle = string.Empty;

        public string Clock
        {
            get
            {
                return _clock;
            }
            set
            {
                Set(ref _clock, value);
            }
        }

        /// <summary>
        /// 绑定到了Increment counter按钮的command属性
        /// RelayCommand类继承自ICommand接口，可以直接在XAML中绑定到Command属性。
        /// 
        /// </summary>
        public RelayCommand IncrementCommand
        {
            get
            {
                return _incrementCommand
                    ?? (_incrementCommand = new RelayCommand(   // ?? null合并运算符。 x??y，如果x不为null,则返回x;否则返回y。
                    () =>
                    {
                        WelcomeTitle = string.Format("Counter clicked {0} times", ++_counter);  //计数并显示点击数
                    }
                    ));
            }
        }

        //导航按钮的command属性
        public RelayCommand<string> NavigateCommand
        {
            get
            {
                return _navigateCommand
                       ?? (_navigateCommand = new RelayCommand<string>(
                           p => _navigationService.NavigateTo(ViewModelLocator.SecondPageKey, p),//导航到SecondPage,并把参数p传过去
                                                                                                 //（p参数来自于xaml中绑定到CommandParameter的NavigationParameterText控件中的内容）
                           p => !string.IsNullOrEmpty(p)    //判断参数p是否为空,为空则返回false,则不会进行导航事件，为true则导航
                           ));
            }
        }

        //绑定到了send message按钮的command属性
        public RelayCommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand
                    ?? (_sendMessageCommand = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(
                            //  发送通知“Testing”，并接收返回数据并显示
                            new NotificationMessageAction<string>(
                                "Testing",      //通知内容
                                reply =>
                                {
                                    WelcomeTitle = reply;   //显示返回值
                                }));
                    }));
            }
        }

        //绑定到了show a dialog按钮的command属性
        public RelayCommand ShowDialogCommand
        {
            get
            {
                return _showDialogCommand
                       ?? (_showDialogCommand = new RelayCommand(
                           async () =>
                           {
                               var dialog = ServiceLocator.Current.GetInstance<IDialogService>();       //获取dialog实例
                               await dialog.ShowMessage("Hello Universal Application", "it works...");  //显示dialog
                           }));
            }
        }

        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public MainViewModel(
            IDataService dataService,
            INavigationService navigationService)   //通过构造参数匹配SimpleIoc中注册的类，获取类的实例（依赖注入）
        {
            _dataService = dataService;
            _navigationService = navigationService;
            Initialize();
        }

        //开始时间更新
        public void RunClock()
        {
            _runClock = true;

            //异步执行更新页面时间的代码
            Task.Run(async () =>
            {
                while (_runClock)
                {
                    try
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Clock = DateTime.Now.ToString("HH:mm:ss");
                        });

                        await Task.Delay(1000);
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        //停止时间更新
        public void StopClock()
        {
            _runClock = false;
        }

        //实例化
        private async Task Initialize()
        {
            try
            {
                var item = await _dataService.GetData();
                WelcomeTitle = item.Title;
            }
            catch (Exception ex)
            {
                // Report error here
                WelcomeTitle = ex.Message;
            }
        }
    }
}