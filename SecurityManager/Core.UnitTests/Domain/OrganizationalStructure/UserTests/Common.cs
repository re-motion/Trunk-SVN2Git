// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Linq;
using System.Security.Principal;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class Common : UserTestBase
  {
    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UserName_SameNameTwice ()
    {
      CreateUser();
      CreateUser();
      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    public void Roles_PropertyWriteAccessGranted ()
    {
      User user = CreateUser();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        SecurityClientFactory securityClientFactory = new SecurityClientFactory ();
        var securityClient = securityClientFactory.CreatedStubbedSecurityClient<User> (SecurityManagerAccessTypes.AssignRole);
        
        Assert.That (securityClient.HasPropertyWriteAccess (user, "Roles"), Is.True);
      }
    }

    [Test]
    public void Roles_PropertyWriteAccessDenied ()
    {
      User user = CreateUser ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        SecurityClientFactory securityClientFactory = new SecurityClientFactory ();
        var securityClient = securityClientFactory.CreatedStubbedSecurityClient<User> ();
        
        Assert.That (securityClient.HasPropertyWriteAccess (user, "Roles"), Is.False);
      }
    }
  }
}
