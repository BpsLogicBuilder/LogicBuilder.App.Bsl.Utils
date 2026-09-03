using LogicBuilder.Attributes;

namespace LogicBuilder.App.Bsl.Utils.Interfaces
{
    public interface ICustomActions
    {
        [AlsoKnownAs("WriteToLog")]
        void WriteToLog(string message);
    }
}
