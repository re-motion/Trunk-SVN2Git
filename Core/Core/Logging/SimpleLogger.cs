using System;
using System.IO;
using System.Text;
using Remotion.Text.Diagnostic;
using Remotion.Utilities;


namespace Remotion.Logging
{
  //TODO: Interface-assemblies are to be considered read-only for normal development purposes. They're only contain 'stable' interfaces that should be compatible accross multiple framework versions.
  public class SimpleLogger : ISimpleLogger
  {
    //TODO MGI: Member fields alsways start with an underscore.
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

    //TODO: Check arguments using ArgumentUtility. Assertion is for invariant conditions in code, not for argument checking.
    public SimpleLogger (string fileName)
    {
      Assertion.IsNotNull(fileName);
      //TODO MGI: Is this really threadsafe? -> Is the parallel Execution of FileStream in separate threads a problem?
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

    public void Sequence (params object[] parameters)
    {
      // TODO: Implement AppendSequence in ToTextBuilder
      bool firstArgument = true;
      var sb = new StringBuilder();
      foreach (var obj in parameters)
      {
        if(!firstArgument)
        {
          sb.Append(", ");
        }
        sb.Append(toText.ToTextString(obj));
      }
      textWriter.WriteLine(sb.ToString());
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

    //TODO MGI: Factroy methods are ususally named 'Create'. 'Get' does not create a new isntace each time.
    //TODO MGI: Statics are at the top of the file. See style guide.
    //------------------------------------------------------------------------
    // Factories
    //------------------------------------------------------------------------
    public static ISimpleLogger Get (bool enableConsole)
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

    public static ISimpleLogger Get (string fileName, bool enable)
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