using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [TestDomain]
  [Instantiable]
  [Serializable]
  [DBTable]
  [BindableDomainObject]
  public abstract class BindableBaseDomainObject : DomainObject
  {
    [StringProperty (MaximumLength = 3)]
    public virtual string BasePropertyWithMaxLength3
    {
      get { return CurrentProperty.GetValue<string> (); }
      set { CurrentProperty.SetValue (value); }
    }

    [StringProperty (MaximumLength = 4)]
    public virtual string BasePropertyWithMaxLength4
    {
      get { return CurrentProperty.GetValue<string> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}
