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
    public readonly CustomClassEmitter DeclaringType;
    public readonly PropertyBuilder PropertyBuilder;

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

      DeclaringType = declaringType;
      _name = name;
      _propertyKind = propertyKind;

      _propertyType = propertyType;
      _indexParameters = indexParameters;
    
      // TODO: As soon as the overload below is publicly available, use it
      // CallingConventions callingConvention = propertyKind == PropertyKind.Instance ? CallingConventions.HasThis : CallingConventions.Standard;
      // PropertyBuilder = DeclaringType.TypeBuilder.DefineProperty (name, attributes, callingConvention, propertyType, null, null, indexParameters,
      //    null, null);
      PropertyBuilder = DeclaringType.TypeBuilder.DefineProperty (name, attributes, propertyType, indexParameters);
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
          PropertyBuilder.SetGetMethod (_getMethod.MethodBuilder);
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
          PropertyBuilder.SetSetMethod (_setMethod.MethodBuilder);
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

    public CustomPropertyEmitter ImplementWithBackingField ()
    {
      string fieldName = MakeBackingFieldName (Name);
      FieldReference backingField;
      if (PropertyKind == PropertyKind.Static)
        backingField = DeclaringType.CreateStaticField (fieldName, PropertyType);
      else
        backingField = DeclaringType.CreateField (fieldName, PropertyType);
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

        CustomMethodEmitter method = DeclaringType.CreateMethod ("get_" + Name, flags);
        method.SetReturnType (PropertyType);
        method.SetParameterTypes (IndexParameters);
        GetMethod = method;
        return method;
      }
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

        CustomMethodEmitter method = DeclaringType.CreateMethod ("set_" + Name, flags);
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
      PropertyBuilder.SetCustomAttribute (customAttribute);
    }

  }
}