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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  [Serializable]
  public class NewObjectFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public NewObjectFunction ()
      : base (WxeTransactionMode.CreateRootWithAutoCommit)
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    public ClassWithAllDataTypes ObjectWithAllDataTypes
    {
      get { return (ClassWithAllDataTypes) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1 ()
    {
      ObjectWithAllDataTypes = ClassWithAllDataTypes.NewObject();

      ClassWithAllDataTypes objectWithAllDataTypes2 = CreateTestObjectWithAllDataTypes();

      ClassForRelationTest objectForRelationTest1 = ClassForRelationTest.NewObject();
      objectForRelationTest1.Name = "ObjectForRelationTest1";
      objectForRelationTest1.ClassWithAllDataTypesMandatory = ObjectWithAllDataTypes;
      objectWithAllDataTypes2.ClassForRelationTestMandatory = objectForRelationTest1;

      ClassForRelationTest objectForRelationTest2 = ClassForRelationTest.NewObject();
      objectForRelationTest2.Name = "ObjectForRelationTest2";
      ObjectWithAllDataTypes.ClassForRelationTestMandatory = objectForRelationTest2;
      objectForRelationTest2.ClassWithAllDataTypesMandatory = objectWithAllDataTypes2;
    }

    private WxePageStep Step2 = new WxePageStep ("NewObject.aspx");


    private ClassWithAllDataTypes CreateTestObjectWithAllDataTypes ()
    {
      ClassWithAllDataTypes test = ClassWithAllDataTypes.NewObject();

      test.ByteProperty = 23;
      test.DateProperty = DateTime.Now;
      test.DateTimeProperty = DateTime.Now;
      test.DecimalProperty = decimal.Parse ("23.2");
      test.DoubleProperty = 23.2;
      test.GuidProperty = new Guid ("{00000008-0000-0000-0000-000000000009}");
      test.Int16Property = 2;
      test.Int32Property = 4;
      test.Int64Property = 8;
      test.SingleProperty = Single.Parse ("9.8");
      test.StringProperty = "aasdf";

      return test;
    }
  }
}
