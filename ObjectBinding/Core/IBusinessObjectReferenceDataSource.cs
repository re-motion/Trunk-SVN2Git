/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   This interface provides functionality to access to the <see cref="IBusinessObject"/> returned by other 
  ///   data sources using the specified <see cref="IBusinessObjectReferenceProperty"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Through the use of an <b>IBusinessObjectReferenceDataSource</b> it is possible to access a referenced 
  ///     <see cref="IBusinessObject"/> identified by the <see cref="IBusinessObjectReferenceProperty"/> from the 
  ///     primary <see cref="IBusinessObject"/> connected to the <see cref="ReferencedDataSource"/>. The referenced
  ///     object is then used as this <see cref="IBusinessObjectReferenceDataSource"/>'s <see cref="BusinessObject"/>, 
  ///     allowing the cascading of <see cref="IBusinessObject"/> objects.
  ///     <note type="inotes">
  ///       The <b>IBusinessObjectReferenceDataSource</b> is usually implemented as a cross between the
  ///       <see cref="IBusinessObjectDataSource"/> from which this interface is inherited and an 
  ///       <see cref="IBusinessObjectBoundControl"/> or <see cref="IBusinessObjectBoundEditableControl"/>.
  ///     </note>
  ///   </para>
  ///   <para>
  ///     <see cref="BusinessObjectReferenceDataSource"/> provides an implementation of this interface. Since the
  ///     <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> is determined by the selected 
  ///     <see cref="ReferenceProperty"/>, the generic <see cref="BusinessObjectReferenceDataSource"/> will be sufficient
  ///     for most (or possibly all) specialized business object models.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectDataSource"/>
  public interface IBusinessObjectReferenceDataSource: IBusinessObjectDataSource
  {
    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
    ///   <see cref="IBusinessObject"/> to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
    ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
    /// </value>
    /// <remarks>
    ///   Usually identical to <see cref="IBusinessObjectBoundControl.Property"/>, i.e. <b>ReferenceProperty</b>
    ///   gets or sets the current value of <see cref="IBusinessObjectBoundControl.Property"/>.
    /// </remarks>
    IBusinessObjectReferenceProperty ReferenceProperty { get; set; }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
    ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
    /// </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
    ///   <see cref="IBusinessObjectReferenceDataSource"/> connects.
    ///  </value>
    /// <remarks>
    ///   Usually identical to <see cref="IBusinessObjectBoundControl.DataSource"/>, i.e. <b>ReferencedDataSource</b>
    ///   gets the current value of <see cref="IBusinessObjectBoundControl.DataSource"/>.
    /// </remarks>
    IBusinessObjectDataSource ReferencedDataSource { get; }
  }
}
