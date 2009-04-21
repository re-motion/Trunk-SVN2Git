// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class SerializationCallbackTest : ClientTransactionBaseTest
  {
    [Test]
    public void SerializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks), ParamList.Empty);

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object)instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_SerializationCallbacks ();
    }

    [Test]
    public void DeserializationEvents ()
    {
      ClassWithSerializationCallbacks instance =
          (ClassWithSerializationCallbacks) RepositoryAccessor.NewObject (typeof (ClassWithSerializationCallbacks), ParamList.Empty);

      Assert.AreNotSame (typeof (ClassWithSerializationCallbacks), ((object) instance).GetType ());

      new SerializationCallbackTester<ClassWithSerializationCallbacks> (new RhinoMocksRepositoryAdapter (), instance, ClassWithSerializationCallbacks.SetReceiver)
          .Test_DeserializationCallbacks ();
    }
  }
}
