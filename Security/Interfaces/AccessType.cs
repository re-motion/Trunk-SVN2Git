using System;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Implementation;
using Remotion.Security.BridgeInterfaces;

namespace Remotion.Security
{
   /// <summary>Represents an access type enum value.</summary>
  /// <remarks>
  /// Use the static <see cref="O:Remotion.Security.AccessType.Get"/> methods to convert an enum to an access type.
  /// <note>For the set of basic access types see <see cref="T:Remotion.Security.GeneralAccessTypes"/>.</note>
  /// </remarks>
  [Serializable]
  public sealed class AccessType : IObjectReference
  {
    // types

    // static members and constants

    private static readonly ICache<EnumWrapper, AccessType> s_cache = 
        VersionDependentImplementationBridge<IAccessTypeCacheImplementation>.Implementation.CreateCache();

    public static AccessType Get (Enum accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);
      return Get (new EnumWrapper (accessType));
    }

    public static AccessType Get (EnumWrapper accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);
      return s_cache.GetOrCreateValue (accessType, delegate (EnumWrapper key) { return new AccessType (key); });
    }

    // member fields

    private EnumWrapper _value;

    // construction and disposing

    private AccessType (EnumWrapper accessType)
    {
      _value = accessType;
    }

    // methods and properties

    public EnumWrapper Value
    {
      get { return _value; }
    }

    public override string ToString ()
    {
      return _value.ToString ();
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return AccessType.Get (_value);
    }
  }
}