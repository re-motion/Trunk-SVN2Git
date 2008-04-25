using System;
using NUnit.Framework;
using Remotion.UnitTests.Globalization.SampleTypes;
using Remotion.Globalization;
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
      _resolverMock = _mockRepository.CreateMock<ResourceManagerResolver<MultiLingualResourcesAttribute>>();
    }

		[TearDown]
		public void TearDown ()
		{
			ResourceManagerResolverUtility.SetCurrent (null);
		}

		[Test]
		public void Current_Initial ()
		{
			Assert.IsNotNull (ResourceManagerResolverUtility.Current);
			Assert.AreSame (ResourceManagerResolverUtility.Default, ResourceManagerResolverUtility.Current);
		}

  	[Test]
		public void Current_Set ()
		{
			IResourceManagerResolverUtility mockUtility = _mockRepository.CreateMock<IResourceManagerResolverUtility>();
			ResourceManagerResolverUtility.SetCurrent (mockUtility);
			Assert.AreSame (mockUtility, ResourceManagerResolverUtility.Current);
		}

		[Test]
		public void Current_Reset ()
		{
			ResourceManagerResolverUtility.SetCurrent (null);
			Assert.AreSame (ResourceManagerResolverUtility.Default, ResourceManagerResolverUtility.Current);
		}

 	[Test]
    public void GetResourceText ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager>();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Resistance is futile");
      _mockRepository.ReplayAll ();

      string text = ResourceManagerResolverUtility.Current.GetResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.AreEqual ("Resistance is futile", text);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetResourceText_Default ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager> ();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Grob")).Return ("Grob");
      _mockRepository.ReplayAll ();

			string text = ResourceManagerResolverUtility.Current.GetResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Grob");
      Assert.AreEqual ("", text);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ResourceException))]
    public void GetResourceText_Throw ()
    {
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Throw (new ResourceException(""));
      _mockRepository.ReplayAll ();

			ResourceManagerResolverUtility.Current.GetResourceText (_resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Grob");
    }

    [Test]
    public void ExistsResourceText_True ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager> ();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Resistance is futile");
      _mockRepository.ReplayAll ();

			bool result = ResourceManagerResolverUtility.Current.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.IsTrue (result);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceText_False ()
    {
      IResourceManager resourceManagerMock = _mockRepository.CreateMock<IResourceManager> ();
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Return (resourceManagerMock);
      Expect.Call (resourceManagerMock.GetString ("Borg")).Return ("Borg");
      _mockRepository.ReplayAll ();

			bool result = ResourceManagerResolverUtility.Current.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.IsFalse (result);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceText_False_Exception ()
    {
      Expect.Call (_resolverMock.GetResourceManager (typeof (ClassWithMultiLingualResourcesAttributes), false)).Throw (new ResourceException (""));
      _mockRepository.ReplayAll ();

			bool result = ResourceManagerResolverUtility.Current.ExistsResourceText (
          _resolverMock, typeof (ClassWithMultiLingualResourcesAttributes), "Borg");
      Assert.IsFalse (result);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExistsResourceTrue ()
    {
      ResourceManagerResolver<MultiLingualResourcesAttribute> resolver =
					new ResourceManagerResolver<MultiLingualResourcesAttribute> ();
      Assert.IsTrue (
					ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (ClassWithMultiLingualResourcesAttributes)));
      Assert.IsTrue (
					ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (InheritedClassWithMultiLingualResourcesAttributes)));
      Assert.IsTrue (
					ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (InheritedClassWithoutMultiLingualResourcesAttributes)));
    }

    [Test]
    public void ExistsResourceFalse ()
    {
			ResourceManagerResolver<MultiLingualResourcesAttribute> resolver =
					new ResourceManagerResolver<MultiLingualResourcesAttribute> ();
      Assert.IsFalse (
					ResourceManagerResolverUtility.Current.ExistsResource (resolver, typeof (ClassWithoutMultiLingualResourcesAttributes)));
    }
  }
}