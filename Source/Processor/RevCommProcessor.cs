using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using Newtonsoft.Json;

namespace RevelationsStudios.RevCommProcessor {

    public class ClientMsg {

        public string DESIGNATION { get; set; }
        public IList<ClientMsgVarUpdates>? VARUPDATES { get; set; }
        public IList<ClientMsgFunCalls>? FUNCCALLS { get; set; }
    }

    public class ClientMsgVarUpdates {

        public string NAME { get; set; }
        public object VALUE { get; set; }
    }

    public class ClientMsgFunCalls {

        public string NAME { get; set; }
        public IList<object> PARAMS { get; set; }
    }

    public class ServerMsg {

        public string DESIGNATION { get; set; }
        public IDictionary<string, object>? PARAMS { get; set; }
    }

    public class RegClassInfo {

        public object objReg { get; set; }
        public Type tyClass { get; set; }
    }

    public class SendFuncInfo {

        public RegClassInfo rciObj { get; set; }
        public string strFuncName { get; set; }
        public string strDesign { get; set; }
    }

    public class DataMap {

        public int nType { get; set; }
        public RegClassInfo rciObj { get; set; }
        public string strFuncVarName { get; set; }
        public string strDataProcessDesign { get; set; }
        public string strParam { get; set; }
        public string strValue { get; set; }
        public MethodInfo miFuncInvoke { get; set; }
    }

    public class DownloadFileList { 
    
        public List<string> STREAMFILELIST { get; set; }
    }

    public class RevCommProcessor {

        private const int BUFFERSIZE = 8192,/* Size of Message Buffer */
                          MAXBUFFERSIZEDIGITS = 10,
                                    /* Maximum Number of Digits in Buffer Size */
                          ENCRYPTKEYSIZE = 32,
                                    /* Size of Encryption Key */
                          ENCRYPTIVBLOCKSIZE = 16;
                                    /* Size of Encryption IV Block */
        private const BindingFlags METHODPERMISSIONTYPES = BindingFlags.Public | BindingFlags.Instance |
                                                           BindingFlags.Static | BindingFlags.NonPublic;
                                    /* Settings for Finding Variables and Methods by Modified Type */                         
        private static bool boolLogFileNotLocked = true;
                                    /* Indicator That File is Not Locked */
        private static Queue<string> qstrLogFileMsgWaiting = new Queue<string>();
                                    /* Waiting Log File Messages for When File is not Locked */
        private Task tskCommunicate = null,
                                    /* Communication Task */
                     tskAsyncDataProcessor = null;
                                    /* Processing Asynchronous Data Processing Returns Task */
        private CancellationTokenSource ctsCloser = new CancellationTokenSource();
                                    /* Controlling Token for Tasks */
        private Dictionary<string, RegClassInfo> dictRegObjects = new Dictionary<string, RegClassInfo>();
                                    /* Registered Objects */
        private Dictionary<string, Dictionary<int,ServerMsg>> dictSendStorage = new Dictionary<string, Dictionary<int, ServerMsg>>();
                                    /* Storage for Sending Messages to Server */
        private ClientMsg cmDirectMsgSend = new ClientMsg() { DESIGNATION = "", 
                                                              VARUPDATES = new List<ClientMsgVarUpdates>(), 
                                                              FUNCCALLS = new List<ClientMsgFunCalls>() };
                                    /* Storage for Sending Direct Client Messages to Server */
        private Dictionary<string, Dictionary<int, List<SendFuncInfo>>> dictSendFuncs = new Dictionary<string, 
                                                                                                       Dictionary<int, List<SendFuncInfo>>>();
                                    /* Storage for Functions to Call When Receiving Response Messages to Server */
        private Dictionary<int, List<SendFuncInfo>> dictsfiHTTPSendFuncs = new Dictionary<int, List<SendFuncInfo>>();
                                    /* Storage for Functions to Call When Receiving HTTP Response Messages to Server */
        private List<SendFuncInfo> ltsfiDirectMsgSendFuncs = new List<SendFuncInfo>();
                                    /* Storage for Functions to Call When Receiving Direct Message Response Messages to Server */
        private Dictionary<int, Dictionary<int, CancellationTokenSource>> dictSendHTTPReceivers = 
            new Dictionary<int, Dictionary<int, CancellationTokenSource>>();
                                    /* List of HTTP Response Messages to Process */
        private Dictionary<int, Dictionary<int, CancellationTokenSource>> dictSendDataProcessReceivers = 
            new Dictionary<int, Dictionary<int, CancellationTokenSource>>();
                                    /* List of Data Processes Response Messages to Process Asyncronously */
        private Dictionary<int, Dictionary<int, DateTime>> dictSendDataProcessStartTime = 
            new Dictionary<int, Dictionary<int, DateTime>>();
                                    /* List of Data Processes Response Messages to Process with Start Time */
        private Dictionary<string, DataMap> dictDataMaps = new Dictionary<string, DataMap>();
                                    /* For Mapping Variables or Functions to Data Processes */
        private List<int> ltnUsedIDs = new List<int>();
                                    /* Used IDs */
        private List<string> ltstrAutoRetDirectMsgDesigns = new List<string>();
                                    /* List of Direct Message Designations to do Auto Retrieval for */
        private bool boolAutoRetProcessCmd = false; 
                                    /* Indicator to Process Automated Retrieval Client Messages */
        private float fAutoRetLimitInMillis = 1000;
                                    /* Automated Retrieval for HTTP Transmission or Data Process Time Limit in Milliseconds */
        private bool boolAutoRetEndTrans = false;   
                                    /* Indicator to Close HTTP Transmission or Delete Data Process After Automated Retrieval */
        private Queue<string> qstrDisplayErrorMsg = new Queue<string>();
                                    /* List of Displayable Error Messages */
        private bool boolDebug = false;
                                    /* Indicator To Do Debugging */
        private enum CONTROLMSGS { CTRL_C_EVENT = 0, CTRL_BREAK_EVENT, CTRL_CLOSE_EVENT, CTRL_LOGOFF_EVENT = 5, CTRL_SHUTDOWN_EVENT };
                                    /* Window Control Messages for Checking for Shutdown */
        private delegate void WindowCloseCheckFunct(CONTROLMSGS cmMsg);
                                    /* Function Type for Checking If Window Containing Server is Shutting Down */
        private WindowCloseCheckFunct sscCheckWindowEnd;
                                    /* Checks If Window is Closing */

        [DllImport("RevCommClient64.dll", EntryPoint = "Activate")]
        private static extern bool ClientActivate();
                                    /* Connects to Server Using Default Settings */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateByHostPort")]
        private static extern bool ClientActivateByHostPort(string strHostName, int nPort);
                                    /* Connects to Server Using Host and Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateWithServer")]
        private static extern bool ClientActivateWithServer();
                                    /* Connects to Server Using Default Settings for Local Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateWithServerByPort")]
        private static extern bool ClientActivateWithServerByPort(int nPort);
                                    /* Connects to Server Using Default Host and Set Port for Local Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateUsingSSL")]
        private static extern bool ClientActivateUsingSSL(string strSSLPrivKeyName);
                                    /* Connects to Server Using Default Settings */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateByHostPortUsingSSL")]
        private static extern bool ClientActivateByHostPortUsingSSL(string strHostName, int nPort, string strSSLPrivKeyName);
                                    /* Connects to Server Using Host and Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateWithServerUsingSSL")]
        private static extern bool ClientActivateWithServerUsingSSL(string strSSLPrivKeyName);
                                    /* Connects to Server Using Default Settings for Local Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "ActivateWithServerByPortUsingSSL")]
        private static extern bool ClientActivateWithServerByPortUsingSSL(int nPort, string strSSLPrivKeyName);
                                    /* Connects to Server Using Default Host and Set Port for Local Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartStream")]
        private static extern void ClientStartStream(int nNewTransID, string strHostName, int nPort);
                                    /* Starts Stream */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPPostAsyncWithHostPort")]
        private static extern void ClientStartHTTPPostAsyncWithHostPort(int nNewTransID, string strHostName, int nPort);
                                    /* Starts Setup to Send Asynchronous HTTP POST Messages */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPPostAsyncWithHost")]
        private static extern void ClientStartHTTPPostAsyncWithHost(int nNewTransID, string strHostName, int nPort);
                                    /* Starts Setup to Send Asynchronous HTTP POST Messages Through Server Default Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPPostAsyncWithHostPort")]
        private static extern void ClientStartHTTPPostAsyncWithHostPort(int nNewTransID, string strHostName);
                                    /* Starts Setup to Send Asynchronous HTTP POST Messages Through Server Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPPostAsyncWithHost")]
        private static extern void ClientStartHTTPPostAsyncWithHost(int nNewTransID, string strHostName);
                                    /* Starts Setup to Send Synchronous HTTP POST Messages Through Server Default Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPPostSyncWithHostPort")]
        private static extern void ClientStartHTTPPostSyncWithHostPort(int nNewTransID, string strHostName, int nPort);
                                    /* Starts Setup to Send Synchronous HTTP POST Messages Through Server Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPPostSyncWithHost")]
        private static extern void ClientStartHTTPPostSyncWithHost(int nNewTransID, string strHostName);
                                    /* Starts Setup to Send Synchronous HTTP POST Messages Through Server Default Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPGetASyncWithHostPort")]
        private static extern void ClientStartHTTPGetAsyncWithHostPort(int nNewTransID, string strHostName, int nPort);
                                    /* Starts Setup to Send Synchronous HTTP POST Messages Through Server Port  */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPGetASyncWithHost")]
        private static extern void ClientStartHTTPGetAsyncWithHost(int nNewTransID, string strHostName);
                                    /* Starts Setup to Send Asynchronous HTTP GET Messages Through Server Default Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPGetSyncWithHostPort")]
        private static extern void ClientStartHTTPGetSyncWithHostPort(int nNewTransID, string strHostName, int nPort);
                                    /* Starts Setup to Send Synchronous HTTP GET Messages */

        [DllImport("RevCommClient64.dll", EntryPoint = "StartHTTPGetSyncWithHost")]
        private static extern void ClientStartHTTPGetSyncWithHost(int nNewTransID, string strHostName);
                                    /* Starts Setup to Send Synchronous HTTP GET Messages Through Server Default Port */

        [DllImport("RevCommClient64.dll", EntryPoint = "SendDirectMsg")]
        private static extern void ClientSendDirectMsg(string strMsg);
                                    /* Sends Direct Messages to Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "SendDirectMsgWithDesign")]
        private static extern void ClientSendDirectMsgWithDesign(string strMsg, string strDesign);
                                    /* Sends Direct Messages with Designation to Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "SendDirectMsgPeerToPeer")]
        private static extern void ClientSendDirectMsgPeerToPeer(string strMsg);
                                    /* Sends Direct Messages to "Peer To Peer" Clients */

        [DllImport("RevCommClient64.dll", EntryPoint = "SendDirectMsgPeerToPeerWithDesign")]
        private static extern void ClientSendDirectMsgPeerToPeerWithDesign(string strMsg, string strDesign);
                                    /* Sends Direct Messages with Designation to "Peer To Peer" Clients */

        [DllImport("RevCommClient64.dll", EntryPoint = "HasPeerToPeerClients")]
        private static extern bool ClientHasPeerToPeerClients();
                                    /* Finds If "Peer To Peer" Clients are Connected */

        [DllImport("RevCommClient64.dll", EntryPoint = "AddStreamMsg")]
        private static extern void ClientAddStreamMsg(int nTransID, string strMsg);
                                    /* Adds to Queue of Messages to be Sent Through Stream (Will Not Store Messages That Have Stream Reserved Characters) */

        [DllImport("RevCommClient64.dll", EntryPoint = "RegisterDataProcess")]
        private static extern void ClientRegisterDataProcess(int nNewTransID, string strDataDesign);
                                    /* Register Data Process for Execution */

        [DllImport("RevCommClient64.dll", EntryPoint = "RegisterDataProcessWithParams")]
        private static extern void ClientRegisterDataProcessWithParams(int nNewTransID, string strDataDesign, string strParamName, string strParamValue);
                                    /* Register Data Process for Execution with Parameters or Add Parameters to Existing One */

        [DllImport("RevCommClient64.dll", EntryPoint = "SendDataProcess")]
        private static extern void ClientSendDataProcess(int nTransID, int nNewRespID, string strDataDesign, bool boolAsync);
                                    /* Sends Data Process Execution Message */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetDataProcessResponsePointer")]
        private static extern IntPtr ClientGetDataProcessResponse(int nTransID, int nRespID);
                                    /* Gets Response Returned from Data Process Execution Message, Before Deleting it from Response Queue and
                                     * Outputs Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckDataProcessResponse")]
        private static extern int ClientCheckDataProcessResponse(int nTransID, int nRespID);
                                    /* Check If Response Returned from Data Process Execution Message, Before Deleting it from Response Queue and
                                     * Outputs Length of Response Message Indicated by the Response ID and Communication Transmission ID If Exists or Has Values, Else Returns 0 */

