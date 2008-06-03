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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  public abstract class ClassWithOneSideRelationPropertiesNotInMapping : DomainObject
  {
    protected ClassWithOneSideRelationPropertiesNotInMapping ()
    {
    }

    [DBBidirectionalRelation ("BaseBidirectionalOneToOne")]
    public abstract ClassWithManySideRelationProperties BaseBidirectionalOneToOne { get; set; }

    [DBBidirectionalRelation ("BaseBidirectionalOneToMany", SortExpression = "The Sort Expression")]
    public abstract ObjectList<ClassWithManySideRelationProperties> BaseBidirectionalOneToMany { get; }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToOne")]
    private ClassWithManySideRelationProperties BasePrivateBidirectionalOneToOne
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    [DBBidirectionalRelation ("BasePrivateBidirectionalOneToMany", SortExpression = "The Sort Expression")]
    private ObjectList<ClassWithManySideRelationProperties> BasePrivateBidirectionalOneToMany
    {
      get { throw new NotImplementedException (); }
    }
  }
}
