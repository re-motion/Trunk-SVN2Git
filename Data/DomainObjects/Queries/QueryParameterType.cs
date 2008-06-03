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

namespace Remotion.Data.DomainObjects.Queries
{
/// <summary>
/// Indicates the type of a <see cref="QueryParameter"/>.
/// </summary>
public enum QueryParameterType
{
  /// <summary>
  /// Instances of <see cref="QueryParameter"/> will be treated as parameters in the query.
  /// </summary>
  Value = 0,

  /// <summary>
  /// Instances of <see cref="QueryParameter"/> will replaced inline into the query.
  /// </summary>
  Text = 1
}
}