        [DllImport("RevCommClient64.dll", EntryPoint = "SendHTTP")]
        private static extern void ClientSendHTTP(int nTransID, int nNewRespID);
                                    /* Send Stored HTTP Message */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetHTTPResponsePointer")]
        private static extern IntPtr ClientGetHTTPResponse(int nTransID, int nRespID);
                                    /* Gets Response Returned from Send Message, Before Deleting it from Response Queue and
                                     * Outputs Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckHTTPResponse")]
        private static extern int ClientCheckHTTPResponse(int nTransID, int nRespID);
                                    /* Check If Response Returned from Send Message, Before Deleting it from Response Queue and
                                     * Outputs Length of Response Message Indicated by the Response ID and Communication Transmission ID If Exists or Has Values, Else Returns 0 */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetStreamMsgPointer")]
        private static extern void ClientGetStreamMsg(int nTransID);
                                    /* Gets a Waiting Message from Stream, Before Deleting it from the Wait Queue and
                                     * Outputs Message as a String If It Exists, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetStreamMsgNextPointer")]
        private static extern void ClientGetStreamMsgNext();
                                    /* Gets Next Waiting Message from Any Stream, Before Deleting it from the Wait Queue and
                                     * Outputs Message as a String If It Exists, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckStreamMsgReady")]
        private static extern int ClientCheckStreamMsgReady();
                                    /* Checks for Stream Messages on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckStreamMsgByIDReady")]
        private static extern int ClientCheckStreamMsgByIDReady(int nTransID);
                                    /* Checks for Stream Messages with Specified Designation on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "ClearStreamMsgs")]
        private static extern bool ClientClearStreamMsgs();
                                    /* Clears Stream Messages on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "ClearStreamMsgsByIDReady")]
        private static extern bool ClientClearStreamMsgsByIDReady(int nTransID);
                                    /* Sends Stream Messages with Designation on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetDirectMsgPointer")]
        private static extern IntPtr ClientGetDirectMsg(string strDesign);
                                    /* Gets a Specified Waiting Direct Message, Before Deleting it from the Wait Queue and
                                     * Outputs Message as a String If It Exists, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetDirectMsgNextPointer")]
        private static extern IntPtr ClientGetDirectMsgNext();
                                    /* Gets a Waiting Direct Message, Before Deleting it from the Wait Queue and
                                     * Outputs Message as a String If It Exists, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckDirectMsgsReady")]
        private static extern int ClientCheckDirectMsgsReady();
                                    /* Checks for Direct Messages on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckDirectMsgsWithDesignReady")]
        private static extern int ClientCheckDirectMsgsWithDesignReady(string strFileDesign);
                                    /* Checks for Direct Messages with Specified Designation on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "ClearDirectMsgs")]
        private static extern bool ClientClearDirectMsgs();
                                    /* Clears Direct Messages on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "ClearDirectMsgsWithDesign")]
        private static extern bool ClientClearDirectMsgsWithDesign(string strDesign);
                                    /* Sends Direct Messages with Designation on Client */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetStreamFile")]
        private static extern void ClientGetStreamFile(string strFileDesign);
                                    /* Gets Downloaded File from Stream and Outputs File to Destination Directory, Before File is Removed */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckStreamFileDownload")]
        private static extern bool ClientCheckStreamFileDownload(string strFileDesign, StringBuilder sbRetFilePath, int nFilePathLen, IntPtr pRetFileContents);
                                    /* Gets If File was Downloaded from Stream and Returns It, Before File is Removed */

        [DllImport("RevCommClient64.dll", EntryPoint = "CheckStreamFileReady")]
        private static extern int ClientCheckStreamFileReady(string strFileDesign);
                                    /* Finds If File is Ready to be Downloaded from Stream and Returns File Length If Found */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetStreamFilePathLength")]
        private static extern int ClientGetStreamFilePathLength(string strFileDesign);
                                    /* Finds Length of File's Path and Name for Retrieval from File Information If File is in Stream */

        [DllImport("RevCommClient64.dll", EntryPoint = "ClearStreamFileDownload")]
        private static extern void ClientClearStreamFileDownload(string strFileDesign);
                                    /* Removes a Downloaded File from Stream */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetStreamFileList")]
        private static extern void ClientGetStreamFileList(StringBuilder sbRetMsg, StringBuilder sbRetMsgLen);
                                    /* Gets List of Downloadable Files */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetHTTPProcessPage")]
        private static extern void ClientSetHTTPProcessPage(int nTransID, string strProcessPathPage);
                                    /* Sets Processing Page for HTTP POST and GET Transmissions */

        [DllImport("RevCommClient64.dll", EntryPoint = "AddHTTPMsgData")]
        private static extern void ClientAddHTTPMsgData(int nTransID, string strVarName, string strValue);
                                    /* Adds a Variable and its Value to the Next Message Being Sent Through HTTP Transmission */

        [DllImport("RevCommClient64.dll", EntryPoint = "ClearHTTPMsgData")]
        private static extern void ClientClearHTTPMsgData(int nTransID);
                                    /* Clears Next Message Being Sent Through HTTP Transmission  */

        [DllImport("RevCommClient64.dll", EntryPoint = "UseHTTPSSL")]
        private static extern void ClientUseHTTPSSL(int nTransID, bool boolUseSSL);
                                    /* Sends Indicator Message to Use SSL for HTTP Message */

        [DllImport("RevCommClient64.dll", EntryPoint = "Close")]
        private static extern void ClientClose(int nTransID);
                                    /* Closes Specified Transmission */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetLogErrorPointer")]
        private static extern IntPtr ClientGetLogError();
                                    /* Gets Log Error Message Before Clearing its Information and
                                     * Outputs Error Message as a String If It Exists, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "GetDisplayErrorPointer")]
        private static extern IntPtr ClientGetDisplayError();
                                    /* Gets Log Error Message Before Clearing its Information and 
                                     * Outputs Error Message as a String If It Exists, Else Blank String */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetStreamMsgSeparator")]
        private static extern void ClientSetStreamMsgSeparator(int nTransID, string strStreamMsgSeparator);
                                    /* Sets Message Part Character for Stream (Defaults to '|||') */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetStreamMsgEnd")]
        private static extern void ClientSetStreamMsgEnd(int nTransID, string strStreamMsgEnd);
                                    /* Sets Message End Character for Stream (Defaults to '|||*') */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetStreamMsgFiller")]
        private static extern void ClientSetStreamMsgFiller(int nTransID, string strStreamMsgFiller);
                                    /* Sets Message Filler Character for Stream (Defaults to '\0') */

        [DllImport("RevCommClient64.dll", EntryPoint = "Disconnect")]
        private static extern void ClientDisconnect();
                                    /* Disconnects Client from Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "DisconnectPeerToPeer")]
        private static extern void ClientDisconnectPeerToPeer();
                                    /* Disconnects "Peer To Peer" Server and Clients */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetMsgPartIndicator")]
        private static extern int ClientSetMsgPartIndicator(string strSetMsgPartIndicate);
                                    /* Sets Message Part Character for Server Messages (Defaults to '||||') */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetMsgStartIndicator")]
        private static extern int ClientSetMsgStartIndicator(string strSetMsgStartIndicate);
                                    /* Sets Message Start Character for Server (Defaults to '*|||') */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetMsgEndIndicator")]
        private static extern int ClientSetMsgEndIndicator(string strSetMsgEndIndicate);
                                    /* Sets Message End Character for Server (Defaults to '|||*') */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetMsgFiller")]
        private static extern void ClientSetMsgFiller(char charSetMsgFiller);
                                    /* Sets Message Filler Character for Server Messages (Defaults to '\0') */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetMsgIndicatorLen")]
        private static extern int ClientSetMsgIndicatorLen(int nSetMsgIndicatorLen);
                                    /* Sets Message Indicator Character Length for Server (Defaults is 4) */

        [DllImport("RevCommClient64.dll", EntryPoint = "IsConnected")]
        private static extern bool ClientIsConnected();
                                    /* Finds If Client is Connected to Server */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugReceivedPointer")]
        private static extern IntPtr ClientDebugReceived(int nMsgIndex);
                                    /* Debug for Getting Message from Receiving Queue by Message Index */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugToSendPointer")]
        private static extern IntPtr ClientDebugToSend(int nMsgIndex);
                                    /* Debug for Getting Message from Sending Queue by Message Index */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugReceivedStoredPointer")]
        private static extern IntPtr ClientDebugReceivedStored(int nMsgIndex);
                                    /* Debug for Getting Message from Receiving Stored Queue by Message Index */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugToSendStoredPointer")]
        private static extern IntPtr ClientDebugToSendStored(int nMsgIndex);
                                    /* Debug for Getting Message from Sending Stored Queue by Message Index */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugSendQueueCount")]
        private static extern int ClientDebugSendQueueCount();
                                    /* Debug for Count of Message in Sending Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugReceivedQueueCount")]
        private static extern int ClientDebugReceivedQueueCount();
                                    /* Debug for Count of Message in Receiving Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugSendStoredQueueCount")]
        private static extern int ClientDebugSendStoredQueueCount();
                                    /* Debug for Count of Message in Sending Stored Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugReceivedStoredQueueCount")]
        private static extern int ClientDebugReceivedStoredQueueCount();
                                    /* Debug for Count of Message in Receiving Stored Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugSendMsgLength")]
        private static extern int ClientDebugSendMsgLength(int nMsgIndex);
                                    /* Debug for Length of Selected Message in Sending Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugReceivedMsgLength")]
        private static extern int ClientDebugReceivedMsgLength(int nMsgIndex);
                                    /* Debug for Length of Selected Message in Receiving Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugSendStoredMsgLength")]
        private static extern int ClientDebugSendStoredMsgLength(int nMsgIndex);
                                    /* Debug for Length of Selected Message in Sending Stored Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "DebugReceivedStoredMsgLength")]
        private static extern int ClientDebugReceivedStoredMsgLength(int nMsgIndex);
                                    /* Debug for Length of Selected Message in Receiving Stored Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetQueueLimit")]
        private static extern void ClientSetQueueLimit(int nNewLimit);
                                    /* Sets Length of Message Queue */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetMsgLateLimit")]
        private static extern void ClientSetMsgLateLimit(int nTimeInMillisecs);
                                    /* Sets Message Viability Time Limit in Milliseconds */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetDropLateMsgs")]
        private static extern void ClientSetDropLateMsgs(bool boolDropLateMsgs);
                                    /* Sets Indicator to Drop Late Messages */

        [DllImport("RevCommClient64.dll", EntryPoint = "SetActivityCheckTimeLimit")]
        private static extern void ClientSetActivityCheckTimeLimit(int nTimeInMillis);
                                    /* Sets Length of Time Between Receipt of Clients and/or Server Messages Before Check is Send */

        [DllImport("RevCommClient64.dll", EntryPoint = "IsInSessionGroup")]
        private static extern bool ClientIsInSessionGroup();
                                    /* Gets Indicator That User is In Server Group Session */

        [DllImport("RevCommClient64.dll", EntryPoint = "IsSessionGroupHost")]
        private static extern bool ClientIsSessionGroupHost();
                                    /* Gets Indicator That User is Host of Server Group Session */

        [DllImport("RevCommClient64.dll", EntryPoint = "Communicate")]
        static extern void ClientCommunicate();
                                    /* Process Client Communications */

        public RevCommProcessor() {

            RegisterObject("GLOBAL", typeof(RevCommProcessor), this);
            RegisterObject("MAIN", typeof(RevCommProcessor), this);

            dictSendStorage.Add("HTTP", new Dictionary<int, ServerMsg>());
            dictSendStorage.Add("STREAMCLIENT", new Dictionary<int, ServerMsg>());
            dictSendStorage.Add("STREAMRAW", new Dictionary<int, ServerMsg>());
            dictSendStorage.Add("DATAPROCESS", new Dictionary<int, ServerMsg>());

            dictSendFuncs.Add("HTTP", new Dictionary<int, List<SendFuncInfo>>());
            dictSendFuncs.Add("DATAPROCESS", new Dictionary<int, List<SendFuncInfo>>());
        }

