// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Text;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary> A collection of business object properties that result in each other. </summary>
  /// <remarks>
  ///   <para>
  ///     A property path is comprised of zero or more <see cref="IBusinessObjectReferenceProperty"/> instances and 
  ///     a final <see cref="IBusinessObjectProperty"/>.
  ///   </para><para>
  ///     In its string representation, the property path uses the <see cref="char"/> returned by the 
  ///     <see cref="IBusinessObjectProvider.GetPropertyPathSeparator"/> method as the separator. The 
  ///     <see cref="IBusinessObjectClass"/> of the next property is used to get the
  ///     <see cref="IBusinessObjectProvider"/>.
  ///   </para>
  /// </remarks>
  public class BusinessObjectPropertyPath : IBusinessObjectPropertyPath
  {
    /// <summary> Property path formatters can be passed to <see cref="String.Format"/> for full <see cref="IFormattable"/> support. </summary>
    public class Formatter : IFormattable
    {
      private IBusinessObject _object;
      private IBusinessObjectPropertyPath _path;

      public Formatter (IBusinessObject obj, IBusinessObjectPropertyPath path)
      {
        _object = obj;
        _path = path;
      }

      public string ToString (string format, IFormatProvider formatProvider)
      {
        return _path.GetString (_object, format);
      }

      public override string ToString ()
      {
        return _path.GetString (_object, null);
      }
    }

    private IBusinessObjectProperty[] _properties;

    /// <summary> Parses the string representation of a property path into a list of properties. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> containing the first property in the path. Must no be <see langword="null"/>. </param>
    /// <param name="propertyPathIdentifier"> A string with a valid property path syntax. Must no be <see langword="null"/> or empty. </param>
    /// <returns> A <see cref="BusinessObjectPropertyPath"/>. </returns>
    public static IBusinessObjectPropertyPath Parse (IBusinessObjectClass objectClass, string propertyPathIdentifier)
    {
      ArgumentUtility.CheckNotNull ("objectClass", objectClass);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);

      char separator = objectClass.BusinessObjectProvider.GetPropertyPathSeparator ();

      string[] propertyIdentifiers = propertyPathIdentifier.Split (separator);
      IBusinessObjectProperty[] properties = new IBusinessObjectProperty[propertyIdentifiers.Length];

      int lastProperty = propertyIdentifiers.Length - 1;

      for (int i = 0; i < lastProperty; i++)
      {
        properties[i] = objectClass.GetPropertyDefinition (propertyIdentifiers[i]);
        if (properties[i] == null)
        {
          throw new ArgumentException (
              string.Format ("BusinessObjectClass '{0}' does not contain a property named '{1}'.", objectClass.Identifier, propertyIdentifiers[i]),
              "propertyPathIdentifier");
        }
        IBusinessObjectReferenceProperty referenceProperty = properties[i] as IBusinessObjectReferenceProperty;
        if (referenceProperty == null)
        {
          throw new ArgumentException (
              string.Format (
                  "Each property in a property path except the last one must be a reference property. Property {0} is of type {1}.",
                  i,
                  properties[i].GetType().FullName));
        }

        objectClass = referenceProperty.ReferenceClass;
      }

      properties[lastProperty] = objectClass.GetPropertyDefinition (propertyIdentifiers[lastProperty]);
      if (properties[lastProperty] == null)
      {
        throw new ArgumentException (
            string.Format (
                "BusinessObjectClass '{0}' does not contain a property named '{1}'.",
                objectClass.Identifier,
                propertyIdentifiers[lastProperty]),
            propertyPathIdentifier);
      }

      return objectClass.BusinessObjectProvider.CreatePropertyPath (properties);
    }

    /// <summary> Initializes a new instance of the <b>BusinessObjectPropertyPath</b> class. </summary>
    /// <param name="properties">
    ///   The properties comprising the property path. 
    ///   Must no be <see langword="null"/> or empty or contain items that are <see langword="null"/>.
    ///   All but the last item must be of type <see cref="IBusinessObjectReferenceProperty"/>.
    /// </param>
    protected internal BusinessObjectPropertyPath (IBusinessObjectProperty[] properties)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("properties", properties);

      for (int i = 0; i < properties.Length - 1; ++i)
      {
        if (!(properties[i] is IBusinessObjectReferenceProperty))
          throw new ArgumentItemTypeException ("properties", i, typeof (IBusinessObjectReferenceProperty), properties[i].GetType());
      }

      _properties = properties;
    }

    /// <summary> Gets the list of properties in this path. </summary>
    public IBusinessObjectProperty[] Properties
    {
      get { return _properties; }
    }

    /// <summary> Gets the last property in this property path. </summary>
    public IBusinessObjectProperty LastProperty
    {
      get { return _properties[_properties.Length - 1]; }
    }

    /// <summary> Gets the value of this property path for the specified object. </summary>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="throwExceptionIfNotReachable"> 
    ///   If <see langword="true"/>, an <see cref="InvalidOperationException"/> is thrown if any but the last property 
    ///   in the path is <see langword="null"/>. If <see langword="false"/>, <see langword="null"/> is returned instead. 
    /// </param>
    /// <param name="getFirstListEntry">
    ///   If <see langword="true"/>, the first value of each list property is processed.
    ///   If <see langword="false"/>, evaluation of list properties causes an <see cref="InvalidOperationException"/>.
    ///   (This does not apply to the last property in the path. If the last property is a list property, the return value is always a list.)
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if any but the last property in the path is <see langword="null"/>, or is not a single-value reference property. 
    /// </exception>
    public virtual object GetValue (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      if (!IsAccessible (obj, throwExceptionIfNotReachable, getFirstListEntry))
        return null;

      IBusinessObject obj2 = GetValueWithoutLast (obj, throwExceptionIfNotReachable, getFirstListEntry);
      if (obj2 == null)
        return null;

      return obj2.GetProperty (LastProperty);
    }

    /// <summary> Gets the string representation of the value of this property path for the specified object. </summary>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="format"> The format string passed to <see cref="IBusinessObject.GetPropertyString">IBusinessObject.GetPropertyString</see>. </param>
    public virtual string GetString (IBusinessObject obj, string format)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      
      if (!IsAccessible (obj, false, true))
        return obj.BusinessObjectClass.BusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ();

      IBusinessObject obj2 = GetValueWithoutLast (obj, false, true);
      if (obj2 == null)
        return string.Empty;

      return obj2.GetPropertyString (LastProperty, format);
    }

    private bool IsAccessible (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
    {
      for (int i = 0; i < (_properties.Length - 1); ++i)
      {
        IBusinessObjectReferenceProperty property = (IBusinessObjectReferenceProperty) _properties[i];

        if (!property.IsAccessible (obj.BusinessObjectClass, obj))
          return false;

        obj = GetProperty (obj, property, getFirstListEntry, i);

        if (obj == null)
        {
          if (throwExceptionIfNotReachable)
            throw new InvalidOperationException (string.Format ("A null value was detected in element {0} of property path {1}. Cannot evaluate rest of path.", i, this));

          return true;
        }
      }
      return LastProperty.IsAccessible (obj.BusinessObjectClass, obj);
    }

    /// <summary> Gets value of this property path minus the penultimate property for the specified object. </summary>
    private IBusinessObject GetValueWithoutLast (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
    {
      for (int i = 0; i < (_properties.Length - 1); ++i)
      {
        IBusinessObjectReferenceProperty property = (IBusinessObjectReferenceProperty) _properties[i];
        obj = GetProperty (obj, property, getFirstListEntry, i);

        if (obj == null)
        {
          if (throwExceptionIfNotReachable)
            throw new InvalidOperationException (string.Format ("A null value was detected in element {0} of property path {1}. Cannot evaluate rest of path.", i, this));

          return null;
        }
      }
      return obj;
    }

    private IBusinessObject GetProperty (IBusinessObject obj, IBusinessObjectProperty property, bool getFirstListEntry, int propertyIndex)
    {
      if (property.IsList)
      {
        if (!getFirstListEntry)
          throw new InvalidOperationException (string.Format ("Element {0} of property path {1} is not a single-value property.", propertyIndex, this));

        IList list = (IList) obj.GetProperty (property);
        if (list.Count > 0)
          obj = (IBusinessObject) list[0];
        else
          obj = null;
      }
      else
      {
        obj = (IBusinessObject) obj.GetProperty (property);
      }
      return obj;
    }

    /// <summary> Sets the value of this property path for the specified object. </summary>
    /// <param name="obj">
    ///   The object that has the first property in the path. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The value to be assiged to the property. </param>
    /// <remarks> <b>SetValue</b> is not implemented in the current version. </remarks>
    /// <exception cref="NotImplementedException"> This method is not implemented. </exception>
    public virtual void SetValue (IBusinessObject obj, object value)
    {
      // TODO: implement
      throw new NotImplementedException ();
    }

    /// <summary> Gets the string representation of this property path. </summary>
    public string Identifier
    {
      get
      {
        StringBuilder sb = new StringBuilder (100);
        char separator = '\0';
        for (int i = 0; i < _properties.Length; i++)
        {
          if (i == 0)
            separator = _properties[i].BusinessObjectProvider.GetPropertyPathSeparator ();
          else
            sb.Append (separator);
          sb.Append (_properties[i].Identifier);
        }
        return sb.ToString ();
      }
    }

    /// <summary> Returns the <see cref="Identifier"/> for this property path. </summary>
    public override string ToString ()
    {
      return Identifier;
    }

  }

}
