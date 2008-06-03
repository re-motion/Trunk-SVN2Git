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

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Indicates the type of a <see cref="QueryDefinition"/>.
/// </summary>
public enum QueryType
{
  /// <summary>
  /// Instances of a <see cref="QueryDefinition"/> return a collection of <see cref="DomainObject"/>s.
  /// </summary>
  Collection = 0,

  /// <summary>
  /// Instances of a <see cref="QueryDefinition"/> return only a single value.
  /// </summary>
  Scalar = 1
}
}
