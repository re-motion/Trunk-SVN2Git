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
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.Globalization.MixedMultiLingualResourcesTests
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
			_resolverUtilityMock = _mockRepository.CreateMock<IResourceManagerResolverUtility>();
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
			Assert.AreEqual ("Bar", MixedMultiLingualResources.GetResourceText (typeof (DateTime), "Foo"));
			_mockRepository.VerifyAll ();
		}

		[Test]
		public void ExistsResourceText_ForwaredToUtility ()
		{
			ResourceManagerResolverUtility.SetCurrent (_resolverUtilityMock);
			Expect.Call (_resolverUtilityMock.ExistsResourceText (MixedMultiLingualResources.Resolver, typeof (DateTime), "Foo")).Return (true);
			_mockRepository.ReplayAll ();
			Assert.IsTrue (MixedMultiLingualResources.ExistsResourceText (typeof (DateTime), "Foo"));
			_mockRepository.VerifyAll ();
		}

		[Test]
		public void ExistsResource_ForwaredToUtility ()
		{
			ResourceManagerResolverUtility.SetCurrent (_resolverUtilityMock);
			Expect.Call (_resolverUtilityMock.ExistsResource (MixedMultiLingualResources.Resolver, typeof (DateTime))).Return (true);
			_mockRepository.ReplayAll ();
			Assert.IsTrue (MixedMultiLingualResources.ExistsResource (typeof (DateTime)));
			_mockRepository.VerifyAll ();
		}
	}
}
