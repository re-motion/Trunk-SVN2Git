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
