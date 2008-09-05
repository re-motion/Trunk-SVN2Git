/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.IO;
using System.Text;

namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// StringBuilder-like class which supports enabling/disabling of its <see cref="Write{T}"/> method 
  /// through its <see cref="Enabled"/> property.
  /// </summary>
  internal class DisableableWriter
  {
    //private readonly StringBuilder _stringBuilder = new StringBuilder ();
    private readonly TextWriter _textWriter;


    public DisableableWriter (TextWriter textWriter)
    {
      _textWriter = textWriter;
      Enabled = true;
    }

    public DisableableWriter ()
      : this (new StringWriter ())
    {
    }

    public bool Enabled { get; set; }

    public TextWriter Write<T> (T t)
    {
      if (Enabled)
      {
        _textWriter.Write (t);
      }
      return _textWriter;
    }

    public override string ToString ()
    {
      return _textWriter.ToString ();
    }
  }
}