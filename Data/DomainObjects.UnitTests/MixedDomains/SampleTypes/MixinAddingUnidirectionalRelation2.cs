using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinAddingUnidirectionalRelation2 : DomainObjectMixin<DomainObject>
  {
    [DBColumn("Computer2ID")]
    public Computer Computer
    {
      get { return Properties[typeof (MixinAddingUnidirectionalRelation2), "Computer"].GetValue<Computer>(); }
      set { Properties[typeof (MixinAddingUnidirectionalRelation2), "Computer"].SetValue (value); }
    }
  }
}