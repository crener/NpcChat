using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using NpcChatSystem;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;

namespace NpcChat.Converters
{
    public class TreeBranchNameConverter : MarkupExtension, IValueConverter
    {
        public NpcChatProject Project { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DialogTreeBranchIdentifier id)
            {
                if (Project == null) return "No Project Set";
                return Project[id]?.Name ?? "Branch Not Found";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
