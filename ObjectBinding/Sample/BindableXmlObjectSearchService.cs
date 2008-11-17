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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class BindableXmlObjectSearchService : ISearchAvailableObjectsService
  {
    public BindableXmlObjectSearchService ()
    {
    }

    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    public IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, ISearchAvailableObjectsArguments searchArguments)
    {
      ReferenceProperty referenceProperty = ArgumentUtility.CheckNotNullAndType<ReferenceProperty> ("property", property);
      BindableObjectClass bindableObjectClass = (BindableObjectClass) referenceProperty.ReferenceClass;

      return (IBusinessObject[]) ArrayUtility.Convert (
                                     XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (bindableObjectClass.TargetType),
                                     typeof (IBusinessObject));
    }
  }
}
