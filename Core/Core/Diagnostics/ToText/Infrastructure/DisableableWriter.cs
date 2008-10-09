// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
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

      if (obj.Equals ("Enabled|Remotion.SecurityManager.Domain.OrganizationalStructure.Delegation, Remotion.SecurityManager"))
      {
        Debugger.Break(); // ("found Remotion.SecurityManager.Domain.OrganizationalStructure.OrganizationalStructureObject, Remotion.SecurityManager");
      }

      return TextWriter;
    }


    public TextWriter Write (object obj)
    {
      if (Enabled)
      {
        //if (DelayedPrefix != null)
        //{
        //  TextWriter.Write (DelayedPrefix);
        //  DelayedPrefix = null;
        //}
        //TextWriter.Write (obj);
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