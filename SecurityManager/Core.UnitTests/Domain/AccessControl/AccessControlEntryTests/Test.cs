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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetAllowedAccessTypes_EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAllowedAccessTypes_ReadAllowed ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce (ace, null);
      _testHelper.CreateDeleteAccessTypeAndAttachToAce (ace, false);

      Assert.That (ace.GetAllowedAccessTypes(), Is.EquivalentTo (new[] { readAccessType }));
    }

    [Test]
    public void GetDeniedAccessTypes_DeleteDenied ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);
      _testHelper.CreateWriteAccessTypeAndAttachToAce (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessTypeAndAttachToAce (ace, false);

      Assert.That (ace.GetDeniedAccessTypes(), Is.EquivalentTo (new[] { deleteAccessType }));
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, null);

      ace.AllowAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (1, allowedAccessTypes.Length);
      Assert.Contains (accessType, allowedAccessTypes);
    }

    [Test]
    public void DenyAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, null);

      ace.DenyAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.That (allowedAccessTypes, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The access type 'Test' is not assigned to this access control entry.\r\nParameter name: accessType")]
    public void AllowAccess_InvalidAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      ace.AllowAccess (accessType);
    }

    [Test]
    public void RemoveAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessTypeAndAttachToAce (ace, true);

      ace.RemoveAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (0, allowedAccessTypes.Length);
    }

    [Test]
    public void AddAccessType()
    {
      var accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Access Type 0", 0);
      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.CreateStatelessAccessControlList();
      securableClassDefinition.AddAccessType (accessType);
      var ace = AccessControlEntry.NewObject();
      securableClassDefinition.StatelessAccessControlList.AccessControlEntries.Add (ace);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, ace.State);

        ace.AddAccessType (accessType);

        Assert.AreEqual (1, ace.GetPermissions().Count);
        Assert.AreSame (accessType, ace.GetPermissions()[0].AccessType);
        Assert.AreEqual (StateType.Changed, ace.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The access type 'Test' has already been added to this access control entry.\r\nParameter name: accessType")]
    public void AddAccessType_ExistingAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject (Guid.NewGuid(), "Test", 42);

      ace.AddAccessType (accessType);
      ace.AddAccessType (accessType);
    }

    [Test]
    public void GetPermissions_SortedByAccessTypeFromSecurableClassDefinition ()
    {
      var accessTypes = new List<AccessTypeDefinition>();
      for (int i = 0; i < 10; i++)
        accessTypes.Add (AccessTypeDefinition.NewObject (Guid.NewGuid(), string.Format ("Access Type {0}", i), i));

      var securableClassDefinition = SecurableClassDefinition.NewObject();
      securableClassDefinition.CreateStatelessAccessControlList();

      foreach (var accessType in accessTypes)
        securableClassDefinition.AddAccessType (accessType);

      var ace = AccessControlEntry.NewObject();
      securableClassDefinition.StatelessAccessControlList.AccessControlEntries.Add (ace);
      foreach (var accessType in accessTypes.AsEnumerable().Reverse())
        ace.AddAccessType (accessType);

      var permissions = ace.GetPermissions();
      for (int i = 0; i < accessTypes.Count; i++)
        Assert.That (permissions[i].AccessType, Is.SameAs (accessTypes[i]));
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.AreEqual (StateType.New, ace.State);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      ace.Index = 1;
      Assert.AreEqual (1, ace.Index);
    }
  }
}
