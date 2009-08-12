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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class ConcreteMixinTypeAttributeUtilityTest
  {
    [Test]
    public void CreateAttributeBuilder ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .AddMixin (typeof (string)).WithDependency (typeof (bool)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      var identifier = new ConcreteMixinTypeIdentifier (typeof (double), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      CustomAttributeBuilder builder = ConcreteMixinTypeAttributeUtility.CreateAttributeBuilder (context, 12, identifier);

      TypeBuilder typeBuilder = ((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope.ObtainDynamicModuleWithWeakName ().DefineType ("Test_ConcreteMixinTypeAttribute");
      typeBuilder.SetCustomAttribute (builder);
      Type type = typeBuilder.CreateType ();

      Console.WriteLine (SeparatedStringBuilder.Build (",", ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ()));
      
      var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixinTypeAttribute> (type, false);
      
      var contextDeserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
      var regeneratedContext = ClassContext.Deserialize (contextDeserializer);
      var regeneratedIdentifier = attribute.GetIdentifier ();

      Assert.That (regeneratedContext, Is.EqualTo (context));
      Assert.That (attribute.MixinIndex, Is.EqualTo (12));
      Assert.That (regeneratedIdentifier, Is.EqualTo (identifier));
    }
  }
}
