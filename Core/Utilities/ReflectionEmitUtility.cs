using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Text;
using Remotion.Utilities;
using System.Collections;

namespace Remotion.Utilities
{
  public static class ReflectionEmitUtility
  {
    public struct CustomAttributeBuilderData
    {
      public readonly object[] ConstructorArgs;
      public readonly PropertyInfo[] NamedProperties;
      public readonly object[] PropertyValues;
      public readonly FieldInfo[] NamedFields;
      public readonly object[] FieldValues;

      public CustomAttributeBuilderData (object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields,
                                         object[] fieldValues)
      {
        ConstructorArgs = constructorArgs;
        FieldValues = fieldValues;
        NamedFields = namedFields;
        PropertyValues = propertyValues;
        NamedProperties = namedProperties;
      }
    }

    public static CustomAttributeBuilder CreateAttributeBuilderFromData (CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CustomAttributeBuilderData builderData = GetCustomAttributeBuilderData (attributeData);
      return new CustomAttributeBuilder (attributeData.Constructor, builderData.ConstructorArgs, builderData.NamedProperties,
          builderData.PropertyValues, builderData.NamedFields, builderData.FieldValues);
    }

    public static CustomAttributeBuilderData GetCustomAttributeBuilderData (CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CheckForPublicFields (attributeData.Constructor.DeclaringType);

      object[] constructorArgs = new object[attributeData.ConstructorArguments.Count];
      for (int i = 0; i < constructorArgs.Length; ++i)
        constructorArgs[i] = GetRealCustomAttributeArgumentValue (attributeData.ConstructorArguments[i]);

      List<PropertyInfo> namedProperties = new List<PropertyInfo> ();
      List<object> propertyValues = new List<object> ();
      List<FieldInfo> namedFields = new List<FieldInfo> ();
      List<object> fieldValues = new List<object> ();

      foreach (CustomAttributeNamedArgument namedArgument in attributeData.NamedArguments)
      {
        switch (namedArgument.MemberInfo.MemberType)
        {
          case MemberTypes.Field:
            namedFields.Add ((FieldInfo) namedArgument.MemberInfo);
            fieldValues.Add (GetRealCustomAttributeArgumentValue (namedArgument.TypedValue));
            break;
          case MemberTypes.Property:
            namedProperties.Add ((PropertyInfo) namedArgument.MemberInfo);
            propertyValues.Add (GetRealCustomAttributeArgumentValue (namedArgument.TypedValue));
            break;
          default:
            Assertion.IsTrue (false);
            break;
        }
      }

      return new CustomAttributeBuilderData (constructorArgs, namedProperties.ToArray (), propertyValues.ToArray (), namedFields.ToArray (),
          fieldValues.ToArray ());
    }

    // This is due to a bug in CustomAttributeData.GetCustomAttributes, where CustomAttributeData misinterprets named arguments if public fields
    // are present on a type, see ReflectionEmitUtilityTests.CreateAttributeBuilderFromData_WithCtorArgumentsNamedArgumentsAndNamedFields_Throws.
    // This check can be removed when the bug has been fixed with a .NET 2.0 framework service pack.
    private static void CheckForPublicFields (Type type)
    {
      FieldInfo[] fields = type.GetFields (BindingFlags.Instance | BindingFlags.Public);
      if (fields.Length > 0)
      {
        string message = string.Format ("Type {0} declares public fields: {1}. Due to a bug in CustomAttributeData.GetCustomAttributes, attributes "
            + "with public fields are currently not supported.", type.FullName,
            SeparatedStringBuilder.Build (", ", fields, delegate (FieldInfo field) { return field.Name; }));
        throw new NotSupportedException (message);
      }
    }

    private static object GetRealCustomAttributeArgumentValue (CustomAttributeTypedArgument typedArgument)
    {
      if (typedArgument.ArgumentType.IsArray)
      {
        IList<CustomAttributeTypedArgument> elements = (IList<CustomAttributeTypedArgument>) typedArgument.Value;
        IList realValueArray = Array.CreateInstance (typedArgument.ArgumentType.GetElementType(), elements.Count);
        for (int i = 0; i < elements.Count; ++i)
          realValueArray[i] = GetRealCustomAttributeArgumentValue (elements[i]);
        return realValueArray;
      }
      else
        return typedArgument.Value;
    }
  }
}