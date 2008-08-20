using System;
using System.IO;
using Remotion.Text.Diagnostic;
using Remotion.Utilities;


namespace Remotion.Logging
{
  public class SimpleLogger : ISimpleLogger
  {
    private TextWriter textWriter;
    private ToTextProvider toText;

    public SimpleLogger(bool enableConsole)
    {
      if(enableConsole)
      {
        textWriter = TextWriter.Synchronized (Console.Out);
      }
      else
      {
        textWriter = new StreamWriter(Stream.Null);
      }
      InitToTextProvider ();
    }

    public SimpleLogger (string fileName)
    {
      Assertion.IsNotNull(fileName);
      textWriter = TextWriter.Synchronized (new StreamWriter(new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write)));
      InitToTextProvider();
    }

    protected void InitToTextProvider ()
    {
      toText = new ToTextProvider();
      toText.UseAutomaticObjectToText = true;
      toText.UseAutomaticStringEnclosing = true;
      toText.UseAutomaticCharEnclosing = true;
    }

    public void It (object obj)
    {
      textWriter.WriteLine(toText.ToTextString(obj));
    }

    public void It (string s)
    {
      textWriter.WriteLine (s);
    }

    public void It (string format, params object[] parameters)
    {
      textWriter.WriteLine(format, parameters);
    }

    public void Item (object obj)
    {
      textWriter.Write (toText.ToTextString (obj));
    }

    public void Item (string s)
    {
      textWriter.Write (s);
    }

    public void Item (string format, params object[] parameters)
    {
      textWriter.Write (format, parameters);
    }

    //------------------------------------------------------------------------
    // Factories
    //------------------------------------------------------------------------
    public static ISimpleLogger GetLogger (bool enableConsole)
    {
      if (enableConsole)
      {
        return new SimpleLogger (enableConsole);
      }
      else
      {
        return new SimpleLoggerNull ();
      }
    }

    public static ISimpleLogger GetLogger (string fileName, bool enable)
    {
      if (enable)
      {
        return new SimpleLogger (fileName);
      }
      else
      {
        return new SimpleLoggerNull ();
      }
    }
  }
}