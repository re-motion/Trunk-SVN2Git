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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class InheritedClassContextRetrievalAlgorithmTest
  {
    private Dictionary<Type, ClassContext> _contexts;
    private InheritedClassContextRetrievalAlgorithm _algorithm;
    private ClassContext _ccObject;
    private ClassContext _ccString;
    private ClassContext _ccList;
    private ClassContext _ccListInt;

    [SetUp]
    public void SetUp ()
    {
      _contexts = new Dictionary<Type, ClassContext>();
      _algorithm = new InheritedClassContextRetrievalAlgorithm (GetExact, GetWithInheritance);
      _ccObject = new ClassContext (typeof (object), new MixinContext[] {new MixinContext (MixinKind.Extending, typeof (NullMixin), MemberVisibility.Private)}, new Type[] {typeof (NullMixin)});
      _ccString = new ClassContext (typeof (string), new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (NullMixin2), MemberVisibility.Private) }, new Type[] { typeof (NullMixin2) });
      _ccList = new ClassContext (typeof (List<>), new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (NullMixin3), MemberVisibility.Private) }, new Type[] { typeof (NullMixin3) });
      _ccListInt = new ClassContext (typeof (List<int>), new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (NullMixin4), MemberVisibility.Private) }, new Type[] { typeof (NullMixin4) });
    }

    private ClassContext GetExact (Type type)
    {
      if (_contexts.ContainsKey (type))
        return _contexts[type];
      else
        return null;
    }

    private ClassContext GetWithInheritance (Type type)
    {
      return _algorithm.GetWithInheritance (type);
    }

    [Test]
    public void DirectGet ()
    {
      _contexts.Add (_ccObject.Type, _ccObject);
      _contexts.Add (_ccString.Type, _ccString);
      _contexts.Add (_ccList.Type, _ccList);
      _contexts.Add (_ccListInt.Type, _ccListInt);

      Assert.AreSame (_ccObject, _algorithm.GetWithInheritance (typeof (object)));
      Assert.AreSame (_ccString, _algorithm.GetWithInheritance (typeof (string)));
      Assert.AreSame (_ccList, _algorithm.GetWithInheritance (typeof (List<>)));
      Assert.AreSame (_ccListInt, _algorithm.GetWithInheritance (typeof (List<int>)));
    }

    [Test]
    public void Get_FromBase ()
    {
      _contexts.Add (_ccObject.Type, _ccObject);

      ClassContext inherited1 = _algorithm.GetWithInheritance (typeof (int));
      Assert.IsNotNull (inherited1);
      Assert.AreNotSame (_ccObject, inherited1);
      Assert.AreEqual (typeof (int), inherited1.Type);
      Assert.That(inherited1.Mixins, Is.EquivalentTo(_ccObject.Mixins));
      Assert.That (inherited1.CompleteInterfaces, Is.EquivalentTo (_ccObject.CompleteInterfaces));

      ClassContext inherited2 = _algorithm.GetWithInheritance (typeof (string));
      Assert.IsNotNull (inherited2);
      Assert.AreNotSame (_ccObject, inherited2);
      Assert.AreEqual (typeof (string), inherited2.Type);
      Assert.That (inherited2.Mixins, Is.EquivalentTo (_ccObject.Mixins));
      Assert.That(inherited2.CompleteInterfaces, Is.EquivalentTo(_ccObject.CompleteInterfaces));
    }

    [Test]
    public void Get_FromInterface ()
    {
      ClassContext ccInterface = new ClassContext (typeof (IMixedInterface), typeof (MixinExtendingMixedInterface));
      _contexts.Add (ccInterface.Type, ccInterface);

      ClassContext inherited = _algorithm.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsNotNull (inherited);
      Assert.AreEqual (typeof (ClassWithMixedInterface), inherited.Type);
      Assert.That (inherited.Mixins, Is.EquivalentTo (ccInterface.Mixins));
      Assert.That (inherited.CompleteInterfaces, Is.EquivalentTo (ccInterface.CompleteInterfaces));
    }

    [Test]
    public void Get_FromInterfacesAndBase ()
    {
      ClassContext ccInterface = new ClassContext (typeof (IMixedInterface), typeof (MixinExtendingMixedInterface));
      _contexts.Add (ccInterface.Type, ccInterface);
      _contexts.Add (_ccObject.Type, _ccObject);

      ClassContext inherited = _algorithm.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsNotNull (inherited);
      Assert.AreEqual (typeof (ClassWithMixedInterface), inherited.Type);
      Assert.That (inherited.Mixins, Is.EquivalentTo (EnumerableUtility.CombineToArray (ccInterface.Mixins, _ccObject.Mixins)));
      Assert.That (inherited.CompleteInterfaces, Is.EquivalentTo (EnumerableUtility.CombineToArray (ccInterface.CompleteInterfaces, _ccObject.CompleteInterfaces)));
    }

    [Test]
    public void Get_FromTypeDefinition ()
    {
      _contexts.Add (_ccList.Type, _ccList);

      ClassContext inherited1 = _algorithm.GetWithInheritance (typeof (List<int>));
      Assert.IsNotNull (inherited1);
      Assert.AreNotSame (_ccList, inherited1);
      Assert.AreEqual (typeof (List<int>), inherited1.Type);
      Assert.That(inherited1.Mixins, Is.EquivalentTo(_ccList.Mixins));
      Assert.That(inherited1.CompleteInterfaces, Is.EquivalentTo(_ccList.CompleteInterfaces));

      ClassContext inherited2 = _algorithm.GetWithInheritance (typeof (List<string>));
      Assert.IsNotNull (inherited2);
      Assert.AreNotSame (_ccList, inherited2);
      Assert.AreEqual (typeof (List<string>), inherited2.Type);
      Assert.That(inherited2.Mixins, Is.EquivalentTo(_ccList.Mixins));
      Assert.That(inherited2.CompleteInterfaces, Is.EquivalentTo(_ccList.CompleteInterfaces));
    }

    [Test]
    public void Get_FromBaseAndTypeDefinition ()
    {
      _contexts.Add (_ccObject.Type, _ccObject);
      _contexts.Add (_ccList.Type, _ccList);

      ClassContext inherited1 = _algorithm.GetWithInheritance (typeof (List<int>));
      Assert.IsNotNull (inherited1);
      Assert.AreNotSame (_ccList, inherited1);
      Assert.AreNotSame (_ccObject, inherited1);
      Assert.AreEqual (typeof (List<int>), inherited1.Type);
      Assert.That (inherited1.Mixins, Is.EquivalentTo (new List<MixinContext> (EnumerableUtility.Combine (_ccList.Mixins, _ccObject.Mixins))));
      Assert.That (inherited1.CompleteInterfaces,
          Is.EquivalentTo (new List<Type> (EnumerableUtility.Combine (_ccList.CompleteInterfaces, _ccObject.CompleteInterfaces))));
    }
  }
}
