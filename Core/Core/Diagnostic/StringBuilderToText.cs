/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Text;

namespace Remotion.Diagnostic
{
  /// <summary>
  /// StringBuilder-like class which supports enabling/disabling of its <see cref="Append{T}"/> method 
  /// through its <see cref="Enabled"/> property.
  /// </summary>
  internal class StringBuilderToText
  {
    private readonly StringBuilder _stringBuilder = new StringBuilder ();

    public StringBuilderToText()
    {
      Enabled = true;
    }

    public bool Enabled { get; set; }

    public StringBuilder Append<T> (T t)
    {
      if (Enabled)
      {
        _stringBuilder.Append (t);
      }
      return _stringBuilder;
    }

    public override string ToString ()
    {
      return _stringBuilder.ToString ();
    }
  }
}