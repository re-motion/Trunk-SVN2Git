// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using System.Resources;
using Remotion.Utilities;

namespace Remotion.Globalization
{
/// <summary>
///   Attribute for specifying the resource container for a type.
/// </summary>
/// <remarks>
/// use the <see cref="MultiLingualResources"/> class to analyze instances of this attribute and to retrieve <see cref="ResourceManager"/>
/// objects for them, eg. when implementing <see cref="IObjectWithResources"/>.
/// </remarks>
[AttributeUsage (AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
public class MultiLingualResourcesAttribute: Attribute, IResourcesAttribute
{
  // types

  // member fields

  /// <summary> The base name of the resource container </summary>
  private string _baseName = null;
  private Assembly _resourceAssembly = null;

  // construction and disposing

  /// <summary> Initalizes an instance. </summary>
  public MultiLingualResourcesAttribute (string baseName)
  {
    SetBaseName (baseName);
  }

  // methods and properties

  /// <summary>
  ///   Gets the base name of the resource container as specified by the attributes construction.
  /// </summary>
  /// <remarks>
  /// The base name of the resource conantainer to be used by this type
  /// (&lt;assembly&gt;.&lt;path inside project&gt;.&lt;resource file name without extension&gt;).
  /// </remarks>
  public string BaseName 
  {
    get { return _baseName; }
  }
  
  protected void SetBaseName (string baseName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("baseName", baseName);
    _baseName = baseName;
  }

  public Assembly ResourceAssembly
  {
    get { return _resourceAssembly; }
  }

  protected void SetResourceAssembly (Assembly resourceAssembly)
  {
    ArgumentUtility.CheckNotNull ("resourceAssembly", resourceAssembly);
    _resourceAssembly = resourceAssembly;
  }

  public override string ToString ()
  {
    return BaseName;
  }
}

}
