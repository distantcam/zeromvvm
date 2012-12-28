using ZeroMVVM;

namespace SimpleExample.UI.Shell
{
    internal class ShellAttachment : Attachment<ShellViewModel>
    {
        protected override void OnAttach()
        {
            viewModel.AttachmentWorking = true;
        }
    }
}