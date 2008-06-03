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

namespace Remotion.ObjectBinding
{

/// <summary>
///   This interface is used for the <see cref="ITypeDescriptorContext.Instance"/> argument of the 
///   Visual Studio .NET designer editor. 
/// </summary>
/// <remarks>
///   <para>
///     The <see cref="Remotion.ObjectBinding.Design.PropertyPathPickerControl"/> uses this interface 
///     to query the <see cref="IBusinessObjectClass"/> of an <see cref="IBusinessObjectReferenceProperty"/>
///     or an <see cref="IBusinessObjectDataSource"/>, respectively.
///   </para><para>
///     Implemented by <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocSimpleColumnDefinition"/> 
///     and <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.PropertyPathBinding"/>.
///   </para>
/// </remarks>
public interface IBusinessObjectClassSource
{
  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of an <see cref="IBusinessObjectReferenceProperty"/>
  ///   or an <see cref="IBusinessObjectDataSource"/>, respectively.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectClass"/> to be queried for the properties offered by the 
  ///   <see cref="Remotion.ObjectBinding.Design.PropertyPathPickerControl"/>.
  /// </value>
  IBusinessObjectClass BusinessObjectClass { get; }
}

}
