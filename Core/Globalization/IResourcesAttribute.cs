using System.Reflection;

namespace Remotion.Globalization
{
  public interface IResourcesAttribute
  {
    /// <summary>
    ///   Gets the base name of the resource container as specified by the attributes construction.
    /// </summary>
    /// <remarks>
    /// The base name of the resource conantainer to be used by this type
    /// (&lt;assembly&gt;.&lt;path inside project&gt;.&lt;resource file name without extension&gt;).
    /// </remarks>
    string BaseName { get; }

    Assembly ResourceAssembly { get; }
  }
}