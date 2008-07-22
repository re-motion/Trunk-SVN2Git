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

namespace Remotion.Data.UnitTests.DomainObjects.TestDomain
{
  [DBTable ("TableWithGuidKey")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithGuidKey : TestDomainBase
  {
    public static ClassWithGuidKey NewObject ()
    {
      return NewObject<ClassWithGuidKey> ().With();
    }

    protected ClassWithGuidKey()
    {
    }

    [DBBidirectionalRelation ("ClassWithGuidKeyOptional")]
    public abstract ClassWithValidRelations ClassWithValidRelationsOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKeyNonOptional")]
    [Mandatory]
    public abstract ClassWithValidRelations ClassWithValidRelationsNonOptional { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public abstract ClassWithInvalidRelation ClassWithInvalidRelation { get; set; }

    [DBBidirectionalRelation ("ClassWithGuidKey")]
    public abstract ClassWithRelatedClassIDColumnAndNoInheritance ClassWithRelatedClassIDColumnAndNoInheritance { get; set; }
  }
}
