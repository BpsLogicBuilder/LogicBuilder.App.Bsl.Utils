using LogicBuilder.App.Bsl.Utils.Interfaces;
using LogicBuilder.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace LogicBuilder.App.Bsl.Utils
{
    [ExcludeFromCodeCoverage]
    public static class CustomActionUtils
    {
        [AlsoKnownAs("WriteToLog")]
        public static void WriteToLog(ICustomActions customActions, string message)
        {
            customActions.WriteToLog(message);
        }
    }
}
