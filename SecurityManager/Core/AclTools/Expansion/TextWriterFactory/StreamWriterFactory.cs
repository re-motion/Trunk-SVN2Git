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
  public class StreamWriterFactory : TextWriterFactoryBase, IToTextConvertible
  {
    public override TextWriter CreateTextWriter (string directory, string name, string extension)
    {
      ArgumentUtility.CheckNotNull ("directory", directory);
      ArgumentUtility.CheckNotNull ("name", name); 

      if (!System.IO.Directory.Exists (directory))
      {
        System.IO.Directory.CreateDirectory (directory);
      }

      if (NameToTextWriterData.ContainsKey (name))
      {
        throw new ArgumentException (To.String.s ("TextWriter with name ").e (name).s (" already exists.").CheckAndConvertToString ());
      }

      string nameWithExtension = AppendExtension (name, extension);

      var textWriterData = new TextWriterData (new StreamWriter (Path.Combine (directory, nameWithExtension)), directory, extension);
      NameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }

    public override TextWriter CreateTextWriter (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name); 
      if (Directory == null)
      {
        throw new InvalidOperationException ("Directory must not be null. Set using \"Directory\"-property before calling \"CreateTextWriter\"");
      }
      return CreateTextWriter (Directory, name, Extension);
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.s ("StreamWriterFactory");
      toTextBuilder.sb ();
      foreach (KeyValuePair<string, TextWriterData> pair in NameToTextWriterData)
      {
        toTextBuilder.e (pair.Key);
      }
      toTextBuilder.se ();
    }

  }
}