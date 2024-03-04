using Inventory.Scripts.Core.Enums;

namespace Inventory.Scripts.Core.Helper
{
    public static class UgiLogsHelper
    {
        public static string Info(this string message)
        {
            return $"<color=#f4f2f4>[{UgiLogType.Info.ToString().ToUpper()}]</color> - {message}";
        }

        public static string Error(this string message)
        {
            return $"<color=#ef3eb9>[{UgiLogType.Configuration.ToString().ToUpper()}]</color> - {message}";
        }

        public static string Configuration(this string message)
        {
            return $"<color=#ffc400>[{UgiLogType.Configuration.ToString().ToUpper()}]</color> - {message}";
        }

        public static string Settings(this string message)
        {
            return $"<color=#1b1b1b>[{UgiLogType.Settings.ToString().ToUpper()}]</color> - {message}";
        }

        public static string Broadcasting(this string message)
        {
            return $"<color=#ef3eb9>[{UgiLogType.Broadcasting.ToString().ToUpper()}]</color> - {message}";
        }

        public static string Editor(this string message)
        {
            return $"<color=#f1942b>[{UgiLogType.Editor.ToString().ToUpper()}]</color> - {message}";
        }

        public static string DraggableSystem(this string message)
        {
            return $"<color=#f59749>[{UgiLogType.DraggableSystem.ToString().ToUpper()}]</color> - {message}";
        }
    }
}