/*
RevCommServer - Communications Hub for RevCommSuite API
 MIT License

 Copyright (c) 2025 RevComGaming

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE. */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Xml;
using System.Data;
using System.Runtime.InteropServices;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace RevelationsStudios.RevCommServer {

	/// <summary>
	/// 	Handles Communication Settings
	/// </summary>
//	[System.Runtime.InteropServices.Guid("B6E1AE1F-D3C4-4f38-A2F7-C659F0CC2D05")]
	public static class ServerApp {

        private static ServerSettings ssConfig = null;
                                    /* Configuration Settings */
        private static TcpListener tlServer = null;   
                                    /* TCP Server Connection Listener */
        private static Communicator cmClientComm;   
                                    /* Client Communcations */
        private enum COMMANDS { SENDMSGUSER, SENDFILEUSER, SENDMSG, SENDFILE, REGISTERFILE, PEERTOPEERDISCONNECT,
                                REGISTERDATAQUERY, REGISTERDATASTATEMENT, REGISTERDATAEVENT, REGISTERDATAMAP, RUNDATAOPERATION,
                                REMOVEDATAOPERATION, STARTGROUP, JOINGROUP, LEAVEGROUP, CLOSEGROUP, LOG };
									/* Type of Server Commands */

        public static void Main(string[] astrArgs) {
			
			string strConfigFilePath = "RevCommConfig.xml";
									/* Path to XML Config File */

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            try {
                
				if (astrArgs.Length >= 1) {
											
					strConfigFilePath = astrArgs[0];
				}
                                    	
				ssConfig = new ServerSettings(strConfigFilePath);

                /* Attempt to Setup a Listener on the Selected IP Address. If it Fails, the Try/Catch Will Setup it up to Try the Next One */
                tlServer = new TcpListener(IPAddress.Parse(ssConfig.Host), ssConfig.Port);
                tlServer.Start();
                
                Log("RevCommServer - Copyright (c) 2025 RevComGaming");
                Log("This program comes with ABSOLUTELY NO WARRANTY");
                Log("This is free software, and you are welcome to redistribute it under certain conditions.");
                Log("For more information, go to http://www.gnu.org/licenses/");

                Log("Action: Application started successfully.");

                Log("Action: Listener setup successfully.");

                cmClientComm = new Communicator();
                
                new Thread(new ThreadStart(cmClientComm.ManageClients)).Start();

                Log("Action: Client communications setup successfully.");
                
                new Thread(new ThreadStart(RunCommands)).Start();

                Log("Action: Command receiver setup successfully.");

                /* Cycle While Application Runs and Get Connections for Clients */
                while (ssConfig.Running) {

                    try {
                        
                        cmClientComm.AddClient(tlServer.AcceptTcpClient());
                    }
                    catch (Exception exError) {
                
                        Log("Action: Running client connection check loop.", exError);
                    }
                }

                Log("Action: Listener closed successfully.");

                ssConfig.UDPSecureFree();
            }
            catch (Exception exError) {
                                    	
                if (tlServer != null) {

                    tlServer.Stop();
                }

                if (ssConfig != null) { 

                    Log("Action: Running client connection check loop.", exError);
                }
                else { 
                
                    Console.WriteLine("Action: Running client connection check loop. Error: Failure during inital startup.");
                }
            }
        }

		/// <summary>
		/// 	Runs Server Commands
		/// </summary>
		private static void RunCommands() {
										
			List<string> ltstrCmds; /* List of Server Commands */
            string[] astrCmdInfo;	/* Information on Selected Command */
            string strCmdMsgStart = ssConfig.CmdMsgStartChars,
                   strCmdMsgEnd = ssConfig.CmdMsgEndChars,
                   strCmdMsgPart = ssConfig.CmdMsgPartEndChars;
                                    /* String Version of Indicator Command Message Start, End, and Part */
            int nMsgStartIndex = 0; /* Index of Actual Start of Message */
            StringSplitOptions ssoRemove = StringSplitOptions.RemoveEmptyEntries;
                                    /* Option for Splitting Strings */
            string[] aCmdPartEndChars = new string[] { strCmdMsgPart };
                                    /* Array of Command Message to Part End Indicator Characters */
//            Dictionary<string, string> dictstrDataParams = new Dictionary<string, string>();
                                    /* Holder for Parameters for Running Data Statements */
            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                    /* Manage Stopping Thread */
//            int nParamCount = 0,    /* Parameter Count */
//                nCounter = 0;       /* Counter for Loop */

            try {

                while (ssConfig.Running) {

                    ltstrCmds = ssConfig.ServerCmds;

			        foreach (string strCmdSelect in ltstrCmds) {

                        if (strCmdSelect.Contains(strCmdMsgPart)) {

                            nMsgStartIndex = strCmdSelect.IndexOf(strCmdMsgStart) + strCmdMsgStart.Length;

                            astrCmdInfo = strCmdSelect.Substring(nMsgStartIndex, strCmdSelect.LastIndexOf(strCmdMsgEnd) - nMsgStartIndex).Split(aCmdPartEndChars, ssoRemove);

                            lock (cmClientComm) { 

				                switch ((COMMANDS)Enum.Parse(typeof(COMMANDS), astrCmdInfo[0], true)) {

                                    case COMMANDS.SENDMSGUSER: {

                                        try {
                                                
                                            if (astrCmdInfo.Length == 3) {

                                                cmClientComm.Send(Convert.ToInt32(astrCmdInfo[1]), astrCmdInfo[2], "");
                                            }
                                            else if (astrCmdInfo.Length > 3) {

                                                cmClientComm.Send(Convert.ToInt32(astrCmdInfo[1]), astrCmdInfo[2], astrCmdInfo[3]);
                                            }        
                                        }
                                        catch (Exception exError) {

                                            Log("Action: Executing server command for sending message to client ID, " + astrCmdInfo[1] + ".", exError);
                                        }

                                        break;
                                    }	
					                case COMMANDS.SENDFILEUSER: {

                                        try {

                                            cmClientComm.SendFile(Convert.ToInt32(astrCmdInfo[1]), astrCmdInfo[2]);
                                        }
                                        catch (Exception exError) {

                                            Log("Action: Executing server command for sending file to client ID, " + astrCmdInfo[1] + ".", exError);
                                        }
						
						                break;
					                }		
					                case COMMANDS.SENDMSG: {

                                        cmClientComm.SendAll(astrCmdInfo[1]);
						                break;
					                }
					                case COMMANDS.SENDFILE: {

                                        cmClientComm.SendFileAll(astrCmdInfo[1]);
						                break;
					                }
                                    case COMMANDS.REGISTERFILE: {
							
                                        ssConfig.AddDownloadFile(astrCmdInfo[1], astrCmdInfo[2]);
						                break;
					                }
                                    case COMMANDS.REGISTERDATAQUERY: {
							
                                        ssConfig.AddDataExecutionQuery(astrCmdInfo[1], astrCmdInfo[2], astrCmdInfo[3]);
						                break;
					                }
                                    case COMMANDS.REGISTERDATASTATEMENT: {
                                            
                                        ssConfig.AddDataExecutionStatement(astrCmdInfo[1], astrCmdInfo[2], astrCmdInfo[3]);
                                        break;
					                }
                                    case COMMANDS.REGISTERDATAEVENT: {

                                        try {

                                            ssConfig.AddDataExecutionEvent(astrCmdInfo[1], 
                                                                           astrCmdInfo[2], 
                                                                           astrCmdInfo[3], 
                                                                           Convert.ToInt32(astrCmdInfo[4]), 
                                                                           astrCmdInfo[5],
                                                                           Convert.ToInt32(astrCmdInfo[6]));
                                        }
                                        catch (Exception exError) {

                                            Log("Action: Executing server command for registering database events for database designation: " + 
                                                astrCmdInfo[1] + ", data designation: " + astrCmdInfo[2] + ".", exError);
                                        }

                                        break;
					                }
                                    case COMMANDS.REGISTERDATAMAP: {

                                        try {
                                                
                                            ssConfig.AddDataMap(astrCmdInfo[1], 
                                                                astrCmdInfo[2], 
                                                                astrCmdInfo[3], 
                                                                astrCmdInfo[4], 
                                                                Convert.ToBoolean(astrCmdInfo[5]), 
                                                                Convert.ToInt32(astrCmdInfo[6]));
                                        }
                                        catch (Exception exError) {

                                            Log("Action: Executing server command for registering database map for database designation: " + astrCmdInfo[1] + ", data designation: " + astrCmdInfo[2] + ".", exError);
                                        }

                                        break;
					                }
                                    case COMMANDS.RUNDATAOPERATION: {

                                        Dictionary<string, string> dictstrDataParams = null;
                                        int nParamCount = astrCmdInfo.Length - 2,
                                            nCounter = 0;

                                        if (nParamCount > 1) {

                                            dictstrDataParams = new Dictionary<string, string>();

                                            for (nCounter = 2; nCounter < nParamCount; nCounter++) {

                                                if (nCounter + 1 < nParamCount) {

                                                    dictstrDataParams.Add(astrCmdInfo[nCounter], astrCmdInfo[nCounter + 1]);
                                                }
                                                else {

                                                    dictstrDataParams.Add(astrCmdInfo[nCounter], "");
                                                }
                                            }
                                        }

                                        try { 

                                            ssConfig.RunDataExecution(astrCmdInfo[1], dictstrDataParams);
                                        }
                                        catch (Exception exError) {

                                            Log("Action: Executing server command for registering database events for data designation, '" + astrCmdInfo[1] + "'.", exError);
                                        }

                                        break;
					                }
                                    case COMMANDS.REMOVEDATAOPERATION: {
                                            
                                        ssConfig.RemoveDataExecution(astrCmdInfo[1], astrCmdInfo[2]);
                                        break;
					                }
                                    case COMMANDS.PEERTOPEERDISCONNECT: {

                                        cmClientComm.PeerToPeerDisconnect();
						                break;
					                }
                                    case COMMANDS.STARTGROUP: {

                                        cmClientComm.CreateGroup(Convert.ToInt32(astrCmdInfo[1]));
						                break;
					                }
                                    case COMMANDS.JOINGROUP: {

                                        cmClientComm.AddToGroup(Convert.ToInt32(astrCmdInfo[1]), astrCmdInfo[2]);
                                        break;
					                }
                                    case COMMANDS.LEAVEGROUP: {

                                        cmClientComm.RemoveFromGroup(Convert.ToInt32(astrCmdInfo[1]));
						                break;
					                }
                                    case COMMANDS.CLOSEGROUP: {
                                            
                                        cmClientComm.CloseGroup(astrCmdInfo[1]);
						                break;
					                }
                                    case COMMANDS.LOG: {
                    
                                        Log(astrCmdInfo[1]);
                                        break;
                                    }
                                    default: {
                    
                                        Log("Action: Executing server command. Error: Invalid command. Message: " + strCmdSelect);
                                        break;
                                    }
                                }
                            }
				        }
                        else {

                            Log("Action: Executing server command. Error: Invalid command message due to missing message part separator. Message: " + strCmdSelect);
                        }
			        }

                    areThreadStopper.WaitOne(1);
                }

                Log("Action: Command receiver stopped successfully.");
            }
            catch (Exception exError) {
            
                Log("Action: Running server command loop.", exError);
            }
		}

        /// <summary>
        ///     Gets List of Server Commands
        /// </summary>
        public static string[] Commands {

            get {

                return Enum.GetNames(typeof(COMMANDS));
            }
        }

        /// <summary>
        /// 	Logs Messages
        /// </summary>
        /// <param name="strLogMsg">Messages to Log</param>
        /// <param name="exSentError">Exception Information</param>
        public static void Log(string strLogMsg, Exception exSentError = null) {
		
			DateTime dtNow = DateTime.Now;	
									/* Current Time */
			string strLogFilePath = "logs/",
				   strLogFileName = "",
				   				 /* Log File Path and File Name */
                   strExceptDetails = "";
                                 /* Details from Exception */
			FileStream fsLogFile = null;
								 /* Access to Log File */
/*			byte[] abyteLogMsg;	 /* Holder for Log Message Being Put into File */
			
			try {

                if (ssConfig != null) { 

                    strLogFilePath = ssConfig.LogFilePath;
                }
			
				/* Add Date to Log File Before Showing */
				strLogMsg = dtNow.ToString("MM/dd/yyyy") + " " + dtNow.ToString("hh:mm:ss.ffff") + ": " + strLogMsg;

                if (exSentError != null) {

                    strExceptDetails = exSentError.InnerException.Message;

                    if (strExceptDetails != "") {

                        strExceptDetails += " Details: " + strExceptDetails + ". ";
                    }

                    strLogMsg += " Exception: " + exSentError.Message + ". " + strExceptDetails + "Stacktrace: " + 
                                 exSentError.StackTrace;
                }
					   				 
				Console.WriteLine(strLogMsg);
				
				if (ssConfig != null && ssConfig.UseLogFile) {
				
					/* If the Log File Has a Directory, Make Sure it Exists, Else Create it, Add Path to File Name */
					if (strLogFilePath != "") {
						
						if (!Directory.Exists(strLogFilePath)) {
						
							Directory.CreateDirectory(strLogFilePath);
						}
						
						strLogFileName = strLogFilePath;
					}
					
					/* Append Message to File, If it Doesn't Exist, Create it */
					strLogFileName += "RevCommServer-" + dtNow.ToString("yyyyMMdd") + ".txt";
	
					fsLogFile = File.Open(strLogFileName, FileMode.Append);

                    lock (fsLogFile) {

                        byte[] abyteLogMsg = new System.Text.UTF8Encoding(true).GetBytes(strLogMsg + "\r\n");

                        fsLogFile.Write(abyteLogMsg, 0, abyteLogMsg.Length);
                        fsLogFile.Close();
                    }
				}
			}
            catch (Exception exError) {
                                    
				Console.WriteLine("Action: Logging message: '" + strLogMsg + "'. Exception: " + exError.Message);
				
				if (fsLogFile != null) {
				
					fsLogFile.Close();
				}
            }
		}		
		
       /// <summary>
       ///    Gets Server Settings
       /// </summary>
       internal static ServerSettings Settings {
        
            get {

                return ssConfig;
            }
        }
	}

    /// <summary>
    ///     Client Communications Object
    /// </summary>
    internal class Communicator {

        private ServerSettings ssConfig = ServerApp.Settings;
                                    /* Configuration Settings */
        private LinkedList<Client> llcntClientList = new LinkedList<Client>();
                                    /* List of Client Information */
        private LinkedList<ClientGroup> llcgGroupList = new LinkedList<ClientGroup>();
                                    /* List of Client in Groups Information */
        private bool boolSendMsgBackSender = ServerApp.Settings.DirectMsgBackSender;
                                    /* Indicator to Send Messages Back to Sender */
		private int nRandSeed = new Random().Next();
									/* Seed for Generating Transaction IDs */
                                    
        /// <summary>
        ///     Processes Messages From Connected Clients
        /// </summary>
        public void ManageClients() {

            LinkedListNode<ClientGroup> llncgSelect = null;
                                    /* Selected Holder for List of Client Group's Information */
            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                    /* Manage Stopping Thread */

            /* Continuely Loop and Process Any Client Streams and Pass Any Messages to Other Clients */
            try {

                while (ssConfig.Running) {

                    lock (llcntClientList) {

                        if (llcntClientList.Count > 0) {

                            ManageMessages(llcntClientList);
                        }
                        else {

                            /* No Clients Connected, Stop Data Map Processing */
                            ServerApp.Settings.PauseAllDataMaps();
                        }
                    }

                    lock (llcgGroupList) {

                        if (llcgGroupList.Count > 0) {

                            llncgSelect = llcgGroupList.First;

                            while (llncgSelect != null) {

                                ManageMessages(llncgSelect.Value.Clients);
                                llncgSelect.Value.ManagePingCheck();
                                llncgSelect = llncgSelect.Next;
                            }
                        }
                    }

                    areThreadStopper.WaitOne(1);
                }

                ServerApp.Log("Action: Client communications close successfully.");
            }
            catch (Exception exError) {

                ServerApp.Log("Action: Managing client operations.", exError);
            }
        }

        /// <summary>
        ///     Processes Messages From Connected Clients
        /// </summary>
        private void ManageMessages(LinkedList<Client> llcntClients) {
            
            LinkedListNode<Client> llncntSelect = llcntClients.First;
                                    /* Selected Holder for Client's Information */
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */
            string strMsg = "";     /* Message from Client */
            int nSelectClientID = 0;/* Selected Client's ID */
            LinkedListNode<Client> llncntSend = null;
                                    /* Selected Holder for Client to Send Message */
            Client cntSendClient = null,
                                    /* Selected Client to Send Message */
                   cntRemoveClient = null;
                                    /* Selected Client to be Removed from Session Groups */
            LinkedListNode<Client> llncntRemove = null;
                                    /* Selected Holder for Client to Be Removed */
            LinkedListNode<ClientGroup> llncgChecked = null;
                                    /* Holder for Selected Client Group for Removing Disconnected Client */
            ClientGroup cngSelect = null;
                                    /* Selected Client Group */

            /* Continuely Loop and Process Any Client Streams and Pass Any Messages to Other Clients */
            try {

                while (llncntSelect != null) {

                    cntClientSelect = llncntSelect.Value;
                    cntClientSelect.GetStreamMsgs();

                    strMsg = cntClientSelect.GetNextMessage();

                    if (strMsg != "") {

                        nSelectClientID = cntClientSelect.ID;
                        llncntSend = llcntClients.First;

                        while (llncntSend != null) {

                            cntSendClient = llncntSend.Value;

                            if (cntSendClient.Connected && (boolSendMsgBackSender || cntSendClient.ID != nSelectClientID)) {

                                cntSendClient.SendTransfer(strMsg);
                            }

                            llncntSend = llncntSend.Next;
                        }
                    }
                    else if (!cntClientSelect.Connected) {

                        llncntRemove = llncntSelect;
                    }

                    llncntSelect = llncntSelect.Next;

                    if (llncntRemove != null) {
                    
                        llcntClients.Remove(llncntRemove);

                        cntRemoveClient = llncntRemove.Value;
                        llncgChecked = llcgGroupList.First;

                        while (llncgChecked != null) {

                            cngSelect = llncgChecked.Value;

                            if (cngSelect.HasClient(cntRemoveClient.ID)) {

                                RemoveFromGroup(cntRemoveClient.ID);
                            }

                            llncgChecked = llncgChecked.Next;
                        }

                        llncntRemove = null;
                    }
                }
            }
            catch (Exception exError) {

                ServerApp.Log("Action: Managing messaging operations.", exError);
            }
        }

        /// <summary>
        ///     Sends Message to a Clients
        /// </summary>
        /// <param name="nClientID">ID of Client to Send Message to</param>
        /// <param name="strMsg">Message to Send to Clients</param>
        /// <param name="strMsgDesign">Optional Message Designation</param>
        public void Send(int nClientID, string strMsg, string strMsgDesign = "") {
            
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */

            if (strMsg != "") {

                cntClientSelect = FindClient(nClientID);
                        
                if (cntClientSelect != null &&
                    cntClientSelect.Connected) {
                        
                    cntClientSelect.SendDirectMsg(strMsg, strMsgDesign);
                }
                else if (cntClientSelect == null) {

                    ServerApp.Log("Action: Sending to client, ID: " + nClientID + ". Send failed. Message: " + strMsg + ". Error: Client not found.");
                }
                else {

                    ServerApp.Log("Action: Sending to client, ID: " + nClientID + ". Send failed. Message: " + strMsg + ". Error: Client not connected.");
                }
            }
            else {

                ServerApp.Log("Action: Sending to client, ID: " + nClientID + ". Send failed. Message: " + strMsg + ". Error: Message blank or no clients have connected.");
            }
        }
        
        /// <summary>
        ///     Sends File to All Clients
        /// </summary>
        /// <param name="nClientID">ID of Client to Send to File to</param>
        /// <param name="strFileDesign">Designation of File to Send</param>
        public void SendFile(int nClientID, string strFileDesign) {
        	
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */
                                    
            if (strFileDesign != "") {

                cntClientSelect = FindClient(nClientID);

                if (cntClientSelect != null && 
                    cntClientSelect.Connected &&
                    !cntClientSelect.SendFile(strFileDesign)) {

                    ServerApp.Log("Action: Sending to file to client, ID: " + nClientID + ". Send failed. File Designation: " + strFileDesign);
                }
            }
        }

        /// <summary>
        ///     Sends Message to All Clients
        /// </summary>
        /// <param name="strMsg">Message to Send to Clients</param>
        public void SendAll(string strMsg) {

            LinkedListNode<Client> llncntSelect = null;
                                    /* Selected Client's Information */
            LinkedListNode<ClientGroup> llncgGroupSelect = null;
                                    /* Selected Client Group's Information */
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */

            if (strMsg != "") {

                if (llcntClientList.Count > 0) { 

                    llncntSelect = llcntClientList.First;

                    while (llncntSelect != null) {

                        cntClientSelect = llncntSelect.Value;

                        if (cntClientSelect.Connected) {

                            cntClientSelect.SendDirectMsg(strMsg);
                        }

                        llncntSelect = llncntSelect.Next;
                    }
                }

                if (llcgGroupList.Count > 0) {

                    llncgGroupSelect = llcgGroupList.First;

                    while (llncgGroupSelect != null) {

                        llncntSelect = llncgGroupSelect.Value.Clients.First;

                        while (llncntSelect != null) {
                    
                            cntClientSelect = llncntSelect.Value;

                            if (cntClientSelect.Connected) {

                                cntClientSelect.SendDirectMsg(strMsg);
                            }

                            llncntSelect = llncntSelect.Next;
                        }

                        llncgGroupSelect = llncgGroupSelect.Next;
                    }
                }
            }
        }
        
        /// <summary>
        ///     Sends File to All Clients
        /// </summary>
        /// <param name="strFileDesign">Designation of File to Send</param>
        public void SendFileAll(string strFileDesign) {
        	
            LinkedListNode<Client> llncntSelect = null;
                                    /* Selected Client's Information */
            LinkedListNode<ClientGroup> llncgGroupSelect = null;
                                    /* Selected Client Group's Information */
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */
                                    
            if (strFileDesign != "") {

                if (llcntClientList.Count > 0) { 

                    llncntSelect = llcntClientList.First;

                    while (llncntSelect != null) {

                        cntClientSelect = llncntSelect.Value;

                        if (cntClientSelect.Connected && !cntClientSelect.SendFile(strFileDesign)) {

                            ServerApp.Log("Action: Sending to file to all clients. Send failed. File Designation: " + strFileDesign);
                        }

                        llncntSelect = llncntSelect.Next;
                    }
                }

                if (llcgGroupList.Count > 0) {

                    llncgGroupSelect = llcgGroupList.First;

                    while (llncgGroupSelect != null) {

                        llncntSelect = llncgGroupSelect.Value.Clients.First;

                        while (llncntSelect != null) {
                    
                            cntClientSelect = llncntSelect.Value;

                            if (cntClientSelect.Connected) {
                                
                                if (cntClientSelect.Connected && !cntClientSelect.SendFile(strFileDesign)) {

                                    ServerApp.Log("Action: Sending to file to all clients. Send failed. File Designation: " + strFileDesign);
                                }
                            }

                            llncntSelect = llncntSelect.Next;
                        }

                        llncgGroupSelect = llncgGroupSelect.Next;
                    }
                }
            }
        }

        /// <summary>
        ///     Add Client to Communications
        /// </summary>
        /// <param name="tcSetConnection">New Client's Client Connection Information</param>
        public void AddClient(TcpClient tcSetConnection) { 
        
			int nNewClientID = new Random(nRandSeed++).Next();
									/* New Client ID */ 
            LinkedListNode<Client> llncntSelect = llcntClientList.First;
                                    /* Selected Client's Information */
            Client cntNew,          /* New Client */ 
                   cntSelect;       /* Selected Client Information */
            string strIP = ((IPEndPoint)tcSetConnection.Client.RemoteEndPoint).Address.ToString();
                                    /* Client IP Address */
            int nPort = ((IPEndPoint)tcSetConnection.Client.RemoteEndPoint).Port;
                                    /* Client's Port */
/*           string strNewIP = cntNew.IP;
                                    /* New Client's IP Address */
/*           byte[] abyteNewEncryptKey = cntNew.EncryptKey,
                    abyteNewEncryptIV = cntNew.EncryptIV;
                                    /* New Client's Encryption Information */

            /* If Client ID Already Exists, Keep Creating Them Until One is Unique */
            while (llncntSelect != null) {

                cntSelect = llncntSelect.Value;

                if (cntSelect.ID != nNewClientID) {

                    llncntSelect = llncntSelect.Next;
                }
				else {

                    nNewClientID = new Random(nRandSeed++).Next();	
					llncntSelect = llcntClientList.First;
				}
			}

            tcSetConnection.NoDelay = true;

            cntNew = new Client(tcSetConnection, nNewClientID);

            llncntSelect = llcntClientList.First;

            while (llncntSelect != null) {

                cntSelect = llncntSelect.Value;

                if (cntSelect.IP == cntNew.IP && !ssConfig.AllowMultipleSameIP) {

                    cntSelect.Close();
                }

                llncntSelect = llncntSelect.Next;
            }

            /* If Having Client Use "Peer To Peer", Have New Client Connect to Existing */
            if (ServerApp.Settings.ClientPeerToPeer) {

                llncntSelect = llcntClientList.First;

                while (llncntSelect != null) {

                    cntSelect = llncntSelect.Value;

                    if (cntSelect.Connected) {

                        cntNew.AddPeerToPeerCandidate(cntSelect);
                    }

                    llncntSelect = llncntSelect.Next;	
			    }
            }

            llcntClientList.AddLast(cntNew);

            ServerApp.Log("Client connected, ID: " + nNewClientID.ToString());
        }

        /// <summary>
        ///     Add Client to Communications
        /// </summary>
        /// <param name="socClient">New Client's Socket Connection Information</param>
        public void AddClient(Socket socClient) { 

            TcpClient tcConnection = new TcpClient();
                                    /* New Client's Client Connection Information */

            tcConnection.Client = socClient;

            AddClient(tcConnection);
        }

        /// <summary>
        ///     Creates Client Group
        /// </summary>
        /// <param name="nClientCreatorID">ID for Group Creating Client</param>
        public void CreateGroup(int nClientCreatorID) {
            
            Client cntClientSelect = FindClient(nClientCreatorID, false);
                                    /* Selected Client Information */

            if (cntClientSelect != null) {

                if (llcntClientList.Remove(cntClientSelect)) {

                    llcgGroupList.AddLast(new ClientGroup(cntClientSelect));
                    
                    ServerApp.Log("Group started, ID: " + cntClientSelect.GroupID + ", by client ID: " + nClientCreatorID.ToString() + ".");
                }
                else {

                    ServerApp.Log("Action: Creating group for client, ID: " + nClientCreatorID.ToString() + " failed. Error: Client could not be removed from main pool.");
                }
            }
            else {

                ServerApp.Log("Action: Creating group for client, ID: " + nClientCreatorID.ToString() + " failed. Error: Client not found or was already in a group.");
            }
        }

        /// <summary>
        ///     Add Client to Group
        /// </summary>
        /// <param name="nClientID">ID for Client to Add to Group</param>
        /// <param name="strGroupID">Group ID</param>
        public void AddToGroup(int nClientID, string strGroupID) {
            
            Client cntClientSelect = FindClient(nClientID, false);
                                    /* Selected Client Information */
            ClientGroup cgGroupSelect = FindGroup(strGroupID);
                                    /* Selected Group Information */

            if (cntClientSelect != null && 
                cgGroupSelect != null) {

                if (llcntClientList.Remove(cntClientSelect)) {

                    if (cgGroupSelect.AddClient(cntClientSelect)) {
                        
                        ServerApp.Log("Group, ID: " + cntClientSelect.GroupID + ", joined by client ID: " + nClientID.ToString() + ".");
                    }
                    else { 
                        
                        ServerApp.Log("Action: Adding client, ID: " + nClientID.ToString() + ", to group ID: '" + strGroupID + "' failed. Error: Client could not be added to group information or was already in it.");
                    }
                }
                else {

                    ServerApp.Log("Action: Adding client, ID: " + nClientID.ToString() + ", to group ID: '" + strGroupID + "' failed. Error: Client could not be removed from pool.");
                }
            }
            else if (cgGroupSelect == null) {

                ServerApp.Log("Action: Adding client, ID: " + nClientID.ToString() + ", to group ID: '" + strGroupID + "' failed. Error: Client group could not be found.");
            }
            else {

                ServerApp.Log("Action: Adding client, ID: " + nClientID.ToString() + ", to group ID: '" + strGroupID + "' failed. Error: Client not found or was already in a group.");
            }
        }

        /// <summary>
        ///     Remove Client from Any Group
        /// </summary>
        /// <param name="nClientID">ID for Client to Remove From Group</param>
        public void RemoveFromGroup(int nClientID) {
            
            Client cntClientSelect = FindClient(nClientID);
                                    /* Selected Client Information */
            LinkedListNode<ClientGroup> llncgGroupSelect = llcgGroupList.First;
                                    /* Selected Client Group Information */
            ClientGroup cgSelect = null;
                                    /* Selected Client Group */

            if (cntClientSelect != null) {
                     
                if (llcgGroupList.Count > 0) {

                    while (llncgGroupSelect != null) {

                        cgSelect = llncgGroupSelect.Value;

                        if (cgSelect.HasClient(nClientID)) {
                            
                            llcntClientList.AddLast(cgSelect.RemoveClient(nClientID));
                            ServerApp.Log("Group, ID: " + cgSelect.ID + ", removed client ID: " + nClientID.ToString() + ".");

                            if (cgSelect.Clients.Count <= 0) {

                                llcgGroupList.Remove(cgSelect);
                                ServerApp.Log("Group, ID: " + cgSelect.ID + ", being closed due to no clients.");
                            }

                            break;
                        }

                        llncgGroupSelect = llncgGroupSelect.Next;
                    }
                }
            }
            else {

                ServerApp.Log("Action: Removing client, ID: " + nClientID.ToString() + ", failed. Error: Client not found.");
            }
        }

        /// <summary>
        ///     Close Group
        /// </summary>
        /// <param name="strGroupID">ID of Group to Close</param>
        public void CloseGroup(string strGroupID) {
            
            ClientGroup cgGroupSelect = FindGroup(strGroupID);
                                    /* Selected Group Information */

            if (cgGroupSelect != null) {

                llcgGroupList.Remove(cgGroupSelect);
                cgGroupSelect.Close();

                foreach (Client cntSelect in cgGroupSelect.Clients) {

                    llcntClientList.AddLast(cntSelect);
                }

                ServerApp.Log("Group, ID: " + strGroupID + ", was closed.");
            }
            else {

                ServerApp.Log("Action: Closing group ID: '" + strGroupID + "' failed. Error: Group not found.");
            }
        }

        /// <summary>
        ///     Disconnects Client's Server and "Peer To Peer" Clients
        /// </summary>
        public void PeerToPeerDisconnect() { 
        
           LinkedListNode<Client> llncntSelect = null;
                                    /* Client Information List */
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */

            if (llcntClientList.Count > 0) {

                llncntSelect = llcntClientList.First;

                while (llncntSelect != null) {

                    cntClientSelect = llncntSelect.Value;

                    if (cntClientSelect.Connected) {

                        cntClientSelect.SendPeerToPeerDisconnect();
                    }

                    llncntSelect = llncntSelect.Next;
                }
            }
        }

        /// <summary>
        ///     Find Selected Client Information
        /// </summary>
        /// <param name="nClientID">ID for Selected Client Information</param>
        /// <param name="boolCheckGroups">Indicator to Check in Groups for Client, Default to True</param>
        /// <returns>Selected Client Information If Found, Else NULL</returns>
        private Client FindClient(int nClientID, bool boolCheckGroups = true) {
            
            LinkedListNode<Client> llncntSelect = null;
                                    /* Selected Client's Information */
            LinkedListNode<ClientGroup> llncgGroupSelect = null;
                                    /* Selected Client Group Information */
            Client cntClientSelect = null;    
                                    /* Selected Client's Information */
                                    
            if (llcntClientList.Count > 0) {

                llncntSelect = llcntClientList.First;

                while (llncntSelect != null) {
                    
                    if (llncntSelect.Value.ID == nClientID) {

                        cntClientSelect = llncntSelect.Value;
                    }

                    if (cntClientSelect == null) {

                        llncntSelect = llncntSelect.Next;
                    }
                    else {

                        break;
                    }
                }
            }

            if (llcgGroupList.Count > 0 && 
                cntClientSelect == null &&
                boolCheckGroups) {

                llncgGroupSelect = llcgGroupList.First;

                while (llncgGroupSelect != null && 
                       cntClientSelect == null) {

                    llncntSelect = llncgGroupSelect.Value.Clients.First;

                    while (llncntSelect != null) {
                    
                        if (llncntSelect.Value.ID == nClientID) {

                            cntClientSelect = llncntSelect.Value;
                        }

                        if (cntClientSelect == null) {

                            llncntSelect = llncntSelect.Next;
                        }
                        else {

                            break;
                        }
                    }

                    llncgGroupSelect = llncgGroupSelect.Next;
                }
            }

            return cntClientSelect;
        }

        /// <summary>
        ///     Find Selected Client Group Information
        /// </summary>
        /// <param name="strGroupID">ID for Selected Client Group Information</param>
        /// <returns>Selected Client Group Information If Found, Else NULL</returns>
        private ClientGroup FindGroup(string strGroupID) {
            
            LinkedListNode<ClientGroup> llncgGroupSelect = null;
                                    /* Selected Client Group Information */
            ClientGroup cgGroupSelect = null;    
                                    /* Selected Client Group's Information */
                                    
            if (llcgGroupList.Count > 0) {

                llncgGroupSelect = llcgGroupList.First;

                while (llncgGroupSelect != null &&
                       cgGroupSelect == null) {

                    if (llncgGroupSelect.Value.ID == strGroupID) {

                        cgGroupSelect = llncgGroupSelect.Value;
                        break;
                    }

                    llncgGroupSelect = llncgGroupSelect.Next;
                }
            }

            return cgGroupSelect;
        }
    }

    /// <summary>
    ///     Information for Groups of Clients
    /// </summary>
    internal class ClientGroup {

        string strID = Guid.NewGuid().ToString();
                                    /* Client ID */
        LinkedList<Client> llcntClients = new LinkedList<Client>();
                                    /* List of Group's Clients */
        Client cntHost = null;      /* Hosting Client */
        bool boolHostByPingSpeed = ServerApp.Settings.HostPingCheck,
                                    /* Indicator to Designation Host by Ping Speed Check */
             boolInPingSpeedCheck = false;
                                    /* Indicator of Currently Done a Ping Speed Check */
        DateTime dtPingCheckEnd;    /* Time Speed Ping Speed Check is to End */
        Dictionary<int, long> dictClientPingLengths = new Dictionary<int, long>();
                                    /* Results of Ping Check in Milliseconds */

        /// <param name="cntCreator">Client That Created Group</param>
        public ClientGroup(Client cntCreator) {

            cntCreator.GroupID = strID;
            llcntClients.AddFirst(cntCreator);
            cntHost = cntCreator;
            cntCreator.SendGroupJoinCreatedMsg();
            cntCreator.SendHostNoticeMsg();
        }

        /// <summary>
        ///     Add Client to Group
        /// </summary>
        /// <param name="cntAdd">Client to Add</param>
        /// <returns>True If Client was Added to Group, Else False</returns>
        public bool AddClient(Client cntAdd) {

            bool boolAdded = false; /* Indicator That Client was Added to Group */

            if (!HasClient(cntAdd.ID)) {

                cntAdd.GroupID = strID;
                llcntClients.AddLast(cntAdd);
                cntAdd.SendGroupJoinCreatedMsg();

                if (!boolInPingSpeedCheck) {

                    boolInPingSpeedCheck = true;
                    dtPingCheckEnd = DateTime.Now.AddMilliseconds(ServerApp.Settings.HostPingWait);

                    foreach (Client cntSelect in llcntClients) {

                        cntSelect.SendPing(true);
                    }
                }
                else {

                    cntAdd.SendPing(true);
                }

                boolAdded = true;
            }

            return boolAdded;
        }

        /// <summary>
        ///     Remove Client from Group
        /// </summary>
        /// <param name="nClientID">ID for Client from Group</param>
        /// <returns>Client Information That was Removed If Found, Else NULL</returns>
        public Client RemoveClient(int nClientID) {

            Client cntSelect = GetClient(nClientID);

            if (cntSelect != null) {

                cntSelect.GroupID = "";
                llcntClients.Remove(cntSelect);
                cntSelect.SendGroupExitMsg();

                if (cntSelect == cntHost) {

                    Close();
                }
            }

            return cntSelect;
        }

        /// <summary>
        ///     Manage Ping Check for Host Selection
        /// </summary>
        public void ManagePingCheck() {

//           long lPingCheckSelect = 0;
                                    /* Selected Ping Check Length */
//           int nHostClientID = 0; /* ID of Selected Host Client */ 

            if (boolInPingSpeedCheck) {

                if (dtPingCheckEnd > DateTime.Now) {

                    long lPingCheckSelect = 0;

                    foreach (Client cntSelect in llcntClients) {

                        if ((lPingCheckSelect = cntSelect.LastPingCheckLength) > 0) {

                            if (!dictClientPingLengths.ContainsKey(cntSelect.ID)) {

                                dictClientPingLengths.Add(cntSelect.ID, lPingCheckSelect);
                            }
                            else {

                                dictClientPingLengths[cntSelect.ID] = lPingCheckSelect;
                            }
                        }
                    }   
                }
                else {

                    long lPingCheckSelect = 0;
                    int nHostClientID = 0;

                    foreach (KeyValuePair<int, long> kvpSelect in dictClientPingLengths) {

                        if (lPingCheckSelect == 0 || lPingCheckSelect > kvpSelect.Value) {

                            lPingCheckSelect = kvpSelect.Value;
                            nHostClientID = kvpSelect.Key;
                        }
                    }

                    foreach (Client cntSelect in llcntClients) {

                        if (nHostClientID == cntSelect.ID) {

                            cntHost = cntSelect;
                            cntSelect.SendHostNoticeMsg();
                            break;
                        }
                    }

                    boolInPingSpeedCheck = false;
                    dictClientPingLengths.Clear();
                }
            }
        }

        /// <summary>
        ///     Closes Group
        /// </summary>
        public void Close() { 

            LinkedListNode<Client> llncntSelect = null;
                                    /* Selected Client's Information */
                                    
            if (llcntClients.Count > 0) {

                llncntSelect = llcntClients.First;

                while (llncntSelect != null) {

                    llncntSelect.Value.GroupID = "";
                    llncntSelect.Value.SendGroupExitMsg();
                    llncntSelect = llncntSelect.Next;
                }

                cntHost = null;
            }
        }

        /// <summary>
        ///     Finds If Client Is In Group
        /// </summary>
        /// <param name="nClientID">ID for Client Information</param>
        /// <returns>true If Found, Else Null</returns>
        public bool HasClient(int nClientID) {

            return GetClient(nClientID) != null;
        }

        /// <summary>
        ///     Returns Client Information
        /// </summary>
        /// <param name="nClientID">ID for Client Information</param>
        /// <returns>Client Information If Found, Else NULL</returns>
        public Client GetClient(int nClientID) {

            LinkedListNode<Client> llncntSelect = llcntClients.First;
                                    /* Selected Client Information */
            Client cntFound = null; /* Found Client Information to Return */
                                  
            while (llncntSelect != null && cntFound == null) {

                if (llncntSelect.Value.ID == nClientID) {

                    cntFound = llncntSelect.Value;
                    break;
                }

                llncntSelect = llncntSelect.Next;
            }

            return cntFound;
        }

        /// <summary>
        ///     List of Clients
        /// </summary>
        public LinkedList<Client> Clients {

            get {

                return llcntClients;
            }
        }

        /// <summary>
        ///     Hosting Client 
        /// </summary>
        public Client Host {

            get {

                return cntHost;
            }
        }

        /// <summary>
        ///     Client Group ID
        /// </summary>
        public string ID {

            get {

                return strID;
            }
        }
    }

    /// <summary>
    ///     Client Information
    /// </summary>
    internal class Client {

        private static ServerSettings ssConfig = ServerApp.Settings;
                                    /* Server Settings */
        private int nMaxSendBytes = ssConfig.MaxMsgLen;
                                    /* Maximum and Constant Amount of Bytes in Send Messages */
        private string strMsgPartEndChars = ssConfig.PartEndChars,
                       strMsgStartChars = ssConfig.StartChars,
					   strMsgEndChars = ssConfig.EndChars;
									/* Characters That Symbolize the End of a Part of the Message, its Start, and End of it */
		private char charMsgFiller = ssConfig.Filler;
                                    /* Message Filler */
    	private enum CLIENTOPERATIONS { INVALID, STARTSTREAM, STARTHTTPPOSTASYNC, STARTHTTPPOSTSYNC, STARTHTTPGETASYNC, STARTHTTPGETSYNC, 
                                        DIRECTMSG, ADDSTREAMMSG, SENDHTTP, GETSTREAMFILE, SETHTTPPROCESSPAGE, ADDHTTPMSGDATA, SETSTREAMMSGSEPARATOR,
                                        SETSTREAMMSGSTART, SETSTREAMMSGEND, SETSTREAMMSGFILLER, CLEARHTTPMSGDATA, CLOSE, USESSL, SETSTREAMSSLSERVERNAME,
                                        PINGRETURN, CLIENTERROR, REGDATAEXEC, PROCESSDATAEXEC, MSGREPLAY, MSGCHECK, UDPSWITCHCONFIRM, PEERTOPEERREGISTER, 
                                        GETSTREAMFILELIST };
                                    /* Client Operation List */
        //        private enum RESPONSEOPERATIONS { DIRECTMSG, HTTPRESPONSE, STREAMMSG, FILESTART, FILEPART, FILEEND, STREAMFILELIST, 
        //                                          LOGERROR, DISPLAYERROR, PEERTOPEERSTART, PEERTOPEERCONNECT, PEERTOPEERENCRYPT, PEERTOPEERDISCONNECT, PEERTOPEERCHECK, 
        //                                          SETQUEUELIMIT, PINGSEND, DATAEXECRETURN, MSGREPLAY, GROUPJOINED, GROUPEXITED, HOSTNOTICE, UDPSWITCHNOTICE };
                                    /* Client Response Operation List (Actual List of Sent Responses) */
        private int nClientID = 0;  /* Client's ID */
        private string strGroupID = "";
                                    /* ID of Group That Client is In */
        private TcpClient tcConnection = null;     
                                    /* TCP Client Connection */
        private bool boolNoSSL = true;
        							/* Indicator to Not Use SSL */
        private NetworkStream nsSender = null;
                                    /* Network Stream for Sending to Client */
        private SslStream ssSecureSend = null;
        							/* SSL Stream for Sending to Client */
        private bool boolMsgProcess = true,
        							/* Indicator to Process Messages in Loop */
                     boolStreamsOutAll = ssConfig.StreamsSendAllOut,
                                    /* Indicator to Send Incoming from Streams Out to All Other All Streams Except Sender */
                     boolClientPeerToPeer = ssConfig.ClientPeerToPeer;
                                    /* Indicator to Have Clients Setup Peer to Peer Connections with Each Other */
        private string strIPAddress = "";
                                    /* Remote IP Address of Client */
        private int nPeerToPeerPort = 0;
                                    /* Client Assigned Port of "Peer-To-Peer" Connections */
        private Queue<string> qstrMsgReceived = new Queue<string>();
                                    /* Queue for Receive Messages*/
        private Queue<byte[]> qbyteMsgSend = new Queue<byte[]>(),
                                    /* Client Send Message Queue */
                              qbyteMsgReplay = new Queue<byte[]>();
                                    /* Message Queue for Replaying Client Requested Messages */
        private Dictionary<long, List<byte[]>> dictltBackups = new Dictionary<long, List<byte[]>>();
                                    /* List of Send Messages for Backup Replays */
        private Dictionary<long, string> dictstrMsgOutOfOrderStore = new Dictionary<long, string>();
                                    /* List for Messages When Received Out of Order */
        private Dictionary<int, CommTrans> dcCommTrans = new Dictionary<int, CommTrans>();
                                    /* Holds All Communiction Transaction Objects */
        private delegate bool delRecStrFuncPtr(string strValue1);
                                    /* Pointer to Receiver Functions That Take a String Value and Returns Boolean */
        private Dictionary<CLIENTOPERATIONS, delRecStrFuncPtr> dcRecFuncs = new Dictionary<CLIENTOPERATIONS, delRecStrFuncPtr>();
                                    /* Holds Pointers to Receiver Functions */
        private long lPingLastInMillis = 0,
                                    /* Length of Time Between Sending and Receiving Ping */
                     lPingCheckInMillis = 0;
                                    /* The Ping Time When Ping Check was Done */
        private string[] aPartEndChars = new string[] { ssConfig.PartEndChars },
                         aMsgMetaDataChars = new string[] { "-" };
                                    /* Array of Message Part End Indicator Characters and Message Metadata Indicator */
        private StringSplitOptions ssoNone = StringSplitOptions.None;
                                    /* Option for Splitting Strings to Remove Empty Entries from Resulting Array */
        private byte[] abyteEncryptKey = new byte[32],
                       abyteEncryptIV = new byte[16];
                                    /* Encryption Key and IV Block */
        private List<Client> ltPeerToPeerInPingCheck = new List<Client>();
                                    /* List of Clients for "Peer to Peer" Connections That Could be In Ping Check */
        private List<DatabaseExecutor> ltdeExecutors = new List<DatabaseExecutor>();
                                    /* List of Data Executors for Data Processes */
        private bool boolUseTCPCom = true,
                                    /* Indicator to Use TCP Client Communications */
                     boolIsNotWebSocket = true;
                                    /* Indicator That Connection is With WebSocket */
        private UdpClient ucConnect = null;
                                    /* UDP Based Client Connection */
        private object objUDPSecCon = null;
                                    /* Secure Connection for UDP Based Communications */
        private long lNextMsgToReceive = 1,
                     lLastMsgSent = 0;
                                    /* Next Message Number to be Recieved and Last Message Sent */
        private DateTime dtMsgReplayTimeout = DateTime.Now;
                                /* Timeout for Sending Missing Message Out of Check to Client  */

        /// <param name="tcSetConnection">Client's Client Connection Information</param>
        /// <param name="nSetClientID">Client ID</param>
        public Client(TcpClient tcSetConnection, int nSetClientID) {

/*           System.Security.Cryptography.RNGCryptoServiceProvider rcspGenerator;
                                    /* Generator for Encryption Key and IV Block Information for "Peer-To-Peer" Server Start */
/*           X509Certificate2 x509c2ServerCert = new X509Certificate2(ssConfig.SSLCertName);
                                    /* Server SSL Certificate */
/*           RSA rsaKey = RSA.Create();
                                    /* Server SSL Key */

            try {

                tcConnection = tcSetConnection;
                nClientID = nSetClientID;
                
                boolNoSSL = !ssConfig.IncSSL;
                
                nsSender = tcConnection.GetStream();
                
                if (!boolNoSSL) {
                
	                ssSecureSend = new SslStream(nsSender, false);

                    X509Certificate2 x509c2ServerCert = new X509Certificate2(ssConfig.SSLCertName);
                    RSA rsaKey = RSA.Create();

                    rsaKey.ImportPkcs8PrivateKey(new ReadOnlySpan<byte>(
                                                    Convert.FromBase64String(
                                                        new Regex(@"-----(BEGIN|END)( RSA)? PRIVATE KEY-----[\W]*")
                                                        .Replace(
                                                            File.ReadAllText(
                                                                ssConfig.SSLPrivKeyName).Trim(), 
                                                            ""))),
                                                 out int _);
                    
                    ssSecureSend.AuthenticateAsServer(new X509Certificate2(x509c2ServerCert.CopyWithPrivateKey(rsaKey).Export(X509ContentType.Pkcs12)), 
                                                      false, 
                                                      SslProtocols.Tls12, 
                                                      true);	
                }

                /* Setup to Access to Receiver Functions */
                dcRecFuncs.Add(CLIENTOPERATIONS.STARTSTREAM, StartStream);
                dcRecFuncs.Add(CLIENTOPERATIONS.STARTHTTPPOSTASYNC, StartHTTPPostAsync);
                dcRecFuncs.Add(CLIENTOPERATIONS.STARTHTTPPOSTSYNC, StartHTTPPostSync);
                dcRecFuncs.Add(CLIENTOPERATIONS.STARTHTTPGETASYNC, StartHTTPGetAsync);
                dcRecFuncs.Add(CLIENTOPERATIONS.STARTHTTPGETSYNC, StartHTTPGetSync);
                dcRecFuncs.Add(CLIENTOPERATIONS.DIRECTMSG, ProcessDirectMsg);
                dcRecFuncs.Add(CLIENTOPERATIONS.ADDSTREAMMSG, AddStreamMsg);
                dcRecFuncs.Add(CLIENTOPERATIONS.SENDHTTP, SendHTTP);
                dcRecFuncs.Add(CLIENTOPERATIONS.GETSTREAMFILE, GetStreamFile);
                dcRecFuncs.Add(CLIENTOPERATIONS.SETHTTPPROCESSPAGE, SetHTTPProcessPage);
                dcRecFuncs.Add(CLIENTOPERATIONS.ADDHTTPMSGDATA, AddHTTPMsgData);
                dcRecFuncs.Add(CLIENTOPERATIONS.SETSTREAMMSGSEPARATOR, SetStreamMsgSeparator);
                dcRecFuncs.Add(CLIENTOPERATIONS.SETSTREAMMSGSTART, SetStreamMsgStart);
                dcRecFuncs.Add(CLIENTOPERATIONS.SETSTREAMMSGEND, SetStreamMsgEnd);
                dcRecFuncs.Add(CLIENTOPERATIONS.SETSTREAMMSGFILLER, SetStreamMsgFiller);
                dcRecFuncs.Add(CLIENTOPERATIONS.CLEARHTTPMSGDATA, ClearHTTPMsgData);
                dcRecFuncs.Add(CLIENTOPERATIONS.CLOSE, TransClose);
                dcRecFuncs.Add(CLIENTOPERATIONS.USESSL, UseSSL);
                dcRecFuncs.Add(CLIENTOPERATIONS.SETSTREAMSSLSERVERNAME, SetStreamSSLServerName);
                dcRecFuncs.Add(CLIENTOPERATIONS.CLIENTERROR, ProcessClientError);
                dcRecFuncs.Add(CLIENTOPERATIONS.PINGRETURN, PingCheck);
                dcRecFuncs.Add(CLIENTOPERATIONS.REGDATAEXEC, RegisterDataExecution);
                dcRecFuncs.Add(CLIENTOPERATIONS.PROCESSDATAEXEC, ProcessDataExecution);
                dcRecFuncs.Add(CLIENTOPERATIONS.MSGREPLAY, ProcessMsgReplay);
                dcRecFuncs.Add(CLIENTOPERATIONS.MSGCHECK, ProcessMsgCheck);
                dcRecFuncs.Add(CLIENTOPERATIONS.UDPSWITCHCONFIRM, ProcessUDPSwitch);
                dcRecFuncs.Add(CLIENTOPERATIONS.GETSTREAMFILELIST, GetStreamFileList);
                
                strIPAddress = ((IPEndPoint)tcConnection.Client.RemoteEndPoint).Address.ToString();

                /* If Clients are Connecting "Peer to Peer" */
                if (boolClientPeerToPeer) {

                    if (ssConfig.HasClientPeerPortAvailable) {

                        if (ssConfig.ClientPeerEncryption) {

                            System.Security.Cryptography.RNGCryptoServiceProvider rcspGenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
                            rcspGenerator.GetBytes(abyteEncryptKey);
                            rcspGenerator.GetBytes(abyteEncryptIV);
                            rcspGenerator.Dispose();
                        }

                        nPeerToPeerPort = ssConfig.AvailableClientPeerToPeerPort;

                        Send(RegisterMsgTracking("PEERTOPEERSTART") + ssConfig.PartEndChars + strIPAddress + ssConfig.PartEndChars + nPeerToPeerPort.ToString(), abyteEncryptKey, abyteEncryptIV);
                    }
                    else {

                        ServerApp.Log("Action: Initializing holder for client information. Error: All available 'Peer-To-Peer' ports have been assigned.");
                    }
                }

                if (ssConfig.UseUDPClients) {

                    ucConnect = new UdpClient();
                    ucConnect.Connect(strIPAddress, ssConfig.UDPPort);
                    nMaxSendBytes = ssConfig.UDPMaxMsgLen;

                    ucConnect.Client.Blocking = false;

                    if (!boolNoSSL) {

                        objUDPSecCon = ssConfig.UDPSecureSetup(ucConnect.Client);
                    }

                    Send(RegisterMsgTracking("UDPSWITCHNOTICE") + ssConfig.PartEndChars + ssConfig.UDPPort);
                }

                new Thread(new ThreadStart(Communicate)).Start();
            }
            catch (Exception exError) {

                boolMsgProcess = false;

                ServerApp.Log("Action: Initializing holder for client information.", exError);

                if (!boolNoSSL) { 

                    ServerApp.Log("Inner Exception: " + exError.InnerException);
                }

                throw exError;
            }
        }

        /// <summary>
        ///     Send Client Message
        /// </summary>
        /// <param name="strMsg">Message to be Sent</param>
        /// <param name="abyteEncryptKey">Optional Encryption Key to Send</param>
        /// <param name="abyteEncryptIV">Optional Encryption IV Block to Send</param>
        /// <param name="boolTrack">Indicator to Track Message as Part of Backups</param>
        public void Send(string strMsg, byte[] abyteEncryptKey = null, byte[] abyteEncryptIV = null, bool boolTrack = true) {

/*           byte[] abyteEncryptHolder = new byte[abyteMsg.Length + ssConfig.PartEndChars.Length + abyteEncryptKey.Length + ssConfig.PartEndChars.Length + abyteEncryptIV.Length];
                                    /* Holder for Adding Encryption Information */
/*           string strPartChars = ssConfig.PartEndChars;
                                    /* Message Part End Characters */
/*           int nHolderLen = 0,
                 nBytePos = 0;      /* Amount of Filler Characters at Add to Message and 
                                       Position in Encryption Holder for Writing */

            if (abyteEncryptKey == null || abyteEncryptIV == null) {

                SendRaw(Encoding.UTF8.GetBytes((strMsgStartChars + strMsg + strMsgEndChars).PadRight(nMaxSendBytes, charMsgFiller)), boolTrack);
            }
            else {

                string strPartChars = ssConfig.PartEndChars;
                int nHolderLen = strMsgStartChars.Length + strMsg.Length + strPartChars.Length +
                                 abyteEncryptKey.Length + strPartChars.Length + abyteEncryptIV.Length + strMsgEndChars.Length;
                byte[] abyteEncryptHolder = null;
                int nBytePos = 0;

                if (nMaxSendBytes > nHolderLen) {

                    abyteEncryptHolder = new byte[nMaxSendBytes];
                }
                else {

                    abyteEncryptHolder = new byte[nHolderLen];
                }

                foreach (byte byteSelect in Encoding.UTF8.GetBytes(strMsgStartChars)) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }

                foreach (byte byteSelect in Encoding.UTF8.GetBytes(strMsg)) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }

                foreach (byte byteSelect in Encoding.UTF8.GetBytes(strPartChars)) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }

                foreach (byte byteSelect in abyteEncryptKey) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }

                foreach (byte byteSelect in Encoding.UTF8.GetBytes(strPartChars)) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }

                foreach (byte byteSelect in abyteEncryptIV) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }

                foreach (byte byteSelect in Encoding.UTF8.GetBytes(strMsgEndChars)) {
                    
                    abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                    nBytePos++;
                }
                
                if (nMaxSendBytes > nHolderLen) {

                    foreach (byte byteSelect in Encoding.UTF8.GetBytes("".PadRight(nMaxSendBytes - nHolderLen, charMsgFiller))) {
                    
                        abyteEncryptHolder.SetValue(byteSelect, nBytePos);
                        nBytePos++;
                    }
                }

                SendRaw(abyteEncryptHolder, boolTrack);
            }
        }

        /// <summary>
        ///     Send Client Message Transferred from Another Client
        /// </summary>
        /// <param name="strMsg">Message to be Sent</param>
        public void SendTransfer(string strMsg) {

            string[] astrMsgParts = strMsg.Split(aPartEndChars, ssoNone),
                     astrMsgMetaData = astrMsgParts[0].Split(aMsgMetaDataChars, ssoNone);
                                    /* Message Parts and its Metadata */

            Send(RegisterMsgTracking(astrMsgMetaData[0]) +
                                           strMsg.Replace(astrMsgParts[0], ""));
        }

        /// <summary>
        ///     Send Client Message with Start and End Indicators, Needed Padding, and UTF-8 Encoding
        /// </summary>
        /// <param name="abyteMsg">Whole Message to be Sent with Start and End Indicators, Needed Padding, and UTF-8 Encoding</param>
        /// <param name="boolTrack">Indicator to Add Message to Backup Tracking</param>
        private void SendRaw(byte[] abyteMsg, bool boolTrack = true) {

            if (boolTrack) {

                dictltBackups[lLastMsgSent].Add(abyteMsg);
            }

            qbyteMsgSend.Enqueue(abyteMsg);
        }

        /// <summary>
        ///     Send Client Message with Adding to Backup Tracking
        /// </summary>
        /// <param name="strMsg">Message to be Sent</param>
        private void SendNoTracking(string strMsg) {

            Send(strMsg, null, null, false);
        }

        /// <summary>
        ///     Sends Direct Message to Client
        /// </summary>
        /// <param name="strMsg">Message to Send</param>
        /// <param name="strMsgDesign">Optional Message Designation</param>
        public void SendDirectMsg(string strMsg, string strMsgDesign = "") {

            Send(RegisterMsgTracking("DIRECTMSG") + ssConfig.PartEndChars + strMsgDesign + ssConfig.PartEndChars + strMsg);
        }

        /// <summary>
        ///     Send User a File
        /// </summary>
        /// <param name="strFileDesign">Designation of File to Send</param>
        /// <returns>Indicator That File was Sent</returns>
        public bool SendFile(string strFileDesign) {
            
            string strFilePathName = ssConfig.GetDownloadFilePathName(strFileDesign);
                                    /* Name and Path of the Selected File */
            FileStream fsFileAccess;/* File Access */
//            StreamReader srFileAccess;/* File Access */
            MemoryStream msMsgStore;/* Storage for Putting Together Message */
            byte[] abyteMsg,        /* Part of Message to be Sent With File */
                   abytePartMsgStart = Encoding.UTF8.GetBytes(strMsgStartChars),
                   abytePartMsgEnd = Encoding.UTF8.GetBytes(strMsgEndChars),
                                    /* Starting and End Indicators for Message to be Sent With File */
                   abyteFile;       /* File to be Sent */
            int nFilePartMsgInfoLen = 0,
                nFilePartMsgStartLen = abytePartMsgStart.Length,
                nFilePartMsgEndLen = abytePartMsgEnd.Length;
                                    /* Length Information at Start of Message, and 
                                       Start and End Indicators of File Part Message Information Characters  */
            bool boolSent = true;   /* Indicator That Message was Sent */
            int nByteCounter = 0,   /* Number of Bytes Read */
                nMaxBytes = 0,      /* Maximum Number of Btyes That Can be Sent */
                nFileLen = 0;       /* Length of File */

            try {

                /* Load File and Setup Storage for File and Messages */
                fsFileAccess = new FileStream(ssConfig.FileLocPath + strFilePathName, FileMode.Open, FileAccess.Read);
                
                nFileLen = (int)fsFileAccess.Length;
                abyteFile = new byte[nFileLen];
                fsFileAccess.Read(abyteFile, 0, nFileLen);
                msMsgStore = new MemoryStream();

                fsFileAccess.Close();

                strFilePathName = strFilePathName.Replace("/", "\\");

                abyteMsg = Encoding.UTF8.GetBytes(RegisterMsgTracking("FILESTART") + strMsgPartEndChars + strFileDesign + strMsgPartEndChars + strFilePathName + strMsgPartEndChars + nFileLen.ToString() + strMsgPartEndChars);
                nFilePartMsgInfoLen = abyteMsg.Length;

                msMsgStore.Write(abytePartMsgStart, 0, nFilePartMsgStartLen);
                msMsgStore.Write(abyteMsg, 0, nFilePartMsgInfoLen);

                nMaxBytes = nMaxSendBytes - (nFilePartMsgStartLen + nFilePartMsgInfoLen + nFilePartMsgEndLen);

                if (nMaxBytes > nFileLen) {

                    nMaxBytes = nFileLen;
                }

                msMsgStore.Write(abyteFile, 0, nMaxBytes);
                msMsgStore.Write(abytePartMsgEnd, 0, nFilePartMsgEndLen);

                if (msMsgStore.Length < nMaxSendBytes) {

                    msMsgStore.Write(Encoding.UTF8.GetBytes("".PadRight(nMaxSendBytes, charMsgFiller)), Convert.ToInt32(msMsgStore.Length), nMaxSendBytes - Convert.ToInt32(msMsgStore.Length));
                }

                SendRaw(msMsgStore.ToArray());
                msMsgStore.Close();

                /* Cycle Through the File, and Send Peices of It */
                for (nByteCounter = nMaxBytes; (nByteCounter + nMaxBytes) < nFileLen; nByteCounter += nMaxBytes) {

                    msMsgStore = new MemoryStream();

                    abyteMsg = Encoding.UTF8.GetBytes(RegisterMsgTracking("FILEPART") + strMsgPartEndChars + strFileDesign + strMsgPartEndChars + strFilePathName + strMsgPartEndChars + nByteCounter + strMsgPartEndChars);
                    nFilePartMsgInfoLen = abyteMsg.Length;

                    nMaxBytes = nMaxSendBytes - (nFilePartMsgStartLen + nFilePartMsgInfoLen + nFilePartMsgEndLen);

                    msMsgStore.Write(abytePartMsgStart, 0, nFilePartMsgStartLen);
                    msMsgStore.Write(abyteMsg, 0, nFilePartMsgInfoLen);
                    msMsgStore.Write(abyteFile, nByteCounter, nMaxBytes);
                    msMsgStore.Write(abytePartMsgEnd, 0, nFilePartMsgEndLen);

                    SendRaw(msMsgStore.ToArray());
                    msMsgStore.Close();
                }

                /* Send File End Message */
                if (nByteCounter < nFileLen) {

                    /* Cycle Through the File, and Send the Last Peices of It */
                    while (nByteCounter < nFileLen) {

                        msMsgStore = new MemoryStream();
                        nMaxBytes = nFileLen - nByteCounter;
                            
                        /* If the End of the File Plus the Message Information Runs Over the Limit, Back Away, and Send a Peice of What's Left of the File */
                        if (nMaxBytes + nFilePartMsgStartLen + nFilePartMsgInfoLen + nFilePartMsgEndLen > nMaxSendBytes) {

                            nMaxBytes -= (nFilePartMsgStartLen + nFilePartMsgInfoLen + nFilePartMsgEndLen);
                        }

                        abyteMsg = Encoding.UTF8.GetBytes(RegisterMsgTracking("FILEEND") + strMsgPartEndChars + strFileDesign + strMsgPartEndChars + strFilePathName + strMsgPartEndChars + nByteCounter + strMsgPartEndChars);
                        nFilePartMsgInfoLen = abyteMsg.Length;

                        msMsgStore.Write(abytePartMsgStart, 0, nFilePartMsgStartLen);
                        msMsgStore.Write(abyteMsg, 0, nFilePartMsgInfoLen);
                        msMsgStore.Write(abyteFile, nByteCounter, nMaxBytes);
                        msMsgStore.Write(abytePartMsgEnd, 0, nFilePartMsgEndLen);

                        if (msMsgStore.Length < nMaxSendBytes) {

                            msMsgStore.Write(Encoding.UTF8.GetBytes("".PadRight(nMaxSendBytes, charMsgFiller)), Convert.ToInt32(msMsgStore.Length), nMaxSendBytes - Convert.ToInt32(msMsgStore.Length));
                        }

                        SendRaw(msMsgStore.ToArray());
                        msMsgStore.Close();

                        nByteCounter += nMaxBytes;
                    }
                }
                else {

                    /* Else Send Empty Message Signaling the End */
                    abyteMsg = Encoding.UTF8.GetBytes(RegisterMsgTracking("FILEEND") + strMsgPartEndChars + strFileDesign + strMsgPartEndChars + strFilePathName + strMsgPartEndChars + nByteCounter + strMsgPartEndChars);
                    nFilePartMsgInfoLen = abyteMsg.Length;

                    msMsgStore = new MemoryStream();
                    msMsgStore.Write(abytePartMsgStart, 0, nFilePartMsgStartLen);
                    msMsgStore.Write(abyteMsg, 0, nFilePartMsgInfoLen);
                    msMsgStore.Write(abytePartMsgEnd, 0, nFilePartMsgEndLen);

                    if (msMsgStore.Length < nMaxSendBytes) { 

                        msMsgStore.Write(Encoding.UTF8.GetBytes("".PadRight(nMaxSendBytes, charMsgFiller)), Convert.ToInt32(msMsgStore.Length), nMaxSendBytes - Convert.ToInt32(msMsgStore.Length));
                    }

                    SendRaw(msMsgStore.ToArray());
                    msMsgStore.Close();
                }
            }
            catch (Exception exError) {

                ServerApp.Log("Action: User sending file.", exError);
                boolSent = false;
            }

            return boolSent;
        }

        /// <summary>Send "Peer To Peer" Connect Message to Client</summary>
        /// <param name="strPeerToPeerServerIP">IP Address of "Peer To Peer" Client</param>
        /// <param name="nClientPeerToPeerPort">Assigned Port of "Peer To Peer" Client</param>
        /// <param name="abyteEncryptKey">Optional Encryption Key for Client's "Peer To Peer" Communications</param>
        /// <param name="abyteEncryptIV">Optional Encryption IV Block for Client's "Peer To Peer" Communications</param>
        private void SendPeerToPeerConnect(string strPeerToPeerServerIP, 
                                           int nClientPeerToPeerPort, 
                                           byte[] abyteEncryptKey = null, 
                                           byte[] abyteEncryptIV = null) {

            if (strPeerToPeerServerIP != strIPAddress || ssConfig.AllowMultipleSameIP) {

                Send(RegisterMsgTracking("PEERTOPEERCONNECT") + 
                     ssConfig.PartEndChars + 
                     strPeerToPeerServerIP + 
                     ssConfig.PartEndChars +
                     nClientPeerToPeerPort.ToString(),
                     abyteEncryptKey, 
                     abyteEncryptIV);
            }
            else { 
            
                ServerApp.Log("Action: Sending 'Peer-to-Peer' connect message to client. Error: IP Addresses are the same and 'AllowMultipleSameIP' setting is false.");
            }
        }
        
        /// <summary>Set Information to Encrypt "Peer To Peer" Messages to Client</summary>
        /// <param name="strPeerToPeerServerIP">IP Address of "Peer To Peer" Client</param>
        /// <param name="nClientPeerToPeerPort">Assigned Port of "Peer To Peer" Client</param>
        /// <param name="abyteEncryptKey">Encryption Key for Client's "Peer To Peer" Communications</param>
        /// <param name="abyteEncryptIV">Encryption IV Block for Client's "Peer To Peer" Communications</param>
        public void SetPeerToPeerEncryption(string strPeerToPeerServerIP, 
                                            int nClientPeerToPeerPort, 
                                            byte[] abyteEncryptKey, 
                                            byte[] abyteEncryptIV) {
            
            if (ssConfig.ClientPeerEncryption) {

                if (strPeerToPeerServerIP != strIPAddress || ssConfig.AllowMultipleSameIP) {
                
                    Send(RegisterMsgTracking("PEERTOPEERENCRYPT") + 
                         ssConfig.PartEndChars + 
                         strPeerToPeerServerIP +
                         ssConfig.PartEndChars + 
                         nClientPeerToPeerPort.ToString(),
                         abyteEncryptKey, 
                         abyteEncryptIV);
                }
                else { 
            
                    ServerApp.Log("Action: Sending 'Peer-to-Peer' encryption message to client. Error: IP Addresses are the same and 'AllowMultipleSameIP' setting is false.");
                }
            }
        }

        /// <summary>Send "Peer To Peer" Disconnect Message to Client</summary>
        public void SendPeerToPeerDisconnect() {
            
            Send(RegisterMsgTracking("PEERTOPEERDISCONNECT"));
        }

        /// <summary>
        ///     Adds Clients for "Peer to Peer" Connections That are Currently in Ping Check to 
        ///     List for Later Connection If Still Active
        /// </summary>
        /// <param name="cntClientInPingCheck">Client for "Peer to Peer" Connection</param>
        public void AddPeerToPeerCandidate(Client cntClientInPingCheck) {

            cntClientInPingCheck.SendPing(true);
            ltPeerToPeerInPingCheck.Add(cntClientInPingCheck);
        }

        /// <summary>Send Ping</summary>
        /// <param name="boolDoPingCheck">Indicator to Do Ping Check, Defaults to False</param>
        public void SendPing(bool boolDoPingCheck = false) {

            long lNowInMillis = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                                    /* Now In Milliseconds */

            if (boolDoPingCheck && lPingCheckInMillis == 0) {

                lPingCheckInMillis = lNowInMillis;
            }
            
            SendNoTracking("PINGSEND" + strMsgPartEndChars + lNowInMillis.ToString());
        }

        /// <summary>
        ///     Retrieve and Sends User Messages
        /// </summary>
        public void Communicate() {

            bool boolPing = ssConfig.Ping;
                                    /* Indicator to Ping Client's at Interval */
            int nPingIntervalInMillis = ssConfig.PingInterval;
                                    /* Amount of Time Between Pings */
            DateTime dtPingTime = DateTime.Now.AddMilliseconds(nPingIntervalInMillis);
                                    /* Time to Ping */
            byte[] abyteMsgReceived = null,
                   abyteMsgSend = null,
                   abyteUDPTemp = null;
                                    /* Message or Selected Part to be Sent or 
                                       Retriever for Message from User, and
                                       Holder for Messages Broken Up for UDP Sends */
            int nMsgLen = 0;        /* Length of Message */
            bool boolDataAvail = false,
                 boolDataRead = false;
            					/* Indicators That Data is Available, Could be or was Read from Selected Stream */
            int nMsgStartIndex = -1,
                nMsgEndIndex = -1;
                                /* Start and Ending Index of Sent Message */
            string strMsgCollect = "",
                   strMsgFound = "";
                                /* Used for Collecting Message Parts or Sending Message, and Recovered Sent Message */
            int nMsgActualStartIndex = 0;
                                /* Actual Starting Index of Message */
            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                /* Manage Stopping Thread */
            List<Client> ltcntPingRemove = new List<Client>();
                                /* List of Clients to Remove from Ping Check */
            bool boolMsgOutOfOrder = false;
                                /* Valid Message Received After Metadata Checked */
            Queue<byte[]> qbyteMsgStoreSend = new Queue<byte[]>();
                                /* Message Queue for Prepping for Sending */
            IPEndPoint iepUDPClient = null;
                                /* IP End Point for UDP Client */
            byte[] abyteWebSockMask = null,
                                /* Mask for Decoding Web Socket */
                   abyteWebSockDecoded = null;
                                /* Decoded Web Socket Data */
            int nWebSockOffset = 0,
                                /* Offset in Decoding WebSocket Values */
                nWebSockMsgLen = 0,
                                /* Len of WebSocket Message */
                nSendCount = 0, /* Amount of Data Being Sent */
                nCounter = 0;   /* Counter for Loop */
            DateTime dtWebSocketWaitEnd = DateTime.Now.AddMilliseconds(ssConfig.WebSocketConnectionWait);
                                /* Time to End Waiting for Web Socket Handshake to Complete */
            bool boolWebSocketSend = false,
                                /* Indicator That Messages Can Be Sent on Web Sockets */
                 boolAfterWebSocketWaitData = false;
                                /* Indicator That Received Data is Waiting After Web Socket Wait */

            try {

                if (!boolUseTCPCom) {

                    iepUDPClient = new IPEndPoint(IPAddress.Parse(strIPAddress), ssConfig.UDPPort);
                }

                while (boolMsgProcess) {

                    /* If Pinging Client, If Interval Has Passed and No Other Messages Being Send, Send Ping Message */
                    if (boolPing && 
                        dtPingTime.CompareTo(DateTime.Now) <= 0) {
                            
                        lock (qbyteMsgSend) {

                            if (qbyteMsgSend.Count <= 0) {

                                SendPing();
                                dtPingTime = DateTime.Now.AddMilliseconds(nPingIntervalInMillis);
                            }
                        }
                    }

                    lock (ltPeerToPeerInPingCheck) {

                        foreach (Client cntSelect in ltPeerToPeerInPingCheck) {

                            if (!cntSelect.InPingCheck && cntSelect.Connected) {

                                cntSelect.SetPeerToPeerEncryption(strIPAddress, nPeerToPeerPort, abyteEncryptKey, abyteEncryptIV);
                                SendPeerToPeerConnect(cntSelect.IP, cntSelect.PeerToPeerPort, cntSelect.EncryptKey, cntSelect.EncryptIV);
                                ltcntPingRemove.Add(cntSelect);
                            }
                            else if (!cntSelect.Connected) {

                                ltcntPingRemove.Add(cntSelect);
                            }
                        }

                        foreach (Client cntSelect in ltcntPingRemove) {

                            ltPeerToPeerInPingCheck.Remove(cntSelect);
                        }
                    }

                    if (boolWebSocketSend) {

                        RegisterDataMaps();

                        lock (ltdeExecutors) {

                            foreach (DatabaseExecutor deSelect in ltdeExecutors) {

                                foreach (int nTransID in deSelect.Results(nClientID)) {

                                    SendDataExecutionResult(nTransID, deSelect.DequeueResult(nClientID, nTransID));
                                }

                                foreach (string strDataMapResult in deSelect.DequeueDataMapResults()) {

                                    SendDirectMsg(strDataMapResult);
                                }
                            }
                        }
                    }

                    if (boolUseTCPCom) {

                        while (nsSender.DataAvailable) { 

                            if (boolNoSSL && 
                                (boolDataRead = nsSender.CanRead) &&
                                (nMsgLen = tcConnection.ReceiveBufferSize) > 0) { 

                                abyteMsgReceived = new byte[nMsgLen];

                                nsSender.Read(abyteMsgReceived, 0, nMsgLen);
                            }
                            else if (ssSecureSend != null &&
                                     (boolDataRead = ssSecureSend.CanRead)) {

                                abyteMsgReceived = new byte[nMaxSendBytes];

                                if ((nMsgLen = ssSecureSend.Read(abyteMsgReceived, 0, nMaxSendBytes)) <= 0) { 

                                    abyteMsgReceived = null;
                                }
                            }

                            if (abyteMsgReceived != null) {

                                if (boolIsNotWebSocket) {

                                    strMsgCollect += Encoding.UTF8.GetString(abyteMsgReceived);
                                }
                                else {
                                    
                                    switch (abyteMsgReceived[0] - 128) {

                                        case 1: {
                                                                                  
                                            nWebSockMsgLen = abyteMsgReceived[1] - 128;

                                            if (nWebSockMsgLen <= 126) {

                                                if (nWebSockMsgLen == 126) {

                                                    nWebSockMsgLen = (abyteMsgReceived[2] << 8) + abyteMsgReceived[3];
                                                    nWebSockOffset = 2;
                                                }

                                                abyteWebSockMask = new byte[] { abyteMsgReceived[nWebSockOffset + 2],
                                                                                abyteMsgReceived[nWebSockOffset + 3],
                                                                                abyteMsgReceived[nWebSockOffset + 4],
                                                                                abyteMsgReceived[nWebSockOffset + 5] };

                                                abyteWebSockDecoded = Encoding.UTF8.GetBytes(new string('\0', nWebSockMsgLen));
                                                nWebSockOffset += 6;

                                                for (nCounter = 0; nCounter < nWebSockMsgLen; nCounter++) {

                                                    abyteWebSockDecoded[nCounter] = (byte)(abyteMsgReceived[nWebSockOffset + nCounter] ^
                                                                                           abyteWebSockMask[nCounter % 4]);
                                                }

                                                strMsgCollect += Encoding.UTF8.GetString(abyteWebSockDecoded);
                                                nWebSockOffset = 0;
                                            }
                                            else {
                                                    
                                                ServerApp.Log("Action: Retrieving and sending user messages. Error: Message was bigger than supported lengths.");
                                            }

                                            break;
                                        }
                                        case 8: {
 
                                            Close();
                                            abyteMsgReceived = null;
                                            break;
                                        }
                                        case 9: {
 
                                            /* Create Fake Response Message to Have Full Message Copied as Pong Response */
                                            strMsgCollect += strMsgStartChars + "PINGPONG" + strMsgPartEndChars + Encoding.UTF8.GetString(abyteWebSockDecoded);
                                            break;
                                        }
                                        case 10: {
 
                                            PingCheck("PINGRETURN" + strMsgPartEndChars + lPingCheckInMillis.ToString());
                                            abyteMsgReceived = null;
                                            break;
                                        }
                                        case 0: {

                                            abyteMsgReceived = null;
                                            ServerApp.Log("Action: Retrieving and sending user messages. Error: WebSocket message was a continuation frame which is not supported.");
                                            break;
                                        }
                                        case 2: {
 
                                            abyteMsgReceived = null;
                                            ServerApp.Log("Action: Retrieving and sending user messages. Error: WebSocket message was a binary frame which is not supported.");
                                            break;
                                        }
                                        default: { 
                                                
                                            abyteMsgReceived = null;
                                            ServerApp.Log("Action: Retrieving and sending user messages. Error: WebSocket message was a pong or unsupported frame which is not supported.");
                                            break;
                                        }
                                    }
                                }
                            }

                            boolDataAvail = true;
                        }
                    }
                    else if (ucConnect.Client.Connected) {

                        while ((nMsgLen = ucConnect.Available) > 0) {

                            if ((abyteMsgReceived = ucConnect.Receive(ref iepUDPClient)) != null) {

                                strMsgCollect += Encoding.UTF8.GetString(abyteMsgReceived);
                                boolDataRead = true;
                            }

                            boolDataAvail = true;
                        }
                    }
                    else {

                        boolUseTCPCom = true;
                        nMaxSendBytes = ssConfig.MaxMsgLen;
                        ServerApp.Log("Action: Retrieving and sending user messages. Error: UDP socket closed for reading, switching back to TCP.");
                    }

                    lock (dictstrMsgOutOfOrderStore) { 
                    
                        if (dictstrMsgOutOfOrderStore.Count > 0 && dictstrMsgOutOfOrderStore.ContainsKey(lNextMsgToReceive)) {

                            strMsgCollect += strMsgStartChars + dictstrMsgOutOfOrderStore[lNextMsgToReceive] + strMsgEndChars;
                            dictstrMsgOutOfOrderStore.Remove(lNextMsgToReceive);
                            boolDataAvail = true;
                            boolDataRead = true;
                        }
                    }

                    if (boolDataAvail || boolAfterWebSocketWaitData) { 

                        if (boolDataRead || boolAfterWebSocketWaitData) {
                        
                            if (strMsgCollect.Trim().Length > 0) {

                                /* If First Message is from Web Socket, Switch Over and Send Back Handshake */
                                if (!boolWebSocketSend && Regex.IsMatch(strMsgCollect, "^GET", RegexOptions.IgnoreCase)) {

                                    qbyteMsgSend.Enqueue(Encoding.UTF8.GetBytes(
                                        "HTTP/1.1 101 Switching Protocols\r\n" +
                                        "Upgrade: websocket\r\n" +
                                        "Connection: Upgrade\r\n" +
                                        "Sec-WebSocket-Accept: " +
                                        Convert.ToBase64String(System.Security.Cryptography.SHA1.Create().ComputeHash(
                                            Encoding.UTF8.GetBytes(
                                                Regex.Match(strMsgCollect, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim() +
                                                                           "258EAFA5-E914-47DA-95CA-C5AB0DC85B11")))
                                        + "\r\n\r\n"));

                                    strMsgCollect = "";
                                    boolIsNotWebSocket = false;
                                    boolPing = false;
                                }

                                if (boolWebSocketSend) {

                                    nMsgStartIndex = strMsgCollect.IndexOf(strMsgStartChars);

                                    while (nMsgStartIndex >= 0) {

                                        nMsgEndIndex = strMsgCollect.IndexOf(strMsgEndChars);
                                        nMsgActualStartIndex = nMsgStartIndex + strMsgStartChars.Length;

                                        if (nMsgEndIndex > -1 && nMsgActualStartIndex < nMsgEndIndex) {

                                            strMsgFound = strMsgCollect.Substring(nMsgActualStartIndex, nMsgEndIndex - nMsgActualStartIndex);

                                            boolMsgOutOfOrder = !ProcessMsg(strMsgFound, boolMsgOutOfOrder);

                                            strMsgCollect = strMsgCollect.Substring(nMsgEndIndex + strMsgEndChars.Length);
                                            abyteMsgReceived = null;
                                        }
                                        else if (nMsgActualStartIndex >= nMsgEndIndex) {

                                            ServerApp.Log("Action: Retrieving and sending user messages. Error: Partial message found, but was removed. Current messages: " + strMsgCollect);

                                            if (nMsgEndIndex >= 0) {

                                                strMsgCollect = strMsgCollect.Substring(nMsgStartIndex);
                                            }
                                            else {

                                                strMsgCollect = "";
                                            }
                                        }

                                        nMsgStartIndex = strMsgCollect.IndexOf(strMsgStartChars);
                                    }
                                }

                                boolAfterWebSocketWaitData = false;
                            }
                        }
                        else {

                            Close();

                   		    if (boolNoSSL) {
                        	
                        	    ServerApp.Log("Action: Retrieving and sending user messages. Error: Retrieving message failed, closed stream.");
                            }
                            else {

                                ServerApp.Log("Action: Retrieving and sending user messages using SSL. Error: Retrieving message failed, closed secure stream.");
                            }
                        }
                    }

                    boolDataAvail = false;
                    boolDataRead = false;

                    lock (ssConfig) { 

                        if (!ssConfig.Running) {

                            Close();
                        }
                    }

                    lock (qbyteMsgReplay) {
                            
                        while (qbyteMsgReplay.Count > 0) {

                            qbyteMsgStoreSend.Enqueue(qbyteMsgReplay.Dequeue());
                        }
                    }

                    lock (qbyteMsgSend) {
                            
                        while (qbyteMsgSend.Count > 0) {

                            qbyteMsgStoreSend.Enqueue(qbyteMsgSend.Dequeue());
                        }
                    }

                    while (qbyteMsgStoreSend.Count > 0) {

                        abyteMsgSend = qbyteMsgStoreSend.Dequeue();

                        if (!boolIsNotWebSocket && boolWebSocketSend) {

                            abyteMsgSend = ConvertWebSocketMsg(abyteMsgSend);
                        }

                        nMsgLen = abyteMsgSend.Length;

                        if (boolUseTCPCom) { 

            	            if (boolNoSSL) {
            	 		
	                            if (nsSender.CanWrite) {

                                    for (nCounter = 0; nCounter < nMsgLen; nCounter += nMaxSendBytes) {

                                        if (nMsgLen - nCounter >= nMaxSendBytes) {
                                            
                                            nSendCount = nMaxSendBytes;
                                        }
                                        else {
                                            
                                            nSendCount = nMsgLen - nCounter;
                                        }

                                        nsSender.Write(abyteMsgSend, nCounter, nSendCount);
                                    }
	                            }
	                            else {

                                    ServerApp.Log("Action: Retrieving and sending user messages. Error: Stream closed, removing client.");
	                                Close();
                                }
            	            }
            	            else if (ssSecureSend != null && 
                                     ssSecureSend.CanWrite) {
                    
	                            for (nCounter = 0; nCounter < nMsgLen; nCounter += nMaxSendBytes) {
                                                
                                    if (nMsgLen - nCounter >= nMaxSendBytes) {

                                        nSendCount = nMaxSendBytes;
                                    }
                                    else {

                                        nSendCount = nMsgLen - nCounter;
                                    }

                                    ssSecureSend.Write(abyteMsgSend, nCounter, nSendCount);
                                }
                            }
                            else {

                                ServerApp.Log("Action: Retrieving and sending user messages using SSL. Error: SSL Stream closed, removing client.");
                                Close();
                            }
                        }
                        else if (ucConnect.Client.Connected) {

                            for (nCounter = 0; nCounter < nMsgLen; nCounter += nMaxSendBytes) {

                                if (nMsgLen - nCounter >= nMaxSendBytes) {

                                    abyteUDPTemp = new byte[nMaxSendBytes];
                                }
                                else {

                                    abyteUDPTemp = new byte[nMsgLen - nCounter];
                                }

                                Buffer.BlockCopy(abyteMsgSend, nCounter, abyteUDPTemp, 0, abyteUDPTemp.Length);

                                if (ucConnect.Send(abyteUDPTemp, abyteUDPTemp.Length) != abyteUDPTemp.Length) {

                                    ServerApp.Log("Action: Retrieving and sending user messages. Error: UDP stream did not send correct amount of data.");
                                }
                            }
                        }
                        else {

                            ServerApp.Log("Action: Retrieving and sending user messages. Error: UDP socket closed for writing, switching back to TCP.");
                            boolUseTCPCom = true;
                            nMaxSendBytes = ssConfig.MaxMsgLen;
                        }
                    }

                    if (!boolWebSocketSend) {

                        if ((boolWebSocketSend = DateTime.Now >= dtWebSocketWaitEnd) && 
                            strMsgCollect.Trim().Length > 0) {

                            boolAfterWebSocketWaitData = true;
                        }
                    }

                    areThreadStopper.WaitOne(1);
                }
            }
            catch (Exception exError) {

                ServerApp.Log("Action: Retrieving and sending user messages.", exError);
                Close();
            }
        }

        private bool ProcessMsg(string strMsg, bool boolMsgsOutOfOrder = false) { 
        
            string[] astrMsgParts = strMsg.Split(aPartEndChars, ssoNone),
                                        /* Message Split into Parts */
                     astrMsgMetaData = astrMsgParts[0].Split(aMsgMetaDataChars, ssoNone);
                                        /* Message Metadata for Sequence and Time Sent */
            long lMsgNumReceived = 0;   /* Squence Number of the Received Message */
            bool boolValidReceived = true,
                 boolMsgIsAhead = false;/* Indicator That a Valid Message was Received or Ahead of Expected */            
            CLIENTOPERATIONS coMsgType; /* Selected Message's Type */
            // Queue<string> qstrMsgOutOfOrderToProcess = new Queue<string>();
                                        /* List for Messages When Received Out of Order to ne Processed */

            if (astrMsgMetaData.Length > 1) {

                if (astrMsgMetaData.Length >= 2) {

                    if (long.TryParse(astrMsgMetaData[1], out lMsgNumReceived)) {

                        if (boolValidReceived = (lNextMsgToReceive == lMsgNumReceived)) {

                            lNextMsgToReceive++;
                        }
                        else if (lNextMsgToReceive < lMsgNumReceived) {

                            boolMsgIsAhead = true;
                        }
                    }
                }

                astrMsgParts[0] = astrMsgMetaData[0];
            }

            if (boolValidReceived) { 

                lock (ssConfig) {

                    /* If Relaying Client Message Elsewhere */
                    ssConfig.TransferClientMsgOut(strMsg);
                }

                /* If Sending Out to All Other Client Streams */
                if (boolStreamsOutAll && astrMsgParts[0] != "DIRECTMSG") {

                    lock (qstrMsgReceived) {

                        qstrMsgReceived.Enqueue(strMsg);
                    }
                }

                /* Process By Running Message Function Command */
                if (Enum.TryParse<CLIENTOPERATIONS>(astrMsgParts[0], true, out coMsgType)) {

                    lock (dcRecFuncs) {

                        dcRecFuncs[coMsgType].Invoke(strMsg);
                    }
                }

                if (boolMsgsOutOfOrder) { 

                    Queue<string> qstrMsgOutOfOrderToProcess = new Queue<string>();

                    lock (dictstrMsgOutOfOrderStore) {

                        foreach (string strMsgStored in dictstrMsgOutOfOrderStore.Values) {

                            qstrMsgOutOfOrderToProcess.Enqueue(strMsgStored);
                        }

                        dictstrMsgOutOfOrderStore.Clear();
                    }

                    while (qstrMsgOutOfOrderToProcess.Count > 0) {

                        boolValidReceived = ProcessMsg(qstrMsgOutOfOrderToProcess.Dequeue());
                    }
                }
            }
            else if (boolMsgIsAhead) {

                lock (dictstrMsgOutOfOrderStore) {
           
                    dictstrMsgOutOfOrderStore.Add(lMsgNumReceived, strMsg);
                }

                /* Send Message to Client to Resend from Expected Message */
                if (lMsgNumReceived > 0 && lNextMsgToReceive < lMsgNumReceived && dtMsgReplayTimeout < DateTime.Now) {

                    SendNoTracking("MSGREPLAY" + strMsgPartEndChars + lNextMsgToReceive.ToString());
                    dtMsgReplayTimeout = DateTime.Now.AddMilliseconds(ssConfig.MsgReplayTimeout);
                }
            }

            return boolValidReceived;
        }
        
        /// <summary>
        ///     Converts Message for Use in WebSockets Sends
        /// </summary>
        /// <param name="abyteMsg">Message to Convert</param>
        /// <returns>Converted Message for WebSocket Send or If Error, Empty Byte Array</returns>
        private byte[] ConvertWebSocketMsg(byte[] abyteMsg) {

            int nMsgType = 1,
                nMsgLen = abyteMsg.Length,
                nWebSockMsgLen = nMsgLen + 2,
                nWebSockSendLen = nMsgLen,
                nOffset = 0;
            string strMsg = Encoding.UTF8.GetString(abyteMsg);
            byte[] abyteWebSocketMsg;

            try { 

                if (nMsgLen <= 65535) {

                    if (nMsgLen > 125) {

                        nWebSockSendLen = 126;
                        nWebSockMsgLen += 2;
                        nOffset = 2;
                    }

                    abyteWebSocketMsg = Encoding.UTF8.GetBytes(new string('\0', nWebSockMsgLen));

                    /* TODO: Check If Using Binnary Type Frame Needed for File Transfers */
                    if (IsMsgAFile(strMsg)) {

                        nMsgType = 2;
                    }
                    else if (strMsg.StartsWith(strMsgStartChars + "PINGSEND")) {

                        nMsgType = 9;
                        nWebSockSendLen = 125;
                    }

                    if (!strMsg.StartsWith(strMsgStartChars + "PINGPONG")) {

                        abyteWebSocketMsg[1] = (byte)nWebSockSendLen;

                        if (nWebSockSendLen == 126) {

                            abyteWebSocketMsg[2] = (byte)((nMsgLen >> 8) & 255);
                            abyteWebSocketMsg[3] = (byte)(nMsgLen & 255);
                        }

                        Buffer.BlockCopy(abyteMsg, 0, abyteWebSocketMsg, nOffset + 2, nMsgLen);
                    }
                    else { 

                        nMsgType = 10;
                        abyteWebSocketMsg = Encoding.UTF8.GetBytes(strMsg.Split(strMsgPartEndChars)[1]);
                    }

                    abyteWebSocketMsg[0] = (byte)(nMsgType | 128);
                }
                else {

                    abyteWebSocketMsg = Encoding.UTF8.GetBytes(new string('\0', nMsgLen));
                    ServerApp.Log("Action: Converting sending message to WebSocket message. Error: Message size of " + nMsgLen + " was bigger than allowed max of 65535.");
                }
            }
            catch (Exception exError) {

                abyteWebSocketMsg = Encoding.UTF8.GetBytes(new string('\0', nMsgLen));
                ServerApp.Log("Action: Converting sending message to WebSocket message", exError);
            }

            return abyteWebSocketMsg;
        }

        /// <summary>
        ///     Is Message Containing Standard Text or Binary File Data
        /// </summary>
        /// <param name="strMsg">Message</param>
        /// <returns>True If Message is for File Data or Continuation of File Data, Else False</returns>
        private bool IsMsgAFile(string strMsg) {

            bool boolIsFile = false;

            if (strMsg.StartsWith(strMsgStartChars + "FILESTART") ||
                strMsg.StartsWith(strMsgStartChars + "FILEPART") ||
                strMsg.StartsWith(strMsgStartChars + "FILEEND")) {

                boolIsFile = true;
            }

            return boolIsFile;
        }

        /// <summary>
		/// 	Receiver Function for Starting Streams
		/// </summary>
		/// <param name="strMessage">Message Containing Information on How to Start Stream</param>
		/// <returns>True If Stream was Setup Successfully, Else False</returns>
		private bool StartStream(string strMsg) {
			
            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nNewTransID = 0,    /* New Transaction ID */
                nPort = 0;          /* New Transaction's Port */
            bool boolTransSetup = false;
                                    /* Identicator That Transaction Has Been Setup */

            try {

                if (int.TryParse(astrMsgParts[1], out nNewTransID)) {

                    if (!dcCommTrans.ContainsKey(nNewTransID)) {

                        if (int.TryParse(astrMsgParts[3], out nPort)) {

			                dcCommTrans.Add(nNewTransID, new CommTrans(nNewTransID, astrMsgParts[2], nPort, "STREAM", true));
					
                            boolTransSetup = true;
                        }
                        else {

                            // Log Error
        		            ServerApp.Log("Action: During setting up communication transaction stream. Error: Getting the stream's port from client message failed. Message: " + strMsg);
                        }
                    }
                    else {
        		            
                        ServerApp.Log("Action: During setting up communication transaction stream, transaction ID reused. ID: " + nNewTransID.ToString());
                    }
                }
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During setting up communication transaction stream. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: Setting up communication transaction stream.", exError);
            }
			
			return boolTransSetup;
		}	

		/// <summary>
		/// 	Starts Setup to Send HTTP Messages
		/// </summary>
        /// <param name="nNewTransID">New Transaction ID</param>
		/// <param name="strHostName">Host Server Name</param>
		/// <param name="nPort">Server Port</param>
		/// <param name="strTransType">Type of Communication is Being Used</param>
		/// <param name="boolAsync">Indicator to Not Wait for Response Communications</param>
		/// <returns>Identifier That New HTTP Transaction was Added</returns>
		private bool StartHTTPTrans(int nNewTransID, string strHostName, int nPort, string strTransType, bool boolAsync) {
			
            bool boolAdded = false; /* Indicator That New HTTP Transmission was Added */
            CommTrans ctNew = null; /* New Communication Transmission */

            if (!dcCommTrans.ContainsKey(nNewTransID)) {

                ctNew = new CommTrans(nNewTransID, strHostName, nPort, strTransType, boolAsync);

                ctNew.AddHTTPMsgData(ssConfig.ClientIDVar, nClientID.ToString());
                ctNew.AddHTTPMsgData(ssConfig.GroupIDVar, strGroupID);
                ctNew.AddHTTPMsgData(ssConfig.TransactionIDVar, nNewTransID.ToString());

                dcCommTrans.Add(nNewTransID, ctNew);

                boolAdded = true;
            }

            return boolAdded;
		}

        /// <summary>
		/// 	Receiver Function for Setup to Send Asynchronous HTTP POST Messages
		/// </summary>
		/// <param name="strMessage">Message Containing Information on How to Start HTTP Transmission</param>
		/// <returns>True If HTTP Transmission was Setup Successfully, Else False</returns>
		private bool StartHTTPPostAsync(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nNewTransID = 0,    /* New Transaction ID */
                nPort = ssConfig.DefaultHTTPPort;          
                                    /* New Transaction's Port */
            bool boolTransSetup = false;
                                    /* Identicator That Transaction Has Been Setup */

            try {

                if (int.TryParse(astrMsgParts[1], out nNewTransID)) {

                    if (astrMsgParts.Length > 3) {

                        int.TryParse(astrMsgParts[3], out nPort);
                    }

			        boolTransSetup = StartHTTPTrans(nNewTransID, astrMsgParts[2], nPort, "HTTPPOST", true);
                }
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During setting up asynchronous HTTP POST communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: Setting up asynchronous HTTP POST communication transaction.", exError);
            }
			
			return boolTransSetup;
		}

        /// <summary>
		/// 	Receiver Function for Setup to Send Synchronous HTTP POST Messages
		/// </summary>
		/// <param name="strMessage">Message Containing Information on How to Start HTTP Transmission</param>
		/// <returns>True If HTTP Transmission was Setup Successfully, Else False</returns>
		private bool StartHTTPPostSync(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nNewTransID = 0,    /* New Transaction ID */
                nPort = ssConfig.DefaultHTTPPort;          
                                    /* New Transaction's Port */
            bool boolTransSetup = false;
                                    /* Identicator That Transaction Has Been Setup */

            try {

                if (int.TryParse(astrMsgParts[1], out nNewTransID)) {

                    if (astrMsgParts.Length > 3) {

                        int.TryParse(astrMsgParts[3], out nPort);
                    }
                    
                    boolTransSetup = StartHTTPTrans(nNewTransID, astrMsgParts[2], nPort, "HTTPPOST", false);
                }
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During setting up synchronous HTTP POST communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: Setting up synchronous HTTP POST communication transaction.", exError);
            }
			
			return boolTransSetup;
		}			

        /// <summary>
		/// 	Receiver Function for Setup to Send Asynchronous HTTP GET Messages
		/// </summary>
		/// <param name="strMessage">Message Containing Information on How to Start HTTP Transmission</param>
		/// <returns>True If HTTP Transmission was Setup Successfully, Else False</returns>
		private bool StartHTTPGetAsync(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nNewTransID = 0,    /* New Transaction ID */
                nPort = ssConfig.DefaultHTTPPort;          
                                    /* New Transaction's Port */
            bool boolTransSetup = false;
                                    /* Identicator That Transaction Has Been Setup */

            try {

                if (int.TryParse(astrMsgParts[1], out nNewTransID)) {

                    if (astrMsgParts.Length > 3) {

                        int.TryParse(astrMsgParts[3], out nPort);
                    }
                    
                    boolTransSetup = StartHTTPTrans(nNewTransID, astrMsgParts[2], nPort, "HTTPGET", true);
                }
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During setting up asynchronous HTTP GET communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: Setting up asynchronous HTTP GET communication transaction.", exError);
            }
			
			return boolTransSetup;
		}

        /// <summary>
		/// 	Receiver Function for Setup to Send Synchronous HTTP GET Messages
		/// </summary>
		/// <param name="strMessage">Message Containing Information on How to Start HTTP Transmission</param>
		/// <returns>True If HTTP Transmission was Setup Successfully, Else False</returns>
		private bool StartHTTPGetSync(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nNewTransID = 0,    /* New Transaction ID */
                nPort = ssConfig.DefaultHTTPPort;          
                                    /* New Transaction's Port */
            bool boolTransSetup = false;
                                    /* Identicator That Transaction Has Been Setup */

            try {

                if (int.TryParse(astrMsgParts[1], out nNewTransID)) {

                    if (astrMsgParts.Length > 3) {

                        int.TryParse(astrMsgParts[3], out nPort);
                    }
                    
                    boolTransSetup = StartHTTPTrans(nNewTransID, astrMsgParts[2], nPort, "HTTPGET", false);
                }
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During setting up synchronous HTTP GET communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: Setting up synchronous HTTP GET communication transaction.", exError);
            }
			
			return boolTransSetup;
		}
		
		/// <summary>
		/// 	Receiver Function That Sets to Use SSL (Defaults to False)
		/// </summary>
		/// <param name="nTransID">Communication Transmission ID</param>
		/// <param name="boolUse">Indicator to Use SSL</param>
		private bool UseSSL(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolUse = false,
                 boolUpdated = false;  
                                    /* Indicator to Use SSL and That Setting was Updated */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
                    
                    if (bool.TryParse(astrMsgParts[2], out boolUse)) {

			            if (dcCommTrans.ContainsKey(nTransID)) {

                			dcCommTrans[nTransID].SSL = boolUse;
                            boolUpdated = true;
			            }
                        else {
                    
                            // Log Error
        		            ServerApp.Log("Action: During setting if using SSL for transmission communications, an invalid transaction ID send. Message: " + strMsg);
                        }
			        }
                    else {
                    
                        // Log Error
        		        ServerApp.Log("Action: During setting if using SSL for transmission communications. Error: Getting the indicator to use SSL failed from client message failed. Message: " + strMsg);
                    }
			    }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting if using SSL for transmission communications. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting if using SSL for transmission communications.", exError);
            }

            return boolUpdated;
		}

		/// <summary>
		/// 	Receiver Function to Add to Queue of Messages to be Sent Through Stream (Will Not Store Messages That Have Stream Reserved Characters)
		/// </summary>
		/// <param name="strMessage">Message Containing Information on Message to Send</param>
		/// <returns>True If Stream Message was Processed Successfully, Else False</returns>
		private bool AddStreamMsg(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolStored = false;/* Indicator That Message was Stored */

            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {

			        if (dcCommTrans.ContainsKey(nTransID)) {
				
				        boolStored = dcCommTrans[nTransID].AddStreamMsg(astrMsgParts[2]);
			        }            
                } 
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During adding message to stream communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During adding message to stream communication transaction.", exError);
            }
			
			return boolStored;
		}

		/// <summary>
		/// 	Receiver Function to Process Direct Messages
		/// </summary>
		/// <param name="strMsg">Message Containing Information on Direct Information from Client</param>
        /// <returns>
        ///     True If Indicator That Message was Processed, 
        ///     Else False If Not Processed or Message Did Not Have Specified Designation for Processing
        /// </returns>
		private bool ProcessDirectMsg(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
            string strDesignSelected = ssConfig.DirectMsgDesignSelected;  
                                    /* Selected Designation for Only Processing Message For */
            bool boolDirectMsgProcess = false;
                                    /* Indicator That Message was Processed */

            try {

                /* If Direct Message Has Specified Designation or No Designation Selected */
			    if (strDesignSelected == astrMsgParts[1] || strDesignSelected == "") {

                    /* If Send to All Client, Place in Queue, Else See If Transfering Message Out */
                    if (ssConfig.DirectMsgSendAllClients) {

                        qstrMsgReceived.Enqueue(strMsg);
                    }

                    boolDirectMsgProcess = true;
			    }         
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During processing direct message.", exError);
            }

            return boolDirectMsgProcess;
		}

		/// <summary>
		/// 	Receiver Method for Setting Processing Page for HTTP POST and GET Transmissions
		/// </summary>
		/// <param name="strMsg">Message Containing Information on Page to Set</param>
		/// <returns>True If HTTP Processing Page was Set Successfully, Else False</returns>
		private bool SetHTTPProcessPage(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolSet = false;   /* Indicator That Processing Page was Sent */

            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
				
				        dcCommTrans[nTransID].SetHTTPProcessPage(astrMsgParts[2]);
                        boolSet = true;
			        }
                } 
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During setting up processing page for HTTP communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting up processing page for HTTP communication transaction.", exError);
            }
			
			return boolSet;
		}
				
		/// <summary>
		/// 	Receiver Function That Adds a Variable and its Value to the Next Message Being Sent Through HTTP Transmission
		/// </summary>
		/// <param name="strMsg">Message Containing Information on Variable and Value to Set</param>
		/// <returns>True If HTTP Processing Variable and Value was Set Successfully, Else False</returns>
		private bool AddHTTPMsgData(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolAdded = false; /* Indicator That Message Data was Added */
			    
            try {

                if (int.TryParse(astrMsgParts[1], out nTransID)) {

			        if (dcCommTrans.ContainsKey(nTransID)) {
				
				        boolAdded = dcCommTrans[nTransID].AddHTTPMsgData(astrMsgParts[2], astrMsgParts[3]);
			        }
                } 
                else {
                
                    // Log Error
        		    ServerApp.Log("Action: During variable and value for HTTP communication transaction. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During variable and value for HTTP communication transaction.", exError);
            }
			
			return boolAdded;
		} 

		/// <summary>
		/// 	Sender Function to Send Stored HTTP Messages
		/// </summary>
		/// <param name="strMsg">Message Containing Information on Transmission to Send</param>
		/// <returns>True If HTTP Transmission was Sent Successfully, Else False</returns>
        private bool SendHTTP(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0,       /* Transaction ID */
			    nRespID = 0;        /* Response ID to Track Response to Sent Transmission */
			bool boolSent = false;  /* Indicator That Message was Sent */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
                    
                    if (int.TryParse(astrMsgParts[2], out nRespID)) {

			            if (dcCommTrans.ContainsKey(nTransID)) {
				
				            if (dcCommTrans[nTransID].Async) {
					
					            new Thread(() => GetHTTPResponse(nTransID, nRespID)).Start();
				            }
				            else {
				
					            GetHTTPResponse(nTransID, nRespID);
                            }

                            boolSent = true;
                        }
                        else {
                    
                            // Log Error
        		            ServerApp.Log("Action: During sending HTTP communications, an invalid transaction ID send. Message: " + strMsg);
                        }
			        }
                    else {
                    
                        // Log Error
        		        ServerApp.Log("Action: During sending HTTP communications. Error: Getting the HTTP tracking message response ID from client message failed. Message: " + strMsg);
                    }
			    }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During sending HTTP communications. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During sending HTTP communications.", exError);
            }
									
			return boolSent;
		}
		
		/// <summary>
		/// 	Gets Response Returned from Send Message, and Sending Message to Client in Send Queue 
		/// </summary>
		/// <param name="nTransID">Communication Transmission ID</param>
		/// <param name="nRespID">ID for Referencing Response</param>
		private void GetHTTPResponse(int nTransID, int nRespID) {
		
			if (dcCommTrans.ContainsKey(nTransID)) {
                
                Send(RegisterMsgTracking("HTTPRESPONSE") + strMsgPartEndChars + nTransID.ToString() + strMsgPartEndChars + nRespID.ToString() + strMsgPartEndChars + 
                     dcCommTrans[nTransID].SendHTTP()); 
			}
		}
		
		/// <summary>
		/// 	Receiver Function That Clears Next Message Being Sent Through HTTP Transmission
		/// </summary>
        /// <param name="strMsg">Message Containing Information on Transmission to Clear</param>
		/// <returns>True If HTTP Transmission Information was Cleared Successfully, Else False</returns>
		private bool ClearHTTPMsgData(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
            CommTrans ctSelect = null; 
                                    /* Selected Communication Transmission */
			int nTransID = 0;       /* Transaction ID */
			bool boolCleared = false;  
                                    /* Indicator That HTTP Transmission Information was Cleared */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
			
                        ctSelect = dcCommTrans[nTransID];

				        ctSelect.ClearHTTPMsgData();

                        ctSelect.AddHTTPMsgData(ssConfig.ClientIDVar, nClientID.ToString());
                        ctSelect.AddHTTPMsgData(ssConfig.GroupIDVar, strGroupID);
                        ctSelect.AddHTTPMsgData(ssConfig.ClientIPAddressVar, strIPAddress);
                        ctSelect.AddHTTPMsgData(ssConfig.TransactionIDVar, nTransID.ToString());

                        boolCleared = true;
			        }
			    }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During clearing HTTP communications variable and value information. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During sending HTTP communications variable and value information.", exError);
            }

            return boolCleared;
		}

		/// <summary>
		/// 	Gets a Waiting Message from Stream, Before Deleting it from the Wait Queue 
		/// </summary>
		public void GetStreamMsgs() {
            
            string strStreamMsg = "";
                                    /* Stream Message */

            foreach (KeyValuePair<int, CommTrans> kvpSelect in dcCommTrans) {
            
                strStreamMsg = kvpSelect.Value.GetStreamMsg();

                if (strStreamMsg != "") {

                    Send(RegisterMsgTracking("STREAMMSG") + strMsgPartEndChars + kvpSelect.Key.ToString() + strMsgPartEndChars + strStreamMsg);
                }
            }
		}
		
		/// <summary>
		/// 	Receiver Function That Sets Server Name for Stream SSL (Defaults to Hostname)
		/// </summary>
        /// <param name="strMsg">Message Containing Information on Transmission to Clear</param>
		/// <returns>True If Stream Transmission SSL Server Name was Set Successfully, Else False</returns>
		private bool SetStreamSSLServerName(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolSet = false;   /* Indicator That Transaction Stream SSL Server Name was Set */

            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
			
				        dcCommTrans[nTransID].StreamSSLServerName = astrMsgParts[2]; 
                        boolSet = true; 
                    }
			    }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting stream communications transmissions SSL server name. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting stream communications transmissions SSL server name.", exError);
            }

            return boolSet;
		}

		/// <summary>
	    ///     Sender Function That Gets Path and Name of Selected Download File
	    /// </summary>
		/// <param name="strMsg">Message Containing Information on Transmission to Send</param>
	    /// <returns>True If File was Downloaded, Else False</returns>
		private bool GetStreamFile(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			bool boolSent = false;  /* Indicator That File was Sent */

            try {
            
                if (astrMsgParts[1] != "") {

                    SendFile(astrMsgParts[1]);
                    boolSent = true;
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During starting file download.", exError);
            }

            return boolSent;
		}

        /// <summary>
        ///     Sender Function That Sends List of Downloaded Files Designations to Client App
        /// </summary>
        public void GetStreamFileList() {

            GetStreamFileList("");
        }
		
		/// <summary>
	    ///     Sender Function That Sends List of Downloaded Files Designations to Client App
	    /// </summary>
		/// <param name="strMsg">Message Containing Information on Transmission to Send</param>
	    /// <returns>True</returns>
	    private bool GetStreamFileList(string strMsg) {

            Send(RegisterMsgTracking("STREAMFILELIST") + strMsgPartEndChars + "{\"STREAMFILELIST\": [\"" + string.Join("\", \"", ssConfig.GetDownloadFileListDesigns()) + "\"]}");
            return true;
		}

		/// <summary>
	    /// 	Receiver Function That Sets Separator Characters for Parts of Stream Message, Default is "|||"
	    /// </summary>
		/// <param name="strMsg">Message Containing New Character to be Set</param>
	    /// <returns>True If Stream Transmission Message Separator Characters was Set, Else False</returns>
	    private bool SetStreamMsgSeparator(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolSet = false;   /* Indicator That Stream Transmission Message Part Indicator was Set */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
			
				        dcCommTrans[nTransID].SetStreamMsgSeparator(astrMsgParts[2]);
                        boolSet = true;
			        }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting stream communications transmissions message part separator. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting stream communications transmissions message part separator.", exError);
            }

            return boolSet;
	    }
	    
	    /// <summary>
	    /// 	Receiver Function That Sets Starting Characters of Stream Message, Default is "%-&>"
	    /// </summary>
		/// <param name="strMsg">Message Containing Information on New Character to be Set</param>
	    /// <returns>True If Stream Transmission Message Start Characters was Set, Else False</returns>
	    private bool SetStreamMsgStart(string strMsg) {
            
            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolSet = false;   /* Indicator That Stream Transmission Message End Indicator was Set */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
			
				        dcCommTrans[nTransID].SetStreamMsgStart(astrMsgParts[2]);
                        boolSet = true;
                    }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting stream communications transmissions message start indicator. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting stream communications transmissions message start indicator.", exError);
            }

            return boolSet;
	    }

	    /// <summary>
	    /// 	Receiver Function That Sets Ending Characters of Stream Message, Default is "<@^$"
	    /// </summary>
		/// <param name="strMsg">Message Containing Information on New Character to be Set</param>
	    /// <returns>True If Stream Transmission Message End Characters was Set, Else False</returns>
	    private bool SetStreamMsgEnd(string strMsg) {
            
            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolSet = false;   /* Indicator That Stream Transmission Message End Indicator was Set */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
			
				        dcCommTrans[nTransID].SetStreamMsgEnd(astrMsgParts[2]);
                        boolSet = true;
                    }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting stream communications transmissions message end indicator. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting stream communications transmissions message end indicator.", exError);
            }

            return boolSet;
	    }
	    
	    /// <summary>
	    /// 	Receiver Function That Sets Ending Characters of Stream Message, Default is '\0'
	    /// </summary>
		/// <param name="strMsg">Message Containing Information on New Character to be Set</param>
	    /// <returns>True If Stream Transmission Message Filler Character was Set, Else False</returns>
	    private bool SetStreamMsgFiller(string strMsg) {
            
            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolSet = false;   /* Indicator That Stream Transmission Message Character Filler was Set */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
                    if (astrMsgParts[2].Length == 1) {

			            if (dcCommTrans.ContainsKey(nTransID)) {
			
				            dcCommTrans[nTransID].SetStreamMsgFiller(astrMsgParts[2].ToCharArray()[0]);
                            boolSet = true;
                        }
                    }
                    else {
                        
                        // Log Error
        		        ServerApp.Log("Action: During setting stream communications transmissions message filler character. Error: The value to set the message filler character with was larger than one character. Message: " + strMsg);
                    }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting stream communications transmissions message filler character. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting stream communications transmissions message filler character.", exError);
            }

            return boolSet;
	    }
		
		/// <summary>
		/// 	Receiver Function for Closing Specified Transmission
		/// </summary>
        /// <param name="strMsg">Message Containing Information on Transmission to Send</param>
	    /// <returns>True If Transmission was Closed, Else False</returns>
		private bool TransClose(string strMsg) {
            
            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
			bool boolRemoved = false;  
                                    /* Indicator That Transmission was Removed */
									
            try {
                
                if (int.TryParse(astrMsgParts[1], out nTransID)) {
			
			        if (dcCommTrans.ContainsKey(nTransID)) {
			
				        dcCommTrans[nTransID].CloseStream();
				        dcCommTrans.Remove(nTransID);
                        boolRemoved = true;
			        }
			    }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During closing transmission communications. Error: Getting the new transaction ID from client message failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During closing transmission communications.", exError);
            }

            return boolRemoved;
		}
	    
	    ///	<summary>
	    /// 	Sending Function That Gets Last Error Message from Each Transmission
	    /// </summary>
		private void GetLastError() {
	                
            string strErrorMsg = "";/* Error Message */
            
            foreach (KeyValuePair<int, CommTrans> kvpSelect in dcCommTrans) {

                strErrorMsg = kvpSelect.Value.LastError;

                if (strErrorMsg != "") {
            
                    Send(RegisterMsgTracking("LOGERROR") + strMsgPartEndChars + kvpSelect.Key.ToString() + strMsgPartEndChars + strErrorMsg);
                }
            }
	    }

        /// <summary>
        /// 	Receiver Function for Processing Client Errors
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Client Error</param>
        /// <returns>True</returns>
        private bool ProcessClientError(string strMsg) {

            ssConfig.TransferClientMsgOut(strMsg);
            return true;
        }

        /// <summary>
        ///     Checks Received Ping Time
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Transmission to Send</param>
        /// <returns>True</returns>
        private bool PingCheck(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
            long lNewPingTime = 0;  /* New Time of Ping Length */
            bool boolPinged = true; /* Indicator That Ping was Calculated */ 

            if (long.TryParse(astrMsgParts[1], out lNewPingTime)) {

                /* If Doing Manditory Ping Check and Current Ping is From it, Exit it */
                if (lPingCheckInMillis > 0 && lNewPingTime == lPingCheckInMillis) {

                    lPingCheckInMillis = 0;
                }

                lPingLastInMillis = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - lNewPingTime;
                ServerApp.Log("Client, ID: " + nClientID.ToString() + " pinged at: " + lPingLastInMillis.ToString() + ".");
            }
            else {
                
                ServerApp.Log("Client received ping check failed on conversion of send value, value: " + astrMsgParts[1]);
                boolPinged = false;
            }

            return boolPinged;
        }
        
        /// <summary>
        ///     Register Data Execution Process by Transaction ID If Not Already and Store Any Optional Parameters 
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Data Process to Register And Get Optional Parameters</param>
        /// <returns>True If Parameter was Registered, Else False</returns>
        public bool RegisterDataExecution(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0;       /* Transaction ID */
            string strDesignation = "";
                                    /* Database Designation */
            DatabaseExecutor deFound;
                                    /* Found Data Executor for Designed Database Query or Statement */
            int nMsgPartCount = astrMsgParts.Length; 
                                    /* Count of Parameters for Data Process */
            bool boolProcessed = false;  
                                    /* Indicator That Transmission was Processed */
            int nCounter = 0;       /* Counter for Loop */
                
            try { 

                if (int.TryParse(astrMsgParts[1], out nTransID)) {

                    if ((strDesignation = astrMsgParts[2]).Length > 0) {

                        deFound = ServerApp.Settings.FindDataDesignationOwner(strDesignation);

                        if (deFound != null) {

                            if (nMsgPartCount > 3) {

                                for (nCounter = 3; nCounter + 1 < nMsgPartCount; nCounter += 2) {

                                    deFound.SetParameter(nClientID, nTransID, astrMsgParts[nCounter], astrMsgParts[nCounter + 1]);
                                }
                            }

                            deFound.SetParameter(nClientID, nTransID, ServerApp.Settings.ClientIDVar, nClientID.ToString());
                            deFound.SetParameter(nClientID, nTransID, ServerApp.Settings.GroupIDVar, strGroupID);
                            deFound.SetParameter(nClientID, nTransID, ServerApp.Settings.ClientIPAddressVar, strIPAddress);
                            deFound.SetParameter(nClientID, nTransID, ServerApp.Settings.TransactionIDVar, nTransID.ToString());

                            lock (ltdeExecutors) {

                                if (!ltdeExecutors.Contains(deFound)) {

                                    ltdeExecutors.Add(deFound);
                                }

                                boolProcessed = true;
                            }
                        }
                    }
                    else {
                        
                        // Log Error
        		        ServerApp.Log("Action: During setting up data execution and/or parameters. Error: Parsing designation for data query or statement failed. Message: " + strMsg);
                    }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setting up data execution and/or parameters. Error: Parsing transaction ID failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setting up data execution and/or parameters.", exError);
            }
			
			return boolProcessed;
        }
        
        /// <summary>
        ///     Register Any Data Executions with Data Maps
        /// </summary>
        public void RegisterDataMaps() {
                
            try { 
            
                List<DatabaseExecutor> ltDataMaps = ServerApp.Settings.GetDataTransactionWithDataMaps();

                if (ltDataMaps.Count > 0) {

                    lock (ltdeExecutors) {

                        foreach (DatabaseExecutor deSelect in ltDataMaps) { 

                            if (!ltdeExecutors.Contains(deSelect)) {

                                deSelect.StartDataMaps();

                                ltdeExecutors.Add(deSelect);
                            }
                        }
                    }
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During registering data executions with data maps.", exError);
            }
        }

        /// <summary>
        ///     Process Data Execution
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Data Process to Register And Get Optional Parameters</param>
        /// <returns>True If Processed, Else False</returns>
        public bool ProcessDataExecution(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			int nTransID = 0,       /* Transaction ID */
                nRespID = 0;        /* Response ID */
            string strDesignation = "";
                                    /* Database Designation */
            bool boolAsync = true;  /* Indicator to Run Asynchronously */
            DatabaseExecutor deFound;
                                    /* Found Data Executor for Designed Database Query or Statement */
			bool boolProcessed = false;  
                                    /* Indicator That Transmission was Processed */
                
            try { 

                if (int.TryParse(astrMsgParts[1], out nTransID)) {

                    if (int.TryParse(astrMsgParts[2], out nRespID)) {

                        if ((strDesignation = astrMsgParts[3]).Length > 0) {

                            if (bool.TryParse(astrMsgParts[4], out boolAsync)) {
                            
                                deFound = ServerApp.Settings.FindDataDesignationOwner(strDesignation);

                                if (deFound != null) {

                                    lock (ltdeExecutors) {

                                        if (!ltdeExecutors.Contains(deFound)) {

                                            ltdeExecutors.Add(deFound);
                                        }

                                        boolProcessed = deFound.Execute(nClientID, nTransID, nRespID, strDesignation, boolAsync);
                                    }
                                }
                            }
                            else {
                        
                                // Log Error
        		                ServerApp.Log("Action: During processing data execution. Error: Parsing indicator to run asynchronously failed. Message: " + strMsg);
                            }
                        }
                        else {
                        
                            // Log Error
        		            ServerApp.Log("Action: During processing data execution. Error: Parsing designation for data query or statement failed. Message: " + strMsg);
                        }
                    }
                    else {
                    
                        // Log Error
        		        ServerApp.Log("Action: During processing data execution. Error: Parsing transaction ID failed. Message: " + strMsg);
                    }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During processing data execution. Error: Parsing transaction ID failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During processing data execution.", exError);
            }
			
			return boolProcessed;
        }

        /// <summary>
        ///     Replay Messages by Sequence Number
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Message Sequence Number to Replay From</param>
        /// <returns>True If Processed, Else False</returns>
        public bool ProcessMsgReplay(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			long lMsgSegNum = 0;    /* Message Sequence Number to Replay */
            bool boolProcessed = false;  
                                    /* Indicator That Transmission was Processed */
                
            try {

                if (long.TryParse(astrMsgParts[1], out lMsgSegNum)) {

                    boolProcessed = SendReplay(lMsgSegNum);
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During setup for replaying messages. Error: Parsing message transaction sequence failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During setup for replaying messages.", exError);
            }
			
			return boolProcessed;
        }

        /// <summary>
        ///     Client Check If Last Message was Received
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Message Sequence Number of Message to Check</param>
        /// <returns>True If Processed, Else False</returns>
        public bool ProcessMsgCheck(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
			long lMsgSegNum = 0;    /* Message Sequence Number to Replay */
            bool boolProcessed = false;  
                                    /* Indicator That Transmission was Processed */
                                    
            try { 

                if (long.TryParse(astrMsgParts[1], out lMsgSegNum)) {

                    /* If Checked Message Doesn't Exists, Then Check Missed Nothing, Exit with Success */
                    if (dictltBackups.ContainsKey(lMsgSegNum)) {

                        boolProcessed = SendReplay(lMsgSegNum);
                    }
                    else {

                        boolProcessed = true;
                    }
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During client message check. Error: Parsing message transaction sequence failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During client message check.", exError);
            }
			
			return boolProcessed;
        }

        /// <summary>
        ///     Process Confirmation to Switch to Using UDP Client Communications
        /// </summary>
        /// <param name="strMsg">Message Containing Information on Confirmation</param>
        /// <returns>True If Processed, Else False</returns>
        public bool ProcessUDPSwitch(string strMsg) {

            string[] astrMsgParts = strMsg.Split(new string[] { strMsgPartEndChars }, StringSplitOptions.None);
                                    /* Parts of Message for Conversion */
            bool boolSwitched = false,
                                    /* Indicator That Switch Occurred */
                 boolProcessed = false;  
                                    /* Indicator That Transmission was Processed */
                
            try {

                if (bool.TryParse(astrMsgParts[1], out boolSwitched)) {

                    /* If Checked Message Doesn't Exists, Then Check Missed Nothing, Exit with Success */
                    if (boolSwitched) {

                        boolUseTCPCom = false;
                    }
                    else {
                    
                        // Log Error
        		        ServerApp.Log("Action: During client UDP client switch confirmation. Error: Client confirmed switch failed.");
                    }

                    boolProcessed = true;
                }
                else {
                    
                    // Log Error
        		    ServerApp.Log("Action: During client UDP client switch confirmation. Error: Parsing message indicator failed. Message: " + strMsg);
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During client UDP client switch confirmation.", exError);
            }
			
			return boolProcessed;
        }

        /// <summary>
        ///     Resends Stored Message from Specified One If Exists
        /// </summary>
        /// <param name="lMsgSegNum">Sequence Number of Starting Message to Send</param>
        /// <returns>True If All Messages Resent from Starting One, Else False</returns>
        private bool SendReplay(long lMsgSegNum) {

//            List<long> ltlBackupSeqNums = new List<long>(dictltBackups.Keys);
                                    /* List of Backup Sequence Numbers */
            bool boolProcessed = false;
                                    /* Indicator That Transmission was Processed */
            string strMsgUpdate = "";
            int nTimeStartIndex = 0,
                nTimeEndIndex = 0;    

            try {

                if (dictltBackups.ContainsKey(lMsgSegNum)) {

                    foreach (byte[] abyteSelect in dictltBackups[lMsgSegNum]) {

                        strMsgUpdate = Encoding.UTF8.GetString(abyteSelect);
                        nTimeStartIndex = strMsgUpdate.IndexOf("-", strMsgUpdate.IndexOf("-") + 1);
                        nTimeEndIndex = strMsgUpdate.IndexOf(strMsgPartEndChars, nTimeStartIndex + 1);

                        if (nTimeEndIndex < 0) {

                            nTimeEndIndex = strMsgUpdate.IndexOf(strMsgEndChars, nTimeStartIndex + 1);
                        }

                        if (nTimeStartIndex >= 0 && nTimeEndIndex >= 0) {

                            nTimeStartIndex++;
                            nTimeEndIndex--;

                            Buffer.BlockCopy(Encoding.UTF8.GetBytes((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString()), 0, abyteSelect, nTimeStartIndex + 1, (nTimeEndIndex + 1) - (nTimeStartIndex + 1));
                                        
                            qbyteMsgReplay.Enqueue(abyteSelect);
                        }
                        else {

                            ServerApp.Log("Action: During replaying messages. Error: The sequence creation time could not extracted to be updated. Sequence #: " + lMsgSegNum.ToString());
                        }
                    }

                    boolProcessed = true;
                }
                else {

                    ServerApp.Log("Action: During replaying messages. Error: A message squence number was sent that does or no longer exists. Sequence #: " + lMsgSegNum.ToString());
                }
            }
            catch (Exception exError) {
            
                // Log Error
        		ServerApp.Log("Action: During replaying messages.", exError);
            }
			
			return boolProcessed;
        }

        /// <summary>
        /// 	Sends Result of Data Execution Process
        /// </summary>
        /// <param name="nTransID">Communication Transmission ID</param>
        /// <param name="dictstrDataResult">List of Data Execution Results</param>
        private void SendDataExecutionResult(int nTransID, Dictionary<int, string> dictstrDataResult) {

            foreach (KeyValuePair<int, string> kvpSelect in dictstrDataResult) {

                Send(RegisterMsgTracking("DATAEXECRETURN") + strMsgPartEndChars + nTransID.ToString() +
                     strMsgPartEndChars + kvpSelect.Key.ToString() +
                     strMsgPartEndChars + kvpSelect.Value);
            }
		}

        /// <summary>
        ///     Registers Message for Backup and adds Message Metadata to Message Type Name
        /// </summary>
        /// <param name="strMsgTypeName">Message Type Name</param>
        /// <returns>Message Type Name With Metadata Added</returns>
        private string RegisterMsgTracking(string strMsgTypeName) {

//            List<long> ltlMsgNums;   /* List of Message Tracking Numbers */

            lLastMsgSent++;

            dictltBackups.Remove(lLastMsgSent);
            dictltBackups.Add(lLastMsgSent, new List<byte[]>());

            if (dictltBackups.Count > ServerApp.Settings.MaxBackupLimit) {

                List<long> ltlMsgNums = new List<long>(dictltBackups.Keys);
                ltlMsgNums.Sort();

                dictltBackups.Remove(ltlMsgNums[0]);
            }

            return strMsgTypeName + "-" + (lLastMsgSent).ToString() + "-" + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();
        }
        /// <summary>
        ///     Sends Group Join Message to Client
        /// </summary>
        public void SendGroupJoinCreatedMsg() {
            
            Send(RegisterMsgTracking("GROUPJOINED"));
        } 

        /// <summary>
        ///     Sends Group Exit Message to Client
        /// </summary>
        public void SendGroupExitMsg() {

            Send(RegisterMsgTracking("GROUPEXITED"));
        }

        /// <summary>
        ///     Sends Host Status Notification Message to Client
        /// </summary>
        public void SendHostNoticeMsg() {

            Send(RegisterMsgTracking("HOSTNOTICE"));
        }


        /// <summary>
        ///     Gets Next Message to be Outward Passed
        /// </summary>
        /// <returns>The Next User Message</returns>
        public string GetNextMessage() {

            string strMsg = "";     /* Next User Message */

            if (HasMessage()) {

                strMsg = qstrMsgReceived.Dequeue();
            }

            return strMsg;
        }

        /// <summary>
        ///     Finds If Any User Messages Exists
        /// </summary>
        /// <returns>True If Any User Messages Exists, Else False</returns>
        public bool HasMessage() {

            return qstrMsgReceived.Count > 0;
        }

        /// <summary>
        ///     If Manditory Ping Check is Being Done
        /// </summary>
        /// <returns>True If In Process of Doing Manditory Ping Check, Else False</returns>
        public bool InPingCheck {

            get { 

                bool boolInPingCheck = false;

                if (lPingCheckInMillis > 0) {

                    boolInPingCheck = true;
                }

                return boolInPingCheck;
            }
        }

        /// <summary>
        ///     Gets Last Length Between Pings in Milliseconds
        /// </summary>
        /// <returns>Last Length of Time Between Pings in Milliseconds</returns>
        public long LastPingCheckLength {

            get {

                return lPingLastInMillis;
            }
        }

        /// <summary>
        ///     Gets If User Connection is Connected
        /// </summary>
        public bool Connected {
            
            get {

                return boolMsgProcess;
            }
        }

        /// <summary>
        ///     Gets Client's ID
        /// </summary>
        public int ID {

            get {

                return nClientID;
            }
        }

        /// <summary>
        ///     Gets Client's IP Address
        /// </summary>
        public string IP {

            get {

                return strIPAddress;
            }
        }

        /// <summary>
        ///     Gets Client's 'Peer-To-Peer' Assigned Port
        /// </summary>
        public int PeerToPeerPort {

            get {

                return nPeerToPeerPort;
            }
        }

        /// <summary>
        ///     Returns ID for Group That Client is Apart of If in a Group, Else Blank String
        /// </summary>
        public string GroupID {

            get {

                return strGroupID;
            }

            set {

                strGroupID = value;
            }
        }

        /// <summary>
        ///     Get Client's Port
        /// </summary>
        public int Port {

            get {

                return ((IPEndPoint)tcConnection.Client.RemoteEndPoint).Port;
            }
        }

        /// <summary>
        ///     Gets Client's Encryption Key
        /// </summary>
        public byte[] EncryptKey {

            get {

                return abyteEncryptKey;
            }
        }

        /// <summary>
        ///     Gets Client's Encryption IV Block
        /// </summary>
        public byte[] EncryptIV {

            get {

                return abyteEncryptIV;
            }
        }

        /// <summary>
        ///     Closes Sending Stream
        /// </summary>
        public void Close() {

            foreach (CommTrans ctSelected in dcCommTrans.Values) {
            
                ctSelected.CloseStream();
            }

            dcCommTrans.Clear();
            
            if (ssSecureSend != null) {

                lock (ssSecureSend) { 
			
				    ssSecureSend.Close();
                    ssSecureSend = null;
			    }
            }
            
            if (nsSender != null) {

                lock (nsSender) { 

                    nsSender.Close();
                    tcConnection.Close();

                    nsSender = null;
                    tcConnection = null;
                }
            }

            if (objUDPSecCon != null) {

                lock (objUDPSecCon) { 

                    ssConfig.UDPSecureClose(objUDPSecCon);
                    objUDPSecCon = null;
                }
            }

            if (ucConnect != null) {

                lock (ucConnect) { 

                    ucConnect.Close();
                    ucConnect = null;
                }
            }

            if (boolMsgProcess) {

                ServerApp.Log("Client disconnected, ID: " + nClientID); 
            }

            boolMsgProcess = false;
        }
    }

	/// <summary>
	/// 	Communication Transactions
	/// </summary>
	internal class CommTrans {

        private static ServerSettings ssConfig = ServerApp.Settings;
                                    /* Server Settings */
		private enum COMMTYPE { HTTPPOST, HTTPGET, STREAM };
									/* Type of Communication */
		private int nTransID = 0;	/* Communication Transaction ID */
		private string strHostName = "";
									/* Host Server Name */
        private int nServerPort = 0;/* Server Access Port */
        private COMMTYPE ctTransType;
                                    /* Transaction Type */
		private bool boolAsync = true;	
									/* Indicator to Not Wait for Response Communications */
		private string strHTTPData = "",
									/* HTTP Data Formatted for Sending with Communications */
					   strHTTPProcessPage = "";
									/* Page for Sending HTTP Transmissions */
		private Dictionary<int, string> dcHTTPResponses = new Dictionary<int, string>();
									/* List of Responses to HTTP Transmissions Stored by Response ID */
        private string strMsgPartEndChars = ssConfig.PartEndChars,
                       strMsgStartChars = ssConfig.StartChars,
	                   strMsgEndChars = ssConfig.EndChars;
	                                /* Characters That Symbolize the Part, Start, and End of a Stream Message */
	    private char charMsgFiller = '\0';
	                                /* Stream Message Filter */
	    private TcpClient tcConnector;
	                                /* Stream TCP Connector for Server */
	    private NetworkStream nsConnection;
	                                /* Stream for Transmissions to and from Server */
	    private SslStream ssSecConnection;	
	        						/* SSL Stream Connection for Transmissions to and from Server */
	    private Queue<string> qstrMsgsSend = new Queue<string>(),
	                          qstrMsgsReceived = new Queue<string>();
	                                /* List of Stream Messages to Send or Received */
	    private bool boolCommunicate = true;
	                                /* Indicator for Stream to Continue Communicating with Server */
	    private Dictionary<string, MemoryStream> dictFileStorage = new Dictionary<string, MemoryStream>(),
	                                /* Holder for Downloading Files from Stream */
	    										 dictFinishFileStorage = new Dictionary<string, MemoryStream>();
	                                /* Holder for Complete Downloaded Files from Stream */
	    private bool boolNotFileOp = true;
	                                /* Indicator That Operation is Related to Files for Stream */
		private enum STREAMRESPONSEOP { MESSAGE, FILESTART, FILEPART, FILEEND };
	                                /* Stream Response Operation List */
	    private string strLastError = "";	
	    							/* Last Error That Occurred */
	    private bool boolNoSSL = true;
	    							/* Indicator That Use SSL is Not Being Used */
	    private string strStreamSSLServerName = "";
	    							/* Name of the Server for SSL Certificate */
		
		/// <param name="nSetTransID">Transaction ID</param>
		/// <param name="strSetHostName">Host Server Name</param>
		/// <param name="nSetPort">Server Access Port</param>
		/// <param name="strSetTransType">Transaction Type</param>
		/// <param name="boolSetAsync">Indicator to Not Wait for Response Communications</param>
		public CommTrans(int nSetTransID, string strSetHostName, int nSetPort, string strSetTransType, bool boolSetAsync) {

            COMMTYPE ctSetTransType;   
                                    /* Type of Communication Transmission */

            if (Enum.TryParse(strSetTransType, out ctSetTransType)) { 

			    if (!strSetHostName.StartsWith("http://") && !strSetHostName.StartsWith("https://")) {
			
				    strSetHostName = "http://" + strSetHostName;
			    }			
			
			    if (strSetHostName.EndsWith("/")) {
			
				    strSetHostName = strSetHostName.Replace("/", "");
			    }
			
			    nTransID = nSetTransID;
			    strHostName = strSetHostName;
			    nServerPort = nSetPort;	
			    ctTransType = ctSetTransType;		
			    boolAsync = boolSetAsync;
			
			    strStreamSSLServerName = strSetHostName.Replace("http://", "").Replace("/", "");

                /* Setup Communication Transmission */
                switch (ctSetTransType) {

                    case COMMTYPE.STREAM: {

                        new Thread(new ThreadStart(StartStream)).Start();
                        break;
                    }
                    case COMMTYPE.HTTPGET: {

                        if (nSetPort == 0) { 
                        
                            if (boolNoSSL) { 

                                nServerPort = ssConfig.DefaultHTTPPort;
                            }
                            else { 
                                    
                                nServerPort = ssConfig.DefaultHTTPSSLPort;
                            }
                        }

                        break;
                    }
                    case COMMTYPE.HTTPPOST: {
                            
                        if (nSetPort == 0) { 
                        
                            if (boolNoSSL) { 

                                nServerPort = ssConfig.DefaultHTTPPort;
                            }
                            else { 
                                    
                                nServerPort = ssConfig.DefaultHTTPSSLPort;
                            }
                        }

                        break;
                    }
                }
            }
            else {

                CloseStream();
                strLastError = "Action: Starting communication transmission. Error: Invalid transmission type, '" + strSetTransType + "'.";
                ServerApp.Log(strLastError);
            }
		}
		
		/// <summary>
		/// 	Starts Stream
		/// </summary>
		private void StartStream() {
			
			if (ctTransType == COMMTYPE.STREAM) {
	
	            try {
						
	                tcConnector = new TcpClient(strHostName, nServerPort);
	                Communicate();
	            }
	            catch (Exception exError) {
	
	                CloseStream();
	                strLastError = "Action: Starting stream. Host: " + strHostName + ", Port: " + nServerPort + 
	                         	   ". Error: " + exError.Message;
                    ServerApp.Log(strLastError);
	            }
			}
		}
		
			/// <summary>
	    ///     Sends and Receives Stream Message from Server
	    /// </summary>
	    private void Communicate() {

            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                    /* Manage Stopping Thread */

	        try {
	
	            while (boolCommunicate) {
	
	                lock (tcConnector) {
	
	                    if (nsConnection == null) {
	                        
	        				nsConnection = tcConnector.GetStream();
	                    }
	
	                    lock (nsConnection) {
	
							if (boolNoSSL) {
			                
			                	DoCommunication();
			                }
			                else {
			                
			                	DoSSLCommunication();
			                }
	                    }
	                }

                    areThreadStopper.WaitOne(1);
	            }
	        }
	        catch (Exception exError) {
	
	        	strLastError = "Action: Downloading file through stream from Host: " + strHostName + ", Port: " + nServerPort + ".";
                ServerApp.Log(strLastError, exError);
	            CloseStream();
	        }
	    }
	    
	    /// <summary>
	    /// 	Processes Stream Communications
	    /// </summary>
	    private void DoCommunication() {
	    
	        byte[] abyteMessage;    /* Holder for Message Being Sent or Received */
	        int nReceiveAmount = 0, /* Amount of Data Recieved */
	            nOrigMsgStartIndex = 0,
	            nOrigMsgEndIndex = 0,
	                                /* The Start and End of the Message in the Original Byte Buffer */
	            nMsgLen = 0;        /* Length of the Actual Sent Message */
	        string strMsg = "";     /* Received Message Part */
	        string[] astrMsgsList,  /* List of Messages */
	                 astrMsgInfo;   /* List of Message Information */
	        STREAMRESPONSEOP sroSendOp;     
	        						/* Operation Sent By Message */
	        string strDownloadFile = "";
	                                /* Name of the Download File */
	        MemoryStream msHolder;  /* Holder for Selected Memory Stream */
	        string strFileLen = ""; /* String Version of File Length */
	        int nFileStartIndex = 0,
	            nFileEndIndex = 0,
	            nFileSendLen = 0;   /* Position in the File Message Where the File Starts, Ends, and the Length of the Send Peice */
	        bool boolFileEnd = false;
	                                /* Indicator that End of Download Message Has Occurred */
	        int nCounter = 0;       /* Counter for Loop */
	
	        if (qstrMsgsSend.Count > 0) {
	
	            /* Send Message */
	            abyteMessage = Encoding.UTF8.GetBytes(qstrMsgsSend.Dequeue()); 
			
				if (boolNoSSL) {
	            
	            	nsConnection.Write(abyteMessage, 0, abyteMessage.Length);
	            }
	            else {
	            
	            	ssSecConnection.Write(abyteMessage, 0, abyteMessage.Length);
	            }
	        } 
	
	        if (nsConnection.DataAvailable) {
	
	            /* Receive Response */
	            nReceiveAmount = tcConnector.ReceiveBufferSize;
	            abyteMessage = new byte[nReceiveAmount];
	
	            /* Get Response Message */
				if (boolNoSSL) {
	            	
	            	nsConnection.Read(abyteMessage, 0, nReceiveAmount);
	            }
	            else {
	            
	            	ssSecConnection.Read(abyteMessage, 0, nReceiveAmount);
	            }
	
	            /* Find Where the Actual Message Starts */
	            for (nCounter = 0; nCounter < nReceiveAmount; nCounter++) {
	            
	                if (abyteMessage[nCounter] != charMsgFiller) {
	
	                    nOrigMsgStartIndex = nCounter;
	                    nCounter = nReceiveAmount;
	                }
	            }
	
	            /* Find Where the Actual Message Ends */
	            for (nCounter = (nReceiveAmount - 1); nCounter >= 0; nCounter--) {
	            
	                if (abyteMessage[nCounter] != charMsgFiller) {
	
	                    nOrigMsgEndIndex = nCounter;
	                    nCounter = 0;
	                }
	            }
	
	            /* If There was Any Message, Find the Length */
	            if (nOrigMsgEndIndex > 0) {
	
	                nMsgLen = (nOrigMsgEndIndex + 1) - nOrigMsgStartIndex;
	            }
	
	            strMsg = Encoding.UTF8.GetString(abyteMessage, nOrigMsgStartIndex, nMsgLen);
	
	            if (strMsg != "") {
	
	                astrMsgsList = Regex.Split(strMsg, Regex.Escape(strMsgEndChars));
	
	                foreach (string strMsgSelect in astrMsgsList) {
	
	                    if (strMsgSelect != "") {
	
	                        astrMsgInfo = Regex.Split(strMsgSelect, Regex.Escape(strMsgPartEndChars));
	
	                        /* If a Valid Response Operation, If So Find If its Not a File Operation */
	                        if (Enum.TryParse(astrMsgInfo[0].Substring(strMsgStartChars.Length - 1), out sroSendOp)) {
	                        
	                            if (STREAMRESPONSEOP.FILESTART != sroSendOp && 
	                        	    STREAMRESPONSEOP.FILEPART != sroSendOp && 
	                        	    STREAMRESPONSEOP.FILEEND != sroSendOp) {
	
	                                boolNotFileOp = true;
	                            }
	                            else { 
	                        
	                                boolNotFileOp = false;
	                            }
	                        }
	
	                        if (boolNotFileOp) {
	
	                        	qstrMsgsReceived.Enqueue(astrMsgInfo[astrMsgInfo.Length - 1]);
	                        }
	                        else {
	
	                            /* If Loading a File Part */
	                            switch (sroSendOp) {
	                        
	                                case STREAMRESPONSEOP.FILEPART: {
	
	                                    strDownloadFile = astrMsgInfo[1];
	
	                                    /* Find Where the Actual Message Starts, Ends, and How Long it is */
	                                    nFileStartIndex = FindCharIndex(abyteMessage, astrMsgInfo[0] + strMsgPartEndChars + astrMsgInfo[1] + strMsgPartEndChars + astrMsgInfo[2] + strMsgPartEndChars, nOrigMsgStartIndex, nOrigMsgEndIndex, true);
	                                    nFileEndIndex = FindCharIndex(abyteMessage, strMsgEndChars, nFileStartIndex, nOrigMsgEndIndex, false);
	
	                                    if (nFileEndIndex > -1) {
	
	                                        nFileSendLen = nFileEndIndex - nFileStartIndex;
	                                    }
	                                    else if (nOrigMsgEndIndex > 0) {
	
	                                        nFileSendLen = (nOrigMsgEndIndex + 1) - nFileStartIndex;
	                                    }
	                                    else {
	
	                                        nFileSendLen = abyteMessage.Length - nFileStartIndex;
	                                    }
	
	                                    /* Load the Currently Sent Part */
	                                    dictFileStorage[strDownloadFile].Write(abyteMessage, nFileStartIndex, nFileSendLen);
	
	                                    break;
	                                }
	                                case STREAMRESPONSEOP.FILESTART: {
	                                    
	                                    strDownloadFile = astrMsgInfo[1];
	                                    strFileLen = astrMsgInfo[2];
	
	                                    if (dictFileStorage.ContainsKey(strDownloadFile)) {
	
	                                        dictFileStorage[strDownloadFile].Close();
	                                        dictFileStorage.Remove(strDownloadFile);
	                                    }
	    
	                                    try {
	
	                                        /* Find Where the Actual Message Starts, Ends, and How Long it is */
	                                        nFileStartIndex = FindCharIndex(abyteMessage, astrMsgInfo[0] + strMsgPartEndChars + strDownloadFile + strMsgPartEndChars + strFileLen + strMsgPartEndChars, nOrigMsgStartIndex, nOrigMsgEndIndex, true);
	                                        nFileEndIndex = FindCharIndex(abyteMessage, strMsgEndChars, nFileStartIndex, nOrigMsgEndIndex, false);
	
	                                        if (nFileEndIndex > -1) {
	
	                                            nFileSendLen = nFileEndIndex - nFileStartIndex;
	                                        }
	                                        else if (nOrigMsgEndIndex > 0) {
	
	                                            nFileSendLen = (nOrigMsgEndIndex + 1) - nFileStartIndex;
	                                        }
	                                        else {
	
	                                            nFileSendLen = abyteMessage.Length - nFileStartIndex;
	                                        }
	
	                                        msHolder = new MemoryStream(Int32.Parse(strFileLen));
	                                        msHolder.Write(abyteMessage, nFileStartIndex, nFileSendLen);
	                                        dictFileStorage.Add(strDownloadFile, msHolder);
	                                    }
	                                    catch {
	
	                                     //   strFileStatus = "Error: Starting file download through stream failed due to receiving invalid file length information.";
	                                        strLastError = "Action: Downloading file: " + strDownloadFile + " from Host: " + strHostName + ", Port: " + nServerPort + ".";
                                            ServerApp.Log(strLastError);
	                                    }
	
	                                    break;
	                                }
	                                case STREAMRESPONSEOP.FILEEND: {
	
	                                    strDownloadFile = astrMsgInfo[1];
	
	                                    /* Find Where the Actual Message Starts, Ends, and How Long it is */
	                                    nFileStartIndex = FindCharIndex(abyteMessage, astrMsgInfo[0] + strMsgPartEndChars + strDownloadFile + strMsgPartEndChars, nOrigMsgStartIndex, nOrigMsgEndIndex, true);
	                                    nFileEndIndex = FindCharIndex(abyteMessage, strMsgEndChars, nFileStartIndex, nOrigMsgEndIndex, false);
	
	                                    if (nFileEndIndex > -1) {
	
	                                        nFileSendLen = nFileEndIndex - nFileStartIndex;
	                                    }
	                                    else {
	
	                                        nFileSendLen = abyteMessage.Length - nFileStartIndex;
	                                    }
	
	                                    msHolder = dictFileStorage[strDownloadFile];
	                                    msHolder.Write(abyteMessage, nFileStartIndex, nFileSendLen);
	
	                                    /* If All of the File Has Been Downloaded, Put it in the File, End Download Either Way */
	                                    if (msHolder.Length == msHolder.Capacity) {
	
	                                        boolFileEnd = true;
	                                    }
	
	                                    break;
	                                }
	                                default: {
	
	                                    /* Else a Peice of the File Message Maybe Sent Without Beginning, Capture it */
	                                    nFileEndIndex = FindCharIndex(abyteMessage, strMsgEndChars, 0, nOrigMsgEndIndex, false);
	
	                                    if (nFileEndIndex > -1) {
	
	                                        nFileSendLen = nFileEndIndex;
	                                    }
	                                    else if (nOrigMsgEndIndex > 0) {
	
	                                        nFileSendLen = nOrigMsgEndIndex + 1;
	                                    }
	                                    else {
	
	                                        nFileSendLen = abyteMessage.Length;
	                                    }
	
	                                    msHolder = dictFileStorage[strDownloadFile];
	                                    msHolder.Write(abyteMessage, 0, nFileSendLen);
	
	                                    /* If All of the File Has Been Downloaded, Put it in the File, End Download Either Way */
	                                    if (msHolder.Length == msHolder.Capacity) {
	
	                                        boolFileEnd = true;
	                                    }
	
	                                    break;
	                                }
	                            }
	                        }
	
	                        /* If the End of the File Download Has Been Reached, Save Out to the File, and Close and Remove Download Assests */
	                        if (boolFileEnd) {
	                        	
	                        	dictFinishFileStorage.Add(strDownloadFile, dictFileStorage[strDownloadFile]);
	                        	dictFileStorage.Remove(strDownloadFile);
	                        	
	                            boolFileEnd = false;
	                            boolNotFileOp = true;
	                        }
	                    }
	                }
	            }
	
	            nOrigMsgStartIndex = 0;
	            nOrigMsgEndIndex = 0;
	            nMsgLen = 0;
	        }
	    }
	    
	    /// <summary>
	    /// 	Processes Stream Communications Through SSL
	    /// </summary>
	    private void DoSSLCommunication() {
	    
	    	lock (ssSecConnection) {
	    					
			    if (ssSecConnection == null) {
					
				    ssSecConnection = new SslStream(nsConnection, true, ValidateHTTPSSLCert);
                    ssSecConnection.AuthenticateAsClient(strStreamSSLServerName);
                }
	    	
	    		DoCommunication();
	    	}
	    }
		
		/// <summary>
		/// 	Adds to Queue of Messages to be Sent Through Stream
		/// </summary>
		/// <param name="strMsg">Message to Send</param>
		/// <returns>True If Message is Stored, Else False</returns>
		public bool AddStreamMsg(string strMsg) {
			
			bool boolStored = false;/* Indicator That Message was Stored */
			
			if (ctTransType == COMMTYPE.STREAM) {
					
			    qstrMsgsSend.Enqueue(strMsg); 
				boolStored = true;
			}
			
			return boolStored;
		}
		
		/// <summary>
		/// 	Sends Stored Messages Through HTTP Transmissions
		/// </summary>
        /// <returns>Returns Response from HTTP Transmission or Blank String</returns>
		public string SendHTTP() {
			 
            string strRetMsg = "";  /* Returned Message from HTTP Transmission */
//            string strSendURL = strHostName;
                                    /* URL to Send Message */
//            HttpWebRequest hwrSender;
                                    /* Send Message to Processing Page */
//            byte[] abyteDataEncode; /* Encoded Data */
//            Stream smPostDataSender;/* Stream for Sending Data with HTTP POST Method */
//            HttpWebResponse hwrRetriever;
                                    /* Retrieves Response Messages from Processing Page */
//            StreamReader srRetrieveRead;
                                    /* Reads Response Message from Retriever */

            try { 

                if (ctTransType == COMMTYPE.HTTPPOST || 
			        ctTransType == COMMTYPE.HTTPGET) {
				
				    string strSendURL = strHostName;
				    HttpWebRequest hwrSender;
				    byte[] abyteDataEncode;	
				    Stream smPostDataSender;
				    HttpWebResponse hwrRetriever;
				    StreamReader srRetrieveRead;
				
				    if (boolNoSSL) {
											
					    /* Setup URL with Port If Not Default, Process Page If Send, and Parameter Data If Using HTTP GET Method and Any Data was Collected */
					    if (nServerPort != 80 && !strSendURL.Contains(":" + nServerPort.ToString())) {
					
						    strSendURL += ":" + nServerPort;
					    }
				    }
				    else {
										
					    /* Else If Using SSL, Make Sure Settings and Default Port Are Correct */
					    strSendURL = strSendURL.Replace("http://", "https://");
	
					    if (nServerPort != 443 && !strSendURL.Contains(":" + nServerPort.ToString())) {
							
					        strSendURL += ":" + nServerPort;
					    }								
				    }
				
				    if (strHTTPProcessPage != "") {
										
					    strSendURL += strHTTPProcessPage;
				    }
										
				    if (ctTransType == COMMTYPE.HTTPGET && strHTTPData != "") {
											
					    strSendURL += "?" + strHTTPData;
				    }
									
				    /* Send Message to Processing Page */
				    hwrSender = (HttpWebRequest)WebRequest.Create(strSendURL);
				
				    /* If Using SSL, Set Default Settings and Callback Function for Checking If Any Certificate Error Occurred */
				    if (!boolNoSSL) {
				
					    hwrSender.Proxy = null;
					    hwrSender.Credentials = CredentialCache.DefaultCredentials;
					
					    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateHTTPSSLCert);
				    }
	
				    /* If Using HTTP POST Method and Any Data was Collected, Add to Message */
				    if (ctTransType == COMMTYPE.HTTPPOST && strHTTPData != "") {
				
					    abyteDataEncode = Encoding.ASCII.GetBytes(strHTTPData);
					
					    hwrSender.Method = "POST";
					    hwrSender.ContentType = "application/x-www-form-urlencoded";
					    hwrSender.ContentLength = abyteDataEncode.Length;

                        smPostDataSender = hwrSender.GetRequestStream();
                        smPostDataSender.Write(abyteDataEncode, 0, abyteDataEncode.Length);
                        smPostDataSender.Close();
				    }
					
                    try {

                        /* Get Response Message from Processing Page */
                        hwrRetriever = (HttpWebResponse)hwrSender.GetResponse();
                        srRetrieveRead = new StreamReader(hwrRetriever.GetResponseStream());
                        strRetMsg = srRetrieveRead.ReadToEnd();

                        srRetrieveRead.Close();
                        hwrRetriever.Close();
                    }
                    catch (WebException weError) {

                        ServerApp.Log("Action: Sending HTTP transmission. Host: " + strHostName + ", Port: " + nServerPort + ", Processing Page: " + strHTTPProcessPage +
                                        ". Error occurred: " + weError.Message);
                    }
				
				    ClearHTTPMsgData();
			    }
            }
	        catch (Exception exError) {
	
				strLastError = "Action: Sending HTTP transmission. Host: " + strHostName + ", Port: " + nServerPort + ", Processing Page: " + strHTTPProcessPage + 
				         	   ". Exception occurred: " + exError.Message;
                ServerApp.Log(strLastError);
	        }

            return strRetMsg;
		}
	
		/// <summary>
		/// 	Validate SSL Certificate Used in Transmissions
		/// </summary>
		/// <param name="objSender">Server Information</param>
		/// <param name="xcServerCert">Server SSL Certificate Information</param>
		/// <param name="xchInteractInfo">Information on HTTP Transmissions Interaction with Certificate</param>
		/// <param name="speErrorInfo">Error Information from Interaction</param>
		/// <returns>True If Server SSL Certificate is Valid, Else False</returns>
		private bool ValidateHTTPSSLCert(object objSender, X509Certificate xcServerCert, X509Chain xchInteractInfo, SslPolicyErrors speErrorInfo) {
		
			bool boolPass = false,  /* Indicator That Server SSL Certificate Passed Validation */
				 boolSelfSigned = false,
									/* Indicator That the Server SSL Certificate was Self-Signed */	
				 boolStatusCheckPass = true;
									/* Indicator That Server SSL Certificate Passed After the Chain Status Checks  */			
			
			/* If There was Any Errors and There is Information on Them */
			if (speErrorInfo != SslPolicyErrors.None || 
			    ((speErrorInfo & SslPolicyErrors.RemoteCertificateChainErrors) != 0 &&
				 (xchInteractInfo != null && xchInteractInfo.ChainStatus != null))) {
			
				if (xcServerCert.Subject == xcServerCert.Issuer) {
										
					boolSelfSigned = true;											
				}
				
				/* Go Through Certificate Interaction Information, If There were Errors and Certificate was not Self-Signed, Invalidate it */
				foreach (X509ChainStatus xcsResult in xchInteractInfo.ChainStatus) {
				
					if (!(boolSelfSigned && xcsResult.Status == X509ChainStatusFlags.UntrustedRoot) && 
						xcsResult.Status != X509ChainStatusFlags.NoError) {
											
						boolStatusCheckPass = false;
						break;
					}
				}
										
				boolPass = boolStatusCheckPass;
			}
			else {
			
				boolPass = true;
			}
			
			return boolPass;
		}
	
		/// <summary>
		/// 	Gets Response Returned from Send Message, Before Deleting it from Response Queue 
		/// </summary>
		/// <param name="nResponseID">ID for Referencing Response</param>
		/// <returns>Response Message as a String Indicated by the Response ID If It Exists, Else Blank String</returns>
		public string GetHTTPResponse(int nResponseID) {
		
			string strMsg = "";	 /* Response Message from Processing Page */
			
			if (ConfirmHTTPResponse(nResponseID)) {
			
				strMsg = dcHTTPResponses[nResponseID];
				dcHTTPResponses.Remove(nResponseID);
			}
			
			return strMsg;
		}
		
		/// <summary>
		/// 	Gets Confirmation That a Response Returned from Send Message 
		/// </summary>
		/// <returns>True If Response to Send Message Exists That is Indicated by the Response ID, Else False</returns>
		public bool ConfirmHTTPResponse(int nResponseID) {
		
			return dcHTTPResponses.ContainsKey(nResponseID);
		}
		
		/// <summary>
		/// 	Gets a Waiting Message from Stream, Before Deleting it from the Wait Queue 
		/// </summary>
		/// <returns>Message as a String If It Exists, Else Blank String</returns>
		public string GetStreamMsg() {
		
			string strMsg = "";	 /* Message from Stream */
			
			if (qstrMsgsReceived.Count > 0) {
				
				strMsg = qstrMsgsReceived.Dequeue();
			}
			
			return strMsg;
		}        
		
		/// <summary>
	    ///     Gets List of Downloaded Files
	    /// </summary>
	     /// <returns>List of Download File Names</returns>
	    public List<string> GetStreamFileList() {
	        
	     	return new List<string>(dictFinishFileStorage.Keys);
	    }
		
		/// <summary>
	    ///     Gets Downloaded File from Stream and Outputs File to Destination Directory, Before File is Removed
	    /// </summary>
	    /// <param name="strFileName">Name of the File</param>
	    /// <param name="strDistPath">Destination Directory Path for Downloaded File (If Blank, Defaults to Root of "C")</param>
	    /// <returns>True If File was Downloaded and Outputted Successfully, Else False</returns>
	    public bool GetStreamFile(string strFileName, string strDestPath) {
	
	        MemoryStream msFileStream;
	                                /* Stream for File */
	        FileStream fsFileAccess;/* Access to File for Creating Downloaded File */
			bool boolCompleted = false;
									/* Indicator That File Output was Successful */
	        
	        try {
	        	
	            if (dictFinishFileStorage.ContainsKey(strFileName)) {
	                
	                if (strDestPath == "") {
	                
	                	strDestPath = "C:/";
	                }
	                
	                if (!strDestPath.EndsWith("/")) {
	                
	                	strDestPath += "/";
	                }
	
	                msFileStream = dictFinishFileStorage[strFileName];
	                
	                fsFileAccess = new FileStream(strDestPath + strFileName, FileMode.Create, FileAccess.Write);
	                fsFileAccess.Write(msFileStream.GetBuffer(), 0, Convert.ToInt32(msFileStream.Length));
	                fsFileAccess.Close();
	                
	                dictFinishFileStorage.Remove(strFileName);
	                
	                boolCompleted = true;
	            }
	        }
	        catch (Exception exError) {
	
				strLastError = "Action: Saving file download from stream to local file. Host: " + strHostName + ", Port: " + nServerPort + ", Processing Page: " + strHTTPProcessPage + 
				         	   ". Exception occurred: " + exError.Message;
                ServerApp.Log(strLastError);
	        }
			
			return boolCompleted;									
	    }
	
	    /// <summary>
	    ///     Finds the Specifed Index of a String That is in a Byte Array
	    /// </summary>
	    /// <param name="abyteSearch">Byte Array to Search</param>
	    /// <param name="strSearchChar">String to Search for</param>
	    /// <param name="nStartIndex">Index to Start Search for in Byte Array</param>
	    /// <param name="nEndIndex">Length of Search in Byte Array</param>
	    /// <param name="boolAddSearchLen">Indicator That Index Should Include Length of the String</param>
	    /// <returns>If String is Found, the Index After it is Returned, Else Returns -1</returns>
	    private int FindCharIndex(byte[] abyteSearch, string strSearchChar, int nStartIndex, int nEndIndex, bool boolAddSearchLen) {
	
	        int nSearchMax = abyteSearch.Length,
	                                /* Maximum Number of Search Characters */
	            nSearchCharLen = strSearchChar.Length,
	                                /* Length of the Send Search Characters */
	            nFoundIndex = -1,   /* The Found Index from the Search */
	            nCounter = 0;       /* Counter for Loop */
	
	        try {
	
	            for (nCounter = nStartIndex; nCounter < nEndIndex; nCounter++) {
	
	                if (nCounter + nSearchCharLen <= nSearchMax) {
	
	                    if (Regex.Match(Encoding.UTF8.GetString(abyteSearch, nCounter, nSearchCharLen), Regex.Escape(strSearchChar)).Success) {
	
	                        nFoundIndex = nCounter;
	
	                        if (boolAddSearchLen) {
	
	                            nFoundIndex += nSearchCharLen;
	                        }
	
	                        nCounter = nEndIndex;
	                    }
	                }
	                else {
	
	                    nCounter = nEndIndex;
	                }
	            }
	        }
	        catch (Exception exError) {
	
				strLastError = "Action: Parsing file download through stream. Host: " + strHostName + ", Port: " + nServerPort + ", Processing Page: " + strHTTPProcessPage + 
				         	   ". Error: Index search failed on string. Exception occurred: " + exError.Message;
                ServerApp.Log(strLastError);
	        }
	
	        return nFoundIndex;
	    }
		
		/// <summary>
		/// 	Sets Processing Page for HTTP Transmissions
		/// </summary>
		/// <param name="strProcessPage">Name and Path of Processing Page on Host Server</param>
		public void SetHTTPProcessPage(string strProcessPage) {
		
			/* Remove Server Information from Processing Page, and Make Sure It Starts with a Directory Slash "/" */
			if (strProcessPage.StartsWith("http://")) {
			
				strProcessPage = strProcessPage.Replace("http://", "");
			}			
			
			if (strProcessPage.StartsWith("https://")) {
			
				strProcessPage = strProcessPage.Replace("https://", "");
			}
			
			if (strProcessPage.StartsWith(strHostName + "/")) {
			
				strProcessPage = strProcessPage.Replace(strHostName + "/", "/");
			}			
			
			if (!strProcessPage.StartsWith("/")) {
			
				strProcessPage = "/" + strProcessPage;
			}
			
			strHTTPProcessPage = strProcessPage;
		}
		
		/// <summary>
		/// 	Adds a Variable and its Value to the Next Message Being Sent ThrougH HTTP Transmission
		/// </summary>
		/// <param name="strVarName">Message Variable to be Sent</param>
		/// <param name="strValue">Value of Message Variable to be Sent</param>
		/// <returns>True If Indicator That the Variable was of Valid Format to be Used in the Next Message and Communication Transmission ID Valid, Else False</returns>
		public bool AddHTTPMsgData(string strVarName, string strValue) {
	
			bool boolAdded = false; /* Indicator That Message Data was Added */
			
			if (!strVarName.Contains(" ")) {
				
				/* If Value Has Inner Spaces and No Quotation Marks, Add Them */
				if (strValue.Trim().Contains(" ") && !((strValue.StartsWith('"'.ToString()) && strValue.EndsWith('"'.ToString())) ||
				                                       (strValue.StartsWith("'") && strValue.EndsWith("'")))) {
				
					strValue = '"' + strValue + '"';
				}
				
				if (strHTTPData != "") {
					
					strHTTPData += "&" + strVarName + "=" + strValue;
				}
				else {
				
					strHTTPData = strVarName + "=" + strValue;
				}
				
				boolAdded = true;
			}
			
			return boolAdded;
		}
		
		/// <summary>
		/// 	Clears Next Message Being Sent ThrougH HTTP Transmission
		/// </summary>
		public void ClearHTTPMsgData() {
			
			strHTTPData = "";
		}
	
	    /// <summary>
	    ///   Closes Off Stream Communications
	    /// </summary>
	    public void CloseStream() {
	
	        boolCommunicate = false;
	
	        if (ssSecConnection != null) {
	        
	        	ssSecConnection.Close();
	        }
	        
	        if (nsConnection != null) {
	        
	            nsConnection.Close();
	        }
	
	        if (tcConnector != null) {
	
	            tcConnector.Close();
	        }
	    }
	    
	    /// <summary>
	    /// 	Sets Separator Characters for Parts of Stream Message, Default is "|||"
	    /// 	(Do Not Set Unless Also Changing the Same Setting on RevCommServer or Sending Server,
	    /// 	 Else Stream Will Fail)
	    /// </summary>
	    /// <param name="strSetMsgPartEndChars">Series of Character for Identifying Part of Stream Messages</param>
	    public void SetStreamMsgSeparator(string strSetMsgPartEndChars) {
	    	
	    	strMsgPartEndChars = strSetMsgPartEndChars;
	    }
	    
	    /// <summary>
	    /// 	Sets Starting Characters of Stream Message, Default is "%-&>"
	    /// 	(Do Not Set Unless Also Changing the Same Setting on RevCommServer or Sending Server,
	    /// 	 Else Stream Will Fail)
	    /// </summary>
	    /// <param name="strSetMsgEndChars">Series of Character for Identifying Start of Stream Messages</param>
	    public void SetStreamMsgStart(string strSetMsgStartChars) {
	    	
	    	strMsgStartChars = strSetMsgStartChars;
	    }
	    
	    /// <summary>
	    /// 	Sets Ending Characters of Stream Message, Default is "<@^$"
	    /// 	(Do Not Set Unless Also Changing the Same Setting on RevCommServer or Sending Server,
	    /// 	 Else Stream Will Fail)
	    /// </summary>
	    /// <param name="strSetMsgEndChars">Series of Character for Identifying End of Stream Messages</param>
	    public void SetStreamMsgEnd(string strSetMsgEndChars) {
	    	
	    	strMsgEndChars = strSetMsgEndChars;
	    }
	    
	    /// <summary>
	    /// 	Sets Ending Characters of Stream Message, Default is '\0'
	    /// 	(Do Not Set Unless Also Changing the Same Setting on RevCommServer or Sending Server,
	    /// 	 Else Stream Will Fail)
	    /// </summary>
	    /// <param name="charSetMsgEndFiller">Character for Filling in Empty Parts of Stream Messages</param>
	    public void SetStreamMsgFiller(char charSetMsgEndFiller) {
	    	
	    	charMsgFiller = charSetMsgEndFiller;
	    }
	    
	    ///	<summary>
	    /// 	Gets Last Error Message
	    /// </summary>
	    public string LastError {
	    
	    	get {
	    		
	    		string strErrorMsg = strLastError;
	    							/* Last Error Message */
	    							
	    		strLastError = "";
	    	
	    		return strErrorMsg;
	    	}
	    }
	    
	    /// <summary>
	    /// 	Gets and Sets If Using SSL
	    /// </summary>
	    public bool SSL {
	    
	    	get {
	    	
	    		bool boolUseSSL = false;
	    							/* Indicator That SSL is Being Used */
	    							
	    		if (!boolNoSSL) {
	    								
	    			boolUseSSL = true;        							
	    		}
	    							
	    		return boolUseSSL;
	    	}
	    	
	    	set {
	    		
	    		if (value) {
	    								
	    			boolNoSSL = false;

                    if (nServerPort == ssConfig.DefaultHTTPPort) { 
                        
                        nServerPort = ssConfig.DefaultHTTPSSLPort;
                    }

                    if (strHostName.StartsWith("http://")) {

                        strHostName = strHostName.Replace("http://", "https://");
                    }
	    		}
	    		else {
	    		
	    			boolNoSSL = true; 

                    if (nServerPort == ssConfig.DefaultHTTPSSLPort) { 
                        
                        nServerPort = ssConfig.DefaultHTTPPort;
                    } 
	    			
	    			if (ssSecConnection != null) {	
	    			
	    				ssSecConnection.Close();
	    				ssSecConnection = null;
	    			}

                    if (strHostName.StartsWith("https://")) {

                        strHostName = strHostName.Replace("https://", "http://");
                    }
	    		}
	    	}
	    }
	    
	    /// <summary>
	    /// 	Sets Server Name for Stream SSL (Defaults to Hostname)
	    /// </summary>
	    public string StreamSSLServerName {
	    
	    	set {
	    	
	    		strStreamSSLServerName = value;
				
	    		ssSecConnection.AuthenticateAsClient(value);
	    	}
	    }
		
		/// <summary>
		/// 	Indicator That Transmissions are to be Asynchronous
		/// </summary>
		public bool Async {
		
			get {
			
				return boolAsync;
			}
		}
		
		/// <summary>
		/// 	Gets Transaction ID
		/// </summary>
		public int ID {
				
			get {
			
				return nTransID;
			}
		}
	}

    /// <summary>
    ///     Data Operations Base Object
    /// </summary>    /// <summary>
    ///     Data Operations Base Object
    /// </summary>
    internal class DataOperation {

        private Queue<MySqlConnection> qmscDBAvailAccess = new Queue<MySqlConnection>();
									/* Available Database Access Connections for Database Commands */
		private List<MySqlConnection>ltmscDBUsedAccess = new List<MySqlConnection>();
									/* Used Database Access Connections for Database Commands */
        private int nMaxConns = ServerApp.Settings.DBMaxConnectPerObj; 
                                    /* Maximum Number of Database Connections */
 
        /// <param name="strServerName">Name of Database Server</param>
        /// <param name="strDatabaseName">Name of Database</param>
        /// <param name="strUserName">Username for Accessing Database</param>
        /// <param name="strPassword">Password for Accessing Database</param>
        public DataOperation(string strServerName,
                             string strDatabaseName,
                             string strUserName,
                             string strPassword) {

            Connection(strServerName, strDatabaseName, strUserName, strPassword);
        }

        public DataOperation() { }

        /// <summary>
        ///     Activate Database Access
        /// </summary>
        /// <param name="strServerName">Name of Database Server</param>
        /// <param name="strDatabaseName">Name of Database</param>
        /// <param name="strUserName">Username for Accessing Database</param>
        /// <param name="strPassword">Password for Accessing Database</param>
        /// <param name="boolUseSSL">Indicator to Use SSL Connection</param>
        /// <returns>Indicator That Connection was Setup</returns>
        protected bool Connection(string strServerName,
                                  string strDatabaseName,
                                  string strUserName,
                                  string strPassword,
                                  bool boolUseSSL = false) {

//            string strConnString = "",
            string strSSLValue = "None";
            bool boolSetup = false;
            int nCounter = 0;

            if (boolUseSSL) {

                strSSLValue = "Required";
            }

            string strConnString = "SERVER=" + strServerName + ";DATABASE=" + strDatabaseName +
                                   ";UID=" + strUserName + ";PASSWORD=" + strPassword + ";SSL Mode=" + strSSLValue;

            try {

                foreach (MySqlConnection qmscSelect in ltmscDBUsedAccess) {

                    Close(qmscSelect, true, true, false);
                }

                for (nCounter = 0; nCounter < nMaxConns; nCounter++) {

                    qmscDBAvailAccess.Enqueue(new MySqlConnection(strConnString));
                }

                boolSetup = qmscDBAvailAccess.Count > 0;
            }
            catch (Exception exError) {

                ServerApp.Log("Action: Setting up data operation's access to database, '" + strDatabaseName + "'.", 
                              exError);
            }

            return boolSetup;
        }

        /// <summary>
        ///     Opens Database Connection
        /// </summary>
        /// <returns>Database Connection If Connection Already Exists, Else NULL</returns>
        protected MySqlConnection Open() {

            MySqlConnection mscRetAccess = null;
                                    /* Copy of Database Access to Send If its Open */

            if (qmscDBAvailAccess.Count > 0) {

                mscRetAccess = qmscDBAvailAccess.Dequeue();

                if (mscRetAccess.State == ConnectionState.Broken) {

                    mscRetAccess.Close();
                }

                if (mscRetAccess.State == ConnectionState.Closed) {

                    mscRetAccess.Open();
                }

                if (mscRetAccess.State != ConnectionState.Broken) {

                    ltmscDBUsedAccess.Add(mscRetAccess);
                }
                else { 

                    /* If Connection is Still Broken, Throw Exception */
                    throw new Exception("Action: Opening connection to database. Error: Connection broken.");
                } 
            }

            return mscRetAccess;
        }

        /// <summary>
        ///     Opens Asynchronous Database Connection
        /// </summary>
        /// <returns>Database Connection If Connection Already Exists, Else NULL</returns>
        protected async Task<MySqlConnection> OpenAsync() {

            MySqlConnection mscRetAccess = null;
                                    /* Copy of Database Access to Send If its Open */

            if (qmscDBAvailAccess.Count > 0) {

                mscRetAccess = qmscDBAvailAccess.Dequeue();

                if (mscRetAccess.State == ConnectionState.Broken) {

                    await mscRetAccess.CloseAsync();
                }

                if (mscRetAccess.State == ConnectionState.Closed) {

                    await mscRetAccess.OpenAsync();
                }

                if (mscRetAccess.State != ConnectionState.Broken) {

                    ltmscDBUsedAccess.Add(mscRetAccess);
                }
                else { 

                    /* If Connection is Still Broken, Throw Exception */
                    throw new Exception("Action: Opening asynchronous connection to database. Error: Connection broken.");
                } 
            }

            return mscRetAccess;
        }

        /// <summary>
        ///     Close Database Connection
        /// </summary>
        /// <param name="boolIsSynch">Indicator Database Connection was Synchronous, Defaults to True</param>
        /// <param name="boolDispose">Indicator to Dispose of Database Connection Aftter Closing, Defaults to False</param>
        /// <param name="boolKeep">Indicator to Put Database Connection Back in Available Pool, Else Pool Count is Reduced, Defaults to True</param>
        protected void Close(bool boolIsSynch = true, bool boolDispose = false, bool boolKeep = true) { 
        
            foreach (MySqlConnection mscSelect in ltmscDBUsedAccess) {

                Close(mscSelect, boolIsSynch, boolDispose, boolKeep);
            }
        }

        /// <summary>
        ///     Close Database Connection
        /// </summary>
        /// <param name="mscDBAccess">Database Connection to Close</param>
        /// <param name="boolIsSynch">Indicator Database Connection was Synchronous, Defaults to True</param>
        /// <param name="boolDispose">Indicator to Dispose of Database Connection Aftter Closing, Defaults to False</param>
        /// <param name="boolKeep">Indicator to Put Database Connection Back in Available Pool, Else Pool Count is Reduced, Defaults to True</param>
        protected async void Close(MySqlConnection mscDBAccess, bool boolIsSynch = true, bool boolDispose = false, bool boolKeep = true) {

            if (mscDBAccess != null && ltmscDBUsedAccess.Contains(mscDBAccess)) { 

                if (boolIsSynch) { 

                    mscDBAccess.Close();
                }
                else {

                    await mscDBAccess.CloseAsync();
                }

                if (boolDispose || !boolKeep) { 
                
                    if (boolIsSynch) { 

                        mscDBAccess.Dispose();
                    }
                    else {

                        await mscDBAccess.DisposeAsync();
                    }
                }

                if (boolKeep) {

                    ltmscDBUsedAccess.Remove(mscDBAccess);
                    qmscDBAvailAccess.Enqueue(mscDBAccess);
                }
                else { 

                    mscDBAccess = null;
                }
            }
        }
    }
	
	/// <summary>
	/// 	Gets and Executed Server Commands
	/// </summary>
	internal class ServerCommands : DataOperation {

        private static ServerSettings ssConfig = ServerApp.Settings;
                                    /* Server Settings */
		private enum COMMANDINPUTTYPE { INVALID, DATABASE, FILE };
									/* Type of Way of Getting Server Commands */
		private COMMANDINPUTTYPE citInput = COMMANDINPUTTYPE.INVALID;
									/* Selected Way of Getting Commands */
		private bool boolDelExec = ssConfig.CmdMsgDelExec;
									/* Indicator to Delete Executed Commands */
        private string strDataTableName = "",
                       strDataIDFieldName = "",
                       strDataValueFieldName = "",
                       				/* Names of Table and ID and Value Fields for Database Commands */
					   strFilePath = "",
									/* Path to File for Commands */
    				   strCmdMsgPartEndChars = ssConfig.CmdMsgPartEndChars;
        /* Characters That Symbolize the End of Part of a Command Message */

        /// <summary>
        /// 	Setup to Use Database for Getting Commands
        /// </summary>
        /// <param name="strServerName">Name of Database Server</param>
        /// <param name="strDatabaseName">Name of Database</param>
        /// <param name="strUserName">Username for Accessing Database</param>
        /// <param name="strPassword">Password for Accessing Database</param>
        /// <param name="strSetDataTableName">Table on Database from Which to Get Commands From</param>
        /// <param name="strSetDataIDFieldName">Table ID Field on Database</param>
        /// <param name="strSetDataValueFieldName">Table Field on Database from Which to Get Commands From</param>
        /// <param name="boolUseSSL">Indicator to Use SSL Connection</param>
        /// <returns>True If Connection to Database was Setup, Else False</returns>
        public bool UseDatabaseCmds(string strServerName, 
                                    string strDatabaseName, 
                                    string strUserName, 
                                    string strPassword, 
                                    string strSetDataTableName,
                                    string strSetDataIDFieldName,
                                    string strSetDataValueFieldName,
                                    bool boolUseSSL = false) {

            bool boolConnSuccess = false;
        							/* Indicator That Database Connection was Successful */
        
        	try {
        		
	        	citInput = COMMANDINPUTTYPE.DATABASE;

                if (strSetDataTableName != "" && strSetDataIDFieldName != "" && strSetDataValueFieldName != "") {
	        		
		        	strDataTableName = strSetDataTableName;
                    strDataIDFieldName = strSetDataIDFieldName;
                    strDataValueFieldName = strSetDataValueFieldName;

                    boolConnSuccess = Connection(strServerName,
                                                 strDatabaseName,
                                                 strUserName,
                                                 strPassword,
                                                 boolUseSSL);
	        	}
        	}
        	catch (Exception exError) {
                 
                Close();
        								
        		ServerApp.Log("Action: Setting up access to database, '" + strDatabaseName + "', for getting commands.", exError);
        	}
        							
        	return boolConnSuccess;
        }
        
        /// <summary>
        /// 	Setup to Use File for Getting Commands
        /// </summary>
        /// <param name="strSetFileName">File Name</param>
        /// <param name="strSetFilePath">Path to File</param>
        /// <returns>True If File Exists, Else False</returns>
        public bool UseFileCmds(string strSetFileName, string strSetFilePath) {
        	
        	bool boolFileExists = false;
        							/* Indicator That File Exists */
        
        	try {
        		
	        	citInput = COMMANDINPUTTYPE.FILE;
	            		
				/* If File Has a Directory Path, Setup and Add to Filename */
				if (strSetFilePath != "") {
					
					if (!strSetFilePath.EndsWith("/")) {
					
						strSetFilePath += "/";
					}
					
					strFilePath = strSetFilePath + strSetFileName;
				}
                else {

                    strFilePath = strSetFileName;
                }
				
				boolFileExists = File.Exists(strFilePath);
        	}
        	catch (Exception exError) {

                Close();
        								
        		ServerApp.Log("Action: Setting up access to file, '" + strFilePath + "', for getting commands.",  
                              exError);
        	}
        							
        	return boolFileExists;
        }       
        
        /// <summary>
        /// 	Gets Server Commands
        /// </summary>
        /// <returns>List of Command String</returns>
        public List<string> GetCommands() {

            int nDBConTimeoutInMillis = ssConfig.DBConnectTimeout;
                                    /* Database Timeout In Milliseconds */
		    MySqlConnection mscDBAccess = null;
									/* Database Access for Database Commands */
			FileStream fsFileAccess = null;
								 	/* Access to File for Commands */
			MySqlDataReader msdrRecReader = null;
									/* Database Record Reader */
            MySqlCommand mscRunner = null;
                                    /* Database Runner */
 			StreamReader srReadFile = null;
  									/* Reads Commands from File */
			List<string> ltstMsgs = new List<string>(),
									/* List of Commands to be Executed */
                         ltstIDs = new List<string>();
                                    /* List of IDs to Possibly be Deleted */
            string strMsg = "",     /* Selected Line from File */
                   strCmdMsgStartChars = ssConfig.CmdMsgStartChars,
                   strCmdMsgEndChars = ssConfig.CmdMsgEndChars;
                                    /* Command Message Start and End Characters */
            string[] astrCmdEndChars = new string[] { strCmdMsgEndChars },
                                    /* Array of Command Message to End Indicator Characters */
                     astrCommandLines; 
                                    /* Lines of Broken up Command */
            Stream stmResultBytes;  /* Database Field Results as Stream */
            Byte[] abyteResult;     /* Database Field Results as Bytes */
            StringSplitOptions ssoRemove = StringSplitOptions.RemoveEmptyEntries;
                                    /* Option for Splitting Strings to Remove Blank Spaces */
 /*           COMMANDINPUTTYPE citTemp;		
                                    /* Temporary Holder for Loaded Commands Being Validated */			
			
			try {
								 		
	        	switch (citInput) {
	        	
	        		case COMMANDINPUTTYPE.DATABASE: {
	        			
						/* If Access to Database Setup, Get Commands, If Deleting Executed Records, Delete Gathered Commands from Database */
						if ((mscDBAccess = Open()) != null) {     				
	        				
                            mscRunner = new MySqlCommand("SELECT " + strDataIDFieldName + ", " + 
                                                                     strDataValueFieldName + " " +
                                                         "FROM " + strDataTableName, mscDBAccess);
                            mscRunner.CommandTimeout = nDBConTimeoutInMillis;
	        				msdrRecReader = mscRunner.ExecuteReader();
	        				
	        				while (msdrRecReader.Read()) {
                                    
                                stmResultBytes = msdrRecReader.GetStream(strDataValueFieldName);  
                                abyteResult = new byte[stmResultBytes.Length];     
                                stmResultBytes.Read(abyteResult, 0, (int)stmResultBytes.Length);
                                stmResultBytes.Dispose();
                                stmResultBytes.Close();
                                strMsg = Encoding.UTF8.GetString(abyteResult);

                                if (strMsg.IndexOf(strCmdMsgStartChars) >= 0 && strMsg.IndexOf(strCmdMsgEndChars) >= 0) {

                                    astrCommandLines = strMsg.Split(astrCmdEndChars, ssoRemove);

                                    foreach (string strCommandSelect in astrCommandLines) {

                                        ltstMsgs.Add(strCommandSelect + strCmdMsgEndChars);
                                    }
                                }
                                else {

                                    ServerApp.Log("Action: Getting commands from " + Enum.GetName(typeof(COMMANDINPUTTYPE), citInput) + ". Error: Invalid message: " + strMsg + ".");
                                }

                                if (boolDelExec) {

                                    ltstIDs.Add(msdrRecReader.GetInt32(strDataIDFieldName).ToString());
                                }
	        				}
                            
	        				msdrRecReader.Close();
	        				msdrRecReader = null;
	        				
	        				foreach (string strID in ltstIDs) {

                                mscRunner = new MySqlCommand("DELETE FROM " + strDataTableName + " " +
                                                             "WHERE " + strDataIDFieldName + " = " + strID, mscDBAccess);
                                mscRunner.CommandTimeout = nDBConTimeoutInMillis;
                                mscRunner.ExecuteNonQuery();
	        				}
	        				
	        				Close(mscDBAccess);
	        			}
                        else {

                            ServerApp.Log("Action: Getting commands from " + Enum.GetName(typeof(COMMANDINPUTTYPE), citInput) + 
                                          ". Error: Database connection was not open for message: " + strMsg + ".");
                        }

                        break;
	        		}
	        		case COMMANDINPUTTYPE.FILE: {
	        			
						/* If File Exists, Access it, Load a Command Per Line, If Deleting the Loaded Commands, Truncate the File Through Setting its File Length to 0 */													
	        			if (strFilePath != "") {
														
							fsFileAccess = File.Open(strFilePath, FileMode.Open);
							
							srReadFile = new StreamReader(fsFileAccess);
							
							while (srReadFile.Peek() > 0) {

                                strMsg = srReadFile.ReadLine();

                                if (strMsg.IndexOf(strCmdMsgStartChars) >= 0 && strMsg.IndexOf(strCmdMsgEndChars) >= 0) {

                                    astrCommandLines = strMsg.Split(astrCmdEndChars, ssoRemove);

                                    foreach (string strCommandSelect in astrCommandLines) {

                                        ltstMsgs.Add(strCommandSelect + strCmdMsgEndChars);
                                    }
                                }
                                else {

                                    ServerApp.Log("Action: Getting commands from " + Enum.GetName(typeof(COMMANDINPUTTYPE), citInput) + ". Error: Invalid message: " + strMsg + ".");
                                }
							}
	        				
							if (boolDelExec) {
							
								fsFileAccess.SetLength(0);
							}
							
	        				fsFileAccess.Close();
	        				srReadFile.Close();
						}
	        		}

                    break;
	        	}			
			}
        	catch (Exception exError) {
										
        		if (mscDBAccess != null) {

                    if (boolDelExec) {

                        if (msdrRecReader != null) {

                            ltstIDs.Add(msdrRecReader.GetInt32(strDataIDFieldName).ToString());
                        }
	        				
	        		    foreach (string strID in ltstIDs) {
	        					
                            mscRunner = new MySqlCommand("DELETE FROM " + strDataTableName + " " +
                                                         "WHERE " + strDataIDFieldName + " = " + strID, mscDBAccess);
                            mscRunner.CommandTimeout = nDBConTimeoutInMillis;
                            mscRunner.ExecuteNonQuery();
	        		    }
	        		}

        			Close(mscDBAccess);						
        		}
										
				if (fsFileAccess != null) {
					
					if (boolDelExec) {
							
						fsFileAccess.SetLength(0);
					}

					fsFileAccess.Close();											
				}
				
				if (msdrRecReader != null) {
				
					msdrRecReader.Close();											
				}
										
 				if (srReadFile != null) {
				
					srReadFile.Close();											
				}
        		
        		ServerApp.Log("Action: Getting commands from " + Enum.GetName(typeof(COMMANDINPUTTYPE), citInput) + ".", exError);
        	}
									
			return ltstMsgs;
        }
	}

    /// <summary>	
    /// 	Executes Statements on the Database
    /// </summary>
    internal class DatabaseExecutor : DataOperation {
        
        public enum MYSQLNUMTYPES { INTEGER, INT, SMALLINT, TINYINT, MEDIUMINT, BIGINT, BIT, DECIMAL, FLOAT, DOUBLE, NUMERIC };
                       /* MySQL Database Numeric Types Acceptable For Conversion */
        private string strDatabaseOutName = "",
                       /* Database Name */
                       strStatementParamChars = ServerApp.Settings.DBParamChar;
                       /* Leading Characters for Database Statement Parameter Variables */
        private bool boolConnSuccess = false,
                       /* Indictor of Database Connection */
                     boolRunDataMaps = false,
                       /* Indicator to Run Data Maps */
                     boolInitialConnect = true;
                        /* Indicator to Setup Initial Connection */
        private Dictionary<string, string> dictstrRegDataQuery = new Dictionary<string, string>(),
                                           dictstrRegDataStatement = new Dictionary<string, string>();
                       /* Holder for Database Registered Queries, Statements, and Events for Execution */
        private List<string> ltstrRegDataEvent = new List<string>();
                       /* Holder for Database Registered Events for Execution */  
        private enum MYSQLEVENTTYPES { ONCE, SECOND, MINUTE, HOUR, DAY, WEEK, MONTH, QUARTER, YEAR };
                        /* MySQL Database Event Interval Types */
        private Dictionary<int, Dictionary<int, Dictionary<string, object>>> dictnTransParamList = null;
                       /* Holder for Transaction's Parameter Lists */
        private Dictionary<int, Dictionary<int, Dictionary<int, MySqlCommand>>> dictmscQueries = new Dictionary<int, Dictionary<int, Dictionary<int, MySqlCommand>>>(),
                       /* List of Queries to Run */
                                                                                dictmscStatements = new Dictionary<int, Dictionary<int, Dictionary<int, MySqlCommand>>>();
                       /* List of Statements to Run */
        private Thread thdExecutor = null,
                        /* Execution of Multiple Data Processes Thread */
                       thdDMProcess = null;
                        /* Data Map Procesor */
       // private enum MYSQLINVTYPES { BINARY, VARBINARY, BLOB, MEDIUMBLOB, LONGBLOB, TINYTEXT, TEXT, MEDIUMTEXT, LONGTEXT, ENUM, BOOL, SET };
        private enum MYSQLINVTYPES { BINARY, VARBINARY };
                        /* MySQL Database Invalid Types For Conversion */
        private Dictionary<int, Dictionary<int, Dictionary<int, string>>> dictstrResults = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>();
                        /* List of Query Results */
        private string strMsgTableName = "",
                       strMsgFieldName = "",
                       strMsgFieldDataType  = "";
                        /* Message Table Name and Field and its Data Type for Processing */
        private class DataMap {

            private string strQueryDataDesign = "",
                                    /* Designation of Query Data */
                           strObjectDesign = "",
                                    /* Design of Client Object for Sending Data to */
                           strVarFuncName = "";
                                    /* Name of Variable or Function to Send Data */
            private bool boolIsVariable = true;
                                    /* Indicator That Data is Being Send to Variable or Function  */
            private int nIntervalInMillis = 500;
                                    /* Interval to Run Data Query in Milliseconds */
            private MySqlCommand mscQuerier = null;
                                    /* Used for Setting up Query */
            private DateTime dtNextRun = DateTime.Now;
                                    /* Time for the Next Run */

            public DataMap(string strSetQueryDataDesign, 
                           string strSetObjectDesign, 
                           string strSetVarFuncName, 
                           bool boolSetIsVariable = true, 
                           int nSetIntervalInMillis = 500,
                           MySqlCommand mscSetQuerier = null) {

                strQueryDataDesign = strSetQueryDataDesign;
                strObjectDesign = strSetObjectDesign;
                strVarFuncName = strSetVarFuncName;
                boolIsVariable = boolSetIsVariable;
                nIntervalInMillis = nSetIntervalInMillis;
                mscQuerier = mscSetQuerier;

                mscQuerier.CommandTimeout = ServerApp.Settings.DBConnectTimeout;
            }
            
            public string QueryDataDesign {

                get {

                    return strQueryDataDesign;
                }
            }

            public string ClientObjectDesign {

                get {

                    return strObjectDesign;
                }
            }
            
            public string VarFuncName {

                get {

                    return strVarFuncName;
                }
            }

            public bool IsVariable {

                get {

                    return boolIsVariable;
                }
            }

            public int IntervalInMillis {

                get {

                    return nIntervalInMillis;
                }
            }

            public MySqlCommand Querier {

                get {

                    return mscQuerier;
                }
            }

            /// <summary>
            ///     Returns True If Interval Has Expired for Executing Query
            /// </summary>
            public bool TimeExpired {

                get {

                    return DateTime.Now >= dtNextRun;
                }
            }

            /// <summary>
            ///     Advances Time for Next Run
            /// </summary>
            public void AdvanceTime() {

                dtNextRun = DateTime.Now.AddMilliseconds(nIntervalInMillis);
            }
        }
        List<DataMap> ltdmSends = new List<DataMap>();
                                    /* List of Data Maps for Sending Data */
        Queue<string> qstrDataMapResults = new Queue<string>();
                                    /* Results from Data Map Processing */

        /// <param name="strServerName">Name of Database Server</param>
        /// <param name="strDatabaseName">Name of Database</param>
        /// <param name="strUserName">Username for Accessing Database</param>
        /// <param name="strPassword">Password for Accessing Database</param>
        /// <param name="strParamChars">Leading Characters for Database Statement Parameter Variables</param>
        /// <param name="strSetMsgTableName">Message Table Name</param>
        /// <param name="strSetMsgFieldName">Message Table's Field Name</param>
        /// <param name="strSetMsgFieldDataType">Message Table's Field's Data Type</param>
        public DatabaseExecutor(string strServerName,
                                string strDatabaseName,
                                string strUserName,
                                string strPassword,
                                string strParamChars = "", 
                                string strSetMsgTableName = "",
                                string strSetMsgFieldName = "",
                                string strSetMsgFieldDataType  = "",
                                bool boolDoSetup = false) : base(strServerName, 
                                                                 strDatabaseName, 
                                                                 strUserName, 
                                                                 strPassword) {

            boolConnSuccess = true;
                
            strDatabaseOutName = strDatabaseName;

            if (strParamChars != null && strParamChars != "") {

                strStatementParamChars = strParamChars;
            }

            strMsgTableName = strSetMsgTableName;
            strMsgFieldName = strSetMsgFieldName;
            strMsgFieldDataType = strSetMsgFieldDataType;

            /* Setup Database for Sending Server Commands */
            if (boolDoSetup) {

                Setup();
            }
        }

        /// <summary>
        ///     Do Setup for Database Communication
        /// </summary>
        public void Setup() {

//            ServerSettings ssConfig = ServerApp.Settings;

            try {

                if (boolInitialConnect &&
                    strMsgTableName != "" && 
                    strMsgFieldName != "" && 
                    strMsgFieldDataType != "") {

                    ServerSettings ssConfig = ServerApp.Settings;

                    strMsgFieldDataType = strMsgFieldDataType.ToUpper();

                    if (ssConfig.ConfirmMessageDataType(strMsgFieldDataType)) {

                        Run(ssConfig.GenerateDefaultDataCleanup());
                        Run(ssConfig.GenerateDefaultDataProcs(strMsgTableName, strMsgFieldName, strMsgFieldDataType));
                        boolInitialConnect = false;
                    }
                    else { 

                        ServerApp.Log("Action: Setting up communications for database, '" + strDatabaseOutName + 
                                      "', for running statements, setting up server commands failed. Error: Invalid message data type sent '" + 
                                      strMsgFieldDataType + "'.");
                    }
                }
            }
            catch (Exception exError)  {

                ServerApp.Log("Action: Setting up communications for database, '" + strDatabaseOutName + 
                              "', for running statements, setting up server commands failed. ", exError);
            }
        }

        /// <summary>
        ///     Opens Database Connection, If Processing Setup, Will Added Setup to Connection to Get Server Commands
        /// </summary>
        /// <param name="boolDoSetup">Identicator for Doing Setup for Database Communication</param>
        /// <returns>Database Connection If Exists, Else NULL</returns>
        protected MySqlConnection Open(bool boolDoSetup = false) {

            if (boolDoSetup) {

                Setup();
            }

            return base.Open();
        }

        /// <summary>
        ///     Opens Asynchronous Database Connection, If Processing Setup, Will Added Setup to Connection to Get Server Commands
        /// </summary>
        /// <param name="boolDoSetup">Identicator for Doing Setup for Database Communication</param>
        /// <returns>Database Connection If Exists, Else NULL</returns>
        protected async Task<MySqlConnection> OpenAsync(bool boolDoSetup = false) {

            if (boolDoSetup) {

                Setup();
            }

            return await base.OpenAsync();
        }

        /// <summary>
        ///     Add Data Query for Data Execution Processing
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Query</param>
        /// <param name="strQuery">Database Query</param>
        public void RegisterQuery(string strDataDesign, string strQuery) {

            RemoveProcess(strDataDesign);
            dictstrRegDataQuery.Add(strDataDesign, strQuery);
        }

        /// <summary>
        ///     Add Data Statement for Data Execution Processing
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Statement</param>
        /// <param name="strStatement">Database Statement</param>
        public void RegisterStatement(string strDataDesign, string strStatement) {
            
            RemoveProcess(strDataDesign);
            dictstrRegDataStatement.Add(strDataDesign, strStatement);
        }

        /// <summary>
        ///     Add Data Event for Data Execution Processing
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Statement</param>
        /// <param name="strStatement">Database Statement</param>
        /// <param name="nInterval">Amount of Interval Time</param>
        /// <param name="strIntervalType">Type of Interval at Which to Run Event</param>
        /// <param name="nDelayInMilliSecs">Event Delay In Milliseconds</param>
        public void RegisterEvent(string strDataDesign, 
                                  string strStatement, 
                                  int nInterval, 
                                  string strIntervalType,
                                  int nDelayInMilliSecs) {
            
            StringBuilder sbMakeStatement = new StringBuilder("CREATE EVENT " + strDataDesign + " " +
                                                              "ON SCHEDULE ");
                                    /* Creates Database Statement for Event */
            List<string> ltstrDataStatements = new List<string>();
                                    /* Holder for Statements to Execute */
            MYSQLEVENTTYPES metIntervalType;
                                    /* Type of Interval */
            DateTime dtDelay = DateTime.Now.AddMilliseconds(nDelayInMilliSecs);       
                                    /* Date and Time of the Delay */

            if (strDataDesign.Length > 64) {

                strDataDesign = strDataDesign.Substring(0, 64);
            }

            if (ltstrRegDataEvent.Count <= 0) {

                Run("SET GLOBAL event_scheduler = ON " + ServerApp.Settings.DBProcDelimiter);
            }

            RemoveProcess(strDataDesign, true);

            if (Enum.TryParse<MYSQLEVENTTYPES>(strIntervalType, out metIntervalType)) {

                if (metIntervalType != MYSQLEVENTTYPES.ONCE) {

                    sbMakeStatement.Append("EVERY " + nInterval.ToString() + " " + strIntervalType + " ");

                    if (nDelayInMilliSecs > 0) {

                        sbMakeStatement.Append("STARTS '" + dtDelay.ToString("yyyy-MM-dd HH:mm:ss") + "' ");
                    }

                    sbMakeStatement.Append("ON COMPLETION PRESERVE ");
                }
                else {

                    sbMakeStatement.Append("AT '" + dtDelay.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                           "ON COMPLETION NOT PRESERVE ");
                }

                sbMakeStatement.Append("DO " + strStatement + " " + ServerApp.Settings.DBProcDelimiter);

                ltstrRegDataEvent.Add(strDataDesign);

                Run(sbMakeStatement.ToString());
            }
            else {

                ServerApp.Log("Action: Registering database event, '" + strDataDesign + "', and failed due to invalid event interval type which include, " + string.Join(", ", Enum.GetNames(typeof(MYSQLEVENTTYPES))) + ".");
            }
        }

        /// <summary>
        ///     Added Data Map
        /// </summary>
        /// <param name="strQueryDataDesign">Designation of Query Data</param>
        /// <param name="strObjectDesign"> Design of Client Object for Sending Data to</param>
        /// <param name="strVarFuncName">Name of Variable or Function to Send Data</param>
        /// <param name="boolIsVariable">Indicator That Data is Being Send to Variable or Function</param>
        /// <param name="nIntervalInMillis">Interval to Run Data Query in Milliseconds</param>
        public void AddDataMap(string strQueryDataDesign, 
                               string strObjectDesign, 
                               string strVarFuncName, 
                               bool boolIsVariable = true, 
                               int nIntervalInMillis = 500) {

            lock(ltdmSends) { 

                ltdmSends.Add(new DataMap(strQueryDataDesign,
                                          strObjectDesign,
                                          strVarFuncName,
                                          boolIsVariable,
                                          nIntervalInMillis,
                                          new MySqlCommand(dictstrRegDataQuery[strQueryDataDesign])));
            }

            if (thdDMProcess == null || !thdDMProcess.IsAlive) {

                thdDMProcess = new Thread(ProcessDataMaps);
                thdDMProcess.Start();
            }
        }

        /// <summary>
        ///     Remove Data Statement from Data Execution Processing
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Statement</param>
        /// <param name="boolForceRemove">Indicator to Run Removal Whether Data Event Process is or isn't Registered</param>
        public void RemoveProcess(string strDataDesign, bool boolForceEventRemoval = false) {

            if (!ltstrRegDataEvent.Contains(strDataDesign)) {

                dictstrRegDataStatement.Remove(strDataDesign);
                dictstrRegDataQuery.Remove(strDataDesign);
            }
            else { 

                ltstrRegDataEvent.Remove(strDataDesign);
                boolForceEventRemoval = true;
            }

            if (boolForceEventRemoval) {

                Run("DROP EVENT IF EXISTS " + strDataDesign + ";");
            }
        }

        /// <summary>
        ///     Adds or Replaces an Existing Database Query or Statement Parameter for a Transaction by ID
        /// </summary>
        /// <param name="nClientID">Client ID</param>
        /// <param name="nTransID">Transaction ID</param>
        /// <param name="strParamName">Name of Parameter</param>
        /// <param name="objParamValue">Value of Parameter</param>
        public void SetParameter(int nClientID, int nTransID, string strParamName, object objParamValue) {

            Dictionary<string, object> dictstrParamList;
                                    /* Selected Parameter Information */

            if (dictnTransParamList != null) {

                if (!dictnTransParamList.ContainsKey(nClientID)) {

                    dictnTransParamList.Add(nClientID, new Dictionary<int, Dictionary<string, object>>());
                }

                if (dictnTransParamList[nClientID].ContainsKey(nTransID)) {

                    dictstrParamList = dictnTransParamList[nClientID][nTransID];
                    dictstrParamList.Remove(strParamName);
                }
                else {

                    dictstrParamList = new Dictionary<string, object>();
                    dictnTransParamList[nClientID].Add(nTransID, dictstrParamList);
                }
            }
            else {

                dictnTransParamList = new Dictionary<int, Dictionary<int, Dictionary<string, object>>>();
                dictnTransParamList.Add(nClientID, new Dictionary<int, Dictionary<string, object>>());
                dictstrParamList = new Dictionary<string, object>();
                dictnTransParamList[nClientID].Add(nTransID, dictstrParamList);
            }

            dictstrParamList.Add(strParamName, objParamValue);
        }

        /// <summary>
        ///     Removes Existing Database Query or Statement Parameter for a Transaction by ID
        /// </summary>
        /// <param name="nClientID">Client ID</param>
        /// <param name="nTransID">Transaction ID</param>
        /// <param name="strParamName">Name of Parameter</param>
        public void RemoveParameter(int nClientID, int nTransID, string strParamName) {

            if (dictnTransParamList != null &&
                dictnTransParamList.ContainsKey(nClientID) &&
                dictnTransParamList[nClientID].ContainsKey(nTransID) && 
                dictnTransParamList[nClientID][nTransID].ContainsKey(strParamName)) {

                dictnTransParamList[nClientID][nTransID].Remove(strParamName);
            }
        }

        /// <summary>
        /// 	Executes Statement on Database
        /// </summary>
        /// <param name="nClientID">Client ID</param>
        /// <param name="nTransID">Transaction ID</param>
        /// <param name="nRespID">Response ID</param>
        /// <param name="strDataDesign">Designation of Query or Statement to Execute on Database</param>
        /// <param name="boolAsync">Indicator to Run Query as Asynchronous Operation</param>
        /// <param name="boolRetResults">Indicator to Return Results</param>
        /// <returns>True If Execution was Started, Else False</returns>
        public bool Execute(int nClientID,
                            int nTransID,
                            int nRespID,
                            string strDataDesign,
                            bool boolAsync = true,
                            bool boolRetResults = true) {

            MySqlCommand mscdQuerier;
                                    /* Database Querier */
            string strStatement = "";
                                    /* Database Query or Statement */
            Dictionary<string, object> dictstrParamList = null;
                                    /* Parameter List */
            bool boolQuery = false, /* Indicator That Database Statement is a Query */
                 boolExecuted = false;
                                    /* Indicator That Database Execution Occurred */
            Dictionary<int, MySqlCommand> dictmscInfo = null;
                                    /* Holder for Query or Statement Information */

            try {

                if (boolConnSuccess) {

                    if (dictstrRegDataQuery.ContainsKey(strDataDesign)) {

                        strStatement = dictstrRegDataQuery[strDataDesign];
                        boolQuery = true;
                    }

                    if (dictstrRegDataStatement.ContainsKey(strDataDesign)) {

                        strStatement = dictstrRegDataStatement[strDataDesign];
                    }

                    if (strStatement != "") {

                        if (dictnTransParamList != null &&
                            dictnTransParamList.ContainsKey(nClientID) &&
                            dictnTransParamList[nClientID].ContainsKey(nTransID)) {

                            dictstrParamList = dictnTransParamList[nClientID][nTransID];

                            if (!dictstrParamList.ContainsKey(ServerApp.Settings.ResponseIDVar)) {

                                dictstrParamList.Add(ServerApp.Settings.ResponseIDVar, nRespID.ToString());
                            }
                            else {

                                dictstrParamList[ServerApp.Settings.ResponseIDVar] = nRespID.ToString();
                            }
                        }

                        mscdQuerier = Build(strStatement, dictstrParamList);
            
                        if (boolAsync) {

                            if (boolQuery) {

                                lock (dictmscQueries) {

                                    if (!dictmscQueries.ContainsKey(nClientID)) {

                                        dictmscQueries.Add(nClientID, new Dictionary<int, Dictionary<int, MySqlCommand>>());
                                    }

                                    if (dictmscQueries[nClientID].ContainsKey(nTransID)) {

                                        if (!dictmscQueries[nClientID][nTransID].ContainsKey(nRespID)) {

                                            dictmscQueries[nClientID][nTransID].Add(nRespID, mscdQuerier);
                                            boolExecuted = true;
                                        }
                                        else {

                                            // Log Error
                                            ServerApp.Log("Action: Executing on database, '" + strDatabaseOutName + "'. Client ID, '" + nClientID + "', transaction, '" + nTransID + 
                                                            "', with response ID, '" + nRespID + "', already exists for the data queries.");
                                        }
                                    }
                                    else {

                                        dictmscInfo = new Dictionary<int, MySqlCommand>();
                                        dictmscInfo.Add(nRespID, mscdQuerier);
                                        dictmscQueries[nClientID].Add(nTransID, dictmscInfo);
                                        boolExecuted = true;
                                    }
                                }
                            }
                            else {

                                lock (dictmscStatements) {

                                    if (!dictmscStatements.ContainsKey(nClientID)) {

                                        dictmscStatements.Add(nClientID, new Dictionary<int, Dictionary<int, MySqlCommand>>());
                                    }

                                    if (dictmscStatements[nClientID].ContainsKey(nTransID)) {
                                            
                                        if (!dictmscStatements[nClientID][nTransID].ContainsKey(nRespID)) {

                                            dictmscStatements[nClientID][nTransID].Add(nRespID, mscdQuerier);
                                            boolExecuted = true;
                                        }
                                        else {

                                            // Log Error
                                            ServerApp.Log("Action: Executing on database, '" + strDatabaseOutName + "'. Client ID, '" + nClientID + "', transaction, '" + nTransID +
                                                            "', with response ID, '" + nRespID + "', already exists for the data queries.");
                                        }
                                    }
                                    else {

                                        dictmscInfo = new Dictionary<int, MySqlCommand>();
                                        dictmscInfo.Add(nRespID, mscdQuerier);
                                        dictmscStatements[nClientID].Add(nTransID, dictmscInfo);
                                        boolExecuted = true;
                                    }
                                }
                            }

                            if (thdExecutor == null || !thdExecutor.IsAlive) {

                                thdExecutor = new Thread(Run);
                                thdExecutor.Start();
                            }
                        }
                        else {
                            
                            Run(nClientID, nTransID, nRespID, mscdQuerier, boolQuery, false, boolRetResults);
                            boolExecuted = true;
                        }
                    }
                    else {

                        ServerApp.Log("Action: Executing on database, '" + strDatabaseOutName + "'. Error: No query or statement with designation, '" + strDataDesign + "' exists.");                
                    }
                }
                else {

                    ServerApp.Log("Action: Executing on database, '" + strDatabaseOutName + "'. Error: No connection to database.");                
                }
           	}
        	catch (Exception exError) {

        		Close();
        								
        		ServerApp.Log("Action: Executing on database, '" + strDatabaseOutName + "'.", exError);
        	}

            return boolExecuted;
        }

        /// <summary>Builds SQL Statment or Query</summary>
        /// <param name="strStatement">Statement to Execute on Database</param>
        /// <param name="dictParamList">List of Parameter Values by Designation</param>
        /// <returns> SQL Command Containing Built Statement or Query</returns>
        private MySqlCommand Build(string strStatement,
                                   Dictionary<string, object> dictParamList = null) {

            MySqlCommand mscdQuerier;
                                    /* Database Querier */
            MySqlParameterCollection mspcParams;
                                    /* Holder for Query Parameters */
                       	
            mscdQuerier = new MySqlCommand(strStatement);
            mscdQuerier.CommandTimeout = ServerApp.Settings.DBConnectTimeout;

            if (dictParamList != null) {

                mspcParams = mscdQuerier.Parameters;

                foreach (KeyValuePair<string, object> kvpSelect in dictParamList) {

                    mspcParams.AddWithValue(strStatementParamChars + kvpSelect.Key.ToLower(), kvpSelect.Value);
                }
            }

            return mscdQuerier;
        }

        /// <summary>
        ///     Runs Database Statement and Gets Results
        /// </summary>
        private void Run() {

            int nClientID = 0,      /* Selected Client ID */
                nTransID = 0;       /* Selected Transaction ID */

            lock (dictmscQueries) { 

                foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, MySqlCommand>>> kvpClient in dictmscQueries) {

                    nClientID = kvpClient.Key;

                    foreach (KeyValuePair<int, Dictionary<int, MySqlCommand>> kvpTransaction in kvpClient.Value) {

                        nTransID = kvpTransaction.Key;

                        foreach (KeyValuePair<int, MySqlCommand> kvpSelect in kvpTransaction.Value) {

                            Run(nClientID, nTransID, kvpSelect.Key, kvpSelect.Value);
                        }
                    }
                }

                dictmscQueries.Clear();
            }

            lock (dictmscStatements) {

                foreach (KeyValuePair<int, Dictionary<int, Dictionary<int, MySqlCommand>>> kvpClient in dictmscStatements) {

                    nClientID = kvpClient.Key;

                    foreach (KeyValuePair<int, Dictionary<int, MySqlCommand>> kvpTransaction in kvpClient.Value) {

                        nTransID = kvpTransaction.Key;

                        foreach (KeyValuePair<int, MySqlCommand> kvpSelect in kvpTransaction.Value) {

                            Run(nClientID, nTransID, kvpSelect.Key, kvpSelect.Value, false);
                        }
                    }
                }

                dictmscStatements.Clear();
            }
        }

        /// <summary>
        ///     Runs Database Statement and Gets Results
        /// </summary>
        /// <param name="nTransID">Transaction ID</param>
        /// <param name="nRespID">Response ID</param>
        /// <param name="mscdQuerier">Database Query Object</param>
        /// <param name="boolQuery">Indicator to Run as Query or Run as Statement</param>
        /// <param name="boolAsync">Indicator to Run Query as Asynchronous Operation</param>
        private async void Run(int nClientID, 
                               int nTransID, 
                               int nRespID, 
                               MySqlCommand mscdQuerier, 
                               bool boolQuery = true, 
                               bool boolAsync = true,
                               bool boolRetResults = true) {

            MySqlConnection mscDBAccess = null;
                                    /* Database Access for Database Commands */
            MySqlDataReader msdrRecReader = null;
                                    /* Reader for Database Results */
            int nFieldCount = 0;    /* Field Count */
            Dictionary<string, string> dictFieldList = new Dictionary<string, string>();
                                    /* Metadata on Fields */
            string strFieldType = "",
                                    /* Selected Field's Type */
                   strFieldValue = "";
                                    /* Selected Field's Converted Value */
            int nRowsAffected = 0;  /* Number of Rows Affected by Database Statement */
            StringBuilder sbStorage = new StringBuilder();
                                    /* Storage for Query Results as JSON */
            bool boolNotFirstRow = false,
                                    /* Indicator That First Row Has Been Passed in Processing */
                 boolInvalidType = false,
                                    /* Indicator That Field's Type as Invalid for Processing */
                 boolContinue = true;
                                    /* Indicator to Continue Processing */
            Dictionary<int, string> dictstrResponse;
                                    /* New Response Information */
            Stream stmResultBytes;  /* Database Process Results as Stream */
            byte[] abyteResult;     /* Database Process Results as Bytes */
//            MySqlCommand mscdDataProcessReturn; 
                                    /* Query for Data Process Returned Information */
//            Dictionary<string, object> dictstrParamList;
                                    /* Parameter Information for Query for Data Process Returned Information */
            List<long>ltlDeleteIDs = new List<long>();
                                    /* List of Data Process IDs for Deletion */
            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                    /* Manage Stopping Thread */
            int nCounter = 0;       /* Counter for Loop */
                       	
            try {
                
                while (boolConnSuccess && boolContinue) {

                    if ((mscDBAccess = boolAsync ? await OpenAsync() : Open()) != null) {

                        mscdQuerier.Connection = mscDBAccess;

                        if (boolQuery) { 

                            msdrRecReader = boolAsync ? (MySqlDataReader)await mscdQuerier.ExecuteReaderAsync() : mscdQuerier.ExecuteReader();
                            nFieldCount = msdrRecReader.FieldCount;

                            for (nCounter = 0; nCounter < nFieldCount; nCounter++) {

                                dictFieldList.Add(msdrRecReader.GetName(nCounter), msdrRecReader.GetDataTypeName(nCounter));
                            }

                            sbStorage.Append("[");

                            while (boolAsync ? await msdrRecReader.ReadAsync() : msdrRecReader.Read()) {

                                if (boolNotFirstRow) {

                                    sbStorage.Append(",");
                                }

                                sbStorage.Append("{");

                                nCounter = 0;

                                foreach (KeyValuePair<string, string> kvpSelect in dictFieldList) {

                                    strFieldType = kvpSelect.Value.ToUpper();

                                    if (!Enum.IsDefined(typeof(MYSQLINVTYPES), strFieldType)) {
                                
                                        sbStorage.Append("\"" + strFieldType + "\":");

                                        strFieldValue = msdrRecReader.GetValue(nCounter).ToString();

                                        if (!(Enum.IsDefined(typeof(MYSQLNUMTYPES), strFieldType))) {

                                            strFieldValue = "\"" + strFieldValue + "\"";
                                        }

                                        sbStorage.Append(strFieldValue);

                                        if (nCounter < nFieldCount - 1) {

                                            sbStorage.Append(",");
                                        }
                                    }
                                    else {

                                        boolInvalidType = true;
                                    }

                                    nCounter++;
                                }

                                sbStorage.Append("}");

                                boolNotFirstRow = true;
                            }

                            sbStorage.Append("]");

                            if (boolAsync) { 

                                await msdrRecReader.CloseAsync();
                                await msdrRecReader.DisposeAsync();
                            
                            }
                            else { 

                                msdrRecReader.Close();
                                msdrRecReader.Dispose();
                            }
                        }
                        else {

                            nRowsAffected = boolAsync ? await mscdQuerier.ExecuteNonQueryAsync() : mscdQuerier.ExecuteNonQuery();

                            sbStorage.Append("[{\"rows\": " + nRowsAffected.ToString() + "}]");

                            Dictionary<string, object> dictstrParamList;

                            if (dictnTransParamList != null && 
                                dictnTransParamList.ContainsKey(nClientID) &&
                                dictnTransParamList[nClientID].ContainsKey(nTransID)) {

                                dictstrParamList = dictnTransParamList[nClientID][nTransID];
                            }
                            else {

                                dictstrParamList = new Dictionary<string, object>();
                                dictstrParamList.Add(ServerApp.Settings.ClientIDVar, nClientID);
                                dictstrParamList.Add(ServerApp.Settings.TransactionIDVar, nTransID);
                                dictstrParamList.Add(ServerApp.Settings.ResponseIDVar, nRespID);
                            }

                            MySqlCommand mscdDataProcessReturn;

                            if (dictstrParamList.ContainsKey(ServerApp.Settings.ClientIDVar)) {

                                mscdDataProcessReturn = Build("SELECT revcomm_data_result_id, " +
                                                              "       result " +
                                                              "FROM revcomm_data_results " +
                                                              "WHERE (client_id = " + ServerApp.Settings.DBParamChar + ServerApp.Settings.ClientIDVar + " " +
                                                              "       AND trans_id = " + ServerApp.Settings.DBParamChar + ServerApp.Settings.TransactionIDVar + ") " +
                                                              "   OR client_id = 0 " +
                                                              "ORDER BY revcomm_data_result_id ASC", dictstrParamList);
                            }
                            else {

                                mscdDataProcessReturn = Build("SELECT revcomm_data_result_id, " +
                                                              "       result " +
                                                              "FROM revcomm_data_results " +
                                                              "WHERE trans_id = " + ServerApp.Settings.DBParamChar + ServerApp.Settings.TransactionIDVar + " " + 
                                                              "ORDER BY revcomm_data_result_id ASC", dictstrParamList);
                            }

                            mscdDataProcessReturn.Connection = mscDBAccess;
                            msdrRecReader = boolAsync ? (MySqlDataReader)await mscdDataProcessReturn.ExecuteReaderAsync() : mscdDataProcessReturn.ExecuteReader();

                            if (msdrRecReader.HasRows) { 

                                sbStorage.Clear();

                                while (boolAsync ? await msdrRecReader.ReadAsync() : msdrRecReader.Read()) {

                                    stmResultBytes = msdrRecReader.GetStream(1);
                                    abyteResult = new byte[stmResultBytes.Length];

                                    if (boolAsync) {
                                        
                                        await stmResultBytes.ReadAsync(abyteResult, 0, (int)stmResultBytes.Length);
                                        await stmResultBytes.DisposeAsync();
                                    }
                                    else {
                                        
                                        stmResultBytes.Read(abyteResult, 0, (int)stmResultBytes.Length);
                                        stmResultBytes.Dispose();
                                    }

                                    sbStorage.Append(Encoding.UTF8.GetString(abyteResult));
                                    stmResultBytes.Close();

                                    ltlDeleteIDs.Add(msdrRecReader.GetInt64(0));
                                }
                            }

                            if (boolAsync) { 

                                await msdrRecReader.CloseAsync();
                                await msdrRecReader.DisposeAsync();
                            
                            }
                            else { 

                                msdrRecReader.Close();
                                msdrRecReader.Dispose();
                            }
                        }

                        if (boolRetResults) { 

                            lock (dictstrResults) {

                                if (!dictstrResults.ContainsKey(nClientID)) {

                                    dictstrResults.Add(nClientID, new Dictionary<int, Dictionary<int, string>>());
                                }

                                if (dictstrResults[nClientID].ContainsKey(nTransID)) {

                                    if (!dictstrResults[nClientID][nTransID].ContainsKey(nRespID)) {

                                        dictstrResults[nClientID][nTransID].Add(nRespID, sbStorage.ToString());
                                    }
                                    else {

                                        ServerApp.Log("Action: Outputting results from database, '" + strDatabaseOutName + "' and client ID, " + nClientID + 
                                                      ", transaction ID, '" + nTransID + "' already has a response ID, '" + nRespID + "'.");
                                    }
                                }
                                else {

                                    dictstrResponse = new Dictionary<int, string>();
                                    dictstrResponse.Add(nRespID, sbStorage.ToString());
                                    dictstrResults[nClientID].Add(nTransID, dictstrResponse);
                                }
                            }
                        }

                        Close(mscDBAccess, !boolAsync);

                        if (ltlDeleteIDs.Count > 0) { 

                            Run("DELETE FROM revcomm_data_results " +
                                "WHERE revcomm_data_result_id IN (" + string.Join(",", ltlDeleteIDs.ToArray()) + ")");
                        }

                        if (boolInvalidType) {

                            ServerApp.Log("Action: Outputting results from database, '" + strDatabaseOutName + "' and skipped invalid columns type which include, " + 
                                          string.Join(", ", Enum.GetNames(typeof(MYSQLINVTYPES))) + ".");
                        }

                        boolContinue = false;
                    }

                    areThreadStopper.WaitOne(1);
                }
           	}
        	catch (Exception exError) {

                Close(mscDBAccess, !boolAsync);
        								
        		ServerApp.Log("Action: Outputting to database, '" + strDatabaseOutName + "'.", exError);
        	}					
        }

        /// <summary>
        ///     Runs Database Statement with No Results
        /// </summary>
        /// <param name="strDataStatement">Data Statements to Run</param>
        public void Run(string strDataStatement) {

            MySqlConnection mscDBAccess = null;
                                    /* Database Access for Database Commands */
            MySqlScript mssQuerier; /* Database Querier */
            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                    /* Manage Stopping Thread */
            bool boolContinue = true;
                                    /* Indicator to Continue Processing */
            
            try { 

                while (boolConnSuccess && boolContinue) { 
               
                    if ((mscDBAccess = Open()) != null) {

                        mssQuerier = new MySqlScript(mscDBAccess, strDataStatement);
                        mssQuerier.Delimiter = ServerApp.Settings.DBProcDelimiter;
                        mssQuerier.Execute();

                        Close(mscDBAccess);

                        boolContinue = false;
                    }

                    areThreadStopper.WaitOne(1);
                }
            }
        	catch (Exception exError) {

        		Close(mscDBAccess);
        								
        		ServerApp.Log("Action: Running statement on database, '" + strDatabaseOutName + "'. Statement: " + strDataStatement + ".", 
                              exError);
        	}
        }

        /// <summary>
        ///     Processes Data Maps
        /// </summary>
        public void ProcessDataMaps() {

            MySqlConnection mscDBAccess = null;
                                    /* Database Access for Database Commands */
            MySqlCommand mscdQuerier = null;
                                    /* Database Querier */
            MySqlDataReader msdrRecReader = null;
                                    /* Reader for Database Results */
            List<string> ltstrDataTypes = new List<string>();
                                    /* List of Data Types */
            int nFieldCount = 0,    /* Count of Fields from Selected Data Query */
                nCounter = 0;
            StringBuilder sbStorage = new StringBuilder();
                                    /* Holder for Creating Data Map Messages */
            string strFieldValue = "",
                                    /* Value of Selected Data Query Field */
                   strVarFuncName = "";
                                    /* Selected Data Map's Variable or Function to Send To */
            AutoResetEvent areThreadStopper = new AutoResetEvent(false);
                                    /* Manage Stopping Thread */
            bool boolNotFirstRow = false,
                                    /* Indicator to Select Data Row is Not First */
                 boolMultiRecs = false,
                                    /* Indicator to Data Has Multiple Records */
                 boolIsVariable = true,
                                    /* Indicator That the Selected Data Map is for a Client Variable */
                 boolContinue = true;
                                    /* Indicator to Continue Process Data Maps */

            while (boolConnSuccess && boolContinue) {

                if (boolRunDataMaps) { 

                    lock (ltdmSends) { 

                        foreach (DataMap dmSelect in ltdmSends) {

                            if (dmSelect.TimeExpired) {

                                if ((mscDBAccess = Open()) != null) {

                                    mscdQuerier = dmSelect.Querier;
                                    mscdQuerier.Connection = mscDBAccess;

                                    msdrRecReader = mscdQuerier.ExecuteReader();

                                    if (ltstrDataTypes.Count <= 0) { 

                                        nFieldCount = msdrRecReader.FieldCount;

                                        for (nCounter = 0; nCounter < nFieldCount; nCounter++) {

                                            ltstrDataTypes.Add(msdrRecReader.GetDataTypeName(nCounter));
                                        }
                                    }

                                    strVarFuncName = dmSelect.VarFuncName;
                                    boolIsVariable = dmSelect.IsVariable;

                                    while (msdrRecReader.Read()) {

                                        if (boolNotFirstRow) {

                                            sbStorage.Append(", ");

                                            if (boolIsVariable) {

                                                sbStorage.Append("[");

                                                if (!boolMultiRecs) {

                                                    sbStorage.Insert(0, "[");
                                                }
                                            }

                                            boolMultiRecs = true;
                                        }

                                        if (!boolIsVariable) {

                                            sbStorage.Append("{\"NAME\": \"" + strVarFuncName + "\", \"PARAMS\": [");
                                        }

                                        nCounter = 0;

                                        foreach (string strFieldType in ltstrDataTypes) {

                                            if (!Enum.IsDefined(typeof(MYSQLINVTYPES), strFieldType.ToUpper())) {

                                                strFieldValue = msdrRecReader.GetValue(nCounter).ToString();

                                                if (!(Enum.IsDefined(typeof(MYSQLNUMTYPES), strFieldType))) {

                                                    strFieldValue = "\"" + strFieldValue + "\"";
                                                }

                                                sbStorage.Append(strFieldValue);

                                                if (nCounter < nFieldCount - 1) {

                                                    sbStorage.Append(",");
                                                }
                                                else if (nCounter > 0) {
                                                
                                                    sbStorage.Append("]");
                                                }
                                            }
                                            else {

                                                ServerApp.Log("Action: Processing data map, desingation: '" + dmSelect.QueryDataDesign + "', and skipped invalid columns type which include: " + 
                                                              string.Join(", ", Enum.GetNames(typeof(MYSQLINVTYPES))) + ".");
                                            }

                                            nCounter++;
                                        }

                                        if (!boolIsVariable) {

                                            sbStorage.Append("]}");
                                        }

                                        boolNotFirstRow = true;
                                    }

                                    if (boolIsVariable) {

                                        if (boolMultiRecs) {

                                            sbStorage.Append("]");
                                        }

                                        if (boolMultiRecs || nFieldCount > 1) {

                                            sbStorage.Insert(0, "[");
                                        }

                                        sbStorage.Insert(0, "\"DESIGNATION\": \"" + dmSelect.ClientObjectDesign + 
                                                            "\", \"VARUPDATES\": [{\"NAME\": \"" + strVarFuncName + "\", \"VALUE\": ");
                                        sbStorage.Append("}]"); 
                                    }
                                    else {

                                        sbStorage.Insert(0, "\"DESIGNATION\": \"" + dmSelect.ClientObjectDesign +
                                                            "\", \"FUNCCALLS\": [");
                                        sbStorage.Append("]");
                                    }

                                    lock (qstrDataMapResults) {

                                        qstrDataMapResults.Enqueue("[{" + sbStorage.ToString() + "}]");
                                    }

                                    msdrRecReader.Close();
                                    msdrRecReader.Dispose();
                                    msdrRecReader = null;

                                    ltstrDataTypes.Clear();
                                    sbStorage.Clear();

                                    Close(mscDBAccess);

                                    dmSelect.AdvanceTime();
                                }

                                boolNotFirstRow = false;
                                boolMultiRecs = false;
                            }
                        }

                        boolContinue = ltdmSends.Count > 0;
                    }
                }

                areThreadStopper.WaitOne(1);
            }
        }

        /// <summary>
        ///     Returns List of Transaction IDs of Completed Data Processes
        /// </summary>
        /// <returns>Returns List of Transaction IDs</returns>
        public List<int> Results(int nClientID) {

            List<int> ltnResultIDs = new List<int>();
                                /* List of Transaction IDs of Completed Results */

            lock (dictstrResults) {

                if (dictstrResults.ContainsKey(nClientID)) { 

                    ltnResultIDs.AddRange(dictstrResults[nClientID].Keys);
                }
            }

            return ltnResultIDs;
        }

        /// <summary>
        ///     Removes and Returns Query Result
        /// </summary>
        /// <param name="nClientID">Client ID</param>
        /// <param name="nTransID">Transaction ID</param>
        /// <returns>Result as List of Responses for the Transaction Per Client or NULL If None Found</returns>
        public Dictionary<int, string> DequeueResult(int nClientID, int nTransID) {

            Dictionary<int, string> dictstrResp = null;  
                                    /* Selected Result */

            lock (dictstrResults) {

                if (dictstrResults.ContainsKey(nClientID) && dictstrResults[nClientID].ContainsKey(nTransID)) {

                    dictstrResp = dictstrResults[nClientID][nTransID];
                    dictstrResults[nClientID].Remove(nTransID);
                }
            }

            return dictstrResp;
        }

        /// <summary>
        ///     Removes and Returns Data Map Query Results
        /// </summary>
        /// <returns>Result as List of Data Map Query Responses</returns>
        public List<string> DequeueDataMapResults() {

            List<string> ltstrResults = null;
                                    /* Current Results */

            lock (qstrDataMapResults) {
                
                ltstrResults = new List<string>(qstrDataMapResults.ToArray());
                qstrDataMapResults.Clear();
            }

            return ltstrResults;
        }

        /// <summary>
        ///     Checks If Data Query or Statement is Registered
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Query or Statement</param>
        /// <returns>True If Designation is Already Registered, Else False</returns>
        public bool IsRegistered(string strDataDesign) {

            bool boolRegistered = false;    
                                    /* Indicator That Designation is Registered */

            if (!(boolRegistered = dictstrRegDataQuery.ContainsKey(strDataDesign))) {

                boolRegistered = dictstrRegDataStatement.ContainsKey(strDataDesign);
            }

            return boolRegistered;
        }

        /// <summary>
        ///     Checks If Transaction for Existing Process Already Exists
        /// </summary>
        /// <param name="nClientID">Client ID</param>
        /// <param name="nTransID">Transaction ID</param>
        /// <returns>True If Transaction ID Exists, Else False</returns>
        public bool TransactionExists(int nClientID, int nTransID) {

            bool boolInResults = false;
                                    /* Indicator That Transaction is in the Results */

            lock (dictstrResults) {

                boolInResults = dictstrResults.ContainsKey(nClientID) && dictstrResults[nClientID].ContainsKey(nTransID);
            }

            return (dictmscQueries.ContainsKey(nClientID) && dictmscQueries[nClientID].ContainsKey(nTransID)) || 
                   (dictmscStatements.ContainsKey(nClientID) && dictmscStatements[nClientID].ContainsKey(nTransID)) || 
                   boolInResults;
        }

        /// <summary>
        ///     Checks If Data Maps are Registered
        /// </summary>
        public bool HasDataMaps {

            get {

                bool boolDataMaps = false;

                lock (ltdmSends) {

                    boolDataMaps = ltdmSends.Count > 0;
                }

                return boolDataMaps;
            }
        }

        /// <summary>
        ///     Start Data Map Processing
        /// </summary>
        public void StartDataMaps() {

            boolRunDataMaps = true;
        }

        /// <summary>
        ///     Stop Data Map Processing
        /// </summary>
        public void StopDataMaps() {

            boolRunDataMaps = false;
        }
	}
	
    /// <summary>	
	/// 	Outputs Information to Database
	/// </summary>
	internal class DatabaseOut : DataOperation {
	
		private string strDataTableName = "",
                       strDataMsgFieldName = "",
                       				/* Table Name and Names of Fields for Storing Messages and Files */
					   strDatabaseOutName = "";
                                    /* Database Name */
        private bool boolConnSuccess = false;
                                    /* Indictor of Database Connection */

        /// <param name="strServerName">Name of Database Server</param>
        /// <param name="strDatabaseName">Name of Database</param>
        /// <param name="strUserName">Username for Accessing Database</param>
        /// <param name="strPassword">Password for Accessing Database</param>
        /// <param name="strSetDataTableName">Table on Database to Output to</param>
        /// <param name="strSetDataMsgFieldName">Table Field on Database Which to Output Messages to</param>
        /// <param name="strSetDataMsgFieldName">Table Field on Database Which to Output Files to</param>
        public DatabaseOut(string strServerName, 
                           string strDatabaseName, 
                           string strUserName, 
                           string strPassword, 
                           string strSetDataTableName, 
                           string strSetDataMsgFieldName) : base(strServerName,
                                                                 strDatabaseName,
                                                                 strUserName,
                                                                 strPassword) {
        
        	try {

                strDatabaseOutName = strDatabaseName;

	        	if (strSetDataTableName != "" && strSetDataMsgFieldName != "") {
	        		
		        	strDataTableName = strSetDataTableName;
	                strDataMsgFieldName = strSetDataMsgFieldName;
		        		
			       	boolConnSuccess = true;
	        	}

                if (!boolConnSuccess) {

                    ServerApp.Log("Action: Setting up access to database, '" + strDatabaseName + "', for outputting messages. Error: Connecting to database failed.");
                }
        	}
        	catch (Exception exError) {

        		Close();
        								
        		ServerApp.Log("Action: Setting up access to database, '" + strDatabaseName + "', for outputting messages.", exError);
        	}
        }
        
		/// <summary>
		/// 	Outputs Message to Database
		/// </summary>
		/// <param name="strMsg">Message to be Outputted to Database</param>
		/// <returns>True If Message was Outputted to Database, Else False</returns>
        public bool Output(string strMsg) {
        	
            MySqlConnection mscDBAccess = null;
                                    /* Database Access for Database Commands */
            MySqlCommand mscRunner = null;
                                    /* Database Runner */
            bool boolMsgSent = false;
									/* Indicator That Message was Sent */            
                       	
            try {

                if (boolConnSuccess) {

                    if ((mscDBAccess = Open()) != null) {

                        mscRunner = new MySqlCommand("INSERT INTO " + strDataTableName + " VALUES (" + strDataMsgFieldName + ") = '" + strMsg + "'", mscDBAccess);
                        mscRunner.CommandTimeout = ServerApp.Settings.DBConnectTimeout;
                        mscRunner.ExecuteNonQuery();

                        Close(mscDBAccess);
                    }
                    else {

                        ServerApp.Log("Action: Outputting to database, '" + strDatabaseOutName + "', message: '" + strMsg + "'. Error: Connection to database was not open.");                
                    }

                    boolMsgSent = true;
                }
                else {

                    ServerApp.Log("Action: Outputting to database, '" + strDatabaseOutName + "', message: '" + strMsg + "'. Error: No connection to database.");                
                }
           	}
        	catch (Exception exError) {

                Close(mscDBAccess);
        								
        		ServerApp.Log("Action: Outputting to database, '" + strDatabaseOutName + "', message: '" + strMsg + "'.", exError);
        	} 

			return boolMsgSent;							
        }
	}

	/// <summary>
	/// 	Outputs to File
	/// </summary>
	internal class FileOut {

        private static ServerSettings ssConfig = ServerApp.Settings;
                                    /* Server Settings */
        private string strFilePathName = "";
									/* Path to File */

        /// <param name="strSetFileName">Name of Output File</param>
        /// <param name="strSetFilePath">Path to Output File</param>
        public FileOut(string strSetFileName, string strSetFilePath) {
            	            		
			/* If File Has a Directory Path, Setup and Add to Filename */
			if (strSetFilePath != "") {
					
				if (!strSetFilePath.EndsWith("/")) {
					
					strSetFilePath += "/";
				}
					
				strFilePathName = strSetFilePath + strSetFileName;
			}
            else {

                strFilePathName = strSetFileName;
            }
        }
        
        /// <summary>
        /// 	Output to File
        /// </summary>
        /// <param name="strOutMsg">Message to Output to File</param>
        /// <returns>True If File Exists and Message was Output to File, Else False</returns>
        public bool Output(string strOutMsg) {
        	
			FileStream fsFileAccess = null;
								 	/* Access to File for Commands */
 			StreamWriter swWriteFile = null;
  									/* Writes to File */
        	bool boolMsgWrite = false;   
                                    /* Indicator That File Exists and Message was Output to File */
        
        	try {
	            		
				/* If File Exists, Access it, Output to it */
                if (strFilePathName != "") {	
												
	        	    if (!File.Exists(strFilePathName)) {

                        File.Create(strFilePathName);
                    }
														
					fsFileAccess = File.Open(strFilePathName, FileMode.Append);
							
					swWriteFile = new StreamWriter(fsFileAccess);

                    swWriteFile.WriteLine(strOutMsg);
							
	        		swWriteFile.Close();
	        		fsFileAccess.Close();

                    boolMsgWrite = true;
				}
        	}
        	catch (Exception exError) {
        		
        		if (swWriteFile != null) {

                    swWriteFile.Close();						
        		}

        		if (fsFileAccess != null) {

                    fsFileAccess.Close();						
        		}
        								
        		ServerApp.Log("Action: Writing to file, '" + strFilePathName + "', for outputting messages failed.", exError);
        	}

            return boolMsgWrite;
        }       
	}

    /// <summary>
    ///     Server Settings
    /// </summary>
    internal class ServerSettings {

        private XmlDocument xdConfig = new XmlDocument();
                                    /* XML Config Document */
        private int nMaxMsgBytes = 8192,
                                    /* Maximum Number of Bytes That Can Be Sent or Received */
                    nUDPMaxMsgBytes = 512,
                                    /* Maximum Number of Bytes for UDP Messages That Can Be Sent or Received */
                    nMaxIndicatorLen = 4;
                                    /* Maximum Indicator Length */
    	private string strHostName = "127.0.0.1",
                                    /* Server's Host Name */
                       strMsgStartChars = "%=&>",
                       strMsgEndChars = "<@^$",
                       strMsgPartEndChars = "!*+#";
                                    /* Characters That Symbolize the Start and End of a Message or Part of it */
        private char charMsgFiller = '\0';
                                    /* Message Filler */
        private int nMaxMsgBackups = 1000,
                    nPort = 59234,
                    nUDPPort = 59333,
                    nClientPeerPortRangeStart = 59432,
                    nClientPeerPortRangeEnd = 59532,
                    nDefaultHTTPPort = 80,
                    nDefaultHTTPSSLPort = 443,
                    nMaxDBConnectPerObj = 10,
                    nBDTimeoutInMillisecs = 300,
                    nMsgReplayTimeoutInMilliSecs = 100;  
                                    /* Maximum Number of Messages to Hold for Backup Replay, 
                                       Selected Port, 
                                       Default UDP Port, 
                                       Default Starting and Ending Range for Ports for Client Peer to Peer Connections
                                       Default HTTP Port
                                       Default HTTP SSL Port
                                       Default Number of Database Connections Specific Objects Can Make,
                                       Default Amount of Time for Database Connections to Return Data,
                                       Amount of Time to Wait in Requesting Message Replay from Client */
        private List<int> ltnClientPeerUsedPorts = new List<int>();
                                    /* List of Used Ports for Client Peer-To-Peer Connections */
	    private string strLogFilePath = "logs/",
	        						/* Path to Where Log Files are Saved to */
					   strFileStorePath = "files/",
									/* Path for Locating Files */	
    	               strCmdMsgStartChars = "+???",
                       strCmdMsgEndChars = "???+",
                       strCmdMsgPartEndChars = "+??+",
                                    /* Characters That Symbolize the Start, End, and End of Part of a Command Message */
                       strDBProcDelimiter = "//",
                                    /* Database Procedural Delimiter for Replacing Default ";" */
                       strDBStatementParamChars = "@";
                                    /* Database Leading Characters for Database Statement Parameter Variables */
        private bool boolDelExec = false,
            						/* Indicator to Deleted Executed Commands */
					 boolUseSSL = false,
                                    /* Indicator to Use SSL on Communications */
                     boolAllowMultipleSameIP = false,
                                    /* Indicator Allow Multiple Connections for the Same IP Address */
                     boolUseUDPClient = false,
                                    /* Indicator to Use UDP For Client Communications */
                     boolUseLogFile = false,
                                    /* Indicator to Use Log File */
                     boolStreamsOutAll = false,
									/* Indicator That Streams Go Out to All Other Streams */ 
                     boolDirectMsgOutAll = false,
                                    /* Indicator That All Direct Messages Are to Go Out to All Other Clients */
                     boolSendDirectMsgBack = false,
                                    /* Indicator That All Direct Messages Are to be Send Back to Sending Client */
                     boolClientPeerToPeer = false,
                                    /* Indicator That Client is to Use Peer to Peer Connections */
                     boolPeerToPeerEncrypt = false;
                                    /* Indicator to Use Encryption on Peer to Peer Connections */
        private string strDirectMsgDesign = "",
                                    /* Selected Direct Message Designation to Operations On */
                       strClientIDVar = "clientid",
                                    /* Variable for Passing Client ID to Backend Processes */
                       strGroupIDVar = "groupid",
                                    /* Variable for Passing Client Group ID to Backend Processes */
                       strClientIPAddressVar = "ipaddress",
                                    /* Variable for Passing Client IP Address to Backend Processes */
                       strTransIDVar = "transactionid",
                                    /* Variable for Passing Transaction ID to Backend Processes */
                       strRespIDVar = "responseid";
                                    /* Variable for Passing Response ID to Backend Processes */
        private bool boolPing = false,
                     boolHostPingCheck = false;
                                    /* Indicator to Ping Clients at an Interval */
        private int nPingIntervalInMillisecs = 500,
                                    /* Amount of Time Between Pings in Milliseconds */
                    nHostPingCheckWaitInMilliSecs = 500,
                                    /* Amount of Time to Wait for Host Ping Check */
                    nWebSocketConnectWaitInMilliSecs = 500;
                                    /* After Web Socket Connection, Wait for HandShake to Complete Before Sending Messages */
        private string strDefaultMsgTableName = "revcomm_msg_out",
                       strDefaultMsgIDFieldName = "revcomm_msg_out_id",
                       strDefaultMsgValueFieldName = "msg",
                       strDefaultMsgIDDataType = "INT",
                       strDefaultMsgValueDataType = "BLOB";
                                    /* Default Names for Server Command Messaging Table, ID Field and Value Field */
        private Dictionary<string, uint> dictDBTypeMaxSize = new Dictionary<string, uint> {
            { "CHAR", 255 },
            { "VARCHAR", 65535 },
            { "BINARY", 255 },
            { "VARBINARY", 65535 },
            { "TINYBLOB", 255 },
            { "TINYTEXT", 255 },
            { "TEXT", 65535 },
            { "BLOB", 65535 },
            { "MEDIUMTEXT", 16777215 },
            { "MEDIUMBLOB", 16777215 },
            { "LONGTEXT", 4294967295 },
            { "LONGBLOB", 4294967295 }
        };                          /* Maximum Size for Outputting Data for Server Command Messaging Table */
        private bool boolMsgTableCreate = true;
                                    /* Indicator to Create or Recreate Message Table */
        private List<DatabaseOut> ltdoDBDirectMsgStore = new List<DatabaseOut>();
        							/* List of Databases to Store Incoming Direct Messages in */
        private List<FileOut> ltfoFileDirectMsgStore = new List<FileOut>();
        							/* List of Databases to Store Incoming Direct Messages in */
		private string strSSLCertName = "", 
									/* Name of the Server's SSL Certificate */
                       strSSLPrivKeyName = "";
                                    /* Name of the Private Key for the Server's SSL Certificate */
		private int nRandSeed = new Random().Next();
									/* Seed for Generating Transaction IDs */
		private List<ServerCommands> ltscCommands = new List<ServerCommands>();
									/* List of Retrievers for Server Commands */
		private bool boolOutUseSSL = false;
									/* Indicator That Selected Outgoing Stream or HTTP Transmission is Using SSL */
        private List<CommTrans> ltctOut = new List<CommTrans>();
        							/* Outgoing Transmissions */
        private Dictionary<string, DatabaseExecutor> dictdeDBDataStore = new Dictionary<string, DatabaseExecutor>();
        							/* List of Databases to Run Queries On Stored by Designation */
        private List<DatabaseOut> ltdoDBClientMsgStore = new List<DatabaseOut>();
        							/* List of Databases to Store Incoming Client Messages in */
        private List<FileOut> ltfoFileClientMsgStore = new List<FileOut>();
        							/* List of Files to Store Incoming Client Messages in */
        private Dictionary<string, string> dcDownloadFileList = new Dictionary<string, string>();
                                    /* List of Server Files That Can be Loaded */
        private Assembly ambOpenSSLDLL = null;
                                    /* OpenSLL DLL Used for DTLS for UDP Client Communcations */
        private object objsctxAccessor;
                                    /* Sets Up DTLS Layer for UDP Client */
        private bool boolNotServerCmdSetup = true,
                     boolNotTransferSetup = true;
                                    /* Server Commands and Transfer Out Setup Not Setup */
        private delegate void WindowCloseCheckFunct(CONTROLMSGS cmMsg);
                                    /* Function Type for Checking If Window Containing Server is Shutting Down */
        private enum CONTROLMSGS { CTRL_C_EVENT = 0, CTRL_BREAK_EVENT, CTRL_CLOSE_EVENT, CTRL_LOGOFF_EVENT = 5, CTRL_SHUTDOWN_EVENT };
                                    /* Window Control Messages for Checking for Shutdown */
        private WindowCloseCheckFunct sscCheckWindowEnd;
                                    /* Checks If Window is Closing */
        private bool boolServerRunning = true,
                                    /* Indicator That Server is Running */                        
                     boolDoDataComSetup = true;
                                    /* Indicator to Do Setup for Database Communications */   
        							
        /// <param name="strXMLSettingFilePath">Path to Server XML Settings File</param>
        public ServerSettings(string strXMLSettingFilePath) {
            
            int nSettingValue = 0;	/* Value from XML Path in File If Converting to Integer */
            string strSettingValue = "";
                                    /* Setting's Value */
            XmlNodeList xnlSettings;/* Selected XML Settings List */
            bool boolNotFound = true;
                                    /* Indicator to Searched Value was not Found */
 //           Type typFuncCall = ambOpenSSLDLL.GetType();
                                    /* Function Container for OpenSLL DLL */

            try { 

                xdConfig.Load(strXMLSettingFilePath);
            
                /* Get Settings for Server Hostname and Port */
                strSettingValue = GetConfigSetting("//settings/hostname");
            
                if (strSettingValue != "") {
            
            	    strHostName = strSettingValue;
                }
            
                strSettingValue = GetConfigSetting("//settings/port");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nPort = nSettingValue;
                }

                strSettingValue = GetConfigSetting("//settings/defaulthttpport");

                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {

                    nDefaultHTTPPort = nSettingValue;
                }

                strSettingValue = GetConfigSetting("//settings/defaulthttpsslport");

                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {

                    nDefaultHTTPSSLPort = nSettingValue;
                }

                /* Allow Multiple Connections for the Same IP Address */
                bool.TryParse(GetConfigSetting("//settings/allowmultiplesameip"), out boolAllowMultipleSameIP);

                strSettingValue = GetConfigSetting("//settings/msgs/maxbytes");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nMaxMsgBytes = nSettingValue;
                }
            
                strSettingValue = GetConfigSetting("//settings/msgs/maxindicator");

                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {

                    if (nSettingValue > 0) { 
 
            	        nMaxIndicatorLen = nSettingValue;
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Setting message indicator maximum length to invalid value.");
                    }
                }
            
                /* Get Characters for Indicating Start, End, and Part of End Stream Message and Message Filler Character */
			    strSettingValue = GetConfigSetting("//settings/msgs/start");
            
                if (strSettingValue != "") {

                    if (strSettingValue != strMsgEndChars && 
                        strSettingValue != strMsgPartEndChars) {

                        if (strSettingValue.Length == nMaxIndicatorLen) { 

                	        strMsgStartChars = strSettingValue;
                        }
                        else {

                            ServerApp.Log("Action: Getting server settings. Error: Invalid number of starting indicator characters.");
                        }
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Starting indicator characters can not be the same as other indicator characters.");
                    }
                }

                strSettingValue = GetConfigSetting("//settings/msgs/end");
            
                if (strSettingValue != "") {

                    if (strSettingValue != strMsgStartChars && 
                        strSettingValue != strMsgPartEndChars) {

                        if (strSettingValue.Length == nMaxIndicatorLen) { 
            
            	            strMsgEndChars = strSettingValue;                    
                        }
                        else {

                            ServerApp.Log("Action: Getting server settings. Error: Invalid number of ending indicator characters.");
                        }
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Ending indicator characters can not be the same as other indicator characters.");
                    }
                }

                strSettingValue = GetConfigSetting("//settings/msgs/partend");
            
                if (strSettingValue != "") {
            
                    if (strSettingValue != strMsgStartChars && 
                        strSettingValue != strMsgEndChars) {

                        if (strSettingValue.Length == nMaxIndicatorLen) {
            	    
                            strMsgPartEndChars = strSettingValue;                        
                        }
                        else {

                            ServerApp.Log("Action: Getting server settings. Error: Invalid number of part end indicator characters.");
                        }
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Part end indicator characters can not be the same as other indicator characters.");
                    }
                }

                strSettingValue = GetConfigSetting("//settings/msgs/fillerchar");
            
                if (strSettingValue != "") {
            
            	    charMsgFiller = strSettingValue.ToCharArray()[0];
                }

                strSettingValue = GetConfigSetting("//settings/msgs/maxbackups");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {

                    if (nSettingValue >= 0) { 
 
            	        nMaxMsgBackups = nSettingValue;
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Setting maximum number of stored messages for replay to invalid value.");
                    }
                }

                /* Default Timeout for Requesting Message Replay from Client */
                strSettingValue = GetConfigSetting("//settings/msgs/msgreplaytimeout");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nMsgReplayTimeoutInMilliSecs = nSettingValue;
                }

                /* Send All Incoming Client Stream Transmission to All Clients Streams */
                bool.TryParse(GetConfigSetting("//settings/streams/sendall"), out boolStreamsOutAll);

                /* Send All Incoming Direct Message Transmissions to All Clients Streams */
                bool.TryParse(GetConfigSetting("//settings/directmsg/sendallclients"), out boolDirectMsgOutAll);

                strSettingValue = GetConfigSetting("//settings/directmsg/designation");
            
                if (strSettingValue != "") {
            
            	    strDirectMsgDesign = strSettingValue;
                }

                /* Send All Incoming Direct Message Transmissions Back to the Sending Client */
                bool.TryParse(GetConfigSetting("//settings/directmsg/sendback"), out boolSendDirectMsgBack);

                /* Default Number of Maximum Database Connections */
                strSettingValue = GetConfigSetting("//settings/database/maxconnectionsperobject");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nMaxDBConnectPerObj = nSettingValue;
                }
                
                /* Timeout for Database Connection Returns */
                strSettingValue = GetConfigSetting("//settings/database/connectiontimeout");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
                    if (nSettingValue > 0) {

                	    nBDTimeoutInMillisecs = nSettingValue;
                    }
                    else { 
                    
                        ServerApp.Log("Action: Getting server settings. Error: Database conenction can not be set to 0, setting to default value of " +  nBDTimeoutInMillisecs + ".");
                    }
                }

                /* Have Client Use UDP Communcations with the Server */
                bool.TryParse(GetConfigSetting("//settings/updclients/enabled"), out boolUseUDPClient);

                /* Default Port for UPD Connections */
                strSettingValue = GetConfigSetting("//settings/updclients/port");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nUDPPort = nSettingValue;
                }

                /* Maximum Number of Bytes for UDP Messages */
                strSettingValue = GetConfigSetting("//settings/updclients/maxbytes");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nUDPMaxMsgBytes = nSettingValue;
                }

                /* Have Client Directly Connected Peer to Peer to Each Other */
                bool.TryParse(GetConfigSetting("//settings/peertopeer/enabled"), out boolClientPeerToPeer);

                /* Starting Port Range for Client Peer to Peer Connections */
                strSettingValue = GetConfigSetting("//settings/peertopeer/port/start");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nClientPeerPortRangeStart = nSettingValue;
                }
                
                /* Ending Port Range for Client Peer to Peer Connections */
                strSettingValue = GetConfigSetting("//settings/peertopeer/port/end");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nClientPeerPortRangeEnd = nSettingValue;
                }

                if (nClientPeerPortRangeStart >= nClientPeerPortRangeEnd) {

                    throw new Exception("Action: Getting default settings. Peer-To-Peer port range is invalid. Starting port: " +
                                        nClientPeerPortRangeStart + " must be less than end port: " + nClientPeerPortRangeEnd);
                }

                /* Use Encryption with Peer to Peer Connections */
                bool.TryParse(GetConfigSetting("//settings/peertopeer/encryption"), out boolPeerToPeerEncrypt);

                /* Variable for Passing Client ID, Group ID, IP Address, Transaction ID, and Response ID to Backend Processes */
                if ((strSettingValue = GetConfigSetting("//settings/userdata/clientid")) != "") {

                    strClientIDVar = strSettingValue;
                }

                if ((strSettingValue = GetConfigSetting("//settings/userdata/groupid")) != "") {

                    strGroupIDVar = strSettingValue;
                }

                if ((strSettingValue = GetConfigSetting("//settings/userdata/ipaddress")) != "") {

                    strClientIPAddressVar = strSettingValue;
                }

                if ((strSettingValue = GetConfigSetting("//settings/userdata/transactionid")) != "") {

                    strTransIDVar = strSettingValue;
                }

                if ((strSettingValue = GetConfigSetting("//settings/userdata/responseid")) != "") {

                    strRespIDVar = strSettingValue;
                }

                /* Have Client Pinged at Intervals */
                bool.TryParse(GetConfigSetting("//settings/ping/enabled"), out boolPing);

                /* Ping Interval */
                strSettingValue = GetConfigSetting("//settings/ping/interval");
            
                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {
            
            	    nPingIntervalInMillisecs = nSettingValue;
                }

                /* Have Host Designation for Session Groups Decided by Ping Checks */
                bool.TryParse(GetConfigSetting("//settings/hosting/bypingspeed"), out boolHostPingCheck);

                /* Length of the Time for Ping Checks for Host Designation for Session Groups  */
                strSettingValue = GetConfigSetting("//settings/hosting/timelimit");

                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {

                    nHostPingCheckWaitInMilliSecs = nSettingValue;
                }

                /*  Time to Wait Before Sending Message on Web Socket Connections to Allow for Handshake  */
                strSettingValue = GetConfigSetting("//settings/websockets/waittime");

                if (strSettingValue != "" && int.TryParse(strSettingValue, out nSettingValue)) {

                    nWebSocketConnectWaitInMilliSecs = nSettingValue;
                }

                /* Get Indicators To Use SSL on Incoming Transmissions */
                bool.TryParse(GetConfigSetting("//settings/ssl/enabled"), out boolUseSSL);

                /* Get Server's SSL Certification Name If Set */
                strSSLCertName = GetConfigSetting("//settings/ssl/certificatename");

                /* Get Name of Private Key for Server's SSL Certification If Set */
                strSSLPrivKeyName = GetConfigSetting("//settings/ssl/privatekeyname");

                /* Get If Log Should be Used, and Where it Should be Created */
                bool.TryParse(GetConfigSetting("//settings/logging/enabled"), out boolUseLogFile);

                strSettingValue = GetConfigSetting("//settings/logfolderpath");
            
                if (strSettingValue != "") {
				
				    if (!strSettingValue.EndsWith("/")) {
				
					    strSettingValue += "/";
				    }
            
            	    strLogFilePath = strSettingValue;
                }
			
			    /* Get Location of Referred to Files */
			    strSettingValue = GetConfigSetting("//settings/filefolderpath");
            
                if (strSettingValue != "") {
				
				    if (!strSettingValue.EndsWith("/")) {
				
					    strSettingValue += "/";
				    }
            
				    strFileStorePath = strSettingValue;
                }
			
			    /* Get Characters for Indicating End of Part of Command Messages, 
				   Indicator to Delete Commands from Source After Execution */
                strSettingValue = GetConfigSetting("//commands/msgpartendchars");
            
                if (strSettingValue != "") {
            
                    if (strSettingValue != strCmdMsgStartChars && 
                        strSettingValue != strCmdMsgEndChars) {

                        if (strSettingValue.Length == nMaxIndicatorLen) {                            

                            foreach (char charSelected in strMsgStartChars) {
                                
                                if (strCmdMsgEndChars.Contains(charSelected.ToString())) {

                                    boolNotFound = false;
                                    break;
                                }
                            }

                            if (boolNotFound) { 

                                foreach (char charSelected in strMsgPartEndChars) {
                                
                                    if (strCmdMsgEndChars.Contains(charSelected.ToString())) {

                                        boolNotFound = false;
                                        break;
                                    }
                                }

                                if (boolNotFound) { 

                                    foreach (char charSelected in strMsgEndChars) {
                                
                                        if (strCmdMsgEndChars.Contains(charSelected.ToString())) {

                                            boolNotFound = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (boolNotFound) {
            
            	                strCmdMsgPartEndChars = strSettingValue;
                            }
                            else {

                                ServerApp.Log("Action: Getting server settings. Error: Part ending indicator characters for commands can not share characters with message identicators.");
                            }
                        }
                        else {

                            ServerApp.Log("Action: Getting server settings. Error: Invalid number of part ending indicator characters for commands.");
                        }
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Part ending indicator characters for commands can not be the same as other indicator characters for commands.");
                    }
                } 

			    /* Get Characters for Indicating Start of Command Messages, 
				   Indicator to Delete Commands from Source After Execution */
                strSettingValue = GetConfigSetting("//commands/msgstartchars");
            
                if (strSettingValue != "") {
            
                    if (strSettingValue != strCmdMsgPartEndChars && 
                        strSettingValue != strCmdMsgEndChars) {

                        if (strSettingValue.Length == nMaxIndicatorLen) {

                            boolNotFound = true;

                            foreach (char charSelected in strMsgStartChars) {
                                
                                if (strCmdMsgStartChars.Contains(charSelected.ToString())) {

                                    boolNotFound = false;
                                    break;
                                }
                            }

                            if (boolNotFound) { 

                                foreach (char charSelected in strMsgPartEndChars) {
                                
                                    if (strCmdMsgStartChars.Contains(charSelected.ToString())) {

                                        boolNotFound = false;
                                        break;
                                    }
                                }

                                if (boolNotFound) { 

                                    foreach (char charSelected in strMsgEndChars) {
                                
                                        if (strCmdMsgStartChars.Contains(charSelected.ToString())) {

                                            boolNotFound = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (boolNotFound) {

                                strCmdMsgStartChars = strSettingValue;
                            }
                            else {

                                ServerApp.Log("Action: Getting server settings. Error: Starting indicator characters for commands can not share characters with message identicators.");
                            }
                        }
                        else {

                            ServerApp.Log("Action: Getting server settings. Error: Invalid number of starting indicator characters for commands.");
                        }
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Starting indicator characters for commands can not be the same as other indicator characters for commands.");
                    }
                }

			    /* Get Characters for Indicating End of Command Messages, 
				   Indicator to Delete Commands from Source After Execution */
                strSettingValue = GetConfigSetting("//commands/msgendchars");
            
                if (strSettingValue != "") {
                        
                    if (strSettingValue != strCmdMsgStartChars && 
                        strSettingValue != strCmdMsgPartEndChars) {

                        if (strSettingValue.Length == nMaxIndicatorLen) {
            	        
                            boolNotFound = true;

                            foreach (char charSelected in strMsgStartChars) {
                                
                                if (strCmdMsgEndChars.Contains(charSelected.ToString())) {

                                    boolNotFound = false;
                                    break;
                                }
                            }

                            if (boolNotFound) { 

                                foreach (char charSelected in strMsgPartEndChars) {
                                
                                    if (strCmdMsgEndChars.Contains(charSelected.ToString())) {

                                        boolNotFound = false;
                                        break;
                                    }
                                }

                                if (boolNotFound) { 

                                    foreach (char charSelected in strMsgEndChars) {
                                
                                        if (strCmdMsgEndChars.Contains(charSelected.ToString())) {

                                            boolNotFound = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (boolNotFound) {

                                strCmdMsgEndChars = strSettingValue;
                            }
                            else {

                                ServerApp.Log("Action: Getting server settings. Error: Ending indicator characters for commands can not share characters with message identicators.");
                            }
                        }
                        else {

                            ServerApp.Log("Action: Getting server settings. Error: Invalid number of ending indicator characters for commands.");
                        }
                    }
                    else { 

                        ServerApp.Log("Action: Getting server settings. Error: Ending indicator characters for commands can not be the same as other indicator characters for commands.");
                    }
                }
            
			    bool.TryParse(GetConfigSetting("//commands/@deleteexecuted"), out boolDelExec);
                
                /* Get Characters for Executing Procedural Database Commands */
                strSettingValue = GetConfigSetting("//commands/dataprocdelimiter");

                if (strSettingValue != "") {

                    strDBProcDelimiter = strSettingValue;
                }

                /* Setup Client Outgoing Message File Relay */
                xnlSettings = GetConfigSettingsList("//relay/file");
			
			    foreach (XmlNode xnSelect in xnlSettings) {
				
				    if (xnSelect.SelectSingleNode("name").InnerText.Trim() != "") {
					
					    ltfoFileClientMsgStore.Add(new FileOut(xnSelect.SelectSingleNode("name").InnerText.Trim(),
                                                               xnSelect.SelectSingleNode("path").InnerText.Trim()));
				    }
				    else {

                        ServerApp.Log("Action: Getting server settings. Error: Setting up direct message output file, name: '" + 
					                  xnSelect.SelectSingleNode("name").InnerText.Trim() + "', path: '" +
					                  xnSelect.SelectSingleNode("path").InnerText.Trim() + "', failed.");
				    }	
			    }
			
			    /* Setup Client Outgoing Message Database Relay */
			    xnlSettings = GetConfigSettingsList("//relay/database");
			
			    foreach (XmlNode xnSelect in xnlSettings) {
				
				    ltdoDBClientMsgStore.Add(new DatabaseOut(xnSelect.SelectSingleNode("server").InnerText.Trim(),
												             xnSelect.SelectSingleNode("name").InnerText.Trim(),
												             xnSelect.SelectSingleNode("username").InnerText.Trim(),
												             xnSelect.SelectSingleNode("password").InnerText.Trim(),
												             xnSelect.SelectSingleNode("table").InnerText.Trim(),
												             xnSelect.SelectSingleNode("msgfield").InnerText.Trim()));
			    }   
			
			    /* Setup Client Outgoing Message Database Relay */
			    xnlSettings = GetConfigSettingsList("//operations/data/database");
			
			    foreach (XmlNode xnSelect in xnlSettings) {
                    
                    if (xnSelect.SelectSingleNode("@designation").InnerText.Trim() != "") {

                        try {

                            strSettingValue = xnSelect.SelectSingleNode("paramchars").InnerText.Trim();
                        }
                        catch (Exception) {

                            strSettingValue = strDBStatementParamChars;
                        }

                        dictdeDBDataStore.Add(xnSelect.SelectSingleNode("@designation").InnerText.Trim(),
                                              new DatabaseExecutor(xnSelect.SelectSingleNode("server").InnerText.Trim(),
                                                                   xnSelect.SelectSingleNode("name").InnerText.Trim(),
                                                                   xnSelect.SelectSingleNode("username").InnerText.Trim(),
                                                                   xnSelect.SelectSingleNode("password").InnerText.Trim(),
                                                                   strSettingValue));

                        if (boolDoDataComSetup) {

                            dictdeDBDataStore[xnSelect.SelectSingleNode("@designation").InnerText.Trim()].Setup();
                            boolDoDataComSetup = false;
                        }
                    }
                   else {

                        ServerApp.Log("Action: Getting server settings. Error: Setting up database executor failed due to missing designation.");
				    }
			    }
                
                /* Setup DTLS for Use with UDP Clients */
                if (boolUseUDPClient && boolUseSSL) {

                    ambOpenSSLDLL = Assembly.LoadFile("libsslMD.dll");
                    System.Type typFuncCall = ambOpenSSLDLL.GetType();

                    typFuncCall.GetMethod("SSL_load_error_strings").Invoke(null, new object[] { });
                    typFuncCall.GetMethod("OPENSSL_init_ssl").Invoke(null, new object[] { 0, null });
                
                    if ((objsctxAccessor = typFuncCall.GetMethod("SSL_CTX_new").
                                           Invoke(null, new object[] { typFuncCall.GetMethod("DTLSv1_2_server_method").
                                                                       Invoke(null, new object[] { })
                                                                     })) == null) {

                        ServerApp.Log("Action: Getting server settings. Error: Setting up OpenSSL DTLS accessor failed.");
                    }
                }
            }
            catch (Exception exError) {

                ServerApp.Log("Action: Loading server settings from config file, '" + strXMLSettingFilePath + "'.", exError);
            }

            /* Setup to Check for Window Shutdown Events */
            sscCheckWindowEnd = new WindowCloseCheckFunct(WindowCloseCheck);
            SetConsoleCtrlHandler(sscCheckWindowEnd, true);
        }

		/// <summary>
		/// 	Gets Single Value from XML Configuration File
		/// </summary>
		/// <param name="strXPath">XML Path to Find Settings Value in File</param>
		/// <returns>Setting Value</returns>
		private string GetConfigSetting(string strXPath) {
		
            XmlNode xnInfoSelect = null;	
                                    /* Selected XML Setting Information */
            string strValue = "";	/* Value from XML Path in File */
                
            try {
            	
                xnInfoSelect = xdConfig.SelectSingleNode(strXPath);
                
                if (xnInfoSelect != null) {
                
                	strValue = xnInfoSelect.InnerText.Trim();
                }
            }
            catch (Exception exError) {
                                    	
            	ServerApp.Log("Action: Getting server setting from XML configuration file using xpath, '" + 
                              strXPath + "'.", exError);
            }
            
            return strValue;
		}
		
		/// <summary>
		/// 	Gets List of Settings from XML Configuration File
		/// </summary>
		/// <param name="strXPath">XML Path to Find List of Settings in File</param>
		/// <returns>List of Settings in XML</returns>
		private XmlNodeList GetConfigSettingsList(string strXPath) {
		
			XmlNodeList xnlSettings = null;
                                    /* Selected XML Settings List */
                
            try {
            	
                xnlSettings = xdConfig.SelectNodes(strXPath);
            }
            catch (Exception exError) {
                                    	
            	ServerApp.Log("Action: Getting list of server settings from XML configuration file using xpath, '" + 
                              strXPath + "'.", exError);
            }
            
            return xnlSettings;
		}

        /// <summary>
        ///     Indicator That Server is Running
        /// </summary>
        public bool Running { 
        
            get {

                return boolServerRunning;
            }
        }

        /// <summary>
        ///     Gets Maximum Message Length
        /// </summary>
        public int MaxMsgLen {

            get {

                return nMaxMsgBytes;
            }
        }
        
		/// <summary>
		/// 	Gets Hosting Server Name
		/// </summary>
        public string Host {
        
        	get {
                  
				return strHostName;                                  		
            }
        }
        
		/// <summary>
		/// 	Gets Access Port Hosting Server Name
		/// </summary>
        public int Port {
        
        	get {
                  
				return nPort;                                  		
            }
        }
        
		/// <summary>
		/// 	Gets Access Default HTTP Port 
		/// </summary>
        public int DefaultHTTPPort {
        
        	get {

                return nDefaultHTTPPort;                                  		
            }
        }
        
		/// <summary>
		/// 	Gets Access Default HTTP SSL Port 
		/// </summary>
        public int DefaultHTTPSSLPort {
        
        	get {

                return nDefaultHTTPSSLPort;                                  		
            }
        }

        /// <summary>
        /// 	Gets Indicator to Allow Multiple Connections from the Same IP Address
        /// </summary>
        public bool AllowMultipleSameIP {
        
        	get {

                return boolAllowMultipleSameIP;
            }
        }

        /// <summary>
        /// 	Gets Indicator Characters for Start of Communication Messages
        /// </summary>
        public string StartChars {
        
        	get {
                  
				return strMsgStartChars;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator Characters for End of Communication Messages
		/// </summary>
        public string EndChars {
        
        	get {
                  
				return strMsgEndChars;                                  		
            }
        }
        
		/// <summary>
		/// 	Gets Indicator Characters for Part of Communication Messages
		/// </summary>
        public string PartEndChars {
        
        	get {
                  
				return strMsgPartEndChars;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Character to Fill Empty Parts of Communication Messages
		/// </summary>
        public char Filler {
        
        	get {
                  
				return charMsgFiller;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Maximum Number of Messages to be Stored in Backup for Replay
		/// </summary>
        public int MaxBackupLimit {
        
        	get {
                  
				return nMaxMsgBackups;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator That Logs are Being Saved to Files
		/// </summary>
        public bool UseLogFile {
        
        	get {
                  
				return boolUseLogFile;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Path to Log File
		/// </summary>
        public string LogFilePath {
        
        	get {
                  
				return strLogFilePath;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Location Path of Referenced Files
		/// </summary>
        public string FileLocPath {
        
        	get {

                return strFileStorePath;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator Characters for Part of Command Messages
		/// </summary>
        public string CmdMsgPartEndChars {
        
        	get {
                  
				return strCmdMsgPartEndChars;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator Characters for Start of Command Messages
		/// </summary>
        public string CmdMsgStartChars {
        
        	get {
                  
				return strCmdMsgStartChars;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator Characters for End of Command Messages
		/// </summary>
        public string CmdMsgEndChars {
        
        	get {
                  
				return strCmdMsgEndChars;                                  		
            }
        }

        /// <summary>
        ///     Gets Character for Database Procedural Delimiter Replacement Value
        /// </summary>
        public string DBProcDelimiter {

            get {

                return strDBProcDelimiter;
            }
        }

        /// <summary>
        ///     Gets Character for Database Parameter Indicator
        /// </summary>
        public string DBParamChar {

            get {

                return strDBStatementParamChars;
            }
        }

        /// <summary>
        /// 	Gets Variable for Passing Client ID to Backend Processes
        /// </summary>
        public string ClientIDVar {
        
        	get {
                  
				return strClientIDVar;                                  		
            }
        }

        /// <summary>
        /// 	Gets Variable for Passing Client Group ID to Backend Processes
        /// </summary>
        public string GroupIDVar {
        
        	get {
                  
				return strGroupIDVar;                                  		
            }
        }
		
		/// <summary>
        /// 	Gets Variable for Passing Client IP Address to Backend Processes
		/// </summary>
        public string ClientIPAddressVar {
        
        	get {
                  
				return strClientIPAddressVar;                                  		
            }
        }
		
		/// <summary>
        /// 	Gets Variable for Passing Transaction ID to Backend Processes
		/// </summary>
        public string TransactionIDVar {
        
        	get {
                  
				return strTransIDVar;                                  		
            }
        }
		
		/// <summary>
        /// 	Gets Variable for Passing Response ID to Backend Processes
		/// </summary>
        public string ResponseIDVar {
        
        	get {
                  
				return strRespIDVar;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator to Delete Executed Command Messages
		/// </summary>
        public bool CmdMsgDelExec {
        
        	get {
                  
				return boolDelExec;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Indicator to Use SSL on Incoming Transmission
		/// </summary>
		public bool IncSSL {
        
        	get {
                  
				return boolUseSSL;                                  		
            }
        }
		
		/// <summary>
		/// 	Gets Timeout for Message Replay Request to Client in Milliseconds
		/// </summary>
		public int MsgReplayTimeout {
        
        	get {
                  
				return nMsgReplayTimeoutInMilliSecs;                                  		
            }
        }
		
		
		/// <summary>
        /// 	Gets Indicator to Send All Incoming Client Stream Transmission to All Clients Streams
		/// </summary>
		public bool StreamsSendAllOut {
        
        	get {
                  
				return boolStreamsOutAll;                                  		
            }
        }

        /// <summary>
        ///     Gets Indicator That Direct Message is Go Out to All Clients
        /// </summary>
        public bool DirectMsgSendAllClients {

            get {

                return boolDirectMsgOutAll;
            }
        }

        /// <summary>
        ///     Gets Designation of Direct Message to Do Specified Operations to, or 
        ///     Empty String if Doing Operations on All Direct Messages
        /// </summary>
        public string DirectMsgDesignSelected { 
        
            get {
    
                return strDirectMsgDesign;
            }
        }

        /// <summary>
        ///     Gets Indicator That All Direct Message are Also to be Send Back to Sending Client
        /// </summary>
        public bool DirectMsgBackSender { 
        
            get {
    
                return boolSendDirectMsgBack;
            }
        }

        /// <summary>
        ///     Gets Maximum Number of Database Connections an Objects Can Have
        /// </summary>
        public int DBMaxConnectPerObj { 
        
            get {

                return nMaxDBConnectPerObj;
            }
        }

        /// <summary>
        ///     Gets Maximum Amount of Time Database Connections Have to Return Data
        /// </summary>
        public int DBConnectTimeout { 
        
            get {

                return nBDTimeoutInMillisecs;
            }
        }        

        /// <summary>
        ///     Gets Indicator to Use UDP Communications for Clients
        /// </summary>
        public bool UseUDPClients { 
        
            get {

                return boolUseUDPClient;
            }
        }
        
		/// <summary>
		/// 	Gets UDP Port 
		/// </summary>
        public int UDPPort {
        
        	get {
                  
				return nUDPPort;                                  		
            }
        }

        /// <summary>
        /// 	Gets Maximum Message Length of UDP Messages
        /// </summary>
        public int UDPMaxMsgLen {
        
        	get {
                  
				return nUDPMaxMsgBytes;                                  		
            }
        }

        /// <summary>
        ///     Gets Indicator That Clients are to Directly Connect Peer to Peer
        /// </summary>
        public bool ClientPeerToPeer { 
        
            get {

                return boolClientPeerToPeer;
            }
        }
        
		/// <summary>
		/// 	Has Port Availble for Client Peer to Peer Connections
		/// </summary>
        public bool HasClientPeerPortAvailable {
        
        	get {
                  
				return nClientPeerPortRangeEnd - nClientPeerPortRangeStart > ltnClientPeerUsedPorts.Count;                                  		
            }
        }
        
		/// <summary>
		/// 	Gets Default Access Port for Client Peer to Peer Connections
		/// </summary>
        public int AvailableClientPeerToPeerPort {
        
        	get {

                // Random randPortSelector = new Random();
                int nPeerToPeerPort = 0;
 //                int nEndPortRange = nClientPeerPortRangeEnd + 1;

                if (HasClientPeerPortAvailable) {

                    Random randPortSelector = new Random();
                    int nEndPortRange = nClientPeerPortRangeEnd + 1;
                    nPeerToPeerPort = randPortSelector.Next(nClientPeerPortRangeStart, nEndPortRange);

                    while (ltnClientPeerUsedPorts.Contains(nPeerToPeerPort)) { 
                
                        nPeerToPeerPort = randPortSelector.Next(nClientPeerPortRangeStart, nEndPortRange);
                    }
                }
                else {

                    throw new Exception("Action: Getting available Peer-To-Peer port. No ports are available.");
                }

				return nPeerToPeerPort;                                  		
            }
        }

        /// <summary>
        ///     Remove Port Registered as "Peer-To-Peer" Used Connection
        /// </summary>
        /// <param name="nUsedPeerClientPort">Used Port Number</param>
        public void RemoveUsedClientPeerToPeerPort(int nUsedPeerClientPort) { 
        
            if (ltnClientPeerUsedPorts.Contains(nUsedPeerClientPort)) {

                ltnClientPeerUsedPorts.Remove(nUsedPeerClientPort);
            }
        }
        
		/// <summary>
		/// 	Gets Indicator to Use Encryption for Client Peer to Peer Connections
		/// </summary>
        public bool ClientPeerEncryption {
        
        	get {
                  
				return boolPeerToPeerEncrypt;                                  		
            }
        }

        /// <summary>
        ///     Gets Indicator That Clients Should be Pinged at Intervals
        /// </summary>
        public bool Ping { 
        
            get {

                return boolPing;
            }
        }
        
		/// <summary>
		/// 	Gets Ping Interval in Milliseconds
		/// </summary>
        public int PingInterval {
        
        	get {
                  
				return nPingIntervalInMillisecs;                                  		
            }
        }

        /// <summary>
        ///     Gets Indicator That Session Group Host Designation is Done by Speed Ping Check
        /// </summary>
        public bool HostPingCheck { 
        
            get {

                return boolHostPingCheck;
            }
        }

        /// <summary>
        /// 	Gets Time Limit on Speed Ping Check for Designating Host Of Session Group
        /// </summary>
        public int HostPingWait {
        
        	get {
                  
				return nHostPingCheckWaitInMilliSecs;                                  		
            }
        }

        /// <summary>
        /// 	Gets Time to Wait for WebSocket Connections to Complete Handshake Before Sending Messages
        /// </summary>
        public int WebSocketConnectionWait {
        
        	get {
                  
				return nWebSocketConnectWaitInMilliSecs;                                  		
            }
        }
        

		/// <summary>
		/// 	Gets Server's SSL Certification Name
		/// </summary>
        public string SSLCertName {
        
        	get {
                  
				return strSSLCertName;                                  		
            }
        }
        

		/// <summary>
		/// 	Gets Name of Private Key for Server's SSL Certification
		/// </summary>
        public string SSLPrivKeyName {
        
        	get {
                  
				return strSSLPrivKeyName;                                  		
            }
        }

		/// <summary>
		/// 	Gets List of Server Commands to be Executed
		/// </summary>
		public List<string> ServerCmds {
		
			get {

                List<string> ltstrCmds = new List<string>();
                                    /* List of Commands for Server */
//                XmlNodeList xnlSettings;/* Selected XML Settings List */
//                ServerCommands scNewCommander;
                                        /* Holder for Newly Created Retriever of Server Commands */
//                string strFilePath = "",/* Optional Path to File */
//                       strMsgTableName = strDefaultMsgTableName,
//                       strMsgIDFieldName = strDefaultMsgIDFieldName,
//                       strMsgValueFieldName = strDefaultMsgValueFieldName,
//                       strMsgIDDataType = strDefaultMsgIDDataType,
//                       strMsgValueDataType = strDefaultMsgValueDataType,
                                        /* Message Table's Name and ID and Column Names and Data Types */
//                       strDBParamChar = "",
                                        /* Indicator Character for Database Procedure Parameter */
//                       strDatabaseDesign = "";
                                        /* Designation of Database Executer */
//                       strDataDesign = "";
                                        /* Designation Data Query of Database Executer */
//                StringBuilder sbDefaultCmdMsg = new StringBuilder();
                                        /* Default Server Command Message */
//                DatabaseExecutor deNewCommander;
                                        /* Database Executor for Server Commands */
//                bool boolCreateDataTable = boolMsgTableCreate,
                                        /* Indicator to Create or Recreate Message Table */
//                     boolUseDBSSL = false;
                                        /* Indicator to Setup Database Communication Processes */

                if (boolNotServerCmdSetup) {

                    try { 

                        XmlNodeList xnlSettings;
                        ServerCommands scNewCommander;
                        string strFilePath = "",
                               strMsgTableName = strDefaultMsgTableName,
                               strMsgIDFieldName = strDefaultMsgIDFieldName,
                               strMsgValueFieldName = strDefaultMsgValueFieldName,
                               strMsgIDDataType = strDefaultMsgIDDataType,
                               strMsgValueDataType = strDefaultMsgValueDataType,
                               strDBParamChar = "",
                               strDatabaseDesign = "",
                               strDataDesign = "";
                        StringBuilder sbDefaultCmdMsg = new StringBuilder();
                        bool boolCreateDataTable = boolMsgTableCreate,
                             boolUseDBSSL = false;
                        DatabaseExecutor deNewCommander;

                        /* Setup Receivers for Getting Server Commands */
                        xnlSettings = GetConfigSettingsList("//operations/commands/file");
			
			            foreach (XmlNode xnSelect in xnlSettings) {
				
				            scNewCommander = new ServerCommands();
				
                            try {

                                strFilePath = xnSelect.SelectSingleNode("path").InnerText.Trim();
                            }
                            catch {

                                strFilePath = "";
                            }

				            if (scNewCommander.UseFileCmds(xnSelect.SelectSingleNode("name").InnerText.Trim(), 
				                                           strFilePath)) {
					
					            ltscCommands.Add(scNewCommander);
				            }
				            else {
					
					            ServerApp.Log("Action: Getting server commands. Error: Setting up to get server commands from file: '" + 
					                          xnSelect.SelectSingleNode("name").InnerText.Trim() + "' failed.");
				            }
			            }
            
			            /* Setup Receivers for Getting Server Commands */
			            xnlSettings = GetConfigSettingsList("//operations/commands/database");
			
			            foreach (XmlNode xnSelect in xnlSettings) {

				            scNewCommander = new ServerCommands();

                            try {

                                strMsgTableName = xnSelect.SelectSingleNode("table").InnerText.Trim();
                            }
                            catch {

                                strMsgTableName = strDefaultMsgTableName;
                            }

                            try {

                                strMsgIDFieldName = xnSelect.SelectSingleNode("idfield").InnerText.Trim();
                            }
                            catch {
                                
                                strMsgIDFieldName = strDefaultMsgIDFieldName;
                            }

                            try {

                                strMsgValueFieldName = xnSelect.SelectSingleNode("valuefield").InnerText.Trim();
                            }
                            catch {
                                
                                strMsgValueFieldName = strDefaultMsgValueFieldName;
                            }

                            try {

                                strMsgIDDataType = xnSelect.SelectSingleNode("iddatatype").InnerText.Trim();

                                try {

                                    Enum.Parse(typeof(DatabaseExecutor.MYSQLNUMTYPES), strMsgIDDataType, true);
                                }
                                catch {

                                    strMsgIDDataType = strDefaultMsgIDDataType;

                                    ServerApp.Log("Action: Getting server commands. Error: Setting up to get server commands from database failed, " + 
                                                  "invalid id field type setting, reverting to default setting.");
                                }
                            }
                            catch {

                                strMsgIDDataType = strDefaultMsgIDDataType;
                            }

                            try {

                                strMsgValueDataType = xnSelect.SelectSingleNode("valuedatatype").InnerText.Trim();

                                if (!dictDBTypeMaxSize.ContainsKey(strMsgValueDataType)) { 
                                
                                    strMsgValueDataType = strDefaultMsgValueDataType;

                                    ServerApp.Log("Action: Getting server commands. Error: Setting up to get server commands from database failed, " + 
                                                  "invalid data field type setting, reverting to default setting.");
                                }

                            }
                            catch {

                                strMsgValueDataType = strDefaultMsgValueDataType;
                            }

                            try {

                                bool.TryParse(xnSelect.SelectSingleNode("createmsgtable").InnerText.Trim(), out boolCreateDataTable);
                            }
                            catch {

                                boolCreateDataTable = boolMsgTableCreate;
                            }

                            try {

                                strDBParamChar = xnSelect.SelectSingleNode("paramchars").InnerText.Trim();
                            }
                            catch {

                                strDBParamChar = ServerApp.Settings.DBParamChar;
                            }

                            try {

                                bool.TryParse(xnSelect.SelectSingleNode("usessl").InnerText.Trim(), out boolUseDBSSL);
                            }
                            catch {

                                boolUseDBSSL = false;
                            }

                            if (scNewCommander.UseDatabaseCmds(xnSelect.SelectSingleNode("server").InnerText.Trim(), 
				                                               xnSelect.SelectSingleNode("name").InnerText.Trim(),
				                                               xnSelect.SelectSingleNode("username").InnerText.Trim(), 
				                                               xnSelect.SelectSingleNode("password").InnerText.Trim(),
                                                               strMsgTableName,
                                                               strMsgIDFieldName,
                                                               strMsgValueFieldName,
                                                               boolUseDBSSL)) {
					
					            ltscCommands.Add(scNewCommander);

                                deNewCommander = new DatabaseExecutor(xnSelect.SelectSingleNode("server").InnerText.Trim(),
                                                                      xnSelect.SelectSingleNode("name").InnerText.Trim(),
                                                                      xnSelect.SelectSingleNode("username").InnerText.Trim(),
                                                                      xnSelect.SelectSingleNode("password").InnerText.Trim(),
                                                                      strDBParamChar,
                                                                      strMsgTableName,
                                                                      strMsgValueFieldName,
                                                                      strMsgValueDataType,
                                                                      boolDoDataComSetup);

                                boolDoDataComSetup = false;

                                if (boolCreateDataTable) {

                                    deNewCommander.Run(ServerApp.Settings.GenerateDefaultMsgTables(strMsgTableName,
                                                                                                   strMsgIDFieldName,
                                                                                                   strMsgValueFieldName,
                                                                                                   strMsgIDDataType,
                                                                                                   strMsgValueDataType));
                                }

                                try {

                                    if (xnSelect.SelectSingleNode("@designation").InnerText.Trim() != "") {

                                        dictdeDBDataStore.Add(xnSelect.SelectSingleNode("@designation").InnerText.Trim(), deNewCommander);
                                    }
                                }
                                catch { }
				            }
				            else {

                                ServerApp.Log("Action: Getting server commands. Error: Setting up to get server commands from database: '" + 
					                          xnSelect.SelectSingleNode("name").InnerText.Trim() + "' failed.");
				            } 
			            }

                        /* Get Default Commands */
                        xnlSettings = GetConfigSettingsList("//operations/commands/registerfile");

                        foreach (XmlNode xnSelect in xnlSettings) {

                            ltstrCmds.Add(strCmdMsgStartChars + "REGISTERFILE" + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("filedesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("filenamepath").InnerText.Trim() + strCmdMsgEndChars);
                        }

                        xnlSettings = GetConfigSettingsList("//operations/commands/registerdataquery");

                        foreach (XmlNode xnSelect in xnlSettings) {

                            strDatabaseDesign = xnSelect.SelectSingleNode("databasedesignation").InnerText.Trim();
                            strDataDesign = xnSelect.SelectSingleNode("datadesignation").InnerText.Trim();

                            ltstrCmds.Add(strCmdMsgStartChars + "REGISTERDATAQUERY" + strCmdMsgPartEndChars +
                                          strDatabaseDesign + strCmdMsgPartEndChars +
                                          strDataDesign + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("query").InnerText.Trim() + strCmdMsgEndChars);

                            foreach (XmlNode xnDataMap in xnSelect.SelectNodes("//datamap")) {

                                ltstrCmds.Add(strCmdMsgStartChars + "REGISTERDATAMAP" + strCmdMsgPartEndChars +
                                              strDatabaseDesign + strCmdMsgPartEndChars +
                                              strDataDesign + strCmdMsgPartEndChars +
                                              xnDataMap.SelectSingleNode("clientobjectdesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                              xnDataMap.SelectSingleNode("clientvarfuncname").InnerText.Trim() + strCmdMsgPartEndChars +
                                              xnDataMap.SelectSingleNode("isvariable").InnerText.Trim() + strCmdMsgPartEndChars +
                                              xnDataMap.SelectSingleNode("interval").InnerText.Trim() + strCmdMsgEndChars);
                            }
                        }

                        xnlSettings = GetConfigSettingsList("//operations/commands/registerdatastatement");

                        foreach (XmlNode xnSelect in xnlSettings) {

                            ltstrCmds.Add(strCmdMsgStartChars + "REGISTERDATASTATEMENT" + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("databasedesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("datadesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("statement").InnerText.Trim() + strCmdMsgEndChars);
                        }

                        xnlSettings = GetConfigSettingsList("//operations/commands/registerdataevent");

                        foreach (XmlNode xnSelect in xnlSettings) {

                            ltstrCmds.Add(strCmdMsgStartChars + "REGISTERDATAEVENT" + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("databasedesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("datadesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("statement").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("interval").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("intervaltype").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("delay").InnerText.Trim() + strCmdMsgEndChars);
                        }

                        xnlSettings = GetConfigSettingsList("//operations/commands/rundataoperation");

                        foreach (XmlNode xnSelect in xnlSettings) {

                            sbDefaultCmdMsg.Append(strCmdMsgStartChars + "RUNDATAOPERATION" + strCmdMsgPartEndChars +
                                                    xnSelect.SelectSingleNode("datadesignation").InnerText.Trim());

                            try {

                                foreach (XmlNode xnMsgPart in xnSelect.SelectNodes("/param")) {

                                    sbDefaultCmdMsg.Append(strCmdMsgPartEndChars + xnMsgPart.SelectSingleNode("/paramname").InnerText.Trim().ToLower());

                                    try {

                                        sbDefaultCmdMsg.Append(strCmdMsgPartEndChars + xnMsgPart.SelectSingleNode("/paramvalue").InnerText.Trim().ToLower());
                                    }
                                    catch {

                                        sbDefaultCmdMsg.Append(strCmdMsgPartEndChars);
                                    }
                                }
                            }
                            catch { }

                            sbDefaultCmdMsg.Append(strCmdMsgEndChars);

                            ltstrCmds.Add(sbDefaultCmdMsg.ToString());

                            sbDefaultCmdMsg.Clear();
                        }

                        xnlSettings = GetConfigSettingsList("//operations/commands/removedataevent");

                        foreach (XmlNode xnSelect in xnlSettings) {

                            ltstrCmds.Add(strCmdMsgStartChars + "REMOVEDATAOPERATION" + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("databasedesignation").InnerText.Trim() + strCmdMsgPartEndChars +
                                          xnSelect.SelectSingleNode("datadesignation").InnerText.Trim() + strCmdMsgEndChars);
                        }

                        boolNotServerCmdSetup = false;
                    }
                    catch (Exception exError) {

                        ServerApp.Log("Action: Getting server commands. Error: Setting up to get server commands failed.", exError);
                    }
                }			
									
				foreach (ServerCommands scSelect in ltscCommands) {

                    foreach (string strSelect in scSelect.GetCommands()) {

                        ltstrCmds.Add(strSelect);
                    }
				}
									
				return ltstrCmds;
			}
		}
		
		/// <summary>
		/// 	Transfer Client Message Out
		/// </summary>
		/// <param name="strMsg">Message to Transfer Out</param>
		public void TransferClientMsgOut(string strMsg) {

            if (boolNotTransferSetup) { 

                XmlNodeList xnlSettings;/* Selected XML Settings List */
                CommTrans ctNew;        /* New Communication Transaction for Outgoing Tranmissions */
            
			    /* Setup Outgoing Stream  */
			    xnlSettings = GetConfigSettingsList("//relay/stream");
			
			    foreach (XmlNode xnSelect in xnlSettings) {
				
				    bool.TryParse(xnSelect.SelectSingleNode("usessl").InnerText.Trim(), out boolOutUseSSL);
				
				    if (xnSelect.SelectSingleNode("hostname").InnerText.Trim() != "" && 
				        xnSelect.SelectSingleNode("port").InnerText.Trim() != "") {
					
					    ctNew = new CommTrans(nRandSeed++, xnSelect.SelectSingleNode("hostname").InnerText.Trim(),  
					                          int.Parse(xnSelect.SelectSingleNode("port").InnerText.Trim()), "STREAM", true);
					
					    if (boolOutUseSSL) {

                            ctNew.SSL = true;
					    }
					
					    ltctOut.Add(ctNew);
				    }
				    else {

                        ServerApp.Log("Action: Getting server settings. Error: Setting up outgoing stream, host: '" + 
					                  xnSelect.SelectSingleNode("hostname").InnerText.Trim() + "', port: " +
					                  xnSelect.SelectSingleNode("port").InnerText.Trim() + ", failed.");
				    }	
			    }
			
			    /* Setup Outgoing HTTP Transmission  */
			    xnlSettings = GetConfigSettingsList("//relay/http");
			
			    foreach (XmlNode xnSelect in xnlSettings) {
				
				    bool.TryParse(xnSelect.SelectSingleNode("usessl").InnerText.Trim(), out boolOutUseSSL);
				
				    if (xnSelect.SelectSingleNode("hostname").InnerText.Trim() != "" && 
				        xnSelect.SelectSingleNode("port").InnerText.Trim() != "") {
					
					    if (xnSelect.SelectSingleNode("method").InnerText.Trim().ToLower() != "get") {

                            ctNew = new CommTrans(nRandSeed++, xnSelect.SelectSingleNode("hostname").InnerText.Trim(),  
						                          int.Parse(xnSelect.SelectSingleNode("port").InnerText.Trim()), "HTTPPOST", true);
					    }
					    else {

                            ctNew = new CommTrans(nRandSeed++, xnSelect.SelectSingleNode("hostname").InnerText.Trim(),
                                                  int.Parse(xnSelect.SelectSingleNode("port").InnerText.Trim()), "HTTPGET", true);
					    }
					
					    if (xnSelect.SelectSingleNode("processingpage").InnerText.Trim() != "") {

                            ctNew.SetHTTPProcessPage(xnSelect.SelectSingleNode("processingpage").InnerText.Trim());
					    }
					
					    if (boolOutUseSSL) {
					
				            ctNew.SSL = true;
					    }
					
					    ltctOut.Add(ctNew);
				    }
				    else {

                        ServerApp.Log("Action: Getting server settings. Error: Setting up outgoing stream, host: '" + 
					                  xnSelect.SelectSingleNode("hostname").InnerText.Trim() + "', port: " +
					                  xnSelect.SelectSingleNode("port").InnerText.Trim() + ", failed.");
				    }
			    }
			
			    /* Setup Outgoing Database  */
			    xnlSettings = GetConfigSettingsList("//relay/database");
			
			    foreach (XmlNode xnSelect in xnlSettings) {
				
				    ltdoDBClientMsgStore.Add(new DatabaseOut(xnSelect.SelectSingleNode("server").InnerText.Trim(),
												             xnSelect.SelectSingleNode("name").InnerText.Trim(),
                                                             xnSelect.SelectSingleNode("username").InnerText.Trim(),
                                                             xnSelect.SelectSingleNode("password").InnerText.Trim(),
                                                             xnSelect.SelectSingleNode("table").InnerText.Trim(),
                                                             xnSelect.SelectSingleNode("msgfield").InnerText.Trim()));
			    }

                boolNotTransferSetup = false;
            }

			/* Send Messages to Each Transmission Points, 
 			   For HTTP Transmissions, Store as Data Before Sending, 
			   For Streams, Add the Message to be Automatically Sent,
			   For Files, Output the Messages to it 
			   For Databases, Output the Messages to it */
			foreach (CommTrans ctSelect in ltctOut) {

                ctSelect.AddHTTPMsgData("message", strMsg);

                ctSelect.SendHTTP();
                ctSelect.AddStreamMsg(strMsg);
			}
				
			foreach (FileOut foSelect in ltfoFileClientMsgStore) {
				
				foSelect.Output(strMsg);
			}
				
			foreach (DatabaseOut doSelect in ltdoDBClientMsgStore) {
				
				doSelect.Output(strMsg);
			}
		}

        /// <summary>
        ///     Adds File Information to List of Downloadable Files
        /// </summary>
        /// <param name="strFileDesign"> Designation of File That Can Be to be Downloaded</param>
        /// <param name="strFilePathName">Path and Name of File That Can be Downloaded</param>
        public void AddDownloadFile(string strFileDesign, string strFilePathName) {

            if (strFileStorePath != "") {
						
				if (!Directory.Exists(strFileStorePath)) {
						
					Directory.CreateDirectory(strFileStorePath);
				}
			}
        
            if (!dcDownloadFileList.ContainsKey(strFileDesign)) {

                dcDownloadFileList.Add(strFileDesign, strFilePathName);
            }
            else {

                dcDownloadFileList[strFileDesign] = strFilePathName;
            }
        }

        /// <summary>
        ///     Get List of Designations for Downloadable Files
        /// </summary>
        /// <returns>List of Designations for Downloadable Files</returns>
        public List<string> GetDownloadFileListDesigns() {
        
            return new List<string>(dcDownloadFileList.Keys);
        }

        /// <summary>
        ///     Gets Name and Path of a Selected Downloadable File
        /// </summary>
        /// <param name="strFileDesign">Designation of Selected File to Get Name and Path for</param>
        /// <returns>Name and Path of the Selected File, or Blank String If it is Not Found</returns>
        public string GetDownloadFilePathName(string strFileDesign) {
        
            string strFilePathName = "";
                                    /* Path and Name of Selected File */

            if (dcDownloadFileList.ContainsKey(strFileDesign)) {

                strFilePathName = dcDownloadFileList[strFileDesign];
            }

            return strFilePathName;
        }

        /// <summary>
        ///     Sets Up Secure UDP Sockets
        /// </summary>
        /// <param name="socMain">Main Communications Socket to Secure</param>
        /// <returns>Secure Socket Object</returns>
        public object UDPSecureSetup(Socket socMain) {

//            Type typFuncCall = ambOpenSSLDLL.GetType();
                                    /* Function Container for OpenSLL DLL */
            object objsslConn = null;
                                    /* Secure Socket Connection */    
                                    
            try {
                
                System.Type typFuncCall = ambOpenSSLDLL.GetType();
                objsslConn = typFuncCall.GetMethod("SSL_new").Invoke(null, new object[] { objsctxAccessor });

                if (objsslConn != null) {

                    if (Int32.Parse(typFuncCall.GetMethod("SSL_set_fd").
                                    Invoke(null, new object[] { objsslConn, socMain }).ToString()) == 0) {
                    
                        if (Int32.Parse(typFuncCall.GetMethod("SSL_connect").
                                        Invoke(null, new object[] { objsslConn }).ToString()) < 0) {

                            ServerApp.Log("Action: Setting up secure connection for UDP client communications. Error: Setting up BIO association failed.");
                        }
                    }
                    else {

                        ServerApp.Log("Action: Setting up secure connection for UDP client communications. Error: Setting up socket association failed.");
                    }
                }
                else {
                
                    ServerApp.Log("Action: Setting up secure connection for UDP client communications. Error: Setting up socket failed.");
                }
            }
            catch (Exception exError) {
                
                ServerApp.Log("Action: Setting up secure connection for for UDP client communications.", exError);
            }

            return objsslConn;
        }

        /// <summary>
        ///     Close Secure Socket for UDP
        /// </summary>
        /// <param name="objsslConn">Secure Socket Object for UDP to Close</param>
        public void UDPSecureClose(object objsslConn) {
            
//            Type typFuncCall = ambOpenSSLDLL.GetType();
                                    /* Function Container for OpenSLL DLL */
            try { 

                if (objsslConn != null) {

                    System.Type typFuncCall = ambOpenSSLDLL.GetType();

                    if (Int32.Parse(typFuncCall.GetMethod("SSL_shutdown").
                                    Invoke(null, new object[] { objsslConn }).ToString()) > 0) {

                        typFuncCall.GetMethod("SSL_free").Invoke(null, new object[] { objsslConn });
                    }
                    else {

                        ServerApp.Log("Action: Closing secure connection for UDP client communications. Error: Shutting down secure socket failed.");
                    }
                }
            }
            catch (Exception exError) {
                
                ServerApp.Log("Action: Closing secure connection for UDP client communications.", exError);
            }
        }

        /// <summary>
        ///     Frees Resources for Secure UDP Communications
        /// </summary>
        public void UDPSecureFree() {

            try { 

                if (objsctxAccessor != null) {

                    ambOpenSSLDLL.GetType().GetMethod("SSL_CTX_free").Invoke(null, new object[] { objsctxAccessor });
                }
            }
            catch (Exception exError) {
                
                ServerApp.Log("Action: Freeing resources for secure connection for UDP client communications.", exError);
            }
        }

        /// <summary>
        ///     Add Data Query for Data Execution Process
        /// </summary>
        /// <param name="strDatabaseExecutorDesign">Designation of Database for Execution</param>
        /// <param name="strDataDesign">Designation of Database Query</param>
        /// <param name="strQuery">Database Query</param>
        public void AddDataExecutionQuery(string strDatabaseExecutorDesign,
                                          string strDataDesign,
                                          string strQuery) {


            if (FindDataDesignationOwner(strDataDesign) == null) {

                dictdeDBDataStore[strDatabaseExecutorDesign].RegisterQuery(strDataDesign, strQuery);
            }
            else {
                
                ServerApp.Log("Action: Registering data query with server settings. Error: Designation: '" + strDataDesign + "' already exists.");
            }
        }

        /// <summary>
        ///     Add Data Statement for Data Execution Process
        /// </summary>
        /// <param name="strDatabaseExecutorDesign">Designation of Database for Execution</param>
        /// <param name="strDataDesign">Designation of Database Statement</param>
        /// <param name="strStatement">Database Statement</param>
        public void AddDataExecutionStatement(string strDatabaseExecutorDesign,
                                              string strDataDesign, 
                                              string strStatement) {

            if (FindDataDesignationOwner(strDataDesign) == null) {

                dictdeDBDataStore[strDatabaseExecutorDesign].RegisterStatement(strDataDesign, strStatement);
            }
            else {
                
                ServerApp.Log("Action: Registering data statement with server settings. Error: Designation: '" + strDataDesign + "' already exists.");
            }
        }

        /// <summary>
        ///     Add Data Event for Data Execution Process
        /// </summary>
        /// <param name="strDatabaseExecutorDesign">Designation of Database for Execution</param>
        /// <param name="strDataDesign">Designation of Database Event</param>
        /// <param name="strStatement">Database Event Statement</param>
        /// <param name="nInterval">Amount of Time for Event Execution</param>
        /// <param name="strIntervalType">Type Time Interval for Event Execution</param>
        /// <param name="nDelayInMilliSecs">Event Delay In Milliseconds</param>
        public void AddDataExecutionEvent(string strDatabaseExecutorDesign,
                                          string strDataDesign, 
                                          string strStatement,
                                          int nInterval,
                                          string strIntervalType,
                                          int nDelayInMilliSecs) {

            if (FindDataDesignationOwner(strDataDesign) == null) {

                dictdeDBDataStore[strDatabaseExecutorDesign].RegisterEvent(strDataDesign, strStatement, nInterval, strIntervalType, nDelayInMilliSecs);
            }
            else {
                
                ServerApp.Log("Action: Registering data event with server settings. Error: Designation: '" + strDataDesign + "' already exists.");
            }
        }

        /// <summary>
        ///     Add Data Query for Data Execution Process
        /// </summary>
        /// <param name="strDatabaseExecutorDesign">Designation of Database for Execution</param>
        /// <param name="strQueryDataDesign">Designation of Database Query</param>
        /// <param name="strObjectDesign">Client Object Designation to Receive Data</param>
        /// <param name="strVarFuncName">Name of Client Object Variable or Function to Receive Data</param>
        /// <param name="boolIsVariable">Identicator That Data is Being Sent to a Variable or Function, Default: True</param>
        /// <param name="nIntervalInMillis">Number of Milliseconds in Interval to Run Query</param>
        public void AddDataMap(string strDatabaseExecutorDesign,
                               string strQueryDataDesign,
                               string strObjectDesign,
                               string strVarFuncName,
                               bool boolIsVariable = true,
                               int nIntervalInMillis = 500) {

            dictdeDBDataStore[strDatabaseExecutorDesign].AddDataMap(strQueryDataDesign, strObjectDesign, strVarFuncName, boolIsVariable, nIntervalInMillis);
        }

        /// <summary>
        ///     Runnings Statement from Data Execution Process
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Statement to Run</param>
        /// <param name="dictParamList">List of Parameter Values for Database Statement to Run</param>
        public void RunDataExecution(string strDataDesign, Dictionary<string, string> dictParamList = null) {

            DatabaseExecutor deRunner = FindDataDesignationOwner(strDataDesign);
                                    /* Designated Database for Running */
            int nClientID = new Random().Next(),
                                    /* Client ID */
                nTransID = new Random().Next();
									/* Transaction ID */

            if (deRunner != null) {

                while (deRunner.TransactionExists(nClientID, nTransID)) {

                    nClientID = new Random().Next();
                    nTransID = new Random().Next();
                }

                if (dictParamList != null) { 

                    foreach (KeyValuePair<string, string> kvpSelect in dictParamList) {

                        deRunner.SetParameter(nClientID, nTransID, kvpSelect.Key, kvpSelect.Value);
                    }
                }

                if (deRunner.Execute(nClientID, nTransID, 1, strDataDesign, false, false)) {

                    ServerApp.Log("Action: Running statement from database process with server settings. Designation: '" + strDataDesign + "'.");
                }
                else {

                    ServerApp.Log("Action: Running statement from database process with server settings. Error: Designation: '" + strDataDesign + "' execution failed.");
                }
            }
            else {
                
                ServerApp.Log("Action: Running statement from database process with server settings. Error: Designation: '" + strDataDesign + "' doesn't exist.");
            }
        }

        /// <summary>
        ///     Remove Statement from Data Execution Process
        /// </summary>
        /// <param name="strDatabaseExecutorDesign">Designation of Database for Execution</param>
        /// <param name="strDataDesign">Designation of Database Event</param>
        public void RemoveDataExecution(string strDatabaseExecutorDesign, 
                                        string strDataDesign) {

            if (FindDataDesignationOwner(strDataDesign) != null) {

                dictdeDBDataStore[strDatabaseExecutorDesign].RemoveProcess(strDataDesign);
            }
            else {
                
                ServerApp.Log("Action: Removing statement from database process with server settings. Error: Designation: '" + strDataDesign + "' doesn't exist.");
            }
        }

        /// <summary>
        ///     Finds Owning Database Executor for Designated Database Query or Statement
        /// </summary>
        /// <param name="strDataDesign">Designation of Database Query or Statement</param>
        /// <returns>Database Executor Object That Owns the Designated Database Query or Statement, Else NULL If Not Found</returns>
        public DatabaseExecutor FindDataDesignationOwner(string strDataDesign) {

            DatabaseExecutor deOwner = null;
                                    /* Owning Database Executor Object */

            foreach (DatabaseExecutor deSelect in dictdeDBDataStore.Values) {

                if (deSelect.IsRegistered(strDataDesign)) {

                    deOwner = deSelect;
                    break;
                }
            }

            return deOwner;
        }

        /// <summary>
        ///     Finds Owning Database Executor for Transaction Being Processed
        /// </summary>
        /// <param name="nClientID">Client ID</param>
        /// <param name="nTransID">Transaction ID</param>
        /// <returns>Database Executor Object That Owns the Transaction Being Processed, Else NULL If Not Found</returns>
        public DatabaseExecutor FindDataTransactionOwner(int nClientID, int nTransID) {

            DatabaseExecutor deOwner = null;
                                    /* Owning Database Executor Object */

            foreach (DatabaseExecutor deSelect in dictdeDBDataStore.Values) {

                if (deSelect.TransactionExists(nClientID, nTransID)) {

                    deOwner = deSelect;
                    break;
                }
            }

            return deOwner;
        }
        
        /// <summary>
        ///     Pauses Data Map Processing Within Database Executors
        /// </summary>
        public void PauseAllDataMaps() {
            
            foreach (DatabaseExecutor deSelect in dictdeDBDataStore.Values) {

                if (deSelect.HasDataMaps) {

                    deSelect.StopDataMaps();
                }
            }
        }
        
        /// <summary>
        ///     Finds List of Database Executors That Have Data Maps
        /// </summary>
        /// <returns>List of Database Executors That Have Data Maps</returns>
        public List<DatabaseExecutor> GetDataTransactionWithDataMaps() {

            List<DatabaseExecutor> ltdeDataMaps = new List<DatabaseExecutor>();
                                    /* List of Data Executors with Data Maps */

            foreach (DatabaseExecutor deSelect in dictdeDBDataStore.Values) {

                if (deSelect.HasDataMaps) {

                    ltdeDataMaps.Add(deSelect);
                }
            }

            return ltdeDataMaps;
        }

        /// <summary>
        ///     Generates Default Database Process Procedures for Server Operations
        /// </summary>
        /// <param name="strMsgTableName">Name of Table for Outputting Server Command Messages to</param>
        /// <param name="strMsgIDFieldName">Name of Table's ID Field for Finding Server Command Messages to</param>
        /// <param name="strMsgValueFieldName">Name of Table's Value Field for Storing Server Command Messages to</param>
        /// <param name="strMsgIDDataType">Data Type of Table ID Field on Database</param>
        /// <param name="strMsgValueDataType">Data Type of Table Field on Database from Which to Get Commands From</param>
        /// <returns>Text for Possibly Deleting and Creating Database Table for Push Server Command Messages</returns>
        public string GenerateDefaultMsgTables(string strMsgTableName, 
                                               string strMsgIDFieldName, 
                                               string strMsgValueFieldName,
                                               string strMsgIDDataType,
                                               string strMsgValueDataType) {

            StringBuilder sbTblSetup = new StringBuilder();
            /* Holder for Database Setup Script */

            sbTblSetup.Append("DROP TABLE IF EXISTS " + strMsgTableName + " " + DBProcDelimiter + " ");
            sbTblSetup.Append("CREATE TABLE IF NOT EXISTS " + strMsgTableName + "(");
            sbTblSetup.Append("    " + strMsgIDFieldName + " " + strMsgIDDataType + " UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbTblSetup.Append("    " + strMsgValueFieldName + " " + strMsgValueDataType + " NOT NULL ");
            sbTblSetup.Append(") " + DBProcDelimiter);

            return sbTblSetup.ToString();
        }

        /// <summary>
        ///     Generates Default Database Process Procedures for Server Operations
        /// </summary>
        /// <param name="strMsgTableName">Name of Table for Outputting Server Command Messages to</param>
        /// <param name="strMsgFieldName">Name of Table's Field for Storing Server Command Messages to</param>
        /// <param name="strMsgFieldDataType">Date Type of Table's Field for Storing Server Command Messages to</param>
        /// <returns>Text for Deleting and Creating Database Procedure to Generate and Push Server Command Messages</returns>
        public string GenerateDefaultDataProcs(string strMsgTableName, string strMsgFieldName, string strMsgFieldDataType) {

            string strDelimiter = DBProcDelimiter;
                                    /* Database Replacement Script Deimiter for Procedures */
            StringBuilder sbDbSetup = new StringBuilder();
                                    /* Holder for Database Setup Script */

            sbDbSetup.Append("CREATE TABLE revcomm_msg_groups (");
            sbDbSetup.Append("    revcomm_msg_group_id INT(10) UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbDbSetup.Append("    owner VARCHAR(25) NOT NULL, ");
            sbDbSetup.Append("    client_id INT(12) UNSIGNED NULL, ");
            sbDbSetup.Append("    trans_id INT(10) UNSIGNED NULL, ");
            sbDbSetup.Append("    resp_id INT(10) UNSIGNED NULL, ");
            sbDbSetup.Append("    date_started DATETIME DEFAULT CURRENT_TIMESTAMP, ");
            sbDbSetup.Append("    UNIQUE uk_revcomm_msg_groups_data_process (client_id, trans_id, resp_id) ");
            sbDbSetup.Append(") " + strDelimiter + " "); 
            sbDbSetup.Append("CREATE TABLE revcomm_msgs (");
            sbDbSetup.Append("    revcomm_msg_id INT(10) UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbDbSetup.Append("    revcomm_msg_group_id INT(10) UNSIGNED NOT NULL, ");
            sbDbSetup.Append("    designation TEXT NOT NULL, ");
            sbDbSetup.Append("    open BOOL NOT NULL DEFAULT TRUE, ");
            sbDbSetup.Append("    CONSTRAINT fk_revcomm_msgs_revcomm_msg_group_id FOREIGN KEY(revcomm_msg_group_id) REFERENCES revcomm_msg_groups (revcomm_msg_group_id) ");
            sbDbSetup.Append(") " + strDelimiter + " ");
            sbDbSetup.Append("CREATE TABLE revcomm_msg_varupdates (");
            sbDbSetup.Append("    revcomm_msg_varupdate_id INT(10) UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbDbSetup.Append("    revcomm_msg_id INT(10) UNSIGNED NOT NULL, ");
            sbDbSetup.Append("    name TEXT NOT NULL, ");
            sbDbSetup.Append("    value " + strMsgFieldDataType + " NOT NULL, ");
            sbDbSetup.Append("    is_not_json BOOL NOT NULL DEFAULT TRUE, ");
            sbDbSetup.Append("    CONSTRAINT fk_revcomm_msg_varupdates_revcomm_msg_id FOREIGN KEY(revcomm_msg_id) REFERENCES revcomm_msgs (revcomm_msg_id) ");
            sbDbSetup.Append(") " + strDelimiter + " ");
            sbDbSetup.Append("CREATE TABLE revcomm_msg_funccalls (");
            sbDbSetup.Append("    revcomm_msg_funccall_id INT(10) UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbDbSetup.Append("    revcomm_msg_id INT(10) UNSIGNED NOT NULL, ");
            sbDbSetup.Append("    func_name TEXT NOT NULL, ");
            sbDbSetup.Append("    CONSTRAINT fk_revcomm_msg_funccalls_revcomm_msg_id FOREIGN KEY(revcomm_msg_id) REFERENCES revcomm_msgs (revcomm_msg_id) ");
            sbDbSetup.Append(") " + strDelimiter + " ");
            sbDbSetup.Append("CREATE TABLE revcomm_msg_funccall_params (");
            sbDbSetup.Append("    revcomm_msg_funccall_param_id INT(10) UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbDbSetup.Append("    revcomm_msg_funccall_id INT(10) UNSIGNED NOT NULL, ");
            sbDbSetup.Append("    param_value " + strMsgFieldDataType + " NOT NULL, ");
            sbDbSetup.Append("    param_order SMALLINT UNSIGNED NOT NULL DEFAULT 1, ");
            sbDbSetup.Append("    is_not_json BOOL NOT NULL DEFAULT TRUE, ");
            sbDbSetup.Append("    CONSTRAINT fk_revcomm_msg_funccall_params_revcomm_msg_funccall_id FOREIGN KEY(revcomm_msg_funccall_id) REFERENCES revcomm_msg_funccalls (revcomm_msg_funccall_id) ");
            sbDbSetup.Append(") " + strDelimiter + " "); 
            sbDbSetup.Append("CREATE TABLE revcomm_data_results (");
            sbDbSetup.Append("    revcomm_data_result_id INT(10) UNSIGNED PRIMARY KEY AUTO_INCREMENT, ");
            sbDbSetup.Append("    client_id INT(12) UNSIGNED NULL, ");
            sbDbSetup.Append("    trans_id INT(10) UNSIGNED NOT NULL, ");
            sbDbSetup.Append("    resp_id INT(10) UNSIGNED NOT NULL, ");
            sbDbSetup.Append("    result " + strMsgFieldDataType + " NOT NULL, ");
            sbDbSetup.Append("    UNIQUE uk_revcomm_data_results_data_process (client_id, trans_id, resp_id) ");
            sbDbSetup.Append(") " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgGroupCreate(strOwner VARCHAR(25), ");
            sbDbSetup.Append("                                            OUT nMsgGroupID INT(10)) ");
            sbDbSetup.Append("BEGIN ");            
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE nAppTransCount SMALLINT DEFAULT 0; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SELECT COUNT(*) ");
            sbDbSetup.Append("   FROM revcomm_msg_groups rmg ");
            sbDbSetup.Append("   WHERE rmg.client_id = 0 ");
            sbDbSetup.Append("     AND rmg.trans_id = 0 ");
            sbDbSetup.Append("   INTO nAppTransCount; ");
            sbDbSetup.Append(" ");  
            sbDbSetup.Append("   CALL RevComm_JSONMsgGroupDataResultCreate(strOwner, 0, 0, nAppTransCount, nMsgGroupID); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgGroupDataResultCreate(strOwner VARCHAR(25), ");
            sbDbSetup.Append("                                                      nClientID INT(12), ");
            sbDbSetup.Append("                                                      nTransID INT(10), ");
            sbDbSetup.Append("                                                      nRespID INT(10), ");
            sbDbSetup.Append("                                                      OUT nMsgGroupID INT(10)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   DECLARE nTransRespCount SMALLINT DEFAULT 0; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF nTransID IS NOT NULL AND nRespID IS NOT NULL ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      SELECT COUNT(*) ");
            sbDbSetup.Append("      FROM revcomm_msg_groups rmg INNER JOIN revcomm_msgs rm ON rm.revcomm_msg_group_id = rmg.revcomm_msg_group_id ");
            sbDbSetup.Append("                                                            AND rm.open = FALSE ");
            sbDbSetup.Append("      WHERE rmg.client_id = nClientID ");
            sbDbSetup.Append("        AND rmg.trans_id = nTransID ");
            sbDbSetup.Append("        AND rmg.resp_id = nRespID ");
            sbDbSetup.Append("      INTO nTransRespCount; ");
            sbDbSetup.Append(" ");                   
            sbDbSetup.Append("      IF nTransRespCount <= 0 ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("        INSERT INTO revcomm_msg_groups (owner, client_id, trans_id, resp_id) VALUES (strOwner, nClientID, nTransID, nRespID); ");
            sbDbSetup.Append("        SET nMsgGroupID = LAST_INSERT_ID(); ");
            sbDbSetup.Append("      ELSE ");
            sbDbSetup.Append("        SET nMsgGroupID = NULL; ");
            sbDbSetup.Append("        CALL RevComm_JSONMsgError(CONCAT('Action: Creating message group. Error: A message group already exists for client_id: ', nClientID, ', trans_id: ', nTransID, ', resp_id: ', nRespID)); ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgCreate(nMsgGroupID INT(10), ");
            sbDbSetup.Append("                                       strDesignation TEXT, ");
            sbDbSetup.Append("                                       OUT nMsgID INT(10)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO revcomm_msgs (revcomm_msg_group_id, designation) VALUES (nMsgGroupID, strDesignation); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SET nMsgID = LAST_INSERT_ID(); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgJSVarUpdate(nSetRevCommMsgID INT(10), ");
            sbDbSetup.Append("                                            strName TEXT, ");
            sbDbSetup.Append("                                            strSetValue " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                            boolSetIsNotJSON BOOL) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE nRevCommMsgID INT(10) DEFAULT 0; ");
            sbDbSetup.Append("   DECLARE strValue " + strMsgFieldDataType + "; ");
            sbDbSetup.Append("   DECLARE boolIsNotJSON BOOL DEFAULT TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SELECT revcomm_msg_id ");
            sbDbSetup.Append("   FROM revcomm_msgs ");
            sbDbSetup.Append("   WHERE revcomm_msg_id = nSetRevCommMsgID ");
            sbDbSetup.Append("     AND open = TRUE ");
            sbDbSetup.Append("   INTO nRevCommMsgID; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF nRevCommMsgID > 0 ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append(" "); 
            sbDbSetup.Append("      IF strSetValue IS NOT NULL ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append("         SET strValue = strSetValue; ");
            sbDbSetup.Append("      ELSE ");
            sbDbSetup.Append("         SET strValue = ''; ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      IF boolSetIsNotJSON IS NOT NULL ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append("         SET boolIsNotJSON = boolSetIsNotJSON; ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      INSERT INTO revcomm_msg_varupdate (revcomm_msg_id, name, value, is_not_json) ");
            sbDbSetup.Append("      VALUES (nRevCommMsgID, strName, strValue, boolIsNotJSON); ");
            sbDbSetup.Append("   ELSE ");
            sbDbSetup.Append("      CALL RevComm_JSONMsgError(CONCAT('Action: Adding variable update to message. Error: There is no open message for revcomm_msg_id: ', nSetRevCommMsgID)); ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgJSFuncCall(nSetRevCommMsgID INT(10), ");
            sbDbSetup.Append("                                           strFuncName TEXT, ");
            sbDbSetup.Append("                                           strFirstParamValue " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                           boolSetParamIsNotJSON BOOL, ");
            sbDbSetup.Append("                                           OUT nRevCommMsgFuncCallID INT(10)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE nRevCommMsgID INT(10) DEFAULT 0; ");
            sbDbSetup.Append("   DECLARE boolParamIsNotJSON BOOL DEFAULT TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SELECT revcomm_msg_id ");
            sbDbSetup.Append("   FROM revcomm_msgs ");
            sbDbSetup.Append("   WHERE revcomm_msg_id = nSetRevCommMsgID ");
            sbDbSetup.Append("     AND open = TRUE ");
            sbDbSetup.Append("   INTO nRevCommMsgID; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF nRevCommMsgID > 0 ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      IF boolSetParamIsNotJSON IS NOT NULL ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append("         SET boolParamIsNotJSON = boolSetParamIsNotJSON; ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      INSERT INTO revcomm_msg_funccalls (revcomm_msg_id, func_name) ");
            sbDbSetup.Append("      VALUES (nRevCommMsgID, strFuncName); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      SET nRevCommMsgFuncCallID = LAST_INSERT_ID(); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      IF strFirstParamValue IS NOT NULL ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         Call RevComm_JSONMsgJSFuncAddParam(nRevCommMsgFuncCallID, strFirstParamValue, 0, boolParamIsNotJSON); ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append("   ELSE ");
            sbDbSetup.Append("      SET nRevCommMsgFuncCallID = NULL; ");
            sbDbSetup.Append("      CALL RevComm_JSONMsgError(CONCAT('Action: Adding function call to message. Error: There is no open message for revcomm_msg_id: ', nSetRevCommMsgID)); ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgJSFuncAddParam(nRevCommMsgFuncCallID INT(10), ");
            sbDbSetup.Append("                                               strParamSetValue " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                               nParamSetIndex SMALLINT, ");
            sbDbSetup.Append("                                               boolSetParamIsNotJSON BOOL) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE strParamValue " + strMsgFieldDataType + "; ");
            sbDbSetup.Append("   DECLARE nParamIndex SMALLINT; ");
            sbDbSetup.Append("   DECLARE boolParamIsNotJSON BOOL DEFAULT TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF strParamSetValue IS NOT NULL ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append("      SET strParamValue = strParamSetValue; ");
            sbDbSetup.Append("   ELSE ");
            sbDbSetup.Append("      SET strParamValue = ''; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF nParamSetIndex IS NOT NULL ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      SET nParamIndex = nParamSetIndex; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   ELSE ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      SELECT MAX(param_order) ");
            sbDbSetup.Append("      FROM revcomm_msg_funccall_params ");
            sbDbSetup.Append("      WHERE revcomm_msg_funccall_id = nRevCommMsgFuncCallID ");
            sbDbSetup.Append("      INTO nParamIndex; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      IF nParamIndex > 0 ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append("         SET nParamIndex = nParamIndex - 1; ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF boolSetParamIsNotJSON IS NOT NULL ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append("      SET boolParamIsNotJSON = boolSetParamIsNotJSON; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   UPDATE revcomm_msg_funccall_params ");
            sbDbSetup.Append("   SET param_order = param_order + 1 ");
            sbDbSetup.Append("   WHERE revcomm_msg_funccall_id = nRevCommMsgFuncCallID ");
            sbDbSetup.Append("     AND param_order >= nParamIndex + 1; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   INSERT INTO revcomm_msg_funccall_params (revcomm_msg_funccall_id, param_value, param_order, is_not_json) ");
            sbDbSetup.Append("   VALUES (nRevCommMsgFuncCallID, strParamValue, nParamIndex + 1, boolParamIsNotJSON); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgSaveClose(nRevCommMsgID INT(10)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");    
            sbDbSetup.Append("  IF RevComm_JSONMsgSaveCheck(nRevCommMsgID) ");
            sbDbSetup.Append("  THEN ");                  
            sbDbSetup.Append(" ");        
            sbDbSetup.Append("     UPDATE revcomm_msgs ");
            sbDbSetup.Append("     SET open = FALSE ");
            sbDbSetup.Append("     WHERE revcomm_msg_id = nRevCommMsgID; ");
            sbDbSetup.Append(" ");              
            sbDbSetup.Append("  ELSE ");
            sbDbSetup.Append("     CALL RevComm_JSONMsgError(CONCAT('Action: Saving and closing a message. Error: Message for revcomm_msg_id: ', nRevCommMsgID, ' is in a group with a closed message waiting for output or invalid.')); ");    
            sbDbSetup.Append("  END IF; ");    
            sbDbSetup.Append(" ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE FUNCTION RevComm_JSONMsgSaveCheck(nRevCommMsgID INT(10)) RETURNS BOOL ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE nOpenMsgCount SMALLINT DEFAULT 0; ");
            sbDbSetup.Append("   DECLARE boolCheckSuccess BOOL DEFAULT FALSE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SELECT COUNT(*) ");
            sbDbSetup.Append("   FROM revcomm_msg_groups rmg INNER JOIN revcomm_msgs rm ON rm.revcomm_msg_group_id = rmg.revcomm_msg_group_id ");
            sbDbSetup.Append("                                                         AND rm.revcomm_msg_id = nRevCommMsgID ");
            sbDbSetup.Append("                                                         AND rm.open = FALSE ");
            sbDbSetup.Append("   INTO nOpenMsgCount; ");
            sbDbSetup.Append(" ");                   
            sbDbSetup.Append("   IF nOpenMsgCount <= 0 ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("     SET boolCheckSuccess = TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   RETURN boolCheckSuccess; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgsOutput(nMsgGroupID INT(10), ");
            sbDbSetup.Append("                                        OUT strMsg " + strMsgFieldDataType + ") ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   DECLARE nRevCommMsgID INT(10) DEFAULT 0; ");
            sbDbSetup.Append("   DECLARE strDesign TEXT; ");
            sbDbSetup.Append("   DECLARE strName TEXT; ");
            sbDbSetup.Append("   DECLARE strValue " + strMsgFieldDataType + "; ");
            sbDbSetup.Append("   DECLARE boolNotJSON BOOL DEFAULT TRUE; ");
            sbDbSetup.Append("   DECLARE nRevCommMsgFuncID INT(10) DEFAULT 0; ");
            sbDbSetup.Append("   DECLARE nClientID INT(12) DEFAULT NULL; ");
            sbDbSetup.Append("   DECLARE nTransID INT(10) DEFAULT NULL; ");
            sbDbSetup.Append("   DECLARE nRespID INT(10) DEFAULT NULL; ");
            sbDbSetup.Append("   DECLARE boolNotCompleted BOOL DEFAULT TRUE; ");
            sbDbSetup.Append("   DECLARE boolNotFirstMsg BOOL DEFAULT FALSE; ");
            sbDbSetup.Append("   DECLARE boolNotFirstVar BOOL DEFAULT FALSE; ");
            sbDbSetup.Append("   DECLARE boolNotFirstFunc BOOL DEFAULT FALSE; ");
            sbDbSetup.Append("   DECLARE boolNotFirstParam BOOL DEFAULT FALSE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE curMsgs CURSOR ");
            sbDbSetup.Append("   FOR SELECT revcomm_msg_id, ");
            sbDbSetup.Append("              designation ");
            sbDbSetup.Append("       FROM revcomm_msgs ");
            sbDbSetup.Append("       WHERE revcomm_msg_group_id = nMsgGroupID ");
            sbDbSetup.Append("         AND open = FALSE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DECLARE CONTINUE HANDLER FOR NOT FOUND SET boolNotCompleted = FALSE; ");
            sbDbSetup.Append("   DECLARE EXIT HANDLER FOR SQLEXCEPTION "); 
            sbDbSetup.Append("   BEGIN ");
            sbDbSetup.Append("        DECLARE nErrorNum INT; ");
            sbDbSetup.Append("        DECLARE txErrorMsg TEXT; ");
            sbDbSetup.Append("   ");
            sbDbSetup.Append("        GET DIAGNOSTICS CONDITION 1 nErrorNum = MYSQL_ERRNO, txErrorMsg = MESSAGE_TEXT; ");
            sbDbSetup.Append("        CALL RevComm_JSONMsgError(CONCAT('Action: Outputting JSON message. Exception: ', nErrorNum, ' - ', txErrorMsg)); ");
            sbDbSetup.Append("   END; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SET strMsg = '[';");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   OPEN curMsgs; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   LOOPMSGS: LOOP ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      FETCH curMsgs INTO nRevCommMsgID, ");
            sbDbSetup.Append("                         strDesign; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      IF boolNotCompleted ");
            sbDbSetup.Append("      THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         IF boolNotFirstMsg ");
            sbDbSetup.Append("         THEN ");
            sbDbSetup.Append("            SET strMsg = RevComm_OutputLimitCheck(strMsg, ','); ");
            sbDbSetup.Append("         END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         SET strMsg = RevComm_OutputLimitCheck(strMsg, CONCAT('{\"DESIGNATION\": \"', strDesign, '\", \"VARUPDATES\": [')); ");
            sbDbSetup.Append("         SET boolNotFirstMsg = TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         BEGIN ");
            sbDbSetup.Append("            DECLARE curMsgVars CURSOR ");
            sbDbSetup.Append("            FOR SELECT name, ");
            sbDbSetup.Append("                       value, ");
            sbDbSetup.Append("                       is_not_json ");
            sbDbSetup.Append("                FROM revcomm_msg_varupdates ");
            sbDbSetup.Append("                WHERE revcomm_msg_id = nRevCommMsgID; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            OPEN curMsgVars; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            LOOPMSGVARS: LOOP ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                FETCH curMsgVars INTO strName, ");
            sbDbSetup.Append("                                      strValue, ");
            sbDbSetup.Append("                                      boolNotJSON; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                IF boolNotCompleted ");
            sbDbSetup.Append("                THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                    IF boolNotFirstVar ");
            sbDbSetup.Append("                    THEN ");
            sbDbSetup.Append("                        SET strMsg = RevComm_OutputLimitCheck(strMsg, ','); ");
            sbDbSetup.Append("                    END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                    IF REPLACE(strValue, '.', '') NOT REGEXP '^-?[0-9]+$' AND boolNotJSON ");
            sbDbSetup.Append("                    THEN ");
            sbDbSetup.Append("                        SET strMsg = RevComm_OutputLimitCheck(strMsg, CONCAT('{\"NAME\": \"', strName, '\", \"VALUE\": \"', strValue, '\"}')); ");
            sbDbSetup.Append("                    ELSE ");
            sbDbSetup.Append("                        SET strMsg = RevComm_OutputLimitCheck(strMsg, CONCAT('{\"NAME\": \"', strName, '\", \"VALUE\": ', strValue, '}')); ");
            sbDbSetup.Append("                    END IF; ");
            sbDbSetup.Append("                    SET boolNotFirstVar = TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                ELSE ");
            sbDbSetup.Append("                    SET boolNotCompleted = TRUE; ");
            sbDbSetup.Append("                    SET boolNotFirstVar = FALSE; ");
            sbDbSetup.Append("                    LEAVE LOOPMSGVARS; ");
            sbDbSetup.Append("                END IF; ");
            sbDbSetup.Append("            END LOOP; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            CLOSE curMsgVars; ");
            sbDbSetup.Append("         END; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         SET strMsg = RevComm_OutputLimitCheck(strMsg, CONCAT('], \"FUNCCALLS\": [')); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         BEGIN");
            sbDbSetup.Append("            DECLARE curMsgFuncs CURSOR ");
            sbDbSetup.Append("            FOR SELECT revcomm_msg_funccall_id, ");
            sbDbSetup.Append("                       func_name ");
            sbDbSetup.Append("                FROM revcomm_msg_funccalls ");
            sbDbSetup.Append("                WHERE revcomm_msg_id = nRevCommMsgID; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            OPEN curMsgFuncs; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            LOOPMSGFUNCS: LOOP ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                FETCH curMsgFuncs INTO nRevCommMsgFuncID, ");
            sbDbSetup.Append("                                       strName; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                IF boolNotCompleted ");
            sbDbSetup.Append("                THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                    IF boolNotFirstFunc ");
            sbDbSetup.Append("                    THEN ");
            sbDbSetup.Append("                        SET strMsg = RevComm_OutputLimitCheck(strMsg, ','); ");
            sbDbSetup.Append("                    END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                    SET strMsg = RevComm_OutputLimitCheck(strMsg, CONCAT('{\"NAME\": \"', strName, '\", \"PARAMS\": [')); ");
            sbDbSetup.Append("                    SET boolNotFirstFunc = TRUE; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                    BEGIN");
            sbDbSetup.Append("                        DECLARE curMsgParams CURSOR ");
            sbDbSetup.Append("                        FOR SELECT param_value, ");
            sbDbSetup.Append("                                   is_not_json ");
            sbDbSetup.Append("                            FROM revcomm_msg_funccall_params ");
            sbDbSetup.Append("                            WHERE revcomm_msg_funccall_id = nRevCommMsgFuncID ");
            sbDbSetup.Append("                            ORDER BY param_order ASC; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                        OPEN curMsgParams; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                        LOOPMSGPARAMS: LOOP ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                            FETCH curMsgParams INTO strValue, ");
            sbDbSetup.Append("                                                    boolNotJSON; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                            IF boolNotCompleted ");
            sbDbSetup.Append("                            THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                                IF boolNotFirstParam ");
            sbDbSetup.Append("                                THEN ");
            sbDbSetup.Append("                                    SET strMsg = RevComm_OutputLimitCheck(strMsg, ','); ");
            sbDbSetup.Append("                                END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                                IF REPLACE(strValue, '.', '') NOT REGEXP '^-?[0-9]+$' AND boolNotJSON ");
            sbDbSetup.Append("                                THEN ");
            sbDbSetup.Append("                                    SET strMsg = RevComm_OutputLimitCheck(strMsg, CONCAT('\"', strValue, '\"')); ");
            sbDbSetup.Append("                                ELSE ");
            sbDbSetup.Append("                                    SET strMsg = RevComm_OutputLimitCheck(strMsg, strValue); ");
            sbDbSetup.Append("                                END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                                SET boolNotFirstParam = TRUE; ");
            sbDbSetup.Append("                            ELSE ");
            sbDbSetup.Append("                                SET boolNotCompleted = TRUE; ");
            sbDbSetup.Append("                                SET boolNotFirstParam = FALSE; ");
            sbDbSetup.Append("                                LEAVE LOOPMSGPARAMS; ");
            sbDbSetup.Append("                            END IF; ");
            sbDbSetup.Append("                        END LOOP; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                        CLOSE curMsgParams; ");
            sbDbSetup.Append("                    END; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                    SET strMsg = RevComm_OutputLimitCheck(strMsg, ']}'); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("                ELSE ");
            sbDbSetup.Append("                    SET boolNotCompleted = TRUE; ");
            sbDbSetup.Append("                    SET boolNotFirstFunc = FALSE; ");
            sbDbSetup.Append("                    LEAVE LOOPMSGFUNCS; ");
            sbDbSetup.Append("                END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            END LOOP; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("            CLOSE curMsgFuncs; ");
            sbDbSetup.Append("         END;");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("         SET strMsg = RevComm_OutputLimitCheck(strMsg, ']}'); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      ELSE ");
            sbDbSetup.Append("        LEAVE LOOPMSGS; ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   END LOOP; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   CLOSE curMsgs; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SET strMsg = RevComm_OutputLimitCheck(strMsg, ']'); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   SELECT client_id, ");
            sbDbSetup.Append("          trans_id, ");
            sbDbSetup.Append("          resp_id ");
            sbDbSetup.Append("   FROM revcomm_msg_groups ");
            sbDbSetup.Append("   WHERE revcomm_msg_group_id = nMsgGroupID ");
            sbDbSetup.Append("   INTO nClientID, ");
            sbDbSetup.Append("        nTransID, ");
            sbDbSetup.Append("        nRespID; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF nClientID IS NOT NULL AND ");
            sbDbSetup.Append("      nTransID IS NOT NULL AND ");
            sbDbSetup.Append("      nRespID IS NOT NULL ");
            sbDbSetup.Append("   THEN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("      INSERT INTO revcomm_data_results (client_id, trans_id, resp_id, result) ");
            sbDbSetup.Append("      VALUES (nClientID, nTransID, nRespID, strMsg); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   Call RevComm_JSONMsgsClear(nMsgGroupID); ");
            sbDbSetup.Append("END " + strDelimiter);
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgError(strErrorMsg TEXT) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "LOG" + CmdMsgPartEndChars + "', strErrorMsg, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JSONMsgsClear(nMsgGroupID INT(10)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DELETE FROM revcomm_msg_funccall_params ");
            sbDbSetup.Append("   WHERE revcomm_msg_funccall_id IN (SELECT mfc.revcomm_msg_funccall_id ");
            sbDbSetup.Append("                                     FROM revcomm_msg_funccalls mfc INNER JOIN revcomm_msgs m ON m.revcomm_msg_group_id = nMsgGroupID ");
            sbDbSetup.Append("                                                                                             AND m.revcomm_msg_id = mfc.revcomm_msg_id); "); 
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DELETE FROM revcomm_msg_funccalls ");
            sbDbSetup.Append("   WHERE revcomm_msg_id IN (SELECT m.revcomm_msg_id ");
            sbDbSetup.Append("                            FROM revcomm_msgs m ");
            sbDbSetup.Append("                            WHERE m.revcomm_msg_group_id = nMsgGroupID); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DELETE FROM revcomm_msg_varupdates ");
            sbDbSetup.Append("   WHERE revcomm_msg_id IN (SELECT m.revcomm_msg_id ");
            sbDbSetup.Append("                            FROM revcomm_msgs m ");
            sbDbSetup.Append("                            WHERE m.revcomm_msg_group_id = nMsgGroupID); ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DELETE FROM revcomm_msgs ");
            sbDbSetup.Append("   WHERE revcomm_msg_group_id = nMsgGroupID; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   DELETE FROM revcomm_msg_groups ");
            sbDbSetup.Append("   WHERE revcomm_msg_group_id = nMsgGroupID; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE FUNCTION RevComm_OutputLimitCheck(strMsg " + strMsgFieldDataType + ", strAdded " + strMsgFieldDataType + ") RETURNS " + strMsgFieldDataType + " ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   DECLARE strResult " + strMsgFieldDataType + " DEFAULT strMsg; ");
            sbDbSetup.Append("   IF strMsg IS NOT NULL AND strAdded IS NOT NULL" );
            sbDbSetup.Append("   THEN");
            sbDbSetup.Append("      IF LENGTH(strMsg) + LENGTH(strAdded) < " + dictDBTypeMaxSize[strMsgFieldDataType] + " ");
            sbDbSetup.Append("      THEN");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("          SET strResult = CONCAT(strResult, strAdded); ");
            sbDbSetup.Append("      ELSE ");
            sbDbSetup.Append("        CALL RevComm_JSONMsgError('Action: Concatenating output string. Error: Values sizes hit max " + strMsgFieldDataType + " datatype size.'); ");
            sbDbSetup.Append("      END IF; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append("   RETURN strResult; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_SendMsgUser(nClientID INT(12), tMsgInfo " + strMsgFieldDataType + ") ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "SENDMSGUSER" + CmdMsgPartEndChars + "', nClientID, '" + CmdMsgPartEndChars + "', tMsgInfo, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_SendMsgUserByDesign(nClientID INT(12), tMsgInfo " + strMsgFieldDataType + ", strSetMsgDesign TEXT) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   DECLARE strMsgDesign TEXT DEFAULT ''; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF strSetMsgDesign IS NOT NULL ");
            sbDbSetup.Append("   THEN");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("       SET strMsgDesign = strSetMsgDesign; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "SENDMSGUSER" + CmdMsgPartEndChars + "', nClientID, '" + CmdMsgPartEndChars + "', tMsgInfo, '" + CmdMsgPartEndChars + "', strMsgDesign, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_StartGroup(nClientID INT(12), strGroupID VARCHAR(36)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF strGroupID IS NULL OR strGroupID = '' ");
            sbDbSetup.Append("   THEN");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("     INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("     VALUES (CONCAT('" + CmdMsgStartChars + "STARTGROUP" + CmdMsgPartEndChars + "', nClientID, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_JoinGroup(nClientID INT(12), strGroupID VARCHAR(36)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF strGroupID IS NOT NULL AND strGroupID != '' ");
            sbDbSetup.Append("   THEN");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("     INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("     VALUES (CONCAT('" + CmdMsgStartChars + "JOINGROUP" + CmdMsgPartEndChars + "', nClientID, '" + CmdMsgPartEndChars + "', strGroupID, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_LeaveGroup(nClientID INT(12)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "LEAVEGROUP" + CmdMsgPartEndChars + "', nClientID, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_CloseGroup(strGroupID VARCHAR(36)) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "CLOSEGROUP" + CmdMsgPartEndChars + "', strGroupID, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_RegisterFile(tFileDesign " + strMsgFieldDataType + ", tFilePath " + strMsgFieldDataType + ") ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "REGISTERFILE" + CmdMsgPartEndChars + "', tFileDesign, '" + CmdMsgPartEndChars + "', tFilePath, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_RegisterDataQuery(tDatabaseDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                           tDataDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                           tDataStatement " + strMsgFieldDataType + ") ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "REGISTERDATAQUERY" + CmdMsgPartEndChars + "', tDatabaseDesign, '" + CmdMsgPartEndChars + "', tDataDesign, '" + CmdMsgPartEndChars + "', tDataStatement, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_RegisterDataStatement(tDatabaseDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                               tDataDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                               tDataStatement " + strMsgFieldDataType + ") ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "REGISTERDATASTATEMENT" + CmdMsgPartEndChars + "', tDatabaseDesign, '" + CmdMsgPartEndChars + "', tDataDesign, '" + CmdMsgPartEndChars + "', tDataStatement, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_RegisterDataEvent(tDatabaseDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                           tEventDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                           tDataStatement " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                           nIntervalAmount SMALLINT, ");
            sbDbSetup.Append("                                           strIntervalType VARCHAR(25), ");
            sbDbSetup.Append("                                           nDelayInMillisecs SMALLINT) ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "REGISTERDATAEVENT" + CmdMsgPartEndChars + "', tDatabaseDesign, '" + CmdMsgPartEndChars + "', tEventDesign, '" + CmdMsgPartEndChars + "', tDataStatement, '" + CmdMsgPartEndChars + 
                                                "', nIntervalAmount, '" + CmdMsgPartEndChars + "', strIntervalType, '" + CmdMsgPartEndChars + "', nDelayInMillisecs, '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter + " ");
            sbDbSetup.Append("CREATE PROCEDURE RevComm_RunDataProcess(tDataDesign " + strMsgFieldDataType + ", ");
            sbDbSetup.Append("                                        tSetParamNameValueList " + strMsgFieldDataType + ") ");
            sbDbSetup.Append("BEGIN ");
            sbDbSetup.Append("   DECLARE tParamNameValueList " + strMsgFieldDataType + " DEFAULT ''; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   IF tSetParamNameValueList IS NOT NULL ");
            sbDbSetup.Append("   THEN");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("       SET tParamNameValueList = tSetParamNameValueList; ");
            sbDbSetup.Append("   END IF; ");
            sbDbSetup.Append(" ");
            sbDbSetup.Append("   INSERT INTO " + strMsgTableName + " (" + strMsgFieldName + ") ");
            sbDbSetup.Append("   VALUES (CONCAT('" + CmdMsgStartChars + "RUNDATAOPERATION" + CmdMsgPartEndChars + "', tDataDesign, '" + CmdMsgPartEndChars + "true" + CmdMsgPartEndChars + "', REPLACE(tParamNameValueList, ',', '" + CmdMsgPartEndChars + "'), '" + CmdMsgEndChars + "')); ");
            sbDbSetup.Append("END " + strDelimiter);

            return sbDbSetup.ToString();
        }

        /// <summary>
        ///     Generates Default Removal of Database Process Procedures for Server Operations
        /// </summary>
        /// <returns>Text for Deleting Database Procedure That Generate and Push Server Command Messages</returns>
        public string GenerateDefaultDataCleanup() {

            string strDelimiter = DBProcDelimiter;
                                    /* Database Replacement Script Deimiter for Procedures */
            StringBuilder sbDbCleanUp = new StringBuilder();
                                    /* Holder for Database Reset Setup Script */

            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgGroupCreate " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgGroupDataResultCreate " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgCreate " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgJSVarUpdate " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgJSFuncCall " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgJSFuncAddParam  " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgSaveClose " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgsOutput " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgError " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JSONMsgsClear " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_SendMsgUser " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_SendMsgUserByDesign " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_StartGroup " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_JoinGroup " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_LeaveGroup " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_CloseGroup " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_RegisterFile " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_RegisterDataEvent " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_RegisterDataQuery " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_RegisterDataStatement " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP PROCEDURE IF EXISTS RevComm_RunDataProcess " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP FUNCTION IF EXISTS RevComm_JSONMsgSaveCheck " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP FUNCTION IF EXISTS RevComm_OutputLimitCheck " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP TABLE IF EXISTS revcomm_data_results " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP TABLE IF EXISTS revcomm_msg_funccall_params " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP TABLE IF EXISTS revcomm_msg_funccalls " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP TABLE IF EXISTS revcomm_msg_varupdates " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP TABLE IF EXISTS revcomm_msgs " + strDelimiter + " ");
            sbDbCleanUp.Append("DROP TABLE IF EXISTS revcomm_msg_groups " + strDelimiter);

            return sbDbCleanUp.ToString();
        }

        public bool ConfirmMessageDataType(string strMsgDataType) {

            return dictDBTypeMaxSize.ContainsKey(strMsgDataType.ToUpper());
        }

        /// <summary>
        ///    Called When a Window Event Occurs to Check for Shutdown
        /// </summary>
        /// <param name="cmMsg">Event Indicator</param>
        private void WindowCloseCheck(CONTROLMSGS cmMsg) {

            switch (cmMsg) { 
           
                case CONTROLMSGS.CTRL_CLOSE_EVENT: {

                    boolServerRunning = false;
                    break;
                }
                case CONTROLMSGS.CTRL_LOGOFF_EVENT: {

                    boolServerRunning = false;
                    break;
                }
                case CONTROLMSGS.CTRL_SHUTDOWN_EVENT: {

                    boolServerRunning = false;
                    break;
                }
            }
        }

        /// <summary>
        ///    Override Function for Getting Alerts If Windows Events Occur
        /// </summary>
        /// <param name="sscCheckFunct">Function to Process the Window Event</param>
        /// <param name="boolAdd">Indicator to Add to Functions Alerted on Windows Events</param>
        /// <returns>True If Window Event Occurs, Else False</returns>
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(WindowCloseCheckFunct sscCheckFunct, bool boolAdd);
    }
}
