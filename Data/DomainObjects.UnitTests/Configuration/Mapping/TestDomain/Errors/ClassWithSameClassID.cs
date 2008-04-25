using System;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [ClassID ("DefinedID")]
  public class ClassWithSameClassID : DomainObject
  {    
  }

  [ClassID ("DefinedID")]
  public class OtherClassWithSameClassID : DomainObject
  {
  }
}