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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectImplementationTest : ClientTransactionBaseTest
  {
    [Test]
    public void Create ()
    {
      var wrapper = SampleBindableDomainObject.NewObject ();
      var mixin = BindableDomainObjectImplementation.Create (wrapper);
      Assert.That (mixin.BusinessObjectClass, Is.Not.Null);
      Assert.That (PrivateInvoke.GetNonPublicProperty (mixin, "This"), Is.SameAs (wrapper));
    }

    [Test]
    public void Deserialization ()
    {
      var wrapper = SampleBindableDomainObject.NewObject ();
      var mixin = BindableDomainObjectImplementation.Create (wrapper);
      var deserializedData = Serializer.SerializeAndDeserialize (Tuple.NewTuple (mixin, wrapper));
      Assert.That (deserializedData.A.BusinessObjectClass, Is.Not.Null);
      Assert.That (PrivateInvoke.GetNonPublicProperty (deserializedData.A, "This"), Is.SameAs (deserializedData.B));
    }

    [Test]
    public void UniqueIdentifierViaImplementation ()
    {
      var instance = SampleBindableDomainObject.NewObject ();
      var mixin = (BindableDomainObjectImplementation) PrivateInvoke.GetNonPublicField (instance, typeof(BindableDomainObject), "_implementation");
      Assert.That (mixin.UniqueIdentifier, Is.EqualTo (instance.ID.ToString()));
    }

    [Test]
    public void BaseUniqueIdentifier ()
    {
      var wrapper = SampleBindableDomainObject.NewObject ();
      var implementation = BindableDomainObjectImplementation.Create (wrapper);
      Assert.That (implementation.BaseUniqueIdentifier, Is.EqualTo (wrapper.ID.ToString()));
    }

    [Test]
    public void UniqueIdentifier_ViaImplementation () // overriding UniqueIdentifier is not possbile in BindableDomainObjects
    {
      var wrapper = SampleBindableDomainObject.NewObject ();
      var implementation = BindableDomainObjectImplementation.Create (wrapper);
      Assert.That (implementation.UniqueIdentifier, Is.EqualTo (wrapper.ID.ToString ()));
    }

    [Test]
    public void BaseDisplayName ()
    {
      var wrapper = SampleBindableDomainObject.NewObject ();
      var implementation = BindableDomainObjectImplementation.Create (wrapper);
      Assert.That (implementation.BaseDisplayName, Is.EqualTo (((IBusinessObject)wrapper).BusinessObjectClass.Identifier));
    }

    [Test]
    public void DisplayName_ViaImplementation_Default ()
    {
      var wrapper = SampleBindableDomainObject.NewObject ();
      var implementation = BindableDomainObjectImplementation.Create (wrapper);
      Assert.That (implementation.DisplayName, Is.EqualTo (((IBusinessObject) wrapper).BusinessObjectClass.Identifier));
    }

    [Test]
    public void DisplayName_ViaImplementation_Overridden ()
    {
      var wrapper = SampleBindableDomainObjectWithOverriddenDisplayName.NewObject ();
      var implementation = BindableDomainObjectImplementation.Create (wrapper);
      Assert.That (implementation.DisplayName, Is.EqualTo ("TheDisplayName"));
    }
  }
}
