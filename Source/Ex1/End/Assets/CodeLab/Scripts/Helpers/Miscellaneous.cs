using UnityEngine;
using System.Collections;

public class LogListener
{
    static LogListener ()
    {
#if NETFX_CORE
        if ( System.Diagnostics.Debugger.IsAttached )
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }
#endif 
    }

    private static void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
#if NETFX_CORE 
        System.Diagnostics.Debug.WriteLine(condition);
        if (type != LogType.Log && type != LogType.Warning )
            System.Diagnostics.Debug.WriteLine(stackTrace); 
#endif 
    }
}
