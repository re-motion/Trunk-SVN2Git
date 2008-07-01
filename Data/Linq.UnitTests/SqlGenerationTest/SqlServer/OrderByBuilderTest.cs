using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.Linq.UnitTests;

namespace Remotion.Data.Linq.UnitTests.SqlGenerationTest.SqlServer
{
  [TestFixture]
  public class OrderByBuilderTest
  {
    [Test]
    public void CombineOrderedFields()
    {
      CommandBuilder commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance);
      OrderByBuilder orderByBuilder = new OrderByBuilder (commandBuilder);

      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause ();
      FieldSourcePath path = ExpressionHelper.GetPathForNewTable ("s1", "s1");
      MemberInfo member = typeof (Student).GetProperty ("First");
      Column column = new Column (path.FirstSource,"c1");
      FieldDescriptor descriptor = new FieldDescriptor (member, path, column);

      OrderingField field1 = new OrderingField (descriptor,OrderDirection.Asc);
      OrderingField field2 = new OrderingField (descriptor, OrderDirection.Desc);
      List<OrderingField> orderingFields = new List<OrderingField> { field1,field2 };

      orderByBuilder.BuildOrderByPart (orderingFields);

      Assert.AreEqual (" ORDER BY [s1].[c1] ASC, [s1].[c1] DESC", commandBuilder.GetCommandText());
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "OrderDirection 2147483647 is not supported.")]
    public void CombineOrderedFields_InvalidDirection ()
    {
      CommandBuilder commandBuilder = new CommandBuilder (new StringBuilder (), new List<CommandParameter> (), StubDatabaseInfo.Instance);
      OrderByBuilder orderByBuilder = new OrderByBuilder (commandBuilder);

      MainFromClause fromClause = ExpressionHelper.CreateMainFromClause ();
      FieldSourcePath path = ExpressionHelper.GetPathForNewTable ("s1", "s1");
      MemberInfo member = typeof (Student).GetProperty ("First");
      Column column = new Column (path.FirstSource, "c1");
      FieldDescriptor descriptor = new FieldDescriptor (member, path, column);

      OrderingField field1 = new OrderingField (descriptor, (OrderDirection) int.MaxValue);
      List<OrderingField> orderingFields = new List<OrderingField> { field1 };

      orderByBuilder.BuildOrderByPart (orderingFields);
    }
  }
}