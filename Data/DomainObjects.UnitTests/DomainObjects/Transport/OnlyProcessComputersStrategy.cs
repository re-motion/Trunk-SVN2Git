/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Transport
{
  public class OnlyProcessComputersStrategy : IGraphTraversalStrategy
  {
    public bool ShouldProcessObject (DomainObject domainObject)
    {
      return domainObject is Computer;
    }

    public bool ShouldFollowLink (DomainObject root, DomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
    {
      return true;
    }
  }
}
