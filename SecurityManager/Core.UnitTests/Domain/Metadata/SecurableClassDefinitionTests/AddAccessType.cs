// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.SecurableClassDefinitionTests
{
  [TestFixture]
  public class AddAccessType : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    [Test]
    public void AddsTwoNewAccessTypes ()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var classDefinitionWrapper = new SecurableClassDefinitionWrapper (SecurableClassDefinition.NewObject());
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        classDefinitionWrapper.SecurableClassDefinition.EnsureDataAvailable();
        Assert.That (classDefinitionWrapper.SecurableClassDefinition.State, Is.EqualTo (StateType.Unchanged));

        classDefinitionWrapper.SecurableClassDefinition.AddAccessType (accessType0);
        classDefinitionWrapper.SecurableClassDefinition.AddAccessType (accessType1);

        Assert.That (classDefinitionWrapper.SecurableClassDefinition.AccessTypes, Is.EqualTo (new[] { accessType0, accessType1 }));
        var references = classDefinitionWrapper.AccessTypeReferences;
        Assert.That (((AccessTypeReference) references[0]).Index, Is.EqualTo (0));
        Assert.That (((AccessTypeReference) references[1]).Index, Is.EqualTo (1));
        Assert.That (classDefinitionWrapper.SecurableClassDefinition.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void AddsPermissionsForNewAccessType()
    {
      var accessType0 = AccessTypeDefinition.NewObject();
      var accessType1 = AccessTypeDefinition.NewObject();
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.AddAccessType (accessType0);

      var testHelper = new AccessControlTestHelper (ClientTransaction.Current);
      var acls = new List<AccessControlList>();
      acls.Add (testHelper.CreateStatefulAcl (securableClassDefinition));
      acls.Add (testHelper.CreateStatelessAcl (securableClassDefinition));

      foreach (var acl in acls)
        acl.CreateAccessControlEntry();

      securableClassDefinition.AddAccessType (accessType1);
      foreach (var acl in acls)
      {
        Assert.That (acl.AccessControlEntries[0].Permissions.Count, Is.EqualTo (2));
        Assert.That (acl.AccessControlEntries[0].Permissions[1].AccessType, Is.SameAs (accessType1));
        Assert.That (acl.AccessControlEntries[0].Permissions[1].Allowed, Is.Null);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The access type 'Test' has already been added to the securable class definition 'Class'.\r\nParameter name: accessType")]
    public void FailsForExistingAccessType ()
    {
      var accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.Name = "Class";
      securableClassDefinition.AddAccessType (accessType);
      securableClassDefinition.AddAccessType (accessType);
    }
  }
}