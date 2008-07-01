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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Defines if and how a property is managed by the persistence framework.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class StorageClassAttribute : Attribute, IMappingAttribute
  {
    private readonly StorageClass _storageClass;

    public StorageClassAttribute (StorageClass storageClass)
    {
      ArgumentUtility.CheckValidEnumValue ("storageClass", storageClass);
      _storageClass = storageClass;
    }

    public StorageClass StorageClass
    {
      get { return _storageClass; }
    }
  }
}
