using System;
using System.Net;
using System.Web.UI;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using System.Security.Cryptography.X509Certificates;


public partial class AuthGmail : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Page.Request["uid"] != null)
        {
            try
            {
                string uid = Page.Request["uid"].ToString();   //username in our system;
                string pwd = Page.Request["pwd"].ToString();   //password in our system;

                LoginGmail(uid, pwd);
            }
            catch
            {
                Response.Write("Fail!");
                return;
            }
        }
        
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        string uid = TextBox1.Text;   //username in our system;
        string pwd = TextBox2.Text;   //password in our system;

        LoginGmail(uid, pwd);
    }

    protected void LoginGmail(string uid, string pwd)
    {
        string cpath = System.Web.Configuration.WebConfigurationManager.AppSettings["cert"].ToString();

        string SSO = System.Configuration.ConfigurationManager.AppSettings["LinkSSO"].ToString();

        bool yn = AuthAccount(uid, pwd);  //Auth with our system;

        if (yn)
        {
            String serviceAccountEmail = "your service account email in google apps admin console ";

            var certificate = new X509Certificate2(cpath, "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   User = "admin email",
                   Scopes = new[] { DirectoryService.Scope.AdminDirectoryUser, DirectoryService.Scope.AdminDirectoryGroup, DirectoryService.Scope.AdminDirectoryGroupMember },
               }.FromCertificate(certificate));


            // Create the service.
            var service = new DirectoryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Admin SDK",
            });

            // Check if the user has a gmail account
            bool yngot = CheckGStatus(uid);

            if (yngot)
            {
                //Start to login gmail;
                if (!CheckGmail(service, uid))
                {
                    Response.Write("no such gmail account");
                }
                else
                {
                    //Sync the groups in google apps domain
                    ExecuteHttp("AuthGmailGroup.aspx?uid=" + uid);

                    #region login gmail
                    writeTimeTick(uid); //write the timetick that the user login

                    if (Page.Request.Url.Query.Contains("SAMLRequest"))
                        Response.Redirect("SingleSignOnSmail.aspx" + Page.Request.Url.Query + "&u=" + uid);
                    else
                        Response.Redirect(SSO + "?u=" + uid);
                    #endregion

                }


            }
            else
            {

                //create gmail account
                if (CreateGmail(service, uid))
                {
                    try
                    {
                        //update the gmail status of this account in DB

                        //Sync the groups in google apps domain
                        ExecuteHttp("AuthGmailGroup.aspx?uid=" + uid);

                        #region login gmail
                        writeTimeTick(uid); //write the timetick that the user login

                        if (Page.Request.Url.Query.Contains("SAMLRequest"))
                            Response.Redirect("SingleSignOnSmail.aspx" + Page.Request.Url.Query + "&u=" + uid);
                        else
                            Response.Redirect(SSO + "?u=" + uid);
                        #endregion


                    }
                    catch (Exception e1)
                    {
                        Response.Write(e1.Message);
                    }
                }
                else
                {
                    Response.Write("Fail to create gmail account<br>");
                }

            }
        }
        else
        {
            Response.Write("fail to login");
        }
    }

    protected bool AuthAccount(string uid, string pwd)
    {
        string _uid = "username in our system";
        string _pwd = "password in our system";

        if (uid == _uid && pwd == _pwd)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected bool CheckGStatus(string uid)
    {
        //read the gmail status in our system
        return true;
    }
    protected bool CheckGmail(DirectoryService service, string account)
    {
        string gmail = account + "@your domain name";

        bool yn = false;   //check if the account exists in apps domain;

        try
        {
            User user = service.Users.Get(gmail).Execute();

            yn = true;
        }
        catch (Exception e1)
        {
            Response.Write("Fail:" + e1.Message + "<br>");
            yn = false;
        }

        return yn;

    }

    protected bool CreateGmail(DirectoryService service, string account)
    {
        // Create a new user.

        try
        {
            var gmail = account + "@your domain name";
           
            
            UserName uname = new UserName();
            uname.FamilyName = "family name";
            uname.GivenName = "given name";
            uname.FullName = "full name";

            User user = new User();
            user.Name = uname;
            user.HashFunction = "SHA-1";
            user.PrimaryEmail = gmail;
            service.Users.Insert(user).Execute();
            
            return true;
        }
        catch (Exception e1)
        {
            Response.Write("error:" + e1.Message + "<br>");
            return false;
        }
    }
    
    public void writeTimeTick(string uid)
    {
        string timeTick = DateTime.Now.Ticks.ToString();

        //insert the user and the time tick into a table
    }

    protected string ExecuteHttp(string url)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)
               WebRequest.Create(url);

            // execute the request
            HttpWebResponse response = (HttpWebResponse)
                request.GetResponse();

            response.Close();
        }
        catch (Exception e)
        {
            return e.ToString();
        }

        return "";
    }
    


}
