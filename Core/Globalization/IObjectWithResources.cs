namespace Remotion.Globalization
{

/// <summary>
///   A class whose instances know where their resource container is.
/// </summary>
/// <remarks>
///   Used to externally controll resource management.
/// </remarks>
public interface IObjectWithResources
{
  /// <summary>
  ///   Returns an instance of <c>IResourceManager</c> for resource container of the object.
  /// </summary>
  IResourceManager GetResourceManager();
}

}
