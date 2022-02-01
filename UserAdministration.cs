// VIGOBAS Identity Management System 
//  Copyright (C) 2021  Vigo IKS 
//  
//  Documentation - visit https://vigobas.vigoiks.no/ 
//  
//  This program is free software: you can redistribute it and/or modify 
//  it under the terms of the GNU Affero General Public License as 
//  published by the Free Software Foundation, either version 3 of the 
//  License, or (at your option) any later version. 
//  
//  This program is distributed in the hope that it will be useful, 
//  but WITHOUT ANY WARRANTY, without even the implied warranty of 
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//  GNU Affero General Public License for more details. 
//  
//  You should have received a copy of the GNU Affero General Public License 
//  along with this program.  If not, see https://www.gnu.org/licenses/.

using Microsoft.MetadirectoryServices;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Vigo.Bas.Agresso.WebServices.UserAdministrationV200702;
using Vigo.Bas.ManagementAgent.Log;
using Vigo.Bas.Common;

namespace Vigo.Bas.Agresso.WebServices
{
    public class UserAdministration
    {
        public UserAdministration()
        {
        }

        #region Resource Registry methods
        public Resource[] GetResources(string client,
                                string resourceId,
                                DateTime dateFrom,
                                DateTime dateTo,
                                WSCredentials credentials,
                                string serviceUrl)
        {
            using (var agressoClient = GetAgressoClient(serviceUrl))
            {
                try
                {
                    var resources = agressoClient.GetResources(client, resourceId, dateFrom, dateTo, credentials);
                    var noResources = resources.Length;
                    Logger.Log.InfoFormat("GetResources returned {0} resources", noResources.ToString());
                    return resources;
                }
                catch (Exception ex)
                {
                    Logger.Log.ErrorFormat("Error in GetResources: {0}", ex.Message);
                    throw;
                }
            }
        }

        public WorkPlace1[] GetWorkplaces(string client,
                        WSCredentials credentials,
                        string serviceUrl)
        {
            using (var agressoClient = GetAgressoClient(serviceUrl))
            {
                try
                {
                    var workplaces = agressoClient.GetWorkplaces(client, credentials);
                    var noWorkplaces = workplaces.Length;
                    Logger.Log.InfoFormat("GetWorkplaces returned {0} workplaces", noWorkplaces.ToString());
                    return workplaces;
                }
                catch (Exception ex)
                {
                    Logger.Log.ErrorFormat("Error in GetWorkplaces: {0}", ex.Message);
                    throw;
                }
            }
        }

        public Organization[] GetOrganization(string client,
                WSCredentials credentials,
                string serviceUrl)
        {
            using (var agressoClient = GetAgressoClient(serviceUrl))
            {
                try
                {
                    var organization = agressoClient.GetOrganization(client, credentials);
                    var noWorkplaces = organization.Length;
                    Logger.Log.InfoFormat("GetOrganization returned {0} organizations", noWorkplaces.ToString());
                    return organization;
                }
                catch (Exception ex)
                {
                    Logger.Log.ErrorFormat("Error in GetOrganization: {0}", ex.Message);
                    throw;
                }
            }
        }

        public Response ModifyResources(Resource[] resources, WSCredentials credentials, string serviceUrl)
        {
            using (var agressoClient = GetAgressoClient(serviceUrl))
            {
                try
                {
                    return agressoClient.ModifyResources(resources, credentials);
                }
                catch (Exception ex)
                {
                    Logger.Log.ErrorFormat("Error in ModifyResources: {0}", ex.Message);
                    throw;
                }
            }
        }

        #endregion

