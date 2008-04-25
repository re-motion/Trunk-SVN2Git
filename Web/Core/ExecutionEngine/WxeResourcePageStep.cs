using System;
using System.Reflection;

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
    : base (resourceAssembly, pageName)
  {
  }

  /// <summary>
  ///   Calls the page using the assemby's resource directory.
  /// </summary>
  public WxeResourcePageStep (Assembly resourceAssembly, WxeVariableReference page)
    : base (resourceAssembly, page)
  {
  }
}

}
