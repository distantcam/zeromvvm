using SimpleExample.UI.Child;
using ZeroMVVM;

namespace SimpleExample.UI.Shell
{
    internal class ShellViewModel : BindableObject
    {
        public ShellViewModel(ChildViewModel child)
        {
            Child = child;
        }

        public ChildViewModel Child { get; set; }
    }
}