// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Text;

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Ordering
{
  public class OrderingTestBase
  {
    protected static void CheckOrderedMixinTypes (object instance, params Type[] expected)
    {
      var mixinTypes = MixinTypeUtility.GetMixinTypesExact (instance.GetType());
      Assert.That (mixinTypes, Is.EqualTo (expected));
    }

    protected T BuildMixedInstance<T> (params Type[] mixins)
    {
      using (MixinConfiguration.BuildNew ().ForClass<T> ().AddMixins (mixins).EnterScope ())
      {
        return ObjectFactory.Create<T>();
      }
    }

    protected T BuildMixedInstance<T> (Action<ClassContextBuilder> configuration, params Type[] mixins)
    {
      var classContextBuilder = MixinConfiguration.BuildNew().ForClass<T>().AddMixins (mixins);
      configuration (classContextBuilder);
      using (classContextBuilder.EnterScope ())
      {
        return ObjectFactory.Create<T> ();
      }
    }

    protected T BuildMixedInstanceWithDeclarativeConfiguration<T> (params Type[] additionalAnalyzedTypes)
    {
      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromTypes (null, additionalAnalyzedTypes.Concat (typeof (OrderingViaAttributeDependencyTest.C)));
      using (mixinConfiguration.EnterScope ())
      {
        return ObjectFactory.Create<T>();
      }
    }

    protected void CheckOrderingException (ActualValueDelegate action, Type targetClass, params Type[] conflictingMixins)
    {
      var expectedMessage = string.Format (
          "The mixins applied to target class '{0}' cannot be ordered. The following mixins require a clear base call ordering, but do not provide "
          + "enough dependency information:{2}{1}.{2}"
          + "Please supply additional dependencies to the mixin definitions, use the AcceptsAlphabeticOrderingAttribute, or adjust the "
          + "mixin configuration accordingly.", 
          targetClass.FullName, 
          BuildMixinListForExceptionMessage (conflictingMixins),
          Environment.NewLine);
      Assert.That (action, Throws.TypeOf<ConfigurationException>().With.Message.EqualTo (expectedMessage));
    }

    protected void CheckCycleException (ActualValueDelegate action, Type targetClass, params Type[] mixinTypes)
    {
      var expectedMessage = string.Format (
          "The mixins applied to target class '{0}' cannot be ordered. The following group of mixins contains circular dependencies:{2}{1}.", 
          targetClass.FullName,
          BuildMixinListForExceptionMessage (mixinTypes),
          Environment.NewLine);
      Assert.That (action, Throws.TypeOf<ConfigurationException> ().With.Message.EqualTo (expectedMessage));
    }

    private static string BuildMixinListForExceptionMessage (IEnumerable<Type> mixinTypes)
    {
      return SeparatedStringBuilder.Build ("," + Environment.NewLine, mixinTypes, m => "'" + m.FullName + "'");
    }
  }
}