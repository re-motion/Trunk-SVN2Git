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
    public struct ExpectedMixinInfo
    {
      public ExpectedMixinInfo (Type expectedMixinType, bool requiresDerivedMixin)
          : this()
      {
        ExpectedMixinType = expectedMixinType;
        RequiresDerivedMixin = requiresDerivedMixin;
      }

      public Type ExpectedMixinType { get; private set; }
      public bool RequiresDerivedMixin { get; private set; }
    }

    private readonly Type _targetType;
    private readonly ExpectedMixinInfo[] _expectedMixinInfo;
    private readonly object[] _suppliedMixins;
    private readonly TargetClassDefinition _targetClassDefinition;

    // TODO: Get rid of targetClassDefinition as soon as ConcreteTypeBuilder doesn't require MixinDefinitions any more. The, pass the 
    // ConcreteMixinTypeInfo into the ExpectedMixinInfo and use this as a cache key. The ConcreteMixinTypeInfo must be built by
    // InitializationCodeGenerator from the MixinDefinition known at code generation build time.
    public MixinArrayInitializer (
        Type targetType,
        ExpectedMixinInfo[] expectedMixinInfo, 
        object[] suppliedMixins, 
        TargetClassDefinition targetClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("expectedMixinInfo", expectedMixinInfo);
      ArgumentUtility.CheckNotNull ("suppliedMixins", suppliedMixins);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);

      _targetType = targetType;
      _expectedMixinInfo = expectedMixinInfo;
      _suppliedMixins = suppliedMixins;
      _targetClassDefinition = targetClassDefinition;
    }

    public object[] CreateMixinArray ()
    {
      var mixins = new object[_expectedMixinInfo.Length];

      FillInSuppliedMixins (mixins);

      for (int i = 0; i < mixins.Length; i++)
      {
        if (mixins[i] == null)
          mixins[i] = CreateMixin (_expectedMixinInfo[i], i);
      }
      return mixins;
    }

    private void FillInSuppliedMixins (object[] mixins)
    {
      for (int i = 0; i < _suppliedMixins.Length; ++i)
      {
        bool matchFound = false;
        for (int j = 0; j < mixins.Length && !matchFound; ++j)
        {
          if (_expectedMixinInfo[j].ExpectedMixinType.IsAssignableFrom (_suppliedMixins[i].GetType()))
          {
            if (mixins[j] != null)
            {
              var message = string.Format (
                  "Two mixins were supplied that would match the expected mixin type '{0}' on target class '{1}'.",
                  _expectedMixinInfo[j].ExpectedMixinType,
                  _targetType);
              throw new InvalidOperationException (message);
            }
            else if (_expectedMixinInfo[j].RequiresDerivedMixin)
            {
              var message = string.Format (
                  "A mixin was supplied that would match the expected mixin type '{0}' on target class '{1}'. However, a derived type must be "
                  + "generated for that mixin type, so the supplied instance cannot be used.",
                  _expectedMixinInfo[j].ExpectedMixinType,
                  _targetType);
              throw new InvalidOperationException (message);
            }
            mixins[j] = _suppliedMixins[i];
            matchFound = true;
          }
        }

        if (!matchFound)
        {
          string message = string.Format (
              "The supplied mixin of type '{0}' is not valid for target type '{1}' in the current configuration.", 
              _suppliedMixins[i].GetType (),
              _targetType);
          throw new InvalidOperationException(message);
        }
      }
    }

    private object CreateMixin (ExpectedMixinInfo mixinInfo, int index)
    {
      Type mixinType = mixinInfo.RequiresDerivedMixin 
          ? ConcreteTypeBuilder.Current.GetConcreteMixinType (_targetClassDefinition.Mixins[index]).GeneratedType 
          : mixinInfo.ExpectedMixinType;
      
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