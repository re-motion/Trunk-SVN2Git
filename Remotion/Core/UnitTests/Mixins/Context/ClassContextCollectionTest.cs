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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;
using System.Linq;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ClassContextCollectionTest
  {
    private ClassContext _ccObjectWithMixin;
    private ClassContext _ccString;
    private ClassContextCollection _collectionWithObjectAndString;

    private ClassContext _ccListOfT;
    private ClassContext _ccListOfString;

    private ClassContextCollection _emptyCollection;

    [SetUp]
    public void SetUp ()
    {
      _ccObjectWithMixin = new ClassContext (typeof (object), typeof (NullMixin2));
      _ccString = new ClassContext (typeof (string));
      _collectionWithObjectAndString = new ClassContextCollection (_ccObjectWithMixin, _ccString);

      _ccListOfT = new ClassContext (typeof (List<>));
      _ccListOfString = new ClassContext (typeof (List<string>));

      _emptyCollection = new ClassContextCollection();
    }

    [Test]
    public void Initialization_ParamsArray ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString);
      Assert.That (collection, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString }));
    }

    [Test]
    public void Initialization_Enumerable ()
    {
      var collection = new ClassContextCollection (((IEnumerable<ClassContext>) new[] { _ccObjectWithMixin, _ccString }));
      Assert.That (collection, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString }));
    }

    [Test]
    public void GetExact ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.AreSame (_ccObjectWithMixin, collection.GetExact (typeof (object)));
      Assert.AreSame (_ccString, collection.GetExact (typeof (string)));
      Assert.IsNull (collection.GetExact (typeof (int)));
      Assert.AreSame (_ccListOfT, collection.GetExact (typeof (List<>)));
      Assert.IsNull (collection.GetExact (typeof (List<int>)));
      Assert.AreSame (_ccListOfString, collection.GetExact (typeof (List<string>)));
    }

    [Test]
    public void GetWithInheritance_Simple ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.AreSame (_ccObjectWithMixin, collection.GetWithInheritance (typeof (object)));
      Assert.AreSame (_ccString, collection.GetWithInheritance (typeof (string)));
      Assert.AreSame (_ccListOfT, collection.GetWithInheritance (typeof (List<>)));
      Assert.AreSame (_ccListOfString, collection.GetWithInheritance (typeof (List<string>)));
    }

    [Test]
    public void GetWithInheritance_Null ()
    {
      Assert.IsNull (_emptyCollection.GetWithInheritance (typeof (int)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromBaseType ()
    {
      ClassContext inherited1 = _collectionWithObjectAndString.GetWithInheritance (typeof (ClassContextCollectionTest));
      Assert.IsNotNull (inherited1);
      Assert.AreEqual (typeof (ClassContextCollectionTest), inherited1.Type);
      Assert.IsTrue (inherited1.Mixins.ContainsKey (typeof (NullMixin2)));

      ClassContext inherited2 = _collectionWithObjectAndString.GetWithInheritance (typeof (ClassContextCollectionTest));
      Assert.AreNotSame (inherited1, inherited2);
      Assert.AreEqual (inherited1, inherited2);

      ClassContext inherited3 = _collectionWithObjectAndString.GetWithInheritance (typeof (int));
      Assert.IsNotNull (inherited3);
      Assert.AreEqual (typeof (int), inherited3.Type);
      Assert.IsTrue (inherited3.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromInterface ()
    {
      var classContext = new ClassContext (typeof (IMixedInterface), typeof (NullMixin), typeof (NullMixin2));
      var collection = new ClassContextCollection (classContext);

      ClassContext inherited = collection.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsNotNull (inherited);
      Assert.AreEqual (typeof (ClassWithMixedInterface), inherited.Type);
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromInterfaceAndBase ()
    {
      var classContext1 = new ClassContext (typeof (IMixedInterface), typeof (NullMixin), typeof (NullMixin2));
      var classContext2 = new ClassContext (typeof (object), typeof (NullMixin3));

      var collection = new ClassContextCollection (classContext1, classContext2);

      ClassContext inherited = collection.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsNotNull (inherited);
      Assert.AreEqual (typeof (ClassWithMixedInterface), inherited.Type);
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin3)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromGenericTypeDefinition ()
    {
      var classContext1 = new ClassContext (typeof (List<>), typeof (NullMixin3));
      var classContext2 = new ClassContext (typeof (List<string>), typeof (NullMixin4));

      var collection = new ClassContextCollection (classContext1, classContext2);

      ClassContext inherited4 = collection.GetWithInheritance (typeof (List<int>));
      Assert.IsNotNull (inherited4);
      Assert.AreEqual (typeof (List<int>), inherited4.Type);
      Assert.IsTrue (inherited4.Mixins.ContainsKey (typeof (NullMixin3)));
      Assert.IsFalse (inherited4.Mixins.ContainsKey (typeof (NullMixin4)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromGenericTypeDefinitionAndBase ()
    {
      var classContext1 = new ClassContext (typeof (List<>), typeof (NullMixin3));
      var classContext2 = new ClassContext (typeof (List<string>), typeof (NullMixin4));
      var classContext3 = new ClassContext (typeof (object), typeof (NullMixin2));

      var collection = new ClassContextCollection (classContext1, classContext2, classContext3);

      ClassContext inherited4 = collection.GetWithInheritance (typeof (List<int>));
      Assert.IsNotNull (inherited4);
      Assert.AreEqual (typeof (List<int>), inherited4.Type);
      Assert.IsTrue (inherited4.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (inherited4.Mixins.ContainsKey (typeof (NullMixin3)));
    }

    [Test]
    public void ContainsExact ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.IsTrue (collection.ContainsExact (typeof (object)));
      Assert.IsTrue (collection.ContainsExact (typeof (string)));
      Assert.IsFalse (collection.ContainsExact (typeof (int)));
      Assert.IsTrue (collection.ContainsExact (typeof (List<>)));
      Assert.IsFalse (collection.ContainsExact (typeof (List<int>)));
      Assert.IsTrue (collection.ContainsExact (typeof (List<string>)));
    }

    [Test]
    public void ContainsWithInheritance ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.IsTrue (collection.ContainsWithInheritance (typeof (object)));
      Assert.IsTrue (collection.ContainsWithInheritance (typeof (string)));
      Assert.IsTrue (collection.ContainsWithInheritance (typeof (ClassContextCollectionTest)));
      Assert.IsTrue (collection.ContainsWithInheritance (typeof (int)));
      Assert.IsTrue (collection.ContainsWithInheritance (typeof (List<>)));
      Assert.IsTrue (collection.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (collection.ContainsWithInheritance (typeof (List<string>)));

      Assert.IsFalse (_emptyCollection.ContainsWithInheritance (typeof (int)));
    }

    [Test]
    public void GetEnumerator ()
    {
      var enumerable = (IEnumerable<ClassContext>) _collectionWithObjectAndString;
      Assert.That (enumerable.ToArray(), Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString }));
    }

    [Test]
    public void NonGeneric_GetEnumerator ()
    {
      var enumerable = (IEnumerable) _collectionWithObjectAndString;
      var enumerator = enumerable.GetEnumerator ();
      Assert.That (enumerator.MoveNext(), Is.True);
    }

    [Test]
    public void CopyTo ()
    {
      var classContexts = new ClassContext[5];
      _collectionWithObjectAndString.CopyTo (classContexts, 2);
      Assert.That (classContexts, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString, null }));
      Assert.That (classContexts[0], Is.Null);
      Assert.That (classContexts[1], Is.Null);
      Assert.That (classContexts[4], Is.Null);
    }

    [Test]
    public void NonGeneric_CopyTo ()
    {
      var classContexts = new object[5];
      ((ICollection) _collectionWithObjectAndString).CopyTo (classContexts, 1);
      Assert.That (classContexts, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString, null }));
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.IsNotNull (((ICollection) _collectionWithObjectAndString).SyncRoot);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.IsFalse (((ICollection) _collectionWithObjectAndString).IsSynchronized);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.IsTrue (((ICollection<ClassContext>) _collectionWithObjectAndString).IsReadOnly);
    }

    [Test]
    public void Contains ()
    {
      var cc3 = new ClassContext (typeof (int));
      var cc4 = new ClassContext (typeof (object), typeof (NullMixin));
      
      Assert.IsTrue (_collectionWithObjectAndString.Contains (_ccObjectWithMixin));
      Assert.IsTrue (_collectionWithObjectAndString.Contains (_ccString));
      Assert.IsFalse (_collectionWithObjectAndString.Contains (cc3));
      Assert.IsFalse (_collectionWithObjectAndString.Contains (cc4));
    }
  }
}