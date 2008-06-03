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
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="IMetadataFactory"/> interface provides factory methods for creating the reflection objects used to create the 
  /// <see cref="BindableObjectClass"/> and it's associated attributes.
  /// </summary>
  public interface IMetadataFactory
  {
    IClassReflector CreateClassReflector (Type targetType, BindableObjectProvider businessObjectProvider);
    IPropertyFinder CreatePropertyFinder (Type concreteType);
    PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider);
  }
}
