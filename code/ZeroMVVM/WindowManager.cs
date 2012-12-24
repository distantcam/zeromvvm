﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ZeroMVVM
{
    public interface IWindowManager
    {
        bool? ShowDialog(object viewModel);

        void ShowWindow(object viewModel);
    }

    public class WindowManager : IWindowManager
    {
        public virtual bool? ShowDialog(object viewModel)
        {
            return CreateWindow(viewModel, true).ShowDialog();
        }

        public virtual void ShowWindow(object viewModel)
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

        protected virtual Window CreateWindow(object viewModel, bool isDialog)
        {
            var viewType = AppRunner.ConventionManager.FindAll(Default.ViewConvention, viewModel.GetType()).Single();

            var view = EnsureWindow(viewModel, Default.GetInstance(viewType), isDialog);

            view.DataContext = viewModel;

            return view;
        }

        protected virtual Window EnsureWindow(object viewModel, object view, bool isDialog)
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

        protected virtual Window InferOwnerOf(Window window)
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

        protected virtual Page CreatePage(object viewModel)
        {
            var viewType = AppRunner.ConventionManager.FindAll(Default.ViewConvention, viewModel.GetType()).Single();

            var view = EnsurePage(viewModel, Default.GetInstance(viewType));

            view.DataContext = viewModel;

            return view;
        }

        protected virtual Page EnsurePage(object model, object view)
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