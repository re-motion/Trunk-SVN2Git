using System;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests
{
  [TestFixture]
  public class SelectedColumnTest
  {
    [Test]
    public void Initialize()
    {
      SelectedColumn column = new SelectedColumn ("column", "alias");
      Assert.AreEqual ("column", column.Name);
      Assert.AreEqual ("alias", column.Alias);
    }

    [Test]
    public void Initialize_WithNull ()
    {
      SelectedColumn column = new SelectedColumn ("column", null);
      Assert.AreEqual ("column", column.Name);
      Assert.IsNull (column.Alias);
    }

    [Test]
    public void Equals_True ()
    {
      SelectedColumn column1 = new SelectedColumn ("c1", "c1");
      SelectedColumn column2 = new SelectedColumn ("c1", "c1");

      Assert.AreEqual (column1, column2);
    }

    [Test]
    public void Equals_DifferentNames ()
    {
      SelectedColumn column1 = new SelectedColumn ("c1", "c1");
      SelectedColumn column2 = new SelectedColumn ("c2", "c1");

      Assert.AreNotEqual (column1, column2);
    }

    [Test]
    public void Equals_DifferentAliases ()
    {
      SelectedColumn column1 = new SelectedColumn ("c1", "c1");
      SelectedColumn column2 = new SelectedColumn ("c1", "c2");

      Assert.AreNotEqual (column1, column2);
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      SelectedColumn column1 = new SelectedColumn ("c1", "c1");
      SelectedColumn column2 = new SelectedColumn ("c1", "c1");

      Assert.AreEqual (column1.GetHashCode(), column2.GetHashCode());
    }
  }
}