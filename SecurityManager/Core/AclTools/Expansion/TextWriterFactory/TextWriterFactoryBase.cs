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
    private readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData> ();
    public abstract TextWriter NewTextWriter (string name);
    public string Directory { get; set; }
    public string Extension { get; set; }

    public Dictionary<string, TextWriterData> NameToTextWriterData
    {
      get { return nameToTextWriterData; }
    }


    protected string AppendExtension(string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      if (String.IsNullOrEmpty (Extension))
      {
        return name;
      }
      else
      {
        return name + "." + Extension;
      }
    }


    public string GetRelativePath (string fromName, string toName)
    {
      ArgumentUtility.CheckNotNull ("fromName", fromName);
      ArgumentUtility.CheckNotNull ("toName", toName);
      if (!nameToTextWriterData.ContainsKey (toName))
      {
        throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" registered => no relative path exists.").CheckAndConvertToString ());
      }
      return ".\\" + AppendExtension(toName);
    }

    public TextWriterData GetTextWriterData (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      return nameToTextWriterData[name];
    }

    public int Count { get { return nameToTextWriterData.Count; } }


  }
}