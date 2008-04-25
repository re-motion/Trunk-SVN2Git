using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public class SignatureChecker
  {
    public bool PropertySignaturesMatch (PropertyInfo propertyOne, PropertyInfo propertyTwo)
    {
      ArgumentUtility.CheckNotNull ("propertyOne", propertyOne);
      ArgumentUtility.CheckNotNull ("propertyTwo", propertyTwo);

      ParameterInfo[] parametersOne = propertyOne.GetIndexParameters();
      ParameterInfo[] parametersTwo = propertyTwo.GetIndexParameters();

      return ParametersMatch (parametersOne, parametersTwo) && TypesMatch (propertyOne.PropertyType, propertyTwo.PropertyType);
    }

    public bool EventSignaturesMatch (EventInfo eventOne, EventInfo eventTwo)
    {
      ArgumentUtility.CheckNotNull ("eventOne", eventOne);
      ArgumentUtility.CheckNotNull ("eventTwo", eventTwo);

      return TypesMatch (eventOne.EventHandlerType, eventTwo.EventHandlerType);
    }


    public bool MethodSignaturesMatch (MethodInfo methodOne, MethodInfo methodTwo)
    {
      ArgumentUtility.CheckNotNull ("methodOne", methodOne);
      ArgumentUtility.CheckNotNull ("methodTwo", methodTwo);

      if (!TypesMatch (methodOne.ReturnType, methodTwo.ReturnType))
        return false;

      if (!ParametersMatch (methodOne.GetParameters(), methodTwo.GetParameters()))
        return false;

      if (!GenericParametersMatch (methodOne, methodTwo))
        return false;

      return true;
    }

    private bool GenericParametersMatch (MethodInfo methodOne, MethodInfo methodTwo)
    {
      if (methodOne.IsGenericMethod != methodTwo.IsGenericMethod)
        return false;

      if (!methodOne.IsGenericMethod)
        return true;

      Type[] genericArgsOne = methodOne.GetGenericArguments();
      Type[] genericArgsTwo = methodTwo.GetGenericArguments();

      if (genericArgsOne.Length != genericArgsTwo.Length)
        return false;

      for (int i = 0; i < genericArgsOne.Length; ++i)
      {
        if (!TypesMatch (genericArgsOne[i], genericArgsTwo[i]))
          return false;
      }
      return true;
    }

    private bool ParametersMatch (ParameterInfo[] paramsOne, ParameterInfo[] paramsTwo)
    {
      if (paramsOne.Length != paramsTwo.Length)
        return false;

      for (int i = 0; i < paramsOne.Length; ++i)
      {
        if (!ParametersMatch (paramsOne[i], paramsTwo[i]))
          return false;
      }
      return true;
    }

    private bool ParametersMatch (ParameterInfo one, ParameterInfo two)
    {
      return one.IsIn == two.IsIn
          && one.IsOut == two.IsOut
              && TypesMatch (one.ParameterType, two.ParameterType);
    }

    private bool TypesMatch (Type one, Type two)
    {
      if (one.IsGenericParameter != two.IsGenericParameter) // both must be generic parameters _or_ no generic parameters
        return false;

      if (one.IsGenericParameter) // comparing generic parameters
      {
        if ((one.DeclaringMethod == null) != (two.DeclaringMethod == null)) // both must come either from a method _or_ from a type
          return false;

        if (one.DeclaringMethod != null) // comparing types from two generic methods? check position and attributes of parameter
          return (one.GenericParameterAttributes == two.GenericParameterAttributes) && (one.GenericParameterPosition == two.GenericParameterPosition);
        else // no, the generic parameters come from a type
          return GenericParametersFromTypeMatch (one, two);
      }
      else // comparing normal types
        return one.Equals (two);
    }

    private bool GenericParametersFromTypeMatch (Type one, Type two)
    {
      return one.Equals (two);
    }
  }
}