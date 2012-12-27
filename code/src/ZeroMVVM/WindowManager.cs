using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ZeroMVVM
{
    public static class WindowManager
    {
        public static bool? ShowDialog(object viewModel)
        {
            return CreateWindow(viewModel, true).ShowDialog();
        }

        public static void ShowWindow(object viewModel)
        {
            NavigationWindow navWindow = null;

            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                navWindow = Application.Current.MainWindow as NavigationWindow;
            }

            if (navWindow != null)
            {
                var window = CreatePage(viewModel);
                navWindow.Navigate(window);
            }
            else
            {
                CreateWindow(viewModel, false).Show();
            }
        }

        private static Window CreateWindow(object viewModel, bool isDialog)
        {
            var viewType = ZAppRunner.ConventionManager.FindAll(Default.ViewConvention, viewModel.GetType()).Single();

            var view = EnsureWindow(viewModel, ZAppRunner.GetInstance(viewType), isDialog);

            ViewModelBinder.Bind(view, viewModel);

            return view;
        }

        private static Window EnsureWindow(object viewModel, object view, bool isDialog)
        {
            var window = view as Window;

            if (window == null)
            {
                window = new Window
                {
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight
                };

                var owner = InferOwnerOf(window);
                if (owner != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = owner;
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            else
            {
                var owner = InferOwnerOf(window);
                if (owner != null && isDialog)
                {
                    window.Owner = owner;
                }
            }

            return window;
        }

        private static Window InferOwnerOf(Window window)
        {
            if (Application.Current == null)
            {
                return null;
            }

            var active = Application.Current.Windows.OfType<Window>()
                .Where(x => x.IsActive)
                .FirstOrDefault();
            active = active ?? Application.Current.MainWindow;
            return active == window ? null : active;
        }

        private static Page CreatePage(object viewModel)
        {
            var viewType = ZAppRunner.ConventionManager.FindAll(Default.ViewConvention, viewModel.GetType()).Single();

            var view = EnsurePage(viewModel, ZAppRunner.GetInstance(viewType));

            view.DataContext = viewModel;

            return view;
        }

        private static Page EnsurePage(object model, object view)
        {
            var page = view as Page;

            if (page == null)
            {
                page = new Page { Content = view };
            }

            return page;
        }
    }
}