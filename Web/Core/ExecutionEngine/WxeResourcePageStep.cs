/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
