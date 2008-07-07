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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.ClassContextTests
{
  [TestFixture]
  public class ClassContextGeneralTest
  {
    [Test]
    public void GetMixinContext ()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType7));

      Assert.IsFalse (classContext.Mixins.ContainsKey (typeof (BT7Mixin1)));
      MixinContext mixinContext = classContext.Mixins[typeof (BT7Mixin1)];
      Assert.IsNull (mixinContext);

      classContext = new ClassContext (typeof (BaseType7), typeof (BT7Mixin1));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT7Mixin1)));
      mixinContext = classContext.Mixins[typeof (BT7Mixin1)];
      Assert.AreSame (mixinContext, classContext.Mixins[typeof (BT7Mixin1)]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Object was tried to be added twice", MatchType = MessageMatch.Contains)]
    public void ConstructorThrowsOnDuplicateMixinContexts ()
    {
      new ClassContext (typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void MixinsOnInterface ()
    {
      MixinConfiguration context = MixinConfiguration.BuildFromActive()
          .ForClass <IBaseType2>().AddMixin<BT2Mixin1>().BuildConfiguration();

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (IBaseType2));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT2Mixin1)));
    }

    [Test]
    public void ConstructorWithMixinParameters()
    {
      ClassContext context = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2));
      Assert.AreEqual (2, context.Mixins.Count);
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (context.Mixins.ContainsKey (typeof (BT1Mixin2)));
      Assert.IsFalse (context.Mixins.ContainsKey (typeof (BT2Mixin1)));
    }

    [Test]
    [Ignore ("TODO")]
    public void ConstructorWithMixinParameters_DefaultValues ()
    {
      
    }

    [Test]
    public void CompleteInterfaces_Empty()
    {
      ClassContext context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[0]);
      Assert.AreEqual (0, context.CompleteInterfaces.Count);
      Assert.IsEmpty (context.CompleteInterfaces);
      Assert.IsFalse (context.CompleteInterfaces.ContainsKey (typeof (IBT5MixinC1)));
    }

    [Test]
    public void CompleteInterfaces_NonEmpty ()
    {
      ClassContext context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[] { typeof (IBT5MixinC1) });
      Assert.AreEqual (1, context.CompleteInterfaces.Count);
      Assert.Contains (typeof (IBT5MixinC1), context.CompleteInterfaces);
      Assert.IsTrue (context.CompleteInterfaces.ContainsKey (typeof (IBT5MixinC1)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin type System.Object was tried to be added twice.\r\n"
        + "Parameter name: mixinTypes")]
    public void ConstructorThrowsOnDuplicateMixinContextsInAdd ()
    {
      new ClassContext (typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void DuplicateCompleteInterfacesAreIgnored ()
    {
      ClassContext context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC1) });
      Assert.AreEqual (1, context.CompleteInterfaces.Count);
    }

    [Test]
    public void ClassContextHasValueEquality ()
    {
      ClassContext cc1 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext();

      ClassContext cc2 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext ();

      Assert.AreEqual (cc1, cc2);
      Assert.AreEqual (cc1.GetHashCode (), cc2.GetHashCode ());

      ClassContext cc3 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext();

      Assert.AreNotEqual (cc1, cc3);

      ClassContext cc4 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext();

      Assert.AreEqual (cc4, cc3);
      Assert.AreEqual (cc4.GetHashCode (), cc3.GetHashCode ());

      ClassContext cc5 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin2)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext ();

      Assert.AreNotEqual (cc4, cc5);

      ClassContext cc6 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC2))
          .BuildClassContext();

      Assert.AreNotEqual (cc4, cc6);

      ClassContext cc7 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1))
          .BuildClassContext ();

      ClassContext cc8 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType2))
          .BuildClassContext ();
      
      Assert.AreEqual (cc7, cc7);
      Assert.AreNotEqual (cc7, cc8);
    }

    [Test]
    public void ClassContextIsSerializable ()
    {
      ClassContext cc = new ClassContextBuilder (typeof (BaseType1))
          .AddCompleteInterface (typeof (IBT5MixinC1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType2))
          .BuildClassContext ();

      ClassContext cc2 = Serializer.SerializeAndDeserialize (cc);
      Assert.AreNotSame (cc2, cc);
      Assert.AreEqual (cc2, cc);
      Assert.IsTrue (cc2.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (cc2.Mixins[typeof (BT1Mixin1)].ExplicitDependencies.ContainsKey (typeof (IBaseType2)));
    }

    [Test]
    public void SpecializeWithTypeArguments ()
    {
      ClassContext original = new ClassContextBuilder (typeof (List<>)).AddMixin<BT1Mixin1>().WithDependency<IBaseType2>().BuildClassContext();

      ClassContext specialized = original.SpecializeWithTypeArguments (new Type[] { typeof (int) });
      Assert.IsNotNull (specialized);
      Assert.AreEqual (typeof (List<int>), specialized.Type);
      Assert.IsTrue (specialized.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (specialized.Mixins[typeof (BT1Mixin1)].ExplicitDependencies.ContainsKey (typeof (IBaseType2)));
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      ClassContext context = new ClassContext (typeof (List<int>));
      Assert.AreEqual (typeof (List<int>), context.Type);
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      ClassContext context = new ClassContext (typeof (object), typeof (IList<int>));

      Assert.IsTrue (context.Mixins.ContainsKey (typeof (IList<int>)));
      Assert.IsTrue (context.Mixins.ContainsAssignableMixin (typeof (IList<int>)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (ICollection<int>)));
      Assert.IsTrue (context.Mixins.ContainsAssignableMixin (typeof (ICollection<int>)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (object)));
      Assert.IsTrue (context.Mixins.ContainsAssignableMixin (typeof (object)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (List<int>)));
      Assert.IsFalse (context.Mixins.ContainsAssignableMixin (typeof (List<int>)));

      Assert.IsFalse (context.Mixins.ContainsKey (typeof (IList<>)));
      Assert.IsFalse (context.Mixins.ContainsAssignableMixin (typeof (List<>)));
    }

    [Test]
    public void CloneForSpecificType ()
    {
      MixinContext[] mixins = new MixinContext[] {
          new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, new Type[] { typeof (IBT1Mixin1) }),
          new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private)
      };
      Type[] interfaces = new Type[] { typeof (ICBT6Mixin1), typeof (ICBT6Mixin2)};
      ClassContext source = new ClassContext (typeof (BaseType1), mixins, interfaces);
      ClassContext clone = source.CloneForSpecificType (typeof (BaseType2));
      Assert.AreNotEqual (source, clone);
      Assert.That(clone.Mixins, Is.EquivalentTo(mixins));
      Assert.That (clone.CompleteInterfaces, Is.EquivalentTo (interfaces));
      Assert.AreEqual (typeof (BaseType2), clone.Type);
      Assert.AreEqual (typeof (BaseType1), source.Type);
    }

    [Test]
    public void Equals_True ()
    {
      ClassContext c1 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] {new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private)},
              new Type[] {typeof (IBT5MixinC1), typeof (IBT5MixinC2)});
      ClassContext c2 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c3 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c4 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] {new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private)},
              new Type[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });

      Assert.AreEqual (c1, c2);
      Assert.AreEqual (c1, c3);
      Assert.AreEqual (c1, c4);

      ClassContext c5 = new ClassContext (typeof (BaseType1));
      ClassContext c6 = new ClassContext (typeof (BaseType1));
      Assert.AreEqual (c5, c6);
    }

    [Test]
    public void Equals_False ()
    {
      ClassContext c1 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c2 =
          new ClassContext (
              typeof (BaseType2),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c3 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1) });
      ClassContext c4 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });
      ClassContext c5 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, typeof (BT1Mixin2)), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });

      Assert.AreNotEqual (c1, c2);
      Assert.AreNotEqual (c1, c3);
      Assert.AreNotEqual (c1, c4);
      Assert.AreNotEqual (c1, c5);
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      ClassContext c1 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c2 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c3 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC2) });
      ClassContext c4 =
          new ClassContext (
              typeof (BaseType1),
              new MixinContext[] { new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private), new MixinContext (MixinKind.Extending, typeof (BT1Mixin2), MemberVisibility.Private) },
              new Type[] { typeof (IBT5MixinC2), typeof (IBT5MixinC1) });

      Assert.AreEqual (c1.GetHashCode (), c2.GetHashCode ());
      Assert.AreEqual (c1.GetHashCode (), c3.GetHashCode ());
      Assert.AreEqual (c1.GetHashCode (), c4.GetHashCode ());

      ClassContext c5 = new ClassContext (typeof (BaseType1));
      ClassContext c6 = new ClassContext (typeof (BaseType1));
      Assert.AreEqual (c5.GetHashCode (), c6.GetHashCode ());
    }
  }
}
