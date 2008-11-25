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
  public class StreamWriterFactory : TextWriterFactoryBase, ITextWriterFactory, IToText
  {
    public override TextWriter NewTextWriter (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      Assertion.IsNotNull (Directory, "Directory must not be null. Set using \"Directory\"-property before calling \"NewTextWriter\"");
      if (!System.IO.Directory.Exists (Directory))
      {
        System.IO.Directory.CreateDirectory (Directory);
      }

      if (NameToTextWriterData.ContainsKey (name))
      {
        throw new ArgumentException (To.String.s ("TextWriter with name ").e (name).s (" already exists.").CheckAndConvertToString ());
      }
      // Append extension if name does not already contain extension
      string nameWithExtension = name.Contains(".") ? name : AppendExtension (name);
      var textWriterData = new TextWriterData (new StreamWriter (Path.Combine (Directory, nameWithExtension)), Directory, Extension);
      NameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.s ("StringWriterFactory");
      toTextBuilder.sb ();
      foreach (KeyValuePair<string, TextWriterData> pair in NameToTextWriterData)
      {
        toTextBuilder.e (pair.Key);
      }
      toTextBuilder.se ();

      foreach (KeyValuePair<string, TextWriterData> pair in NameToTextWriterData)
      {
        toTextBuilder.nl ().e (pair.Key).nl ().e (pair.Value.TextWriter.ToString ()).nl ();
      }

    }
  }
}