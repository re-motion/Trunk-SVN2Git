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
using Remotion.Collections;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class OtherBusinessObjectImplementationProvider : IBusinessObjectProvider
  {
    public IBusinessObjectService GetService (Type serviceType)
    {
      throw new NotImplementedException();
    }

    public T GetService<T> () where T: IBusinessObjectService
    {
      throw new NotImplementedException();
    }

    public void AddService (Type serviceType, IBusinessObjectService service)
    {
      throw new NotImplementedException();
    }

    public void AddService<T> (T service) where T: IBusinessObjectService
    {
      throw new NotImplementedException();
    }

    public char GetPropertyPathSeparator ()
    {
      throw new NotImplementedException();
    }

    public IBusinessObjectPropertyPath CreatePropertyPath (IBusinessObjectProperty[] properties)
    {
      throw new NotImplementedException();
    }

    public string GetNotAccessiblePropertyStringPlaceHolder ()
    {
      throw new NotImplementedException();
    }
  }
}
