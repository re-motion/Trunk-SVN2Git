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
using System.IO;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  // TODO AE: Consider storing TextWriters instead.
  public class TextWriterData
  {
    public TextWriterData (TextWriter textWriter, string directory, string extension)
    {
      ArgumentUtility.CheckNotNull ("textWriter", textWriter);
      ArgumentUtility.CheckNotNull ("directory", directory); // directory empty OK
      // extension NULL OK
      TextWriter = textWriter;
      Directory = directory;
      Extension = extension;
    }

    public TextWriter TextWriter { get; private set; }
    // TODO AE: Not used.
    public string Directory { get; private set; }
    // TODO AE: Not used.
    public string Extension { get; private set; }
  }
}