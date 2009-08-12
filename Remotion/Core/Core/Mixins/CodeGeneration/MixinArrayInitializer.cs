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
using Remotion.Mixins.Definitions;
using Remotion.Reflection;
using Remotion.Text;
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

    public void CheckMixinArray (object[] mixins)
    {
      ArgumentUtility.CheckNotNull ("mixins", mixins);

      if (mixins.Length != _expectedMixinInfo.Length)
        throw CreateInvalidMixinArrayException(mixins);

      for (int i = 0; i < mixins.Length; ++i)
      {
        if (!GetConcreteExpectedMixinType (i).IsAssignableFrom (mixins[i].GetType()))
          throw CreateInvalidMixinArrayException (mixins);
      }
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
      foreach (var suppliedMixin in suppliedMixins)
      {
        int index = GetSuppliedMixinIndex (suppliedMixin);
        if (index == -1)
        {
          string message = string.Format (
              "The supplied mixin of type '{0}' is not valid for target type '{1}' in the current configuration.",
              suppliedMixin.GetType (),
              _targetType);
          throw new InvalidOperationException (message);
        }
        else
        {
          if (targetArray[index] != null)
          {
            var message = string.Format (
                "Two mixins were supplied that would match the expected mixin type '{0}' on target class '{1}'.",
                _expectedMixinInfo[index].ExpectedMixinType,
                _targetType);
            throw new InvalidOperationException (message);
          }

          targetArray[index] = suppliedMixin;
        }
      }
    }

    private int GetSuppliedMixinIndex (object suppliedMixin)
    {
      var suppliedMixinType = suppliedMixin.GetType ();

      for (int index = 0; index < _expectedMixinInfo.Length; ++index)
      {
        var expectedMixinType = _expectedMixinInfo[index].ExpectedMixinType;
        if (GetConcreteExpectedMixinType (index).IsAssignableFrom (suppliedMixinType))
        {
          return index;
        }
        else if (_expectedMixinInfo[index].ExpectedMixinType.IsAssignableFrom (suppliedMixinType))
        {
          var message = string.Format (
              "A mixin was supplied that would match the expected mixin type '{0}' on target class '{1}'. However, a derived type must be "
              + "generated for that mixin type, so the supplied instance cannot be used.",
              expectedMixinType,
              _targetType);
          throw new InvalidOperationException (message);
        }
      }

      return -1;
    }

    private object CreateMixin (ExpectedMixinInfo mixinInfo, int index)
    {
      Type mixinType = GetConcreteExpectedMixinType (index);

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

    private InvalidOperationException CreateInvalidMixinArrayException (object[] mixins)
    {
      var expectedMixinTypes = SeparatedStringBuilder.Build (", ", GetConcreteExpectedMixinTypes ());
      var givenMixinTypes = SeparatedStringBuilder.Build (", ", mixins, mixin => mixin.GetType ().ToString ());
      var message = string.Format (
          "Invalid mixin instances supplied. Expected the following mixin types (in this order): ('{0}'). The given types were: ('{1}').",
          expectedMixinTypes,
          givenMixinTypes);
      return new InvalidOperationException (message);
    }

    private IEnumerable<Type> GetConcreteExpectedMixinTypes ()
    {
      for (int i = 0; i < _expectedMixinInfo.Length; ++i)
        yield return GetConcreteExpectedMixinType (i);
    }

    private Type GetConcreteExpectedMixinType (int index)
    {
      if (_expectedMixinInfo[index].NeedsDerivedMixin)
      {
        return ConcreteTypeBuilder.Current.GetConcreteMixinType (
            _targetClassDefinition.ConfigurationContext,
            _targetClassDefinition.Mixins[index].GetConcreteMixinTypeIdentifier ()).GeneratedType;
      }
      else
      {
        return _expectedMixinInfo[index].ExpectedMixinType;
      }
    }
  }
}