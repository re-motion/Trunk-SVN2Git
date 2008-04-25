using System;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  public static class DomainObjectIDs
  {
    public static ObjectID ObjectWithAllDataTypes1 = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));
    public static ObjectID ObjectWithUndefinedEnum = new ObjectID ("ClassWithUndefinedEnum", new Guid ("{4F85CEE5-A53A-4bc5-B9D3-448C48946498}"));
  }
}