// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using System.Runtime.CompilerServices;
using Remotion.Collections;
using Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler;

namespace Remotion.Diagnostics.ToText.Infrastructure.ToTextProviderHandler
{
  /// <summary>
  /// Handles automatic conversion of arbitrary instances to human readable text form through reflection in
  /// <see cref="ToTextProvider"/>'s <see cref="ToTextProvider.ToText"/> fallback cascade.
  /// </summary>
  public class ToTextProviderAutomaticObjectToTextHandler : ToTextProviderHandler
  {
    // Define a cache instance (dictionary syntax)
    private static readonly InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]> s_memberInfoCache = new InterlockedCache<Tuple<Type, BindingFlags>, MemberInfo[]> ();

    public override void ToTextIfTypeMatches (ToTextParameters toTextParameters, ToTextProviderHandlerFeedback toTextProviderHandlerFeedback)
    {
      ToTextProviderHandler.CheckNotNull (toTextParameters, toTextProviderHandlerFeedback);

      Object obj = toTextParameters.Object;
      Type type = toTextParameters.Type;
      IToTextBuilder toTextBuilder = toTextParameters.ToTextBuilder;
      var settings = toTextParameters.ToTextBuilder.ToTextProvider.Settings;

      // Primitives and Type|s lead to endless recursion when treating them with reflection, so this handler skips them.
      if(type.IsPrimitive || (obj is Type))
      {
        return;
      }

      if (settings.UseAutomaticObjectToText)
      {
        ObjectToText (obj, toTextBuilder, settings.EmitPublicProperties, settings.EmitPublicFields, settings.EmitPrivateProperties, settings.EmitPrivateFields);
        toTextProviderHandlerFeedback.Handled = true;
      }
    }


    private void ObjectToText (object obj, IToTextBuilder toTextBuilder, bool emitPublicProperties, bool emitPublicFields, bool emitPrivateProperties, bool emitPrivateFields)
    {
      Type type = obj.GetType ();

      toTextBuilder.WriteInstanceBegin (type,null);

      if (emitPublicProperties)
      {
        ObjectToTextProcessMemberInfos (
            "Public Properties", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property, toTextBuilder);
      }
      if (emitPublicFields)
      {
        ObjectToTextProcessMemberInfos ("Public Fields", obj, BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field, toTextBuilder);
      }
      if (emitPrivateProperties)
      {
        ObjectToTextProcessMemberInfos ("Non Public Properties", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Property, toTextBuilder);
      }
      if (emitPrivateFields)
      {
        ObjectToTextProcessMemberInfos ("Non Public Fields", obj, BindingFlags.Instance | BindingFlags.NonPublic, MemberTypes.Field, toTextBuilder);
      }
      toTextBuilder.WriteSequenceEnd ();
    }

    private void ObjectToTextProcessMemberInfos (string message, object obj, BindingFlags bindingFlags, MemberTypes memberTypeFlags, IToTextBuilder toTextBuilder)
    {
      Type type = obj.GetType ();

      // Cache the member info result
      MemberInfo[] memberInfos = s_memberInfoCache.GetOrCreateValue (new Tuple<Type, BindingFlags> (type, bindingFlags), tuple => tuple.A.GetMembers (tuple.B));

      foreach (var memberInfo in memberInfos)
      {
        if ((memberInfo.MemberType & memberTypeFlags) != 0)
        {
          string name = memberInfo.Name;

          // Skip compiler generated backing fields
          bool isCompilerGenerated = memberInfo.IsDefined (typeof (CompilerGeneratedAttribute), false);
          if (!isCompilerGenerated)
          {
            object value = GetValue (obj, type, memberInfo);
            // WriteElement ToText value
            toTextBuilder.WriteElement (name, value);
          }
        }
      }
    }

    private object GetValue (object obj, Type type, MemberInfo memberInfo)
    {
      object value = null;
      if (memberInfo is PropertyInfo)
      {
        value = ((PropertyInfo) memberInfo).GetValue (obj, null);
      }
      else if (memberInfo is FieldInfo)
      {
        value = ((FieldInfo) memberInfo).GetValue (obj);
      }
      else
      {
        throw new System.NotImplementedException ();
      }
      return value;
    }

    private bool AutomaticObjectToText
    {
      get { return true; }
    }
  }
}
