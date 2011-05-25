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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class DomainModelConstraintProviderTest
  {
    private DomainModelConstraintProvider _domainModelConstraintProvider;

    [SetUp]
    public void SetUp ()
    {
      _domainModelConstraintProvider = new DomainModelConstraintProvider();
    }

    [Test]
    public void IsNullable_NoAttribute ()
    {
      var result =
          _domainModelConstraintProvider.IsNullable (
              PropertyInfoAdapter.Create (typeof (ClassWithStringProperties).GetProperty ("NoAttribute")));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsNullable_NullableFromAttribute ()
    {
      var result =
          _domainModelConstraintProvider.IsNullable (
              PropertyInfoAdapter.Create (typeof (ClassWithStringProperties).GetProperty ("NullableFromAttribute")));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsNullable_NotNullableFromAttribute ()
    {
      var result =
          _domainModelConstraintProvider.IsNullable (
              PropertyInfoAdapter.Create (typeof (ClassWithStringProperties).GetProperty ("NotNullable")));

      Assert.That (result, Is.False);
    }

    [Test]
    public void GetMaxLength_NoAttribute ()
    {
      var result =
          _domainModelConstraintProvider.GetMaxLength (
              PropertyInfoAdapter.Create (typeof (ClassWithStringProperties).GetProperty ("NoAttribute")));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetMaxLength_MaxLengthFromAttribute ()
    {
      var result =
          _domainModelConstraintProvider.GetMaxLength (
              PropertyInfoAdapter.Create (typeof (ClassWithStringProperties).GetProperty ("MaximumLength")));

      Assert.That (result, Is.EqualTo(100));
    }

  }
}