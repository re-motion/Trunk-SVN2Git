/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Text;
using Remotion.Diagnostic;
using Remotion.Utilities;


namespace Remotion.Logging
{
  public class SimpleLogger : ISimpleLogger
  {
    private TextWriter _textWriter;
    private ToTextProvider _toText;


    //------------------------------------------------------------------------
    // Factories
    //------------------------------------------------------------------------
    public static ISimpleLogger Create (bool enableConsole)
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

    public static ISimpleLogger Create (string fileName, bool enable)
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


    public SimpleLogger(bool enableConsole)
    {
      if(enableConsole)
      {
        _textWriter = TextWriter.Synchronized (Console.Out);
      }
      else
      {
        _textWriter = new StreamWriter(Stream.Null);
      }
      InitToTextProvider ();
    }

    public SimpleLogger (string fileName)
    {
      ArgumentUtility.CheckNotNull ("fileName",fileName);
      // Ensure that usage of the SimpleLogger from different threads is synchronized.
      _textWriter = TextWriter.Synchronized (new StreamWriter(new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write)));
      InitToTextProvider();
    }

    protected void InitToTextProvider ()
    {
      _toText = new ToTextProvider();
      _toText.UseAutomaticObjectToText = true;
      _toText.UseAutomaticStringEnclosing = true;
      _toText.UseAutomaticCharEnclosing = true;
    }

    //TODO: rename
    public void It (object obj)
    {
      _textWriter.WriteLine(_toText.ToTextString(obj));
    }

    public void It (string s)
    {
      _textWriter.WriteLine (s);
    }

    public void It (string format, params object[] parameters)
    {
      _textWriter.WriteLine(format, parameters);
    }

    //TODO: rename
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
        sb.Append(_toText.ToTextString(obj));
      }
      _textWriter.WriteLine(sb.ToString());
    }

    //TODO: rename
    public void Item (object obj)
    {
      _textWriter.Write (_toText.ToTextString (obj));
    }

    public void Item (string s)
    {
      _textWriter.Write (s);
    }


    public void Item (string format, params object[] parameters)
    {
      _textWriter.Write (format, parameters);
    }


    bool INullObject.IsNull
    {
      get { return false; }
    }


  }
}