// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class DomainObjectDeleteHandlerTest : DomainTest
  {
    [Test]
    public void InitializeWithOneObjectList_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant ("name", "id"), testHelper.CreateTenant ("name", "id") };
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler (list);

      deleteHandler.Delete();

      Assert.That (deleteHandler.IsDeleted);
      Assert.That (list[0].State, Is.EqualTo (StateType.Deleted));
      Assert.That (list[1].State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void InitializeWithTwoObjectLists_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      testHelper.Transaction.EnterDiscardingScope ();
      var list1 = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant ("name", "id"), testHelper.CreateTenant ("name", "id") };
      var list2 = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant ("name", "id"), testHelper.CreateTenant ("name", "id") };
      testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler (list1, list2);

      deleteHandler.Delete ();

      Assert.That (deleteHandler.IsDeleted);
      Assert.That (list1[0].State, Is.EqualTo (StateType.Deleted));
      Assert.That (list1[1].State, Is.EqualTo (StateType.Deleted));
      Assert.That (list2[0].State, Is.EqualTo (StateType.Deleted));
      Assert.That (list2[1].State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void InitializeWithEmptyObjectList_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      testHelper.Transaction.EnterDiscardingScope ();
      var list = new ObjectList<BaseSecurityManagerObject>();
      testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler (list);

      deleteHandler.Delete ();
   
      Assert.That (deleteHandler.IsDeleted);
    }

    [Test]
    public void InitializeWithoutObjectLists_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      testHelper.Transaction.EnterDiscardingScope ();
      testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler ();

      deleteHandler.Delete ();

      Assert.That (deleteHandler.IsDeleted);
    }

    [Test]
    public void InitializeWithDuplicateObject_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      testHelper.Transaction.EnterDiscardingScope ();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant ("name", "id") };
      testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler (list, list);

      deleteHandler.Delete ();

      Assert.That (deleteHandler.IsDeleted);
      Assert.That (list[0].State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void InitializeWithDuplicateObject_AndDeleteWhileObjectIsNew ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      testHelper.Transaction.EnterDiscardingScope ();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant ("name", "id") };

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler (list, list);

      deleteHandler.Delete ();

      Assert.That (deleteHandler.IsDeleted);
      Assert.That (list[0].State, Is.EqualTo (StateType.Discarded));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The Delete operation my only be performed once.")]
    public void Delete_Twice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      testHelper.Transaction.EnterDiscardingScope ();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant ("name", "id")};
      testHelper.Transaction.CreateSubTransaction ().EnterDiscardingScope ();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler (list);

      deleteHandler.Delete ();
      Assert.That (deleteHandler.IsDeleted);
      
      deleteHandler.Delete ();
    }

  }
}