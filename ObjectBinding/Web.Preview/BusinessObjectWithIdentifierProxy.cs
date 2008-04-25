using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web
{
  public sealed class BusinessObjectWithIdentityProxy
  {
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;

    private BusinessObjectWithIdentityProxy ()
    {
    }

    public BusinessObjectWithIdentityProxy (IBusinessObjectWithIdentity obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      _uniqueIdentifier = obj.UniqueIdentifier;
      _displayName = obj.DisplayNameSafe;
    }

    public BusinessObjectWithIdentityProxy (string uniqueIdentifier, string displayName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
    }

    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public string DisplayName
    {
      get { return _displayName; }
    }
  }
}