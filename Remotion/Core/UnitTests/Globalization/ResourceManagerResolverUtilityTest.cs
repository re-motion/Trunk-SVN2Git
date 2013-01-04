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
using Remotion.UnitTests.Globalization.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class ResourceManagerResolverUtilityTest
  {
    private MockRepository _mockRepository;
    private ResourceManagerResolver<MultiLingualResourcesAttribute> _resolverMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _resolverMock = _mockRepository.StrictMock<ResourceManagerResolver<MultiLingualResourcesAttribute>>();
    }

		[TearDown]
		public void TearDown ()
		{
			ResourceManagerResolverUtility.SetCurrent (null);
		}

		[Test]
		public void Current_Initial ()
		{
		  Assert.That (ResourceManagerResolverUtility.Current, Is.Not.Null);
		  Assert.That (ResourceManagerResolverUtility.Current, Is.SameAs (ResourceManagerResolverUtility.Default));
		}

  	[Test]
		public void Current_Set ()
		{
			IResourceManagerResolverUtility mockUtility = _mockRepository.StrictMock<IResourceManagerResolverUtility>();
			ResourceManagerResolverUtility.SetCurrent (mockUtility);
  	  Assert.That (ResourceManagerResolverUtility.Current, Is.SameAs (mockUtility));
		}

		[Test]
		public void Current_Reset ()
		{
			ResourceManagerResolverUtility.SetCurrent (null);
		  Assert.That (ResourceManagerResolverUtility.Current, Is.SameAs (ResourceManagerResolverUtility.Default));
		}

 	[Test]
    public void GetResourceText ()
    {
      IResourceManager resourceManagerMock = _mockRepository.StrictMock<IResourceManager>();

      Expect
          .Call (_resolverMock.GetResourceManagerCacheEntry (typeof (ClassWithMultiLingualResourcesAttributes), false))
          .Return (ResourceManagerCacheEntry.Create (typeof (ClassWithMultiLingualResourcesAttributes), resourceManagerMock));
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Resistance is futile");
      _mockRepository.ReplayAll ();

      string text = ResourceManagerResolverUtility.Current.GetResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
 	  Assert.That (text, Is.EqualTo ("Resistance is futile"));

 	  _mockRepository.VerifyAll();
    }

    [Test]
    public void GetResourceText_Default ()
    {
      IResourceManager resourceManagerMock = _mockRepository.StrictMock<IResourceManager> ();
      Expect
          .Call (_resolverMock.GetResourceManagerCacheEntry (typeof (ClassWithMultiLingualResourcesAttributes), false))
          .Return (ResourceManagerCacheEntry.Create (typeof (ClassWithMultiLingualResourcesAttributes), resourceManagerMock));
      Expect.Call (resourceManagerMock.GetString ("Grob")).Return ("Grob");
      _mockRepository.ReplayAll ();

			string text = ResourceManagerResolverUtility.Current.GetResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Grob");
      Assert.That (text, Is.EqualTo (""));

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ResourceException))]
    public void GetResourceText_Throw ()
    {
      Expect
          .Call (_resolverMock.GetResourceManagerCacheEntry (typeof (ClassWithMultiLingualResourcesAttributes), false))
          .Throw (new ResourceException (""));
      _mockRepository.ReplayAll ();

			ResourceManagerResolverUtility.Current.GetResourceText (_resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Grob");
    }

    [Test]
    public void ExistsResourceText_True ()
    {
      IResourceManager resourceManagerMock = _mockRepository.StrictMock<IResourceManager> ();
      Expect
          .Call (_resolverMock.GetResourceManagerCacheEntry (typeof (ClassWithMultiLingualResourcesAttributes), false))
          .Return (ResourceManagerCacheEntry.Create (typeof (ClassWithMultiLingualResourcesAttributes), resourceManagerMock));
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Resistance is futile");
      _mockRepository.ReplayAll ();

			bool result = ResourceManagerResolverUtility.Current.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.That (result, Is.True);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceText_False ()
    {
      var resourceManagerMock = _mockRepository.StrictMock<IResourceManager> ();
      Expect
          .Call (_resolverMock.GetResourceManagerCacheEntry (typeof (ClassWithMultiLingualResourcesAttributes), false))
          .Return (ResourceManagerCacheEntry.Create (typeof (ClassWithMultiLingualResourcesAttributes), resourceManagerMock));
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Borg");
      _mockRepository.ReplayAll ();

			bool result = ResourceManagerResolverUtility.Current.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.That (result, Is.False);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceText_False_Exception ()
    {
      Expect
          .Call (_resolverMock.GetResourceManagerCacheEntry (typeof (ClassWithMultiLingualResourcesAttributes), false))
          .Throw (new ResourceException (""));
      _mockRepository.ReplayAll ();

			bool result = ResourceManagerResolverUtility.Current.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.That (result, Is.False);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceTrue ()
    {
      ResourceManagerResolver<MultiLingualResourcesAttribute> resolver =
					new ResourceManagerResolver<MultiLingualResourcesAttribute> ();
      Assert.That (ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (ClassWithMultiLingualResourcesAttributes)), Is.True);
      Assert.That (ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (InheritedClassWithMultiLingualResourcesAttributes)), Is.True);
      Assert.That (ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (InheritedClassWithoutMultiLingualResourcesAttributes)), Is.True);
    }

    [Test]
    public void ExistsResourceFalse ()
    {
			ResourceManagerResolver<MultiLingualResourcesAttribute> resolver =
					new ResourceManagerResolver<MultiLingualResourcesAttribute> ();
      Assert.That (ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (ClassWithoutMultiLingualResourcesAttributes)), Is.False);
    }
  }
}
