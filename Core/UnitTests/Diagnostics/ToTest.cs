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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;
using Remotion.Diagnostics.ToText;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class ToTest
  {
    // TODO: Store & replace the ToTextProvider To.Text uses and restore it after the tests.

    //public class Test
    //{
    //  public Test ()
    //  {
    //    Name = "DefaultName";
    //    Int = 1234567;
    //  }

    //  public Test (string name, int i0)
    //  {
    //    Name = name;
    //    Int = i0;
    //    ListListString = new List<List<string>>();
    //  }

    //  public string Name { get; set; }
    //  public int Int { get; set; }
    //  public LinkedList<string> LinkedListString { get; set; }
    //  public List<List<string>> ListListString { get; set; }
    //  public Object[][][] Array3D { get; set; }
    //  public Object[,] RectangularArray2D { get; set; }
    //  public Object[,,] RectangularArray3D { get; set; }
    //}

    //public class Test2
    //{
    //  public Test2 ()
    //  {
    //    Name = "DefaultName";
    //    Int = 1234567;
    //  }

    //  public Test2 (string name, int i0)
    //  {
    //    Name = name;
    //    Int = i0;
    //    ListListString = new List<List<string>>();
    //  }

    //  public string Name { get; set; }
    //  public int Int { get; set; }
    //  public LinkedList<string> LinkedListString { get; set; }
    //  public List<List<string>> ListListString { get; set; }
    //  public Object[][][] Array3D { get; set; }
    //  public Object[,] RectangularArray2D { get; set; }
    //  public Object[,,] RectangularArray3D { get; set; }
    //}


    //[Test]
    //[Ignore]
    //public void ObjectTest ()
    //{
    //  Object o = 5711;
    //  Assert.That (To.Text (o), Is.EqualTo (o.ToString()));

    //  Object o2 = new object();
    //  Assert.That (To.Text (o2), Is.EqualTo (o2.ToString()));
    //}

    //[Test]
    //public void FallbackToStringTest ()
    //{
    //  //FallbackToStringTestSingleType ("abcd EFGH");
    //  FallbackToStringTestSingleType (87971132);
    //  //FallbackToStringTestSingleType (4786.5323);
    //  int i = 8723;
    //  FallbackToStringTestSingleType (i);
    //}

    //private void FallbackToStringTestSingleType<T> (T t)
    //{
    //  Assert.That (To.Text (t), Is.EqualTo (t.ToString()));
    //}

    //private void RegisterHandlers ()
    //{
    //  To.RegisterHandler<Int32> ((x, ttb) => ttb.sbLiteral ("[Int32: ", "", ",", "", "]").ts (x).se());
    //  To.RegisterHandler<Test> ((x, ttb) => ttb.sbLiteral ("<<Test: ", "", ";", "", ">>").e ("Name", x.Name).e ("Int", x.Int).se());
    //}


    //[Test]
    //public void RegisteredHandlerTest ()
    //{
    //  To.ClearHandlers();
    //  RegisterHandlers();
    //  var test = new ToTest.Test ("That's not my name", 179);
    //  string toTextTest = To.Text (test);
    //  Log ("toTextTest=" + toTextTest);
    //  Assert.That (toTextTest, Is.EqualTo ("<<Test: Name=\"That's not my name\";Int=[Int32: 179]>>"));
    //}

    //[Test]
    //public void NullTest ()
    //{
    //  To.ClearHandlers();
    //  Object o = null;
    //  Assert.That (To.Text (o), Is.EqualTo ("null"));
    //}

    //[Test]
    //public void IntToStringFallbackTest ()
    //{
    //  To.ClearHandlers();
    //  int i = 908;
    //  Assert.That (To.Text (i), Is.EqualTo ("908"));
    //}


    //[Test]
    //public void InitStandardHandlersTest ()
    //{
    //  To.ClearHandlers();
    //  To.TextEnableAutomatics (true);
    //  Assert.That (To.Text ("Some text"), Is.EqualTo ("\"Some text\""));
    //  Assert.That (To.Text ('x'), Is.EqualTo ("'x'"));
    //}


    //[Test]
    //public void ClearHandlersTest ()
    //{
    //  To.RegisterSpecificTypeHandler<Object> (x => "[ClearHandlersTest]" + x);
    //  Object o = new object ();
    //  string toTextTest = To.Text (o);
    //  Log ("[ClearHandlersTest] toTextTest=" + toTextTest);
    //  Assert.That (To.Text (o), Is.EqualTo ("[ClearHandlersTest]" + o));
    //  To.ClearSpecificTypeHandlers();
    //  Assert.That (To.Text (o), Is.EqualTo (o.ToString()));
    //}


    public class ToTextTest {}
    [ToTextSpecificHandler]
    public class ToTextTestToTextSpecificTypeHandler : ToTextSpecificTypeHandler<ToTextTest>
    {
      public override void ToText (ToTextTest t, IToTextBuilderBase toTextBuilder)
      {
        toTextBuilder.s ("handled by ToTextTestToTextSpecificTypeHandler");
      }
    }

    public interface IToTextInterfaceTest {}
    public class ToTextInterfaceTest : IToTextInterfaceTest  { }
    [ToTextSpecificHandler]
    public class IToTextInterfaceTestToTextSpecificTypeHandler : ToTextSpecificInterfaceHandler<IToTextInterfaceTest>
    {
      public override void ToText (IToTextInterfaceTest t, IToTextBuilderBase toTextBuilder)
      {
        toTextBuilder.s ("handled by IToTextInterfaceTestToTextSpecificTypeHandler");
      }
    }


    [Test]
    public void GetTypeHandlersTest ()
    {
      var handlerMap = To.GetTypeHandlers ();
      Assert.That (handlerMap.Count, Is.GreaterThan(1));

      IToTextSpecificTypeHandler simpleHandler;
      handlerMap.TryGetValue (typeof (ToTextTest), out simpleHandler);
      Assert.That (simpleHandler, Is.Not.Null);
      Assert.That (simpleHandler is ToTextTestToTextSpecificTypeHandler, Is.True);

      //foreach (var pair in handlerMap)
      //{
      //  Log (pair.Key + " " + pair.Value);
      //}
    }

    [Test]
    public void ToTextTestToTextSpecificTypeHandlerTest ()
    {
      Assert.That (To.Text (new ToTextTest ()), Is.EqualTo ("handled by ToTextTestToTextSpecificTypeHandler"));
    }

    [Test]
    public void IToTextInterfaceTestToTextSpecificTypeHandlerTest ()
    {
      var toTextProviderSettings = To.ToTextProvider.Settings;
      bool useInterfaceHandlers = toTextProviderSettings.UseInterfaceHandlers;
      try
      {
        toTextProviderSettings.UseInterfaceHandlers = true;
        Assert.That (To.Text (new ToTextInterfaceTest ()), Is.EqualTo ("handled by IToTextInterfaceTestToTextSpecificTypeHandler"));
      }
      finally
      {
        toTextProviderSettings.UseInterfaceHandlers = useInterfaceHandlers;
      }
    }


    [Test]
    public void ToConsoleTest ()
    {
      To.Console.s ("ToConsoleTest");
    }

    [Test]
    public void ToErrorTest ()
    {
      To.Error.s ("ToErrorTest");
    }

    [Test]
    public void ToTempLogTest ()
    {
      var s = @"  line1
line2   
line3";
      //To.TempLog.s ("ToTempLogTest").AppendRawEscapedString (s).s(s).e(x => s).Flush();
      To.TempLog.s ("ToTempLogTest").sEsc (s).s (s).e (x => s).Flush ();
      Log (System.IO.Path.GetTempPath());
    }


    public static void Log (string s)
    {
      Console.WriteLine (s);
    }

    public static void LogVariables (string format, params object[] parameterArray)
    {
      Log (String.Format (format, parameterArray));
    }
  }
}