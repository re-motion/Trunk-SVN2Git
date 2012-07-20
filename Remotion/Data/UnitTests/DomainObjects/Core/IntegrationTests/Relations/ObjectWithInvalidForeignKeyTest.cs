// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;


namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Relations
{
  [TestFixture]
  public class ObjectWithInvalidForeignKeyTest : ClientTransactionBaseTest
  {
    [Test]
    public void AccessInvalidForeignKeyRelation ()
    {
      var id = new ObjectID (typeof (ClassWithInvalidRelation), new Guid ("{AFA9CF46-8E77-4da8-9793-53CAA86A277C}"));
      var objectWithInvalidRelation = (ClassWithInvalidRelation) ClassWithInvalidRelation.GetObject (id);

      Assert.That (objectWithInvalidRelation.ClassWithGuidKey.IsInvalid, Is.True);

      // Overwriting the invalid ID is possible!
      var classWithGuidKey = ClassWithGuidKey.NewObject ();
      classWithGuidKey.ClassWithValidRelationsNonOptional = ClassWithValidRelations.NewObject ();
      objectWithInvalidRelation.ClassWithGuidKey = classWithGuidKey;

      SetDatabaseModifyable ();
      TestableClientTransaction.Commit ();

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var reloadedObject = (ClassWithInvalidRelation) ClassWithInvalidRelation.GetObject (id);
        reloadedObject.ClassWithGuidKey.EnsureDataAvailable ();
        Assert.That (reloadedObject.ClassWithGuidKey.IsInvalid, Is.False);
      }

      // Note: See also NotFoundObjectsTest
    }
  }
}