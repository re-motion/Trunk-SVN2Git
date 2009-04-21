// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;
using Remotion.Diagnostics.ToText;
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
        return new SimpleLogger (Console.Out, To.ToTextProvider);
      else
        return new SimpleLoggerNull();
    }

    public static ISimpleLogger CreateForFile (string fileName, bool enable)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);
      if (enable)
        return new SimpleLogger (new StreamWriter (new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write)), To.ToTextProvider);
      else
        return new SimpleLoggerNull();
    }

    private readonly TextWriter _textWriter;
    private readonly ToTextProvider _toText;

    private SimpleLogger (TextWriter textWriter, ToTextProvider toTextProvider)
    {
      // Ensure that usage of the SimpleLogger from different threads is synchronized.
      _textWriter = TextWriter.Synchronized (textWriter);

      _toText = toTextProvider;
      if (_toText == null)
      {
        _toText = new ToTextProvider();
      }
      _toText.Settings.UseAutomaticObjectToText = false;
      _toText.Settings.UseAutomaticStringEnclosing = true;
      _toText.Settings.UseAutomaticCharEnclosing = true;
    }

    //TODO: rename
    public void It (object obj)
    {
      //_textWriter.WriteLine (_toText.ToTextString (obj));
      _textWriter.WriteLine (obj);
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
