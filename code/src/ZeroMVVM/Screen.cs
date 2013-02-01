using System;
using System.Threading.Tasks;

namespace ZeroMVVM
{
    public class ViewModel : BindableObject
    {
        private static readonly Logger Log = ZAppRunner.GetLogger<ViewModel>();

        public virtual void TryClose()
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> CanCloseAsync()
        {
            return Task.Factory.StartNew<bool>(CanClose);
        }

        public virtual bool CanClose()
        {
            return true;
        }
    }
}