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
using Remotion.Diagnostics.ToText.Handlers;
using Remotion.Utilities;


namespace Remotion.Development.UnitTesting.Logging
{
  public class SimpleLogger : ISimpleLogger
  {
    //------------------------------------------------------------------------
    // Factories
    //------------------------------------------------------------------------
    public static ISimpleLogger CreateForConsole (bool enableConsole)
    {
      if (enableConsole)
        return new SimpleLogger (Console.Out);
      else
        return new SimpleLoggerNull();
    }

    public static ISimpleLogger CreateForFile (string fileName, bool enable)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);
      if (enable)
        return new SimpleLogger (new StreamWriter (new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write)));
      else
        return new SimpleLoggerNull();
    }

    private readonly TextWriter _textWriter;
    private readonly ToTextProvider _toText;

    private SimpleLogger (TextWriter textWriter)
    {
      // Ensure that usage of the SimpleLogger from different threads is synchronized.
      _textWriter = TextWriter.Synchronized (textWriter);

      _toText = new ToTextProvider();
      _toText.Settings.UseAutomaticObjectToText = true;
      _toText.Settings.UseAutomaticStringEnclosing = true;
      _toText.Settings.UseAutomaticCharEnclosing = true;
    }

    //TODO: rename
    public void It (object obj)
    {
      _textWriter.WriteLine (_toText.ToTextString (obj));
    }

    public void It (string s)
    {
      _textWriter.WriteLine (s);
    }

    public void It (string format, params object[] parameters)
    {
      _textWriter.WriteLine (format, parameters);
    }

    //TODO: rename
    public void Sequence (params object[] parameters)
    {
      _textWriter.WriteLine (StringUtility.Concat (parameters));
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