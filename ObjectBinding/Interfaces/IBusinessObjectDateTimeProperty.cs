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

namespace Remotion.ObjectBinding
{
  /// <summary> The <b>IBusinessObjectDateTimeProperty</b> interface is used for accessing <see cref="DateTime"/> values. </summary>
  /// <remarks>
  /// <note type="inotes">
  /// The objects returned for this property must implement the <see cref="IFormattable"/> interface in order to be displayed by the 
  /// <see cref="IBusinessObject.GetPropertyString(IBusinessObjectProperty,string)"/> methods.
  /// </note>
  /// </remarks>
  public interface IBusinessObjectDateTimeProperty : IBusinessObjectProperty
  {
    DateTimeType Type { get; }
  }

  /// <summary>
  /// The <see cref="DateTimeType"/> enum defines the list of possible data types supported by the <see cref="IBusinessObjectDateTimeProperty"/>.
  /// </summary>
  public enum DateTimeType
  {
    DateTime,
    Date
  }
}
