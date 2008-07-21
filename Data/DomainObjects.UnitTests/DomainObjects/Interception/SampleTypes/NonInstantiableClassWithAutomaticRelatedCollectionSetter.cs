/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class NonInstantiableClassWithAutomaticRelatedCollectionSetter : DomainObject
  {
    public static NonInstantiableClassWithAutomaticRelatedCollectionSetter NewObject ()
    {
      return NewObject<NonInstantiableClassWithAutomaticRelatedCollectionSetter> ().With ();
    }

    protected NonInstantiableClassWithAutomaticRelatedCollectionSetter()
    {
    }

    [DBBidirectionalRelation ("RelatedObjects")]
    public abstract NonInstantiableClassWithAutomaticRelatedCollectionSetter Parent { get; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<NonInstantiableClassWithAutomaticRelatedCollectionSetter> RelatedObjects { get; set; }
  }
}
