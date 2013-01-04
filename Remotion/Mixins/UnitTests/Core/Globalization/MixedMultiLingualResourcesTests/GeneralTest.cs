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
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Globalization.MixedMultiLingualResourcesTests
{
	[TestFixture]
	public class GeneralTest
	{
		private MockRepository _mockRepository;
		private IResourceManagerResolverUtility _resolverUtilityMock;

		[SetUp]
		public void SetUp()
		{
			_mockRepository = new MockRepository();
			_resolverUtilityMock = _mockRepository.StrictMock<IResourceManagerResolverUtility>();
		}

		[TearDown]
		public void TearDown()
		{
			ResourceManagerResolverUtility.SetCurrent (null);
		}

		[Test]
		public void GetResourceText_ForwaredToUtility ()
		{
			ResourceManagerResolverUtility.SetCurrent (_resolverUtilityMock);
			Expect.Call (_resolverUtilityMock.GetResourceText (MixedMultiLingualResources.Resolver, typeof (DateTime), "Foo")).Return ("Bar");
			_mockRepository.ReplayAll ();
		  Assert.That (MixedMultiLingualResources.GetResourceText (typeof (DateTime), "Foo"), Is.EqualTo ("Bar"));
		  _mockRepository.VerifyAll ();
		}

		[Test]
		public void ExistsResourceText_ForwaredToUtility ()
		{
			ResourceManagerResolverUtility.SetCurrent (_resolverUtilityMock);
			Expect.Call (_resolverUtilityMock.ExistsResourceText (MixedMultiLingualResources.Resolver, typeof (DateTime), "Foo")).Return (true);
			_mockRepository.ReplayAll ();
		  Assert.That (MixedMultiLingualResources.ExistsResourceText (typeof (DateTime), "Foo"), Is.True);
		  _mockRepository.VerifyAll ();
		}

		[Test]
		public void ExistsResource_ForwaredToUtility ()
		{
			ResourceManagerResolverUtility.SetCurrent (_resolverUtilityMock);
			Expect.Call (_resolverUtilityMock.ExistsResource (MixedMultiLingualResources.Resolver, typeof (DateTime))).Return (true);
			_mockRepository.ReplayAll ();
		  Assert.That (MixedMultiLingualResources.ExistsResource (typeof (DateTime)), Is.True);
		  _mockRepository.VerifyAll ();
		}
	}
}
