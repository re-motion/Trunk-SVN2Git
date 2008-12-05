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
using System.Reflection;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{

/// <summary>
///   Calls pages that are stored in the resource directory.
/// </summary>
/// <remarks>
///   The resource directory is <c>&lt;ApplicationRoot&gt;/res/&lt;AssemblyName&gt;/</c>.
/// </remarks>
[Serializable]
public class WxeResourcePageStep: WxePageStep
{
  /// <summary>
  ///   Calls the page using the calling assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (string pageName)
    : this (Assembly.GetCallingAssembly(), pageName)
  {
  }

  /// <summary>
  ///   Calls the page using the calling assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (WxeVariableReference page)
    : this (Assembly.GetCallingAssembly(), page)
  {
  }

  /// <summary>
  ///   Calls the page using the resource directory of the assembly's type.
  /// </summary>
  public WxeResourcePageStep (Type resourceType, string pageName)
    : this (resourceType.Assembly, pageName)
  {
  }

  /// <summary>
  ///   Calls the page using the resource directory of the assembly's type.
  /// </summary>
  public WxeResourcePageStep (Type resourceType, WxeVariableReference page)
    : this (resourceType.Assembly, page)
  {
  }

  /// <summary>
  ///   Calls the page using the assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (Assembly resourceAssembly, string pageName)
    : base (new ResourceObject(resourceAssembly, pageName))
  {
  }

  /// <summary>
  ///   Calls the page using the assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (Assembly resourceAssembly, WxeVariableReference page)
    : base (new ResourceObjectWithVarRef(resourceAssembly, page))
  {
  }
}

}
