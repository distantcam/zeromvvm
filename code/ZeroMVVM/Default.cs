using Conventional.Conventions;
using ZeroMVVM.Conventions;

namespace ZeroMVVM
{
    public static class Default
    {
        static Default()
        {
            ViewConvention = new ViewConvention();
            ViewModelConvention = new ViewModelConvention();
        }

        public static Convention ViewModelConvention { get; set; }

        public static Convention ViewConvention { get; set; }
    }
}