        /// <summary>
        ///     Processes Incoming Communications
        /// </summary>
        /// <param name="strJSONMsg">JSON Message</param>
        private void ProcessIncoming(string strJSONMsg) {

            IList<ClientMsg> ltJSONMsg;
                                    /* List of Messages to Process */
            RegClassInfo rciRegSelect = null;
                                    /* Selected Registered Object */
            String strFuncName;     /* Selected Function Name */
            List<object> ltobjFuncParamValues = new List<object>();
                                    /* Parameter Values of the Selected Function */
            MethodInfo miFuncFound = null; 
                                    /* Found Matching Function */
            ParameterInfo[] apmSelect;
                                    /* Information on Parameters for Selected Method Signature */
            int nFuncParamCount = 0,/* Selected Function Parameter Count */
                nParamCountFound = 0;
                                    /* Found Matching Function Parameter Count */
            List<Type> lttyTypeList;/* Used for Finding Method by List of Parameter Types */
            bool boolFuncValid = true;
                                    /* Indicator That Selected Function Signature is a Valid Version */
            int nCounter = 0;       /* Counter for Loop */

            try {

                if (boolDebug) {

                    Log("Processed: " + strJSONMsg);
                }

                if (strJSONMsg != "") {

                    ltJSONMsg = JsonConvert.DeserializeObject<List<ClientMsg>>(strJSONMsg.Substring(0, strJSONMsg.LastIndexOf("]") + 1));

                    foreach (ClientMsg cmUpdateSets in ltJSONMsg) {

                        if (dictRegObjects.ContainsKey(cmUpdateSets.DESIGNATION)) {

                            rciRegSelect = dictRegObjects[cmUpdateSets.DESIGNATION];
                        }
                        else {

                            rciRegSelect = dictRegObjects["GLOBAL"];
                        }

                        if (cmUpdateSets.VARUPDATES != null) {

                            foreach (ClientMsgVarUpdates cmvVarUpdates in cmUpdateSets.VARUPDATES) {

                                if (rciRegSelect.objReg != null) {

                                    rciRegSelect.tyClass.GetField(cmvVarUpdates.NAME, METHODPERMISSIONTYPES)
                                                        .SetValue(rciRegSelect.objReg, cmvVarUpdates.VALUE);
                                }
                            }
                        }

                        if (cmUpdateSets.FUNCCALLS != null) {

                            foreach (ClientMsgFunCalls cmfFuncCalls in cmUpdateSets.FUNCCALLS) {

                                strFuncName = cmfFuncCalls.NAME;
                                ltobjFuncParamValues = new List<object>();
                                ltobjFuncParamValues.AddRange(cmfFuncCalls.PARAMS);
                                nFuncParamCount = ltobjFuncParamValues.Count;

                                /* Check Object's Methods for Find Method with Matching Signature */
                                foreach (MethodInfo miFuncSelect in rciRegSelect.tyClass.GetMethods(METHODPERMISSIONTYPES)) {

                                    /* If Version of Method Found, Check for Matching Parameters with Trailing Optional Ones */
                                    if (miFuncSelect.Name.ToLower() == strFuncName.ToLower()) { 

                                        apmSelect = miFuncSelect.GetParameters();
                                        nParamCountFound = apmSelect.Length;
                                        lttyTypeList = new List<Type>();

                                        if (nFuncParamCount <= nParamCountFound) { 

                                            for (nCounter = 0; nCounter < nParamCountFound && boolFuncValid; nCounter++) {

                                                lttyTypeList.Add(apmSelect[nCounter].ParameterType);

                                                if (nCounter < nFuncParamCount) {

                                                    try {

                                                        /* For Required Parameter Check If Type is Compatible, If Not, Exception Will Invalidate Signature */
                                                        TypeDescriptor.GetConverter(ltobjFuncParamValues[nCounter].GetType())
                                                                      .ConvertTo(ltobjFuncParamValues[nCounter], apmSelect[nCounter].ParameterType);
                                                    }
                                                    catch (Exception exError) {

                                                        if (ltobjFuncParamValues.GetType().GetMethod("ToString") != null) {

                                                            ltobjFuncParamValues[nCounter] = ltobjFuncParamValues[nCounter].ToString();
                                                        }
                                                        else { 

                                                            Log(exError.Message, true);
                                                            boolFuncValid = false;
                                                        }
                                                    }
                                                }
                                                else if (!apmSelect[nCounter].IsOptional) {

                                                    /* Else Selected Method Signature Doesn't Match Received Parameters */
                                                    boolFuncValid = false;
                                                }
                                            }

                                            /* If Selected Method Signature Matches Sent Parameters, Method was Confirmed, and the Count of Parameter Count is the Same as Sent,
                                               Exit Loop with the Selected Signature for Invoking, Else The Last Match will be Invoked */
                                            if (boolFuncValid &&
                                                (miFuncFound = rciRegSelect.tyClass.GetMethod(strFuncName, METHODPERMISSIONTYPES, null, lttyTypeList.ToArray(), null)) != null &&
                                                nFuncParamCount == nParamCountFound) {

                                                break;
                                            }

                                            boolFuncValid = true;
                                        }
                                    }
                                }

                                if (miFuncFound != null) {

                                    if (miFuncFound.GetParameters().Length > nFuncParamCount) {

                                        nFuncParamCount = miFuncFound.GetParameters().Length;

                                        for (nCounter = ltobjFuncParamValues.Count; nCounter < nFuncParamCount; nCounter++) { 

                                            /* If Parameter is Optional, Add Missing Parameter Value to List for Method Invoking  */
                                            ltobjFuncParamValues.Add(Type.Missing);
                                        }
                                    }

                                    if (rciRegSelect.objReg != null) {

                                        miFuncFound.Invoke(rciRegSelect.objReg,
                                                           METHODPERMISSIONTYPES | BindingFlags.OptionalParamBinding, 
                                                           null,
                                                           ltobjFuncParamValues.ToArray(), 
                                                           CultureInfo.InvariantCulture);
                                    }
                                    else {

                                        miFuncFound.Invoke(Activator.CreateInstance(rciRegSelect.tyClass),
                                                           METHODPERMISSIONTYPES | BindingFlags.OptionalParamBinding, 
                                                           null,
                                                           ltobjFuncParamValues.ToArray(), 
                                                           CultureInfo.InvariantCulture); ;
                                    }

                                    miFuncFound = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exError) {

                /* If Not Getting a List of Updated Rows Returned by the Database */
                if (!strJSONMsg.Contains("[{\"rows\":")) { 

                    if (exError.InnerException != null) {

                        Log("Processing incoming messages failed. Exception: " + exError.Message + 
                            ". Inner Exception: " + exError.InnerException.Message + ". Backtrace: " +
                            string.Join(" || ", exError.StackTrace), true);
                    }
                    else { 
                    
                        Log("Processing incoming messages failed. Exception: " + exError.Message + 
                            ". Backtrace: " + string.Join(" || ", exError.StackTrace), true);
                    }
                }
            }
        }

        /// <summary>
        ///     Runs Functions After Message Returns. Function Can Have a Single String Parameter or None
        /// </summary>
        /// <param name="sfiExecute">Information on Function to Execute</param>
        /// <param name="strRespMsg">Optional Returned Message to Pass as a Parameter If Function Takes a Single String</param>
        private void RunRetFunc(SendFuncInfo sfiExecute, string strRespMsg = null) {

            MethodInfo miFuncSelect;/* Selected Function to Update */                                 
/*            List<object> ltobjFuncParamValues = new List<object>();
                                    /* Parameter Values of the Selected Function */

            if (sfiExecute.rciObj.objReg != null) {

                if ((miFuncSelect = sfiExecute.rciObj.tyClass.GetMethod(sfiExecute.strFuncName, METHODPERMISSIONTYPES)) != null) {

                    List<object> ltobjFuncParamValues = new List<object>();

                    if (miFuncSelect.GetParameters().Length > 0) {

                        ltobjFuncParamValues.Add(strRespMsg);
                    }

                    try {

                        miFuncSelect.Invoke(sfiExecute.rciObj.objReg,
                                            METHODPERMISSIONTYPES | BindingFlags.OptionalParamBinding,
                                            null,
                                            ltobjFuncParamValues.ToArray(),
                                            CultureInfo.InvariantCulture);
                    }
                    catch (Exception exError)  {

                        Log("During running response process method, '" + sfiExecute.strFuncName + "' exception occurred.", true, exError);
                    }
                }
                else {

                    Log("During running response process method, '" + sfiExecute.strFuncName + "', does not exist.", true);
                }
            }
        }

        /// <summary>
        ///     Connects to Server
        /// </summary>
        /// <param name="strHostNameIP">Optional Hostname or IP Address to Connect, Defaults to 127.0.0.1</param>
        /// <param name="nPort">Optional Port to Connect to, Defaults to Client Settings</param>
        /// <param name="boolStartServer">Optional Indicator to Start Local Server, Defaults to False</param>
        /// <param name="strSSLPrivKeyName">Optional Name of SSL Private Key, Required Only If Using SSL Connection</param>
        /// <param name="boolSSLConnect">Optional Indicator to Use SSL to Use SSL, Defaults to False</param>
        /// <returns>True If Connection is Made, Else False</returns>
        public bool Connect(string strHostNameIP = "", 
                            int nPort = 0, 
                            bool boolStartServer = false, 
                            string strSSLPrivKeyName = "",
                            bool boolSSLConnect = false) {

            bool boolConnected = IsConnected();
                                    /* Indicator That Client Server Connection was Made */

            try {

                if (boolSSLConnect == false || (boolSSLConnect == true && strSSLPrivKeyName != "")) { 

                    if (!boolConnected) {

                        // If Not Starting the Server Before Connecting to it
                        if (!boolStartServer) {

                            // If No Hostname or IP Address was Set, Connect with Default Settings
                            if (strHostNameIP == "") {

                                if (boolSSLConnect) { 
                                
                                    if (!(boolConnected = ClientActivateUsingSSL(strSSLPrivKeyName))) {

                                        Log("Connecting client to server using default settings and SSL failed.", true);
                                    }
                                }
                                else if (!(boolConnected = ClientActivate())) {

                                    Log("Connecting client to server using default settings failed.", true);
                                }
                            }
                            else if (nPort != 0) {

                                // Else Connect with Sent Settings
                                if (boolSSLConnect) { 
                                
                                    if (!(boolConnected = ClientActivateByHostPortUsingSSL(strHostNameIP, nPort, strSSLPrivKeyName))) {

                                        Log("Connecting client to server using host, " + strHostNameIP + ", port: " + nPort + " and SSL failed.", true);
                                    }
                                }
                                else if (!(boolConnected = ClientActivateByHostPort(strHostNameIP, nPort))) {

                                    Log("Connecting client to server using host, '" + strHostNameIP + "', port: " + nPort + " failed.", true);
                                }
                            }
                            else {

                                Log("Can not connect client to server using host due to invalid settings.", true);
                            }
                        }
                        else if (nPort != 0) {

                            // If No Default Settings were Sent, Start Server and Connect to it Using Default Settings
                            if (boolSSLConnect) { 
                                
                                if (!(boolConnected = ClientActivateWithServerUsingSSL(strSSLPrivKeyName))) {

                                    Log("Starting server and connecting client to it using SSL failed.", true);
                                }
                            }
                            else if (!(boolConnected = ClientActivateWithServer())) {

                                Log("Starting server and connecting client to it failed.", true);
                            }
                        }
                        else if (boolSSLConnect) { 
                        
                            if (!(boolConnected = ClientActivateWithServerByPortUsingSSL(nPort, strSSLPrivKeyName))) {

                                Log("Starting server and connecting client to it using port: " + nPort + " failed.", true);
                            }
                        }
                        else if (!(boolConnected = ClientActivateWithServerByPort(nPort))) {

                            Log("Starting server and connecting client to it using port: " + nPort + " failed.", true);
                        }

                        if (boolConnected) {

                            if (tskCommunicate == null) {

                                tskCommunicate = ComSetup();
                            }
                        }

                        /* Setup to Check for Window Shutdown Events */
                        sscCheckWindowEnd = new WindowCloseCheckFunct(WindowCloseCheck);
                        SetConsoleCtrlHandler(sscCheckWindowEnd, true);
                    }
                }
                else {

                    Log("Connecting client to server failed. Error: SSL connection's private key file name not set.", true);
                }
            }
            catch (Exception exError) {

                Log("Connecting client to server failed.", true, exError);
            }

            return boolConnected;
        }

        /// <summary>
        ///     Connect to Server Using SSL
        /// </summary>
        /// <param name="strSSLPrivKeyName">Name of SSL Private Key</param>
        /// <param name="strHostNameIP">Optional Hostname or IP Address to Connect, Defaults to 127.0.0.1</param>
        /// <param name="nPort">Optional Port to Connect to, Defaults to Client Settings</param>
        /// <param name="boolStartServer">Optional Indicator to Start Local Server, Defaults to False</param>
        /// <returns>True If Connection is Made, Else False</returns>
        public bool ConnectWithSSL(string strSSLPrivKeyName, 
                                   string strHostNameIP = "", 
                                   int nPort = 0, 
                                   bool boolStartServer = false) {

            return Connect(strHostNameIP, nPort, boolStartServer, strSSLPrivKeyName, true);
        }

        /// <summary>
        ///     Setups Communication Thread
        /// </summary>
        /// <returns>Task<returns>
        private async Task ComSetup() {

            await Task.Run(() => Communicate(), ctsCloser.Token);
        }

        /// <summary>
        ///     Setups Automated Thread for Processing Data Processes
        /// </summary>
        /// <returns>Task<returns>
        private async Task AsyncDataProcessorSetup() {

            await Task.Run(() => DoDataProcessResponses(), ctsCloser.Token);
        }

        /// <summary>
        ///     Starts HTTP Transmission with "Port" Method Registered to Transaction ID Using Hostname/IP and Optional Port and Async or Synched
        /// </summary>
        /// <param name="nNewTransID">HTTP Transmission ID</param>
        /// <param name="strHostNameIP">Hostname or IP Address</param>
        /// <param name="nPort">Optional Port, Defaults to Client Default Values Per Protocol</param>
        /// <param name="boolAsync">Optional Indicator to do HTTP Transmission Asynchronously, Defaults to True</param>
        /// <returns>True If HTTP Transmission was Successfully Started, Else False</returns>
        public bool StartHTTPPost(int nNewTransID, string strHostNameIP, int nPort = 0, bool boolAsync = true) {

            bool boolStarted = false;  
                                    // Indicator That Storage was Started

            if (!ValidateTransID(nNewTransID)) { 

                if (IsConnected()) { 
            
                    if (boolAsync) {

                        if (nPort != 0) {

                            ClientStartHTTPPostAsyncWithHostPort(nNewTransID, strHostNameIP, nPort);
                        }
                        else {

                            ClientStartHTTPPostAsyncWithHost(nNewTransID, strHostNameIP);
                        }
                    }
                    else if (nPort != 0) {

                        ClientStartHTTPPostSyncWithHostPort(nNewTransID, strHostNameIP, nPort);
                    }           
                    else {

                        ClientStartHTTPPostSyncWithHost(nNewTransID, strHostNameIP);
                    }

                    dictSendStorage["HTTP"].Add(nNewTransID, null);
                    boolStarted = true;
                }
                else { 

                    Log("During starting HTTP POST message, can not start due to not being connected.");
                }
            }

            return boolStarted;
        }

        /// <summary>
        ///     # Starts HTTP Transmission with "Get" Method Registered to Transaction ID Using Hostname/IP and Optional Port and Async or Synched
        /// </summary>
        /// <param name="nNewTransID">HTTP Transmission ID</param>
        /// <param name="strHostNameIP">Hostname or IP Address</param>
        /// <param name="nPort">Optional Port, Defaults to Client Default Values Per Protocol</param>
        /// <param name="boolAsync">Optional Indicator to do HTTP Transmission Asynchronously, Defaults to True</param>
        /// <returns>True If HTTP Transmission was Successfully Started, Else False</returns>
        public bool StartHTTPGet(int nNewTransID, string strHostNameIP, int nPort = 0, bool boolAsync = true) {

            bool boolStarted = false;
                                    // Indicator That Storage was Started  
              
            if (!ValidateTransID(nNewTransID)) { 
                    
                if (IsConnected()) {

                    if (boolAsync) {

                        if (nPort != 0) {

                            ClientStartHTTPGetAsyncWithHostPort(nNewTransID, strHostNameIP, nPort);
                        }
                        else {

                            ClientStartHTTPGetAsyncWithHost(nNewTransID, strHostNameIP);
                        }

                    }
                    else if (nPort != 0) {

                        ClientStartHTTPGetSyncWithHostPort(nNewTransID, strHostNameIP, nPort);
                    }
                    else {

                        ClientStartHTTPGetSyncWithHost(nNewTransID, strHostNameIP);
                    }

                    dictSendStorage["HTTP"].Add(nNewTransID, null);
                    boolStarted = true;
                }
                else {

                    Log("During starting HTTP GET message, can not start due to not being connected.");
                }
            }

            return boolStarted;
        }
        
        /// <summary>
        ///     Setup Retrieval of HTTP Results
        /// </summary>
        /// <param name="nTransID">HTTP Transaction ID</param>
        /// <param name="nRespID">HTTP Response ID for the Transaction</param>
        /// <param name="fAutoRetLimitInMillis">The Amount of Time in Milliseconds to Wait for a Response</param>
        /// <param name="boolAutoRetProcessCmd">Indicator to Automatically Process The Results as Commands</param>
        /// <param name="boolAutoRetEndTrans">Indicator to End Data Process After Final Results are Done</param>
        /// <returns>Task for Retrieving HTTP Result</returns>
        private async Task SendReceiverHTTPSetup(int nTransID,
                                                 int nNewRespID,
                                                 float fSetAutoRetLimitInMillis,
                                                 bool boolSetAutoRetProcessCmd,
                                                 bool boolSetAutoRetEndTrans,
                                                 CancellationTokenSource ctsNewTaskCloser) {

                await Task.Run(() => SendReceiverHTTP(nTransID,
                                                      nNewRespID,
                                                      fSetAutoRetLimitInMillis,
                                                      boolSetAutoRetProcessCmd,
                                                      boolSetAutoRetEndTrans, 
                                                      ctsNewTaskCloser.Token));
        }

        /// <summary>
        ///     Setup Retrieval of Data Process Results
        /// </summary>
        /// <param name="nTransID">Data Process' Transaction ID</param>
        /// <param name="nRespID">Data Process' Response ID for the Transaction</param>
        /// <param name="fAutoRetLimitInMillis">The Amount of Time in Milliseconds to Wait for a Response</param>
        /// <param name="boolAutoRetProcessCmd">Indicator to Automatically Process The Results as Commands</param>
        /// <param name="boolAutoRetEndTrans">Indicator to End Data Process After Final Results are Done</param>
        /// <returns>Task for Retrieving Data Process Result</returns>
        private async Task SendReceiverDataProcessSetup(int nTransID,
                                                        int nNewRespID,
                                                        float fSetAutoRetLimitInMillis,
                                                        bool boolSetAutoRetProcessCmd,
                                                        bool boolSetAutoRetEndTrans,
                                                        CancellationTokenSource ctsNewTaskCloser) {

                await Task.Run(() => SendReceiverDataProcess(nTransID,
                                                             nNewRespID,
                                                             fSetAutoRetLimitInMillis,
                                                             boolSetAutoRetProcessCmd,
                                                             boolSetAutoRetEndTrans, 
                                                             ctsNewTaskCloser.Token));
        }

        /// <summary>
        ///     Processes Communications from Server
        /// </summary>
        private void Communicate() {

            string strDebugMsg = "";/* Debug Message */
            CancellationToken ctClose = ctsCloser.Token;
                                    /* Control for Closing Tasks */

            while (IsConnected()) {

                ClientCommunicate();
                GetLogError();
                GetDisplayError();

                if (DebugMode()) {

                   if ((strDebugMsg = Debug()) != "") {

                        Log(strDebugMsg);
                   }
                   else if (qstrLogFileMsgWaiting.Count > 0) {

                        Log();
                   }
                }

                if (AutoRetProcessCmd && !RunAutoDirectMsgByDesign()) {
                
                    GetDirectMsgNext(true);
                }

                RunAutoRetCleanup();

                Task.Delay(1, ctClose);
            }
        }

        /// <summary>
        ///     Start Data Process
        /// </summary>
        /// <param name="nNewTransID">New Transaction ID</param>
        /// <param name="strDataDesign">Designation of Data Source Being Called</param>
        /// <returns>True If Data Process was Setup and Started, Else False</returns>
        public bool StartDataProcess(int nNewTransID, string strDataDesign) {

            bool boolStarted = false;
                                    /* Indicator That Storage was Started */
          
            if (!ValidateTransID(nNewTransID)) {

                dictSendStorage["DATAPROCESS"].Add(nNewTransID, 
                                                   new ServerMsg { DESIGNATION = strDataDesign,
                                                                   PARAMS = new Dictionary<string, object>() });
                boolStarted = true;
            }

            return boolStarted;

        }

        /// <summary>
        ///     Adds Variable Updates for Direct Client Message to Existing Stored Information for Sending to Server Later
        ///     If an Unsent Message Exists and the Designation is not that of the Existing Message, Addition will Fail
        /// </summary>
        /// <param name="strRegObjDesign">Designation of New or Exising Client Message Being Built</param>
        /// <param name="strVarName">Name of Client Variable Being Added to Message for Update</param>
        /// <param name="objVarValue">Value to Update Client Variable That is Being Added to Message</param>
        /// <returns>Indicator That Message was Stored</returns>
        public bool DirectClientMsgAddVar(string strRegObjDesign, string strVarName, object objVarValue) {

            bool boolStored = false;/* Indicator That Update was Stored */

            if (cmDirectMsgSend.DESIGNATION != "") {

                if (cmDirectMsgSend.DESIGNATION == strRegObjDesign) {

                    cmDirectMsgSend.VARUPDATES.Add(new ClientMsgVarUpdates() { NAME = strVarName, VALUE = objVarValue });
                    boolStored = true;
                }
            }
            else {

                cmDirectMsgSend.DESIGNATION = strRegObjDesign;
                cmDirectMsgSend.VARUPDATES.Add(new ClientMsgVarUpdates() { NAME = strVarName, VALUE = objVarValue });
                boolStored = true;
            }

            return boolStored;
        }

        /// <summary>
        ///     Adds Function Call Updates for Direct Client Message to Existing Stored Information for Sending to Server Later
        ///     If an Unsent Message Exists and the Designation is not that of the Existing Message, Addition will Fail
        /// </summary>
        /// <param name="strRegObjDesign">Designation of New or Exising Client Message Being Built</param>
        /// <param name="strFuncName">Name of Client Function to be Called Added to Message for Update</param>
        /// <param name="aobjParams">Optional List of Values to Pass as Parameters to Client Function to be Called Added to Message for Update</param>
        /// <returns>Indicator That Message was Stored</returns>
        public bool DirectClientMsgAddFuncCall(string strRegObjDesign, string strFuncName, object[] aobjParams = null) {

            bool boolStored = false;/* Indicator That Update was Stored */

            if (aobjParams == null) { 
            
                aobjParams = new object[0];
            }

            if (cmDirectMsgSend.DESIGNATION != "") {

                if (cmDirectMsgSend.DESIGNATION == strRegObjDesign) {

                    cmDirectMsgSend.FUNCCALLS.Add(new ClientMsgFunCalls() { NAME = strFuncName, PARAMS = aobjParams });
                    boolStored = true;
                }
            }
            else {
                
                cmDirectMsgSend.DESIGNATION = strRegObjDesign;
                cmDirectMsgSend.FUNCCALLS.Add(new ClientMsgFunCalls() { NAME = strFuncName, PARAMS = aobjParams });
                boolStored = true;
            }

            return boolStored;
        }

        /// <summary>
        ///     Adds Function Call Updates for Direct Client Message to Existing Stored Information for Sending to Server Later
        ///     If an Unsent Message Exists and the Designation is not that of the Existing Message, Addition will Fail
        /// </summary>
        /// <param name="strRegObjDesign">Designation of New or Exising Client Message Being Built</param>
        /// <param name="strFuncName">Name of Client Function to be Called Added to Message for Update</param>
        /// <param name="objParam">Value to Pass as Parameters to Client Function to be Called Added to Message for Update</param>
        /// <returns>Indicator That Message was Stored</returns>
        public bool DirectClientMsgAddFuncCall(string strRegObjDesign, string strFuncName, object objParam) {

            return DirectClientMsgAddFuncCall(strRegObjDesign, strFuncName, new List<object> { objParam }.ToArray());
        }


        /// <summary>
        ///     Stored Data Process Parameters By Transaction ID
        /// </summary>
        /// <param name="nTransID">Transaction ID</param>
        /// <param name="strParamName">Parameter Name to Send to Data Process</param>
        /// <param name="objParamValue">Parameter Value to Send to Data Process</param>
        /// <returns>True If Parameters Were Added to Data Process to be Called, Else False</returns>
        public bool AddDataProcessParams(int nTransID, string strParamName, object objParamValue) {

            bool boolStoraged = false;  
                                    /* Indicator That Parameter was Stored */
              
            if (dictSendStorage["DATAPROCESS"].ContainsKey(nTransID)) {

                if (objParamValue.GetType() == typeof(bool)) {

                    if ((bool)objParamValue) {

                        objParamValue = "1";
                    }
                    else {

                        objParamValue = "0";
                    }
                }
                else if (objParamValue.GetType() != typeof(string)) {

                    objParamValue = objParamValue.ToString();
                }

                dictSendStorage["DATAPROCESS"][nTransID].PARAMS.Add(strParamName, objParamValue);
                boolStoraged = true;
                  
            }

            return boolStoraged;
        }

        /// <summary>
        ///     Add a Function to a Data Map
        /// </summary>
        /// <param name="strDesign">Designation for the Data Map</param>
        /// <param name="objSource">Source Object for Function for Data Map</param>
        /// <param name="strSetFuncName">Function for the Data Map</param>
        /// <param name="strSetDataProcessDesign">Designation of the Server Data Process to Run</param>
        /// <param name="strSetDataParamName">Parameter Name for the Value in Server Data Process to Replace</param>
        public void AddDataMapFunc(string strDesign,
                                   object objSource,
                                   string strSetFuncName,
                                   string strSetDataProcessDesign,
                                   string strSetDataParamName) {
            
            Type tySourceClass = objSource.GetType();
                                    /* Class Type of Source Object */
            MethodInfo miFuncSelect;/* Selected Function to Update */

            ClearDataMap(strDesign);

            if ((miFuncSelect = tySourceClass.GetMethod(strSetFuncName, METHODPERMISSIONTYPES)) != null) {

                dictDataMaps.Add(strDesign, new DataMap() {

                    nType = 0,
                    rciObj = new RegClassInfo() {

                        objReg = objSource,
                        tyClass = tySourceClass
                    },
                    strFuncVarName = strSetFuncName,
                    strDataProcessDesign = strSetDataProcessDesign,
                    strParam = strSetDataParamName,
                    strValue = null,
                    miFuncInvoke = miFuncSelect
                });
            }
            else {

                Log("Setting up data map for function, designation: '" + strDesign + "', failed on finding the function, '" +
                    strSetFuncName + "', accessible within object.", true);
            }
        }

        /// <summary>
        ///     Add a Variable to a Data Map
        /// </summary>
        /// <param name="strDesign">Designation for the Data Map</param>
        /// <param name="objSource">Source Object for Variable for Data Map</param>
        /// <param name="strSetVarName">Variable for the Data Map</param>
        /// <param name="strSetDataProcessDesign">Designation of the Server Data Process to Run</param>
        /// <param name="strSetDataParamName">Parameter Name for the Value in Server Data Process to Replace</param>
        public void AddDataMapVar(string strDesign,
                                   object objSource,
                                   string strSetVarName,
                                   string strSetDataProcessDesign,
                                   string strSetDataParamName) {

            ClearDataMap(strDesign);

            dictDataMaps.Add(strDesign, new DataMap() {

                nType = 1,
                rciObj = new RegClassInfo() {

                    objReg = objSource,
                    tyClass = objSource.GetType()
                },
                strFuncVarName = strSetVarName,
                strDataProcessDesign = strSetDataProcessDesign,
                strParam = strSetDataParamName,
                strValue = null,
                miFuncInvoke = null
            });;
        }

        /// <summary>
        ///     Set the Function to be Called When a Client Message is Received Based on its Designation. 
        ///     If no Designation Set, It's Executed After Receipt of Any Client Message
        /// </summary>
        /// <param name="objDest">Object Belonging to Which the Function to Run</param>
        /// <param name="strSetFuncName">Name of Function to Run</param>
        /// <param name="strSetDesign">
        ///     Optional Name of Designation of the Client Message to Run Upon Message Receipt. 
        ///     If Not Set, Function Executed Upon Receipt of Any Client Message
        /// </param>
        /// <returns>Indicator That Direct Client Message Return Function was Set</returns>
        public bool SetDirectClientMsgFuncs(object objDest, string strSetFuncName, string strSetDesign = null) {

            bool boolSet = false;   /* Indicator That Return Function was Stored */

            if (objDest != null &&
                strSetFuncName != "") {

                ltsfiDirectMsgSendFuncs.Add(new SendFuncInfo() {

                    rciObj = new RegClassInfo() {

                        objReg = objDest,
                        tyClass = objDest.GetType()
                    },
                    strFuncName = strSetFuncName,
                    strDesign = strSetDesign
                });
            }

            return boolSet;
        }

        /// <summary>
        ///     Set HTTP Response Functions
        /// </summary>
        /// <param name="nTransID">HTTP Transmission ID</param>
        /// <param name="objDestination">Object of Function to Call with HTTP Response</param>
        /// <param name="strFuncName">Function to Call with HTTP Response</param>
        /// <returns>Indicator That HTTP Response was Set</returns>
        public bool SetHTTPResponseFuncs(int nTransID, object objDestination, string strFuncName) {

            bool boolSet = false;   // Indicator That Parameter was Stored
             
            if (objDestination != null && strFuncName != "") {

                if (!dictsfiHTTPSendFuncs.ContainsKey(nTransID)) {

                    dictsfiHTTPSendFuncs.Add(nTransID, new List<SendFuncInfo>());
                }

                dictsfiHTTPSendFuncs[nTransID].Add(new SendFuncInfo { rciObj = new RegClassInfo { objReg = objDestination,
                                                                                                  tyClass = objDestination.GetType() },
                                                                      strFuncName = strFuncName,
                                                                      strDesign = ""});
            }

            return boolSet;
        }

        /// <summary>
        ///     Set the Function to be Called When a Client Message is Received Based on its Designation. 
        ///     If no Designation Set, It's Executed After Receipt of Any Client Message
        /// </summary>
        /// <param name="objDest">Object Belonging to Which the Function to Run</param>
        /// <param name="strSetFuncName">Name of Function to Run</param>
        /// <param name="strSetDesign">
        ///     Optional Name of Designation of the Client Message to Run Upon Message Receipt. 
        ///     If Not Set, Function Executed Upon Receipt of Any Client Message
        /// </param>
        /// <returns>Indicator That Direct Client Message Return Function was Set</returns>
        public bool SetDataProcessResponseFuncs(int nTransID, object objDest, string strSetFuncName) {

            bool boolSet = false;   /* Indicator That Return Function was Stored */

            if (objDest != null &&
                strSetFuncName != "") {

                if (!dictSendFuncs["DATAPROCESS"].ContainsKey(nTransID)) {

                    dictSendFuncs["DATAPROCESS"].Add(nTransID, new List<SendFuncInfo>());
                }

                dictSendFuncs["DATAPROCESS"][nTransID].Add(new SendFuncInfo() {

                    rciObj = new RegClassInfo() {

                        objReg = objDest,
                        tyClass = objDest.GetType()
                    },
                    strFuncName = strSetFuncName,
                    strDesign = null
                });

                boolSet = true;
            }

            return boolSet;
        }

        /// <summary>
        ///     Sets Data Mapped Variable, and Processes Data Map
        /// </summary>
        /// <typeparam name="T">Value Type for Data Map Variable</typeparam>
        /// <param name="strDesign">Designation for Data Map</param>
        /// <param name="objValue">Value to Set Variable of Data Map</param>
        public void SetDataMapVar<T>(string strDesign, T objValue) {

            // DataMap dmSelect = dictDataMaps[strDesign];       
                                    /* Selected Data Map */

            if (dictDataMaps.ContainsKey(strDesign)) {

                DataMap dmSelect = dictDataMaps[strDesign];       
                                    /* Selected Data Map */

                if (dmSelect.nType == 1) {

                    dmSelect.rciObj.tyClass.GetField(dmSelect.strFuncVarName, METHODPERMISSIONTYPES)
                                           .SetValue(dmSelect.rciObj.objReg, objValue);
                    SendDataMapMsg(strDesign, objValue);
                }
            }
        }

        /// <summary>
        ///     Sending Data Map Nessage
        /// </summary>
        /// <param name="strDesign">Data Process Designation for Use with Data Map</param>
        /// <param name="objValue">Value for Sending</param>
        private void SendDataMapMsg(string strDesign, object objValue) {

//            int nMsgID = GetUniqueID();
                                    /* Transmision ID for Data Map */
            DataMap dmSending = dictDataMaps[strDesign];
                                    /* Information on Data Map */
            string strSendValue = "";
                                    /* Holder for Value for Checking to Sendt */

            if (objValue != null) {

                strSendValue = objValue.ToString();
            }

            if (dmSending.strValue != strSendValue) {

                int nMsgID = GetUniqueID();

                if (StartDataProcess(nMsgID, dmSending.strDataProcessDesign)) {

                    if (AddDataProcessParams(nMsgID, dmSending.strParam, strSendValue)) {

                        if (SendDataProcess(nMsgID, GetUniqueID())) {

                            dmSending.strValue = strSendValue;
                        }
                        else {

                            Log("Processing data map, designation: '" + strDesign + "', send failed.", true);
                        }
                    }
                    else {

                        Log("Processing data map, designation: '" + strDesign + "', adding parameter failed.", true);
                    }
                }
                else {

                    Log("Processing data map, designation: '" + strDesign + "', starting failed.", true);
                }
            }
        }

        /// <summary>
        ///     Sends Direct Client Message with Optional Designation to Server Before Removing it from Storage
        /// </summary>
        /// <param name="strMsgDesign">Optional Designation for Message on the Server Side</param>
        /// <param name="boolSendServer">Indicator to Send Message to Connected Server, Defaults to True</param>
        /// <param name="boolSendPeerToPeer">Indicator to Send Message to Connected "Peer-to-Peer" Clients</param>
        /// <returns>Indicator That Message was Sent</returns>
        public bool SendDirectClientMsg(string strMsgDesign = "", bool boolSendServer = true, bool boolSendPeerToPeer = false) {

            bool boolSend = false;  /* Indicator That Message was Sent */

            /* TODO - Implement Peer-To-Peer */

            if (cmDirectMsgSend.VARUPDATES.Count > 0 || cmDirectMsgSend.FUNCCALLS.Count > 0) {

                SendDirectRawMsg(JsonConvert.SerializeObject(new List<ClientMsg> { cmDirectMsgSend }), strMsgDesign, boolSendServer, boolSendPeerToPeer);
                ClearDirectClientMsgDesign();
                boolSend = true;
            }

            return boolSend;
        }

        /// <summary>
        ///     Sends Direct Raw Message with Optional Designation to Server or "Peer To Peer" Clients or Both or Neither
        /// </summary>
        /// <param name="strMsg">Message to Send</param>
        /// <param name="strMsgDesign">Optional Server Designation for Message</param>
        /// <param name="boolSendServer">Indicator to Send Message to Connected Server, Defaults to True</param>
        /// <param name="boolSendPeerToPeer">Indicator Send Message to Connected "Peer-to-Peer" Clients</param>
        public void SendDirectRawMsg(string strMsg, string strMsgDesign = "", bool boolSendServer = true, bool boolSendPeerToPeer = false) { 
              
            if (boolSendServer) { 
                   
                if (IsConnected()) { 
                    
                    if (strMsgDesign == "") { 
                             
                        ClientSendDirectMsg(strMsg);
                    }
                    else {  
                             
                        ClientSendDirectMsgWithDesign(strMsg, strMsgDesign);
                    }
                }
                else { 

                    Log("During sending raw message, can not send due to not being connected.");
                }
            }
              
            if (boolSendPeerToPeer && ClientHasPeerToPeerClients()) { 
                    
                if (strMsgDesign == "") { 
                        
                    ClientSendDirectMsgPeerToPeer(strMsg);
                }  
                else {  
                        
                    ClientSendDirectMsgPeerToPeerWithDesign(strMsg, strMsgDesign);
                }
            }
        }

        /// <summary>
        ///     Sends Stored HTTP Transmission Message Associated with Transmission ID and Response ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="nNewRespID">Response ID for Transmission</param>
        /// <param name="boolAutoRetrieval">Indicator to Automatically Retrieve Result of Data Process, Defaults to True</param>
        /// <returns>True If Data Process was Sent, Else False</returns>
        public bool SendHTTP(int nTransID, int nNewRespID, bool boolAutoRetrieval = true) {

//            DateTime tmStart = DateTime.Now;
                                    /* Start Time of Execution */
            bool boolSend = false;  // Indicator That Message was Sent

            if (dictSendStorage["HTTP"].ContainsKey(nTransID)) {

                ClientSendHTTP(nTransID, nNewRespID);
                boolSend = true;

                if (boolAutoRetrieval) {

                    lock (dictSendHTTPReceivers) {

                        if (!dictSendHTTPReceivers.ContainsKey(nTransID)) {

                            dictSendHTTPReceivers.Add(nTransID, new Dictionary<int, CancellationTokenSource>());
                        }

                        if (!dictSendHTTPReceivers[nTransID].ContainsKey(nNewRespID)) {

                            dictSendHTTPReceivers[nTransID].Add(nNewRespID, new CancellationTokenSource());
                        }
                        else {

                            Log("During setting up of HTTP transmission auto retrieval for transaction ID, " + nTransID + ", and response ID, " + nNewRespID +
                                ", auto retrieval was already running. Waiting until completion before setting up again");

                            DateTime tmStart = DateTime.Now;

                            while (IsConnected() && DateTime.Now.Subtract(tmStart).TotalMilliseconds < fAutoRetLimitInMillis) {
                
                                Thread.Sleep(1);
                            }

                            if (dictSendHTTPReceivers[nTransID].ContainsKey(nNewRespID)) {

                                dictSendHTTPReceivers[nTransID].Remove(nNewRespID);
                            }
                        }

                        SendReceiverHTTPSetup(nTransID,
                                              nNewRespID,
                                              fAutoRetLimitInMillis,
                                              boolAutoRetProcessCmd,
                                              boolAutoRetEndTrans,
                                              dictSendHTTPReceivers[nTransID][nNewRespID]);
                    }
                }
            }

              return boolSend;
        }

        /// <summary>
        ///     Send Data Process Information to Server
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="nNewRespID">Response ID for Transmission</param>
        /// <param name="boolAsync">Indicator to Run Retrieval as Asynchronized Process, Defaults to True</param>
        /// <param name="boolAutoRetrieval">Indicator to Automatically Retrieve Result of Data Process, Defaults to True</param>
        /// <returns>True If Data Process was Sent, Else False</returns>
        public bool SendDataProcess(int nTransID, int nNewRespID, bool boolAsync = true, bool boolAutoRetrieval = true) {

            string strDesignation = "";
                                    /* Selected Designation */
            bool boolSend = false;  /* Indicator That Message was Sent */
//            CancellationTokenSource ctsNewTaskControl = new CancellationTokenSource();
                                    /* Cancel Token for New Task */

            if (dictSendStorage["DATAPROCESS"].ContainsKey(nTransID)) {

                strDesignation = dictSendStorage["DATAPROCESS"][nTransID].DESIGNATION;

                if (dictSendStorage["DATAPROCESS"][nTransID].PARAMS.Count <= 0) {

                    if (IsConnected()) {

                        ClientRegisterDataProcess(nTransID, strDesignation);
                    }
                    else {

                        Log("During sending data process, can not register due to not being connected.");
                    }
                }
                else if (IsConnected()) {

                    foreach (KeyValuePair<string, object> kvpParams in dictSendStorage["DATAPROCESS"][nTransID].PARAMS) {

                        ClientRegisterDataProcessWithParams(nTransID, strDesignation, kvpParams.Key, kvpParams.Value.ToString());
                    }
                }
                else {

                    Log("During sending data process, can not register with parameters due to not being connected.");
                }

                if (IsConnected()) {

                    ClientSendDataProcess(nTransID, nNewRespID, strDesignation, boolAsync);
                    boolSend = true;
                }
                else {

                    Log("During sending data process, can not send due to not being connected.");
                }

                if (boolAutoRetrieval) {

                    if (boolAsync) {

                        lock (dictSendDataProcessReceivers) { 

                            if (!dictSendDataProcessReceivers.ContainsKey(nTransID)) {

                                dictSendDataProcessReceivers.Add(nTransID, new Dictionary<int, CancellationTokenSource>());
                            }
           
                            if (!dictSendDataProcessReceivers[nTransID].ContainsKey(nNewRespID)) {

                                    CancellationTokenSource ctsNewTaskCloser = new CancellationTokenSource();

                                    dictSendDataProcessReceivers[nTransID].Add(nNewRespID, ctsNewTaskCloser);

                                    SendReceiverDataProcessSetup(nTransID,
                                                                 nNewRespID,
                                                                 fAutoRetLimitInMillis,
                                                                 boolAutoRetProcessCmd,
                                                                 boolAutoRetEndTrans,
                                                                 ctsNewTaskCloser);
                            }
                            else {

                                Log("During setting up of data process auto retrieval for transaction ID, " + nTransID + ", and response ID, " + nNewRespID +
                                    ", auto retrieval was already running.");
                            }
                        }
                    }
                    else { 

                        lock (dictSendDataProcessStartTime) { 

                            if (!dictSendDataProcessStartTime.ContainsKey(nTransID)) {

                                dictSendDataProcessStartTime.Add(nTransID, new Dictionary<int, DateTime>());
                            }
           
                            if (!dictSendDataProcessStartTime[nTransID].ContainsKey(nNewRespID)) {

                                dictSendDataProcessStartTime[nTransID].Add(nNewRespID, DateTime.Now);
                            
                                if (tskAsyncDataProcessor == null) {

                                    tskAsyncDataProcessor = AsyncDataProcessorSetup();
                                }
                            }
                            else {

                                Log("During setting up of data process auto retrieval for transaction ID, " + nTransID + ", and response ID, " + nNewRespID +
                                    ", auto retrieval was already running.");
                            }
                        }
                    }
                }
            }

            return boolSend;
        }

        /// <summary>
        ///     Processes Returned Results from HTTP Transmission
        /// </summary>
        /// <param name="nTransID">HTTP Transaction ID</param>
        /// <param name="nRespID">HTTP Response ID for the Transaction</param>
        /// <param name="fAutoRetLimitInMillis">The Amount of Time in Milliseconds to Wait for a Response</param>
        /// <param name="boolAutoRetProcessCmd">Indicator to Automatically Process The Results as Commands</param>
        /// <param name="boolAutoRetEndTrans">Indicator to End Data Process After Final Results are Done</param>
        /// <param name="ctsProcessClose">Tasks' Closing Control Token</param>
        private void SendReceiverHTTP(int nTransID,
                                      int nRespID,
                                      float fAutoRetLimitInMillis,
                                      bool boolAutoRetProcessCmd,
                                      bool boolAutoRetEndTrans,
                                      CancellationToken ctsProcessClose) {

            bool boolNotFinished = true;
                                    /* Indicator That Login Check is not Finished */
            DateTime tmStart = DateTime.Now;
                                    /* Start Time of Execution */
       
            while (boolNotFinished && IsConnected() && DateTime.Now.Subtract(tmStart).TotalMilliseconds < fAutoRetLimitInMillis) {
                
                if (GetHTTPResponse(nTransID, nRespID, boolAutoRetProcessCmd) == "") {

                    Task.Delay(1, ctsProcessClose);
                }
                else {

                    if (boolAutoRetEndTrans) {

                        TranClose(nTransID);
                    }

                    boolNotFinished = false;
                }
            }

            lock (dictSendHTTPReceivers) { 

                if (dictSendHTTPReceivers.ContainsKey(nTransID) && dictSendHTTPReceivers[nTransID].ContainsKey(nRespID)) {

                    dictSendHTTPReceivers[nTransID].Remove(nRespID);
                }
            }
        }

        /// <summary>
        ///     Processes Returned Results from Data Processes
        /// </summary>
        /// <param name="nTransID">Data Process' Transaction ID</param>
        /// <param name="nRespID">Data Process' Response ID for the Transaction</param>
        /// <param name="fAutoRetLimitInMillis">The Amount of Time in Milliseconds to Wait for a Response</param>
        /// <param name="boolAutoRetProcessCmd">Indicator to Automatically Process The Results as Commands</param>
        /// <param name="boolAutoRetEndTrans">Indicator to End Data Process After Final Results are Done</param>
        /// <param name="ctsProcessClose">Tasks' Closing Control Token</param>
        private void SendReceiverDataProcess(int nTransID,
                                             int nRespID,
                                             float fAutoRetLimitInMillis,
                                             bool boolAutoRetProcessCmd,
                                             bool boolAutoRetEndTrans,
                                             CancellationToken ctsProcessClose) {

            bool boolNotFinished = true;
                                    /* Indicator That Login Check is not Finished */
            DateTime tmStart = DateTime.Now;
                                    /* Start Time of Execution */
       
            while (boolNotFinished && IsConnected() && DateTime.Now.Subtract(tmStart).TotalMilliseconds < fAutoRetLimitInMillis) {
                
                if (GetDataProcessResponse(nTransID, nRespID, boolAutoRetProcessCmd, boolAutoRetEndTrans) == "") {

                    Task.Delay(1, ctsProcessClose);
                }
                else { 
                        
                    boolNotFinished = false;
                }
            }

            lock (dictSendDataProcessReceivers) { 

                if (boolAutoRetEndTrans && dictSendDataProcessReceivers.ContainsKey(nTransID)) {

                    foreach (CancellationTokenSource ctsSelect in dictSendDataProcessReceivers[nTransID].Values) {

                        if (ctsSelect != null) { 

                            ctsSelect.Cancel();
                        }
                    }

                    dictSendDataProcessReceivers.Remove(nTransID);
                }
            }
        }

        /// <summary>
        ///     Process All Data Process Responses
        /// </summary>
        private void DoDataProcessResponses() {

            float fSetAutoRetLimitInMillis = AutoRetLimitInMillis;
                                    /* Limit of Data Process Check */
            bool boolSetAutoRetProcessCmd = AutoRetProcessCmd;
                                    /* Indicator to Process Returned Commands */
            bool boolSetAutoRetEndTrans = AutoRetEndTrans;
                                    /* Indicator to End Transaction When Completed */
            Dictionary<int, Dictionary<int, DateTime>> dictWaitingRespToProcess = new Dictionary<int, Dictionary<int, DateTime>>();
                                    /* List of Data Processes Response Messages to Process with Start Time */
            Dictionary<int, List<int>> dictRemoveResps = new Dictionary<int, List<int>>();
                                    /* List of Transmissions' Responses To Remove */
            bool boolRestart = false;
                                    /* Indicator to Restart Check When Response is Not Found */
            int nTranID = 0;        /* Selected Transaction ID */

            while (IsConnected()) {

                lock (dictSendDataProcessStartTime) {

                    foreach (KeyValuePair<int, Dictionary<int, DateTime>> kvpTrans in dictSendDataProcessStartTime) { 

                        nTranID = kvpTrans.Key;

                        if (!dictWaitingRespToProcess.ContainsKey(nTranID)) { 
                        
                            dictWaitingRespToProcess.Add(nTranID, kvpTrans.Value);
                        }
                        else { 
                        
                            foreach (KeyValuePair<int, DateTime> kvpResp in kvpTrans.Value) { 

                                if (!dictWaitingRespToProcess[nTranID].ContainsKey(kvpResp.Key)) { 
                                    
                                    dictWaitingRespToProcess[nTranID].Add(kvpResp.Key, kvpResp.Value);
                                }
                                else { 
                                 
                                    RevCommProcessor.Log("During registering data processes for syncrohonous processing for transaction ID, " + nTranID + ", and response ID, " + kvpResp.Key +
                                                         ", was already registered.");
                                }
                            }
                        }
                    }

                    dictSendDataProcessStartTime.Clear();
                }

                foreach (KeyValuePair<int, Dictionary<int, DateTime>> kvpTrans in dictWaitingRespToProcess) {

                    nTranID = kvpTrans.Key;

                    foreach (KeyValuePair<int, DateTime> kvpResp in kvpTrans.Value) {

                        if ((DateTime.Now.Subtract(kvpResp.Value).TotalMilliseconds < fAutoRetLimitInMillis) && 
                            (boolRestart = (GetDataProcessResponse(nTranID, kvpResp.Key, boolAutoRetProcessCmd, boolAutoRetEndTrans) == ""))) {

                            break;
                        }
                        else { 
                            
                            if (!dictRemoveResps.ContainsKey(nTranID)) {

                                dictRemoveResps.Add(nTranID, new List<int>());
                            }

                            dictRemoveResps[nTranID].Add(kvpResp.Key);
                        }
                    }

                    if (boolRestart) {

                        break;
                    }
                }

                foreach (KeyValuePair<int, List<int>> kvpRemoveIDs in dictRemoveResps) {

                    nTranID = kvpRemoveIDs.Key;

                    foreach (int nRespID in kvpRemoveIDs.Value) {

                        dictWaitingRespToProcess[nTranID].Remove(nRespID);
                    }

                    if (boolSetAutoRetEndTrans && dictWaitingRespToProcess[nTranID].Count <= 0) { 
                        
                        dictWaitingRespToProcess.Remove(nTranID);
                    }
                }

                dictRemoveResps.Clear();

                Task.Delay(1);
            }
        }

        /// <summary>
        ///     Changes Designation of Existing Direct Client Message
        /// </summary>
        /// <param name="strRegObjDesign">Designation to Change to</param>
        /// <returns>Indicator That Designation was Changed</returns>
        public bool ChangeDirectClientMsgDesign(string strRegObjDesign) {

            bool boolChanged = false;  
                                    /* Indicator That Update was Stored */
                            
            if (cmDirectMsgSend.VARUPDATES.Count > 0 || cmDirectMsgSend.FUNCCALLS.Count > 0) {

                cmDirectMsgSend.DESIGNATION = strRegObjDesign;
                boolChanged = true;
            }

            return boolChanged;
        }

        /// <summary>
        ///     Clears Existing Direct Client Message
        /// </summary>
        public void ClearDirectClientMsgDesign() {

            cmDirectMsgSend = new ClientMsg() { DESIGNATION = "",
                                                VARUPDATES = new List<ClientMsgVarUpdates>(),
                                                FUNCCALLS = new List<ClientMsgFunCalls>() };
        }

        /// <summary>
        ///     Gets Next Direct Message of a Designation
        /// </summary>
        /// <param name="strDesign">Designation of Next Direct Message to Retreive</param>
        /// <param name="boolProcessCmd">Optional Indicator to Process Message Registered Object Update</param>
        /// <returns>Retrieved Direct Message</returns> 
        public string GetDirectMsg(string strDesign, bool boolProcessCmd = false) {
                        
            int nMsgLen = ClientCheckDirectMsgsWithDesignReady(strDesign);
                                    /* Length of Return Message */
            string strRetMsg = "";  /* Direct Message to Return */

            if (nMsgLen > 0) {

                strRetMsg = GetClientReturnValue(ClientGetDirectMsg(strDesign));

                if (boolProcessCmd && nMsgLen > 0 && strRetMsg != "") {

                    ProcessIncoming(strRetMsg);
                }

                if (nMsgLen > 0) {

                    foreach (SendFuncInfo sfiSelect in ltsfiDirectMsgSendFuncs) { 

                        if (sfiSelect.strDesign == strDesign) {

                            RunRetFunc(sfiSelect, strRetMsg);
                        }
                    }
                }
            }

            return strRetMsg;
        }

        /// <summary>
        ///     Gets Next Direct Message
        /// </summary>
        /// <param name="boolProcessCmd">Optional Indicator to Process Message Registered Object Update</param>
        /// <returns>Retrieved Direct Message</returns> 
        public string GetDirectMsgNext(bool boolProcessCmd = false) {
             
            int nMsgLen = ClientCheckDirectMsgsReady();
                                    /* Length of Return Message */
            string strRetMsg = "";  /* Direct Message to Return */

            if (nMsgLen > 0) {

                strRetMsg = GetClientReturnValue(ClientGetDirectMsgNext());

                if (boolProcessCmd && nMsgLen > 0 && strRetMsg != "") {
                    
                    ProcessIncoming(strRetMsg);
                }

                if (nMsgLen > 0) {

                    foreach (SendFuncInfo sfiSelect in ltsfiDirectMsgSendFuncs) { 

                        if (sfiSelect.strDesign == "") {

                            RunRetFunc(sfiSelect, strRetMsg);
                        }
                    }
                }
            }
              
            return strRetMsg;
        }

        /// <summary>
        ///     Gets Stored HTTP Transmission Message Associated with Transmission ID and Response ID
        /// </summary>
        /// <param name="nTransID">Data Process' Transaction ID</param>
        /// <param name="nRespID">Data Process' Response ID for the Transaction</param>
        /// <param name="boolProcessCmd">Optional Indicator to Process Message Registered Object Update</param>
        /// <returns>Retrieved Direct Message</returns> 
        public string GetHTTPResponse(int nTransID, int nRespID, bool boolProcessCmd = false) {

            int nActualMsgLen = ClientCheckHTTPResponse(nTransID, nRespID);
                                    /* Length of Waiting Message */
            string strRetMsg = "";  /* Direct Message to Return */
              
            if (nActualMsgLen > 0) {

                strRetMsg = GetClientReturnValue(ClientGetHTTPResponse(nTransID, nRespID));

                if (boolProcessCmd && strRetMsg != "") {

                    ProcessIncoming(strRetMsg);
                }

                if (dictsfiHTTPSendFuncs.ContainsKey(nTransID) && nActualMsgLen > 0) {

                    foreach (SendFuncInfo sfiSelect in dictsfiHTTPSendFuncs[nTransID]) {

                        RunRetFunc(sfiSelect, strRetMsg);
                    }
                }
            }
              
            return strRetMsg;
        }

        /// <summary>
        ///     Gets Stored Data Process Message Associated with Transmission ID and Response ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="nRespID">Transmission's Response ID</param>
        /// <param name="boolProcessCmd">Optional Indicator to Process the Response as a Client Message. Defaults to False</param>
        /// <param name="boolDeleteTrans">Optional Indicator to Delete the Transmission at the End of Processing. Defaults to False</param>
        /// <returns>Returned Message from Data Process</returns>
        public string GetDataProcessResponse(int nTransID,
                                             int nRespID,
                                             bool boolProcessCmd = false,
                                             bool boolDeleteTrans = false) {

            int nActualMsgLen = ClientCheckDataProcessResponse(nTransID, nRespID);
                                    /* Actual Length of Waiting Message */
            string strRetMsg = "";  /* Returned Message */

            if (nActualMsgLen > 0) {

                strRetMsg = GetClientReturnValue(ClientGetDataProcessResponse(nTransID, nRespID));

                if (boolProcessCmd) {

                    ProcessIncoming(strRetMsg);
                }

                if (dictSendFuncs["DATAPROCESS"].ContainsKey(nTransID)) { 
                  
                    foreach (SendFuncInfo sfiSelect in dictSendFuncs["DATAPROCESS"][nTransID]) {

                        RunRetFunc(sfiSelect, strRetMsg);
                    }
                }

                if (boolDeleteTrans) {

                    if (dictSendStorage["DATAPROCESS"].ContainsKey(nTransID)) { 

                        dictSendStorage["DATAPROCESS"].Remove(nTransID);
                    }

                    if (dictSendFuncs["DATAPROCESS"].ContainsKey(nTransID)) {

                        dictSendFuncs["DATAPROCESS"].Remove(nTransID);
                    }
                }
            }

            return strRetMsg;
        }

        /// <summary>
        ///     Gets Log Errors from Client
        /// </summary>
        public void GetLogError() {

            string strRetMsg = GetClientReturnValue(ClientGetLogError());  
                                        /* Returned Message */

            if (strRetMsg != "") {

                Log(strRetMsg, true);
            }
        }

        /// <summary>
        ///     Get Displayable Errors from Client
        /// </summary>
        public void GetDisplayError() {

            string strRetMsg = GetClientReturnValue(ClientGetDisplayError());  
                                        /* Returned Message */

            if (strRetMsg != "") {

                qstrDisplayErrorMsg.Enqueue(strRetMsg);
            }
        }

        /// <summary>
        ///     Dequeues Oldest Displayable Error
        /// </summary>
        /// <returns>Oldest Displayable Error, If None Exists, Returns Blank String</returns>
        public string DequeueDisplayError() {

            string strErrorMsg = "";

            if (HasDisplayError()) {

                strErrorMsg = qstrDisplayErrorMsg.Dequeue();
            }

            return strErrorMsg;
        }

        /// <summary>
        ///     Indicator If There are Any Displayable Errors Available
        /// </summary>
        /// <returns>True If Any Displayable Errors Exists, Else False</returns>
        public bool HasDisplayError() {

            return qstrDisplayErrorMsg.Count > 0;
        }

        /// <summary>
        ///     Gets Unique ID
        /// </summary>
        /// <returns>ID</returns>
        public int GetUniqueID() {

            Random rndGenerator = new Random();
            int nID = rndGenerator.Next(999999);
                                /* New ID */
                                 
            while (ltnUsedIDs.Contains(nID)) { 

                nID = rndGenerator.Next(999999);
            }

            ltnUsedIDs.Add(nID);

            return nID;
        }

        /// <summary>
        ///     Clears Direct Messages
        /// </summary>
        /// <param name="strDesign">Optional Designation of Messages to Delete, Default Deletes All Direct Messages</param>
        /// <returns></returns>
        public bool ClearDirectMsgs(string strDesign = "") {

            bool boolSuccessful = false;
            // Indicator That Deletion was Successful

            if (strDesign == "") {

                boolSuccessful = ClientClearDirectMsgs();
            }
            else {

                boolSuccessful = ClientClearDirectMsgsWithDesign(strDesign);
            }

            return boolSuccessful;
        }

        /// <summary>
        ///     Removes Data Map and its Setups
        /// </summary>
        /// <param name="strDesign">Designation of Data Map to Remove</param>
        public void ClearDataMap(String strDesign) { 

            if (dictDataMaps.ContainsKey(strDesign)) { 

                dictDataMaps.Remove(strDesign);
            }
        }

        /// <summary>
        ///     Starts a File Download from Server Using its Designation
        /// </summary>
        /// <param name="strFileDesign">Designation of File to Start Download For</param>
        public void FileDownloadStart(string strFileDesign) {

            ClientGetStreamFile(strFileDesign);
        }
        /// <summary>
        ///     Checks If File Download is Completed and Saves it Using its Designation 
        /// </summary>
        /// <param name="strFileDesign">Designation of File to be Downloaded</param>
        /// <param name="strFilePathOverride">Optional Directory Destination can be Set, Else it Downloads to Default Directory</param>
        /// <returns>True If Specified File were Downloaded Successfully, Else False If Failure or Specified File Has Not Completed Downloading<returns>
        public bool FileDownloadFinish(string strFileDesign, string strFilePathOverride = "") {

            bool boolDownloaded = false;
                                    /* Indicator That File or Files were Downloaded */
            int nFileLen = ClientCheckStreamFileReady(strFileDesign);
                                    /* Check If File is Ready for Download, and Get Its Length */
            // int nFilePathLen = 0;/* Length of File's Name and Path */
/*             byte[] abyteMsg = new byte[nFileLen];
                                    /* Returned Message */
/*             StringBuilder sbFilePathMsg = new StringBuilder(nFilePathLength);
                                    /* Returned Message */
 //            string strFileDownloadPath = sbFilePathMsg.ToString();
                                    /* File Path from Downloaded File */
//             string[] aFileDownloadPathList = strFileDownloadPath.Split('\\');
                                    /* List of Peices of Download Path */

            try {

                if (nFileLen > 0) {

                    int nFilePathLen = ClientGetStreamFilePathLength(strFileDesign);

                    if (nFilePathLen > 0) {

                        byte[] abyteMsg = new byte[nFileLen];
                        StringBuilder sbFilePathMsg = new StringBuilder(nFilePathLen);

                        if (ClientCheckStreamFileDownload(strFileDesign, 
                                                          sbFilePathMsg, 
                                                          nFilePathLen, 
                                                          Marshal.UnsafeAddrOfPinnedArrayElement(abyteMsg, 0))) {

                            if (strFilePathOverride == "") {

                                strFilePathOverride = sbFilePathMsg.ToString();
                            }
                            else {

                                string strFileDownloadPath = sbFilePathMsg.ToString();

                                if (strFilePathOverride.IndexOf('\\') != strFilePathOverride.Length - 1) {


                                    strFilePathOverride += "\\";
                                }

                                string[] aFileDownloadPathList = strFileDownloadPath.Split('\\');

                                if (aFileDownloadPathList.Length > 0) {

                                    strFilePathOverride += aFileDownloadPathList[aFileDownloadPathList.Length - 1];
                                }
                            }

                            File.WriteAllBytes(strFilePathOverride, abyteMsg);

                            boolDownloaded = true;
                        }
                    }
                    else {

                        Log("Downloading file, designation: " + strFileDesign + " failed, due to no file path length found.", true);
                    }
                }
            }
            catch(Exception exError) {

                Log("Downloading file, designation: " + strFileDesign + " failed. Message: " + exError.Message, true);
                throw exError;
            }

            return boolDownloaded;
        }

        /// <summary>
        ///     Clear Download File Information from Stream
        /// </summary>
        /// <param name="strFileDesign"></param>
        public void ClearStreamFileDownload(string strFileDesign) {

            ClientClearStreamFileDownload(strFileDesign);
        }

        /// <summary>
        ///     Gets List of Files by Designations That Are Available for Download
        /// </summary>
        /// <returns>List of Designation of Files for Download</returns>
        public List<string> GetAvailableFileList() {
            
            StringBuilder sbMsg = new StringBuilder(BUFFERSIZE); 
                                    /* Returned Message */
            StringBuilder sbRetLen = new StringBuilder(MAXBUFFERSIZEDIGITS);
                                    /* Length for Retrieved Message */
            List<string> ltstrFileList = new List<string>();
                                    /* List of Downloadable File Designation */
              
            sbRetLen.Append(BUFFERSIZE.ToString().PadLeft(MAXBUFFERSIZEDIGITS, '0'));

            ClientGetStreamFileList(sbMsg, sbRetLen);

            if (sbMsg.ToString().Trim() != "") {

                ltstrFileList = JsonConvert.DeserializeObject<DownloadFileList>(sbMsg.ToString().Trim()).STREAMFILELIST;
            }

            return ltstrFileList;
        }

        /// <summary>
        ///     Releases ID
        /// </summary>
        /// <param name="nID">ID to Release</param>
        public void ReleaseID(int nID) { 
        
            if (ltnUsedIDs.Contains(nID)) {

                ltnUsedIDs.Remove(nID);
            }
        }

        /// <summary>
        ///     Disconnects Client from Server and "Peer-to-Peer"
        /// </summary>
        public void Disconnect() {

            ClientDisconnect();

            lock (dictSendDataProcessReceivers) { 

                foreach (KeyValuePair<int, Dictionary<int, CancellationTokenSource>> kvpTrans in dictSendDataProcessReceivers) {

                    foreach (KeyValuePair<int, CancellationTokenSource> kvpResp in kvpTrans.Value) {

                        if (kvpResp.Value != null) { 

                            kvpResp.Value.Cancel();
                        }
                    }
                }
            }

            ctsCloser.Cancel();
            tskCommunicate = null;
            tskAsyncDataProcessor = null;
        }

        /// <summary>
        ///     Register Object
        /// </summary>
        /// <param name="strDesignation">Designation of Object to Register</param>
        /// <param name="tyClass">Class Type of Object to Register</param>
        /// <param name="objRegistrant">Object to Register</param>
        /// <param name="boolIsShared">Indicator That Object is to be Shared Between Clients, Defaults to False</param>
        /// <param name="boolHostOnly">Indicator That Object Interaction Must Occur with Host First, Defaults to False</param>
        public void RegisterObject(string strDesignation, 
                                   Type tyObjClass,
                                   object objRegistrant = null,
                                   bool boolIsShared = false,
                                   bool boolHostOnly = false) {

            if (dictRegObjects.ContainsKey(strDesignation)) {

                dictRegObjects.Remove(strDesignation);
            }

            dictRegObjects.Add(strDesignation, new RegClassInfo { objReg = objRegistrant, tyClass = tyObjClass });

            /* TODO - Do Shared and Host Functionality */
        }

        /// <summary>
        ///     Unregister Object
        /// </summary>
        /// <param name="strDesignation">Designation of Object to Unregister</param>
        public void UnregisterObject(string strDesignation) {

            dictRegObjects.Remove(strDesignation);
        }

        /// <summary>
        ///     Checks If Client is Connected to Server
        /// </summary>
        /// <returns>Indicator That Client is Connected to Server</returns>
        public bool IsConnected() {

            return ClientIsConnected();
        }

        /// <summary>
        ///     Indicator to Process Automated Retrieval Client Messages
        /// </summary>
        /// <returns>True If Processing Automated Retrieval Client Messages</returns>
        public bool AutoRetProcessCmd {

            get {

                return boolAutoRetProcessCmd;
            }

            set {

                boolAutoRetProcessCmd = value;
            }
        }

        /// <summary>
        ///     Sets and Gets for Timeout in Milliseconds of Automated Processes
        /// </summary>
        /// <returns>Timeout in Milliseconds of Automated Processes</returns>
        public float AutoRetLimitInMillis {
            
            get {

                return fAutoRetLimitInMillis;
            }

            set {

                fAutoRetLimitInMillis = value;
            }
        }

        /// <summary>
        ///     Indicator to End Automated Process After Running
        /// </summary>
        /// <returns>True If Ending Automated Process After Running, Else False</returns>
        public bool AutoRetEndTrans {
            
            get {

                return boolAutoRetEndTrans;
            }

            set {

                boolAutoRetEndTrans = value;
            }
        }

        /// <summary>
        ///     Runs Data Map Function, and Processes Data Map
        /// </summary>
        /// <typeparam name="T">Return Type for Data Map Function</typeparam>
        /// <param name="strDesign">Designation for Data Map</param>
        /// <param name="aobjParams">List of Parameters for Data Map Function</param>
        /// <returns>Value from Data Map Function Execution, Else On Failure or Execution, Returns NULL for Nullible Type, 0 for Numeric Types, '\0' for String Types</returns>
        public T RunDataMapFunc<T>(string strDesign, object[] aobjParams) {

//            DataMap dmRun;        /* Data Map to Run */
//            object objRawValue = null;
                                    /* Raw Object Value Returned from Data Map Function Execution */
            T genRetValue = default(T);
                                    /* Type Converted Return Value from Data Map Function Execution */

            if (dictDataMaps.ContainsKey(strDesign) && dictDataMaps[strDesign].nType == 0) {

                try {

                    DataMap dmRun = dictDataMaps[strDesign];

                    object objRawValue = dmRun.miFuncInvoke.Invoke(dmRun.rciObj.objReg,
                                                                   METHODPERMISSIONTYPES | BindingFlags.OptionalParamBinding,
                                                                   null,
                                                                   aobjParams,
                                                                   CultureInfo.InvariantCulture);

                    genRetValue = (T)Convert.ChangeType(objRawValue, typeof(T));

                    SendDataMapMsg(strDesign, objRawValue);
                }
                catch (Exception exError) {

                    Log("During executing data map function for designation: " + strDesign + ".", true, exError);
                }
            }

            return genRetValue;
        }

        /// <summary>
        ///     Sets HTTP Transmission's Destination Page Associated with Transmission ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="strPageURL">URL's Page for HTTP Transmission</param>
        public void SetHTTPProcessPage(int nTransID, string strPageURL) {

            ClientSetHTTPProcessPage(nTransID, strPageURL);
        }

        /// <summary>
        ///     Adds HTTP Transmission's Variable and Value Pair for Sending to Destination Page Associated with Transmission ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="strVariableName">URL Param Name</param>
        /// <param name="strValue">URL Param Value</param>
        public void AddHTTPMsgData(int nTransID, string strVariableName, string strValue) {

            ClientAddHTTPMsgData(nTransID, strVariableName, strValue);
        }

        /// <summary>
        ///     Clears HTTP Transmission's Variable and Value Pairs for Sending to Destination Page Associated with Transmission ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        public void ClearHTTPMsgData(int nTransID) {

            ClientClearHTTPMsgData(nTransID);
        }

        /// <summary>
        ///     Clears HTTP Transmission's Variable and Value Pairs for Sending to Destination Page Associated with Transmission ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="boolUseSSL">Indicator That HTTP URL Uses SSL</param>
        public void UseHTTPSSL(int nTransID, bool boolUseSSL) {

            ClientUseHTTPSSL(nTransID, boolUseSSL);
        }

        /// <summary>
        ///     Close Stream or HTTP Transmission's Associated with Transmission ID
        /// </summary>
        /// <param name="nTransID">Transmission ID</param>
        /// <param name="boolReleaseID">Indicator to Release ID for Reassignment</param>
        /// <returns>Indicator That Transmision was Closed</returns>
        public bool TranClose(int nTransID, bool boolReleaseID = false) {

            bool boolClosed = false;    // Indicator That Transmission was Closed

            if (ValidateTransID(nTransID)) {

                ClientClose(nTransID);

                /* TODO - 'STREAMCLIENT' and 'STREAMRAW' */
                if (dictSendStorage["HTTP"].ContainsKey(nTransID)) {


                    dictSendStorage["HTTP"].Remove(nTransID);

                    if (dictsfiHTTPSendFuncs.ContainsKey(nTransID)) {

                        dictsfiHTTPSendFuncs[nTransID].Clear();
                    }

                    foreach (CancellationTokenSource dictCancel in dictSendHTTPReceivers[nTransID].Values) {

                        dictCancel.Cancel();
                    }

                }

                boolClosed = true;
            }

            if (boolReleaseID && ltnUsedIDs.Contains(nTransID)) {

                ReleaseID(nTransID);
            }

            return boolClosed;
        }


        /// <summary>
        ///     Automated Running of Direct Messages in a List When Received from Server
        /// </summary>
        /// <returns>Indicator That Any of the Automated Designated Messages were Ran</returns>
    public bool RunAutoDirectMsgByDesign() {

            bool boolRunning = ltstrAutoRetDirectMsgDesigns.Count > 0 && AutoRetProcessCmd;

            if (boolRunning) {

                foreach (string strDesign in  ltstrAutoRetDirectMsgDesigns) {

                    GetDirectMsg(strDesign, true);
                }
            }

            return boolRunning;
        }

        /// <summary>
        ///     Automated Removal of HTTP and Data Process Receivers
        /// </summary>
        public void RunAutoRetCleanup() {

            List<int> ltTransIDDeletion = new List<int>();
                                    /* List of Transmission IDs for Deletion */
            bool boolTransDelete = false;
                                    /* Indicator to Delete Currently Selection Transmission ID */

            lock (dictSendHTTPReceivers) { 

                foreach (KeyValuePair<int, Dictionary<int, CancellationTokenSource>> kvpTrans in dictSendHTTPReceivers) {

                    if (kvpTrans.Value.Count > 0) {

                        foreach (KeyValuePair<int, CancellationTokenSource> kvpResp in kvpTrans.Value) {

                            /* If Any of the Transmission's Response's Tasks are not Cancelled, 
                               Do Not Delete Transmission's Receiver */
                            if (!(boolTransDelete = kvpResp.Value.IsCancellationRequested)) {

                                break;
                            }
                        }
                    }
                    else {

                        boolTransDelete = true;
                    }

                    if (boolTransDelete) {

                        ltTransIDDeletion.Add(kvpTrans.Key);
                        boolTransDelete = false;
                    }
                }

                foreach (int nTransID in ltTransIDDeletion) {

                    dictSendHTTPReceivers.Remove(nTransID);
                }
            }

            ltTransIDDeletion = new List<int>();
            boolTransDelete = false;

            lock (dictSendDataProcessReceivers) { 

                foreach (KeyValuePair<int, Dictionary<int, CancellationTokenSource>> kvpTrans in dictSendDataProcessReceivers) {

                    if (kvpTrans.Value.Count > 0) {

                        foreach (KeyValuePair<int, CancellationTokenSource> kvpResp in kvpTrans.Value) {

                            /* If Any of the Transmission's Response's Tasks are not Cancelled, 
                               Do Not Delete Transmission's Receiver */
                            if (kvpResp.Value != null && !(boolTransDelete = kvpResp.Value.IsCancellationRequested)) {

                                break;
                            }
                        }
                    }
                    else {

                        boolTransDelete = true;
                    }

                    if (boolTransDelete) {

                        ltTransIDDeletion.Add(kvpTrans.Key);
                        boolTransDelete = false;
                    }
                }

                foreach (int nTransID in ltTransIDDeletion) {

                    dictSendDataProcessReceivers.Remove(nTransID);
                }
            }
        }

        /// <summary>
        ///     Adds a Direct Message Designation to be Ran Automatically when Received from Server
        /// </summary>
        /// <param name="strDesign">Direct Message Designation to Add</param>
        public void AddAutoRetDirectMsgDesigns(string strDesign) { 
             
            if (strDesign != "" && !ltstrAutoRetDirectMsgDesigns.Contains(strDesign)) {

                ltstrAutoRetDirectMsgDesigns.Add(strDesign);
            }
        }

        /// <summary>
        ///     Removes a Direct Message Designation to be Ran Automatically when Received from Server
        /// </summary>
        /// <param name="strDesign">Direct Message Designation to Remove</param>
        public void RemoveAutoRetDirectMsgDesigns(string strDesign) {

            if (strDesign != "" && ltstrAutoRetDirectMsgDesigns.Contains(strDesign)) {

                ltstrAutoRetDirectMsgDesigns.Add(strDesign);
            }
        }

        /// <summary>
        ///     Set Queue Limit
        /// </summary>
        /// <param name="nNewLimit">New Queue Limit</param>
        public void SetQueueLimit(int nNewLimit) {

            ClientSetQueueLimit(nNewLimit);
        }

        /// <summary>
        ///     Checks If Transaction ID is Valid
        /// </summary>
        /// <param name="nNewTransID">Transaction ID to Check</param>
        /// <returns>True If Transaction ID is in Used and Valid, Else False</returns>
        private bool ValidateTransID(int nNewTransID) {

            return dictSendStorage["STREAMCLIENT"].ContainsKey(nNewTransID) ||
                   dictSendStorage["STREAMRAW"].ContainsKey(nNewTransID) ||
                   dictSendStorage["HTTP"].ContainsKey(nNewTransID) ||
                   dictSendStorage["DATAPROCESS"].ContainsKey(nNewTransID);
        }

        /// <summary>
        /// 	Logs Messages
        /// </summary>
        /// <param name="strLogMsg">Messages to Log</param>
        /// <param name="boolError">Indicator That Message is an Error, Defaults to False</param>
        /// <param name="exSentError">Exception Information</param>
        public static void Log(string strLogMsg = "", bool boolError = false, Exception exSentError = null) {
		
			DateTime dtNow = DateTime.Now;	
									/* Current Time */
			string strLogFilePath = "Logs/",
				   strLogFileName = strLogFilePath + "Log-";
				   				 /* Log File Path and File Name */
			FileStream fsLogFile = null;
								 /* Access to Log File */
			byte[] abyteLogMsg;	 /* Holder for Log Message Being Put into File */
			
			try {
				
				/* If the Log File Has a Directory, Make Sure it Exists, Else Create it, Add Path to File Name */
				if (!Directory.Exists(strLogFilePath)) {
						
				    Directory.CreateDirectory(strLogFilePath);
				}
                
                if (strLogMsg != "") {

                    if (boolError) {

                        strLogMsg = " Error - " + strLogMsg;
                    }
			
				    /* Add Date to Log File Before Showing */
				    strLogMsg = dtNow.ToString("hh:mm:ss.ffff") + ": " + strLogMsg;

                    if (exSentError != null) {

                        strLogMsg += " Exception: " + exSentError.Message + ". Stacktrace: " + exSentError.StackTrace;
                    }
					   				 
				    Console.WriteLine(strLogMsg);
                }

                if (boolLogFileNotLocked) {

                    boolLogFileNotLocked = false;
					
				    /* Append Message to File, If it Doesn't Exist, Create it */
				    strLogFileName += dtNow.ToString("yyyyMMdd") + ".txt";
	
				    fsLogFile = File.Open(strLogFileName, FileMode.Append);

                    while (qstrLogFileMsgWaiting.Count > 0) { 

                        abyteLogMsg = new System.Text.UTF8Encoding(true).GetBytes(qstrLogFileMsgWaiting.Dequeue() + "\r\n");

                        fsLogFile.Write(abyteLogMsg, 0, abyteLogMsg.Length);
                    }
                
                    if (strLogMsg != "") {

                        abyteLogMsg = new System.Text.UTF8Encoding(true).GetBytes(strLogMsg + "\r\n");

                        fsLogFile.Write(abyteLogMsg, 0, abyteLogMsg.Length);
                    }

                    fsLogFile.Close();

                    boolLogFileNotLocked = true;
                }
                else if (strLogMsg != "") { 
                
                    qstrLogFileMsgWaiting.Enqueue(strLogMsg);
                }

			}
            catch (Exception exError) {
                                    
				Console.WriteLine("Error: Writing to log file failed. Message: " + exError.Message);
				
				if (fsLogFile != null) {
				
					fsLogFile.Close();
				}
            }
		}

        /// <summary>
        ///     Debug Information on Current Communications Being Processed
        /// </summary>
        /// <returns>Returns Debug Information on Current Communications</returns>
        public string Debug() {

            int nMsgCount = DebugReceivedQueueCount();
                                        /* Count of Messages in Queue */
            string strMsg = "Received: ";
                                        /* Returned Message List */
            int nCounter = 0;           /* Counter for Loop */

            if (nMsgCount > 0) {

                for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

                    if (nCounter > 0) {

                        strMsg += " | ";
                    }

                    strMsg += "R-" + (nCounter + 1) + " - " + DebugReceived(nCounter);
                }
            }
            else {

                strMsg += "None";
            }

            nMsgCount = DebugReceivedStoredQueueCount();
            strMsg += " ----- Received - Stored: ";

            if (nMsgCount > 0) {

                for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

                    if (nCounter > 0) {

                        strMsg += " | ";
                    }

                    strMsg += "RS-" + (nCounter + 1) + " - " + DebugReceivedStored(nCounter);
                }
            }
            else {

                strMsg += "None";
            }

            nMsgCount = DebugToSendQueueCount();
            strMsg += " ----- Sending: ";

            if (nMsgCount > 0) {

                for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

                    if (nCounter > 0) {

                        strMsg += " | ";
                    }

                    strMsg += "SD-" + nCounter + 1 + " - " + DebugToSend(nCounter);
                }
            }
            else {

                strMsg += "None";
            }

            nMsgCount = DebugToSendStoredQueueCount();
            strMsg += " ----- Sending - Stored: ";

            if (nMsgCount > 0) {

                for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

                    if (nCounter > 0) {

                        strMsg += " | ";
                    }

                    strMsg += "SS" + (nCounter + 1) + " - " + DebugToSendStored(nCounter);
                }
            }
            else {


                strMsg += "None";
            }

            return strMsg;
        }

