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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   This <see langword="abstract"/> class provides base functionality usually required by an implementation of
  ///   <see cref="IBusinessObjectReferenceDataSource"/>.
  /// </summary>
  /// <remarks>
  ///   Only members with functionality common to all implementations of <see cref="IBusinessObjectReferenceDataSource"/> 
  ///   have been implemented. The actual implementation of the <see cref="IBusinessObjectBoundEditableControl"/> 
  ///   interface is left to the child class. 
  /// </remarks>
  /// <seealso cref="IBusinessObjectReferenceDataSource"/>
  public abstract class BusinessObjectReferenceDataSourceBase : BusinessObjectDataSource, IBusinessObjectReferenceDataSource
  {
    /// <summary>
    ///   The <see cref="IBusinessObject"/> accessed through <see cref="ReferenceProperty"/> and provided as 
    ///   the <see cref="BusinessObject"/>.
    /// </summary>
    private IBusinessObject _businessObject;

    /// <summary>
    ///   A flag that is cleared when the <see cref="BusinessObject"/> is loaded from or saved to the
    ///   <see cref="ReferencedDataSource"/>.
    /// </summary>
    private bool _hasBusinessObjectChanged = false;

    /// <summary>
    ///   See <see cref="IBusinessObjectReferenceDataSource.ReferenceProperty">IBusinessObjectReferenceDataSource.ReferenceProperty</see>
    ///   for information on how to implement this <see langword="abstract"/> property.
    /// </summary>
    public abstract IBusinessObjectReferenceProperty ReferenceProperty { get; }

    /// <summary>
    ///   See <see cref="IBusinessObjectReferenceDataSource.ReferencedDataSource">IBusinessObjectReferenceDataSource.ReferencedDataSource</see>
    ///   for information on how to implement this <see langword="abstract"/> property.
    /// </summary>
    public abstract IBusinessObjectDataSource ReferencedDataSource { get; }

    /// <summary> 
    ///   Loads the <see cref="BusinessObject"/> from the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/> and populates the bound controls using 
    ///   <see cref="BusinessObjectDataSource.LoadValues"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <remarks>
    ///   For details on <see cref="LoadValue"/>, 
    ///   see <see cref="IBusinessObjectDataSource.LoadValues">IBusinessObjectDataSource.LoadValues</see>.
    /// </remarks>
    /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
    public void LoadValue (bool interim)
    {
      // load value from "parent" data source
      if (HasValidBinding) //->  requires Businessobject=set, not required for edge cases
      {
        //what about multiple LoadValues commands??? maybe check created and do an auto-delete?
        //if created and interim=true do not load object from parent, i.e. skip down to LoadValues(interim)
        //if created and interim=false and SupportsDelete -> delete before execuuting remaining logic

        //_hasBusinessObjectCreated = false;
        var businessObject = (IBusinessObject) ReferencedDataSource.BusinessObject.GetProperty (ReferenceProperty);
        if (businessObject == null && SupportsDefaultValueSemantics)
        {
          businessObject = ReferenceProperty.CreateDefaultValue (ReferencedDataSource.BusinessObject);
          //_hasBusinessObjectCreated = true;
        }

        BusinessObject = businessObject;
        _hasBusinessObjectChanged = false;
      }

      // load values into "child" controls
      LoadValues (interim);
    }

    /// <summary> 
    ///   Saves the values from the bound controls using <see cref="BusinessObjectDataSource.SaveValues"/>
    ///   and writes the <see cref="BusinessObject"/> back into the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <remarks>
    ///   For details on <see cref="SaveValue"/>, 
    ///   see <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
    /// </remarks>
    /// <seealso cref="IBusinessObjectBoundEditableControl.SaveValue">IBusinessObjectBoundEditableControl.SaveValue</seealso>
    public void SaveValue (bool interim)
    {
      if (!interim && IsBusinessObjectSetToDefaultValue())
      {
        if (ReferenceProperty.SupportsDelete)
          ReferenceProperty.Delete (ReferencedDataSource.BusinessObject, BusinessObject);

        BusinessObject = null;
      }

      // save values from "child" controls
      SaveValues (interim);

      // if required, save value into "parent" data source
      if (HasValidBinding && RequiresWriteBack)
      {
        ReferencedDataSource.BusinessObject.SetProperty (ReferenceProperty, BusinessObject);
        _hasBusinessObjectChanged = false;
        //_hasBusinessObjectCreated = false;
      }
    }

    /// <summary>
    /// Evaluates whether the <see cref="BusinessObjectReferenceDataSource"/> contains a value that will be written back into the 
    /// <see cref="ReferencedDataSource"/> during <see cref="SaveValue"/>.
    /// </summary>
    /// <returns></returns>
    public bool HasValue ()
    {
      if (BusinessObject == null)
        return false;
      else if (IsBusinessObjectSetToDefaultValue())
        return false;
      else
        return true;
    }

    /// <summary> 
    ///   Gets a flag that is <see langword="true"/> if the <see cref="BusinessObject"/> has been set since the last
    ///   call to <see cref="LoadValue"/> or <see cref="SaveValue"/>.
    /// </summary>
    public bool HasBusinessObjectChanged
    {
      get { return _hasBusinessObjectChanged; }
    }

    /// <summary>
    ///   Tests whether the <see cref="IBusinessObjectReferenceDataSource"/> can be bound to the 
    ///   <paramref name="property"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///   <see langword="true"/> if the <paremref name="property"/> is of type 
    ///   <see cref="IBusinessObjectReferenceProperty"/>.
    /// </returns>
    /// <seealso cref="IBusinessObjectBoundControl.SupportsProperty">IBusinessObjectBoundControl.SupportsProperty</seealso>
    public bool SupportsProperty (IBusinessObjectProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      return property is IBusinessObjectReferenceProperty;
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> accessed through the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> An <see cref="IBusinessObject"/> or <see langword="null"/>. </value>
    public override IBusinessObject BusinessObject
    {
      get { return _businessObject; }
      set
      {
        _businessObject = value;
        _hasBusinessObjectChanged = true;
      }
    }

    /// <summary> 
    ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectClass"/> or <see langword="null"/> if no <see cref="ReferenceProperty"/> is set.
    /// </value>
    public override IBusinessObjectClass BusinessObjectClass
    {
      get
      {
        IBusinessObjectReferenceProperty property = ReferenceProperty;
        return (property == null) ? null : property.ReferenceClass;
      }
    }

    private bool HasValidBinding
    {
      get { return ReferencedDataSource != null && ReferencedDataSource.BusinessObject != null && ReferenceProperty != null; }
    }

    private bool RequiresWriteBack
    {
      get { return (_hasBusinessObjectChanged || /*_hasBusinessObjectCreated ||*/ ReferenceProperty.ReferenceClass.RequiresWriteBack); }
    }

    private bool SupportsDefaultValueSemantics
    {
      get { return (Mode == DataSourceMode.Edit && ReferenceProperty.SupportsDefaultValue); }
    }

    private bool IsBusinessObjectSetToDefaultValue ()
    {
      if (HasValidBinding && BusinessObject != null && ReferenceProperty.SupportsDefaultValue)
      {
        if (BoundControls.Any (c => c.HasValue))
          return false;

        var properties = BoundControls.Select (c => c.Property).Distinct ().ToArray ();
        return ReferenceProperty.IsDefaultValue (ReferencedDataSource.BusinessObject, BusinessObject, properties);
      }
      else
      {
        return false;
      }
    }
  }
}
