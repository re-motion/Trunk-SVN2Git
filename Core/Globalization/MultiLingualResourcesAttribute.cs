using System;
using System.Collections;
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
}

}
