/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   Extends an <see cref="IBusinessObjectBoundWebControl"/> with functionality for validating the control's 
  ///   <see cref="IBusinessObjectBoundControl.Value"/> and writing it back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     See <see cref="IBusinessObjectBoundEditableControl.SaveValue"/> for a description of the data binding 
  ///     process.
  ///   </para><para>
  ///     See <see cref="BusinessObjectBoundEditableWebControl"/> for the <see langword="abstract"/> default 
  ///     implementation.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundWebControl"/>
  /// <seealso cref="IBusinessObjectBoundEditableControl"/>
  /// <seealso cref="IValidatableControl"/>
  /// <seealso cref="IBusinessObjectDataSourceControl"/>
  public interface IBusinessObjectBoundEditableWebControl
      :
          IBusinessObjectBoundWebControl,
          IBusinessObjectBoundEditableControl,
          IValidatableControl,
          IEditableControl
  {
  }
}
