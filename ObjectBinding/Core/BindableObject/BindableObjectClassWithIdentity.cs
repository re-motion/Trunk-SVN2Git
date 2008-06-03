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

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectClassWithIdentity : BindableObjectClass, IBusinessObjectClassWithIdentity
  {
    private readonly Type _getObjectServiceType;

    public BindableObjectClassWithIdentity (Type concreteType, BindableObjectProvider businessObjectProvider)
        : base (concreteType, businessObjectProvider)
    {
      _getObjectServiceType = GetGetObjectServiceType();
    }

    public IBusinessObjectWithIdentity GetObject (string uniqueIdentifier)
    {
      IGetObjectService service = GetGetObjectService();
      return service.GetObject (this, uniqueIdentifier);
    }

    private IGetObjectService GetGetObjectService ()
    {
      IGetObjectService service = (IGetObjectService) BusinessObjectProvider.GetService (_getObjectServiceType);
      if (service == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' required for loading objectes of type '{1}' is not registered with the '{2}' associated with this type.",
                _getObjectServiceType.FullName,
                TargetType.FullName,
                typeof (BusinessObjectProvider).FullName));
      }
      return service;
    }

    private Type GetGetObjectServiceType ()
    {
      GetObjectServiceTypeAttribute attribute = AttributeUtility.GetCustomAttribute<GetObjectServiceTypeAttribute> (ConcreteType, true);
      if (attribute == null)
        return typeof (IGetObjectService);
      return attribute.Type;
    }
  }
}
