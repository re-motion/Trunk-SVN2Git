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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  internal interface IPropertyAccessorStrategy
  {
    Type GetPropertyType (PropertyDefinition propertyDefinition, IRelationEndPointDefinition relationEndPointDefinition);
    bool HasChanged (PropertyAccessor propertyAccessor, ClientTransaction transaction);
    bool HasBeenTouched (PropertyAccessor accessor, ClientTransaction transaction);
    bool IsNull (PropertyAccessor propertyAccessor, ClientTransaction transaction);
    object GetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction);
    void SetValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction, object value);
    object GetOriginalValueWithoutTypeCheck (PropertyAccessor propertyAccessor, ClientTransaction transaction);
  }
}
