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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class InterfaceTest
  {

    [Test]
    public void TestInterfaceMembers ()
    {
      IClientTransactionExtension extension = new SecurityClientTransactionExtension ();
      extension.ObjectsLoaded (null, null);
      extension.ObjectDeleted (null, null);
      extension.PropertyValueRead (null, null, null, null, ValueAccess.Current);
      extension.PropertyValueChanged (null, null, null, null, null);
      extension.RelationRead (null, null, null, (DomainObjectCollection) null, ValueAccess.Current);
      extension.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
      extension.RelationChanged (null, null, null);
      extension.Committing (null, null);
      extension.Committed (null, null);
      extension.RollingBack (null, null);
      extension.RolledBack (null, null);
    }
  }
}
