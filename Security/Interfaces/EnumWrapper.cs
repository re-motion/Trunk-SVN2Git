using System;

namespace Remotion.Security
{
  /// <summary>Represents an access type enum value.</summary>
  /// <remarks>
  /// Use the static <see cref="O:Remotion.Security.AccessType.Get"/> methods to convert an enum to an access type.
  /// <note>For the set of basic access types see <see cref="T:Remotion.Security.GeneralAccessTypes"/>.</note>
  /// </remarks>
  /// <summary>Wraps an enum and exposes the enum information as string.</summary>
  /// <remarks>Used for example to cross web service boundaries, when the server is unaware of a given enum type.</remarks>
  [Serializable]
  public struct EnumWrapper : IEquatable<EnumWrapper>
  {
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumWrapper"/> class, setting the wrapper's name to a string of the format "enumValue|enumType".
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    public EnumWrapper (Enum enumValue)
        : this (ArgumentUtility.CheckNotNull ("enumValue", enumValue).ToString(), GetPartialAssemblyQualifiedName (enumValue.GetType()))
    {
      Type type = enumValue.GetType ();
      if (Attribute.IsDefined (type, typeof (FlagsAttribute), false))
      {
        throw new ArgumentException (string.Format (
                "Enumerated type '{0}' cannot be wrapped. Only enumerated types without the {1} can be wrapped.", 
                type.FullName, 
                typeof (FlagsAttribute).FullName),
            "enumValue");
      }
    }

    private static string GetPartialAssemblyQualifiedName (Type type)
    {
      return type.FullName + ", " + type.Assembly.GetName ().Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumWrapper"/> class, setting the wrapper's name to the specified string.
    /// </summary>
    /// <param name="name">The name to be set.</param>
    public EnumWrapper (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      _name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumWrapper"/> class, setting the wrapper's name to a string of the format "name|typeName".
    /// </summary>
    /// <param name="name">The name to be set.</param>
    /// <param name="typeName">The type name to be integrated into the name.</param>
    public EnumWrapper (string name, string typeName)
        : this (string.Format ("{0}|{1}", 
          ArgumentUtility.CheckNotNullOrEmpty ("name", name), 
          ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName)))
    {
    }

    public string Name
    {
      get { return _name; }
    }

    public override string ToString ()
    {
      return _name;
    }

    public bool Equals (EnumWrapper other)
    {
      return base.Equals (other);
    }
  }

}