[assembly: ReleaseDateAttribute("2015-03-02", "2015-03-02")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.5", FrameworkDisplayName=".NET Framework 4.5")]
[assembly: System.Windows.Markup.XmlnsDefinitionAttribute("http://zeromvvm.github.com/", "ZeroMVVM.XAML")]

namespace ZeroMVVM
{
    
    public abstract class Attachment<T> : ZeroMVVM.IAttachment
    
    {
        protected T viewModel;
        protected Attachment() { }
        protected abstract void OnAttach();
    }
    public class BindableObject : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging
    {
        public BindableObject() { }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
        protected internal void OnPropertyChanged(string name) { }
        protected internal void OnPropertyChanging(string name) { }
    }
    public interface IAttachment
    {
        void AttachTo(object obj);
    }
    public interface IContainer
    {
        object GetInstance(System.Type type);
        void Setup(System.Collections.Generic.IEnumerable<System.Type> typesToRegister, System.Collections.Generic.IEnumerable<System.Type> viewModelTypesToRegister);
    }
    public class Logger
    {
        public Logger() { }
        public virtual void Debug(string message) { }
        public virtual void Error(string message) { }
        public virtual void Info(string message) { }
        public virtual void Warn(string message) { }
    }
    public class ViewModel : ZeroMVVM.BindableObject
    {
        public ViewModel() { }
        public virtual bool CanClose() { }
        public virtual System.Threading.Tasks.Task<bool> CanCloseAsync() { }
        public virtual void TryClose() { }
    }
    public class static ViewModelBinder
    {
        public static void Bind(System.Windows.FrameworkElement view, object viewModel) { }
    }
    public class static WindowManager
    {
        public static System.Nullable<bool> ShowDialog(object viewModel) { }
        public static void ShowWindow(object viewModel) { }
    }
    public class static ZAppRunner
    {
        public static Conventional.IConventionManager ConventionManager { get; }
        public static ZeroMVVM.Logger GetLogger<T>() { }
        public static ZeroMVVM.Logger GetLogger(System.Type type) { }
        public static ZeroMVVM.Logger GetLogger(string name) { }
        public static void Start<T>() { }
        public class static Default
        {
            public static System.Type AttachmentConvention { get; set; }
            [System.Runtime.CompilerServices.DynamicAttribute()]
            public static object IoC { get; set; }
            [System.Runtime.CompilerServices.DynamicAttribute(new bool[] {
                    false,
                    false,
                    true})]
            public static System.Func<string, object> Logger { get; set; }
            public static System.Type ViewConvention { get; set; }
            public static System.Type ViewModelConvention { get; set; }
        }
    }
}
namespace ZeroMVVM.Conventions
{
    
    public class AttachmentConvention : Conventional.Conventions.Convention
    {
        public AttachmentConvention() { }
    }
    public class ViewConvention : Conventional.Conventions.Convention
    {
        public ViewConvention() { }
    }
    public class ViewModelConvention : Conventional.Conventions.Convention
    {
        public ViewModelConvention() { }
    }
}
namespace ZeroMVVM.Dynamic
{
    
    public class AutofacRegistrationHelper : System.Dynamic.IDynamicMetaObjectProvider
    {
        public AutofacRegistrationHelper(object instance) { }
    }
    public class StaticMembersDynamicWrapper : System.Dynamic.IDynamicMetaObjectProvider
    {
        public StaticMembersDynamicWrapper(System.Type type) { }
    }
}
namespace ZeroMVVM.XAML
{
    
    public class BoolToVisibilityConverter : ZeroMVVM.XAML.MarkupConverter
    {
        public BoolToVisibilityConverter() { }
        public bool Invert { get; set; }
        protected override object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
    }
    public class DebugConverter : ZeroMVVM.XAML.MarkupConverter
    {
        public DebugConverter() { }
        public bool BreakOnConverter { get; set; }
        protected override object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
    }
    [System.Windows.Markup.MarkupExtensionReturnTypeAttribute(typeof(System.Windows.Data.IValueConverter))]
    public abstract class MarkupConverter : System.Windows.Markup.MarkupExtension, System.Windows.Data.IValueConverter
    {
        protected MarkupConverter() { }
        protected abstract object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture);
        protected abstract object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture);
        public override object ProvideValue(System.IServiceProvider serviceProvider) { }
    }
    public class NullToVisibilityConverter : ZeroMVVM.XAML.MarkupConverter
    {
        public NullToVisibilityConverter() { }
        public bool Invert { get; set; }
        protected override object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        protected override object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
    }
}