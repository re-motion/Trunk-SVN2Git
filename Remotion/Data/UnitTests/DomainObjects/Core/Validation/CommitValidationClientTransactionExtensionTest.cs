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
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Validation
{
  [TestFixture]
  public class CommitValidationClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    [Test]
    public void DefaultKey ()
    {
      Assert.That (CommitValidationClientTransactionExtension.DefaultKey, Is.EqualTo (typeof (CommitValidationClientTransactionExtension).FullName));
    }

    [Test]
    public void Key ()
    {
      var extension = new CommitValidationClientTransactionExtension (tx => { throw new NotImplementedException(); });
      Assert.That (extension.Key, Is.EqualTo (CommitValidationClientTransactionExtension.DefaultKey));
    }

    [Test]
    public void CommitValidate ()
    {
      var data1 = PersistableDataObjectMother.Create ();
      var data2 = PersistableDataObjectMother.Create ();

      var transaction = ClientTransaction.CreateRootTransaction();

      var validatorMock = MockRepository.GenerateStrictMock<IPersistableDataValidator>();
      var extension = new CommitValidationClientTransactionExtension (
          tx =>
          {
            Assert.That (tx, Is.SameAs (transaction));
            return validatorMock;
          });

      validatorMock.Expect (mock => mock.Validate (data1));
      validatorMock.Expect (mock => mock.Validate (data2));
      validatorMock.Replay();

      extension.CommitValidate (transaction, Array.AsReadOnly (new[] { data1, data2 }));

      validatorMock.VerifyAllExpectations();
    }
  }
}