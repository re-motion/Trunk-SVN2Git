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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  [CLSCompliant (false)]
  [Extends (typeof (ClassWithAllDataTypes), MixinTypeArguments = new Type[] { typeof (ClassWithAllDataTypes) })]
  [Serializable]
  public class MixinWithAccessToDomainObjectProperties<TDomainObject> : DomainObjectMixin<TDomainObject>
      where TDomainObject : DomainObject
  {
    public bool OnDomainObjectCreatedCalled = false;
    public bool OnDomainObjectLoadedCalled = false;
    public LoadMode OnDomainObjectLoadedLoadMode;

    [StorageClassNone]
    public new ObjectID ID
    {
      get { return base.ID; }
    }

    public new Type GetPublicDomainObjectType ()
    {
      return base.GetPublicDomainObjectType ();
    }

    [StorageClassNone]
    public new StateType State
    {
      get { return base.State; }
    }

    [StorageClassNone]
    public new bool IsDiscarded
    {
      get { return base.IsDiscarded; }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    [StorageClassNone]
    public new TDomainObject This
    {
      get { return base.This; }
    }

    protected override void OnDomainObjectCreated ()
    {
      OnDomainObjectCreatedCalled = true;
    }

    protected override void OnDomainObjectLoaded (LoadMode loadMode)
    {
      OnDomainObjectLoadedCalled = true;
      OnDomainObjectLoadedLoadMode = loadMode;
    }
  }
}
