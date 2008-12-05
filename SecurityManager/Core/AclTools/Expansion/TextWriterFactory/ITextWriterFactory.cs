// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  /// <summary>
  /// Creates <see cref="TextWriter"/>|s through <see cref="CreateTextWriter(string,string,string)"/> in the currently set 
  /// <see cref="Directory"/>.
  /// Stores references to the created <see cref="TextWriter"/>|s. 
  /// </summary>
  public interface ITextWriterFactory
  {
    TextWriter CreateTextWriter (string directory, string name, string extension);
    TextWriter CreateTextWriter (string name); 
    string GetRelativePath (string fromName, string toName); 

    string Directory { get; set; } 
    string Extension { get; set; }
  }
}