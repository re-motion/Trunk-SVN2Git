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
using System.IO;
using System.Collections.Generic;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  // TODO AE: Test-only class -> move to UnitTests project.
  public class StringWriterFactory : TextWriterFactoryBase, IToTextConvertible
  {
    public override TextWriter NewTextWriter (string directory, string name, string extension)
    {
      ArgumentUtility.CheckNotNull ("directory", directory);
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("extension", extension);
      var textWriterData = new TextWriterData (new StringWriter (), directory, extension);
      NameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }
    
    public override TextWriter NewTextWriter (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      return NewTextWriter (Directory, name, Extension);
    }

    public void ToText (IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.s ("StringWriterFactory");
      toTextBuilder.sb();
      foreach (KeyValuePair<string, TextWriterData> pair in NameToTextWriterData)
      {
        toTextBuilder.e (pair.Key);
      }
      toTextBuilder.se ();

      foreach (KeyValuePair<string, TextWriterData> pair in NameToTextWriterData)
      {
        toTextBuilder.nl().e (pair.Key).nl().e(pair.Value.TextWriter.ToString()).nl();
      }
    
    }
  }
}