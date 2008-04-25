using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Development.UnitTesting;

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
        ExpectedMessage = "The property PropertyReferenceTest.Property cannot be loaded, it has no getter.")]
    public void LoadPropertyWithoutGetterThrows ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
          .SetReturnType (typeof (string));

      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithSelfOwner.ToExpression ()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      SuppressAssemblySave ();

      GetBuiltType ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The property PropertyReferenceTest.Property cannot be stored, it has no setter.")]
    public void SavePropertyWithoutSetterThrows ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
          .SetReturnType (typeof (string));

      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (propertyWithSelfOwner, NullExpression.Instance));
      methodEmitter.AddStatement (new ReturnStatement (NullExpression.Instance));

      SuppressAssemblySave ();

      GetBuiltType ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "A property's address cannot be loaded.")]
    public void LoadPropertyAddressThrows ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      CustomMethodEmitter methodEmitter = GetMethodEmitter (false)
          .SetReturnType (typeof (string));

      LocalReference valueAddress = methodEmitter.DeclareLocal (typeof (string).MakeByRefType());
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (valueAddress, propertyWithSelfOwner.ToAddressOfExpression()));
      methodEmitter.AddStatement (new ReturnStatement (NullExpression.Instance));

      SuppressAssemblySave ();

      GetBuiltType ();
    }
  }
}