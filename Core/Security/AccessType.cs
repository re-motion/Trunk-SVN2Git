using System;
using System.Collections.Generic;
using Remotion.Utilities;
using System.Runtime.Serialization;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces
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

    private static readonly Dictionary<EnumWrapper, AccessType> s_cache = new Dictionary<EnumWrapper, AccessType> ();

    public static AccessType Get (Enum accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);
      return Get (new EnumWrapper (accessType));
    }

    //TODO FS: Move cache implementation into CacheProvider, move to Securiy Assembly 
    public static AccessType Get (EnumWrapper accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      if (s_cache.ContainsKey (accessType))
        return s_cache[accessType];

      lock (s_cache)
      {
        if (s_cache.ContainsKey (accessType))
          return s_cache[accessType];

        AccessType temp = new AccessType (accessType);
        s_cache.Add (accessType, temp);
        return temp;
      }
    }

    // member fields

    private EnumWrapper _value;

    // construction and disposing

    private AccessType (EnumWrapper accessType)
    {
      //Type type = TypeUtility.GetType (accessType.TypeName);
      //if (!Attribute.IsDefined (type, typeof (AccessTypeAttribute), false))
      //{
      //  throw new ArgumentException (string.Format ("Enumerated type '{0}' cannot be used as an access type. Valid access types must have the {1} applied.",
      //          type.FullName, typeof (AccessTypeAttribute).FullName),
      //      "accessType");
      //}

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