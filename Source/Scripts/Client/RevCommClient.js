/* RevCommClient.js - Client Communication Processing Script for RevCommSuite API

 MIT License

 Copyright (c) 2022 Bigman

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

var CLIENT = {

	SERVERURL: 'localhost',			/* Default Server URL */
	SERVERPORT: 59234,				/* Default Server Port */
    ENCRYPTKEYSIZE: 32,            	/* Size of Encryption Key */
    ENCRYPTIVBLOCKSIZE: 16,        	/* Size of Encryption IV Block */
	strMsgPartIndicate: '!*+#',		/* Message Part Starting Indicator */
	strMsgStartIndicate: '%=&>',	/* Message Starting Indicator */
	strMsgEndIndicate: '<@^$',		/* Message Ending Indicator */
	nMsgIndicatorLen: 4,			/* Message Indicator Length */
	strMsgFiller: '\0',				/* Message Filler Character */
	amsiListToSend: [],				
	amsiListReceived: [],			/* List of Information on Messages from Server and
		   						   	   Message to be Sent to Server */
	amsiListStoredToSend: [],
	amsiListStoredReceived: [],
	amsiListBackupSent: [],			/* List of Information on Messages Stored from Server and
	   								   Message to be Sent to Server to be Added to Queues When They Are Available and
	   								   Backup of Sent Messages */
	fAutoRetLimitInMillis: 1000,  	/* Automated Retrieval for HTTP Transmission or Data Process Time Limit in Milliseconds */
    boolAutoRetProcessCmd: false, 	/* Indicator to Process Automated Retrieval Client Messages */
    boolAutoRetEndTrans: false,   	/* Indicator to Close HTTP Transmission or Delete Data Process After Automated Retrieval */
	boolConnected: false,			/* Indicator That Client is Connected */
	boolRunConnection: true,		/* Indicator to Run Processes */
	boolUseSSL: false,				/* Indicator to Use SSL Connection */
	wsServerConnect: null,			/* Server Web Socket Connection */
	wsPeertoPeerConnect: null,		/* Peer-to-Peer Web Socket Connection */
	appciListClients: [],			/* List of Peer-to-Peer Connections */
	strEncryptKey: '',				/* Peer-to-Peer Encryption Key */
	strEncryptIV: '',				/* Peer-to-Peer Encryption IV Block */
	nQueueMsgLimit: 2000,			/* Limit of Messages in Queue */
	boolMsgDropped: false,			/* Indicator That a Message was Dropped */
	llSendMsgs: 0,					/* Count of Sending Messages */	
	llReceivedMsgs: 0,				/* Count of Received Messages */	
	nTimeToLateInMillis: 30,		/* Amount of Time to Message is Considered Late */	
	boolRemoveLateMsgs: false,		/* Indicator to Drop Late Messages */
	nTimeToCheckActInMillis: 30,    /* Amount of Time Till Check Activity If no Messages Were Received */
	llLastActOrCheckInMillis: new Date().getTime(),
									/* Time Since Last Action of Check */
	strLeftOver: "",				/* Holds Left Over Part of Received Messages */	
	boolMsgsSync: true,				/* Indicator That Messages are in Sync */
	objReceivers: {HTTP: {}, DATAPROCESS: {}},
                                   	/* Holds Threads for Receiving Responses for HTTP Transmissions and Data Processes */
	astrAutoRetDirectMsgDesigns: [],/* List of Direct Message Designations to Automatically Execute */
	boolDebug: false,				/* Indicator to Send Debug Messages */
	Connect: function(strHostNameIP, nPort, boolSetUseSSL) {
		
		var strURL,						/* Connection URL */
			wsServerSetup;				/* Web Socket for Setting up Connection */
			
		if (!Number.isInteger(nPort)) {

			nPort = this.SERVERPORT;
		}

		if (boolSetUseSSL || this.boolUseSSL) {
			
			strURL = 'wss://';
		}
		else {
			
			strURL = 'ws://';
		}
		
		if (typeof(strHostNameIP) == 'string' && strHostNameIP != '') {
			
			strURL += strHostNameIP;
		}
		else {
			
			strURL += this.SERVERURL;
		}
		
		strURL += ':' + nPort;
		
		try {

			wsServerSetup = new WebSocket(strURL);

			wsServerSetup.onopen = function() {
			
				if (!this.boolConnected) {

					this.boolConnected = true;
		
					this.nComTimerID = setInterval(function() {

						CLIENT.Communicate();
					}, 100);
					
					this.nPeerToPeerComTimerID = setInterval(function() {
						
						CLIENT.PeerToPeerCommunicate();
					}, 100);
					
					if (!this.AddSendMsg("GETSTREAMFILELIST")) {
								
						this.AddLogErrorMsg("After server connection, sending message 'GETSTREAMFILE' failed.");
					}
				}
			}.bind(this);
			
			wsServerSetup.onmessage = this.ReceiveMsg.bind(this);
			wsServerSetup.onclose = this.Close.bind(this);
			wsServerSetup.onerror = this.ClientError.bind(this);
			wsServerSetup.onmessageerror = this.ClientError.bind(this);

			this.wsServerConnect = wsServerSetup;
		}
		catch (exError) {
			
			/* Connection Failed */
			this.AddLogErrorMsg("During starting client server connection, exception occurred. Exception: " + exError.message);
		}
	},
	Communicate: function() {
		
		try {

			if (this.wsServerConnect) {

				if (this.boolRunConnection) {

					this.ProcessReplay();
					
					if ((this.boolRunConnection = this.SendMsg())) {
				
						this.AddStoredMsgs();
						this.CheckQueueLimitChange();
						this.CheckLastActivity();
						this.ProcessPing();
					}
					
					this.GetLogError();
                    this.GetDisplayError();
                    
                    if (this.boolDebug) {
                    	
                    	this.Debug();
                    }
					
/*					this.GetStreamMsgNext(this.boolAutoRetProcessCmd);
					
					if (this.RunAutoDirectMsgByDesign()) {

						this.GetDirectMsgNext(false);
					}
					else {

						this.GetDirectMsgNext(this.boolAutoRetProcessCmd);
					}*/
					
					if (this.AutoRetProcessCmd() && !this.RunAutoDirectMsgByDesign()) {

						this.GetDirectMsgNext(true);
					}
				}
				else {
			
					this.Close();
				}
			}
			else {

				this.AddLogErrorMsg("During running server message sender and receiver, connecting to server failed.");
				this.boolRunConnection = false;
			}

			if (!this.boolRunConnection) {
		
				this.Close();
			}
		}
		catch (exError) {

			this.AddLogErrorMsg("During running server message sender and receiver, an exception occurred. Exception: " + exError.message);
			this.boolRunConnection = false;
		}
	},
	PeerToPeerCommunicate: function() {
	
		var appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			ppciSelected = null,		/* Selected "Peer To Peer" Client Information */
			strMsg = "",				/* Received Message */ 
			nMsgLen = 0,				/* Length of Received Message */
			nQueueCount = this.DebugReceivedQueueCount(),
										/* Queue Count */
			boolNotCompleted = true,	/* Indicator That Processing is Not Complete */
			strPeerIPAddress = "",		/* Current IP Address */
			msgSelect,					/* Selected Message Information */
			boolIPNotFound = true;		/* Indicator That New Client was not Already Connected */

		try {

			this.CheckPeerToPeerDisconnect();

			/* Do Communications with Clients, 
			   Move Through Client and Retrieve Any Messages Including Getting Completed Messages, and
			   Send Current Message */
			
			this.DoPeerToPeerNegotiation();
			
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
				
				ppciSelected = appciConnected[nCounter];
				
				if (!ppciSelected.Connect()) {
					
					ppciSelected.CloseClient();
					appciConnected.splice(nCounter, 1);
					nCounter--;
					nListCount--;
				}
			}
			
			while (boolNotCompleted && nQueueCount < this.nQueueMsgLimit) {
				
				boolNotCompleted = false;
				
				for (nCounter = 0; nCounter < nListCount; nCounter++) {
					
					ppciSelected = appciConnected[nCounter];
				
					strMsg = ppciSelected.DequeueMsg();
					nMsgLen = strMsg.length;
	
					if (nMsgLen > 0) { 
	
						this.AddReceivedMsg(strMsg, nMsgLen);
						nQueueCount = this.DebugReceivedQueueCount();
						boolNotCompleted = true;
					}
				}
			}

			/* Check for Server Messages to Connect to "Peer To Peer" Server as Client */
			this.CheckStartPeerToPeerConnect();

			/* If "Peer To Peer" Server Setup, Check for New Client's Connecting, 
				Else Check If Main Server Requires It */
			if (this.HasPeerToPeerServer()) {
					
				/* Check If Existing Encryption Infromation Exists */
				if ((msgSelect = this.DequeueReceivedMsg("PEERTOPEERENCRYPT"))) {
					
					strPeerIPAddress = msgSelect.GetSegment(1);

					for (nCounter = 0; nCounter < nListCount && boolIPNotFound; nCounter++) {
						
						ppciSelected = appciConnected[nCounter];
						
						if (ppciSelected.GetPeerIPAddress() == strPeerIPAddress) {
							
							ppciSelected.StartNegotiation(new PeertoPeerClient(new WebSocket('ws://' + strPeerIPAddress + ':' + msgSelect.GetSegment(2)), 
																		       null,
																		       strPeerIPAddress,  
																		       msgSelect.GetSegment(3), 
																		       msgSelect.GetSegment(4),
																		       this.strEncryptKey, 
																		       this.strEncryptIV));

							boolIPNotFound = false;
						}
					}
					
					if (boolIPNotFound) {
						
						appciConnected.push(new PeertoPeerClient(new WebSocket('ws://' + strPeerIPAddress + ':' + msgSelect.GetSegment(2)), 
														       	 null,
														         strPeerIPAddress,  
														         msgSelect.GetSegment(3), 
														         msgSelect.GetSegment(4),
														         this.strEncryptKey, 
														         this.strEncryptIV));
					}
				}
			}
			else {

				this.CheckStartPeerToPeerServer();
			}
			
			this.GetLogError();
            this.GetDisplayError();
            
            if (this.boolDebug) {
            	
            	this.Debug();
            }
		}
		catch (exError) {

			this.AddLogErrorMsg("During running 'Peer To Peer' client message sender and receiver, an exception occurred. Exception: " + exError.message);
			this.boolRunConnection = false;
		}
	},
	ReceiveMsg: function(evReceived) {
		
		var strMsgReceived = evReceived.data,
			nReceivedAmount = strMsgReceived.length;
									/* Received Message and Length */
		
		if (this.boolRunConnection && nReceivedAmount > 0) {
			
			if (this.DebugReceivedQueueCount() < this.nQueueMsgLimit) {
				
				this.AddReceivedMsg(strMsgReceived, nReceivedAmount);
			}
			else {
				
				this.StoreReceivedMsg(strMsgReceived, nReceivedAmount);
			}
			
			this.llLastActOrCheckInMillis = new Date().getTime();
		}
	},
	ClientError: function() {

		this.AddLogErrorMsg("During running of client, unknown error occurred.");
	},
	StartPeerToPeerServer: function(strHostNameIP, nPort, strSetEncryptKey, strSetEncryptIV) {
		
		if (typeof(strHostNameIP) == 'string' && strHostNameIP != '' && Number.isInteger(nPort)) {
		
			if (typeof(strSetEncryptKey) == 'string' && strSetEncryptKey != "" && 
				typeof(strSetEncryptIV) == 'string' && strSetEncryptIV != "") {
				
				if (strSetEncryptKey.length >= this.ENCRYPTKEYSIZE && strSetEncryptIV.length >= this.ENCRYPTIVBLOCKSIZE) {
					
					this.strEncryptKey = strSetEncryptKey;
					this.strEncryptIV = strSetEncryptIV;
				}
				else {
					
					/* Invalid Encryption Key or IV Block Sent */
					this.AddLogErrorMsg("During starting 'Peer-to-Peer' server, sent encryption key and IV block that were invalid lengths. Server started without encryption.");
				}
			} 
			else {
				
				/* Invalid Encryption Key or IV Block Sent */
				this.AddLogErrorMsg("During starting 'Peer-to-Peer' server, sent encryption key and IV block were invalid. Server started without encryption.");
			}
			
			try {

				/* If No Encyption Information Sent, Start with Unencrypted Communications */
				this.wsPeertoPeerConnect = new WebSocket('ws://' + strHostName + ':' + nPort);
			}
			catch (exError) {
				
				this.AddLogErrorMsg("During starting 'Peer-to-Peer' server failed, exception occurred. Exception: " + exError.message);
			}
		}
		else {
			
			this.AddLogErrorMsg("During starting 'Peer-to-Peer' server failed, sent hostname or port was invalid.");
		}
	},
	CheckStartPeerToPeerServer: function() {

		var amsgSelect = this.DequeueReceivedMsg("PEERTOPEERSTART");				
									/* Returned Message List */
/*			msgStart,				/* Start of Message */	
/*			strIP = msgStart.GetSegment(1),
										/* IP Address */
/*			strPort = msgStart.GetSegment(2);
										/* IP Port */

		if (amsgSelect && !this.HasPeerToPeerServer()) {

			var msgStart = amsgSelect[0];
				strIP = msgStart.GetSegment(1),
				strPort = msgStart.GetSegment(2);

			if (strIP != "" && strPort != "") {

				this.StartPeerToPeerServer(strIP, strPort, msgStart.GetSegment(3), msgStart.GetSegment(4));
			}
			else {
			
				this.AddLogErrorMsg("Processing message 'PEERTOPEERSTART' failed due to invalid message. Message: '" + msgStart.GetMessage() + "'.");
			}
		}
		else if (this.HasPeerToPeerServer()) {
	 
			this.AddLogErrorMsg("Processing message 'PEERTOPEERSTART' failed due 'Peer To Peer' server connection already alive.");
		}
	},
	StartPeerToPeerConnect: function(strHostNameIP, nPort, strSetEncryptKey, strSetEncryptIV) {

		var MSGPARTINDICATOR = this.GetMsgPartIndicator(),
									/* Message Part Indicator */
			strClientEncryptKey = null,	
			strClientEncryptIV = null,	
									/* Encryption Key and IV Block for Client Communcations */
			wsNewConnect = null,	/* New Peer-to-Peer Connection */
			ppciSelected = null,	/* Selected Client Connection */
			appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0;
		
		for (nCounter = 0; nCounter < nListCount && !ppciSelected; nCounter++) {

			if (appciConnected[nCounter].GetPeerIPAddress() == strHostNameIP) {
				
				ppciSelected = appciConnected[nCounter];
			}
		}
		
		if (typeof(strSetEncryptKey) == 'string' && strSetEncryptKey != "" && 
			typeof(strSetEncryptIV) == 'string' && strSetEncryptIV != "" && 
			strSetEncryptKey.length >= ENCRYPTKEYSIZE && 
			strSetEncryptIV.length >= ENCRYPTIVBLOCKSIZE) {
					
			strClientEncryptKey = strSetEncryptKey;
			strClientEncryptIV = strSetEncryptIV;
		}
		else {
			
			/* Invalid Encryption Key or IV Block Sent */
			this.AddLogErrorMsg("During starting 'Peer-to-Peer' connection, sent encryption key and IV block were invalid. Connection started without encryption.");
		}
		
		try {
			
			wsPeertoPeerConnect = new WebSocket('ws://' + strHostName + ':' + nPort);
			
			if (!ppciSelected) {

				ppciSelected = new PeertoPeerClient(wsNewConnect, 
													null,
													strHostNameIP,  
													strClientEncryptKey, 
													strClientEncryptIV, 
													this.strEncryptKey, 
													this.strEncryptIV);

				ppciSelected.SetReceivedMsgLateLimit(this.nTimeToLateInMillis);
				ppciSelected.SetReceivedDropLateMsgs(this.boolRemoveLateMsgs);
				ppciSelected.SetReceivedCheckTimeLimit(this.nTimeToCheckActInMillis);
				ppciSelected.SetBackupQueueLimit(this.nBackupQueueMsgLimit);
				
				appciConnected.push(ppciSelected);
				
			}
			else {
				
				ppciSelected.StartNegotiation(new PeertoPeerClient(wsNewConnect, 
																   null,
																   strHostNameIP,  
																   strClientEncryptKey, 
																   strClientEncryptIV,
																   this.strEncryptKey, 
																   this.strEncryptIV));
			}
		}
		catch (exError) {
			
			/* Exception Occurred */
			this.AddLogErrorMsg("During starting 'Peer-to-Peer' connection, exception occurred. Storing encryption information for later connection. Exception: " + exError.message);
			
			if (wsPeertoPeerConnect) {
				
				try {

					wsPeertoPeerConnect.close();
				}
				catch(exError) {
					
					this.AddLogErrorMsg("During starting 'Peer-to-Peer' connection, exception occurred. Exception: " + exError.message);
				}
			}
			
			if (strClientEncryptKey && 
				strClientEncryptIV && 
				!this.FindReceivedMsg("PEERTOPEERENCRYPT", [strHostNameIP])) {
				
				this.AddReceivedMsg(this.GetMsgStartIndicator() + "PEERTOPEERENCRYPT" + 
									MSGPARTINDICATOR + strHostNameIP + 
									MSGPARTINDICATOR + nPort + 
									MSGPARTINDICATOR + strClientEncryptKey + 
									MSGPARTINDICATOR + strClientEncryptIV +
									this.GetMsgEndIndicator());
			}
		}
	},
	CheckStartPeerToPeerConnect: function() {
	
		var amsgInfo = this.DequeueReceivedMsg("PEERTOPEERCONNECT"),				
									/* Returned Message List */
			msgStart;				/* Starting Message */
//			strIP = NULL,			/* IP Address for "Peer To Peer" Server to Connect to */
//			strPort = NULL;			/* Port for "Peer To Peer" Server to Connect to */

		if (amsgInfo) {

			msgStart = amsgInfo[0];
			strIP = msgStart.GetSegment(1);
			strPort = msgStart.GetSegment(2);

			/* If Information was Found, Connect to "Peer To Peer" Server */
			if (strIP && strPort && strIP != "" && strPort != "") {
				
				this.StartPeerToPeerConnect(strIP, strPort, msgStart.GetSegment(3), msgStart.GetSegment(4));
			}			
			else {
	
				this.AddLogErrorMsg("Processing message 'PEERTOPEERCONNECT' failed due to invalid message. Message: '" + msgStart.GetMessage() + "'.");
			}
		}
	},
	CheckPeerToPeerDisconnect: function() {

		if (this.DequeueReceivedMsg("PEERTOPEERDISCONNECT")) {

			this.ClosePeerToPeer();
		}
	},
	CheckQueueLimitChange: function() {
	
		var amsiMsgList = this.DequeueReceivedMsg("SETQUEUELIMIT"),			
										/* Returned Message */
			strNewQueueLimit = "";		/* New Queue Limit */

		if (amsiMsgList) {

			strNewQueueLimit = amsiMsgList[0].GetSegment(1);

			if (strNewQueueLimit != "") {

				try {

					this.nQueueMsgLimit = parseInt(strNewQueueLimit);
				}
				catch (exError) {

					this.AddLogErrorMsg("Processing message 'SETQUEUELIMIT' failed due to invalid message. Exception: '" + exError.message + "'.");
				}
			}			
			else {
	
				this.AddLogErrorMsg("Processing message 'SETQUEUELIMIT' failed due to invalid message. Message: '" + amsiMsgList[0].GetMessage() + "'.");
			}
		}
	},
	ValidateCreateMsg: function(strMsg, nMsgLen) {
		
		var msgCheck = null,	/* Check for Valid Message Information */
			llSeqNum = 0,		/* Message Sequence Number */
			llTimeInMillis = 0,	/* Message Time Sent in Milliseconds */
			boolInvalid = false,	
								/* Indicator That Message is Invalid */
			boolNoSyncBypass = true;
								/* Indicator to Not Bypass Message Out of Sync Indicator When 
								   Appropriate Message Information Record Arrives 
								   While Out of Sync */

		try {

			msgCheck = MESSAGE.CreateMsg(strMsg, nMsgLen);

			if (msgCheck.HasStart()) {

				llSeqNum = msgCheck.GetMetaDataSeqNum();
				llTimeInMillis = msgCheck.GetMetaDataTime();
			}

			/* If Metadata Exists */ 
			if (llSeqNum > 0) {

				/* If Message is in Sequence */
				if (this.llReceivedMsgs + 1 == llSeqNum) {
					
					this.llReceivedMsgs++;
					
					if (this.boolMsgsSync) {
						
						if (this.boolRemoveLateMsgs && llTimeInMillis > 0) {
	
							boolInvalid = new Date().getTime() - llTimeInMillis >= this.nTimeToLateInMillis;
						}
					}
					else {
						
						/* Appropriate Message Arrived While Out of Sync, Set Bypass to Add to Received Queue */
						boolNoSyncBypass = false;
					}
				}
				else {
					
					this.RepairReceivedMsg(strMsg);
				}
			}
			else if (!this.boolMsgsSync) {
				
				this.RepairReceivedMsg(strMsg);
			}

			if ((!this.boolMsgsSync && boolNoSyncBypass) || boolInvalid) {

				msgCheck = null;
			}
		}
		catch (exError) {
		
			this.AddLogErrorMsg("During validating message, an exception occurred. Exception: " + exError.message);
		}

		return msgCheck;
	},
	RepairReceivedMsg: function(strMsg) {
		
		var nMsgLastStartIndex = -1,
			amsiListQueue = this.amsiListReceived,
			nMsgCount = this.amsiListReceived.length,
//			msgSelect = null,	
// 			aMsgsFound = null,
			llNextMsgNum = this.llReceivedMsgs + 1,
			nCounter = 0;
			
		for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
			if (amsiListQueue[nCounter].HasStart()) {
				
				nMsgLastStartIndex = nCounter;
			}
		}

		if (this.boolMsgsSync) {
			
			var msgSelect = MESSAGE.CreateMsg(strMsg);
			
			/* If New Message is Not in Sequence with Received Messages, and the Last Messages is Incomplete, 
			   Remove the Last Message, Request Resend from Server */
			if (msgSelect.GetMetaDataSeqNum() > 0 &&
			    this.llReceivedMsgs < msgSelect.GetMetaDataSeqNum()) {
			
				if (nMsgCount > 0 && amsiListQueue[nMsgCount - 1].HasEnd()) {
				
					llNextMsgNum = this.llReceivedMsgs;
					this.llReceivedMsgs--;
					
					if (nMsgCount > 1) {
	
						this.amsiListReceived.splice(nMsgLastStartIndex, amsiListQueue.length - (nMsgLastStartIndex + 1));
					}
					else {
						
						this.amsiListReceived = [];
					}
				}
				else {
					
					this.StoreReceivedMsg(strMsg, strMsg.length);
				}
				
				if (!this.AddSendMsg("MSGREPLAY", [llNextMsgNum], false)) {
					
					this.AddLogErrorMsg("During resequencing of received messages, sending message 'MSGREPLAY' for message sequence number, " + 
										llNextMsgNum + ", failed.");
				}
				
				this.boolMsgsSync = false;
			}
		}	
		else {
			
			var aMsgsFound = null;	
					
			while ((aMsgsFound = this.DequeueStoredMsg(this.llReceivedMsgs + 1))) {
				
				this.amsiListReceived.concat(aMsgsFound);
				this.llReceivedMsgs++;
			}
			
			/* If No More Stored Message Waiting to be Put in Queue to Go Back in Sync, Change Mode */
			if (this.amsiListStoredReceived.length > 0) {
			
				amsiListQueue = this.amsiListStoredReceived;
			
				for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
					
					/* If Message Found Where Still Out of Sync, Exit, and Keep in Out of Sync Mode */
					if (this.llReceivedMsgs < amsiListQueue.GetMetaDataSeqNum()) {
						
						this.boolMsgsSync = false;
						break;
					}
					else {
						
						/* Else Assume the End, and No Out of Sync Message Found, Go Back to In Sync Mode */
						this.boolMsgsSync = true;	
					}
				}
			}
			else {
				
				this.boolMsgsSync = true;
			}
		}	
	},
	CheckLastActivity: function() {

		if (new Date().getTime() - this.llLastActOrCheckInMillis >= this.nTimeToCheckActInMillis) {
			
			if (!this.AddSendMsg("MSGCHECK", [this.llReceivedMsgs + 1], false)) {
						
				this.AddLogErrorMsg("Sending message 'MSGCHECK' for message activity check, " + (this.llReceivedMsgs + 1) + ", failed.");
			}

			this.llLastActOrCheckInMillis = new Date().getTime();
		}
	},
	DebugMaskPeerToPeerIP: function(strMsg, nMsgLen) {
	
		var strIP = '',				/* IP Addresses to Remove */
			strFillerVals = '';		/* Filler Values for Replacing IP Addresses */
//			nIPIndex = strMsg.indexOf(MESSAGE.FindSegmentInString(strMsg, 1, nMsgLen)),
									/* Index of Segment to Remove IP Addresses */
//			nIPLen = strIP.length,	/* Index of Segment to Remove IP Addresses */
//			nCounter = 0;			/* Counter for Loop */
			
		if (strMsg && 
			strMsg.indexOf("PEERTOPEER") > 0 &&
			(strIP = MESSAGE.FindSegmentInString(strMsg, 1, nMsgLen)).length > 0) {
		
			var nIPIndex = strMsg.indexOf(strIP),
				nIPLen = strIP.length,
				nCounter = 0;
				
			if (nIPIndex >= 0) {
			
				for (nCounter = 0; nCounter < nIPLen; nCounter++) {
				
					strFillerVals += "#";
				}
				
				strMsg.replace(strIP, strFillerVals);
			}
		}
		
		return strMsg;
	}, 
	DoPeerToPeerNegotiation: function() {

		var amsgInfo = this.DequeueReceivedMsg("PEERTOPEERNEGOTIATE"),				
									/* Returned Message List */
			ppciSelected = null,	/* Selected Client Connection */
			appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
//			strIPAddress = "";		/* IP Address */
			nCounter = 0;

		if (!amsgInfo) {
			
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
		
				appciConnected[nCounter].CheckNegotiation();
			}
		}
		else {

			var strIPAddress = amsgInfo[0].GetSegment(1);

			for (nCounter = 0; nCounter < nListCount; nCounter++) {
		
				if (appciConnected[nCounter].GetPeerIPAddress() == strIPAddress) {
					
					appciConnected[nCounter].DoNegotiation();
				}
			}
		}
	},
	EnqueueSendMsg: function(strMsg) {
	
		var MSGENDINDICATORLEN = this.GetMsgEndIndicatorLength();
									/* Length of End Indicator */
			nMsgLen = strMsg.length,/* Length of Message */
			nNewMsgStartIndex = -1,
			nNewMsgEndIndex = -1,	/* New Message's Starting and Ending Index */
			boolSuccess = false,	/* Indicator That Adding Message was Successful */
			strMsgSelect = "",		/* Selected Part of Message */
			msgSelect = null,		/* Selected Message Information Record */
			msgStore = null;		/* Selected Store Message Information Record */
			nQueueStoreCount = this.DebugToSendStoredQueueCount(),
			nQueueCount = this.DebugToSendQueueCount(),
									/* Queue Store and Regular Queue Count */
			nSendCount = 0,			/* Number of Sends Enqueued */ 
			nCounter = 0; 			/* Counter for Loop */

		try {
			
			nNewMsgStartIndex = MESSAGE.FindStringStartIndex(strMsg);
			nNewMsgEndIndex = MESSAGE.FindStringEndIndex(strMsg);

			if (nNewMsgStartIndex >= 0 && nNewMsgEndIndex >= 0) {

				nCounter = nNewMsgStartIndex;
				nNewMsgEndIndex += (MSGENDINDICATORLEN - 1);

				if (this.amsiListToSend.length) {

					msgSelect = this.amsiListToSend[this.amsiListToSend.length - 1];
				}
				
				if (this.amsiListStoredToSend.length) {

					msgStore = this.amsiListStoredToSend[this.amsiListStoredToSend.length - 1];
				}
			
				while (nCounter < nMsgLen && nSendCount < this.nQueueMsgLimit) {

					/* If Message Doesn't Fit in the Buffer, Grab a Buffer Size Piece of It */
					if (nNewMsgEndIndex - nCounter <= this.BUFFERSIZE) {
						
						strMsgSelect = strMsg.substr(nCounter, (nNewMsgEndIndex - nCounter) + 1);
						nCounter += nNewMsgEndIndex - nCounter;
					}
					else if (nCounter + this.BUFFERSIZE <= nMsgLen) {
					
						strMsgSelect = strMsg.substr(nCounter, this.BUFFERSIZE);
						nCounter += this.BUFFERSIZE;
					}
					else {
						
						strMsgSelect = strMsg.substring(nCounter);
						nCounter = nMsgLen - nCounter;
					}

					/* If the Last Message was the Complete Message, and the New One Has a Starting Indicator
					   Put New Message in its Own, Else Add to Last Message */
					if (!msgSelect || (msgSelect.IsComplete() || msgSelect.HasEnd()) && nQueueCount < this.nQueueMsgLimit) {

						msgSelect = MESSAGE.CreateMsg(strMsgSelect);
						this.amsiListToSend.push(msgSelect);
						boolSuccess = true;
					}
					else if (nQueueStoreCount < this.nQueueMsgLimit) {

						/* Else Send Queue Has an Incomplete Message, If the Last Message in the Sending Store Queue
						   was the Complete Message, and the New One Has a Starting Indicator
						   Put New Message in its Own, Else Add to Last Message */
						if (!msgStore || msgStore.IsComplete() || msgStore.HasEnd()) {

							msgStore = MESSAGE.CreateMsg(strMsgSelect);
							this.amsiListStoredToSend.push(msgStore);
							boolSuccess = true;
						}
						else {

							/* Else the Last Message in the Store is Incomplete, Note that the Message was Dropped */
							this.AddLogErrorMsg("During adding message to send, message being put into store queue failed due to queue have an incomplete message at the end.");
						}
					}
					else {
					
						this.boolMsgDropped = true;
					}

					if (boolSuccess) {

						if (nCounter < nMsgLen) {

							nNewMsgStartIndex = MESSAGE.FindStringStartIndex(strMsg.substring(nCounter));
							nNewMsgEndIndex = MESSAGE.FindStringEndIndex(strMsg.substring(nCounter));

							if (nNewMsgStartIndex >= nCounter && nNewMsgEndIndex > nNewMsgStartIndex) {

								nCounter = nNewMsgStartIndex;
								nNewMsgEndIndex += (MSGENDINDICATORLEN - 1);
							}
							else {
							
								nCounter = nMsgLen;
							}
						}
					}
					else {

						nCounter = nMsgLen;
					}

					nSendCount++;
				}
			}
			else {

				this.AddLogErrorMsg("During adding message to send, could not add message due invalid format from missing start or end indicator. Message: '" + strMsg + "'.");
				boolSuccess = false;
			}
		}
		catch (exError) {
			
			this.AddLogErrorMsg("During adding message to send, an exception occurred. Exception: " + exError.message);
			boolSuccess = false;
		}

		return boolSuccess;
	},
	AddReceivedMsg: function(strMsg, nMsgLen) {

		var MSGFILLERCHAR = this.GetMsgFiller(),
									/* Message Filler Character */
			MSGENDINDICATORLEN = this.GetMsgEndIndicatorLength(),
									/* Length of Message End Indicator */
			nMsgStartIndex = 0,		
			nMsgEndIndex = 0,		/* Starting and Ending Index of Message */
			nIndexLen = 0;			/* Length Between Index */
			msgSelect = null,		/* Selected Message Information Record */
			nMsgLastIndex = 0,		/* Index of Last Completed Message Information Record */
			amsiListQueue = this.amsiListReceived,
			nMsgCount = amsiListQueue.length,	
			boolNoStart = false,	/* Indicator That Message Has No Start Index */
			boolCompleted = false, 	/* Indicator That Selected Message Completed */
			nCounter = 0;			/* Counter for Loop */
					
		try {

			if (strMsg && strMsg.trim() != "") {

				if (!Number.isInteger(nMsgLen)) {
					
					nMsgLen = strMsg.length;
				}

				if (this.amsiListReceived.length) {
					
					msgSelect = amsiListQueue[amsiListQueue.length - 1];
					
					if (this.strLeftOver.length) {
						
						strMsg = this.strLeftOver + strMsg;
						nMsgLen = strMsg.length;
						this.strLeftOver = "";
					}
				}
					
				/* Find If Initial Message Exists */
				nMsgStartIndex = MESSAGE.FindStringStartIndex(strMsg, nMsgLen);
				nMsgEndIndex = MESSAGE.FindStringEndIndex(strMsg, nMsgLen);

				/* If it Has No Start, Get All of Message  */
				if (nMsgStartIndex < 0 || (nMsgEndIndex > 0 && nMsgStartIndex >= nMsgEndIndex)) {

					nMsgStartIndex = 0;
					boolNoStart = true;

					for (nCounter = 0; nCounter < nMsgLen; nCounter++) { 

						if (strMsg[nCounter] == MSGFILLERCHAR || strMsg[nCounter] == '\0') {

							nMsgStartIndex++;
						}
						else {

							nCounter = nMsgLen;
						}
					}
				}
				
				/* If it Has No End, Get All of Message  */
				if (nMsgEndIndex >= 0) {
					
					nMsgEndIndex += (MSGENDINDICATORLEN - 1);
				}
				else {

					nMsgEndIndex = nMsgLen - 1;
				}
					
				nIndexLen = (nMsgEndIndex - nMsgStartIndex) + 1;

				if (nIndexLen > 0) {

					/* If Other Messages in Received Queue and the Last Message in Queue is Complete, Add New Message */
					if (msgSelect) {

						boolCompleted = msgSelect.IsComplete() || msgSelect.HasEnd();

						if ((boolCompleted && !boolNoStart) || (!boolCompleted && boolNoStart)) {
						
							msgSelect = this.ValidateCreateMsg(strMsg.substring(nMsgStartIndex), nIndexLen);
							
							if (msgSelect) {
	
								amsiListQueue.push(msgSelect);
							}
						}
						else if (!boolCompleted && !boolNoStart) {

							this.RepairReceivedMsg(strMsg.substring(nMsgStartIndex));
						}
						else if (boolCompleted && boolNoStart) {

							if (this.boolMsgsSync) {

								this.AddLogErrorMsg("During adding received message to queue, new message arrived without start indicator. Dropped message: '" + strMsg.substring(nMsgStartIndex) + "'. Last message: '" + msgSelect.GetMessage() + "'.");
							}
							else {
							
								this.RepairReceivedMsg(strMsg.substring(nMsgStartIndex));
							}
						}
						else {
						
							this.AddLogErrorMsg("During adding received message to queue, new message got inconsistant information. Dropped message: '" + strMsg.substring(nMsgStartIndex) + "'. Last message: '" + msgSelect.GetMessage() + "'.");								
						}
					}
					else if (!boolNoStart) {
			
						/* Else Received Queue is Empty, Add Message */
						msgSelect = this.ValidateCreateMsg(strMsg.substring(nMsgStartIndex), nIndexLen);
						
						if (msgSelect) {

							amsiListQueue.push(msgSelect);
						}			
					}
					else {

						this.AddLogErrorMsg("During adding received message to queue, new message with no start indicator. Dropped message: '" + strMsg.substring(nMsgStartIndex) + "'.");
					}
				}

				if (nMsgLen - (nMsgEndIndex + 1) > this.strMsgStartIndicate.length) {
				
					this.AddReceivedMsg(strMsg.substring(nMsgEndIndex + 1), nMsgLen - (nMsgEndIndex + 1));
				}
				else {
	
					this.strLeftOver = strMsg.substring(nMsgEndIndex + 1);
				}
			}
		}
		catch (exError) {
		
			this.AddLogErrorMsg("During adding received message to queue, an exception occurred. Exception: " + exError.message + ". Message: " + strMsg);
		}
	},
	StoreSendMsg: function(strMsg) {
	
		var MSGENDINDICATORLEN =  this.GetMsgEndIndicatorLength();
									/* Length of Message End Indicator */
			nMsgLen = strMsg.length,/* Length of Message */
			nNewMsgStartIndex = -1,
			nNewMsgEndIndex = -1,	/* New Message's Starting and Ending Index */
			nIndexLen = 0,			/* Length Between Index */
			strMsgSelect = "",		/* Selected Part of Message */
			nCounter = 0; 			/* Counter for Loop */

		try {
		
			nNewMsgStartIndex = MESSAGE.FindStringStartIndex(strMsg);
			nNewMsgEndIndex = MESSAGE.FindStringEndIndex(strMsg);

			if (nNewMsgStartIndex >= 0 && nNewMsgEndIndex >= 0) {
					
				nCounter = nNewMsgStartIndex;
				nNewMsgEndIndex += (MSGENDINDICATORLEN - 1);
				
				while (nCounter < nMsgLen) {
					
					if (nCounter > 0) {
					
						nIndexLen = nNewMsgEndIndex - nCounter;
					}
					else {
					
						nIndexLen = nNewMsgEndIndex + 1;
					}

					/* If Message Doesn't Fit in the Buffer, Grab a Buffer Size Piece of It */
					if (nIndexLen <= this.BUFFERSIZE) {
						
						strMsgSelect = strMsg.substring(nCounter, nIndexLen);
						nCounter += nIndexLen;
					}
					else if (nCounter + this.BUFFERSIZE <= nMsgLen) {
					
						strMsgSelect = strMsg.substr(nCounter, this.BUFFERSIZE);
						nCounter += this.BUFFERSIZE;
					}
					else {
						
						strMsgSelect = strMsg.substring(nCounter);
						nCounter = nMsgLen - nCounter;
					}
					
					this.amsiListStoredToSend.push(MESSAGE.CreateMsg(strMsgSelect));

					if (nCounter < nMsgLen) {

						nNewMsgStartIndex = MESSAGE.FindStringStartIndex(strMsg.substring(nCounter));
						nNewMsgEndIndex = MESSAGE.FindStringEndIndex(strMsg.substring(nCounter));

						if (nNewMsgStartIndex >= nCounter && nNewMsgEndIndex > nNewMsgStartIndex) {

							nCounter = nNewMsgStartIndex;
							nNewMsgEndIndex += (MSGENDINDICATORLEN - 1);
						}
						else {
							
							nCounter = nMsgLen;
						}
					}
				}
			}
			else {

				this.AddLogErrorMsg("During storing message to send, could not add message due invalid format from missing start or end indicator. Message: '" + strMsg + "'.");
			}
		}
		catch (exError) {
			
			this.AddLogErrorMsg("During storing message to send, an exception occurred. Exception: " + exError.message);
		}
	},
	StoreReceivedMsg: function(strMsg, nMsgLen) {

		var MSGSTARTINDICATORLEN = this.GetMsgStartIndicatorLength(),
			MSGENDINDICATORLEN = this.GetMsgEndIndicatorLength();
									/* Length of Message Start and End Indicator */
			nMsgStartIndex = 0,		
			nMsgEndIndex = 0,		/* Starting and Ending Index of Message */
			nIndexLen = 0,			/* Length Between Index */
			boolHasStart = true,	/* Indicator That Message Has Start Index */
			boolHasEnd = true,		/* Indicator That Message Has End Index */
			nQueueCount = this.DebugReceivedStoredQueueCount(),
									/* Queue Count */
			amsiListQueue = this.amsiListStoredReceived,
									/* Stored Received Message List */
			nMsgCount = amsiListQueue.length,	
									/* Count of Stored Received Message List */
			nLastMsgEndIndex = -1,	/* Index for Selected Message Information Ending Record */
			nCounter = 0;			/* Counter for Loop */
		
		try {

			if (strMsg && strMsg.trim() != "") {

				if (!Number.isInteger(nMsgLen)) {
					
					nMsgLen = strMsg.length;
				}

				/* Find If Initial Message Exists */
				nMsgStartIndex = MESSAGE.FindStringStartIndex(strMsg, nMsgLen);
				nMsgEndIndex = MESSAGE.FindStringEndIndex(strMsg, nMsgLen);
			
				/* If it Contains a Full Message, Get All of Message */
				if (nMsgStartIndex < 0 || (nMsgEndIndex > 0 && nMsgStartIndex >= nMsgEndIndex)) {

					nMsgStartIndex = 0;
					boolHasStart = false;
				}
			
				if (nMsgEndIndex >= 0) {
					
					nMsgEndIndex += (MSGENDINDICATORLEN - 1);
				}
				else {

					/* If No Ending of Message, Get All of It */					
					nMsgEndIndex = nMsgLen - 1;
					boolHasEnd = false;
				}

				nIndexLen = (nMsgEndIndex - nMsgStartIndex) + 1;
				
				if (nIndexLen > 0) {
					
					for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
		
						if (amsiListQueue[nCounter].HasEnd()) {
							
							nLastMsgEndIndex = nCounter;
						}
					}
					
					if (boolHasStart && nMsgCount > 0 && !amsiListQueue[nMsgCount - 1].HasEnd()) {
						
						if (nLastMsgEndIndex >= 0) {
	
							this.amsiListStoredReceived.splice(nLastMsgEndIndex + 1);
						}
						else {
							
							this.amsiListStoredReceived = [];
						}
						
						boolMsgDropped = true;
					}
						
					if (boolHasStart && nQueueCount < this.nQueueMsgLimit) {

						this.amsiListStoredReceived.push(MESSAGE.CreateMsg(strMsg.substring(nMsgStartIndex), nIndexLen));
					}
					else {
						
						boolMsgDropped = true;
					}
					
					if (nMsgLen - (nMsgEndIndex + 1) > 0) {
						
						this.StoreReceivedMsg(strMsg.substring(nMsgEndIndex + 1), nMsgLen - (nMsgEndIndex + 1));
					}
				}
			}
		}
		catch (exError) {
		
			this.AddLogErrorMsg("During storing received message to queue, an exception occurred. Exception: " + exError.message);
		}
	},
	MoveSentMsgsToBackup: function() {

		var amsiListQueue = this.amsiListToSend,
			nMsgCount = amsiListQueue.length,				
									/* Selected Message Queue and Count of Messages in Queue */
			msgSelect,				/* Selected Message Information Record */
			amsgForBackup = [],
			msgBackupSelect,		/* Messages Selected for Backup and Selected Backup Message Information Record */
			llSeqNum = 0,	 		/* Message Sequence Number */
			nQueueCount = 0,		/* Count of Messages in Backup Queue */
			nCounter = 0;			/* Counter for Loop */
		
		try {

			/* Cycle Through Send Queue for Those Message Being Tracked, and Put in a Holder Queue, Before Removing from Send Queue */
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

				msgSelect = amsiListQueue[nCounter];

				/* If Message is Being Tracked */ 
				if (msgSelect.GetMetaDataSeqNum() > 0) { 
	
					/* If First to be Check or In Sequence, Put in Holder Queue */
					if (llSeqNum == 0 || llSeqNum + 1 == msgSelect.GetMetaDataSeqNum()) {
				
						llSeqNum = msgSelect.GetMetaDataSeqNum();
						
						amsgForBackup.push(msgSelect);
						nQueueCount++;
					}
					else if (llSeqNum < msgSelect.GetMetaDataSeqNum()) {

						/* Else Found a Missing Message from Send Queue, Clear Holder Queue, and Put in Next After Missing */
						this.AddLogErrorMsg("During storing backup messages, messages missing from send queue. Dropping any messages before missing ones.");

						amsgForBackup = [msgSelect];
						nQueueCount = 1;
					}
					else {
					
						/* Else Found a Old Message from Send Queue Already Passed Sequence in Holder, Do Not Add to Holder Queue */
						this.AddLogErrorMsg("During storing backup messages, old message found in send queue. Dropping message.");
					}
				}
			}
			
			this.amsiListToSend = [];

			/* If Holder Queue Has Messages */
			if (nQueueCount) {
				
				amsiListQueue = this.amsiListBackupSent;
				nMsgCount = amsiListQueue.length;
				msgSelect = amsiListQueue[nMsgCount - 1];
				
				if (nMsgCount) {

					/* If Holder Has Next in Sequence, Add to End of Backup Queue */
					if (msgSelect.GetMetaDataSeqNum() + 1 == amsgForBackup[0].GetMetaDataSeqNum()) {

						this.amsiListBackupSent = amsiListQueue.concat(amsgForBackup);
						nQueueCount += nMsgCount;
					}
					else if (msgSelect.GetMetaDataSeqNum() < amsgForBackup[0].GetMetaDataSeqNum()) {

						/* Else Found Old Messages from Backup Queue, Clear Backup Queue, and Replace with Holder Queue */
						this.AddLogErrorMsg("During storing backup messages, messages missing from backup queue. Dropping current messages from backup queue.");

						this.amsiListBackupSent = amsgForBackup;
						nQueueCount = nMsgCount;
					}
					else {
					
						/* Else Found Old Messages Holder Queue, Not Adding to Backup Queue */
						this.AddLogErrorMsg("During storing backup messages, messages being put into backup queue are old. Backup failed.");
					}
				}
				else {
		
					this.amsiListBackupSent = amsgForBackup;
					nQueueCount = nMsgCount;
				}
			}

			/* Remove Oldest Messages from the Backup Queue When Over Limit */
			if (nQueueCount > this.nQueueMsgLimit) {

				this.amsiListBackupSent.splice(0, this.nQueueMsgLimit);
			}
		}
		catch (exError) {

			this.AddLogErrorMsg("During storing backup messages, an exception occurred. Exception: " + exError.message);
		}
	},
	SendMsg: function() {

		var amsiListQueue = this.amsiListToSend,
			nMsgCount = amsiListQueue.length,				
									/* Selected Message Queue and Count of Messages in Queue */
			strWholeSend = "",		/* Total Message to be Sent */
			boolNoError = true;		/* Indicator That There was Not a Valid Error */

		try {
				
			if (nMsgCount) {
				
				for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

					strWholeSend += amsiListQueue[nCounter].GetMessage();
				}

				this.wsServerConnect.send(strWholeSend);
				this.MoveSentMsgsToBackup();	
			}
		}
		catch (exError) {

			this.AddLogErrorMsg("During dequeuing and sending message, an exception occurred. Exception: " + exError.message);
			boolNoError = false;
		}

		return boolNoError;
	},
	AddSendMsg: function(strMsgTypeName, astrMsgParams, boolTrack) {

		var MSGPARTINDICATOR = this.GetMsgPartIndicator(),
										/* Message Part Indicator */
			strMsg = strMsgTypeName,	/* Message to be Sent */
			nMsgParamsLen = 0,			/* Number of Message Parameters */
			boolMsgAdded = false,		/* Indicator That Message was Added */
			nCounter = 0; 				/* Counter for Loop */

		try {

			if (typeof(astrMsgParams) == 'object' && Array.isArray(astrMsgParams)) {
				
				nMsgParamsLen = astrMsgParams.length;
			}

			if (boolTrack || typeof(boolTrack) != 'boolean') {

				strMsg += "-" + (++this.llSendMsgs);
			}

			/* Part Together Message to be Sent with Indicators for Parameters and Added the End Afterwards */
			for (nCounter = 0; nCounter < nMsgParamsLen; nCounter++) {
	
				 strMsg += MSGPARTINDICATOR + astrMsgParams[nCounter];
			}

			boolMsgAdded = this.EnqueueSendMsg(this.GetMsgStartIndicator() + strMsg + this.GetMsgEndIndicator());
		}
		catch (exError) {

			this.AddLogErrorMsg("During adding 'Send To' message of type, '" + strMsgTypeName + "', an exception occurred. Exception: " + exError.message);
		}

		return boolMsgAdded;
	},
	AddStoredMsgs: function() {
		
		var amsiListQueue = this.amsiListToSend,
			nMsgCount = amsiListQueue.length;				
									/* Selected Message Queue and Count of Messages in Queue */

		if (nMsgCount.length > 0 && 
			(amsiListQueue[nMsgCount - 1].IsComplete() || amsiListQueue[nMsgCount - 1].HasEnd())) {
				
			this.amsiListToSend = amsiListQueue.concat(this.amsiListStoredToSend);
			this.amsiListStoredToSend = [];
		}
		
		amsiListQueue = this.amsiListReceived,
		nMsgCount = amsiListQueue.length;				
		
		if (nMsgCount.length > 0 && 
			(amsiListQueue[nMsgCount - 1].IsComplete() || amsiListQueue[nMsgCount - 1].HasEnd())) {
			
			this.amsiListReceived = amsiListQueue.concat(this.amsiListStoredReceived);
			this.amsiListStoredReceived = [];
		}

		if (this.amsiListStoredToSend.length && this.amsiListStoredReceived.length && this.boolMsgDropped) {
		
			this.AddLogErrorMsg("During adding stored messages, discovered that messages were dropped while store queues were full.");
			this.boolMsgDropped = false;
		}
	},
	SendPeerToPeerMsg: function(strMsgTypeName, astrMsgParams) {

		var MSGPARTINDICATOR = this.GetMsgPartIndicator(),
									/* Message Part Indicator */
			nMsgParamsLen = astrMsgParams.length,
									/* Number of Message Parameters */
			strMsg = strMsgTypeName,/* Message to be Sent */
			appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0; 			/* Counter for Loop */		
		
		/* Part Together Message to be Sent with Indicators for Parameters and Added the End Afterwards */
		for (nCounter = 0; nCounter < nMsgParamsLen; nCounter++) {

			 strMsg += MSGPARTINDICATOR + astrMsgParams[nCounter];
		}

		strMsg = this.GetMsgStartIndicator() + strMsg + this.GetMsgEndIndicator();	

		for (nCounter = 0; nCounter < nListCount; nCounter++) {

			appciConnected[nCounter].Send(strMsg, true);
		}
	},
	ProcessReplay: function() {

		var amsgList = this.DequeueReceivedMsg("MSGREPLAY"),		
									/* Message for Getting Message Replay */
//			msgSelect,				/* Selected Message Information */
			amsiListQueue = this.amsiListBackupSent,
			nMsgCount = amsiListQueue.length,				
									/* Selected Message Queue and Count of Messages in Queue */
			strWholeSend = "";		/* Total Message to be Sent */
//			nSeqReplayNum = 0,		/* Message Sequence Number for Start of Replay */

		try {

			if (amsgList) {
			
				var msgSelect,
					nSeqReplayNum = parseInt(msgSelect.GetSegment(1));

				for (nCounter = 0; nCounter < nMsgCount; nCounter++) {

					msgSelect = amsiListQueue[nCounter];
					
					if (msgSelect.GetMetaDataSeqNum() >= nSeqReplayNum) { 

						strWholeSend += amsiListQueue[nCounter].GetMessage();
					}
				}

				if (strWholeSend != "") {
					
					this.wsServerConnect.send(strWholeSend);
				}
				else {
				
					this.AddLogErrorMsg("During sending replay messages, no replay messages found, replay failed.");
				}
			}
		}
		catch (exError) {

			this.AddLogErrorMsg("During sending replay messages, an exception occurred. Exception: " + exError.message);
		}
	},
	ProcessPing: function() {

		var amsgList = this.DequeueReceivedMsg("PINGSEND");			
									/* Message for Send Ping */

		if (amsgList) {

			this.AddSendMsg("PINGRETURN", [amsgList[0].GetSegment(1)], false);
		}
	},
	FindReceivedMsg: function(strMsgTypeName, mxMsgCritSearch, boolDelete) {
		
		var MSGSTARTINDICATOR = this.GetMsgStartIndicator(),
			MSGPARTINDICATOR = this.GetMsgPartIndicator(),
			strSearchInMsg = MSGSTARTINDICATOR + strMsgTypeName;
										/* Starting and Part Indicator of Message and Part of Message to Search For */
			boolNotEndFound = true,		/* Indicator That End of Message was Not Found */
			msgSelect = null,			/* Selected Message Information */
			nCheckMsgIndex = -1,		/* Index of Starting Message Information Holder for Checking for Message */
			nStartMsgIndex = -1,		/* Index of Starting Message Information Holder for Message */
			nEndMsgIndex = -1,			/* Index of Ending Message Information Holder for Message */
			aMsgFound = null,			/* Found Message Information Holders */
			strMsgCheck = '',			/* Message for Checking Across Holders */
			amsiListQueue = this.amsiListReceived,
			nMsgCount = amsiListQueue.length,	
										/* Count of Messages in Queue */
			nListCount = 0,				/* Count of Message Criteria */
			boolCriteriaMatch = true,	/* Indicator That Message Criteria Matches Those from Search */
			nCounter = 0, 				/* Counter for Loop */
			nCriteraCounter = 0;		/* Counter for Criteria Loop */	

		if (typeof(mxMsgCritSearch) == "object") {

			nListCount = mxMsgCritSearch.length;
			
			for (nCounter = 0; nCounter < nListCount; nCounter++) {

				strSearchInMsg += MSGPARTINDICATOR + mxMsgCritSearch[nCounter];
			}
		}
		else if (typeof(mxMsgCritSearch) == "string" || 
				 typeof(mxMsgCritSearch) == "number") {

			strSearchInMsg += MSGPARTINDICATOR + mxMsgCritSearch;
			mxMsgCritSearch = [mxMsgCritSearch];
			nListCount = 1;
		}

		if (strMsgTypeName != "STREAMFILE") {

			for (nCounter = 0; nCounter < nMsgCount && boolNotEndFound; nCounter++) {

				msgSelect = amsiListQueue[nCounter];
		
				if (nStartMsgIndex <= -1) {
					
					for (nCriteraCounter = 0; nCriteraCounter < nListCount && boolCriteriaMatch; nCriteraCounter++) {
						
						boolCriteriaMatch = msgSelect.GetSegment(nCounter + 1).indexOf(mxMsgCritSearch[nCounter]) == 0
					}
					
					if (boolCriteriaMatch && msgSelect.GetSegment(0).indexOf(strMsgTypeName) == 0) {
						
						nStartMsgIndex = nCounter;
						
						if (msgSelect.IsComplete()) {
							
							nEndMsgIndex = nCounter;
							boolNotEndFound = false;	
						}
					}
					else if (msgSelect.HasStart() || strMsgCheck.length) {
					
						if (msgSelect.HasStart()) {
							
							if (msgSelect.FindMetaDataInString() >= 0) {
						
								strMsgCheck = msgSelect.GetMessage();
								strMsgCheck = strMsgCheck.substring(0, msgSelect.FindMetaDataInString()) +
											  strMsgCheck.substring(strMsgCheck.indexOf(MSGPARTINDICATOR + msgSelect.GetSegment(1)));
							}
							else {
								
								strMsgCheck = msgSelect.GetMessage();
							}
							
							nCheckMsgIndex = nCounter;
						}
						else {
							
							strMsgCheck += msgSelect.GetMessage();
							
							if (strMsgCheck.indexOf(strSearchInMsg) > -1) {

								nStartMsgIndex = nCheckMsgIndex;
								
								if (msgSelect.HasEnd()) {
									
									nEndMsgIndex = nCounter;
									boolNotEndFound = false;	
								}
								
								strMsgCheck = '';
								nCheckMsgIndex = -1;
							}
							else if (strMsgCheck.length >= strSearchInMsg.length) {
								
								strMsgCheck = '';
								nCheckMsgIndex = -1;
							}
						}
					}
					
					boolCriteriaMatch = true;
				}
				else if (msgSelect.HasEnd()) {
					
					nEndMsgIndex = nCounter;
					boolNotEndFound = false;	
				}
			}
			
			/* If Beginning and End of Message was Found, Get Message, and Remove the Processed Message Information If Directed To */
			if (nStartMsgIndex >= 0 && nEndMsgIndex >= 0) {

				aMsgFound = amsiListQueue.slice(nStartMsgIndex, nEndMsgIndex + 1);
					
				if (boolDelete) {

					amsiListQueue.splice(nStartMsgIndex, (nEndMsgIndex - nStartMsgIndex) + 1);
				}
			}
		}
		else {

			aMsgFound = this.ProcessFile(mxMsgCritSearch);
		}
		
		return aMsgFound;
	},
	DequeueReceivedMsg: function(strMsgTypeName, mxMsgCritSearch) {
	
		return this.FindReceivedMsg(strMsgTypeName, mxMsgCritSearch, true);
	},
	DequeueStoredMsg: function(mxMsgTypeID, mxMsgCritSearch) {
		
		var MSGSTARTINDICATOR = this.GetMsgStartIndicator(),
			MSGPARTINDICATOR = this.GetMsgPartIndicator(),
			strSearchInMsg = MSGSTARTINDICATOR + mxMsgTypeID;
										/* Starting and Part Indicator of Message and Part of Message to Search For */
			amsiListQueue = this.amsiListStoredReceived,
			nMsgCount = amsiListQueue.length,	
			msgSelect = null,			/* Selected Message Information */
			nStartMsgIndex = -1,		/* Index of Starting Message Information Holder for Message */
			nEndMsgIndex = -1,			/* Index of Ending Message Information Holder for Message */
			aMsgFound = null,			/* Found Message Information Holders */
			boolNotEndFound = true;		/* Indicator That End of Message was Not Found */
			nCounter = 0;				/* Counter for Loop */

		try {

			if (typeof(mxMsgTypeID) == "string") {
				
				if (typeof(mxMsgCritSearch) == "object") {
	
					nListCount = mxMsgCritSearch.length;
					
					for (nCounter = 0; nCounter < nListCount; nCounter++) {
	
						strSearchInMsg += MSGPARTINDICATOR + mxMsgCritSearch[nCounter];
					}
				}
				else if (typeof(mxMsgCritSearch) == "string" || 
						 typeof(mxMsgCritSearch) == "number") {
	
					strSearchInMsg += MSGPARTINDICATOR + mxMsgCritSearch;
				}
				
				for (nCounter = 0; nCounter < nMsgCount && boolNotEndFound; nCounter++) {
	
					msgSelect = amsiListQueue[nCounter];
	
					if (nStartMsgIndex <= -1) {
	
						if (msgSelect.FindInMsg(strSearchInMsg) > -1) { 
	
							/* The First Part of the Message was Found, Grab the Message Information That it as in,
								Check for the End of the Message in the Message Information */
							nStartMsgIndex = nCounter; 
			
							if (msgSelect.IsComplete()) {
						
								/* End of Message was Found in the Same Message Information, Grab the Message Information it was in, Set Indicator */
								nEndMsgIndex = nCounter;
								boolNotEndFound = false;
							}
						}
					}
					else if (msgSelect.HasEnd()) {
	
						nEndMsgIndex = nCounter;
						boolNotEndFound = false;	
					}
				}
			}
			else if (typeof(mxMsgTypeID) == "number") {
				
				for (nCounter = 0; nCounter < nMsgCount && boolNotEndFound; nCounter++) {
	
					msgSelect = amsiListQueue[nCounter];
	
					if (nStartMsgIndex <= -1) {
	
						if (msgSelect.GetMetaDataSeqNum() == mxMsgTypeID && msgSelect.HasStart()) { 
	
							/* The First Part of the Message was Found, Grab the Message Information That it as in,
								Check for the End of the Message in the Message Information */
							nStartMsgIndex = nCounter; 
			
							if (msgSelect.IsComplete()) {
						
								/* End of Message was Found in the Same Message Information, Grab the Message Information it was in, Set Indicator */
								nEndMsgIndex = nCounter;
								boolNotEndFound = false;
							}
						}
					}
					else if (msgSelect.HasEnd()) {
	
						nEndMsgIndex = nCounter;
						boolNotEndFound = false;	
					}
				}			
			}
			
			/* If Beginning and End of Message was Found, Remove the Processed Message Information If Directed To */
			if (nStartMsgIndex >= 0 && nEndMsgIndex >= 0) {

				aMsgFound = amsiListQueue.splice(nStartMsgIndex, (nEndMsgIndex - nStartMsgIndex) + 1);
			}
		}
		catch (exError) {

			this.AddLogErrorMsg("During dequeuing stored message type or sequence ID, '" + mxMsgTypeID + "', an exception occurred. Exception: " + exError.message);
		}
	
		return aMsgFound;
	},
	ClearReceivedMsg: function(strMsgTypeName, mxMsgCritSearch) {

		while (this.DequeueReceivedMsg(strMsgTypeName, mxMsgCritSearch)) {} 

		return true;
	},
	GetMsg: function(aMsgList, nMsgIndex) {

		var strMsg = null;			/* Selected Message */

		try {

			strMsg = aMsgList[nMsgIndex].GetMessage();
		}
		catch (exError) {

			this.AddLogErrorMsg("During getting message, an exception occurred. Exception: " + exError.message);
		}

		return strMsg;
	},
	ProcessFile: function(mxMsgCritSearch) {
	
		var MSGPARTINDICATOR = this.GetMsgPartIndicator(),
			MSGPARTINDICATORLEN = this.GetMsgPartIndicatorLength(),
									/* Message Part Indicator and its Length */
			msgSelect = null,		/* Selected Message Information */
			strFileDesign = null,	/* File Designation */
			strFilePath = null, 	/* File Path */
			nFileLen = 0,			/* Length of File */
			amsgParts = null,		/* List of File Part Messages */
			amsgTotal = null,		/* List of All File Messages */
			msgSelect = null,		/* Selected Message Information */
			nMsgNumTest = 0,		/* File Part Number to Test */
			nMsgNumLast = 0,		/* Last File Part Number to Test */
			strErrorMsg = "",		/* Error Message */
			boolPartsDone = false,	/* Indicator That All Parts of the File Have Been Collected */
			boolContinue = true;	/* Indicator to Continue Processing File */

		/* If the Beginning and End of the Expected Message Exists */
		if (this.FindReceivedMsg("FILESTART", mxMsgCritSearch, false) &&
			this.FindReceivedMsg("FILEEND", mxMsgCritSearch, false)) {
			
			amsgTotal = this.DequeueReceivedMsg("FILESTART", mxMsgCritSearch);
			msgSelect = amsgTotal[0];

			strFileDesign = msgSelect.GetSegment(1);
			nFileDesignLen = strFileDesign.length;

			strFilePath = msgSelect.GetSegment(2);	
			nFilePathLen = strFilePath.length;

			nFileLen = parseInt(msgSelect.GetSegment(3));

			amsgParts = this.DequeueReceivedMsg("FILEPART", mxMsgCritSearch);

			while (boolContinue && amsgParts) {
					
				msgSelect = amsgParts[0];
				
				nMsgNumTest = parseInt(msgSelect.GetSegment(3));

				if (msgSelect.GetSegment(1).indexOf(strFileDesign) >= 0 &&
					msgSelect.GetSegment(2).indexOf(strFilePath) >= 0 &&
					nMsgNumTest > nMsgNumLast && 
					nFileLen > nMsgNumTest) {

					amsgTotal = amsgTotal.concat(amsgParts);
					nMsgNumLast = nMsgNumTest;
					
					if (boolPartsDone || !(amsgParts = this.DequeueReceivedMsg("FILEPART", mxMsgCritSearch))) {
						
						amsgParts = this.DequeueReceivedMsg("FILEEND", mxMsgCritSearch);
						boolPartsDone = true;
					}
				}
				else if (!boolPartsDone) {
					
					strErrorMsg = "During processing file from stream, getting file parts failed due out of order messages. File type: '" + strFileDesign + "' messages deleted.";
					boolContinue = false;
				}
				else {
					
					strErrorMsg = "During processing file from stream, getting file end failed due out of order messages. File type: '" + strFileDesign + "' messages deleted.";
					boolContinue = false;
				}
			}

			if (!boolContinue) {

				this.AddLogErrorMsg(strErrorMsg);

				while (this.DequeueReceivedMsg("FILEPART", mxMsgCritSearch)) {}

				while (this.DequeueReceivedMsg("FILEEND", mxMsgCritSearch)) {}
			}
		}

		return amsgTotal;
	},
	CheckFile: function(mxMsgCritSearch) {
		
		var MSGSTARTINDICATOR = this.GetMsgStartIndicator(),
			MSGPARTINDICATOR = this.GetMsgPartIndicator();
									/* Start and Part Message Indicators */
			strMsgCritSearch = "",	/* Criteria Added to Message Information to Search for it */	
			amsiListQueue = this.amsiListReceived,
			nMsgCount = amsiListQueue.length,	
			nFileLen = 0,			/* Length of File Contents */
			nCurrentLen = 0;		/* Length of Currently Found File Contents */		
			msgSelect,				/* Selected Message Information */
			boolStartFound = false,
			boolEndFound = false,	/* Indicator That Start and End of File was Found */
			nListCount = 0,
			nCounter = 0; 				/* Counter for Loop */	

		if (typeof(mxMsgCritSearch) == "object" && Array.isArray(mxMsgCritSearch)) {
	
			nListCount = mxMsgCritSearch.length;
			
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
	
				strMsgCritSearch += MSGPARTINDICATOR + mxMsgCritSearch[nCounter];
			}
		}
		else if (typeof(mxMsgCritSearch) == "string" || 
				 typeof(mxMsgCritSearch) == "number") {
	
			strMsgCritSearch += MSGPARTINDICATOR + mxMsgCritSearch;
		}
		
		for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
			msgSelect = amsiListQueue[nCounter];
		
			if (msgSelect.FindInMsg(MSGSTARTINDICATOR + "FILESTART" + MSGPARTINDICATOR + strMsgCritSearch) > -1) {
			
				nCurrentLen += parseInt(msgSelect.GetSegment(4));
				boolStartFound = true;
			}
			else if (msgSelect.FindInMsg(MSGSTARTINDICATOR + "FILEPART" + MSGPARTINDICATOR + strMsgCritSearch) > -1) {
			
				nCurrentLen += parseInt(msgSelect.GetSegment(4));
			}
			else if (msgSelect.FindInMsg(MSGSTARTINDICATOR + "FILEEND" + MSGPARTINDICATOR + strMsgCritSearch) > -1) {
			
				nCurrentLen += parseInt(msgSelect.GetSegment(4));
				boolEndFound = true;
			}
		}

		if (boolStartFound && boolEndFound) {
		
			nFileLen = nCurrentLen;
		}
		
		return nFileLen;
	},
	AddLogErrorMsg: function(strErrorMsg) {

		this.AddErrorMsg("LOGERRORMSG", strErrorMsg);
	},
	AddDisplayErrorMsg: function(strErrorMsg) {

		this.AddErrorMsg("DISPLAYERRORMSG", strErrorMsg);
	},
	AddReplacementErrorMsg: function(strLogErrorMsg, strDisplayErrorMsg) {
	 
		this.AddErrorMsg("LOGERRORMSG", strLogErrorMsg);
		this.AddErrorMsg("DISPLAYERRORMSG", strDisplayErrorMsg);
	},
	AddErrorMsg: function(strErrorMsgType, strErrorMsg) {
		
		var MSGSTARTINDICATOR = this.GetMsgStartIndicator(),
			MSGPARTINDICATOR = this.GetMsgPartIndicator(),
			MSGENDINDICATOR = this.GetMsgEndIndicator();
									/* Message Start, Part and End Indicator */
			MSGSTARTINDICATORLEN = this.GetMsgStartIndicatorLength(),
			MSGPARTINDICATORLEN = this.GetMsgPartIndicatorLength(),
			MSGENDINDICATORLEN = this.GetMsgEndIndicatorLength(),
									/* Length of Message Start, Part and End Indicator */
			MSGRECEIVEDADDLEN = MSGSTARTINDICATORLEN + MSGPARTINDICATORLEN + MSGENDINDICATORLEN + strErrorMsgType.length,
									/* Additional Message Information for Receiving */
			MSGSENDINGADDLEN = MSGSTARTINDICATORLEN + MSGPARTINDICATORLEN + MSGENDINDICATORLEN + 11,
									/* Additional Message Information for Sending */
			nIndex = 0;

		try {

			if (strErrorMsg.indexOf(MSGSTARTINDICATOR + "LOGERRORMSG") < 0 && 
				strErrorMsg.indexOf(MSGSTARTINDICATOR + "DISPLAYERRORMSG") < 0 && 
				strErrorMsg.indexOf(MSGSTARTINDICATOR + "CLIENTERROR") < 0) {
				
				strErrorMsg.replace(/MSGSTARTINDICATOR/g, " ");
				strErrorMsg.replace(/MSGPARTINDICATOR/g, " ");
				strErrorMsg.replace(/MSGENDINDICATOR/g, " ");

				/* If it is a Display Error Message, Don't Send to Server */
				if (strErrorMsgType != "DISPLAYERRORMSG") {
		
					if (console) {
	
						console.log(strErrorMsg);
					}
					
					if (strErrorMsg.length() + MSGSENDINGADDLEN <= this.BUFFERSIZE) {
		
						this.StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + strErrorMsg + MSGENDINDICATOR);
					}
					else {
				
						this.StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + strErrorMsg.substr(0, this.BUFFERSIZE - MSGSENDINGADDLEN - 4) + "..." + MSGENDINDICATOR);
					}
				}
				else {
					
					postMessage({TYPE: "ERROR",
								 MESSAGE: strErrorMsg});
				}
			}
			else {

				if (console) {
					
					console.log("Error in sending '" + strErrorMsgType  + "' message: '" + strLogErrorMsg  + "'"); 
				}
				
				if (strErrorMsgType != "DISPLAYERRORMSG") {

					this.StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + "Sending error message failed." + MSGENDINDICATOR);
				}
			}
		}
		catch (exError) {

			this.StoreReceivedMsg(MSGSTARTINDICATOR + strErrorMsgType + MSGPARTINDICATOR + exError.message + MSGENDINDICATOR);

			if (strErrorMsgType != "DISPLAYERRORMSG") {

				this.StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + exError.message + MSGENDINDICATOR);
			}
		}
	},
	Close: function() {

		clearInterval(this.nComTimerID);
		clearInterval(this.nPeerToPeerComTimerID);
		
		this.ClosePeerToPeer();
		
		if (this.wsServerConnect != null) {

			try {

				this.wsServerConnect.close();
			}
			catch(exError) {
				
				this.AddLogErrorMsg("During closing client, closing client connection failed. Exception: " + exError.message);
			}
			
			this.wsServerConnect = null;
		}
		
		this.boolConnected = false;
	},
	ClosePeerToPeer: function() {
		
		var appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0;

		for (nCounter = 0; nCounter < nListCount; nCounter++) {
			
			appciConnected[nCounter].CloseClient();
		}
				
		/* Close "Peer To Peer" Server Socket, and 
			Go Through Clients Closing Connections */
		if (this.wsPeertoPeerConnect != null) {
			
			try {

				this.wsPeertoPeerConnect.close();
			}
			catch(exError) {
				
				this.AddLogErrorMsg("During closing 'Peer-to-Peer' client, closing connections failed. Exception: " + exError.message);
			}
			
			this.wsPeertoPeerConnect = null;
		}
	},
	AddStreamMsg: function(nTransID, strMsg) {
		
		if (!this.AddSendMsg("ADDSTREAMMSG", [nTransID, strMsg], true)) {
				
			this.AddLogErrorMsg("Sending message 'ADDSTREAMMSG' with transaction ID: '" + nTransID + "' and message: '" + strMsg + "' failed failed.");
		}
	},
	StartHTTPPost: function(nNewTransID, strHostNameIP, nPort, boolAsync) {
		
		if (boolAsync) {
			
			if (!this.AddSendMsg("STARTHTTPPOSTASYNC", [nNewTransID, strHostNameIP, nPort], true)) {
				
				this.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + nNewTransID + "', host: '" + strHostName + "', port: " + nPort + " failed.");
			}	
		}
		else if (!this.AddSendMsg("STARTHTTPPOSTSYNC", [nNewTransID, strHostNameIP, nPort], true)) {
				
			this.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + nNewTransID + "', host: '" + strHostName + "', port: " + nPort + " failed.");
		}	
	},
	StartHTTPGet: function(nNewTransID, strHostNameIP, nPort, boolAsync) {
		
		if (boolAsync) {
			
			if (!this.AddSendMsg("STARTHTTPGETASYNC", [nNewTransID, strHostNameIP, nPort], true)) {
				
				this.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + nNewTransID + "', host: '" + strHostName + "', port: " + nPort + " failed.");
			}	
		}
		else if (!this.AddSendMsg("STARTHTTPGETSYNC", [nNewTransID, strHostNameIP, nPort], true)) {
				
			this.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + nNewTransID + "', host: '" + strHostName + "', port: " + nPort + " failed.");
		}	
	},
	StartStream: function(nNewTransID, strHostNameIP, nPort) {

		if (!this.AddSendMsg("STARTSTREAM", [nNewTransID, strHostNameIP, nPort], true)) {
			
			this.AddLogErrorMsg("Sending message 'STARTSTREAM' with transaction ID: '" + nNewTransID + "', host: '" + strHostName + "', port: " + nPort + " failed.");
		}
	},
	SendDirectRawMsg: function(strMsg, strMsgDesign, boolSendServer, boolSendPeerToPeer) {
		
		if (!strMsgDesign) {
			
			strMsgDesign = "";
		}
					
		if (typeof(boolSendServer) != 'boolean') {
			
			boolSendServer = true;
		}
		
		if (typeof(boolSendPeerToPeer) != 'boolean') {
			
			boolSendPeerToPeer = false;
		}
		
		if (boolSendServer) {

			if (!this.AddSendMsg("DIRECTMSG", [strMsgDesign, strMsg], true)) {
						
				this.AddLogErrorMsg("Sending message 'DIRECTMSG', '" + strMsgDesign + "' and message: '" + strMsg + "' failed.");
			}
		}
		
		if (boolSendPeerToPeer) {

			if (!this.SendPeerToPeerMsg("DIRECTMSG", [strMsgDesign, strMsg], true)) {
						
				this.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, '" + strMsgDesign + "' and message: '" + strMsg + "' failed.");
			}
		}
	},
	SendHTTP: function(nTransID, nNewRespID, boolAutoRetrieval) {

		var aHTTPReceivers = this.objReceivers['HTTP'];
									/* HTTP Receivers */
		
		if (this.AddSendMsg("SENDHTTP", [nTransID, nNewRespID], true)) {
			
			if (boolAutoRetrieval) {
				
                if (!(aHTTPReceivers[nTransID] && aHTTPReceivers[nTransID][nNewRespID])) {

                	aHTTPReceivers[nTransID] = {};
                }
                
                if (!aHTTPReceivers[nTransID][nNewRespID]) {

                	aHTTPReceivers[nTransID][nNewRespID] = {RECEIVER: null, ACTIVE: true};
                }
                else {
                
                	this.AddLogErrorMsg("During setting up of HTTP transmission auto retrieval for transaction ID, " + nTransID +  
    									", and response ID, " + nNewRespID + ", auto retrieval was already running.");                        
                }
                   
                aHTTPReceivers[nTransID][nNewRespID]['RECEIVER'] = setInterval(new Function("												\
            		var nTransID = " + nTransID + ",																						\
            					/* Transmission ID */																						\
            			nRespID = " + nNewRespID + ",																						\
            					/* Response ID */																							\
                        fAutoRetLimitInMillis = " + this.AutoRetLimitInMillis() + ",														\
            					/* Time Limit in Milliseconds for Retrival */																\
                        boolAutoRetProcessCmd = " + this.AutoRetProcessCmd() + ",															\
            					/* Indicator to Process Response as Client Message Commands */												\
                        boolAutoRetEndTrans = " + this.AutoRetEndTrans() + ",																\
            					/*  Indicator to Close HTTP Transmission */																	\
                        nStartTimeInMillis = " + new Date().getTime() + ";																	\
                                /* Start Time of Execution */ 																				\
                        																													\
                     /* Continously Check Until Response from Server is Received */															\
                     if (CLIENT.IsConnected() && new Date().getTime() - nStartTimeInMillis < fAutoRetLimitInMillis) {						\
                          																													\
                          if (CLIENT.GetHTTPResponse(nTransID, nRespID, boolAutoRetProcessCmd) != '' && boolAutoRetEndTrans) {				\
                            	                     																						\
                              CLIENT.TranClose(nTransID); 																					\
                          }																													\
                     }																														\
                     else {																													\
																																			\
					       CLIENT.TranClose(nTransID);  																					\
					 }"), 100);
			}
		}
		else {
			
			this.AddLogErrorMsg("Sending message 'SENDHTTP' with transaction ID: " + nTransID + " with response ID: " + 
								nNewRespID + " failed.");
		}		
	},
	SendDataProcess: function(nTransID, nNewRespID, boolAsync, boolAutoRetrieval, strDesignation, aobjParams) {
		
        var aDataProcessReceivers = this.objReceivers['DATAPROCESS'];
									/* Data Process Receivers */
        	boolNoError = true,     /* Indicator That No Error Has Occurred */
        	nParamCount = 0;		/* Parameter Count */
        
        if (Array.isArray(aobjParams) && (nParamCount = aobjParams.length)) {
        	 
        	for (var nCounter = 0; nCounter < nParamCount && boolNoError; nCounter++) {
 		
        		if (!(boolNoError = this.AddSendMsg("REGDATAEXEC", [nTransID, strDesignation, aobjParams[nCounter].NAME, aobjParams[nCounter].VALUE], true))) {
       			
        			this.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: " + nTransID + ", response ID: " + nNewRespID + ", data designation: '" + strDesignation +  
        								"' with parameters, name: '" + aobjParams[nCounter].NAME + "' and value: '" + aobjParams[nCounter].VALUE + "' failed while registering.");
        		}
        	}
        }
        else if (!(boolNoError = this.AddSendMsg("REGDATAEXEC", [nTransID, strDesignation], true))) {
    	
			this.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: " + nTransID + ", response ID: " + nNewRespID + ", data designation: '" + strDesignation + "' failed while registering.");
    	}
        
        if (boolNoError) {
        	
        	if (!(boolNoError = this.AddSendMsg("PROCESSDATAEXEC", [nTransID, nNewRespID, strDesignation, boolAsync], true))) {
        		
        		this.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: " + nTransID + ", response ID: " + nNewRespID + ", data designation: '" + strDesignation + "' failed.");
        	}
	
	        if (boolAutoRetrieval && boolNoError) {
	        	
                if (!(aDataProcessReceivers[nTransID] && aDataProcessReceivers[nTransID][nNewRespID])) {

                	aDataProcessReceivers[nTransID] = {};
                }
                
                if (!aDataProcessReceivers[nTransID][nNewRespID]) {

                	aDataProcessReceivers[nTransID][nNewRespID] = {RECEIVER: null, ACTIVE: true};
                }
                else {
                
                	this.AddLogErrorMsg("During setting up of data process auto retrieval for transaction ID, " + nTransID +  
    									", and response ID, " + nNewRespID + ", auto retrieval was already running.");                        
                }
                   
                aDataProcessReceivers[nTransID][nNewRespID]['RECEIVER'] = setInterval(new Function("										\
            		var nTransID = " + nTransID + ",																						\
            					/* Transmission ID */																						\
            			nRespID = " + nNewRespID + ",																						\
            					/* Response ID */																							\
                        fAutoRetLimitInMillis = " + this.AutoRetLimitInMillis() + ",														\
            					/* Time Limit in Milliseconds for Retrival */																\
                        boolAutoRetProcessCmd = " + this.AutoRetProcessCmd() + ",															\
            					/* Indicator to Process Response as Client Message Commands */												\
                        boolAutoRetEndTrans = " + this.AutoRetEndTrans() + ",																\
            					/*  Indicator to Close Data Process Transmission */															\
                        nStartTimeInMillis = " + new Date().getTime() + ";																	\
                                /* Start Time of Execution */ 																				\
                        																													\
                     /* Continously Check Until Response from Server is Received */															\
                     if (CLIENT.IsConnected() && new Date().getTime() - nStartTimeInMillis < fAutoRetLimitInMillis) {						\
                          																													\
                          CLIENT.GetDataProcessResponse(nTransID, nRespID, boolAutoRetEndTrans, boolAutoRetProcessCmd);						\
                     }																														\
                     else {																													\
																																			\
						  CLIENT.ReceiverClose('DATAPROCESS', nTransID, nRespID);															\
					 }"), 100);
	        }
        }
	},
	GetHTTPResponse: function(nTransID, nRespID, boolProcessCmd) {
		
		var aMsgList = this.DequeueReceivedMsg("HTTPRESPONSE", [nTransID, nRespID]),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "HTTP",
									 TRANSID: nTransID,
									 RESPID: nRespID,
									 MESSAGE: Message.FindSegmentInString(strMsg, 3),
									 AUTOPROCESS: boolProcessCmd,
									 AUTODELETE: this.AutoRetEndTrans()});
		}
	
		if (strMsg != "") {

			postMessage(strMsg);
		
			if (this.AutoRetEndTrans()) {
	  	  	
				this.ReceiverClose('HTTP', nTransID, nRespID);
			}
		}
		
		return strMsg;
	},
	GetDataProcessResponse: function(nTransID, nRespID, boolDeleteTrans, boolProcessCmd) {
		
		var aMsgList = this.DequeueReceivedMsg("DATAEXECRETURN", [nTransID, nRespID]),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "DATAPROCESS",
									 TRANSID: nTransID,
									 RESPID: nRespID,
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 3),
									 AUTOPROCESS: boolProcessCmd,
									 AUTODELETE: boolDeleteTrans || this.AutoRetEndTrans()});
		}
	
		if (strMsg != "") {
  	
			postMessage(strMsg);
		
			if (boolDeleteTrans || this.AutoRetEndTrans()) {
	  	
				this.ReceiverClose('DATAPROCESS', nTransID, nRespID);
			}
		}
		
		return strMsg;		
	},
	GetStreamMsg: function(nTransID, boolProcessCmd) {
		
		var aMsgList = this.DequeueReceivedMsg("STREAMMSG", [nTransID]),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "STREAMMSG",
									 TRANSID: nTransID,
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 2),
									 AUTOPROCESS: boolProcessCmd});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	GetStreamMsgNext: function(boolProcessCmd) {
		
		var aMsgList = this.DequeueReceivedMsg("STREAMMSG"),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "STREAMMSG",
									 TRANSID: aMsgList[0].GetSegment(1),
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 2),
									 AUTOPROCESS: boolProcessCmd});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	GetDirectMsg: function(strDesign, boolProcessCmd) {
		
		var aMsgList = this.DequeueReceivedMsg("DIRECTMSG", [strDesign]),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
					
			strMsg = JSON.stringify({TYPE: "DIRECTMSG",
									 DESIGNATION: strDesign,
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 2),
									 AUTOPROCESS: boolProcessCmd});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	GetDirectMsgNext: function(boolProcessCmd) {

		var aMsgList = this.DequeueReceivedMsg("DIRECTMSG"),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}

			strMsg = JSON.stringify({TYPE: "DIRECTMSG",
									 DESIGNATION: aMsgList[0].GetSegment(1),
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 2),
									 AUTOPROCESS: boolProcessCmd});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	ClearStreamMsgs: function(nTransID) {
		
		var aParams = [];			/* List of Parameters */
		
		if (nTransID) {
			
			aParams[0] = nTransID;
		}
		
		this.ClearReceivedMsg("STREAMMSG", aParams);
	},
	ClearDirectMsgs: function(strDesign) {
		
		var aParams = [];			/* List of Parameters */
		
		if (strDesign) {
			
			aParams[0] = strDesign;
		}
		
		this.ClearReceivedMsg("DIRECTMSG", aParams);
	},
	FileDownloadStart: function(strFileDesign) {

		if (!this.AddSendMsg("GETSTREAMFILE", [strFileDesign], true)) {
					
			this.AddLogErrorMsg("Sending message 'GETSTREAMFILE', '" + strFileDesign + "' failed.");
		}
	},
	FileDownloadFinish: function(strFileDesign, strFilePathOverride) {
		
		var nFileLen = this.CheckFile(mxMsgCritSearch);
									/* Length of file */
//			aMsgList = this.DequeueReceivedMsg("STREAMFILE", [strFileDesign]),
//			nMsgCount = 0,
//			nCurrentLen = 0,		/* Current Length of Collected Message */
//			strMsg = "",			/* Message List, Count, and Message */
//			aFilePathParts = [],	/* File's Path from Message in Parts */
//			nCounter = 0;			/* Counter for Loop */
		
		if (nFileLen) {
			
			var aMsgList = this.DequeueReceivedMsg("STREAMFILE", [strFileDesign]),
				nMsgCount = 0,
				nCurrentLen = 0,
				strMsg = "",
				nCounter = 0;

			if (aMsgList) {
				
				nMsgCount = aMsgList.length;
				
				for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
				
					strMsg += aMsgList[nCounter].GetSegment(4);
					nCurrentLen += aMsgList[nCounter].GetSegmentLength(4);
				}
				
				if (nFileLen >= nCurrentLen) {
				
					if (typeof(strFilePathOverride) != 'string' || strFilePathOverride == '') {
						
						strFilePathOverride = aMsgList[0].GetSegment(2);
					}
					else {
	                    
	                    if (strFilePathOverride.lastIndexOf('/') != strFilePathOverride.length - 1) {
	          
	                        strFilePathOverride += '/';
	                    }
						
						var aFilePathParts = aMsgList[0].GetSegment(2).split('/');
	
	                    if (aFilePathParts.length > 0) {
	                    
	                        strFilePathOverride += aFilePathParts.pop();
	                    }
					}
					
					strMsg = JSON.stringify({TYPE: "STREAMFILE",
											 DESIGNATION: strFileDesign,
											 MESSAGE: strMsg,
											 FILEPATH: strFilePathOverride,
											 FILELENGTH: nFileLen,
											 AUTOPROCESS: boolProcessCmd});
				}
			
				if (strMsg != "") {
			
					postMessage(strMsg);
				}
			}
			else {
				
				strMsg = "";
				this.AddLogErrorMsg("Getting file: " + strFileDesign + " failed due to invalid collection length of " + nCurrentLen + " of total " + nFileLen + ".");
			}
			
			return strMsg;
		}
	},
	GetAvailableFileList: function() {
		
		var aMsgList = this.FindReceivedMsg("STREAMFILELIST"),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "STREAMFILELIST",
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 1)});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	SetHTTPProcessPage: function(nTransID, strPageURL) {
		
		if (!this.AddSendMsg("SETHTTPPROCESSPAGE", [nTransID, strPageURL], true)) {

			this.AddLogErrorMsg("Sending message 'SETHTTPPROCESSPAGE' with transaction ID: " + nTransID + " with page URL, '" + strPageURL + "', failed.");
		}
	},
	AddHTTPMsgData: function(nTransID, strVarName, strValue) {
		
		if (!this.AddSendMsg("ADDHTTPMSGDATA", [nTransID, strVarName, strValue], true)) {

			this.AddLogErrorMsg("Sending message 'ADDHTTPMSGDATA' with transaction ID: " + nTransID + " with variable name: '" + strVarName + "', variable value: " + strValue + " failed.");
		}
	},
	ClearHTTPMsgData: function(nTransID) {
		
		if (!this.AddSendMsg("CLEARHTTPMSGDATA", [nTransID], true)) {

			this.AddLogErrorMsg("Sending message 'CLEARHTTPMSGDATA' with transaction ID: " + nTransID + " failed.");
		}
	},
	UseHTTPSSL: function(nTransID, boolUseSSL) {

		if (!this.AddSendMsg("USESSL", [nTransID, boolUseSSL], true)) {

			this.AddLogErrorMsg("Sending message 'USESSL' with transaction ID: " + nTransID + " and SSL indicator set to " + boolUseSSL + " failed.");
		}
	},
	TranClose: function(nTransID, strType) {

		if (this.AddSendMsg("CLOSE", [nTransID], true)) {
			
			if (typeof(strType) == 'string' && strType != "") {
				
				this.ReceiverClose(strType, nTransID);
			}
		}
		else {
			
			this.AddLogErrorMsg("Sending message 'CLOSE' with transaction ID: " + nTransID + " failed.");
		}
	},
	ReceiverClose: function(strType, nTransID, nRespID) {
		
		var aTransReceivers = this.objReceivers[strType][nTransID];
									/* List of Transmission's Receivers */
//			aRespIDs = aTransReceivers.keys();
									/* List of Response IDs for Removing all of Transmission's Receivers */
//			nIDCount = aRespIDs.length,
									/* Count of Response IDs for a Transmission */
//			nCounter = 0;			/* Count for Loop */
			
		try {

			if (nRespID) {

				clearInterval(aTransReceivers[nRespID]['RECEIVER']);

				delete this.objReceivers[strType][nTransID][nRespID];
			}
			else {
		
				var aRespIDs = Object.keys(aTransReceivers);
					nIDCount = aRespIDs.length,
					nCounter = 0;
					
				for (nCounter = 0; nCounter < nIDCount; nCounter++) {
					
					clearInterval(aTransReceivers[aRespIDs[nCounter]]['RECEIVER']);
				};
				
				delete this.objReceivers[strType][nTransID];
			}
		}
		catch (exError) {
			
			this.AddLogErrorMsg("Ending receiver for type '" + strType + "', transmission ID, " + nTransID + 
								", and response ID: '" + 
								nRespID + "' failed. Exception: " + exError.message);
		}
	},	
	RunAutoDirectMsgByDesign: function() {
        
        var astrDirectMsgDesignList = this.astrAutoRetDirectMsgDesigns,
									/* List of Direct Message Designations To Run for Execution  */
        	nListCount = astrDirectMsgDesignList.length,
        							/* Count of List of Direct Message Designations To Run for Execution  */
        	boolRunning = nListCount && this.AutoRetProcessCmd(),
        							/* Indicator To Run Process for Executing Certain Direct Messages by Designation */
        	nCounter = 0;			/* Counter for Loop */
        	
        if (boolRunning) {
             
        	for (nCounter < 0; nCounter < nListCount; nCounter++) {
        		
        		this.GetDirectMsg(astrDirectMsgDesignList[nCounter], true);
        	}
        }
        
        return boolRunning;
	},
	SetStreamTranMsgSeparatorChar: function(nTransID, strMsgSeparatorChars) {
		
		if (!this.AddSendMsg("SETSTREAMMSGSEPARATOR", [nTransID], true)) {

			this.AddLogErrorMsg("Sending message 'SETSTREAMMSGSEPARATOR' with transaction ID: " + nTransID + " failed.");
		}
	},
	SetStreamTranEndChars: function(nTransID, strMsgEndChars) {

		if (!this.AddSendMsg("SETSTREAMMSGSTART", [nTransID], true)) {

			this.AddLogErrorMsg("Sending message 'SETSTREAMMSGSTART' with transaction ID: " + nTransID + " failed.");
		}
	},
	SetStreamTranFillerChar: function(nTransID, charMsgFillerChar) {

		if (!this.AddSendMsg("SETSTREAMMSGEND", [nTransID], true)) {

			this.AddLogErrorMsg("Sending message 'SETSTREAMMSGEND' with transaction ID: " + nTransID + " failed.");
		}
	},
	GetLogError: function() {
		
		var aMsgList = this.DequeueStoredMsg("LOGERRORMSG"),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (!aMsgList) {
			
			aMsgList = this.DequeueReceivedMsg("LOGERRORMSG");
		}

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "LOGERRORMSG",
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 1)});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	GetDisplayError: function() {
		
		var aMsgList = this.DequeueStoredMsg("DISPLAYERRORMSG"),
			nMsgCount = 0,
			strMsg = "",			/* Message List, Count, and Message */
			nCounter = 0;			/* Couner for Loop */

		if (!aMsgList) {
			
			aMsgList = this.DequeueReceivedMsg("DISPLAYERRORMSG");
		}

		if (aMsgList) {
			
			nMsgCount = aMsgList.length;
			
			for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
			
				strMsg += aMsgList[nCounter].GetMessage();
			}
			
			strMsg = JSON.stringify({TYPE: "DISPLAYERRORMSG",
									 MESSAGE: MESSAGE.FindSegmentInString(strMsg, 1)});
		}
	
		if (strMsg != "") {
	
			postMessage(strMsg);
		}
		
		return strMsg;
	},
	AddAutoRetDirectMsgDesigns: function(strDesign) {
		
		if (!this.astrAutoRetDirectMsgDesigns.includes(strDesign)) {
			
			this.astrAutoRetDirectMsgDesigns.push(strDesign);
		}
	},
	RemoveAutoRetDirectMsgDesigns: function(strDesign) {

		if (this.astrAutoRetDirectMsgDesigns.includes(strDesign)) {
			
			this.astrAutoRetDirectMsgDesigns.splice(this.astrAutoRetDirectMsgDesigns.indexOf(strDesign), 1);
		}
	},
	SetQueueLimit: function(nNewLimit) {

		var appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0;

		this.nQueueMsgLimit = nNewLimit;

		for (nCounter = 0; nCounter < nListCount; nCounter++) {
						
			appciConnected[nCounter].SetBackupQueueLimit(nNewLimit);
		}
	},
	SetMsgLateLimit: function(nTimeInMillisecs) {

		var appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0;

		this.nTimeToLateInMillis = nTimeInMillisecs;

		for (nCounter = 0; nCounter < nListCount; nCounter++) {
						
			appciConnected[nCounter].SetReceivedMsgLateLimit(nTimeInMillisecs);
		}
	},
	SetDropLateMsgs: function(boolDropLateMsgs) {

		var appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0;

		this.boolRemoveLateMsgs = boolDropLateMsgs;

		for (nCounter = 0; nCounter < nListCount; nCounter++) {
						
			appciConnected[nCounter].SetReceivedDropLateMsgs(boolDropLateMsgs);
		}
	},
	SetActivityCheckTimeLimit: function(nTimeInMillis) {

		var appciConnected = this.appciListClients,
			nListCount = appciConnected.length,
			nCounter = 0;

		this.nTimeToCheckActInMillis = nTimeInMillis;

		for (nCounter = 0; nCounter < nListCount; nCounter++) {
						
			appciConnected[nCounter].SetReceivedCheckTimeLimit(nTimeInMillis);
		}
	},
	AutoRetLimitInMillis: function(fSetAutoRetLimitInMillis) {

		if (typeof(fSetAutoRetLimitInMillis) == 'number') {

			this.fAutoRetLimitInMillis = fSetAutoRetLimitInMillis;
		}
		
		return this.fAutoRetLimitInMillis;
	},
	AutoRetProcessCmd: function(boolSetAutoRetProcessCmd) {

		if (typeof(boolSetAutoRetProcessCmd) == 'boolean') {
		
			this.boolAutoRetProcessCmd = boolSetAutoRetProcessCmd;
		}
		
		return this.boolAutoRetProcessCmd;
	},
	AutoRetEndTrans: function(boolSetAutoRetEndTrans) {

		if (typeof(boolSetAutoRetEndTrans) == 'boolean') {
	    
			this.boolAutoRetEndTrans = boolSetAutoRetEndTrans;
		}
		
		return this.boolAutoRetEndTrans;
	},	
	Debug: function() {
	
        var nMsgCount = this.DebugReceivedQueueCount(),
                                    /* Count of Messages in Queue */
            strMsg = 'Received: ';  /* Returned Message List */
            
		if (nMsgCount > 0) {
		      
		    strMsg += this.DebugWrite('R-', nMsgCount, this.DebugReceived);
		}
		else {
		      
			strMsg += 'None';
		}
		  
		nMsgCount = this.DebugReceivedStoredQueueCount();
		strMsg += ' ----- Received - Stored: ';
		 
		if (nMsgCount > 0) {
		    
		    strMsg += this.DebugWrite('RS-', nMsgCount, this.DebugReceivedStored);
		}
		else {
		     
			strMsg += 'None';
		}
		  
		nMsgCount = this.DebugToSendQueueCount();
		strMsg += ' ----- Sending: ';
		  
		if (nMsgCount > 0) {
		    
		    strMsg += this.DebugWrite('SD-', nMsgCount, this.DebugToSend);
		}
		else {
		      
			strMsg += 'None';
		}
		 
		nMsgCount = this.DebugToSendStoredQueueCount();
		strMsg += ' ----- Sending - Stored: ';
		 
		if (nMsgCount > 0) {
		     
		    strMsg += this.DebugWrite('SS-', nMsgCount, this.DebugToSendStored);
		}
		else {
		     
			strMsg += 'None';
		}
		
		if (console) {
		
			console.log(strMsg);
		}
		
		return strMsg
    },
	DebugWrite: function(strMsgType, nMsgCount, fnGetMsg) {
          
    	var strMsg = '',			/* Returned Message List */
            nCounter = 0;           /* Counter for Loop */
		     
		for (nCounter = 0; nCounter < nMsgCount; nCounter++) {
	          
	    	if (nCounter > 0) {
	          
	        	strMsg += ' | ';
	        }
	           
	        strMsg += strMsgType + (nCounter + 1) + ' - ' + fnGetMsg.apply(this, [nCounter]);
	    }
        
        return strMsg;
	},
	DebugReceived: function(nMsgIndex) {

		return this.DebugMaskPeerToPeerIP(this.GetMsg(this.amsiListReceived, nMsgIndex), this.GetMsg(this.amsiListReceived, nMsgIndex).length);
	},
	DebugToSend: function(nMsgIndex) {
		
		return this.DebugMaskPeerToPeerIP(this.GetMsg(this.amsiListToSend, nMsgIndex), this.GetMsg(this.amsiListToSend, nMsgIndex).length);
	},
	DebugReceivedStored: function(nMsgIndex) {

		return this.DebugMaskPeerToPeerIP(this.GetMsg(this.amsiListStoredReceived, nMsgIndex), this.GetMsg(this.amsiListStoredReceived, nMsgIndex).length);
	},
	DebugToSendStored: function(nMsgIndex) {
		
		return this.DebugMaskPeerToPeerIP(this.GetMsg(this.amsiListStoredToSend, nMsgIndex), this.GetMsg(this.amsiListStoredToSend, nMsgIndex).length);
	},
	DebugBackupSeqs: function() {

		var amsgListBackupCheck = this.amsgListBackupSent,
			nListCount = amsgListBackupCheck.length,
			strDebugMsg = "",		/* Debug Message */
			nCounter = 0;

		for (nCounter = 0; nCounter < nListCount; nCounter++) {

			if (strDebugMsg != "") {

				strDebugMsg += ", ";
			}
				
			strDebugMsg += amsgListBackupCheck[nCounter].GetMetaDataSeqNum();
		}

		if (strDebugMsg == "") {

			strDebugMsg = "No messages in backup.";
		}

		return strDebugMsg;
	},
	DebugToSendQueueCount: function() {

		return this.amsiListToSend.length;
	},
	DebugReceivedQueueCount: function() {

		return this.amsiListReceived.length;
	},
	DebugToSendStoredQueueCount: function() {

		return this.amsiListStoredToSend.length;
	},
	DebugReceivedStoredQueueCount: function() {

		return this.amsiListStoredReceived.length;
	},
	DebugSendMsgLength: function(nMsgIndex) {
	
		return this.DebugToSend(nMsgIndex).length;
	},
	DebugReceivedMsgLength: function(nMsgIndex) {
	
		return this.DebugReceived(nMsgIndex).length;
	},
	DebugActivate: function(boolOn) {
		
		this.boolDebug = boolOn;
	},
	SetMsgPartIndicator: function(strSetMsgPartIndicate) {

		if (this.strMsgStartIndicate != strSetMsgPartIndicate &&
			this.strMsgEndIndicate != strSetMsgPartIndicate &&
			this.nMsgIndicatorLen == strSetMsgPartIndicate.length()) {
		
			this.strMsgPartIndicate = strSetMsgPartIndicate;
		}
		else {
		
			this.AddLogErrorMsg("During setting message part indicators, indicator was the same as another or an invalid length. Setting failed.");
		}
	},
	SetMsgStartIndicator: function(strSetMsgStartIndicate) {

		if (this.strMsgPartIndicate != strSetMsgStartIndicate &&
			this.strMsgEndIndicate != strSetMsgStartIndicate &&
			this.nMsgIndicatorLen == strSetMsgStartIndicate.length()) {

			this.strMsgStartIndicate = strSetMsgStartIndicate;
		}
		else {
		
			this.AddLogErrorMsg("During setting message start indicators, indicator was the same as another or an invalid length. Setting failed.");
		}
	},
	SetMsgEndIndicator: function(strSetMsgEndIndicate) {
		
		if (this.strMsgStartIndicate != strSetMsgEndIndicate &&
			this.strMsgPartIndicate != strSetMsgEndIndicate &&
			this.nMsgIndicatorLen == strSetMsgEndIndicate.length()) {

			this.strMsgEndIndicate = strSetMsgEndIndicate;
		}
		else {
		
			this.AddLogErrorMsg("During setting message end indicators, indicator was the same as another or an invalid length. Setting failed.");
		}
	},
	SetMsgIndicatorLen: function(nSetMsgIndicatorLen) {

		if (nSetMsgIndicatorLen > 0) {
		
			this.nMsgIndicatorLen = nSetMsgIndicatorLen;
		}
		else {
		
			this.AddLogErrorMsg("During setting message indicator length, indicator length can not be set to zero. Setting failed.");
		}
	},
	SetMsgFiller: function(strSetMsgFiller) {

		if (strSetMsgFiller.length == 1) {

			this.strMsgFiller = strSetMsgFiller;
		}
		else {
		
			this.AddLogErrorMsg("During setting message filler character, filler character length can not be greater than one. Setting failed.");
		}
	},
	GetMsgStartIndicator: function() {
	
		return this.strMsgStartIndicate;
	},
	GetMsgPartIndicator: function() {
	
		return this.strMsgPartIndicate;
	},
	GetMsgEndIndicator: function() {
	
		return this.strMsgEndIndicate;
	},
	GetMsgStartIndicatorLength: function() {
	
		return this.strMsgStartIndicate.length;
	},
	GetMsgPartIndicatorLength: function() {
	
		return this.strMsgPartIndicate.length;
	},
	GetMsgEndIndicatorLength: function() {
	
		return this.strMsgEndIndicate.length;
	},
	GetMsgFiller: function() {

		return this.strMsgFiller;
	},
	IsConnected: function() {
	
		return this.wsServerConnect != null;
	},
	HasPeerToPeerServer: function() {
	
		return this.wsPeertoPeerConnect != null;
	},
	HasPeerToPeerClients: function() {
	
		return this.appciListClients.length > 0;
	}
},
MESSAGE = {
	strMsg: '',
	boolHasEnd: false,
	boolHasStart: false,
	nStartIndex: -1,
	nEndIndex: -1,
	nLength: 0,
	CreateMsg: function(strSetMsg) {

		var objNewMsgInfo = {...MESSAGE};
		objNewMsgInfo.SetMsg(strSetMsg);
		return objNewMsgInfo;
	},
	SetMsg: function(strSetMsg) {
		
		var nMsgLen = strSetMsg.length,
									/* Length of Message */
			nMsgStartIndex,
			nMsgEndIndex;			/* Message's Start and Ending Indexes */
		
		this.strMsg = strSetMsg;
		nMsgStartIndex = this.FindStringStartIndex(strSetMsg, nMsgLen);
		nMsgEndIndex = this.FindStringEndIndex(strSetMsg, nMsgLen);

		if (nMsgEndIndex >= 0 && nMsgStartIndex >= 0 && nMsgStartIndex < nMsgEndIndex) {
			
			nMsgEndIndex += (CLIENT.GetMsgEndIndicatorLength() - 1);

			this.nLength = (nMsgEndIndex - nMsgStartIndex) + 1;

			this.boolHasStart = true;
			this.boolHasEnd = true;
		}
		else if (nMsgEndIndex >= 0) {
		
			nMsgEndIndex += (CLIENT.GetMsgEndIndicatorLength() - 1);
			nMsgStartIndex = 0;
			this.nLength = nMsgEndIndex + 1;
			this.boolHasEnd = true;
		}
		else if (nMsgStartIndex >= 0) {
		
			nMsgEndIndex = nMsgLen - 1;

			if (nMsgStartIndex > 0) {

				this.nLength = nMsgLen - (nMsgStartIndex + 1);
			}
			else {
			
				this.nLength = nMsgLen;
			}

			this.boolHasStart = true;
		}
		else {
		
			nMsgEndIndex = nMsgLen - 1;
			nMsgStartIndex = 0;
			this.nLength = nMsgLen;
		}
		
		this.nStartIndex = nMsgStartIndex;
		this.nEndIndex = nMsgEndIndex;
	},
	GetMetaDataSeqNum: function(strCheck, nCheckLen) {
		
		var strMsgType,				/* Message Type Name */
			nMsgTypeLen = 0,
			nNumIndex = 0,
			nNumLen = 0;
			llSeqNum = 0;			/* Length of Message Type and Possible Metadata and 
									   Index of the Message Sequence Number and Length and Sequence Number */

		try {
			
			if (typeof(strCheck) != 'string' || strCheck == '') {
				
				strCheck = this.strMsg;
			}
			
			if (!Number.isInteger(nCheckLen)) {
				
				nCheckLen = strCheck.length;
			}
			
			/* If Metadata Exists */
			if ((strMsgType = strCheck.substr(0, nCheckLen)) != '' && 
				(nMsgTypeLen = strMsgType.length) > 0 && 
				(nNumIndex = strMsgType.indexOf('-') + 1) > 0) {

				/* Check If There is More Metadata, If Not, Get the Rest of the String as the Sequence Number */ 
				if (nMsgTypeLen - (nNumIndex + 1) > 0 && 
					(nNumLen = strMsgType.substr(nNumIndex, nMsgTypeLen - (nNumIndex + 1)).indexOf('-')) <= 0) {
			
					nNumLen = nMsgTypeLen - nNumIndex;
				}

				llSeqNum = parseInt(strMsgType.substr(nNumIndex, nNumLen));
			}
		}
		catch (exError) {
				
			throw exError;
		}

		return llSeqNum;
	},
	GetMetaDataTime: function(strCheck, nCheckLen) {
	
		var strMsgType = '',/* Message Type Name */
			nMsgTypeLen = 0,
			nNumIndex = 0,
			nTimeIndex = 0,
			nTimeLen = 0;	/* Length of Message Type and Possible Metadata and 
			   				   Index of the Message Sequence Number, Time,  and Time Length */
			llTimeInMillis = 0;
							/* Time Sent in Milliseconds */
		
		try {
	
			if (typeof(strCheck) != 'string' || strCheck == '') {
				
				strCheck = this.strMsg;
			}
			
			if (!Number.isInteger(nCheckLen)) {
				
				nCheckLen = strCheck.length;
			}		
			
			/* If Metadata Exists, Get to Time Index */
			if ((strMsgType = strCheck.substr(0, nCheckLen)) != '' && 
				(nMsgTypeLen = strMsgType.length) > 0 && 
				(nNumIndex = strMsgType.indexOf('-') + 1) > 0 &&
				nMsgTypeLen - (nNumIndex + 1) > 0 && 
				(nTimeIndex = strMsgType.substr(nNumIndex, nMsgTypeLen - (nNumIndex + 1)).indexOf("-")) > 0) {
			
				/* Check If There is More Metadata, If Not, Get the Rest of the String as the Time */ 
				if (nMsgTypeLen - ((nNumIndex + 1) + (nTimeIndex + 1)) > 0 && 
				    (nTimeLen = strMsgType.substr(nTimeIndex + (nNumIndex + 1), nMsgTypeLen - ((nNumIndex + 1) + (nTimeIndex + 1))).indexOf("-")) <= 0) {
				
					nTimeLen = nMsgTypeLen - nTimeIndex;
				}
				
				llTimeInMillis = parseInt(strMsgType.substr(nTimeIndex + (nNumIndex + 1), nTimeLen));
			}	
		}
		catch (exError) {
	
			throw exError;
		}
	
		return llTimeInMillis;
	},
	FindMetaDataInString: function(strCheck, nCheckLen) {

		var strMsgType = '',	/* Message Type Name */
			nMsgTypeLen = 0,
			nNumIndex = -1;		/* Length of Message Type and Possible Metadata and 
								   Index of the Message Sequence Number */
		try {
			
			if (typeof(strCheck) != 'string' || strCheck == '') {
				
				strCheck = this.strMsg;
			}
			
			if (!Number.isInteger(nCheckLen)) {
				
				nCheckLen = strCheck.length;
			}		
			
			/* If Metadata Exists, Get to Time Index */
			if ((strMsgType = strCheck.substr(0, nCheckLen)) != '' && 
				(nMsgTypeLen = strMsgType.length) > 0 && 
				(strMsgType.indexOf('-') + 1) > 0) {
				
				nNumIndex = strCheck.substr(0, nCheckLen).indexOf('-');
			}	
		}
		catch (exError) {
	
			throw exError;
		}
		
		return nNumIndex;
	},
	FindInMsg: function(strSearchString, nSearchLen) {
		
		var strSearchMsg = this.strMsg,	/* Message to be Searched */
			nMsgLen = this.nLength,		/* Message Length */
			nMsgStartIndex = -1,		/* Starting Index of Found String */
			nMetadataStartIndex = -1,
			nCleanMsgLen = nMsgLen,		/* Holder of Clean Message Without Metadata */
			nRestIndex = -1,			/* Index of the Rest of the Message After Metadata */
			strCleanMsg;				/* Holder for Clean Message Without Metadata */		

		try {

			/* If No Metadata, Get Whole Message */
			if (nMetadataStartIndex = this.FindMetaDataInString(strSearchMsg, nMsgLen) < 0) {

				strCleanMsg = strSearchMsg.substring(nMsgLen);
			}
			else {
				
				/* Else Remove Metadata from Clean Message for Search */
				nCleanMsgLen = nMetadataStartIndex;
				strCleanMsg = strSearchMsg.substring(nCleanMsgLen);

				if ((nRestIndex = strSearchMsg.indexOf(CLIENT.GetMsgEndIndicator())) < 0) {
				
					if ((nRestIndex = this.FindStringEndIndex(strSearchMsg)) < 0) {
					
						nRestIndex = (nMsgLen - 1) - nMetadataStartIndex;
					}
				} 

				nCleanMsgLen += nMsgLen - (nRestIndex + 1);
				strCleanMsg += strSearchMsg.substr(nRestIndex, nMsgLen - (nRestIndex + 1));
			}

			if (nCleanMsgLen >= nSearchLen) { 

				nMsgStartIndex = strCleanMsg.substr(0, nCleanMsgLen).indexOf(strSearchString);
			}
		}
		catch (exError) { 
		
			throw exError;
		}
				
		return nMsgStartIndex;
	},
	FindSegmentInString: function(strCheck, nSegNum, nCheckLen) {

		var MSGPARTINDICATOR = CLIENT.GetMsgPartIndicator(),
									/* Message Part Indicator */
			MSGPARTINDICATORLEN = CLIENT.GetMsgPartIndicatorLength(),
									/* Length of Message Part Indicator */
		    nPartStartIndex = 0,	/* Index of the Start of the Selected Part */
			nPartLen = 0,			/* Length of the Selected Part */
			nCoveredIndex = 0,		/* Last Index of Covered Message */
			nCounter = 0,			/* Counter for Loop */
			strSegment = '';		/* Found Message Segment */
				
		try {
				
			if (Number.isInteger(nCheckLen) && strCheck.length > nCheckLen) {
				
				strCheck = strCheck.substr(0, nCheckLen);
			}
			else if (!Number.isInteger(nCheckLen) || strCheck.length != nCheckLen) {
				
				nCheckLen = strCheck.length;
			}		
				
			nPartStartIndex = strCheck.indexOf(MSGPARTINDICATOR);
			nPartLen = this.FindSegmentLengthInString(strCheck, nSegNum, nCheckLen);

			if (nPartLen > 0) {

				if (nPartStartIndex >= 0) {
		
					nCoveredIndex = nPartStartIndex + MSGPARTINDICATORLEN;

					/* If Any Segment Outside of Start, Else If Starting Message Type Segment, Else Return Nothing */
					if (nSegNum > 0) {

						for (nCounter = 1; nCounter < nSegNum && nPartStartIndex >= 0; nCounter++) {
		
							nPartStartIndex = strCheck.substr(nCoveredIndex, nCheckLen - (nCoveredIndex + 1)).indexOf(MSGPARTINDICATOR);

							if (nPartStartIndex >= 0) {
						
								nCoveredIndex += nPartStartIndex + MSGPARTINDICATORLEN;
							}
						}

						strSegment = strCheck.substr(nCoveredIndex, nPartLen);
					}
				}

				if (nSegNum == 0) {

					nPartStartIndex = this.FindStringStartIndex(strCheck.substr(0, nCheckLen), nCheckLen);

					if (nPartStartIndex >= 0) {
				
						nCoveredIndex = nPartStartIndex + CLIENT.GetMsgStartIndicatorLength();
					}

					strSegment = strCheck.substr(nCoveredIndex, nPartLen);
				}
			}
		}
		catch (exError) { 
		
			throw exError;
		}

		return strSegment;
	},
	FindSegmentLengthInString: function(strCheck, nSegNum, nCheckLen) {

		var MSGPARTINDICATOR = CLIENT.GetMsgPartIndicator(),
										/* Message Part Indicator */
			MSGPARTINDICATORLEN = CLIENT.GetMsgPartIndicatorLength(),
										/* Length of Message Part Indicator */
		    nSegmentLen = 0,			/* Length of Message Segment */
			nNextStartIndex = 0,		/* Index of the Start of the Next Part or End */
			nRemainingLen = nCheckLen,	/* Remaining of the Message Length */
			nCoveredIndex = 0,			/* Last Index of Covered Message */
			nCounter = 0;				/* Counter for Loop */

		try {
			
			if (Number.isInteger(nCheckLen) && strCheck.length > nCheckLen) {
				
				strCheck = strCheck.substr(0, nCheckLen);
			}
			else if (!Number.isInteger(nCheckLen) || strCheck.length != nCheckLen) {
				
				nRemainingLen = nCheckLen = strCheck.length;
			}	

			if (nSegNum > 0) {

				for (nCounter = 0; nCounter < nSegNum && nNextStartIndex >= 0; nCounter++) {

					nNextStartIndex = strCheck.substr(nCoveredIndex, nRemainingLen).indexOf(MSGPARTINDICATOR);

					if (nNextStartIndex >= 0) {

						nCoveredIndex += nNextStartIndex + MSGPARTINDICATORLEN;
						nRemainingLen = nCheckLen - nCoveredIndex;
					}
				}

				if (nNextStartIndex >= 0) {

					nNextStartIndex = strCheck.substr(nCoveredIndex, nRemainingLen).indexOf(MSGPARTINDICATOR);

					if (nNextStartIndex >= 0) {
				
						nSegmentLen = nNextStartIndex;
					}
				}
				
				if (nNextStartIndex < 0)  {
 
					nNextStartIndex = this.FindStringEndIndex(strCheck.substr(nCoveredIndex, nRemainingLen), nRemainingLen);
					
					if (nNextStartIndex >= 0) {
				
						nSegmentLen = nNextStartIndex;
					}
					else {
				
						nSegmentLen = nCheckLen - nCoveredIndex;
					}
				}
			}
			else if (nSegNum == 0) {

				nNextStartIndex = this.FindStringStartIndex(strCheck.substr(0, nCheckLen), nCheckLen);

				if (nNextStartIndex >= 0) {
			
					nCoveredIndex = nNextStartIndex + MSGPARTINDICATORLEN;
					nNextStartIndex = strCheck.substr(nCoveredIndex, nCheckLen - nCoveredIndex).indexOf(MSGPARTINDICATOR);
				
					if (nNextStartIndex >= 0) {

						nSegmentLen = nNextStartIndex;
					}
					else {

						nNextStartIndex = this.FindStringEndIndex(strCheck.substr(nCoveredIndex, nCheckLen - nCoveredIndex), nCheckLen - nCoveredIndex);
						
						if (nNextStartIndex >= 0) {

							nSegmentLen = nNextStartIndex;
						}
						else {
						
							nSegmentLen = nCheckLen - nCoveredIndex;
						}
					}
				}
			}
		}
		catch (exError) { 
		
			throw exError;
		}

		return nSegmentLen;
	},
	GetSegment: function(nSegNum) {

		var strSegment = '';		/* Found Message Segment */

		try {

			strSegment = this.FindSegmentInString(this.strMsg.substring(this.nStartIndex), nSegNum, this.nLength);
		}
		catch (exError) { 
		
			throw exError;
		}

		return strSegment;
	},
	GetSegmentLength: function(nSegNum) {
		
		var nSegmentLen = 0;		/* Length of Message Segment */

		try {

			nSegmentLen = this.FindSegmentLengthInString(this.strMsg.substring(this.nStartIndex), nSegNum, this.nLength);
		}
		catch (exError) { 
		
			throw exError;
		}

		return nSegmentLen;
	},
	FindStringStartIndex: function(strCheck, nCheckLen) {
		
		if (typeof(strCheck) != 'string' || strCheck == '') {
			
			strCheck = this.strMsg;
		}
		
		if (!Number.isInteger(nCheckLen)) {
			
			nCheckLen = strCheck.length;
		}
		
		return strCheck.substr(0, nCheckLen).indexOf(CLIENT.GetMsgStartIndicator());
	},
	FindStringEndIndex: function(strCheck, nCheckLen) {
		
		if (typeof(strCheck) != 'string' || strCheck == '') {
			
			strCheck = this.strMsg;
		}
		
		if (!Number.isInteger(nCheckLen)) {
			
			nCheckLen = strCheck.length;
		}
		
		return strCheck.substr(0, nCheckLen).indexOf(CLIENT.GetMsgEndIndicator());
	},
	GetMessage: function() {
		
		return this.strMsg;
	},
	Length: function() {
		
		return this.nLength;
	},
	HasStart: function() {

		return this.boolHasStart;
	},
	HasEnd: function() {

		return this.boolHasEnd;
	},
	IsComplete: function() {

		return this.boolHasStart && this.boolHasEnd;
	}
},
PeertoPeerClient = {
	socClient: null,
	strHomeIPAddress: '',
	strPeerIPAddress: '',
	strEncryptKey: '',
	strEncryptIV: '',
	strDecryptKey: '',
	strDecryptIV: '',
//	strLeftOverMsgPart: '',
//	nLeftOverMsgLen: 0,
	boolIsNegotiating: false,
	boolIsANegotiation: true,
	boolHasEncryptInfo: true,
	boolConnected: false,
	amsiListReceived: [],			/* List of Information on Messages from Server */
	amsgListBackupSent: [],
	llSendMsgs: 0,			
	llReceivedMsgs: 0,
	nTimeToLateInMillis: 30,
	boolRemoveLateMsgs: false,
	nTimeToCheckActInMillis: 30,
	llLastActOrCheckInMillis: new Date().getTime(),
	nBackupQueueMsgLimit: 2000,
	appcListNegotiate: [],
	constructor(socSetClient, 
				strSetHomeIPAddress,
				strSetPeerIPAddress,  
				strSetEncryptKey, 
				strSetEncryptIV, 
				strSetDecryptKey, 
				strSetDecryptIV) {

		if (socSetClient && typeof(socSetClient) == 'WebSocket') {
			
			socSetClient.onmessage = this.Receive;
			socSetClient.onclose = this.CloseClient;
			socSetClient.onerror = this.ErrorClient;
			socSetClient.onmessageerror = this.ErrorClient;
			
			this.socClient = socSetClient;
			
			if (typeof(strSetHomeIPAddress) != 'string' || strSetHomeIPAddress == '') {

				strSetHomeIPAddress = socSetClient.address().address;
			}
			
			if (typeof(strSetPeerIPAddress) != 'string' || strSetPeerIPAddress == '') {
				
				strSetPeerIPAddress = socSetClient.remoteAddress;
			}
			
			if (typeof(strSetPeerIPAddress) == 'string' && strSetPeerIPAddress != '' &&
				typeof(strSetHomeIPAddress) == 'string' && strSetHomeIPAddress != '') {
				
				this.strHomeIPAddress = strSetHomeIPAddress;
				this.strPeerIPAddress = strSetPeerIPAddress;
						
				if (typeof(strSetEncryptKey) == 'string' && strSetEncryptKey != '' &&
					typeof(strSetEncryptIV) == 'string' && strSetEncryptIV != '' && 
					typeof(strSetDecryptKey) == 'string' && strSetDecryptKey != '' && 
					typeof(strSetDecryptIV) == 'string' && strSetDecryptIV != '') {
					
					this.strEncryptKey = strSetEncryptKey;
					this.strEncryptIV = strSetEncryptIV;
					this.strDecryptKey = strSetDecryptKey;
					this.strDecryptIV = strSetDecryptIV;
					this.boolHasEncryptInfo = true;
				}
				else {
	
					this.boolHasEncryptInfo = false;
				}
				
				this.boolConnected = true;
	
				this.Sync(false);
			}
			else if (typeof(strSetPeerIPAddress) != 'string' || strSetPeerIPAddress == '') {
		
				throw("During setting up client 'Peer To Peer' information, getting Peer IP address failed.");
			}
			else {
				
				throw("During setting up client 'Peer To Peer' information, getting Home IP address failed.");
			}
		}
		else {
			
			throw("During setting up client 'Peer To Peer' information, invalid web socket was sent.");
		}
	},
	Send: async function(strMsg, boolTrack) {

		try {
			
			if (this.boolConnected) {

				if (this.socClient) {

					if (boolTrack) {
					
						strMsg = this.AddTracking(strMsg);
					}

					strMsg = await this.EncryptMsg(strMsg);

					this.socClient.send(strMsg);
					this.MoveSentMsgToBackup(strMsg);
				}
				else {

					CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message sender, connecting to client failed.");
				}
			}
		}
		catch (exError) {

			if (!this.boolIsNegotiating && !this.boolIsANegotiation) {

				CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message sender, an exception occurred. Exception: " + exError.message);
			}

			this.CloseClient();
		}
	},
	Receive: async function(evReceived) {

		var nReceivedAmount = 0,		/* Amount Received from Server */
			strNewMsg = "",				/* New Message */
			strMsgType = "";			/* Message Type */
//			llSeqNum = 0,				/* Message Sequence Number */
//			llTimeInMillis = 0;			/* Time in Milliseconds */
//			socSender = this.socClient, /* Web Socket Holder for Sending to Other Connections */
//			amsgListBackupCheck = this.amsgListBackupSent;
										/* List of Back Message Information Records */
//			pmsgSelect = null;			/* Selected Message Information Record */
//			llTimeInMillis = parseInt(MESSAGE.FindSegmentInString(strNewMsg, 1));
//			boolResyncing = (MESSAGE.FindSegmentInString(strNewMsg, 2) == "1"),
//			boolPreviousSynced = (parseInt(MESSAGE.FindSegmentInString(strNewMsg, 3)) == llStartTimeInMillis);
										/* Indicator That Negotiator is Resyncing and If They were Previous In Sync */
//			appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
//			ppcSelected,				/* Selected Negotating "Peer To Peer" Client Information */
//			ppcDisconnected,			/* Disconnected Selected Negotating "Peer To Peer" Client Information */
//			strPeertoPeerMsg = '',		/* Peer-to-Peer Receive Message
//			nListCount = 0,				/* List Array Count */
//			nCounter = 0;				/* Counter for Loop */

		try {

			if (this.boolConnected) {

				if (this.socClient) {

					strNewMsg = evReceived.data;
					nReceivedAmount = strNewMsg.length;

					if (nReceivedAmount > 0) {

						strNewMsg = new TextDecoder().decode(await this.DecryptMsg(strNewMsg));
						nReceivedAmount = strNewMsg.length;
						
						if (nReceivedAmount > 0 && strNewMsg != "") {

							strMsgType = MESSAGE.FindSegmentInString(strNewMsg, 0);

							if (strMsgType != "NEGOTIATESYNC" && strMsgType != "MSGREPLAY" && strMsgType != "MSGCHECK") {

								var llSeqNum = MESSAGE.GetMetaDataSeqNum(strNewMsg, nReceivedAmount);

								if (llSeqNum == 0 || llReceivedMsgs + 1 == llSeqNum) {
								
									var llTimeInMillis = MESSAGE.GetMetaDataTime(strNewMsg, nReceivedAmount);

									if (llTimeInMillis == 0 || !(new Date().getTime() - llTimeInMillis >= this.nTimeToLateInMillis && this.boolRemoveLateMsgs)) {
									
										this.amsiListReceived.push(strNewMsg);
									}

									this.llReceivedMsgs++;
								}
								else {

									this.SendReplay(true);
								}
							}
							else if (strMsgType == "MSGREPLAY" || strMsgType == "MSGCHECK") {
							
								var llSeqNum = parseInt(MESSAGE.FindSegmentInString(strNewMsg, 1)),
									socSender = this.socClient,
									amsgListBackupCheck = this.amsgListBackupSent,
									ppcSelected = null,
									nListCount = amsgListBackupCheck.length,				
									nCounter = 0;

								for (nCounter = 0; nCounter < nListCount; nCounter++) {
								
									ppcSelected = amsgListBackupCheck[nCounter];
									
									if (llSeqNum <= ppcSelected.GetMetaDataSeqNum()) {

										try {

											ppcSelected.send(pmsgSelect.GetMessage());
											nCounter = nBackupCount;
										}
										catch (exError) {

											if (!this.boolIsNegotiating && !this.boolIsANegotiation) {

												// Error
												// "During running client 'Peer To Peer' message receiver, sending replayed message, #" + pmsgSelect -> GetMetaDataSeqNum() + ", failed for message type, '" + strMsgType + "'.");
											}
										}
									}
								}
								
								strNewMsg = '';
								nReceivedAmount = 0;
							}
							else {

								var llTimeInMillis = parseInt(MESSAGE.FindSegmentInString(strNewMsg, 1)),
									boolResyncing = (MESSAGE.FindSegmentInString(strNewMsg, 2) == "1"),
									boolPreviousSynced = (parseInt(MESSAGE.FindSegmentInString(strNewMsg, 3)) == llStartTimeInMillis);
									
								if (llTimeInMillis < llStartTimeInMillis || (boolResyncing && boolPreviousSynced)) {
									
									llStartTimeInMillis = llTimeInMillis;
								}
								else if (llTimeInMillis > llStartTimeInMillis && boolResyncing && !boolPreviousSynced) {
							
									this.Sync(false);
									this.SendNegotiation();
								}

								strNewMsg = '';
								nReceivedAmount = 0;
							}

							this.llLastActOrCheckInMillis = new Date().getTime();
						}
					}									
					else if (nReceivedAmount <= 0) {

						if (nReceivedAmount != 0) {
					
							if (!this.boolIsNegotiating && !this.boolIsANegotiation) {

								CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, receiving message from client failed");
							}

							this.CloseClient();
						}

//						if (nLeftOverMsgLen > 0 && boolLeftOverNotRan) {
//							
//							nReceivedAmount = nLeftOverMsgLen;
//							memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
//							memcpy(&acharMsgReceived[0], pcharLeftOverMsgPart, nReceivedAmount);
//							
//							ClearLeftOverEncryptMsg();
//
//							nReceivedAmount = DecryptMsg(&acharMsgReceived[0], 
//														 nReceivedAmount);
//							
//							if (nReceivedAmount > 0) {
//
//								memcpy(pcharReturn, &acharMsgReceived[0], nReceivedAmount);
//							}
//							else {
//
//								boolLeftOverNotRan = false;
//							}
//						}

						/* Processing Connections Being Negotiated */
						var appcNegotiators = this.appcListNegotiate,
							ppcSelected = null,
							ppcDisconnected = null,
							nListCount = appcNegotiators.length,				
							nCounter = 0;

						for (nCounter = 0; nCounter < nListCount; nCounter++) {
					
							ppcSelected = appcNegotiators[nCounter];
			
							if (ppciSelected.Connected()) {
		
								strPeertoPeerMsg = ppciSelected.Receive({data: strNewMsg});
								nReceivedAmount = strPeertoPeerMsg.length;

								if (nReceivedAmount > 0) {
								
									strNewMsg = strPeertoPeerMsg;
								}
								else if (nReceivedAmount != 0) {
				
									ppcDisconnected = ppcSelected;
								}
							}
							else {
						
								ppcDisconnected = ppcSelected;
							}

							if (ppcDisconnected) {

								appcNegotiators.splice(nCounter, 1);
								nCounter--;
								nListCount--;
								ppcDisconnected.CloseClient();
								ppciDisconnected = null;
							}
						}
						
						/* If Wait for Message Activity Has Gone Passed Limit, Send Message Check */
						if (new Date().getTime() - this.llLastActOrCheckInMillis >= this.nTimeToCheckActInMillis) {

							this.SendReplay(false);

							this.llLastActOrCheckInMillis = new Date().getTime();
						}
					}
				}
				else {
				
					CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, connecting to client failed.");
				}
			}
		}
		catch (exError) {

			CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, an exception occurred. Exception: " + exError.message);
		}

		return nReceivedAmount;
	},
	DequeueMsg: function() {
		
		return this.amsiListReceived.shift();
	},
	AddTracking: function(strMsg) {

		var strTrackingData = '',	/* Tracking Data to Add */
			strNewMsg = '',			/* Message with Added Tracking */
			nNewMsgLen = 0,
			nMsgTypeLen = 0,
			nStartIndex = 0;			/* Message Length, Length of Message with Adding Tracking and it Length and
										   Message Type Length and Start Index of Message */

		try {

			nMsgTypeLen = MESSAGE.FindSegmentLengthInString(strMsg, 0, strMsg.length);
			nStartIndex = MESSAGE.FindStringStartIndex(strMsg, strMsg.length);

			if (nMsgTypeLen > 0 && nStartIndex >= 0) {

				if (nStartIndex > 0) {

					nNewMsgLen = (nStartIndex + 1) + CLIENT.GetMsgStartIndicatorLength() + nMsgTypeLen;
				}
				else {
					
					nNewMsgLen = CLIENT.GetMsgStartIndicatorLength() + nMsgTypeLen;
				}
					
				strNewMsg = strMsg.substr(0, nNewMsgLen) + '-' + (new Date().getTime()) + '-' + (++this.llSendMsgs) +
				 			strMsg.substring(nNewMsgLen);
			}
		}
		catch (exError) {
		
			CLIENT.AddLogErrorMsg("During client 'Peer to Peer' adding of tracking to message, an exception occurred. Exception: " + exError.message);	
		}

		return strNewMsg;
	},
	MoveSentMsgToBackup: function(strMsg, nMsgLen) {

		var	pmsgSelect = null,		/* Selected Message Information Record */
			llSeqNum = 0,			/* Message Sequence Number */
			nQueueCount = this.amsgListBackupCheck.length, 		
									/* Count of Messages in Backup Queue */
			nListCount = 0,			/* List Array Count */
			nCounter = 0;			/* Counter for Loop */
				
		try {
						
			if ((llSeqNum = MESSAGE.GetMetaDataSeqNum(strMsg, nMsgLen) > 0)) {

				if (nQueueCount > 0) {

					pmsgSelect = this.amsgListBackupSent[nQueueCount - 1];
					
					if (pmsgSelect.GetMetaDataSeqNum() + 1 == llSeqNum) {
					
						this.amsgListBackupSent.push(MESSAGE.CreateMsg(strMsg));
						nQueueCount++;
					}
					else if (llSeqNum > pmsiSelect.GetMetaDataSeqNum()) {

						/* Else Found Old Messages from Backup Queue, Clear Backup Queue, and Replace with Holder Queue */
						CLIENT.AddLogErrorMsg("During storing peer-to-peer backup messages, messages missing from backup queue. Dropping current messages from backup queue.");
						this.amsgListBackupSent = [MESSAGE.CreateMsg(strMsg)];
						nQueueCount = 1;
					}
					else {
							
						/* Else Found Old Messages Holder Queue, Not Adding to Backup Queue */
						CLIENT.AddLogErrorMsg("During storing peer-to-peer backup messages, messages being put into backup queue are old. Backup failed.");
					}
				}
				else {
				
					this.amsgListBackupSent = [MESSAGE.CreateMsg(strMsg)];
					nQueueCount = 1;
				}
			}

			/* Remove Oldest Messages from the Backup Queue When Over Limit */
			if (nQueueCount > this.nBackupQueueMsgLimit) {

				this.amsgListBackupSent.splice(0, this.nBackupQueueMsgLimit);
			}
		}
		catch (exError) {

			CLIENT.AddLogErrorMsg("During storing backup messages, an exception occurred. Exception: " + exError.message);
		}
	},
	SendReplay(boolIsNotCheck) {

		var strMsg = "";			/* Replay Message */
		
		if (this.socClient) {

			if (boolIsNotCheck) {
		
				strMsg = CLIENT.GetMsgStartIndicator() + "MSGREPLAY" + CLIENT.GetMsgPartIndicator() + (llReceivedMsgs + 1) + CLIENT.GetMsgEndIndicator();
			}
			else {
		
				strMsg = CLIENT.GetMsgStartIndicator() + "MSGCHECK" + CLIENT.GetMsgPartIndicator() + (llReceivedMsgs + 1) + CLIENT.GetMsgEndIndicator();
			}

			try {
			
				this.socClient.send(strMsg);
			}
			catch (exError) {

				if (!this.boolIsNegotiating && !this.boolIsANegotiation) {
					
					if (boolIsNotCheck) {

						CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, sending replay message failed.");
					}
					else {

						CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, sending check message failed.");
					}
				}

				this.CloseClient();
			}
		}
	},
	EncryptMsg: function(strMsg) {
		
		try {
			
			if (this.boolHasEncryptInfo && strMsg.length) {
				
				strMsg = window.crypto.subtle.encrypt(
							  {
								  name: "AES-GCM",
								  iv: this.strEncryptIV
							  },
							  this.strEncryptKey,
							  new TextEncoder().encode(strMsg)
						 );
			}
			else {
				
				strMsg = '';
			}
		}
		catch (exError) {

			CLIENT.AddLogErrorMsg("During client 'Peer to Peer' encryption operation, an exception occurred. Exception: " + exError.message);
		}

		return strMsg;
	},
	DecryptMsg: function(strMsg) {
		
		try {
			
			if (this.boolHasEncryptInfo && strMsg.length) {
				
				strMsg = window.crypto.subtle.decrypt(
							  {
								  name: "AES-GCM",
								  iv: this.strDecryptIV
							  },
							  this.strDecryptKey,
							  strMsg
						 );
			}
			else {
				
				strMsg = '';
			}
		}
		catch (exError) {

			CLIENT.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, an exception occurred. Exception: " + exError.message);
		}

		return strMsg;
	},
	StartNegotiation: function(ppciNegotiateInfo) {

//		var boolNegotiateEncypted = ppciNegotiateInfo.CanEncrypt();
										/* Indicator That Negotating "Peer To Peer" Client Information is Encrypted */

		if (ppciNegotiateInfo instanceof PeertoPeerClient && ppciNegotiateInfo.Connected()) {

			var boolNegotiateEncrypted = ppciNegotiateInfo.CanEncrypt();

			if ((this.boolHasEncryptInfo == boolNegotiateEncrypted) || 
				(!this.boolHasEncryptInfo && boolNegotiateEncrypted)) {

				ppciNegotiateInfo.SetReceivedMsgLateLimit(this.nTimeToLateInMillis);
				ppciNegotiateInfo.SetReceivedDropLateMsgs(this.boolRemoveLateMsgs);
				ppciNegotiateInfo.SetReceivedCheckTimeLimit(this.nTimeToCheckActInMillis);
				ppciNegotiateInfo.SetBackupQueueLimit(this.nBackupQueueMsgLimit);
				
				this.appcListNegotiate.push(ppciNegotiateInfo);

				if (!this.boolHasEncryptInfo && boolNegotiateEncrypted) {
				
					this.CloseClient();
				}
				
				this.boolIsNegotiating = false;
			}
			else {
			
				ppciNegotiateInfo.CloseClient();

				CLIENT.AddLogErrorMsg("During starting client 'Peer To Peer' negotations, adding new one for " + this.strPeerIPAddress + " since new doesn't have encryption when old one does.");
			}
		}
	},
	CheckNegotiation: function() {
			
		this.boolIsANegotiation = false;

		if (!this.boolIsNegotiating && this.appcListNegotiate.length) {
		
			this.SendNegotiation();
			this.boolIsNegotiating = true;
		}
		else if (!this.appcListNegotiate.length) {
		
			this.boolIsNegotiating = false;
		}

		return this.boolIsNegotiating;
	},
	SendNegotiation: function() {

		var appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
			ppciSelected,				/* Selected Negotiating Connection */
			ppciDisconnected = null,	/* Disconnected Selected Negotating "Peer To Peer" Client Information */
			strMsg = CLIENT.GetMsgStartIndicator() + "PEERTOPEERNEGOTIATE" + 
					 CLIENT.GetMsgPartIndicator() + this.strHomeIPAddress + 
					 CLIENT.GetMsgEndIndicator(),
										/* Negotiation Message */
			nListCount = appcNegotiators.length,
										/* List Array Count */
			nCounter = 0;				/* Counter for Loop */

		for (nCounter = 0; nCounter < nListCount; nCounter++) {

			ppciSelected = appcNegotiators[nCounter];
			
			if (ppciSelected.Connected()) {

				ppciSelected.Send(strMsg, false);
			}
			else {
							
				ppciSelected.CloseClient();
				appcNegotiators.splice(nCounter, 1);
				nCounter--;
				nListCount--;
			}
		}
	},
	DoNegotiation: function() {

		var appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
			ppciSelected,				/* Selected Negotiating Connection */
			ppciDisconnected = null,	/* Disconnected Selected Negotating "Peer To Peer" Client Information */
			nListCount = appcNegotiators.length,
										/* List Array Count */
			nCounter = 0;				/* Counter for Loop */

		try {
			
			for (nCounter = 0; nCounter < nListCount; nCounter++) {

				ppciSelected = appcNegotiators[nCounter];

				if (ppciSelected.Connected()) {

					if (ppciSelected.GetStartTimeInMillis() > this.llStartTimeInMillis) {
						
						this.SendNegotiation();
					}
					else if (ppciSelected.GetStartTimeInMillis() == this.llStartTimeInMillis) {

						this.Sync(true);
						ppciSelected.Sync(true);
						this.SendNegotiation();
					}
					else {
			
						this.CloseClient();
					}
				}
				else {

					ppciSelected.CloseClient();
					appcNegotiators.splice(nCounter, 1);
					nCounter--;
					nListCount--;
				}
			}
		}
		catch (exError) {

			CLIENT.AddLogErrorMsg("During running client 'Peer To Peer' negotations, an exception occurred. Exception: " + exError.message);
		}
	},
	Sync: function(boolResync) {
		
		var MSGPARTINDICATOR = CLIENT.GetMsgPartIndicator(),
										/* Message Part Indicator */
			llPreviousTimeInMillis = this.llStartTimeInMillis,
										/* Previous Time In Milliseconds */
			strResyncInfo = "0";		/* Information on Sync Being a Resync, Defaults to 0 as False */

		if (boolResync) {

			this.llStartTimeInMillis = new Date().getTime() + (Math.random() * 100);
			strResyncInfo = "1";
		}

		this.Send(CLIENT.GetMsgStartIndicator() + "NEGOTIATESYNC" + 
				  MSGPARTINDICATOR + this.llStartTimeInMillis + 
				  MSGPARTINDICATOR + strResyncInfo + 
				  MSGPARTINDICATOR + llPreviousTimeInMillis + 
				  CLIENT.GetMsgEndIndicator(), false);
	},
	GetStartTimeInMillis: function() {

		return this.llStartTimeInMillis;
	},
	CanEncrypt: function() {

		return this.boolHasEncryptInfo;
	},
	GetEncryptKey: function() {
		
		return this.strEncryptKey;
	},
	GetEncryptIV: function() {

		return this.strEncryptIV;
	},
	GetPeerIPAddress: function() {

		return this.strPeerIPAddress;
	},
	Connected: function() {

		return this.boolConnected;
	},
	SetReceivedMsgLateLimit: function(nTimeInMillisecs) {

		var appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
			nListCount = appcNegotiators.length,
										/* List Array Count */
			nCounter = 0;				/* Counter for Loop */

		if (Number.isInteger(nTimeInMillisecs)) {
			
			this.nTimeToLateInMillis = nTimeInMillisecs;
	
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
	
				appcNegotiators[nCounter].SetReceivedMsgLateLimit(nTimeInMillisecs);
			}
		}
	},
	SetReceivedDropLateMsgs: function(boolDropLateMsgs) {
		
		var appcNegotiators = this.appcListNegotiate,
									/* List of Negotating "Peer To Peer" Client Information  */
			nListCount = appcNegotiators.length,
									/* List Array Count */
			nCounter = 0;			/* Counter for Loop */

		this.boolRemoveLateMsgs = boolDropLateMsgs;
			
		for (nCounter = 0; nCounter < nListCount; nCounter++) {
			
			appcNegotiators[nCounter].SetReceivedDropLateMsgs(boolDropLateMsgs);
		}
	},
	SetReceivedCheckTimeLimit: function(nTimeInMillis) {

		var appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
			nListCount = appcNegotiators.length,
										/* List Array Count */
			nCounter = 0;				/* Counter for Loop */

		if (Number.isInteger(nTimeInMillis)) {
			
			this.nTimeToCheckActInMillis = nTimeInMillis;
	
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
	
				appcNegotiators[nCounter].SetReceivedCheckTimeLimit(nTimeInMillis);
			}
		}
	},
	SetBackupQueueLimit: function(nNewLimit) {

		var appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
			nListCount = appcNegotiators.length,
										/* List Array Count */
			nCounter = 0;				/* Counter for Loop */

		if (Number.isInteger(nNewLimit)) {
			
			this.nBackupQueueMsgLimit = nNewLimit;
	
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
	
				appcNegotiators[nCounter].SetBackupQueueLimit(nNewLimit);
			}
		}
	},
	CloseClient: function() {

		var appcNegotiators = this.appcListNegotiate,
										/* List of Negotating "Peer To Peer" Client Information  */
			nListCount = appcNegotiators.length,
										/* List Array Count */
			ppciNegotiateWinner = null, /* Selected Negotating "Peer To Peer" Client Information Winner */
			nCounter = 0;				/* Counter for Loop */

		for (nCounter = 0; nCounter < nListCount; nCounter++) {
			
			ppciSelected = appcNegotiators[nCounter];

			if (ppciSelected.Connected()) {

				if (ppciNegotiateWinner == null) { 

					ppciNegotiateWinner = ppciSelected;
				}
				else {
										
					ppciNegotiateWinner.StartNegotiation(ppciSelected);
				}
			}
		}

		if (this.socClient) {

			try {

				this.socClient.close();
			}
			catch(exError) {
				
				CLIENT.AddLogErrorMsg("During closing client 'Peer To Peer' connection, an exception occurred. Exception: " + exError.message);
			}
			
			this.socClient = null;
		}

		this.boolConnected = false;
	},
	ErrorClient: function(eEvent) { 

		CLIENT.AddLogErrorMsg("During operation of 'Peer To Peer' client, error occurred. Code: " + eEvent.code + ", reason: " + eEvent.reason);
		
		this.CloseClient();
	}
};

String.prototype.substr = function(nIndex, nLength) {
	
	if (!Number.isInteger(nLength)) {
		
		nLength = this.length; 
	}
	return this.substring(nIndex, nIndex + nLength);
}

onmessage = function(objData) {
	
	if (objData) {

		CLIENT[objData.data[0]].apply(CLIENT, objData.data.slice(1));
	}
}