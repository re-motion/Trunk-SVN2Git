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
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Context
{
  [TestFixture]
  public class ClassContextCollectionTest
  {
    private ClassContext _cc1;
    private ClassContext _cc2;
    private ClassContextCollection _collection;

    [SetUp]
    public void SetUp ()
    {
      _cc1 = new ClassContext (typeof (object), typeof (NullMixin2));
      _cc2 = new ClassContext (typeof (string));
      _collection = new ClassContextCollection ();

      _collection.Add (_cc1);
      _collection.Add (_cc2);
    }

    [Test]
    public void Add ()
    {
      Assert.That (_collection, Is.EqualTo (new ClassContext[] {_cc1, _cc2}));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A class context for type System.Object was already added.")]
    public void Add_DuplicateKey ()
    {
      _collection.Add (_cc1);
    }

    [Test]
    public void AddedEvent ()
    {
      _collection.Clear();

      ClassContext addedContext = null;
      _collection.ClassContextAdded += delegate (object sender, ClassContextEventArgs args)
      {
        Assert.AreSame (_collection, sender);
        addedContext = args.ClassContext;
      };

      _collection.Add (_cc1);
      Assert.AreSame (_cc1, addedContext);

      _collection.AddOrReplace (_cc2);
      Assert.AreSame (_cc2, addedContext);

      _collection.AddOrReplace (_cc1);
      Assert.AreSame (_cc1, addedContext);
    }

    [Test]
    public void RemoveExact ()
    {
      Assert.IsTrue (_collection.RemoveExact (typeof (object)));
      Assert.That (_collection, Is.EqualTo (new ClassContext[] { _cc2 }));

      Assert.IsFalse (_collection.RemoveExact (typeof (object)));
      Assert.That (_collection, Is.EqualTo (new ClassContext[] { _cc2 }));

      Assert.IsTrue (_collection.RemoveExact (typeof (string)));
      Assert.That (_collection, Is.Empty);
    }

    [Test]
    public void Remove ()
    {
      ClassContext cc3 = new ClassContext (typeof (string), typeof (NullMixin));
      ICollection<ClassContext> collectionAsICollection = _collection;

      Assert.IsTrue (collectionAsICollection.Remove (_cc1));
      Assert.That (_collection, Is.EqualTo (new ClassContext[] { _cc2 }));

      Assert.IsFalse (collectionAsICollection.Remove (_cc1));
      Assert.That (_collection, Is.EqualTo (new ClassContext[] { _cc2 }));

      Assert.IsFalse (collectionAsICollection.Remove (cc3));
      Assert.That (_collection, Is.EqualTo (new ClassContext[] { _cc2 }));

      Assert.IsTrue (collectionAsICollection.Remove (_cc2));
      Assert.That (_collection, Is.Empty);
    }

    [Test]
    public void RemovedEvent ()
    {
      ClassContext removedContext = null;
      _collection.ClassContextRemoved += delegate (object sender, ClassContextEventArgs args)
      {
        Assert.AreSame (_collection, sender);
        removedContext = args.ClassContext;
      };

      _collection.RemoveExact (_cc1.Type);
      Assert.AreSame (_cc1, removedContext);

      ClassContext cc2Equivalent = _cc2.CloneForSpecificType (_cc2.Type);
      ((ICollection<ClassContext>)_collection).Remove (cc2Equivalent);
      Assert.AreNotSame (cc2Equivalent, removedContext);
      Assert.AreSame (_cc2, removedContext);

      _collection.Add (_cc1);
      _collection.Clear();

      Assert.AreSame (_cc1, removedContext);
    }

    [Test]
    public void AddOrReplace_Replace ()
    {
      ClassContext cc3 = new ClassContext (typeof (object));
      _collection.AddOrReplace (cc3);
      Assert.AreSame (cc3, _collection.GetExact (typeof (object)));
      Assert.That (_collection, Is.EquivalentTo(new ClassContext[] { _cc2, cc3 }));
    }

    [Test]
    public void AddOrReplace_Add ()
    {
      ClassContext cc3 = new ClassContext (typeof (ClassContextCollectionTest));
      _collection.AddOrReplace (cc3);
      Assert.AreSame (cc3, _collection.GetExact (typeof (ClassContextCollectionTest)));
      Assert.AreSame (_cc1, _collection.GetExact (typeof (object)));
      Assert.That (_collection, Is.EqualTo (new ClassContext[] { _cc1, _cc2, cc3 }));
    }

    [Test]
    public void GetExact ()
    {
      ClassContext cc3 = new ClassContext (typeof (List<>));
      _collection.Add (cc3);
      ClassContext cc4 = new ClassContext (typeof (List<string>));
      _collection.Add (cc4);

      Assert.AreSame (_cc1, _collection.GetExact (typeof (object)));
      Assert.AreSame (_cc2, _collection.GetExact (typeof (string)));
      Assert.IsNull (_collection.GetExact (typeof (int)));
      Assert.AreSame (cc3, _collection.GetExact (typeof (List<>)));
      Assert.IsNull (_collection.GetExact (typeof (List<int>)));
      Assert.AreSame (cc4, _collection.GetExact (typeof (List<string>)));
    }

    [Test]
    public void GetWithInheritance_Simple ()
    {
      ClassContext cc3 = new ClassContext (typeof (List<>));
      _collection.Add (cc3);
      ClassContext cc4 = new ClassContext (typeof (List<string>));
      _collection.Add (cc4);

      Assert.AreSame (_cc1, _collection.GetWithInheritance (typeof (object)));
      Assert.AreSame (_cc2, _collection.GetWithInheritance (typeof (string)));
      Assert.AreSame (cc3, _collection.GetWithInheritance (typeof (List<>)));
      Assert.AreSame (cc4, _collection.GetWithInheritance (typeof (List<string>)));
    }

    [Test]
    public void GetWithInheritance_Null ()
    {
      _collection.Clear ();

      Assert.IsNull (_collection.GetWithInheritance (typeof (int)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromBaseType ()
    {
      ClassContext inherited1 = _collection.GetWithInheritance (typeof (ClassContextCollectionTest));
      Assert.IsNotNull (inherited1);
      Assert.AreEqual (typeof (ClassContextCollectionTest), inherited1.Type);
      Assert.IsTrue (inherited1.Mixins.ContainsKey (typeof (NullMixin2)));

      ClassContext inherited2 = _collection.GetWithInheritance (typeof (ClassContextCollectionTest));
      Assert.AreNotSame (inherited1, inherited2);
      Assert.AreEqual (inherited1, inherited2);

      ClassContext inherited3 = _collection.GetWithInheritance (typeof (int));
      Assert.IsNotNull (inherited3);
      Assert.AreEqual (typeof (int), inherited3.Type);
      Assert.IsTrue (inherited3.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromInterface ()
    {
      _collection.Clear ();
      ClassContext classContext = new ClassContext (typeof (IMixedInterface), typeof (NullMixin), typeof (NullMixin2));
      _collection.Add (classContext);

      ClassContext inherited = _collection.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsNotNull (inherited);
      Assert.AreEqual (typeof (ClassWithMixedInterface), inherited.Type);
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin2)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromInterfaceAndBase ()
    {
      _collection.Clear();
      ClassContext classContext1 = new ClassContext (typeof (IMixedInterface), typeof (NullMixin), typeof (NullMixin2));
      ClassContext classContext2 = new ClassContext (typeof (object), typeof (NullMixin3));
      _collection.Add (classContext1);
      _collection.Add (classContext2);

      ClassContext inherited = _collection.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.IsNotNull (inherited);
      Assert.AreEqual (typeof (ClassWithMixedInterface), inherited.Type);
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (inherited.Mixins.ContainsKey (typeof (NullMixin3)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromGenericTypeDefinition ()
    {
      _collection.Clear();

      ClassContext cc3 = new ClassContext (typeof (List<>), typeof (NullMixin3));
      _collection.Add (cc3);
      ClassContext cc4 = new ClassContext (typeof (List<string>), typeof (NullMixin4));
      _collection.Add (cc4);

      ClassContext inherited4 = _collection.GetWithInheritance (typeof (List<int>));
      Assert.IsNotNull (inherited4);
      Assert.AreEqual (typeof (List<int>), inherited4.Type);
      Assert.IsTrue (inherited4.Mixins.ContainsKey (typeof (NullMixin3)));
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromGenericTypeDefinitionAndBase ()
    {
      ClassContext cc3 = new ClassContext (typeof (List<>), typeof (NullMixin3));
      _collection.Add (cc3);
      ClassContext cc4 = new ClassContext (typeof (List<string>), typeof (NullMixin4));
      _collection.Add (cc4);

      ClassContext inherited4 = _collection.GetWithInheritance (typeof (List<int>));
      Assert.IsNotNull (inherited4);
      Assert.AreEqual (typeof (List<int>), inherited4.Type);
      Assert.IsTrue (inherited4.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.IsTrue (inherited4.Mixins.ContainsKey (typeof (NullMixin3)));
    }

    [Test]
    public void ContainsExact ()
    {
      ClassContext cc3 = new ClassContext (typeof (List<>));
      _collection.Add (cc3);
      ClassContext cc4 = new ClassContext (typeof (List<string>));
      _collection.Add (cc4);

      Assert.IsTrue (_collection.ContainsExact (typeof (object)));
      Assert.IsTrue (_collection.ContainsExact (typeof (string)));
      Assert.IsFalse (_collection.ContainsExact (typeof (int)));
      Assert.IsTrue (_collection.ContainsExact (typeof (List<>)));
      Assert.IsFalse( _collection.ContainsExact (typeof (List<int>)));
      Assert.IsTrue (_collection.ContainsExact (typeof (List<string>)));
    }

    [Test]
    public void ContainsWithInheritance ()
    {
      ClassContext cc3 = new ClassContext (typeof (List<>));
      _collection.Add (cc3);
      ClassContext cc4 = new ClassContext (typeof (List<string>));
      _collection.Add (cc4);

      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (object)));
      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (string)));
      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (ClassContextCollectionTest)));
      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (int)));
      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (List<>)));
      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (List<int>)));
      Assert.IsTrue (_collection.ContainsWithInheritance (typeof (List<string>)));

      _collection.Clear ();

      Assert.IsFalse (_collection.ContainsWithInheritance (typeof (int)));
    }

    [Test]
    public void GetEnumerator ()
    {
      List<ClassContext> classContexts = new List<ClassContext> (_collection);
      Assert.That (classContexts, Is.EqualTo (new ClassContext[] { _cc1, _cc2 }));
    }

    [Test]
    public void NonGeneric_GetEnumerator ()
    {
      IEnumerable collectionAsIEnumerable = _collection;
      List<ClassContext> classContexts = new List<ClassContext> (EnumerableUtility.Cast<ClassContext> (collectionAsIEnumerable));
      Assert.That (classContexts, Is.EqualTo (new ClassContext[] { _cc1, _cc2 }));
    }

    [Test]
    public void CopyTo ()
    {
      ClassContext[] classContexts = new ClassContext[5];
      _collection.CopyTo (classContexts, 2);
      Assert.That (classContexts, Is.EqualTo (new ClassContext[] { null, null, _cc1, _cc2, null }));
    }

    [Test]
    public void NonGeneric_CopyTo ()
    {
      object[] classContexts = new object[5];
      ((ICollection) _collection).CopyTo (classContexts, 1);
      Assert.That (classContexts, Is.EqualTo (new ClassContext[] { null, _cc1, _cc2, null, null }));
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.IsNotNull (((ICollection) _collection).SyncRoot);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.IsFalse (((ICollection) _collection).IsSynchronized);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.IsFalse (((ICollection<ClassContext>) _collection).IsReadOnly);
    }

    [Test]
    public void Contains ()
    {
      ClassContext cc3 = new ClassContext (typeof (int));
      ClassContext cc4 = new ClassContext (typeof (object), typeof (NullMixin));
      Assert.IsTrue (_collection.Contains (_cc1));
      Assert.IsTrue (_collection.Contains (_cc2));
      Assert.IsFalse (_collection.Contains (cc3));
      Assert.IsFalse (_collection.Contains (cc4));
    }
  }
}
