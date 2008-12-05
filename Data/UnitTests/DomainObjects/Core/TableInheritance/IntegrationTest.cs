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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class IntegrationTest : TableInheritanceMappingTest
  {
    private ObjectID _rootFolderID;
    private ObjectID _folder1ID;
    private ObjectID _fileInRootFolderID;
    private ObjectID _fileInFolder1ID;

    private ObjectID _derivedClassWithEntity1ID;
    private ObjectID _derivedClassWithEntity2ID;
    private ObjectID _derivedClassWithEntity3ID;

    private ObjectID _derivedClassWithEntityFromBaseClass1ID;
    private ObjectID _derivedClassWithEntityFromBaseClass2ID;
    private ObjectID _derivedClassWithEntityFromBaseClass3ID;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootFolderID = CreateFolderObjectID ("{1A45A89B-746E-4a9e-AC2C-E960E90C0DAD}");
      _folder1ID = CreateFolderObjectID ("{6B8A65C1-1D49-4dab-97D7-F466F3EAB91E}");
      _fileInRootFolderID = CreateFileObjectID ("{023392E2-AB99-434f-A71F-8A9865D10C8C}");
      _fileInFolder1ID = CreateFileObjectID ("{6108E150-6D3C-4e38-9865-895BD143D180}");

      _derivedClassWithEntity1ID = CreateDerivedClassWithEntityWithHierarchyObjectID ("{137DA04C-2B53-463e-A893-D8B246D6BFA9}");
      _derivedClassWithEntity2ID = CreateDerivedClassWithEntityWithHierarchyObjectID ("{6389C3AB-9E65-4bfb-9321-EC9F50B6A479}");
      _derivedClassWithEntity3ID = CreateDerivedClassWithEntityWithHierarchyObjectID ("{15526A7A-57EC-42c3-95A7-B40E46784846}");

      _derivedClassWithEntityFromBaseClass1ID = CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID ("{24F27B35-68F8-4035-A454-33CFC1AF6339}");
      _derivedClassWithEntityFromBaseClass2ID = CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID ("{9C730A8A-8F83-4b26-AF40-FB0C3D4DD387}");
      _derivedClassWithEntityFromBaseClass3ID = CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID ("{953B2E51-C324-4f86-8FA0-3AFA2A2E4E72}");
    }

    [Test]
    public void LoadObjectsWithSamePropertyNameInDifferentInheritanceBranches ()
    {
      Folder rootFolder = Folder.GetObject (_rootFolderID);
      Assert.AreEqual (new DateTime (2006, 2, 1), rootFolder.CreatedAt);

      File fileInRootFolder = File.GetObject (_fileInRootFolderID);
      Assert.AreEqual (new DateTime (2006, 2, 3), fileInRootFolder.CreatedAt);
    }


    [Test]
    public void CompositePatternNavigateOneToMany ()
    {
      Folder rootFolder = Folder.GetObject (_rootFolderID);

      Assert.AreEqual (2, rootFolder.FileSystemItems.Count);
      Assert.AreEqual (_fileInRootFolderID, rootFolder.FileSystemItems[0].ID);
      Assert.AreEqual (_folder1ID, rootFolder.FileSystemItems[1].ID);

      Folder folder1 = Folder.GetObject (_folder1ID);

      Assert.AreEqual (1, folder1.FileSystemItems.Count);
      Assert.AreEqual (_fileInFolder1ID, folder1.FileSystemItems[0].ID);
    }

    [Test]
    public void CompositePatternNavigateManyToOne ()
    {
      Folder folder1 = Folder.GetObject (_folder1ID);
      Assert.AreEqual (_rootFolderID, folder1.ParentFolder.ID);

      File fileInRootFolder = File.GetObject (_fileInRootFolderID);
      Assert.AreEqual (_rootFolderID, fileInRootFolder.ParentFolder.ID);

      File fileInFolder1 = File.GetObject (_fileInFolder1ID);
      Assert.AreEqual (_folder1ID, fileInFolder1.ParentFolder.ID);
    }

    [Test]
    public void ObjectHierarchyNavigateOneToMany ()
    {
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity1 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity1ID);

      Assert.AreEqual (3, derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntity3ID, derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy[0].ID);
      Assert.AreEqual (_derivedClassWithEntity2ID, derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy[1].ID);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass1ID, derivedClassWithEntity1.ChildAbstractBaseClassesWithHierarchy[2].ID);

      Assert.AreEqual (3, derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass1ID, derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy[0].ID);
      Assert.AreEqual (_derivedClassWithEntity2ID, derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy[1].ID);
      Assert.AreEqual (_derivedClassWithEntity3ID, derivedClassWithEntity1.ChildDerivedClassesWithEntityWithHierarchy[2].ID);

      DerivedClassWithEntityWithHierarchy derivedClassWithEntity2 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity2ID);
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity3 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity3ID);

      Assert.AreEqual (1, derivedClassWithEntity2.ChildAbstractBaseClassesWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass2ID, derivedClassWithEntity2.ChildAbstractBaseClassesWithHierarchy[0].ID);
      Assert.AreEqual (1, derivedClassWithEntity2.ChildDerivedClassesWithEntityWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass2ID, derivedClassWithEntity2.ChildDerivedClassesWithEntityWithHierarchy[0].ID);

      Assert.AreEqual (1, derivedClassWithEntity3.ChildAbstractBaseClassesWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass3ID, derivedClassWithEntity3.ChildAbstractBaseClassesWithHierarchy[0].ID);
      Assert.AreEqual (1, derivedClassWithEntity3.ChildDerivedClassesWithEntityWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass3ID, derivedClassWithEntity3.ChildDerivedClassesWithEntityWithHierarchy[0].ID);

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
          DerivedClassWithEntityFromBaseClassWithHierarchy.GetObject (_derivedClassWithEntityFromBaseClass1ID);

      Assert.IsEmpty (derivedClassWithEntityFromBaseClass1.ChildAbstractBaseClassesWithHierarchy);
      Assert.IsEmpty (derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityWithHierarchy);
      Assert.AreEqual (2, derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityFromBaseClassWithHierarchy.Count);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass2ID, derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityFromBaseClassWithHierarchy[0].ID);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass3ID, derivedClassWithEntityFromBaseClass1.ChildDerivedClassesWithEntityFromBaseClassWithHierarchy[1].ID);
    }

    [Test]
    public void ObjectHierarchyNavigateManyToOne ()
    {
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity2 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity2ID);

      Assert.AreEqual (_derivedClassWithEntity1ID, derivedClassWithEntity2.ParentAbstractBaseClassWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntity1ID, derivedClassWithEntity2.ParentDerivedClassWithEntityWithHierarchy.ID);

      DerivedClassWithEntityWithHierarchy derivedClassWithEntity3 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity3ID);
      Assert.AreEqual (_derivedClassWithEntity1ID, derivedClassWithEntity3.ParentAbstractBaseClassWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntity1ID, derivedClassWithEntity3.ParentDerivedClassWithEntityWithHierarchy.ID);

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
          DerivedClassWithEntityFromBaseClassWithHierarchy.GetObject (_derivedClassWithEntityFromBaseClass1ID);

      Assert.AreEqual (_derivedClassWithEntity1ID, derivedClassWithEntityFromBaseClass1.ParentAbstractBaseClassWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntity1ID, derivedClassWithEntityFromBaseClass1.ParentDerivedClassWithEntityWithHierarchy.ID);

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass2 =
          DerivedClassWithEntityFromBaseClassWithHierarchy.GetObject (_derivedClassWithEntityFromBaseClass2ID);

      Assert.AreEqual (_derivedClassWithEntity2ID, derivedClassWithEntityFromBaseClass2.ParentAbstractBaseClassWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntity2ID, derivedClassWithEntityFromBaseClass2.ParentDerivedClassWithEntityWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass1ID, derivedClassWithEntityFromBaseClass2.ParentDerivedClassWithEntityFromBaseClassWithHierarchy.ID);

      DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass3 =
          DerivedClassWithEntityFromBaseClassWithHierarchy.GetObject (_derivedClassWithEntityFromBaseClass3ID);

      Assert.AreEqual (_derivedClassWithEntity3ID, derivedClassWithEntityFromBaseClass3.ParentAbstractBaseClassWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntity3ID, derivedClassWithEntityFromBaseClass3.ParentDerivedClassWithEntityWithHierarchy.ID);
      Assert.AreEqual (_derivedClassWithEntityFromBaseClass1ID, derivedClassWithEntityFromBaseClass3.ParentDerivedClassWithEntityFromBaseClassWithHierarchy.ID);
    }

    [Test]
    public void UnidirectionalRelationToClassWithoutDerivation ()
    {
      ObjectID client2 = CreateObjectID (typeof (Client), "{58535280-84EC-41d9-9F8F-BCAC64BB3709}");

      DerivedClassWithEntityWithHierarchy derivedClassWithEntity1 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity1ID);
      Assert.AreEqual (DomainObjectIDs.Client, derivedClassWithEntity1.ClientFromAbstractBaseClass.ID);

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        derivedClassWithEntity1 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity1ID);
        Assert.AreEqual (client2, derivedClassWithEntity1.ClientFromDerivedClassWithEntity.ID);
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
            DerivedClassWithEntityFromBaseClassWithHierarchy.GetObject (_derivedClassWithEntityFromBaseClass1ID);
        Assert.AreEqual (DomainObjectIDs.Client, derivedClassWithEntityFromBaseClass1.ClientFromDerivedClassWithEntityFromBaseClass.ID);
      }
    }

    [Test]
    public void UnidirectionalRelationToAbstractClass ()
    {
      DerivedClassWithEntityWithHierarchy derivedClassWithEntity1 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity1ID);
      Assert.AreEqual (_rootFolderID, derivedClassWithEntity1.FileSystemItemFromAbstractBaseClass.ID);

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        derivedClassWithEntity1 = DerivedClassWithEntityWithHierarchy.GetObject (_derivedClassWithEntity1ID);
        Assert.AreEqual (_fileInRootFolderID, derivedClassWithEntity1.FileSystemItemFromDerivedClassWithEntity.ID);
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        DerivedClassWithEntityFromBaseClassWithHierarchy derivedClassWithEntityFromBaseClass1 =
            DerivedClassWithEntityFromBaseClassWithHierarchy.GetObject (_derivedClassWithEntityFromBaseClass1ID);

        Assert.AreEqual (_fileInFolder1ID, derivedClassWithEntityFromBaseClass1.FileSystemItemFromDerivedClassWithEntityFromBaseClass.ID);
      }
    }

    private ObjectID CreateFolderObjectID (string guid)
    {
      return CreateObjectID (typeof (Folder), guid);
    }

    private ObjectID CreateFileObjectID (string guid)
    {
      return CreateObjectID (typeof (File), guid);
    }

    private ObjectID CreateDerivedClassWithEntityWithHierarchyObjectID (string guid)
    {
      return CreateObjectID (typeof (DerivedClassWithEntityWithHierarchy), guid);
    }

    private ObjectID CreateDerivedClassWithEntityFromBaseClassWithHierarchyObjectID (string guid)
    {
      return CreateObjectID (typeof (DerivedClassWithEntityFromBaseClassWithHierarchy), guid);
    }

    private ObjectID CreateObjectID (Type classType, string guid)
    {
      return new ObjectID (classType, new Guid (guid));
    }
  }
}
