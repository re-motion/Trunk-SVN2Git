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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class UnidirectionalRelationTest : TableInheritanceMappingTest
  {
    [Test]
    [Ignore ("TODO: Implement referential integrity for unidirectional relationships.")]
    public void DeleteAndCommitWithConcreteTableInheritance()
    {
      SetDatabaseModifyable ();

      ClassWithUnidirectionalRelation classWithUnidirectionalRelation =
          ClassWithUnidirectionalRelation.GetObject (DomainObjectIDs.ClassWithUnidirectionalRelation);
      classWithUnidirectionalRelation.DomainBase.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();

      try
      {
        Dev.Null = classWithUnidirectionalRelation.DomainBase;
        Assert.Fail ("Expected ObjectDiscardedException");
      }
      catch (ObjectDiscardedException)
      {
        // succeed
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        ClassWithUnidirectionalRelation reloadedObject =
            ClassWithUnidirectionalRelation.GetObject (DomainObjectIDs.ClassWithUnidirectionalRelation);

        Assert.IsNull (reloadedObject.DomainBase);
      }
    }
  }
}
