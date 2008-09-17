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
