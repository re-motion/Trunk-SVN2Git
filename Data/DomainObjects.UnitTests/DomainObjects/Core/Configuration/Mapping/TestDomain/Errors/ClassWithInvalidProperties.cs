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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidProperties : DomainObject
  {
    protected ClassWithInvalidProperties ()
    {
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException (); }
    }

    [StringProperty]
    private ClassWithInvalidProperties PropertyWithStringAttribute
    {
      get { throw new NotImplementedException (); }
    }

    [BinaryProperty]
    private ClassWithInvalidProperties PropertyWithBinaryAttribute
    {
      get { throw new NotImplementedException (); }
    }
  }
}
