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
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements a strong-typed wrapper for <b><a href="http://l2sprof.com/">Linq to Sql Profiler</a></b>. (Tested for build 661)
  /// </summary>
  /// <remarks>
  /// The wrapper uses runtime-binding to redirect the calls to Linq to Sql Profiler's API. This removes the static dependcy on Linq to Sql Profiler.
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public sealed class LinqToSqlAppender : IObjectReference
  {
    private static readonly DoubleCheckedLockingContainer<LinqToSqlAppender> _instance =
        new DoubleCheckedLockingContainer<LinqToSqlAppender> (() => new LinqToSqlAppender("re-store ClientTransaction"));

    public static LinqToSqlAppender Instance
    {
      get { return _instance.Value; }
    }

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

    private LinqToSqlAppender (string name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      Type linqToSqlProfilerType =
          Type.GetType ("HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlProfiler, HibernatingRhinos.Profiler.Appender", true, false);

      Type linqToSqlAppenderType =
          Type.GetType ("HibernatingRhinos.Profiler.Appender.LinqToSql.LinqToSqlAppender, HibernatingRhinos.Profiler.Appender", true, false);

      CreateDelegate<Action> (linqToSqlProfilerType, "Initialize")();
      var getAppender = CreateDelegate (
          linqToSqlProfilerType, "GetAppender", typeof (Func<,>).MakeGenericType (typeof (string), linqToSqlAppenderType));
      _linqToSqlAppender = getAppender.DynamicInvoke (name);

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

    public void ConnectionStarted (Guid sessionID)
    {
      _connectionStarted (sessionID);
    }

    public void ConnectionDisposed (Guid sessionID)
    {
      _connectionDisposed (sessionID);
    }

    public void StatementRowCount (Guid sessionID, Guid queryID, int rowCount)
    {
      _statementRowCount (sessionID, queryID, rowCount);
    }

    public void StatementError (Guid sessionID, Exception e)
    {
      _statementError (sessionID, e);
    }

    public void CommandDurationAndRowCount (Guid sessionID, long milliseconds, int? rowCount)
    {
      _commandDurationAndRowCount (sessionID, milliseconds, rowCount);
    }

    public void StatementExecuted (Guid sessionID, Guid queryID, string statement)
    {
      _statementExecuted (sessionID, queryID, statement);
    }

    public void TransactionBegan (Guid sessionID, IsolationLevel isolationLevel)
    {
      _transactionBegan (sessionID, isolationLevel);
    }

    public void TransactionCommit (Guid sessionID)
    {
      _transactionCommit (sessionID);
    }

    public void TransactionDisposed (Guid sessionID)
    {
      _transactionDisposed (sessionID);
    }

    public void TransactionRolledBack (Guid sessionID)
    {
      _transactionRolledBack (sessionID);
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

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return LinqToSqlAppender.Instance;
    }
  }
}