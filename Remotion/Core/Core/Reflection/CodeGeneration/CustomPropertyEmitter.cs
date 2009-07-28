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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public enum PropertyKind
  {
    Static,
    Instance
  }

  public class CustomPropertyEmitter : IAttributableEmitter
  {
    private readonly CustomClassEmitter _declaringType;
    private readonly PropertyBuilder _propertyBuilder;

    private readonly Type _propertyType;
    private readonly string _name;
    private readonly PropertyKind _propertyKind;
    private readonly Type[] _indexParameters;

    private CustomMethodEmitter _getMethod;
    private CustomMethodEmitter _setMethod;

    public CustomPropertyEmitter (CustomClassEmitter declaringType, string name, PropertyKind propertyKind, Type propertyType, Type[] indexParameters, PropertyAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);
      ArgumentUtility.CheckNotNull ("indexParameters", indexParameters);

      _declaringType = declaringType;
      _name = name;
      _propertyKind = propertyKind;

      _propertyType = propertyType;
      _indexParameters = indexParameters;
    
      // TODO: As soon as the overload below is publicly available, use it
      // CallingConventions callingConvention = propertyKind == PropertyKind.Instance ? CallingConventions.HasThis : CallingConventions.Standard;
      // PropertyBuilder = DeclaringType.TypeBuilder.DefineProperty (name, attributes, callingConvention, propertyType, null, null, indexParameters,
      //    null, null);
      _propertyBuilder = _declaringType.TypeBuilder.DefineProperty (name, attributes, propertyType, indexParameters);
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public Type[] IndexParameters
    {
      get { return _indexParameters; }
    }

    public CustomMethodEmitter GetMethod
    {
      get { return _getMethod; }
      set
      {
        if (value != null)
        {
          _getMethod = value;
          _propertyBuilder.SetGetMethod (_getMethod.MethodBuilder);
        }
        else
          throw new ArgumentNullException ("value", "Due to limitations in Reflection.Emit, property accessors cannot be set to null.");
      }
    }

    public CustomMethodEmitter SetMethod
    {
      get { return _setMethod; }
      set
      {
        if (value != null)
        {
          _setMethod = value;
          _propertyBuilder.SetSetMethod (_setMethod.MethodBuilder);
        }
        else
          throw new ArgumentNullException ("value", "Due to limitations in Reflection.Emit, property accessors cannot be set to null.");
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public PropertyKind PropertyKind
    {
      get { return _propertyKind; }
    }

    public CustomClassEmitter DeclaringType
    {
      get { return _declaringType; }
    }

    public PropertyBuilder PropertyBuilder
    {
      get { return _propertyBuilder; }
    }

    public CustomPropertyEmitter ImplementWithBackingField ()
    {
      string fieldName = MakeBackingFieldName (Name);
      FieldReference backingField;
      if (PropertyKind == PropertyKind.Static)
        backingField = _declaringType.CreateStaticField (fieldName, PropertyType);
      else
        backingField = _declaringType.CreateField (fieldName, PropertyType);
      return ImplementWithBackingField (backingField);
    }

    private static string MakeBackingFieldName (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return "_fieldFor" + propertyName;
    }

    public CustomPropertyEmitter ImplementWithBackingField (FieldReference backingField)
    {
      ArgumentUtility.CheckNotNull ("backingField", backingField);
      if (GetMethod != null)
        GetMethod.AddStatement (new ReturnStatement (backingField));
      if (SetMethod != null)
      {
        SetMethod.AddStatement (
            new AssignStatement (backingField, SetMethod.ArgumentReferences[IndexParameters.Length].ToExpression()));
        SetMethod.ImplementByReturningVoid();
      }
      return this;
    }

    public CustomMethodEmitter CreateGetMethod ()
    {
      if (GetMethod != null)
        throw new InvalidOperationException ("This property already has a getter method.");
      else
      {
        MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
        if (PropertyKind == PropertyKind.Static)
          flags |= MethodAttributes.Static;

        #if(false)
           // Leads to e.g. get_Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.MutableNameProperty()
          CustomMethodEmitter method = _declaringType.CreateMethod ("get_" + Name, flags);
        #else
          CustomMethodEmitter method = _declaringType.CreateMethod (BuildAccessorMethodName(Name,"get"), flags);
        #endif

        method.SetReturnType (PropertyType);
        method.SetParameterTypes (IndexParameters);
        GetMethod = method;
        return method;
      }
    }


    public static string BuildAccessorMethodName(string propertyName, string accessorName)
    {
      string s = propertyName;
      string sPath = "";
      string sPureName = s;
      int iSplit = s.LastIndexOf ('.');
      if (iSplit >= 0)
      {
        sPath = s.Substring (0, iSplit) + ".";
        sPureName = s.Substring (iSplit + 1, s.Length - iSplit - 1);
      }
      return sPath + accessorName + "_" + sPureName;
    }

    public CustomMethodEmitter CreateSetMethod ()
    {
      if (SetMethod != null)
        throw new InvalidOperationException ("This property already has a setter method.");
      else
      {
        MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
        if (PropertyKind == PropertyKind.Static)
          flags |= MethodAttributes.Static;

#if(false)
          // Leads to e.g. get_Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.MutableNameProperty()
          CustomMethodEmitter method = _declaringType.CreateMethod ("set_" + Name, flags);
#else
        CustomMethodEmitter method = _declaringType.CreateMethod (BuildAccessorMethodName (Name,"set"), flags);
        #endif


        Type[] setterParameterTypes = new Type[IndexParameters.Length + 1];
        IndexParameters.CopyTo (setterParameterTypes, 0);
        setterParameterTypes[IndexParameters.Length] = PropertyType;
        method.SetParameterTypes (setterParameterTypes);
        SetMethod = method;
        return method;
      }
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      _propertyBuilder.SetCustomAttribute (customAttribute);
    }

  }
}
