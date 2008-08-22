using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Remotion.Logging;

namespace Remotion.Text.Diagnostic
{
  public static class To
  {
    private static readonly SimpleLogger log = new SimpleLogger (true);
    private static readonly ToTextProvider toTextProvider = new ToTextProvider();

    //private static To() 

    public static string Text (object obj)
    {
      return toTextProvider.ToTextString (obj);
    }


    public static void RegisterHandler<T> (Action<T, ToTextBuilder> handler)
    {
      toTextProvider.RegisterHandler<T>(handler); 
    }

    public static void ClearHandlers ()
    {
      toTextProvider.ClearHandlers ();
    }


    public static void TextEnableAutomatics (bool enable)
    {
      toTextProvider.UseAutomaticObjectToText = enable;
      toTextProvider.UseAutomaticStringEnclosing = enable;
      toTextProvider.UseAutomaticCharEnclosing = enable;
    }

    private static void Log (string s)
    {
      //Console.WriteLine ("[To]: " + s);
      log.It("[To]: " + s);
    }

    private static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }


  }
}