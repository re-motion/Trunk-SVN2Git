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
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  [Uses (typeof (StubStoragePersistentMixin))]
  [Uses (typeof (NullMixin))]
  [DBTable]
  [StorageProviderStub]
  public class StubStorageTargetClassForPersistentMixin : DomainObject
  {
    public static StubStorageTargetClassForPersistentMixin NewObject ()
    {
      return NewObject<StubStorageTargetClassForPersistentMixin> ().With ();
    }

    public static StubStorageTargetClassForPersistentMixin GetObject (ObjectID id)
    {
      return GetObject<StubStorageTargetClassForPersistentMixin> (id);
    }
  }
}
