using System.Diagnostics;
using ZeroMVVM;

namespace SimpleExample.UI.Shell
{
    internal class ShellAttachment : Attachment<ShellViewModel>
    {
        protected override void OnAttach()
        {
            Debug.WriteLine("[TESTING] Attached to " + viewModel);
        }
    }

    internal class ShellViewModel : BindableObject
    {
    }
}