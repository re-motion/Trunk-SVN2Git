using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class TryFinallyStatement : Statement
  {
    private readonly IEnumerable<Statement> _tryStatements;
    private readonly IEnumerable<Statement> _finallyStatements;

    public TryFinallyStatement (IEnumerable<Statement> tryStatements, IEnumerable<Statement> finallyStatements)
    {
      ArgumentUtility.CheckNotNull ("tryStatements", tryStatements);
      ArgumentUtility.CheckNotNull ("finallyStatements", finallyStatements);

      _tryStatements = tryStatements;
      _finallyStatements = finallyStatements;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("gen", gen);

      gen.BeginExceptionBlock ();

      foreach (Statement statement in _tryStatements)
        statement.Emit (member, gen);

      gen.BeginFinallyBlock ();
      
      foreach (Statement statement in _finallyStatements)
        statement.Emit (member, gen);

      gen.EndExceptionBlock ();
      gen.Emit (OpCodes.Nop); // ensure a leave target for try block
    }
  }
}