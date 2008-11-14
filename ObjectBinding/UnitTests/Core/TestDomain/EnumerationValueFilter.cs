/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class EnumerationValueFilter:IEnumerationValueFilter
  {
    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("property", property);

      return (int) value.Value % 2 == 1;
    }
  }
}