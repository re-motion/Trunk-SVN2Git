using System;
using Remotion.Utilities;

namespace Remotion.Security
{
  //TODO FS: Move to SecurityInterfaces
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
    /// <summary> Parses strings in the format <c>Name|TypeName</c>. </summary>
    /// <param name="value"> A <see cref="String"/> in the format <c>Name|TypeName</c>. Must not be <see langword="null"/> or emtpy. </param>
    /// <returns> A new instance of the <see cref="EnumWrapper"/> type initalized with the specified <b>Name</b> and <b>TypeName</b>. </returns>
    /// <exception cref="ArgumentException">The <paramref name="value"/> is not in the format <c>Name|TypeName</c>.</exception>
    [Obsolete ("Will be removed when moved into interfaces assembly.", false)]
    public static EnumWrapper Parse (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      string[] parts = value.Split (new char[] { '|' }, 2);

      if (parts.Length == 1)
        throw new ArgumentException (string.Format ("The value '{0}' did not contain the type name of the enumerated value. Expected format: 'Name|TypeName'", value), "value");

      string name = parts[0];
      string typeName = parts[1];

      if (name.Length == 0)
        throw new ArgumentException (string.Format ("The value '{0}' did not contain the name of the enumerated value. Expected format: 'Name|TypeName'", value), "value");

      if (typeName.Length == 0)
        throw new ArgumentException (string.Format ("The value '{0}' did not contain the type name of the enumerated value. Expected format: 'Name|TypeName'", value), "value");

      return new EnumWrapper (name, typeName);
    }

    private readonly string _typeName;
    //TODO FS: Use fullname of enum in current format "value|PartialAssemblyQualifiedName"
    //TODO FS: StateEnumValues must be migrated (SecurityManager!!!)
    private readonly string _name;
    private Enum _enumValue;

    public EnumWrapper (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      Type type = enumValue.GetType ();
      if (Attribute.IsDefined (type, typeof (FlagsAttribute), false))
      {
        throw new ArgumentException (string.Format (
                "Enumerated type '{0}' cannot be wrapped. Only enumerated types without the {1} can be wrapped.", 
                type.FullName, 
                typeof (FlagsAttribute).FullName),
            "enumValue");
      }
      
      _typeName = TypeUtility.GetPartialAssemblyQualifiedName (type);
      _name = enumValue.ToString ();
      _enumValue = enumValue;
    }

    public EnumWrapper (string name, string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      _name = name;
      _typeName = TypeUtility.ParseAbbreviatedTypeName (typeName);
      _enumValue = null;
    }

    [Obsolete ("Will be removed when moved into interfaces assembly.", true)]
    public string TypeName
    {
      get { return _typeName; }
    }	

    public string Name
    {
      get { return _name; }
    }

    /// <summary> Returns the enum value wrapped by this <see cref="EnumWrapper"/>. </summary>
    /// <returns> The enum value designated by the <see cref="TypeName"/> and <see cref="Name"/> properties. </returns>
    /// <exception cref="TypeLoadException"> The <see cref="TypeName"/> cannot by found. </exception>
    /// <exception cref="InvalidOperationException"> 
    ///   The <see cref="TypeName"/> is not an enumerated type. 
    ///   <br/>- or -<br/>
    ///   The <see cref="Name"/> does not designate a valid value of the enumerated type.
    /// </exception>
    [Obsolete ("Will be removed when moved into interfaces assembly.", true)]
    public Enum GetEnum ()
    {
      if (_enumValue == null)
      {
        Type type = Type.GetType (_typeName, true, false);
        
        if (!type.IsEnum)
          throw new InvalidOperationException (string.Format ("The type '{0}' is not an enumerated type.", _typeName));
        if (!Enum.IsDefined (type, _name))
          throw new InvalidOperationException (string.Format ("The enumerated type '{0}' does not define the value '{1}'.", _typeName, _name));
          
        _enumValue = (Enum) Enum.Parse (type, _name, false);
      }

      return _enumValue;
    }

    /// <summary> Compares the supplied object parameter to the <see cref="TypeName"/> and  <see cref="Name"/> properties. </summary>
    /// <param name="obj"> The object to be compared. </param>
    /// <returns>
    ///   <see langword="true"/> if object is an instance of <see cref="EnumWrapper"/> and the two are equal; otherwise <see langword="false"/>.
    ///   If object is a null reference, <see langword="false"/> is returned.
    /// </returns>
    public override bool Equals (object obj)
    {
      if (!(obj is EnumWrapper))
        return false;

      return Equals ((EnumWrapper) obj);
    }

    /// <summary>
    ///   Compares the supplied object parameter to the <see cref="Name"/> and <see cref="TypeName"/> property of the <see cref="EnumWrapper"/> 
    ///   object.
    /// </summary>
    /// <param name="value"> The <see cref="EnumWrapper"/> instance to be compared. </param>
    /// <returns> <see langword="true"/> if the two are equal; otherwise <see langword="false"/>. </returns>
    public bool Equals (EnumWrapper value)
    {
      return this._name.Equals (value._name, StringComparison.Ordinal)
          && this._typeName.Equals (value._typeName, StringComparison.Ordinal);
    }

    public override int GetHashCode ()
    {
      return _name.GetHashCode () ^ _typeName.GetHashCode ();
    }

    /// <summary> 
    ///   Formats the <see cref="EnumWrapper"/> instance's <see cref="TypeName"/> and <see cref="Name"/> properties into a <see cref="String"/>
    ///   of the format <c>Name|TypeName</c>. 
    /// </summary>
    public override string ToString ()
    {
      return _name + "|" + _typeName;
    }
  }

}