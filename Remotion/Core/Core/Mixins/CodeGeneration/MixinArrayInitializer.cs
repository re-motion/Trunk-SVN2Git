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
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  public class MixinArrayInitializer
  {
    public class ExpectedMixinInfo
    {
      public ExpectedMixinInfo (Type expectedMixinType, bool needsDerivedMixin)
      {
        ArgumentUtility.CheckNotNull ("expectedMixinType", expectedMixinType);

        ExpectedMixinType = expectedMixinType;
        NeedsDerivedMixin = needsDerivedMixin;
      }

      public Type ExpectedMixinType { get; private set; }
      public bool NeedsDerivedMixin { get; private set; }
    }

    private readonly Type _targetType;
    private readonly ExpectedMixinInfo[] _expectedMixinInfo;
    private readonly TargetClassDefinition _targetClassDefinition;

    // TODO: Get rid of targetClassDefinition as soon as ConcreteTypeBuilder doesn't require MixinDefinitions any more. The, pass the 
    // ConcreteMixinTypeInfo into the ExpectedMixinInfo and use this as a cache key. The ConcreteMixinTypeInfo must be built by
    // InitializationCodeGenerator from the MixinDefinition known at code generation build time.
    public MixinArrayInitializer (
        Type targetType,
        ExpectedMixinInfo[] expectedMixinInfo, 
        TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("expectedMixinInfo", expectedMixinInfo);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      _targetType = targetType;
      _expectedMixinInfo = expectedMixinInfo;
      _targetClassDefinition = targetClassDefinition;
    }

    public object[] CreateMixinArray (object[] suppliedMixins)
    {
      var mixins = new object[_expectedMixinInfo.Length];

      FillInSuppliedMixins (mixins, suppliedMixins);

      for (int i = 0; i < mixins.Length; i++)
      {
        if (mixins[i] == null)
          mixins[i] = CreateMixin (_expectedMixinInfo[i], i);
      }
      return mixins;
    }

    private void FillInSuppliedMixins (object[] targetArray, object[] suppliedMixins)
    {
      if (suppliedMixins == null)
        return;

      // Note: This has a complexity of O(n*m) where n is the number of suppliedMixins and m is the total number of mixins.
      // We assume that m and especially n are very small.
      for (int i = 0; i < suppliedMixins.Length; ++i)
      {
        bool matchFound = false;
        var suppliedMixinType = suppliedMixins[i].GetType();

        for (int j = 0; j < targetArray.Length && !matchFound; ++j)
        {
          var expectedMixinType = _expectedMixinInfo[j].ExpectedMixinType;
          if (expectedMixinType.IsAssignableFrom (suppliedMixinType))
          {
            if (targetArray[j] != null)
            {
              var message = string.Format (
                  "Two mixins were supplied that would match the expected mixin type '{0}' on target class '{1}'.",
                  expectedMixinType,
                  _targetType);
              throw new InvalidOperationException (message);
            }
            else if (_expectedMixinInfo[j].NeedsDerivedMixin 
                && !ConcreteTypeBuilder.Current.GetConcreteMixinType (_targetClassDefinition.Mixins[j]).GeneratedType.IsAssignableFrom (suppliedMixinType))
            {
              var message = string.Format (
                  "A mixin was supplied that would match the expected mixin type '{0}' on target class '{1}'. However, a derived type must be "
                  + "generated for that mixin type, so the supplied instance cannot be used.",
                  expectedMixinType,
                  _targetType);
              throw new InvalidOperationException (message);
            }
            targetArray[j] = suppliedMixins[i];
            matchFound = true;
          }
        }

        if (!matchFound)
        {
          string message = string.Format (
              "The supplied mixin of type '{0}' is not valid for target type '{1}' in the current configuration.",
              suppliedMixinType,
              _targetType);
          throw new InvalidOperationException(message);
        }
      }
    }

    private object CreateMixin (ExpectedMixinInfo mixinInfo, int index)
    {
      Type mixinType = mixinInfo.NeedsDerivedMixin 
          ? ConcreteTypeBuilder.Current.GetConcreteMixinType (_targetClassDefinition.Mixins[index]).GeneratedType 
          : mixinInfo.ExpectedMixinType;

      if (mixinType.IsValueType)
      {
        return Activator.CreateInstance (mixinType); // there's always a public constructor for value types
      }
      else
      {
        try
        {
          return ObjectFactory.Create (mixinType, ParamList.Empty);
        }
        catch (MissingMethodException ex)
        {
          string message = string.Format (
              "Cannot instantiate mixin '{0}' applied to class '{1}', there is no visible default constructor.",
              mixinInfo.ExpectedMixinType,
              _targetType);
          throw new MissingMethodException (message, ex);
        }
      }
    }
  }
}