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
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class SerializatiionTest
  {
    [Test]
    public void Serialization ()
    {
      SecurityClientTransactionExtension extension = new SecurityClientTransactionExtension ();
      SecurityClientTransactionExtension deserializedExtension = Serializer.SerializeAndDeserialize (extension);

      Assert.AreNotSame (extension, deserializedExtension);
    }
  }
}
