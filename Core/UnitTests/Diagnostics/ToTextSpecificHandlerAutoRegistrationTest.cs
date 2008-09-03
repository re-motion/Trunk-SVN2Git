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
using System.Collections;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Handlers;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTextSpecificHandlerAutoRegistrationTest
  {
    private readonly ISimpleLogger log = SimpleLogger.CreateForConsole (true);

    private class TestSimpleSimple
    {
      public TestSimpleSimple ()
      {
        Name = "I like it";
      }

      public string Name { get; set; }

      public override string ToString ()
      {
        return String.Format ("((TestSimple) Name:{0})", Name);
      }
    }

    private class TestSimple
    {
      public TestSimple ()
      {
        Name = "ABC abc";
        Int = 54321;
      }

      public TestSimple (string name, int i)
      {
        Name = name;
        Int = i;
      }

      public string Name { get; set; }
      public int Int { get; set; }

      public override string ToString ()
      {
        return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
      }
    }

    //[ToTextHandlerTargetType (typeof (TestSimple))]
    [ToTextSpecificHandler]
    class TestSimpleToTextSpecificTypeHandler : ToTextSpecificTypeHandler<TestSimple>
    {
      public override void ToText (TestSimple t, ToTextBuilder toTextBuilder)
      {
        toTextBuilder.sb ("[", ",", "]").tt ("TestSimple").tt (t.Name).tt (t.Int).se ();
      }
    }

    [ToTextSpecificHandler]
    class TestSimpleSimpleToTextSpecificTypeHandler : ToTextSpecificTypeHandler<TestSimpleSimple>
    {
      public override void ToText (TestSimpleSimple t, ToTextBuilder toTextBuilder)
      {
        //toTextBuilder.beginInstance (typeof (TestSimple)).m ("name", t.Name).endInstance ();
        //toTextBuilder.sb ("[", ",", "]").s ("name=").tt (t.Name).se ();
        toTextBuilder.sb ("[", ",", "]").tt ("TestSimpleSimple").tt (t.Name).se ();
      }
    }



    private ToTextSpecificHandlerAttribute RetrieveTextHandlerAttribute<T> ()
    {
      return AttributeUtility.GetCustomAttribute<ToTextSpecificHandlerAttribute> (typeof (T), false);
    }

    private ToTextSpecificHandlerAttribute RetrieveTextHandlerAttribute (Type type)
    {
      return AttributeUtility.GetCustomAttribute<ToTextSpecificHandlerAttribute> (type, false);
    }


    [Test]
    public void RetrieveTestSimpleToTextSpecificTypeHandlerAttributeTest ()
    {
      ToTextSpecificHandlerAttribute attribute = RetrieveTextHandlerAttribute<TestSimpleToTextSpecificTypeHandler>();
      Assert.That (attribute, Is.Not.Null);
      //Assert.That (attribute.Type, Is.EqualTo (typeof (TestSimple)));
    }



    [Test]
    public void SimpleToTextSpecificTypeHandlerAttributeTest ()
    {
      ITypeDiscoveryService _typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
          new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, false));

      const bool excludeGlobalTypes = true;
      ICollection types = _typeDiscoveryService.GetTypes (typeof (IToTextSpecificTypeHandler), excludeGlobalTypes);

      foreach (Type type in types)
      {
        log.It ("type=" + type);

        if (type == typeof (TestSimpleToTextSpecificTypeHandler)) {
          Assert.That (RetrieveTextHandlerAttribute (type), Is.Not.Null);
          log.It ("attribute found");
          Type baseType = type.BaseType;
          Assert.That (baseType.IsGenericType, Is.True);
          log.It ("baseType.Name=" + baseType.Name);
          Type[] genericArguments = baseType.GetGenericArguments();
          log.Item ("List type arguments ({0}):", genericArguments.Length);
          Assert.That (genericArguments.Length, Is.EqualTo (1));
          Type handledType = genericArguments[0];
          Assert.That (handledType, Is.EqualTo(typeof(TestSimple)));
        }
      }
    }




    [Test]
    public void AutoregisterTest ()
    {
      var toTextProvider = new ToTextProvider ();
      var testSimple = new TestSimple ();
      //Assert.That (toTextProvider.ToTextString (testSimple), Is.EqualTo (testSimple.ToString()));
      var resultWithNoHandler = ToTextProviderHandlerToText<ToTextProviderRegisteredTypeHandler> (testSimple, toTextProvider);
      Assert.That (resultWithNoHandler, Is.EqualTo (""));

      var autoRegistrator = new ToTextSpecificHandlerCollector ();
      autoRegistrator.FindAndRegister (toTextProvider);
      //var result = toTextProvider.ToTextString (testSimple);
      var resultWithHandler = ToTextProviderHandlerToText<ToTextProviderRegisteredTypeHandler> (testSimple, toTextProvider);
      log.It ("testSimple=" + testSimple);
      //Assert.That (resultWithHandler, Is.EqualTo ("[TestSimple  The Name=ABC abc,The Number=54321]"));
      Assert.That (resultWithHandler, Is.EqualTo ("[TestSimple,ABC abc,54321]"));
    }

    [Test]
    public void AutoregisterMultipleHandlersTest ()
    {
      var toTextProvider = new ToTextProvider ();
      var testSimple = new TestSimple ();
      var testSimpleSimple = new TestSimpleSimple ();

      var autoRegistrator = new ToTextSpecificHandlerCollector ();
      autoRegistrator.FindAndRegister (toTextProvider);
      var testSimpleResult = ToTextProviderHandlerToText<ToTextProviderRegisteredTypeHandler> (testSimple, toTextProvider);
      var testSimpleSimpleResult = ToTextProviderHandlerToText<ToTextProviderRegisteredTypeHandler> (testSimpleSimple, toTextProvider);
      log.It ("testSimpleResult=" + testSimpleResult);
      log.It ("testSimpleSimpleResult=" + testSimpleSimpleResult);
      Assert.That (testSimpleResult, Is.EqualTo ("[TestSimple,ABC abc,54321]"));
      Assert.That (testSimpleSimpleResult, Is.EqualTo ("[TestSimpleSimple,I like it]"));
    }


    [Test]
    [Ignore]
    public void DiscoverMultipleHandlersTest ()
    {
      var toTextProvider = new ToTextProvider ();
      var testSimple = new TestSimple ();
      var testSimpleSimple = new TestSimpleSimple ();

      var autoRegistrator = new ToTextSpecificHandlerCollector ();
      //autoRegistrator.FindAndRegister (toTextProvider);

      var handlerMap = autoRegistrator.DiscoverHandlers<IToTextSpecificTypeHandler>();

      var testSimpleResult = ToTextProviderHandlerToText<ToTextProviderRegisteredTypeHandler> (testSimple, toTextProvider);
      var testSimpleSimpleResult = ToTextProviderHandlerToText<ToTextProviderRegisteredTypeHandler> (testSimpleSimple, toTextProvider);
      log.It ("testSimpleResult=" + testSimpleResult);
      log.It ("testSimpleSimpleResult=" + testSimpleSimpleResult);
      Assert.That (testSimpleResult, Is.EqualTo ("[TestSimple,ABC abc,54321]"));
      Assert.That (testSimpleSimpleResult, Is.EqualTo ("[TestSimpleSimple,I like it]"));
    }



    private string ToTextProviderHandlerToText<T> (object obj, ToTextProvider toTextProvider) where T : IToTextProviderHandler
    {
      var toTextParameters = ToTextProviderTest.CreateToTextParameters (obj);
      var feedback = new ToTextProviderHandlerFeedback ();
      toTextProvider.GetToTextProviderHandler<T> ().ToTextIfTypeMatches (toTextParameters, feedback);
      var result = toTextParameters.ToTextBuilder.CheckAndConvertToString();
      return result;
    }


    // Logging
    private void LogVariables (string format, params object[] parameters)
    {
      log.Item (format, parameters);
    }
    
    private void Log (string s)
    {
      log.It (s);
    }
  }


}