        public int DebugReceivedQueueCount() {

            return ClientDebugReceivedQueueCount();
        }

        public int DebugToSendQueueCount() {

            return ClientDebugSendQueueCount();
        }

        public int DebugReceivedStoredQueueCount() {

            return ClientDebugReceivedStoredQueueCount();
        }

        public int DebugToSendStoredQueueCount() {

            return ClientDebugSendStoredQueueCount();
        }

        /// <summary>
        ///     Debug for Getting Message from Receiving Queue by Message Index
        /// </summary>
        /// <param name="nMsgIndex">Index for Message to Retrieve from Receiving Queue</param>
        /// <returns>Return Message from Receiving Queue If Found, Else Blank String</returns>
        public string DebugReceived(int nMsgIndex) {
              
            return "(" + ClientDebugReceivedMsgLength(nMsgIndex) + ") " + GetClientReturnValue(ClientDebugReceived(nMsgIndex));
        }

        /// <summary>
        ///     Debug for Getting Message from Sending Queue by Message Index
        /// </summary>
        /// <param name="nMsgIndex">Index for Message to Send from Sending Queue</param>
        /// <returns>Return Message from Sending Queue If Found, Else Blank String</returns>
        public string DebugToSend(int nMsgIndex) {
                                        
            return "(" + ClientDebugSendMsgLength(nMsgIndex) + ") " + GetClientReturnValue(ClientDebugToSend(nMsgIndex));
        }

