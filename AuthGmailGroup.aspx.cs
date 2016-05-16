// This sample is only for non-commercial use
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using System.Security.Cryptography.X509Certificates;


public partial class AuthGmailGroup : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        AuthGroup(Request["uid"].ToString());
    }

    public void AuthGroup(string uid)
    {
        string cpath = System.Web.Configuration.WebConfigurationManager.AppSettings["cert"];

        
        bool yn = AuthAccount(uid);  //check if the account exists in our system and has a gmail account;

        if (yn)
        {
            String serviceAccountEmail = "your service account";

            var certificate = new X509Certificate2(cpath, "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    User = "your admin email",
                    Scopes = new[] { DirectoryService.Scope.AdminDirectoryUser, DirectoryService.Scope.AdminDirectoryGroup, DirectoryService.Scope.AdminDirectoryGroupMember,
                                    DirectoryService.Scope.AdminDirectoryOrgunit},
                }.FromCertificate(certificate));

            // Create the service.
            var service = new DirectoryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = null,
                ApplicationName = "Admin SDK",
            });

            //sync gmail group
            CheckGroup(service, uid, "google group email");

            //sync gmail organization
            CheckOrgunit(service, uid, "google ou");
            
        }

    }
    

    protected bool AuthAccount(string account)
    {
        bool ynexist = true;

        if (ynexist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
        
    protected bool CheckGroup(DirectoryService service, string account, string groupemail)
    {        
        bool yn = false;   //if already in this group;
        bool yn1 = false;  //if in another group
        bool ynteagrp = false; //if already in classroom teacher group

        string email = account + "@your domain";

        try
        {
            // Query for a single user and get his groups;
            var groupReq = service.Groups.List();
            groupReq.UserKey = email;
            Groups feed = groupReq.Execute();
            
            if (groupemail == "")
            {
                //remove all his groups
                if (feed.GroupsValue != null)
                {
                    foreach (Group group in feed.GroupsValue)
                    {

                        try
                        {
                            if (group.Email != "classroom_teachers@your domain name") //the member in classroom teacher group is not neccessary to be removed
                            {
                                service.Members.Delete(group.Email, email).Execute();
                            }
                        }
                        catch (Exception e)
                        {
                        }

                    }
                }
                return true;
            }

            //Check if the user is in this group already 
            if (feed.GroupsValue != null)
            {
                foreach (Group group in feed.GroupsValue)
                {
                    if (group.Email == groupemail)
                    {
                        yn = true;
                    }
                    if (group.Email != groupemail)
                    {
                        yn1 = true;
                    }
                    if (group.Email == "classroom_teachers@your domain name")
                    {
                        ynteagrp = true;
                    }
                }
            }

            #region join this group
            if (!yn)
            {
                //join this group
                try
                {
                    try
                    {
                        Group ge = service.Groups.Get(groupemail).Execute();
                    }
                    catch (Exception e1)
                    {
                        //create group
                        Group ge = new Group();
                        ge.Name = "this group name";
                        ge.Email = groupemail;
                        service.Groups.Insert(ge).Execute();
                    }

                    try
                    {
                        //join this group
                        var userEmail = email;
                        var groupEmail = groupemail;
                        var member = new Member();
                        member.Email = userEmail;
                        member.Role = "MEMBER";

                        service.Members.Insert(member, groupEmail).Execute();
                    }
                    catch (Exception e1)
                    {

                    }


                }
                catch (Exception e2)
                {
                    
                }
                yn = true;
            }
            #endregion

            if (yn1)
            {
                //remove the user from the original group
                foreach (Group group in feed.GroupsValue)
                {

                    try
                    {
                        if (group.Email != groupemail)
                            service.Members.Delete(group.Email, email).Execute();
                    }
                    catch (Exception e)
                    {
                    }

                }
            }

            if (!ynteagrp)
            {
                //join classroom teacher group
                try
                {
                    var userEmail = email;
                    var groupEmail = "classroom_teachers@your domain name";
                    var member = new Member();
                    member.Email = userEmail;
                    member.Role = "MEMBER";

                    service.Members.Insert(member, groupEmail).Execute();
                }
                catch (Exception e1)
                {

                }
            }

        }
        catch (Exception e1)
        {
            yn = false;
        }

        return yn;

    }

    protected bool CheckOrgunit(DirectoryService service, string account, string ou)
    {
    
        bool yn = false;   //check if the user in this organization already

        string email = account + "@your domain name";

        try
        {
            // Query for a single user and get his OU;
            User user = service.Users.Get(email).Execute();

            if (ou == "")
            {
                //move the user to root ou
                if (user.OrgUnitPath != "/")
                {
                    try
                    {
                        User user1 = new User();
                        user1.OrgUnitPath = "/";
                        user1.PrimaryEmail = email;
                        service.Users.Update(user1, email).Execute();
                    }
                    catch (Exception e)
                    {
                    }
                }
                return true;
            }

            //already in this ou
            if (user.OrgUnitPath == "/" + ou)
                return true;

            //check if the ou exists
            User user3 = service.Users.Get("admin email").Execute();
            try
            {
                OrgUnit ou1 = service.Orgunits.Get(user3.CustomerId, ou).Execute();
            }
            catch (Exception e)
            {
                //create the ou
                OrgUnit orgunit = new OrgUnit();
                orgunit.Name = ou;
                orgunit.ParentOrgUnitPath = "/";
                orgunit.Description = ou;
                service.Orgunits.Insert(orgunit, user3.CustomerId).Execute();
            }
            


            //join thie ou
            try
            {
                var userEmail = email;
                var user2 = new User();
                user2.PrimaryEmail = userEmail;
                user2.OrgUnitPath = "/" + ou;
                service.Users.Update(user2, email).Execute();
            }
            catch (Exception e1)
            {

            }


        }
        catch (Exception e1)
        {
            yn = false;
        }

        return yn;

    }
   
}
