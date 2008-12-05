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
