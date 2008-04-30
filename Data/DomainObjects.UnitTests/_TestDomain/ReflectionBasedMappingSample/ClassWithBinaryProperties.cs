using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithBinaryProperties : DomainObject
  {
    protected ClassWithBinaryProperties ()
    {
    }

    public abstract byte[] NoAttribute { get; set; }

    [BinaryProperty (IsNullable = true)]
    public abstract byte[] NullableFromAttribute { get; set; }

    [BinaryProperty (IsNullable = false)]
    public abstract byte[] NotNullable { get; set; }

    [BinaryProperty (MaximumLength = 100)]
    public abstract byte[] MaximumLength { get; set; }

    [BinaryProperty (IsNullable = false, MaximumLength = 100)]
    public abstract byte[] NotNullableAndMaximumLength { get; set; }
  }
}