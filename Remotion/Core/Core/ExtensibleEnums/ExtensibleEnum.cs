// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Base class for extensible enums. Create extensible enums by deriving a class representing the enumeration
  /// from <see cref="ExtensibleEnum{T}"/> and define extension methods on <see cref="ExtensibleEnumDefinition{T}"/>
  /// to define the values of the enumeration. Each value is uniquely identified by the <see cref="ID"/> string
  /// passed to the <see cref="ExtensibleEnum{T}"/> constructor. Value comparisons act solely based on this identifier.
  /// </summary>
  public abstract class ExtensibleEnum<T> : IExtensibleEnum
      where T: ExtensibleEnum<T>
  {
    /// <summary>
    /// Provides access to all values of this extensible enum type.
    /// </summary>
    /// <remarks>Values of the extensible enum type are defined by declaring extension methods against 
    /// <see cref="ExtensibleEnumDefinition{T}"/> and can be accessed via this field.</remarks>
    public static readonly ExtensibleEnumDefinition<T> Values = (ExtensibleEnumDefinition<T>) ExtensibleEnumDefinitionCache.Instance.GetDefinition (typeof (T));

    /// <summary>
    /// Initializes a new enumeration value with an <see cref="ID"/> prefix and a short ID. The actual <see cref="ID"/> is formed of both prefix
    /// and short ID.
    /// </summary>
    /// <param name="idPrefix">The prefix (e.g. a namespace) of the identifier of the value being created. This identifier is used for equality 
    /// comparisons and hash code calculations. Can be <see langword="null" />.</param>
    /// <param name="shortID">The short identifier of the value being created. This identifier is used for equality comparisons
    /// and hash code calculations.</param>
    protected ExtensibleEnum (string idPrefix, string shortID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("shortID", shortID);

      IDPrefix = idPrefix;
      ShortID = shortID;
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
    /// Initializes a new enumeration value with a declaring type and a short ID. The actual <see cref="ID"/> is formed of the full name of the
    /// declaring type and the short ID.
    /// </summary>
    /// <param name="declaringType">The type declaring the extension method defining the enum value. This type's full name is used as a prefix of the 
    /// identifier of the value being created. This identifier is used for equality comparisons and hash code calculations.</param>
    /// <param name="shortID">The short identifier of the value being created. This identifier is used for equality comparisons
    /// and hash code calculations.</param>
    protected ExtensibleEnum (Type declaringType, string shortID)
        : this (
            ArgumentUtility.CheckNotNull ("declaringType", declaringType).FullName, 
            ArgumentUtility.CheckNotNullOrEmpty ("shortID", shortID))
    {
    }

    /// <summary>
    /// Initializes a new enumeration value, automatically setting its <see cref="ID"/> using the extension method defining the value.
    /// </summary>
    /// <param name="currentMethod">The extension method defining the enum value, use <see cref="MethodBase.GetCurrentMethod"/> to retrieve the value
    /// to pass for this parameter. The method's full name is used as the identifier of the value being  created. This identifier is used for 
    /// equality comparisons and hash code calculations.</param>
    protected ExtensibleEnum (MethodBase currentMethod)
        : this (
            ArgumentUtility.CheckNotNull ("currentMethod", currentMethod).DeclaringType,
            currentMethod.Name)
    {
    }

    /// <summary>
    /// Gets the full identifier representing this extensible enum value. This is the combination of <see cref="IDPrefix"/> and <see cref="ShortID"/>.
    /// </summary>
    /// <value>The ID of this value.</value>
    public string ID 
    {
      get { return string.IsNullOrEmpty (IDPrefix) ? ShortID : IDPrefix + "." + ShortID; }
    }
    
    /// <summary>
    /// Gets the ID prefix. This is used to form the <see cref="ID"/> representing this extensible enum value.
    /// </summary>
    /// <value>The ID prefix of this value.</value>
    public string IDPrefix { get; private set; }

    /// <summary>
    /// Gets the short ID. This is used to form the <see cref="ID"/> representing this extensible enum value.
    /// </summary>
    /// <value>The short ID of this value.</value>
    public string ShortID { get; private set; }

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

    /// <inheritdoc />
    public override string ToString ()
    {
      return ID;
    }
  }
}