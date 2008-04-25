using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    protected DomainTest()
    {
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
    }

    [SetUp]
    public virtual void SetUp()
    {
    }

    [TearDown]
    public virtual void TearDown()
    {
      ClientTransactionScope.ResetActiveScope();
      BindableObjectProvider.SetCurrent (null);
    }
  }
}