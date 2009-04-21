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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class EventEmitterTest : CodeGenerationBaseTest
  {
    private CustomClassEmitter _classEmitter;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = new CustomClassEmitter (Scope, UniqueName, typeof (object));
    }

    public override void TearDown ()
    {
      if (!_classEmitter.HasBeenBuilt)
        _classEmitter.BuildType();

      base.TearDown();
    }

    private object BuildInstance ()
    {
      return Activator.CreateInstance (_classEmitter.BuildType ());
    }

    private void AddEventMethod (object instance, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent(instance, eventEmitter).GetAddMethod(true).Invoke (instance, new[] {method});
    }

    private void AddEventMethod (Type type, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent (type, eventEmitter).GetAddMethod (true).Invoke (null, new object[] { method });
    }

    private void RemoveEventMethod (object instance, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent (instance, eventEmitter).GetRemoveMethod (true).Invoke (instance, new object[] { method });
    }

    private void RemoveEventMethod (Type type, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent (type, eventEmitter).GetRemoveMethod (true).Invoke (null, new object[] { method });
    }

    private EventInfo GetEvent (Type builtType, CustomEventEmitter eventEmitter)
    {
      return builtType.GetEvent (eventEmitter.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private EventInfo GetEvent (object instance, CustomEventEmitter eventEmitter)
    {
      return GetEvent (instance.GetType (), eventEmitter);
    }

    private void ImplementEventAddMethod (CustomEventEmitter eventEmitter)
    {
      FieldReference field;
      if (eventEmitter.EventKind == EventKind.Static)
        field = _classEmitter.CreateStaticField ("AddCalled", typeof (bool));
      else
        field = _classEmitter.CreateField ("AddCalled", typeof (bool));

      eventEmitter.AddMethod.AddStatement (new AssignStatement (field, new ConstReference (true).ToExpression ()));
      eventEmitter.AddMethod.ImplementByReturningVoid ();
    }

    private void ImplementEventRemoveMethod (CustomEventEmitter eventEmitter)
    {
      FieldReference field;
      if (eventEmitter.EventKind == EventKind.Static)
        field = _classEmitter.CreateStaticField ("RemoveCalled", typeof (bool));
      else
        field = _classEmitter.CreateField ("RemoveCalled", typeof (bool));

      eventEmitter.RemoveMethod.AddStatement (new AssignStatement (field, new ConstReference (true).ToExpression ()));
      eventEmitter.RemoveMethod.ImplementByReturningVoid ();
    }

    private bool AddCalled (object instance)
    {
      FieldInfo field = instance.GetType().GetField ("AddCalled");
      return (bool) field.GetValue (instance);
    }

    private bool AddCalled (Type type)
    {
      FieldInfo field = type.GetField ("AddCalled");
      return (bool) field.GetValue (null);
    }

    private bool RemoveCalled (object instance)
    {
      FieldInfo field = instance.GetType ().GetField ("RemoveCalled");
      return (bool) field.GetValue (instance);
    }

    private bool RemoveCalled (Type type)
    {
      FieldInfo field = type.GetField ("RemoveCalled");
      return (bool) field.GetValue (null);
    }

    [Test]
    public void SimpleEvent ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("SimpleEvent", EventKind.Instance, typeof (EventHandler));

      Assert.AreEqual ("SimpleEvent", eventEmitter.Name);
      Assert.AreEqual (typeof (EventHandler), eventEmitter.EventType);
      Assert.AreEqual (EventKind.Instance, eventEmitter.EventKind);

      ImplementEventAddMethod (eventEmitter);
      ImplementEventRemoveMethod (eventEmitter);

      object instance = BuildInstance ();
      Assert.IsFalse (AddCalled (instance));
      Assert.IsFalse (RemoveCalled (instance));

      AddEventMethod (instance, eventEmitter, (EventHandler) delegate { });
      
      Assert.IsTrue (AddCalled (instance));
      Assert.IsFalse (RemoveCalled (instance));
      
      RemoveEventMethod (instance, eventEmitter, (EventHandler) delegate { });

      Assert.IsTrue (AddCalled (instance));
      Assert.IsTrue (RemoveCalled (instance));
    }

    [Test]
    public void StaticEvent ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("StaticEvent", EventKind.Static, typeof (Func<string>));

      Assert.AreEqual ("StaticEvent", eventEmitter.Name);
      Assert.AreEqual (typeof (Func<string>), eventEmitter.EventType);
      Assert.AreEqual (EventKind.Static, eventEmitter.EventKind);

      ImplementEventAddMethod (eventEmitter);
      ImplementEventRemoveMethod (eventEmitter);

      Type type = _classEmitter.BuildType ();

      Assert.IsFalse (AddCalled (type));
      Assert.IsFalse (RemoveCalled (type));

      AddEventMethod (type, eventEmitter, (Func<string>) delegate { return null; });

      Assert.IsTrue (AddCalled (type));
      Assert.IsFalse (RemoveCalled (type));

      RemoveEventMethod (type, eventEmitter, (Func<string>) delegate { return null; });

      Assert.IsTrue (AddCalled (type));
      Assert.IsTrue (RemoveCalled (type));
    }

    [Test]
    public void DefaultAddMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("DefaultAddMethod", EventKind.Static, typeof (EventHandler));
      Assert.IsNotNull (eventEmitter.AddMethod);
      Type type = _classEmitter.BuildType ();
      Assert.IsNotNull (GetEvent (type, eventEmitter).GetAddMethod ());
    }

    [Test]
    public void DefaultRemoveMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("DefaultRemoveMethod", EventKind.Static, typeof (EventHandler));
      Assert.IsNotNull (eventEmitter.RemoveMethod);
      Type type = _classEmitter.BuildType ();
      Assert.IsNotNull (GetEvent (type, eventEmitter).GetRemoveMethod ());
    }

    [Test]
    public void CustomAddMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("CustomAddMethod", EventKind.Static, typeof (EventHandler));
      eventEmitter.AddMethod = _classEmitter.CreateMethod ("CustomAdd", MethodAttributes.Public | MethodAttributes.Static)
        .SetParameterTypes (typeof (EventHandler));
    }

    [Test]
    public void CustomRemoveMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("CustomRemoveMethod", EventKind.Static, typeof (EventHandler));
      eventEmitter.RemoveMethod = _classEmitter.CreateMethod ("CustomRemove", MethodAttributes.Public | MethodAttributes.Static)
        .SetParameterTypes (typeof (EventHandler));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Add methods can only be assigned once.")]
    public void AddMethodCannotBeSetTwice ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddMethodCannotBeSetTwice", EventKind.Static, typeof (EventHandler));
      CustomMethodEmitter defaultAdd = eventEmitter.AddMethod;
      eventEmitter.AddMethod = _classEmitter.CreateMethod ("invalid", MethodAttributes.Public | MethodAttributes.Static);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Remove methods can only be assigned once.")]
    public void RemoveMethodCannotBeSetTwice ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddMethodCannotBeSetTwice", EventKind.Static, typeof (EventHandler));
      CustomMethodEmitter defaultRemove = eventEmitter.RemoveMethod;
      eventEmitter.RemoveMethod = _classEmitter.CreateMethod ("invalid", MethodAttributes.Public | MethodAttributes.Static);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
        ExpectedMessage = "Event accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void AddMethodCannotBeSetToNull()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddMethodCannotBeSetToNull", EventKind.Static, typeof (string));
      eventEmitter.AddMethod = null;
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
       ExpectedMessage = "Event accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void RemoveMethodCannotBeSetToNull ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("RemoveMethodCannotBeSetToNull", EventKind.Static, typeof (string));
      eventEmitter.RemoveMethod = null;
    }

    [Test]
    public void AddCustomAttribute ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddCustomAttribute", EventKind.Static, typeof (string));
      eventEmitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      Type type = _classEmitter.BuildType ();
      Assert.IsTrue (GetEvent (type, eventEmitter).IsDefined (typeof (SimpleAttribute), false));
      Assert.AreEqual (1, GetEvent (type, eventEmitter).GetCustomAttributes (false).Length);
      Assert.AreEqual (new SimpleAttribute(), GetEvent (type, eventEmitter).GetCustomAttributes (false)[0]);
    }
  }
}
