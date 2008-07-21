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
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.TableInheritance
{
  [TestFixture]
  public class ObjectIDTest : TableInheritanceMappingTest
  {
    [Test]
    public void InitializeWithAbstractType ()
    {
      try
      {
        new ObjectID (typeof (DomainBase), Guid.NewGuid ());
        Assert.Fail ("ArgumentException was expected.");
      }
      catch (ArgumentException ex)
      {
        string expectedMessage = string.Format (
            "An ObjectID cannot be constructed for abstract type '{0}' of class '{1}'.\r\nParameter name: classType",
            typeof (DomainBase).AssemblyQualifiedName, "TI_DomainBase");

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }
  }
}