        /// <summary>
        ///     Debug for Getting Message from Receiving Stored Queue by Message Index
        /// </summary>
        /// <param name="nMsgIndex">Index for Message to Receive from Receiving Queue</param>
        /// <returns>Return Message from Receiving Queue If Found, Else Blank String</returns>
        public string DebugReceivedStored(int nMsgIndex) {

            return "(" + ClientDebugReceivedStoredMsgLength(nMsgIndex) + ") " + GetClientReturnValue(ClientDebugReceivedStored(nMsgIndex));
        }

        /// <summary>
        ///    Debug for Getting Message from Sending Stored Queue by Message Index
        /// </summary>
        /// <param name="nMsgIndex">Index for Message to Send from Sending Queue</param>
        /// <returns>Return Message from Sending Queue If Found, Else Blank String</returns>
        public string DebugToSendStored(int nMsgIndex) {

            return "(" + ClientDebugSendStoredMsgLength(nMsgIndex) + ") " + GetClientReturnValue(ClientDebugToSendStored(nMsgIndex));
        }

        public void DebugActivate(bool boolOn = true) {

            boolDebug = boolOn;
        }

        public bool DebugMode() {

            return boolDebug;
        }
        /// <summary>
        ///     Convert Client Pointer to Returned Value into String
        /// </summary>
        /// <param name="ipClientValue">Pointer to Client Value</param>
        /// <returns>Client Value as String If Converted, Else Blank String</returns>
        private string GetClientReturnValue(IntPtr ipClientValue) {

            string strRetMsg = "";

            try {
    
                strRetMsg = Marshal.PtrToStringUTF8(ipClientValue);
            }
            catch (Exception exError) { 
            
                Log("During converting client pointer to returned value into string, an exception occurred.", true, exError);
            }

            return strRetMsg;
        }

        /// <summary>
        ///    Called When a Window Event Occurs to Check for Shutdown
        /// </summary>
        /// <param name="cmMsg">Event Indicator</param>
        private void WindowCloseCheck(CONTROLMSGS cmMsg) {

            switch (cmMsg) { 
           
                case CONTROLMSGS.CTRL_CLOSE_EVENT: {

                    Disconnect();
                    break;
                }
                case CONTROLMSGS.CTRL_LOGOFF_EVENT: {
                        
                    Disconnect();
                    break;
                }
                case CONTROLMSGS.CTRL_SHUTDOWN_EVENT: {
                        
                    Disconnect();
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