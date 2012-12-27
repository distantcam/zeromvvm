using System.Windows;

namespace ZeroMVVM
{
    public static class ViewModelBinder
    {
        private static Logger Log = ZAppRunner.GetLogger(typeof(ViewModelBinder));

        public static void Bind(FrameworkElement view, object viewModel)
        {
            Log.Info(string.Format("Binding {0} to {1}.", view, viewModel));

            view.DataContext = viewModel;
        }
    }
}