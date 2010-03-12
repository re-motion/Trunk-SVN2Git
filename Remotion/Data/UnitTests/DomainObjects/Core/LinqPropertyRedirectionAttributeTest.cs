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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class LinqPropertyRedirectionAttributeTest
  {
    [Test]
    public void GetTargetProperty_NonRedirected ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var redirected = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      Assert.That (redirected, Is.SameAs (property));
    }

    [Test]
    public void GetTargetProperty_SimpleRedirected ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var redirected = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      Assert.That (redirected, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void GetTargetProperty_TwiceRedirected ()
    {
      var property = typeof (Order).GetProperty ("RedirectedRedirectedOrderNumber");

      var redirected = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      Assert.That (redirected, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.SelfRedirected' redirects LINQ queries "
        + "to itself.")]
    public void GetTargetProperty_InfiniteRedirection ()
    {
      var property = typeof (ClassWithInvalidRedirectedProperties).GetProperty ("SelfRedirected");

      LinqPropertyRedirectionAttribute.GetTargetProperty (property);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.RedirectedToNonexistent' redirects LINQ queries "
        + "to the property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.Nonexistent', which does not exist.")]
    public void GetTargetProperty_RedirectionToNonExistentProperty ()
    {
      var property = typeof (ClassWithInvalidRedirectedProperties).GetProperty ("RedirectedToNonexistent");

      LinqPropertyRedirectionAttribute.GetTargetProperty (property);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.RedirectedToPropertyWithOtherType' "
        + "redirects LINQ queries to the property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.PropertyWithOtherType', which has a "
        + "different return type.")]
    public void GetTargetProperty_RedirectionToPropertyWithOtherType ()
    {
      var property = typeof (ClassWithInvalidRedirectedProperties).GetProperty ("RedirectedToPropertyWithOtherType");

      LinqPropertyRedirectionAttribute.GetTargetProperty (property);
    }
  }
}