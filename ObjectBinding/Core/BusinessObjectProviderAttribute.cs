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

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Use the <see cref="BusinessObjectProviderAttribute"/> to associate a <see cref="IBusinessObjectProvider"/> with a business object implementation.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
  public abstract class BusinessObjectProviderAttribute : Attribute
  {
    private readonly Type _businessObjectProviderType;

    protected BusinessObjectProviderAttribute (Type businessObjectProviderType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("businessObjectProviderType", businessObjectProviderType, typeof (IBusinessObjectProvider));
      _businessObjectProviderType = businessObjectProviderType;
    }

    public Type BusinessObjectProviderType
    {
      get { return _businessObjectProviderType; }
    }
  }
}
