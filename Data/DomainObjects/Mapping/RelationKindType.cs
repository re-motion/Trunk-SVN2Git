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

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Defines the kind of a given <see cref="RelationDefinition"/>.
  /// </summary>
  public enum RelationKindType
  {
    /// <summary>
    /// There is a one-to-one relationship between referenced objects.
    /// </summary>
    OneToOne,
    /// <summary>
    /// There is a one-to-many (or many-to-one) relationship between referenced objects.
    /// </summary>
    OneToMany,
    /// <summary>
    /// There is a one-to-many relationship between referenced objects, but only the "many" side has a reference to its one related object; there
    /// is no back-reference to the many objects from the "one" side.
    /// </summary>
    Unidirectional
  }
}