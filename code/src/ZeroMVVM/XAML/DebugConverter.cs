using System;
using System.Diagnostics;
using System.Globalization;

namespace ZeroMVVM.XAML
{
    public class DebugConverter : MarkupConverter
    {
        public bool BreakOnConverter { get; set; }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (BreakOnConverter && Debugger.IsAttached)
                Debugger.Break();

            Debug.WriteLine("[DEBUGCONVERTER Convert] Value      = " + value);
            Debug.WriteLine("[DEBUGCONVERTER Convert] TargetType = " + targetType);
            Debug.WriteLine("[DEBUGCONVERTER Convert] Parameter  = " + parameter);
            Debug.WriteLine("[DEBUGCONVERTER Convert] Culture    = " + culture);

            return value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (BreakOnConverter && Debugger.IsAttached)
                Debugger.Break();

            Debug.WriteLine("[DEBUGCONVERTER ConvertBack] Value      = " + value);
            Debug.WriteLine("[DEBUGCONVERTER ConvertBack] TargetType = " + targetType);
            Debug.WriteLine("[DEBUGCONVERTER ConvertBack] Parameter  = " + parameter);
            Debug.WriteLine("[DEBUGCONVERTER ConvertBack] Culture    = " + culture);

            return value;
        }
    }
}