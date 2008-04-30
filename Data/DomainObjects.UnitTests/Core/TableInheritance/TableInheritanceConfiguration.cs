using System;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  public class TableInheritanceConfiguration: BaseConfiguration
  {
    private static TableInheritanceConfiguration s_instance;

    public static TableInheritanceConfiguration Instance
    {
      get
      {
        if (s_instance == null)
          throw new InvalidOperationException ("TableInheritanceConfiguration has not been Initialized by invoking Initialize()");
        return s_instance;
      }
    }

    public static void Initialize()
    {
      s_instance = new TableInheritanceConfiguration();
    }

    private readonly DomainObjectIDs _domainObjectIDs;

    private TableInheritanceConfiguration()
    {
      _domainObjectIDs = new DomainObjectIDs();
    }

    public DomainObjectIDs GetDomainObjectIDs()
    {
      return _domainObjectIDs;
    }
  }
}