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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.SampleTypes
{
  public class MixinAddingUnidirectionalRelation1 : DomainObjectMixin<DomainObject>
  {
    public Computer Computer
    {
      get { return Properties[typeof (MixinAddingUnidirectionalRelation1), "Computer"].GetValue<Computer>(); }
      set { Properties[typeof (MixinAddingUnidirectionalRelation1), "Computer"].SetValue (value); }
    }
  }
}
