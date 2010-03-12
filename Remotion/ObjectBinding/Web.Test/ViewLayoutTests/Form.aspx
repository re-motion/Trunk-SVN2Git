<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="OBWTest.ViewLayoutTests.Form" %>

<asp:content contentplaceholderid="head" runat="server">
</asp:content>
<asp:content contentplaceholderid="body" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" AsyncPostBackTimeout="3600" />
    <remotion:SingleView ID="OuterSingleView" runat="server">
      <TopControls>
      <div style="background: lime">
      Top Controls
      </div>
      </TopControls>
      
      <View>
    
        <div style="position: absolute; left:0; width: 33%; top: 0; height: 100%;">
          <remotion:SingleView ID="InnerSingleView" runat="server">
            <TopControls>
            <div style="background: yellow">
            Top Controls
            </div>
            </TopControls>
            
            <View>
            <div style="background: yellow">
            View
            </div>
            </View>
            
            <BottomControls>
            <div style="background: yellow">
            Bottom Controls
            </div>
           </BottomControls>
          </remotion:SingleView>
        </div>
        
        <div style="position: absolute; right:0; width: 66%; top: 0; height: 100%;">
          <remotion:TabbedMultiView ID="InnerMultiView" runat="server">
            <TopControls>
            <div style="background: cyan">
            Top Controls
            </div>
            </TopControls>
            
            <Views>
              <remotion:TabView Title="First Tab">
                <div style="background: cyan">
                View
                </div>
              </remotion:TabView>
            </Views>
            
            <BottomControls>
            <div style="background: cyan">
            Bottom Controls
            </div>
           </BottomControls>
          </remotion:TabbedMultiView>
        </div>

      </View>
      
      <BottomControls>
      <div style="background: lime">
      Bottom Controls
      </div>
     </BottomControls>
    </remotion:SingleView>
</asp:content>
