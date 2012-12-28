using SimpleExample.UI.Child;
using ZeroMVVM;

namespace SimpleExample.UI.Shell
{
    internal class ShellViewModel : BindableObject
    {
        public ShellViewModel(ChildViewModel child)
        {
            Child = child;
            ViewModelWorking = true;
        }

        public ChildViewModel Child { get; set; }

        public bool AttachmentWorking { get; set; }

        public bool ViewModelWorking { get; set; }
    }
}