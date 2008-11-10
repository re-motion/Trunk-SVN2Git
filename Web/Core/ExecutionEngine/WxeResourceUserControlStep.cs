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
  ///   Calls user controls that are stored in the resource directory.
  /// </summary>
  /// <remarks>
  ///   The resource directory is <c>&lt;ApplicationRoot&gt;/res/&lt;AssemblyName&gt;/</c>.
  /// </remarks>
  [Serializable]
  public class WxeResourceUserControlStep : WxeUserControlStep
  {
    /// <summary>
    ///   Calls the user controls using the calling assemby's resource directory.
    /// </summary>
    public WxeResourceUserControlStep (string userControlName)
      : this (Assembly.GetCallingAssembly (), userControlName)
    {
    }

    /// <summary>
    ///   Calls the user controls using the calling assemby's resource directory.
    /// </summary>
    public WxeResourceUserControlStep (WxeVariableReference userControl)
      : this (Assembly.GetCallingAssembly (), userControl)
    {
    }

    /// <summary>
    ///   Calls the user controls using the resource directory of the assembly's type.
    /// </summary>
    public WxeResourceUserControlStep (Type resourceType, string userControlName)
      : this (resourceType.Assembly, userControlName)
    {
    }

    /// <summary>
    ///   Calls the user controls using the resource directory of the assembly's type.
    /// </summary>
    public WxeResourceUserControlStep (Type resourceType, WxeVariableReference userControl)
      : this (resourceType.Assembly, userControl)
    {
    }

    /// <summary>
    ///   Calls the user controls using the assemby's resource directory.
    /// </summary>
    public WxeResourceUserControlStep (Assembly resourceAssembly, string userControlName)
      : base (new ResourceObject (resourceAssembly, userControlName))
    {
    }

    /// <summary>
    ///   Calls the user controls using the assemby's resource directory.
    /// </summary>
    public WxeResourceUserControlStep (Assembly resourceAssembly, WxeVariableReference userControl)
      : base (new ResourceObjectWithVarRef (resourceAssembly, userControl))
    {
    }
  }
}