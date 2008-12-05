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