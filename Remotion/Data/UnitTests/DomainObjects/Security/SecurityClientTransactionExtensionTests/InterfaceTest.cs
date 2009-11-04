// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
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
