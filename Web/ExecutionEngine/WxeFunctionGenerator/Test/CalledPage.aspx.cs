using System;
using Remotion.Web.ExecutionEngine;
using System.Collections.Generic;

namespace Test
{
  // <WxePageFunction>
  //   <Parameter name="input" type="string" />
  //   <Parameter name="other" type="List{int[,][]}" />
  //   <Parameter name="output" type="string" direction="Out" />
  //   <Parameter name="bothways" type="string" direction="InOut" />
  //   <ReturnValue type="string" />
	// </WxePageFunction>
  public partial class CalledPage: WxePage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

		protected void Button1_Click (object sender, EventArgs e)
		{
			ReturnValue = "thank you";
			Return ();
		}

    public static void Call (IWxePage page, WxeArgument<string> input, WxeArgument<List<int[,][]>> other, WxeArgument<string> ReturnValue)
    {

    }

    public static void Call (IWxePage page, string input, List<int[,][]> other, string ReturnValue)
    {
    }

    public static void foo()
    {
      WxeArgument<List<int[,][]>> list = null;
      IWxePage page = null;
      Call (page, "", list, (WxeArgument<string>) null);
      Call (page, "", new List<int[,][]>(), "");
    }
  }

  public interface IWxeArgument<T>
  {
    
  }

  public class WxeArgument<T>
  {
    private T _value;

    public static implicit operator WxeArgument<T> (T value)
    {
      return new WxeArgument<T> (value);
    }

    public WxeArgument (T value)
    {
      _value = value;
    }
  }

}
