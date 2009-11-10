// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Base class for extensible enums. Create extensible enums by deriving a class representing the enumeration
  /// from <see cref="ExtensibleEnum{T}"/> and define extension methods on <see cref="ExtensibleEnumDefinition{T}"/>
  /// to define the values of the enumeration. Each value is uniquely identified by the <see cref="ID"/> string
  /// passed to the <see cref="ExtensibleEnum{T}"/> constructor. Value comparisons act solely based on this identifier.
  /// </summary>
  /// <remarks>
  /// Instances of this class should be immutable, i.e. they should not change once initialized. This is due to their value
  /// semantics (a value retrieved from a cache should not be distinguishable from a value freshly created by an extension method call) and
  /// leads to inherent thread-safety.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  [Serializable]
  public abstract class ExtensibleEnum<T> : IExtensibleEnum
      where T: ExtensibleEnum<T>
  {
    /// <summary>
    /// Provides access to all values of this extensible enum type.
    /// </summary>
    /// <remarks>Values of the extensible enum type are defined by declaring extension methods against 
    /// <see cref="ExtensibleEnumDefinition{T}"/> and can be accessed via this field.</remarks>
    public static readonly ExtensibleEnumDefinition<T> Values = (ExtensibleEnumDefinition<T>) ExtensibleEnumUtility.GetDefinition (typeof (T));

    /// <summary>
    /// Initializes a new enumeration value with a declaration space and a value name. The actual <see cref="ID"/> is formed by combining declaration
    /// space and value name.
    /// </summary>
    /// <param name="declarationSpace">A string identifying the declaration space of the identifier of the value being created. This can be a 
    /// namespace, a type name, or anything else that helps in uniquely identifying the enum value. It is used as a prefix to the <see cref="ID"/>
    /// of the value. This identifier is used for equality comparisons and hash code calculations. Can be <see langword="null" />.</param>
    /// <param name="valueName">The name of the value being created. This identifier is used for equality comparisons
    /// and hash code calculations.</param>
    protected ExtensibleEnum (string declarationSpace, string valueName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("valueName", valueName);

      DeclarationSpace = declarationSpace;
      ValueName = valueName;
    }

    /// <summary>
    /// Initializes a new enumeration value with a given <see cref="ID"/>.
    /// </summary>
    /// <param name="id">The identifier of the value being created. This identifier is used for equality comparisons
    /// and hash code calculations.</param>
    protected ExtensibleEnum (string id)
        : this (
            (string) null, 
            ArgumentUtility.CheckNotNullOrEmpty ("id", id))
    {
    }

    /// <summary>
    /// Initializes a new enumeration value with a declaring type and a value name. The actual <see cref="ID"/> is formed of the full name of the
    /// declaring type and the value name.
    /// </summary>
    /// <param name="declaringType">The type declaring the extension method defining the enum value. This type's full name is used as a prefix of the 
    /// identifier of the value being created. This identifier is used for equality comparisons and hash code calculations.</param>
    /// <param name="valueName">The name of the value being created. This identifier is used for equality comparisons
    /// and hash code calculations.</param>
    protected ExtensibleEnum (Type declaringType, string valueName)
        : this (
            ArgumentUtility.CheckNotNull ("declaringType", declaringType).FullName, 
            ArgumentUtility.CheckNotNullOrEmpty ("valueName", valueName))
    {
    }

    /// <summary>
    /// Initializes a new enumeration value, automatically setting its <see cref="ID"/> using the extension method defining the value.
    /// </summary>
    /// <param name="currentMethod">The extension method defining the enum value, use <see cref="MethodBase.GetCurrentMethod"/> to retrieve the value
    /// to pass for this parameter. The method's full name is used as the identifier of the value being created. This identifier is used for 
    /// equality comparisons and hash code calculations.</param>
    protected ExtensibleEnum (MethodBase currentMethod)
        : this (
            ArgumentUtility.CheckNotNull ("currentMethod", currentMethod).DeclaringType,
            currentMethod.Name)
    {
    }

    /// <summary>
    /// Gets the full identifier representing this extensible enum value. This is the combination of <see cref="DeclarationSpace"/> and 
    /// <see cref="ValueName"/>. Use <see cref="ExtensibleEnumDefinition{T}.GetValueInfoByID"/> to retrieve an <see cref="ExtensibleEnum{T}"/>
    /// value by its <see cref="ID"/>.
    /// </summary>
    /// <value>The ID of this value. Once an <see cref="ExtensibleEnum{T}"/> instance is constructed, this value is guaranteed to never change.</value>
    public string ID 
    {
      get { return string.IsNullOrEmpty (DeclarationSpace) ? ValueName : DeclarationSpace + "." + ValueName; }
    }
    
    /// <summary>
    /// Gets a string identifying the declaration space of the identifier of the value being created. This can be a 
    /// namespace, a type name, or anything else that helps in uniquely identifying the enum value. It is used as a prefix to the <see cref="ID"/>
    /// of the value. Can be <see langword="null" />.
    /// </summary>
    /// <value>The declaration space of this value, or <see langword="null" /> if the value does not define a declaration space.
    /// Once an <see cref="ExtensibleEnum{T}"/> instance is constructed, this value is guaranteed to never change.</value>
    public string DeclarationSpace { get; private set; }

    /// <summary>
    /// Gets name of this value. This is a part of the <see cref="ID"/> of this extensible enum value.
    /// </summary>
    /// <value>The name of this value. Once an <see cref="ExtensibleEnum{T}"/> instance is constructed, this value is guaranteed to never change.</value>
    public string ValueName { get; private set; }

    /// <summary>
    /// Gets the localized name of the value represented by this instance by using the <see cref="ExtensibleEnumInfo{T}.ResourceManager"/> associated
    /// with the declaring type of the extension method which defines the value.
    /// </summary>
    /// <returns>The localized name of this value.</returns>
    public string GetLocalizedName ()
    {
      return GetValueInfo().ResourceManager.GetString (ID);
    }

    /// <summary>
    /// Gets the <see cref="ExtensibleEnumInfo{T}"/> object describing the value represented by this instance.
    /// </summary>
    /// <returns>The <see cref="ExtensibleEnumInfo{T}"/> for this value.</returns>
    public ExtensibleEnumInfo<T> GetValueInfo ()
    {
      return Values.GetValueInfoByID (ID);
    }

    /// <summary>
    /// Gets the type of the extensible enum this value belongs to.
    /// </summary>
    public Type GetEnumType ()
    {
      return typeof (T);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance. Equality is
    /// determined by comparing the <see cref="ID"/> and type of the values for equality.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    /// 	<see langword="true" /> if the specified <see cref="System.Object"/> is an extensible enum of the same
    /// 	type and with an equal <see cref="ID"/> as this instance; otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals (object obj)
    {
      return obj != null && obj.GetType() == GetType() &&  ((ExtensibleEnum<T>) obj).ID == ID;
    }

    /// <inheritdoc />
    public override int GetHashCode ()
    {
      return ID.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this <see cref="ExtensibleEnum{T}"/> value. The string returned by this method is meant
    /// to be read, not parsed. Use <see cref="ID"/> to get a string that can be used to get back to the actual value. Use 
    /// <see cref="GetLocalizedName()"/> to get a localized name of the value.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this <see cref="ExtensibleEnum{T}"/> value.
    /// </returns>
    public override string ToString ()
    {
      return ID;
    }

    IExtensibleEnumInfo IExtensibleEnum.GetValueInfo ()
    {
      return GetValueInfo ();
    }
  }
}
