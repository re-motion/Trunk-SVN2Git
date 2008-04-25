using System;
using System.ComponentModel.Design;
using Remotion.Design;

namespace Remotion.UnitTests.Design
{
  public class StubDesignModeHelper: DesignModeHelperBase
  {
    public StubDesignModeHelper (IDesignerHost designerHost)
        : base (designerHost)
    {
    }

    public override string GetProjectPath()
    {
      throw new NotImplementedException();
    }

    public override System.Configuration.Configuration GetConfiguration()
    {
      throw new NotImplementedException();
    }
  }
}