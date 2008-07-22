/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class InvalidatedTransactionListenerTest
  {
    [Test]
    public void ListenerIsSerializable ()
    {
      InvalidatedTransactionListener listener = new InvalidatedTransactionListener();
      InvalidatedTransactionListener deserializedListener = Serializer.SerializeAndDeserialize (listener);
      Assert.IsNotNull (deserializedListener);
    }

    [Test]
    public void AllMethodsMustThrow ()
    {
      InvalidatedTransactionListener listener = new InvalidatedTransactionListener();
      MethodInfo[] methods = typeof (InvalidatedTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      Assert.AreEqual (34, methods.Length);

      foreach (MethodInfo method in methods)
      {
        object[] arguments =
            Array.ConvertAll<ParameterInfo, object> (method.GetParameters(), delegate (ParameterInfo p) { return GetDefaultValue (p.ParameterType); });

        ExpectException (typeof (InvalidOperationException), "The transaction can no longer be used because it has been discarded.",
            listener, method, arguments);
      }
    }

    private void ExpectException (Type expectedExceptionType, string expectedMessage, InvalidatedTransactionListener listener,
        MethodInfo method, object[] arguments)
    {
      try
      {
        method.Invoke (listener, arguments);
        Assert.Fail (BuildErrorMessage (expectedExceptionType, method, arguments, "the call succeeded."));
      }
      catch (TargetInvocationException tex)
      {
        Exception ex = tex.InnerException;
        if (ex.GetType () == expectedExceptionType)
        {
          if (ex.Message == expectedMessage)
            return;
          else
            Assert.Fail (
                BuildErrorMessage (
                    expectedExceptionType,
                    method,
                    arguments,
                    string.Format ("the message was incorrect.\nExpected: {0}\nWas:      {1}", expectedMessage, ex.Message)));
        }
        else
        {
          Assert.Fail (BuildErrorMessage (expectedExceptionType, method, arguments, "the exception type was " + ex.GetType() + ".\n" + ex.ToString ()));
        }
      }
    }

    private string BuildErrorMessage (Type expectedExceptionType, MethodInfo method, object[] arguments, string problem)
    {
      return string.Format ("Expected {0} when calling {1}({2}), but {3}", expectedExceptionType, method.Name,
         ReflectionUtility.GetSignatureForArguments (arguments), problem);
    }

    private object GetDefaultValue (Type t)
    {
      if (t.IsValueType)
        return Activator.CreateInstance (t);
      else
        return null;
    }
  }
}
