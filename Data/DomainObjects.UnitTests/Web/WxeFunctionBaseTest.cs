using System;
using System.Collections.Specialized;
using System.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.AspNetFramework;
using Remotion.Web.UnitTests.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web
{
  [CLSCompliant (false)]
  public class WxeFunctionBaseTest : StandardMappingTest
  {
    private WxeContextMock _context;

    public override void SetUp ()
    {
      _context = new WxeContextMock (WxeContextTest.CreateHttpContext());

      base.SetUp ();
    }

    public WxeContextMock Context
    {
      get { return _context; }
    }
  }
}