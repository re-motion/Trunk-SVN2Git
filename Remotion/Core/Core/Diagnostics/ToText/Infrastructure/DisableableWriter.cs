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
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  /// <summary>
  /// Wrapper around <see cref="TextWriter"/> class which supports enabling/disabling of its <see cref="Write"/> method 
  /// through its <see cref="Enabled"/> property.
  /// </summary>
  public class DisableableWriter : IDisposable
  {
    // private readonly TextWriter _textWriter;

    public DisableableWriter (TextWriter textWriter)
    {
      TextWriter = textWriter;
      Enabled = true;
    }

    public DisableableWriter ()
        : this (new StringWriter ())
    {
    }


    public string DelayedPrefix
    {
      get;
      private set;
    }

    public TextWriter TextWriter
    {
      get;
      private set;
    }

    public bool Enabled { get; set; }


    public TextWriter WriteAlways (object obj)
    {
      if (DelayedPrefix != null)
      {
        TextWriter.Write (DelayedPrefix);
        DelayedPrefix = null;
      }
      TextWriter.Write (obj);

      return TextWriter;
    }


    public TextWriter Write (object obj)
    {
      if (Enabled)
      {
        WriteAlways (obj);
      }
      return TextWriter;
    }

    public override string ToString ()
    {
      return TextWriter.ToString ();
    }

    public void Flush ()
    {
      if (Enabled)
      {
        TextWriter.Flush();
      }
    }

    public void WriteDelayedAsPrefix (string delayedPrefix)
    {
      DelayedPrefix = delayedPrefix;
    }

    public void ClearDelayedPrefix ()
    {
      DelayedPrefix = null;
    }

    public void Close ()
    {
      TextWriter.Close ();
    }

    void IDisposable.Dispose ()
    {
      Close ();
    }
  }
}