using System.Runtime.CompilerServices;

namespace r_crwUI_A.Interfaces
{
    internal interface ILogger
    {
        void WriteLog(string message, 
            [CallerMemberName] string memberName = null, 
            [CallerFilePath] string sourceFilePath = null, 
            [CallerLineNumber] int sourceLineNumber = 0);
    }
}
