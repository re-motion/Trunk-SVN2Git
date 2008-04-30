using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Queries
{
  [TestFixture]
  public class QueryParameterTest : StandardMappingTest
  {
    private QueryParameter _parameter;

    public override void SetUp ()
    {
      base.SetUp ();

      _parameter = new QueryParameter ("name", "value", QueryParameterType.Value);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual ("name", _parameter.Name);
      Assert.AreEqual ("value", _parameter.Value);
      Assert.AreEqual (QueryParameterType.Value, _parameter.ParameterType);
    }

    [Test]
    public void SetValue ()
    {
      _parameter.Value = "NewValue";
      Assert.AreEqual ("NewValue", _parameter.Value);
    }

    [Test]
    public void SetParameterType ()
    {
      _parameter.ParameterType = QueryParameterType.Text;
      Assert.AreEqual (QueryParameterType.Text, _parameter.ParameterType);
    }
  }
}
