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
using System.Collections.Generic;
using System.IO;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  public abstract class TextWriterFactoryBase : ITextWriterFactory
  {
    private readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData> (); // TODO AE: _

    public abstract TextWriter CreateTextWriter (string directory, string name, string extension);
    public abstract TextWriter CreateTextWriter (string name);
    public string Directory { get; set; }
    public string Extension { get; set; }

    public Dictionary<string, TextWriterData> NameToTextWriterData
    {
      get { return nameToTextWriterData; }
    }


    public static string AppendExtension(string name, string extension)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      if (String.IsNullOrEmpty (extension))
      {
        return name;
      }
      else
      {
        return name + "." + extension;
      }
    }

    // TODO AE: Test case where no writer with name toName is found.
    // TODO AE: fromName parameter is not used
    public string GetRelativePath (string fromName, string toName)
    {
      ArgumentUtility.CheckNotNull ("fromName", fromName);
      ArgumentUtility.CheckNotNull ("toName", toName);
      if (!nameToTextWriterData.ContainsKey (toName))
      {
        throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" registered => no relative path exists.").CheckAndConvertToString ());
      }
      return ".\\" + AppendExtension(toName, Extension); // TODO: Implement in more robust manner
    }

    // TODO AE: Note: Test-only member.
    public TextWriterData GetTextWriterData (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      return nameToTextWriterData[name];
    }

    // TODO AE: Note: Test-only member.
    public int Count { get { return nameToTextWriterData.Count; } }


  }
}