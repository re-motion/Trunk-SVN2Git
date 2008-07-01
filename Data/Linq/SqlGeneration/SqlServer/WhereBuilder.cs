using System;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.Linq.SqlGeneration.SqlServer
{
  public class WhereBuilder : IWhereBuilder
  {
    private readonly CommandBuilder _commandBuilder;
    private readonly IDatabaseInfo _databaseInfo;
    private readonly BinaryConditionBuilder _builder;

    public WhereBuilder (CommandBuilder commandBuilder, IDatabaseInfo databaseInfo)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      _commandBuilder = commandBuilder;
      _databaseInfo = databaseInfo;
      _builder = new BinaryConditionBuilder (_commandBuilder, _databaseInfo);
    }

    public void BuildWherePart (ICriterion criterion)
    {
      if (criterion != null)
      {
        _commandBuilder.Append (" WHERE ");
        AppendCriterion (criterion);
      }
    }

    private void AppendCriterion (ICriterion criterion)
    {
      if (criterion is BinaryCondition)
        AppendBinaryCondition ((BinaryCondition) criterion);
      else if (criterion is ComplexCriterion)
        AppendComplexCriterion ((ComplexCriterion) criterion);
      else if (criterion is NotCriterion)
        AppendNotCriterion ((NotCriterion) criterion);
      else if (criterion is Constant || criterion is Column) // cannot use "as" operator here because Constant/Column are value types
        AppendTopLevelValue (criterion);
      else
        throw new NotSupportedException ("The criterion kind " + criterion.GetType ().Name + " is not supported.");
    }

    private void AppendBinaryCondition (BinaryCondition condition)
    {
      _builder.BuildBinaryConditionPart (condition);
    }

    private void AppendTopLevelValue (IValue value)
    {
      if (value is Constant)
      {
        Constant constant = (Constant) value;
        if (constant.Value == null)
          throw new NotSupportedException ("NULL constants are not supported as WHERE conditions.");
        else
          _commandBuilder.AppendConstant (constant);
      }
      else
      {
        _commandBuilder.AppendEvaluation ((Column) value);
        _commandBuilder.Append ("=1");
      }
    }

    private void AppendComplexCriterion (ComplexCriterion criterion)
    {
      _commandBuilder.Append ("(");
      AppendCriterion (criterion.Left);

      switch (criterion.Kind)
      {
        case ComplexCriterion.JunctionKind.And:
          _commandBuilder.Append (" AND ");
          break;
        case ComplexCriterion.JunctionKind.Or:
          _commandBuilder.Append (" OR ");
          break;
      }

      AppendCriterion (criterion.Right);
      _commandBuilder.Append (")");
    }

    private void AppendNotCriterion (NotCriterion criterion)
    {
      _commandBuilder.Append ("NOT ");
      AppendCriterion (criterion.NegatedCriterion);
    }
  }
}