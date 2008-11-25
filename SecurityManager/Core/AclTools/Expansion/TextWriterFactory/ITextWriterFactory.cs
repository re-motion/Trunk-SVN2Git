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

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  /// <summary>
  /// Creates <see cref="TextWriter"/>|s through <see cref="NewTextWriter"/> in the currently set 
  /// <see cref="Directory"/>.
  /// Stores references to the created <see cref="TextWriter"/>|s.
  /// </summary>
  public interface ITextWriterFactory
  {
    TextWriter NewTextWriter (string name);
    string GetRelativePath (string fromName, string toName);

    string Directory { get; set; }
    string Extension { get; set; }
  }
}