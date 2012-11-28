// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Remotion.ObjectBinding.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   A <see cref="PropertyPathBinding"/> encapsulates the creation of a 
  ///   <see cref="IBusinessObjectPropertyPath"/> from its string representation and an
  ///   <see cref="IBusinessObjectDataSource"/>
  /// </summary>
  public class PropertyPathBinding : BusinessObjectControlItem, IBusinessObjectClassSource
  {
    /// <summary> <see langword="true"/> once the <see cref="IBusinessObjectPropertyPath"/> has been set. </summary>
    private bool _isPropertyPathEvaluated;
    /// <summary> 
    ///   The <see cref="IBusinessObjectPropertyPath"/> mananged by this 
    ///   <see cref="PropertyPathBinding"/>.
    /// </summary>
    private IBusinessObjectPropertyPath _propertyPath;
    /// <summary> 
    ///   The <see cref="string"/> representing the <see cref="IBusinessObjectPropertyPath"/> mananged 
    ///   by this <see cref="PropertyPathBinding"/>.
    /// </summary>
    private string _propertyPathIdentifier;

    private bool _isDynamic;

    /// <summary> 
    ///   Initializes a new instance of the <see cref="PropertyPathBinding"/> class with the
    ///   <see cref="IBusinessObjectPropertyPath"/> managed by this instance.
    ///  </summary>
    /// <param name="propertyPath">
    ///   The <see cref="IBusinessObjectPropertyPath"/> mananged by this 
    ///   <see cref="PropertyPathBinding"/>.
    /// </param>
    public PropertyPathBinding (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      SetPropertyPath (propertyPath);
    }

    public PropertyPathBinding (string propertyPathIdentifier)
    {
      PropertyPathIdentifier = propertyPathIdentifier;
    }

    public PropertyPathBinding ()
    {
    }

    /// <summary>
    ///   Returns a <see cref="string"/> that represents this <see cref="PropertyPathBinding"/>.
    /// </summary>
    /// <returns>
    ///   Returns the class name of the instance.
    /// </returns>
    public override string ToString ()
    {
      return GetType ().Name;
    }

    /// <summary> 
    ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> used to evaluate the 
    ///   <see cref="PropertyPathIdentifier"/>. 
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IBusinessObjectDataSource DataSource
    {
      get
      {
        if (OwnerControl != null)
          return OwnerControl.DataSource;
        return null;
      }
    }

    // TODO: merge with GetDynamicPropertyPath
    // TODO: UnitTests
    /// <summary> 
    ///   Gets the <see cref="IBusinessObjectPropertyPath"/> mananged by this <see cref="PropertyPathBinding"/>.
    /// </summary>
    public IBusinessObjectPropertyPath GetPropertyPath ()
    {
      if (_isPropertyPathEvaluated)
        return _propertyPath;

      if (_isDynamic)
        throw new InvalidOperationException ("Dynamic property paths must be resolved using the GetDynamicPropertyPath method.");

      if (OwnerControl == null)
        throw new InvalidOperationException ("The property path could not be resolved because the object is not part of an IBusinessObjectBoundControl.");

      bool isDesignMode = Remotion.Web.Utilities.ControlHelper.IsDesignMode (OwnerControl);
      bool isDataSourceNull = DataSource == null;
      bool isBusinessObjectClassNull = BusinessObjectClass == null;

      if (isDesignMode && isBusinessObjectClassNull)
        return null;

      if (StringUtility.IsNullOrEmpty (_propertyPathIdentifier))
      {
        _propertyPath = null;
      }
      else
      {
        if (isDataSourceNull)
          throw new InvalidOperationException ("The property path could not be resolved because the DataSource is not set.");

        _propertyPath = BusinessObjectPropertyPath.ParseStatic (BusinessObjectClass, _propertyPathIdentifier);
      }
      _isPropertyPathEvaluated = true;

      return _propertyPath;
    }

    // TODO: merge with GetPropertyPath
    // TODO: UnitTests
    public IBusinessObjectPropertyPath GetDynamicPropertyPath (IBusinessObjectClass businessObjectClass)
    {
      ArgumentUtility.CheckNotNull ("businessObjectClass", businessObjectClass);

      IBusinessObjectPropertyPath propertyPath = null;

      if (!StringUtility.IsNullOrEmpty (_propertyPathIdentifier))
        propertyPath = BusinessObjectPropertyPath.ParseDynamic (businessObjectClass, _propertyPathIdentifier);

      _propertyPath = null;
      _isPropertyPathEvaluated = false;

      return propertyPath;
    }

    /// <summary> 
    ///   Sets the <see cref="IBusinessObjectPropertyPath"/> mananged by this <see cref="PropertyPathBinding"/>.
    /// </summary>
    public void SetPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      _propertyPath = propertyPath;
      _propertyPathIdentifier = (propertyPath == null) ? string.Empty : propertyPath.Identifier;
      _isPropertyPathEvaluated = true;
      _isDynamic = false;
    }

    [DefaultValue (false)]
    [Category ("Data")]
    public bool IsDynamic
    {
      get { return _isDynamic; }
      set { _isDynamic = value; }
    }

    /// <summary> 
    ///   Gets or sets the <see cref="string"/> representing the 
    ///   <see cref="IBusinessObjectPropertyPath"/> mananged by this <see cref="PropertyPathBinding"/>.
    /// </summary>
    /// <value> 
    ///   A <see cref="string"/> formatted as a valid property path. 
    /// </value>
    [Editor (typeof (PropertyPathPickerEditor), typeof (UITypeEditor))]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [Description ("A string representing a valid property path.")]
    //  No default value
    public string PropertyPathIdentifier
    {
      get
      {
        return _propertyPathIdentifier;
      }
      set
      {
        _propertyPathIdentifier = value;
        _propertyPath = null;
        _isPropertyPathEvaluated = false;
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IBusinessObjectClass BusinessObjectClass
    {
      get
      {
        IBusinessObjectReferenceProperty property = null;
        if (OwnerControl != null)
          property = OwnerControl.Property as IBusinessObjectReferenceProperty;
        if (property != null)
          return property.ReferenceClass;
        if (DataSource != null)
          return DataSource.BusinessObjectClass;
        return null;
      }
    }
  }

}
