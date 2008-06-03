/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   Provides functionality for binding an <see cref="ISmartControl"/> to an <see cref="IBusinessObject"/> using
  ///   an <see cref="IBusinessObjectDataSourceControl"/>. 
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     See the <see href="Remotion.ObjectBinding.html">Remotion.ObjectBinding</see> namespace documentation for general information on the 
  ///     data binding process.
  ///   </para><para>
  ///     See <see cref="BusinessObjectBoundWebControl"/> for the <see langword="abstract"/> default implementation.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundControl"/>
  /// <seealso cref="ISmartControl"/>
  /// <seealso cref="IBusinessObjectBoundEditableWebControl"/>
  /// <seealso cref="IBusinessObjectDataSourceControl"/>
  public interface IBusinessObjectBoundWebControl : IBusinessObjectBoundControl, ISmartControl, IControlWithDesignTimeSupport
  {
    /// <summary>
    ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the 
    ///   <see cref="IBusinessObjectDataSource"/> this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    /// <value>A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current naming container.</value>
    /// <remarks>
    ///   The value of this property is used to find the <see cref="IBusinessObjectDataSourceControl"/> in the controls collection.
    ///   <note type="inotes">Apply an <see cref="BusinessObjectDataSourceControlConverter"/> when implementing the property.</note>
    /// </remarks>
    string DataSourceControl { get; set; }
  }
}
