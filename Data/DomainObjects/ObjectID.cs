/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Uniquely identifies a domain object.
  /// </summary>
  /// <remarks>
  /// <b>ObjectID</b> supports values of type <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
  /// </remarks>
  [Serializable]
  public sealed class ObjectID : IFlattenedSerializable
  {
    // types

    // static members and constants

    private const char c_delimiter = '|';
    private const string c_escapedDelimiter = "&pipe;";
    private const string c_escapedDelimiterPlaceholder = "&amp;pipe;";

    /// <summary>
    /// Tests whether two specified <see cref="ObjectID"/> objects are equivalent.
    /// </summary>
    /// <param name="id1">The <see cref="ObjectID"/> object that is to the left of the equality operator.</param>
    /// <param name="id2">The <see cref="ObjectID"/> object that is to the right of the equality operator.</param>
    /// <returns></returns>
    public static bool operator == (ObjectID id1, ObjectID id2)
    {
      return Equals (id1, id2);
    }

    /// <summary>
    /// Tests whether two specified <see cref="ObjectID"/> objects are different.
    /// </summary>
    /// <param name="id1">The <see cref="ObjectID"/> object that is to the left of the inequality operator.</param>
    /// <param name="id2">The <see cref="ObjectID"/> object that is to the right of the inequality operator.</param>
    /// <returns></returns>
    public static bool operator != (ObjectID id1, ObjectID id2)
    {
      return !Equals (id1, id2);
    }

    /// <summary>
    /// Determines whether the specified <see cref="ObjectID"/> instances are considered equal.
    /// </summary>
    /// <param name="id1">The first <see cref="ObjectID"/> to compare.</param>
    /// <param name="id2">The second <see cref="ObjectID"/> to compare.</param>
    /// <returns><see langword="true"/> if the both <see cref="ObjectID"/>s are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals (ObjectID id1, ObjectID id2)
    {
      if (object.ReferenceEquals (id1, id2))
        return true;
      if (object.ReferenceEquals (id1, null))
        return false;

      return id1.Equals (id2);
    }

    /// <summary>
    /// Converts the string representation of the ID to an <see cref="ObjectID"/> instance.
    /// </summary>
    /// <param name="objectIDString">A string containing the object ID to convert.</param>
    /// <returns>
    ///   An <see cref="ObjectID"/> instance equivalent to the object ID contained in <paramref name="objectIDString"/>. Must not be <see langword="null"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="objectIDString"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="objectIDString"/> is an empty string.</exception>
    /// <exception cref="System.FormatException">
    ///   <paramref name="objectIDString"/> does not contain the string representation of an object ID.
    /// </exception>
    public static ObjectID Parse (string objectIDString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("objectIDString", objectIDString);

      string[] parts = objectIDString.Split (c_delimiter);

      if (parts.Length != 3)
      {
        throw new FormatException (
            string.Format (
                "Serialized ObjectID '{0}' is not correctly formatted.",
                objectIDString));
      }

      for (int i = 0; i < parts.Length; i++)
        parts[i] = Unescape (parts[i]);

      object value = GetValue (parts[2], parts[1]);

      return new ObjectID (parts[0], value);
    }

    private static object GetValue (string typeName, string value)
    {
      Type type = Type.GetType (typeName);

      if (type == typeof (Guid))
        return new Guid (value);
      else if (type == typeof (int))
        return int.Parse (value);
      else if (type == typeof (string))
        return value;
      else
        throw new FormatException (string.Format ("Type '{0}' is not supported.", typeName));
    }

    private static string Unescape (string value)
    {
      if (value.IndexOf (c_escapedDelimiter) >= 0)
        value = value.Replace (c_escapedDelimiter, c_delimiter.ToString());

      if (value.IndexOf (c_escapedDelimiterPlaceholder) >= 0)
        value = value.Replace (c_escapedDelimiterPlaceholder, c_escapedDelimiter);

      return value;
    }

    // member fields

    [NonSerialized]
    private ClassDefinition _classDefinition;

    private string _classDefinitionID;
    private object _value;

    // construction and disposing

    /// <summary>
    /// Initializes a new instance of the <b>ObjectID</b> class with the specified class ID and ID value.
    /// </summary>
    /// <param name="classID">The ID of the class of the object. Must not be <see langword="null"/>.</param>
    /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="classID"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException">
    ///   <paramref name="classID"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.Guid"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> has an unsupported type or is a string and contains invalid characters. Supported types are <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.IdentityTypeNotSupportedException">
    ///   The type of <paramref name="value"/> is not supported by the underlying <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/>.
    /// </exception>
    /// <exception cref="Mapping.MappingException"/>The specified <paramref name="classID"/> could not be found in the mapping configuration.
    public ObjectID (string classID, object value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classID);
      Initialize (classDefinition, value, "classID");
    }

    /// <summary>
    /// Initializes a new instance of the <b>ObjectID</b> class with the specified class type and ID value.
    /// </summary>
    /// <param name="classType">The <see cref="System.Type"/> of the class of the object. Must not be <see langword="null"/>.</param>
    /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="classType"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException">
    ///   <paramref name="value"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.Guid"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> has an unsupported type or is a string and contains invalid characters. Supported types are <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.IdentityTypeNotSupportedException">
    ///   The type of <paramref name="value"/> is not supported by the underlying <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/>.
    /// </exception>
    /// <exception cref="Mapping.MappingException"/>The specified <paramref name="classType"/> could not be found in the mapping configuration.
    public ObjectID (Type classType, object value)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classType);
      Initialize (classDefinition, value, "classType");
    }

    /// <summary>
    /// Initializes a new instance of the <b>ObjectID</b> class with the specified <see cref="Mapping.ClassDefinition"/> and ID value.
    /// </summary>
    /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> of the object. Must not be <see langword="null"/>.</param>
    /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="classDefinition"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException">
    ///   <paramref name="value"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.Guid"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> has an unsupported type or is a string and contains invalid characters. Supported types are <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.IdentityTypeNotSupportedException">
    ///   The type of <paramref name="value"/> is not supported by the underlying <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/>.
    /// </exception>
    /// <exception cref="Mapping.MappingException"/>The specified <paramref name="classDefinition"/> could not be found in the mapping configuration.
    public ObjectID (ClassDefinition classDefinition, object value)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ClassDefinition classDefinitionByClassType = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinition.ClassType);
      ClassDefinition classDefinitionByClassID = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinition.ID);

      if (!object.ReferenceEquals (classDefinitionByClassID, classDefinitionByClassType))
      {
        throw CreateArgumentException (
            "classDefinition",
            "The ClassID '{0}' and the ClassType '{1}' do not refer to the same ClassDefinition in the mapping configuration.",
            classDefinition.ID,
            classDefinition.ClassType);
      }

      if (!object.ReferenceEquals (classDefinitionByClassID, classDefinition))
      {
        throw CreateArgumentException (
            "classDefinition",
            "The provided ClassDefinition '{0}' is not the same reference as the ClassDefinition found in the mapping configuration.",
            classDefinition.ID);
      }

      Initialize (classDefinition, value, "classDefinition");
    }

    private void Initialize (ClassDefinition classDefinition, object value, string argumentName)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      if (classDefinition.IsAbstract)
      {
        throw CreateArgumentException (
            argumentName,
            "An ObjectID cannot be constructed for abstract type '{0}' of class '{1}'.",
            classDefinition.ClassType.AssemblyQualifiedName,
            classDefinition.ID);
      }

      CheckValue ("value", value);

      StorageProviderDefinition storageProviderDefinition =
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (classDefinition.StorageProviderID);

      storageProviderDefinition.CheckIdentityType (value.GetType());

      _classDefinition = classDefinition;
      _classDefinitionID = _classDefinition.ID;
      _value = value;
    }

    // methods and properties

    /// <summary>
    /// Gets the ID of the <see cref="Persistence.StorageProvider"/> which stores the object.
    /// </summary>
    public string StorageProviderID
    {
      get { return ClassDefinition.StorageProviderID; }
    }

    /// <summary>
    /// Gets the ID value used to identify the object in the storage provider.
    /// </summary>
    /// <remarks>
    /// <b>Value</b> can be of type <see cref="System.Guid"/>, <see cref="System.Int32"/> or <see cref="System.String"/>.
    /// </remarks>
    public object Value
    {
      get { return _value; }
    }

    /// <summary>
    /// The class ID of the object class.
    /// </summary>
    public string ClassID
    {
      get { return ClassDefinition.ID; }
    }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> associated with this <b>ObjectID</b>.
    /// </summary>
    public ClassDefinition ClassDefinition
    {
      get
      {
        if (_classDefinition == null)
          _classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (_classDefinitionID);
        return _classDefinition;
      }
    }

    /// <summary>
    /// Returns the string representation of the current <see cref="ObjectID"/>.
    /// </summary>
    /// <returns>A <see cref="String"/> that represents the current <see cref="ObjectID"/>.</returns>
    public override string ToString ()
    {
      Type valueType = Value.GetType();

      return Escape (ClassID) + c_delimiter +
          Escape (Value.ToString()) + c_delimiter +
              Escape (valueType.FullName);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode ()
    {
      return ClassID.GetHashCode() ^ Value.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified <see cref="ObjectID"/> is equal to the current <b>ObjectID</b>.
    /// </summary>
    /// <param name="obj">The <see cref="ObjectID"/> to compare with the current <b>ObjectID</b>. </param>
    /// <returns><see langword="true"/> if the specified <see cref="ObjectID"/> is equal to the current <b>ObjectID</b>; otherwise, <see langword="false"/>.</returns>
    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (this.GetType() != obj.GetType())
        return false;

      ObjectID other = (ObjectID) obj;
      if (!object.Equals (this.ClassID, other.ClassID))
        return false;
      if (!object.Equals (this.Value, other.Value))
        return false;

      return true;
    }

    private void CheckValue (string argumentName, object value)
    {
      Type valueType = value.GetType();

      if (valueType != typeof (Guid) && valueType != typeof (int) && valueType != typeof (string))
        throw CreateArgumentException (argumentName, "Remotion.Data.DomainObjects.ObjectID does not support values of type '{0}'.", valueType);

      if (valueType == typeof (string) && ((string) value).IndexOf (c_escapedDelimiterPlaceholder) >= 0)
      {
        throw new ArgumentException (
            string.Format (
                "Value cannot contain '{0}'.", c_escapedDelimiterPlaceholder),
            "value");
      }

      if (valueType == typeof (string) && string.Empty.Equals (value))
        throw new ArgumentEmptyException (argumentName);

      if (valueType == typeof (Guid) && Guid.Empty.Equals (value))
        throw new ArgumentEmptyException (argumentName);
    }

    private string Escape (string value)
    {
      if (value.IndexOf (c_escapedDelimiter) >= 0)
        value = value.Replace (c_escapedDelimiter, c_escapedDelimiterPlaceholder);

      if (value.IndexOf (c_delimiter) >= 0)
        value = value.Replace (c_delimiter.ToString(), c_escapedDelimiter);

      return value;
    }

    private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), argumentName);
    }

    #region Serialization

    private ObjectID (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _classDefinitionID = info.GetValueForHandle<string> ();
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (_classDefinitionID);
      object value = info.GetValue<object> ();

      _classDefinition = classDefinition;
      _value = value;
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure( FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_classDefinitionID);
      info.AddValue (_value);
    }

    #endregion
  }
}
