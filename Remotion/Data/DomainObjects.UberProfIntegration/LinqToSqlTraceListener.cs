// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements <see cref="IPersistenceTraceListener"/> for <b><a href="http://l2sprof.com/">Linq to Sql Profiler</a></b>. (Tested for build 661)
  /// </summary>
  /// <remarks>
  /// The instantiation is comparatively expensive due to the use of Reflection for binding to Linq to Sql Profiler's API.
  /// Invocation of the actual profiling API only has minimal overhead compared to a statically bound trace listener implementation.
  /// Therefor, it is recommended to register the <see cref="LinqToSqlTraceListener"/> in a singleton-configuration in your IoC container.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public class LinqToSqlTraceListener : IPersistenceTraceListener
  {
    private readonly object _linqToSqlAppender;
    private readonly Action<Guid> _connectionStarted;
    private readonly Action<Guid> _connectionDisposed;
    private readonly Action<Guid, Guid, int> _statementRowCount;
    private readonly Action<Guid, Exception> _statementError;
    private readonly Action<Guid, long, int?> _commandDurationAndRowCount;
    private readonly Action<Guid, Guid, string> _statementExecuted;
    private readonly Action<Guid, IsolationLevel> _transactionBegan;
    private readonly Action<Guid> _transactionCommit;
    private readonly Action<Guid> _transactionDisposed;
    private readonly Action<Guid> _transactionRolledBack;

    public LinqToSqlTraceListener ()
    {
      Type linqToSqlProfilerType =
          Type.GetType ("HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlProfiler, HibernatingRhinos.Profiler.Appender", true, false);

      Type linqToSqlAppenderType =
          Type.GetType ("HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlAppender, HibernatingRhinos.Profiler.Appender", true, false);

      CreateDelegate<Action> (linqToSqlProfilerType, "Initialize")();
      var getAppender = CreateDelegate (
          linqToSqlProfilerType, "GetAppender", typeof (Func<,>).MakeGenericType (typeof (string), linqToSqlAppenderType));
      _linqToSqlAppender = getAppender.DynamicInvoke ("re-store UberProf PoC - RestoreProfiler");

      _connectionStarted = CreateDelegate<Action<Guid>> (_linqToSqlAppender, "ConnectionStarted");
      _connectionDisposed = CreateDelegate<Action<Guid>> (_linqToSqlAppender, "ConnectionDisposed");
      _statementRowCount = CreateDelegate<Action<Guid, Guid, int>> (_linqToSqlAppender, "StatementRowCount");
      _statementError = CreateDelegate<Action<Guid, Exception>> (_linqToSqlAppender, "StatementError");
      _commandDurationAndRowCount = CreateDelegate<Action<Guid, long, int?>> (_linqToSqlAppender, "CommandDurationAndRowCount");
      _statementExecuted = CreateDelegate<Action<Guid, Guid, string>> (_linqToSqlAppender, "StatementExecuted");
      _transactionBegan = CreateDelegate<Action<Guid, IsolationLevel>> (_linqToSqlAppender, "TransactionBegan");
      _transactionCommit = CreateDelegate<Action<Guid>> (_linqToSqlAppender, "TransactionCommit");
      _transactionDisposed = CreateDelegate<Action<Guid>> (_linqToSqlAppender, "TransactionDisposed");
      _transactionRolledBack = CreateDelegate<Action<Guid>> (_linqToSqlAppender, "TransactionRolledBack");
    }

    public void TraceConnectionOpened (Guid clientTransactionID, Guid connectionID)
    {
      _connectionStarted (clientTransactionID);
    }

    public void TraceConnectionClosed (Guid clientTransactionID, Guid connectionID)
    {
      _connectionDisposed (clientTransactionID);
    }

    public void TraceQueryCompleted (Guid clientTransactionID, Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
      _statementRowCount (clientTransactionID, queryID, rowCount);
    }

    public void TraceQueryError (Guid clientTransactionID, Guid connectionID, Guid queryID, Exception e)
    {
      _statementError (clientTransactionID, e);
    }

    public void TraceQueryExecuted (Guid clientTransactionID, Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
      _commandDurationAndRowCount (clientTransactionID, (long) durationOfQueryExecution.Milliseconds, (int?) null);
    }

    public void TraceQueryExecuting (
        Guid clientTransactionID, Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
      _statementExecuted (clientTransactionID, queryID, AppendParametersToCommandText (commandText, parameters));
    }

    public void TraceTransactionBegan (Guid clientTransactionID, Guid connectionID, IsolationLevel isolationLevel)
    {
      _transactionBegan (clientTransactionID, isolationLevel);
    }

    public void TraceTransactionCommitted (Guid clientTransactionID, Guid connectionID)
    {
      _transactionCommit (clientTransactionID);
    }

    public void TraceTransactionDisposed (Guid clientTransactionID, Guid connectionID)
    {
      _transactionDisposed (clientTransactionID);
    }

    public void TraceTransactionRolledback (Guid clientTransactionID, Guid connectionID)
    {
      _transactionRolledBack (clientTransactionID);
    }

    private string AppendParametersToCommandText (string commandText, IDictionary<string, object> parameters)
    {
      StringBuilder builder = new StringBuilder (commandText).AppendLine().AppendLine ("-- Parameters:");
      foreach (string str in parameters.Keys)
      {
        string str2 = str;
        if (!str2.StartsWith ("@"))
          str2 = "@" + str2;
        builder.Append ("-- ").Append (str2).Append (" = [-[").Append (parameters[str]).AppendLine ("]-]");
      }
      return builder.ToString();
    }

    private TSignature CreateDelegate<TSignature> (object target, string methodName)
    {
      try
      {
        return (TSignature) (object) Delegate.CreateDelegate (typeof (TSignature), target, methodName, false, true);
      }
      catch (ArgumentException ex)
      {
        throw CreateMissingMethodException (target, methodName, typeof (TSignature), ex);
      }
    }

    private TSignature CreateDelegate<TSignature> (Type target, string methodName)
    {
      return (TSignature) (object) CreateDelegate (target, methodName, typeof (TSignature));
    }

    private Delegate CreateDelegate (Type target, string methodName, Type signature)
    {
      try
      {
        return Delegate.CreateDelegate (signature, target, methodName, false, true);
      }
      catch (ArgumentException ex)
      {
        throw CreateMissingMethodException (target, methodName, signature, ex);
      }
    }

    private MissingMethodException CreateMissingMethodException (object target, string methodName, Type signatureType, Exception innerException)
    {
      Type targetType = (target is Type) ? (Type) target : target.GetType();

      Assertion.IsTrue (typeof (Delegate).IsAssignableFrom (signatureType));
      MethodInfo invoke = signatureType.GetMethod ("Invoke");
      Type returnType = invoke.ReturnType;
      Type[] parameters = invoke.GetParameters().Select (p => p.ParameterType).ToArray();

      return new MissingMethodException (
          string.Format (
              "Type {0} does not define a method {3} {1}({2}).",
              targetType.AssemblyQualifiedName,
              methodName,
              StringUtility.ConcatWithSeparator (parameters, ", "),
              returnType == typeof (void) ? "void" : returnType.FullName),
          innerException);
    }
  }
}