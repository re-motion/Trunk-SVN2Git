using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.UnitTests
{
  [TestFixture]
  public class DoubleCheckedLockingContainerTest
  {
    public interface IFactory
    {
      SampleClass Create();
    }

    public class SampleClass
    {
    }

    private MockRepository _mocks;

    [SetUp]
    public void SetUp()
    {
      _mocks = new MockRepository();
    }

    [Test]
    public void SetAndGetValue()
    {
      SampleClass expected = new SampleClass();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { throw new NotImplementedException(); });

      container.Value = expected;
      Assert.AreSame (expected, container.Value);
    }

    [Test]
    public void GetValueFromFactory()
    {
      SampleClass expected = new SampleClass();
      IFactory mockFactory = _mocks.CreateMock<IFactory>();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { return mockFactory.Create(); });
      Expect.Call (mockFactory.Create()).Return (expected);
      _mocks.ReplayAll();

      SampleClass actual = container.Value;

      _mocks.VerifyAll();
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void SetNull()
    {
      SampleClass expected = new SampleClass ();
      IFactory mockFactory = _mocks.CreateMock<IFactory> ();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { return mockFactory.Create (); });
      _mocks.ReplayAll ();

      container.Value = null;

      _mocks.VerifyAll ();

      _mocks.BackToRecordAll ();
      Expect.Call (mockFactory.Create ()).Return (expected);

      _mocks.ReplayAll ();

      SampleClass actual = container.Value;

      _mocks.VerifyAll ();
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void HasValue ()
    {
      SampleClass expected = new SampleClass ();
      IFactory mockFactory = _mocks.CreateMock<IFactory> ();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { return mockFactory.Create (); });

      _mocks.ReplayAll ();

      Assert.IsFalse (container.HasValue);

      _mocks.VerifyAll ();

      _mocks.BackToRecordAll ();
      Expect.Call (mockFactory.Create ()).Return (expected);

      _mocks.ReplayAll ();

      SampleClass actual = container.Value;

      Assert.IsTrue (container.HasValue);
      _mocks.VerifyAll ();

      container.Value = null;
      Assert.IsFalse (container.HasValue);
    }
  }
}