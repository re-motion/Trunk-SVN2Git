using System;
using System.Collections.Specialized;

namespace Remotion.Globalization
{
  /// <summary> A <b>Null Object</b> implementation of <see cref="IResourceManager"/>. </summary>
  /// <remarks> 
  ///   Use <see cref="Instance"/> to access the well defined instance of the <see cref="NullResourceManager"/>.
  /// </remarks>
  public sealed class NullResourceManager : IResourceManager
  {
    public static readonly NullResourceManager Instance = new NullResourceManager();

    private NullResourceManager ()
    {
    }

    NameValueCollection IResourceManager.GetAllStrings ()
    {
      return new NameValueCollection();
    }

    NameValueCollection IResourceManager.GetAllStrings (string prefix)
    {
      return new NameValueCollection();
    }

    string IResourceManager.GetString (string id)
    {
      return id;
    }

    string IResourceManager.GetString (Enum enumValue)
    {
      return ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue);
    }

    public bool ContainsResource (string id)
    {
      return false;
    }

    public bool ContainsResource (Enum enumValue)
    {
      return false;
    }

    string IResourceManager.Name
    {
      get { return "Remotion.Globalization.NullResourceManager"; }
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}