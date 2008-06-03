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
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="IStorageSpecificIdentifierAttribute"/> interface is used as a storage provider indifferent marker interface for more 
  /// conrete attributes such as the <see cref="DBColumnAttribute"/>.
  /// </summary>
  public interface IStorageSpecificIdentifierAttribute : IMappingAttribute
  {
    /// <summary>
    /// Gets the <see cref="string"/> used as the identifier.
    /// </summary>
    //TODO: Always allow null values.
    string Identifier { get; }
  }
}
