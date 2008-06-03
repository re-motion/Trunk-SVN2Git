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
public interface IRelationEndPointDefinition : INullObject
{
  RelationDefinition RelationDefinition { get; }
  ClassDefinition ClassDefinition { get; }
  string PropertyName { get; }
  Type PropertyType { get; }
  bool IsPropertyTypeResolved { get; }
  string PropertyTypeName { get; }
  bool IsMandatory { get; }
  CardinalityType Cardinality { get; }
  bool IsVirtual { get; }

  bool CorrespondsTo (string classID, string propertyName);
  void SetRelationDefinition (RelationDefinition relationDefinition);
}
}
