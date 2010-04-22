using Remotion.Utilities;

namespace Remotion.Web
{
  /// <summary>
  /// Represents the absolute URL for a resource file that is not constructed using the <see cref="ResourceUrlResolver"/> infrastrcuture.
  /// </summary>
  public class StaticResourceUrl : IResourceUrl
  {
    private readonly string _url;

    public StaticResourceUrl (string url)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("url", url);

      _url = url;
    }

    public string GetUrl ()
    {
      return _url;
    }
  }
}