using System;
using System.Web.UI.WebControls;
using Google.Apps.SingleSignOn;

public partial class SingleSignOn : System.Web.UI.Page
{
    protected Literal LiteralAssertionUrl;

    protected string actionUrl = string.Empty;

    protected string ActionUrl
    {
        get { return actionUrl; }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {

        string username = Request["u"].ToString();  
                
        Response.Write("Login Gmail" + username.ToString() + "<br><hr>");
        SetupGoogleLoginForm(username);
        
        if (readTimeTick(username.ToString().Trim()))
        {
            Response.Write("Timeout!");
            Response.Redirect("your login page");
        }
        else
        {
            SetupGoogleLoginForm(username);
        }
    }

    private void SetupGoogleLoginForm(string userName)
    {
        string samlRequest = Request.QueryString["SAMLRequest"];
        if (samlRequest == null)
        {
            samlRequest = Request.Form["SAMLRequest"];
        }
        string relayState = Request.QueryString["RelayState"];
        if (relayState == null)
        {
            relayState = Request.Form["RelayState"];
        }

        if (samlRequest != null && relayState != null)
        {
            string responseXml;
            string actionUrl;

            SamlParser.CreateSignedResponse(
                samlRequest, userName, out responseXml, out actionUrl);

            this.actionUrl = actionUrl;
            

            var url = actionUrl;
            FormSamlResponse.Action = url;
            ltrPostData.Text = "<input type='hidden' name='SAMLResponse' value='" + responseXml + "'>" +
                "<input type='hidden' name='RelayState' value='" + relayState + "'>";
        }
    }

    public bool readTimeTick(string uid)
    {
        DateTime endTime = DateTime.Now;
        TimeSpan span;

        //read timetick table;
        string startTimeTick = ""; 

        try
        {
            //read timetick of the user from a table

        }
        catch
        {
        }
        finally
        {
            
        }

        if (startTimeTick == "")
        {
            return false;
        }
        else
        {
            int ds = Convert.ToInt16(System.Configuration.ConfigurationManager.ConnectionStrings["ds"].ToString());
            span = new TimeSpan(endTime.Ticks - Convert.ToInt64(startTimeTick));

            if (Convert.ToInt32(span.TotalSeconds) > ds)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}