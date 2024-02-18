using HandyControl.Controls;
using QuickCAD1.VModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickCAD1.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        /// <summary>
        /// 是否拖动中
        /// </summary>
        private bool isDragging;
        private Point startPoint; 
        /// <summary>
        /// 是否全屏
        /// </summary>
        private bool isFullCreen { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public CurrentTimeVMod TimeModel { get; set; }
        /// <summary>
        /// 菜单项
        /// </summary>
        public ObservableCollection<MenuItemVMod> MenuItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            TimeModel = new CurrentTimeVMod();
            #region 定时更新当前时间
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                TimeModel.UpdateTime();
            };
            timer.Interval = new System.TimeSpan(0, 0, 1);
            timer.Start();
            #endregion

            MenuItems = new ObservableCollection<MenuItemVMod>
            {
                new MenuItemVMod { Header = "首页",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/首页.png" },
                new MenuItemVMod { Header = "手动任务" ,IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"},
                new MenuItemVMod { Header = "资源管理",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png",
                    Children = new ObservableCollection<MenuItemVMod>
                    {
                        new MenuItemVMod { Header = "资源1",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png" },
                        new MenuItemVMod { Header = "资源2",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png",
                            Children = new ObservableCollection<MenuItemVMod>
                            {
                                new MenuItemVMod { Header = "资源1" , IconUrl = "pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"},
                                new MenuItemVMod { Header = "资源2" ,IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"}
                            }
                        }
                    }
                },
                new MenuItemVMod { Header = "资源管理",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png",
                    Children = new ObservableCollection<MenuItemVMod>
                    {
                        new MenuItemVMod { Header = "资源1",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png" },
                        new MenuItemVMod { Header = "资源2",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png",
                            Children = new ObservableCollection<MenuItemVMod>
                            {
                                new MenuItemVMod { Header = "资源1" , IconUrl = "pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"},
                                new MenuItemVMod { Header = "资源2" ,IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"}
                            }
                        }
                    }
                },
               new MenuItemVMod { Header = "资源管理",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png",
                    Children = new ObservableCollection<MenuItemVMod>
                    {
                        new MenuItemVMod { Header = "资源1",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png" },
                        new MenuItemVMod { Header = "资源2",IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png",
                            Children = new ObservableCollection<MenuItemVMod>
                            {
                                new MenuItemVMod { Header = "资源1" , IconUrl = "pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"},
                                new MenuItemVMod { Header = "资源2" ,IconUrl="pack://application:,,,/QuickCAD1;component/Resources/Img/手动任务.png"}
                            }
                        }
                    }
                }
            };

            DataContext = this;
        }

        // 鼠标按下事件
        private void MenuBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            startPoint = e.GetPosition(this);
        }

        // 鼠标移动事件
        private void MenuBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newPoint = e.GetPosition(this);
                double deltaX = newPoint.X - startPoint.X;
                double deltaY = newPoint.Y - startPoint.Y;

                // 更新窗口位置
                Left += deltaX;
                Top += deltaY;

                startPoint = newPoint;
            }
        }

        // 鼠标松开事件
        private void MenuBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        /// <summary>
        /// 全屏切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullCreen_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            isFullCreen = !isFullCreen;
            BitmapImage currentImage = isFullCreen ? (BitmapImage)FindResource("CancelFullScreenImg") : (BitmapImage)FindResource("FullScreenImg");
            WindowState = isFullCreen ? WindowState.Maximized: WindowState.Normal;
            //this.FullCreen_Image.Source = currentImage;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
