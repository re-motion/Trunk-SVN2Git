﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Linq;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace Remotion.TypePipe.IntegrationTests.TypeAssembly
{
  [TestFixture]
  public class InterfaceImplementationTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void Implement ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IAddedInterface obj) => obj.AddedMethod());
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            mutableType.AddInterface (typeof (IAddedInterface));
            var method = mutableType.GetOrAddMutableMethod (interfaceMethod);
            method.SetBody (
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  Assert.That (ctx.HasPreviousBody, Is.False);

                  return Expression.Constant ("implemented");
                });
          });

      var instance = (IAddedInterface) Activator.CreateInstance (type);

      Assert.That (instance.AddedMethod(), Is.EqualTo ("implemented"));
    }

    [Ignore ("TODO 5229")]
    [Test]
    public void Implement_InvalidCandidates ()
    {
      var interfaceMethod1 = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IInvalidCandidates obj) => obj.NonPublicCandidate());
      var interfaceMethod2 = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IInvalidCandidates obj) => obj.NonVirtualCandidate());

      AssembleType<DomainType> (
          mutableType =>
          {
            mutableType.AddInterface (typeof (IInvalidCandidates));

            var messageFormat = "Interface method '{0}' cannot be implemented because a method with equal name and signature already "
                                + "exists. Use MutableType.AddExplicitOverride to create an explicit implementation.";
            Assert.That (
                () => mutableType.GetOrAddMutableMethod (interfaceMethod1),
                Throws.InvalidOperationException.With.Message.EqualTo (string.Format (messageFormat, interfaceMethod1.Name)));
            Assert.That (
                () => mutableType.GetOrAddMutableMethod (interfaceMethod2),
                Throws.InvalidOperationException.With.Message.EqualTo (string.Format (messageFormat, interfaceMethod2.Name)));

            // Implement the interface, otherwise the type is invalid and cannot be generated.
            mutableType.AddExplicitOverride (interfaceMethod1).SetBody (ctx => Expression.Empty());
            mutableType.AddExplicitOverride (interfaceMethod2).SetBody (ctx => Expression.Empty());
          });
    }

    [Test]
    public void Modify_Implicit ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.Method());
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var method = mutableType.GetOrAddMutableMethod (interfaceMethod);
            method.SetBody (
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return ExpressionHelper.StringConcat (ctx.PreviousBody, Expression.Constant (" modified"));
                });
          });

      var instance = (IDomainInterface) Activator.CreateInstance (type);

      Assert.That (instance.Method(), Is.EqualTo ("DomainType.Method modified"));
    }

    [Test]
    public void Modify_Explicit_Added ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.Method());
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var explicitImplementation = mutableType.AllMutableMethods.Single (m => m.Name == "UnrelatedMethod");
            explicitImplementation.AddExplicitBaseDefinition (interfaceMethod);

            var method = mutableType.GetOrAddMutableMethod (interfaceMethod);
            method.SetBody (
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return ExpressionHelper.StringConcat (ctx.PreviousBody, Expression.Constant (" modified"));
                });
          });

      var instance = (IDomainInterface) Activator.CreateInstance (type);

      Assert.That (instance.Method(), Is.EqualTo ("DomainType.UnrelatedMethod modified"));
    }

    [Test]
    public void Modify_Explicit_Existing ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IDomainInterface obj) => obj.ExplicitlyImplemented());
      AssembleType<DomainType> (
          mutableType =>
          {
            var method = mutableType.GetOrAddMutableMethod (interfaceMethod);

            Assert.That (method.CanSetBody, Is.False);
            Assert.That (method.BaseMethod, Is.Null);
          });
    }

    [Test]
    public void Modify_Explicit_ExistingOnBase ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IBaseInterface obj) => obj.ExplicitlyImplemented());
      var message = "Cannot override final method 'DomainTypeBase.Remotion.TypePipe.IntegrationTests.TypeAssembly."
                    + "InterfaceImplementationTest.IBaseInterface.ExplicitlyImplemented'.";
      AssembleType<DomainType> (
          mt => Assert.That (() => mt.GetOrAddMutableMethod (interfaceMethod), Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (message)));
    }

    [Test]
    public void Override_Implicit ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IBaseInterface obj) => obj.BaseMethod());
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var method = mutableType.GetOrAddMutableMethod (interfaceMethod);
            method.SetBody (
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.True);
                  return ExpressionHelper.StringConcat (ctx.PreviousBody, Expression.Constant (" implicitly overridden"));
                });
          });

      var instance = (IBaseInterface) Activator.CreateInstance (type);

      Assert.That (instance.BaseMethod(), Is.EqualTo ("DomainTypeBase.BaseMethod implicitly overridden"));
    }

    [Test]
    public void Override_Explicit_ShadowedBase ()
    {
      var interfaceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((IBaseInterface obj) => obj.ShadowedBaseMethod());
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var method = mutableType.GetOrAddMutableMethod (interfaceMethod);
            method.SetBody (
                ctx =>
                {
                  Assert.That (ctx.HasBaseMethod, Is.False);
                  return ExpressionHelper.StringConcat (ctx.PreviousBody, Expression.Constant (" explicitly overridden"));
                });
          });

      var instance = (IBaseInterface) Activator.CreateInstance (type);

      Assert.That (instance.ShadowedBaseMethod(), Is.EqualTo ("DomainTypeBase.ShadowedBaseMethod explicitly overridden"));
    }

    public abstract class DomainTypeBase : IBaseInterface
    {
      public virtual string BaseMethod ()
      {
        return "DomainTypeBase.BaseMethod";
      }

      public virtual string ShadowedBaseMethod ()
      {
        return "DomainTypeBase.ShadowedBaseMethod";
      }

      void IBaseInterface.ExplicitlyImplemented () { }
    }

    public class DomainType : DomainTypeBase, IDomainInterface
    {
      public virtual string Method ()
      {
        return "DomainType.Method";
      }

      void IDomainInterface.ExplicitlyImplemented () { }

      public new string ShadowedBaseMethod ()
      {
        return "DomainType.ShadowedBaseMethod";
      }

      public virtual string UnrelatedMethod ()
      {
        return "DomainType.UnrelatedMethod";
      }

      internal virtual void NonPublicCandidate () { }
      public void NonVirtualCandidate () { }
    }

    public interface IBaseInterface
    {
      string BaseMethod ();
      string ShadowedBaseMethod ();
      void ExplicitlyImplemented ();
    }
    public interface IDomainInterface
    {
      string Method ();
      void ExplicitlyImplemented ();
    }
    public interface IAddedInterface
    {
      string AddedMethod ();
    }
    public interface IInvalidCandidates
    {
      void NonPublicCandidate ();
      void NonVirtualCandidate ();
    }
  }
}