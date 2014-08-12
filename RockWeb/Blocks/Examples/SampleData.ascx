<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SampleData.ascx.cs" Inherits="RockWeb.Blocks.Examples.SampleData" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-flask"></i> Sample Data Import</h1>
            </div>
            <div class="panel-body">

                <Rock:NotificationBox ID="nbMessage" runat="server" Visible="true" NotificationBoxType="Warning" Title="Important!" Text="Never load sample data into your production (real) Rock system.  This sample data is for training and testing purposes only."></Rock:NotificationBox>
                <Rock:NotificationBox ID="nbError" runat="server" Visible="false" NotificationBoxType="Danger" Title="Problem Detected" Text="I can't see the file with the sample data.  It's either missing or there is a network problem preventing me from accessing it."></Rock:NotificationBox>

                <asp:Panel ID="pnlInputForm" runat="server">
                    <fieldset>
                        <Rock:RockTextBox ID="tbPassword" runat="server" Label="Password to use for Logins" Help="The password to use for any user logins that are created in the sample data.  Note: If you don't provide a password no logins will be added." ></Rock:RockTextBox>
                    </fieldset>
                    <Rock:BootstrapButton ID="bbtnLoadData" runat="server" CssClass="btn btn-primary" OnClick="bbtnLoadData_Click" Text="Load Sample Data" DataLoadingText="Loading...(this may take a few minutes)"></Rock:BootstrapButton>           
                </asp:Panel>

                <Rock:RockLiteral ID="lTime" runat="server"></Rock:RockLiteral>

            </div>
        </div>
        
    </ContentTemplate>
</asp:UpdatePanel>