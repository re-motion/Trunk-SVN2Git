// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class PropertyReferenceTest : SnippetGenerationBaseTest
  {
    [Test]
    public void InstancePropertyReference ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));
      propertyEmitter.CreateGetMethod();
      propertyEmitter.CreateSetMethod();
      propertyEmitter.ImplementWithBackingField ();

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
          .SetReturnType (typeof (string));
      
      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);
      Assert.AreEqual (typeof (string), propertyWithSelfOwner.Type);
      
      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithSelfOwner.ToExpression()));
      methodEmitter.AddStatement (new AssignStatement (propertyWithSelfOwner, new ConstReference ("New").ToExpression()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      object instance = GetBuiltInstance ();
      PrivateInvoke.SetPublicProperty (instance, "Property", "Old");
      Assert.AreEqual ("Old", InvokeMethod());
      Assert.AreEqual ("New", PrivateInvoke.GetPublicProperty (instance, "Property"));
    }

    [Test]
    public void StaticPropertyReference ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Static, typeof (string));
      propertyEmitter.CreateGetMethod ();
      propertyEmitter.CreateSetMethod ();
      propertyEmitter.ImplementWithBackingField ();

      CustomMethodEmitter methodEmitter = GetMethodEmitter (true)
          .SetReturnType (typeof (string));

      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithNoOwner = new PropertyReference (null, propertyEmitter.PropertyBuilder);
      Assert.AreEqual (typeof (string), propertyWithNoOwner.Type);

      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithNoOwner.ToExpression ()));
      methodEmitter.AddStatement (new AssignStatement (propertyWithNoOwner, new ConstReference ("New").ToExpression ()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      PrivateInvoke.SetPublicStaticProperty (GetBuiltType(), "Property", "Old");
      Assert.AreEqual ("Old", InvokeMethod());
      Assert.AreEqual ("New", PrivateInvoke.GetPublicStaticProperty (GetBuiltType (), "Property"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The property .*.Property cannot be loaded, it has no getter.", MatchType = MessageMatch.Regex)]
    public void LoadPropertyWithoutGetterThrows ()
    {
      CustomPropertyEmitter propertyEmitter = UnsavedClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      CustomMethodEmitter methodEmitter = GetUnsavedMethodEmitter (false).SetReturnType (typeof (string));

      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithSelfOwner.ToExpression ()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      GetUnsavedBuiltType ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The property .*.Property cannot be stored, it has no setter.", MatchType = MessageMatch.Regex)]
    public void SavePropertyWithoutSetterThrows ()
    {
      CustomPropertyEmitter propertyEmitter = UnsavedClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      CustomMethodEmitter methodEmitter = GetUnsavedMethodEmitter (false)
          .SetReturnType (typeof (string));

      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (propertyWithSelfOwner, NullExpression.Instance));
      methodEmitter.AddStatement (new ReturnStatement (NullExpression.Instance));

      GetUnsavedBuiltType ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "A property's address cannot be loaded.")]
    public void LoadPropertyAddressThrows ()
    {
      CustomPropertyEmitter propertyEmitter = UnsavedClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      CustomMethodEmitter methodEmitter = GetUnsavedMethodEmitter (false)
          .SetReturnType (typeof (string));

      LocalReference valueAddress = methodEmitter.DeclareLocal (typeof (string).MakeByRefType());
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (valueAddress, propertyWithSelfOwner.ToAddressOfExpression()));
      methodEmitter.AddStatement (new ReturnStatement (NullExpression.Instance));

      GetUnsavedBuiltType ();
    }
  }
}
