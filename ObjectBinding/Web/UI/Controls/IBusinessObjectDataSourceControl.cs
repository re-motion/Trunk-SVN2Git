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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///  The <b>IBusinessObjectDataSourceControl</b> interface defines the methods and 
  ///  properties required to implement a control that provides an <see cref="IBusinessObjectDataSource"/>
  ///  to controls of type <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> 
  ///  or <b>ASCX User Control</b>.
  /// </summary>
  /// <include file='doc\include\UI\Controls\IBusinessObjectDataSourceControl.xml' path='IBusinessObjectDataSourceControl/Class/*' />
  public interface IBusinessObjectDataSourceControl : IBusinessObjectDataSource, IControl
  {
    /// <summary> Prepares all bound controls implementing <see cref="IValidatableControl"/> for validation. </summary>
    void PrepareValidation ();

    /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
    /// <returns> <see langword="true"/> if no validation errors where found. </returns>
    bool Validate ();
  }
}
