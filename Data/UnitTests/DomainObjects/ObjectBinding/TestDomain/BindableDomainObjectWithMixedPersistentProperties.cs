/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [DBTable]
  [Uses (typeof (MixinAddingPersistentProperties))]
  public class BindableDomainObjectWithMixedPersistentProperties : BindableDomainObject
  {
    public static BindableDomainObjectWithMixedPersistentProperties NewObject ()
    {
      return NewObject<BindableDomainObjectWithMixedPersistentProperties>().With();
    }
  }
}