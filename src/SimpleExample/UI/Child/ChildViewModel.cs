using ZeroMVVM;

namespace SimpleExample.UI.Child
{
    internal class ChildViewModel : BindableObject
    {
        public ChildViewModel()
        {
            Message = "Working";
        }

        public string Message { get; set; }
    }
}