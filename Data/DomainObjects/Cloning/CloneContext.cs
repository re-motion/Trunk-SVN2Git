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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Cloning
{
  public class CloneContext
  {
    private readonly DomainObjectCloner _cloner;
    private readonly SimpleDataStore<DomainObject, DomainObject> _clones = new SimpleDataStore<DomainObject, DomainObject> ();
    private readonly Queue<Tuple<DomainObject, DomainObject>> _shallowClones = new Queue<Tuple<DomainObject, DomainObject>> ();

    public CloneContext (DomainObjectCloner cloner)
    {
      ArgumentUtility.CheckNotNull ("cloner", cloner);
      _cloner = cloner;
    }

    public virtual Queue<Tuple<DomainObject, DomainObject>> ShallowClones
    {
      get { return _shallowClones; }
    }

    public virtual T GetCloneFor<T> (T domainObject)
        where T : DomainObject
    {
      return (T) _clones.GetOrCreateValue (domainObject, delegate (DomainObject cloneTemplate)
      {
        DomainObject clone = _cloner.CreateValueClone (cloneTemplate);
        ShallowClones.Enqueue (Tuple.NewTuple (cloneTemplate, clone));
        return clone;
      });
    }
  }
}