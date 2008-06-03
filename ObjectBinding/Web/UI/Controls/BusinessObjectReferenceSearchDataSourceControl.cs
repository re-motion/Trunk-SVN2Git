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
using System.ComponentModel;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectReferenceSearchDataSourceControl</b> is used to supply an <see cref="IBusinessObjectClass"/>
  ///   to an <see cref="IBusinessObjectBoundWebControl"/> used for displaying a search result.
  /// </summary>
  /// <remarks>
  ///   Since a search result is usually an <see cref="IBusinessObject"/> list without an actual parent object
  ///   to connect to the data source, normal data binding is not possible. By using the 
  ///   <b>BusinessObjectReferenceSearchDataSourceControl</b> it is possible to provide the meta data to the bound
  ///   controls dispite the lack of a parent object.
  /// </remarks>
  public class BusinessObjectReferenceSearchDataSourceControl: BusinessObjectReferenceDataSourceControl
  {
    /// <summary> Multiplicity is always supported. </summary>
    /// <param name="isList"> Not evaluated. </param>
    /// <returns> Always <see langword="true"/>. </returns>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return true;
    }

    /// <summary> Not supported by <see cref="BusinessObjectReferenceSearchDataSourceControl"/>. </summary>
    /// <param name="interim"> Not evaluated. </param>
    public override void LoadValues (bool interim)
    {
      throw new NotSupportedException ("Use BusinessObjectReferenceDataSourceControl for actual data.");
    }

    /// <summary>
    ///   Gets or sets the current <see cref="DataSourceMode"/> of this 
    ///   <see cref="BusinessObjectReferenceSearchDataSourceControl"/>.
    /// </summary>
    /// <value> <see cref="DataSourceMode.Search"/>. </value>
    /// <exception cref="NotSupportedException"> Thrown upon an attempt to set a value other than <see cref="DataSourceMode.Search"/>. </exception>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [DefaultValue (DataSourceMode.Search)]
    public override DataSourceMode Mode
    {
      get { return DataSourceMode.Search; }
      set { if (value != DataSourceMode.Search) throw new NotSupportedException ("BusinessObjectReferenceSearchDataSourceControl supports only DataSourceMode.Search."); }
    }
  }
}
