using System;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  public class ParameterType
  {
  }

  public interface IBaseInterface
  {
  }

  public interface IDerivedInterface : IBaseInterface
  {
  }

  public interface IGenericInterface<T> : IBaseInterface
  {
  }

  public interface IGenericInterface<T1, T2> : IBaseInterface
  {
  }

  public interface IDerivedGenericInterface : IGenericInterface<ParameterType>
  {
  }

  public interface IDoubleDerivedGenericInterface : IDerivedGenericInterface
  {
  }

  // inherits IGenericInterface<> twice
  public interface IDoubleInheritingGenericInterface : IGenericInterface<int>, IGenericInterface<ParameterType>
  {
  }

  public interface IGenericDerivedGenericInterface<T> : IGenericInterface<ParameterType>
      where T: struct
  {
  }

  public interface IDerivedGenericInterface<T> : IGenericInterface<T>
      where T: ParameterType
  {
  }

  public interface IDerivedOpenGenericInterface<T> : IGenericInterface<ParameterType, T>
  {
  }

  public class BaseType
  {
  }

  public class DerivedType : BaseType, IDerivedInterface
  {
  }

  public class GenericType<T> : BaseType
  {
  }

  public class GenericType<T1, T2> : BaseType
  {
  }

  public class DerivedGenericType : GenericType<ParameterType>
  {
  }

  public class GenericDerivedGenericType<T> : GenericType<ParameterType>
      where T: struct
  {
  }

  public class DerivedGenericType<T> : GenericType<T>
      where T: ParameterType
  {
  }

  public class DerivedOpenGenericType<T> : GenericType<ParameterType, T>
  {
  }

  public class TypeWithBaseInterface : IBaseInterface
  {
  }

  public class TypeWithGenericInterface<T> : TypeWithBaseInterface, IGenericInterface<T>
  {
  }

  public class TypeWithGenericInterface<T1, T2> : TypeWithBaseInterface, IGenericInterface<T1, T2>
  {
  }

  public class DerivedTypeWithGenericInterface<T> : TypeWithGenericInterface<T>
  {
  }

  public class TypeWithDerivedGenericInterface : TypeWithGenericInterface<ParameterType>
  {
  }

  public class TypeWithGenericDerivedGenericInterface<T> : BaseType, IGenericDerivedGenericInterface<T>
      where T: struct
  {
  }

  public class TypeWithDerivedGenericInterface<T> : TypeWithBaseInterface, IDerivedGenericInterface<T>
      where T: ParameterType
  {
  }

  public class TypeWithDerivedOpenGenericInterface<T> : TypeWithBaseInterface, IDerivedOpenGenericInterface<T>
  {
  }
}