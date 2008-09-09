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

namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// Wrapper around <see cref="TextWriter"/> class which supports enabling/disabling of its <see cref="Write{T}"/> method 
  /// through its <see cref="Enabled"/> property.
  /// </summary>
  public class DisableableWriter
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



    public TextWriter Write<T> (T t)
    {
      //Console.WriteLine ("TextWriter: Enabled=" + Enabled);
      if (Enabled)
      {
        if (DelayedPrefix != null)
        {
          TextWriter.Write (DelayedPrefix);
        }
        TextWriter.Write (t);
        //Console.WriteLine ("Wrote to: " + TextWriter.GetHashCode ());
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


  }
}