        #region User Registry methods
        public User[] GetUsersByUserId(ArrayOfString userIds, WSCredentials wsCredentials, string serviceUrl)
        {
            var userIdList = WSObjectToXML(userIds);
            Logger.Log.DebugFormat("GetUsersByUserId called with userIds: {0}", userIdList);
            try
            {
                using (var client = GetAgressoClient(serviceUrl))
                {
                    var response = client.GetUsersByUserId(userIds, wsCredentials);
                    var responseXml = WSObjectToXML(response);
                    Logger.Log.DebugFormat("GetUsersByUserId responded: {0}", responseXml);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("GetUsersByUserId failed with error: {0}", ex.Message);
                throw;
            }
        }

        public void CreateUsers(User[] users, bool rollbackEnabled, WSCredentials wsCredentials, string serviceUrl)
        {
            var userList = WSObjectToXML(users);
            Logger.Log.DebugFormat("CreateUsers called with rollbackEnabled: {0} for users: {1}", rollbackEnabled, userList);
            try
            {
                using (var client = GetAgressoClient(serviceUrl))
                {
                    var response = client.CreateUsers(users, rollbackEnabled, wsCredentials);
                    var responseXml = WSObjectToXML(response);
                    Logger.Log.DebugFormat("CreateUsers responded: {0}", responseXml);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("CreateUsers failed with error: {0}", ex.Message);
                throw;
            }
        }

        public void ModifyUsers(User[] users, 
                                bool rollbackEnabled, 
                                WSCredentials wsCredentials, 
                                string serviceUrl)
        {
            var userList = WSObjectToXML(users);
            Logger.Log.DebugFormat("ModifyUsers called with rollbackEnabled: {0} for users: {1}", rollbackEnabled, userList);
            try
            {
                using (var client = GetAgressoClient(serviceUrl))
                {
                    var response = client.ModifyUsers(users, rollbackEnabled, wsCredentials);
                    var responseXml = WSObjectToXML(response);
                    Logger.Log.DebugFormat("ModifyUsers responded: {0}", responseXml);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("ModifyUsers failed with error: {0}", ex.Message);
                throw;
            }
        }

        public User GetUser(string userId, WSCredentials wsCredentials, string serviceUrl)
        {
            Logger.Log.DebugFormat("GetUser called with userId: {0}", userId);
            try
            {
                using (var client = GetAgressoClient(serviceUrl))
                {
                    var response = client.GetUser(userId, wsCredentials);
                    var responseXml = WSObjectToXML(response);
                    Logger.Log.DebugFormat("GetUser responded: {0}", responseXml);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("GetUser failed with error: {0}", ex.Message);
                throw;
            }
        }

        public User[] GetUsers(string userId,
                                string userName,
                                string roleId,
                                string client,
                                bool activeOnly,
                                WSCredentials wsCredentials,
                                string serviceUrl)
        {
            Logger.Log.DebugFormat("GetUsers called with parameters userId: {0}, userName: {1}, roleId: {2}, client: {3}, activeOnly: {4}", userId, userName, roleId, client, activeOnly);
            try
            {
                using (var soapClient = GetAgressoClient(serviceUrl))
                {
                    var response = soapClient.GetUsers(userId, userName, roleId, client, activeOnly, wsCredentials);
                    string responseString = "";
                    if (response != null)
                    {
                        responseString = WSObjectToXML(response);
                    }
                    else
                    {
                        responseString = "Empty response";
                    }
                    Logger.Log.DebugFormat("GetUsers responded: {0}", responseString);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("GetUsers failed with error: {0}", ex.Message);
                throw;
            }
        }
        #endregion

        #region Private methods
        private UserAdministrationV200702SoapClient GetAgressoClient(string httpAddress)
        {
            Logger.Log.InfoFormat("GetAgressoClient called with parameter httpAddress = {0}", httpAddress);
            var endpointAddress = new EndpointAddress(httpAddress);
            try
            {
                string filename = Path.Combine(MAUtils.MAFolder, "webservice.config");
                Binding fileConfigBinding = null;

                if (File.Exists(filename))
                    fileConfigBinding = WebserviceUtil.ResolveBinding("agressoMA", filename);

                UserAdministrationV200702SoapClient agressoClient;

                if (fileConfigBinding != null)
                {
                    agressoClient = new UserAdministrationV200702SoapClient(fileConfigBinding, endpointAddress);
                    Logger.Log.Debug("Using webservice binding from webservice.config");
                }
                else
                {
                    BasicHttpBinding binding = new BasicHttpBinding();
                    binding.MaxReceivedMessageSize = 2147483647;
                    binding.SendTimeout = TimeSpan.Parse("00:10:00");
                    string protocol = httpAddress.Substring(0, httpAddress.IndexOf(":"));
                    if (protocol == "https")
                    {
                        binding.Security.Mode = BasicHttpSecurityMode.Transport;
                    }
                    agressoClient = new UserAdministrationV200702SoapClient(binding, endpointAddress);
                }
                return agressoClient;
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("GetAgressoClient failed with error: {0}", ex.Message);
                throw;
            }
        }

        private string GetResponseItem(object response)
        {
            Logger.Log.Debug("GetResponseItem is called");
            string filter = "/response/*";
            return GetXMLNodesFromWSObject(response, filter);
        }

        private string WSObjectToXML(object obj)
        {
            string filter = "/";
            String returnStr = "";
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            StringBuilder result = new StringBuilder();
            XmlDocument xDoc = new XmlDocument();
            using (var writer = XmlWriter.Create(result))
            {
                serializer.Serialize(writer, obj);
            }
            xDoc.LoadXml(result.ToString());

            XmlNodeList nodelist = xDoc.SelectNodes(filter);

            if (nodelist != null)
            {
                foreach (XmlNode node in nodelist)
                {
                    returnStr += node.OuterXml;
                }
            }
            return returnStr;
        }

        private string GetXMLNodesFromWSObject(object obj, string filter)
        {
            Logger.Log.DebugFormat("GetXMLNodesFromWSObject is called with filter {0}", filter);
            String returnStr = "";
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                StringBuilder result = new StringBuilder();
                XmlDocument xDoc = new XmlDocument();
                using (var writer = XmlWriter.Create(result))
                {
                    serializer.Serialize(writer, obj);
                }
                xDoc.LoadXml(result.ToString());

                XmlNodeList nodelist = xDoc.SelectNodes(filter);

                if (nodelist != null)
                {
                    foreach (XmlNode node in nodelist)
                    {
                        returnStr += node.OuterXml;
                    }
                }
                return returnStr;
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("GetXMLNodesFromWSObject failed with error: {0}", ex.Message);
                throw;
            }
        }

        public WSCredentials GetCredentials(KeyedCollection<string, ConfigParameter> configParameters)
        {
            WSCredentials credentials = new WSCredentials();

            if (configParameters["Username"].Value != null)
            {
                credentials.Username = configParameters["Username"].Value.Trim();
            }
            if (configParameters["Password"].SecureValue != null)
            {
                credentials.Password = Decrypt(configParameters["Password"].SecureValue);
            }
            if (configParameters["Client"].Value != null)
            {
                credentials.Client = configParameters["Client"].Value.Trim();
            }
            return credentials;
        }

        private string Decrypt(SecureString inStr)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(inStr);
            string decrString = Marshal.PtrToStringUni(ptr);
            return decrString;
        }
        #endregion
    }
}
