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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Reflection;


namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class ConstructionTest : ClientTransactionBaseTest
  {
    [DBTable]
    public class DomainObjectWithSpecialConstructor : DomainObject
    {
      public string S;
      public object O;

      public DomainObjectWithSpecialConstructor (string s)
      {
        S = s;
      }

      public DomainObjectWithSpecialConstructor (object o)
      {
        O = o;
      }

      public static DomainObjectWithSpecialConstructor NewObject (string s)
      {
        return NewObject<DomainObjectWithSpecialConstructor>(ParamList.Create(s));
      }

      public static DomainObjectWithSpecialConstructor NewObject (object o)
      {
        return NewObject<DomainObjectWithSpecialConstructor> (ParamList.CreateDynamic (o));
      }
    }

    [Test]
    public void ConstructorSelection ()
    {
      DomainObjectWithSpecialConstructor d1 = DomainObjectWithSpecialConstructor.NewObject ("string");
      Assert.AreEqual ("string", d1.S);
      Assert.IsNull (d1.O);

      object obj = new object ();
      DomainObjectWithSpecialConstructor d2 = DomainObjectWithSpecialConstructor.NewObject (obj);
      Assert.IsNull (d2.S);
      Assert.AreSame (obj, d2.O);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DomainObject constructors must not be called directly. Use " 
        + "DomainObject.NewObject to create DomainObject instances.")]
    public void ConstructorThrowsIfCalledDirectly ()
    {
      new DomainObjectWithSpecialConstructor ("string");
    }

    [Test]
    public void ConstructorWorksIfCalledIndirectly ()
    {
      var instance = DomainObjectWithSpecialConstructor.NewObject ("string");
      Assert.That (instance, Is.Not.Null);
    }
  }
}
