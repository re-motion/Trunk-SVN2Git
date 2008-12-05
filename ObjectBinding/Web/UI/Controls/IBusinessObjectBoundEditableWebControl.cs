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
