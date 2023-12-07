/*
RevCommClient - Connector for RevCommServer and peer to peer communications 
				in the form of RevComm format JSON or "raw" direct messages.

 MIT License

 Copyright (c) 2023 RevComGaming

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
 SOFTWARE.

  OpenSSL License
  ---------------

								 Apache License
						   Version 2.0, January 2004
						https://www.apache.org/licenses/

   TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION

   1. Definitions.

	  "License" shall mean the terms and conditions for use, reproduction,
	  and distribution as defined by Sections 1 through 9 of this document.

	  "Licensor" shall mean the copyright owner or entity authorized by
	  the copyright owner that is granting the License.

	  "Legal Entity" shall mean the union of the acting entity and all
	  other entities that control, are controlled by, or are under common
	  control with that entity. For the purposes of this definition,
	  "control" means (i) the power, direct or indirect, to cause the
	  direction or management of such entity, whether by contract or
	  otherwise, or (ii) ownership of fifty percent (50%) or more of the
	  outstanding shares, or (iii) beneficial ownership of such entity.

	  "You" (or "Your") shall mean an individual or Legal Entity
	  exercising permissions granted by this License.

	  "Source" form shall mean the preferred form for making modifications,
	  including but not limited to software source code, documentation
	  source, and configuration files.

	  "Object" form shall mean any form resulting from mechanical
	  transformation or translation of a Source form, including but
	  not limited to compiled object code, generated documentation,
	  and conversions to other media types.

	  "Work" shall mean the work of authorship, whether in Source or
	  Object form, made available under the License, as indicated by a
	  copyright notice that is included in or attached to the work
	  (an example is provided in the Appendix below).

	  "Derivative Works" shall mean any work, whether in Source or Object
	  form, that is based on (or derived from) the Work and for which the
	  editorial revisions, annotations, elaborations, or other modifications
	  represent, as a whole, an original work of authorship. For the purposes
	  of this License, Derivative Works shall not include works that remain
	  separable from, or merely link (or bind by name) to the interfaces of,
	  the Work and Derivative Works thereof.

	  "Contribution" shall mean any work of authorship, including
	  the original version of the Work and any modifications or additions
	  to that Work or Derivative Works thereof, that is intentionally
	  submitted to Licensor for inclusion in the Work by the copyright owner
	  or by an individual or Legal Entity authorized to submit on behalf of
	  the copyright owner. For the purposes of this definition, "submitted"
	  means any form of electronic, verbal, or written communication sent
	  to the Licensor or its representatives, including but not limited to
	  communication on electronic mailing lists, source code control systems,
	  and issue tracking systems that are managed by, or on behalf of, the
	  Licensor for the purpose of discussing and improving the Work, but
	  excluding communication that is conspicuously marked or otherwise
	  designated in writing by the copyright owner as "Not a Contribution."

	  "Contributor" shall mean Licensor and any individual or Legal Entity
	  on behalf of whom a Contribution has been received by Licensor and
	  subsequently incorporated within the Work.

   2. Grant of Copyright License. Subject to the terms and conditions of
	  this License, each Contributor hereby grants to You a perpetual,
	  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
	  copyright license to reproduce, prepare Derivative Works of,
	  publicly display, publicly perform, sublicense, and distribute the
	  Work and such Derivative Works in Source or Object form.

   3. Grant of Patent License. Subject to the terms and conditions of
	  this License, each Contributor hereby grants to You a perpetual,
	  worldwide, non-exclusive, no-charge, royalty-free, irrevocable
	  (except as stated in this section) patent license to make, have made,
	  use, offer to sell, sell, import, and otherwise transfer the Work,
	  where such license applies only to those patent claims licensable
	  by such Contributor that are necessarily infringed by their
	  Contribution(s) alone or by combination of their Contribution(s)
	  with the Work to which such Contribution(s) was submitted. If You
	  institute patent litigation against any entity (including a
	  cross-claim or counterclaim in a lawsuit) alleging that the Work
	  or a Contribution incorporated within the Work constitutes direct
	  or contributory patent infringement, then any patent licenses
	  granted to You under this License for that Work shall terminate
	  as of the date such litigation is filed.

   4. Redistribution. You may reproduce and distribute copies of the
	  Work or Derivative Works thereof in any medium, with or without
	  modifications, and in Source or Object form, provided that You
	  meet the following conditions:

	  (a) You must give any other recipients of the Work or
		  Derivative Works a copy of this License; and

	  (b) You must cause any modified files to carry prominent notices
		  stating that You changed the files; and

	  (c) You must retain, in the Source form of any Derivative Works
		  that You distribute, all copyright, patent, trademark, and
		  attribution notices from the Source form of the Work,
		  excluding those notices that do not pertain to any part of
		  the Derivative Works; and

	  (d) If the Work includes a "NOTICE" text file as part of its
		  distribution, then any Derivative Works that You distribute must
		  include a readable copy of the attribution notices contained
		  within such NOTICE file, excluding those notices that do not
		  pertain to any part of the Derivative Works, in at least one
		  of the following places: within a NOTICE text file distributed
		  as part of the Derivative Works; within the Source form or
		  documentation, if provided along with the Derivative Works; or,
		  within a display generated by the Derivative Works, if and
		  wherever such third-party notices normally appear. The contents
		  of the NOTICE file are for informational purposes only and
		  do not modify the License. You may add Your own attribution
		  notices within Derivative Works that You distribute, alongside
		  or as an addendum to the NOTICE text from the Work, provided
		  that such additional attribution notices cannot be construed
		  as modifying the License.

	  You may add Your own copyright statement to Your modifications and
	  may provide additional or different license terms and conditions
	  for use, reproduction, or distribution of Your modifications, or
	  for any such Derivative Works as a whole, provided Your use,
	  reproduction, and distribution of the Work otherwise complies with
	  the conditions stated in this License.

   5. Submission of Contributions. Unless You explicitly state otherwise,
	  any Contribution intentionally submitted for inclusion in the Work
	  by You to the Licensor shall be under the terms and conditions of
	  this License, without any additional terms or conditions.
	  Notwithstanding the above, nothing herein shall supersede or modify
	  the terms of any separate license agreement you may have executed
	  with Licensor regarding such Contributions.

   6. Trademarks. This License does not grant permission to use the trade
	  names, trademarks, service marks, or product names of the Licensor,
	  except as required for reasonable and customary use in describing the
	  origin of the Work and reproducing the content of the NOTICE file.

   7. Disclaimer of Warranty. Unless required by applicable law or
	  agreed to in writing, Licensor provides the Work (and each
	  Contributor provides its Contributions) on an "AS IS" BASIS,
	  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
	  implied, including, without limitation, any warranties or conditions
	  of TITLE, NON-INFRINGEMENT, MERCHANTABILITY, or FITNESS FOR A
	  PARTICULAR PURPOSE. You are solely responsible for determining the
	  appropriateness of using or redistributing the Work and assume any
	  risks associated with Your exercise of permissions under this License.

   8. Limitation of Liability. In no event and under no legal theory,
	  whether in tort (including negligence), contract, or otherwise,
	  unless required by applicable law (such as deliberate and grossly
	  negligent acts) or agreed to in writing, shall any Contributor be
	  liable to You for damages, including any direct, indirect, special,
	  incidental, or consequential damages of any character arising as a
	  result of this License or out of the use or inability to use the
	  Work (including but not limited to damages for loss of goodwill,
	  work stoppage, computer failure or malfunction, or any and all
	  other commercial damages or losses), even if such Contributor
	  has been advised of the possibility of such damages.

   9. Accepting Warranty or Additional Liability. While redistributing
	  the Work or Derivative Works thereof, You may choose to offer,
	  and charge a fee for, acceptance of support, warranty, indemnity,
	  or other liability obligations and/or rights consistent with this
	  License. However, in accepting such obligations, You may act only
	  on Your own behalf and on Your sole responsibility, not on behalf
	  of any other Contributor, and only if You agree to indemnify,
	  defend, and hold each Contributor harmless for any liability
	  incurred by, or claims asserted against, such Contributor by reason
	  of your accepting any such warranty or additional liability.

   END OF TERMS AND CONDITIONS
 */

#include "Stdafx.h"
#include <winsock2.h>
#include <ws2tcpip.h>
#include <cstdlib>
#include <string>
#include <fstream>
#include <time.h>
#include <exception>

#include <openssl/bio.h>
#include <openssl/ssl.h>
#include <openssl/err.h>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

class MsgInfo {

	private:

		char* pcharMsg;
		bool boolHasEnd;
		bool boolHasStart;
		int nStartIndex;
		int nEndIndex;
		int nLength;
		MsgInfo* pmsiNextInfo;

		void SetupInfo(char* pcharSetMsg, int nSetMsgLen);
		void SetupInfo(string strSetMsg);

	public:
		
		MsgInfo(char* pcharSetMsg, int nSetLen);
		MsgInfo(string strSetMsg);

		static int FindStringStartIndex(char* pcharCheck, int nCheckLen);
		static int FindStringStartIndex(string strCheck);
		static int FindStringEndIndex(char* pcharCheck, int nMsgLen);
		static int FindStringEndIndex(string strCheck);
		static int FindInString(char* pcharCheck, char* pcharFind, int nCheckLen, int nFindLen);
		static int FindInString(char* pcharCheck, string strFind, int nCheckLen);
		static int FindInString(string strCheck, string strFind);
		static char* FindSegmentInString(char* pcharCheck, int nSegNum, int nCheckLen);
		static char* FindSegmentInString(string strCheck, int nSegNum);
		static int FindSegmentLengthInString(char* pcharCheck, int nSegNum, int nCheckLen);
		static int FindSegmentLengthInString(string strCheck, int nSegNum);
		static char* AppendString(char* pcharMainMsg, int nMainLen, char* pcharAppendString, int nAppendLen);
		static char* AppendString(string strMainMsg, string strAppendString);
		static char* AppendString(char* pcharMainMsg, int nMainLen, char* pcharAppendString, int nAppendLen, bool boolAddEnd);
		static long long GetMetaDataSeqNum(string strCheck);
		static long long GetMetaDataSeqNum(char* pcharCheck, int nCheckLen);
		static long long GetMetaDataTime(string strCheck);
		static long long GetMetaDataTime(char* pcharCheck, int nCheckLen);
		static int FindMetaDataInString(char* pcharMsg, int nMsgLen);
		
		char* GetSegment(int nSegNum);
		int GetSegmentLength(int nSegNum);
		int GetStartIndex();
		int GetEndIndex();
				   
		bool IsComplete();
		bool HasStart();
		bool HasEnd();
		string GetMsgString();
		char* GetMsgArray();
		int Length();

		long long GetMetaDataSeqNum();
		long long GetMetaDataTime();

		void SetNextMsgInfo(MsgInfo* pmsiSetNextInfo);
		MsgInfo* GetNextMsgInfo();
};

class PeerToPeerClientInfo {

	private:

		SOCKET socClient;
		string strHomeIPAddress,
			   strPeerIPAddress;
		char* pcharEncryptKey,
			* pcharEncryptIV,
		    * pcharDecryptKey,
			* pcharDecryptIV,
			* pcharLeftOverMsgPart;
		int nLeftOverMsgLen;
		bool boolHasEncryptInfo,
			 boolIsNegotiating,
			 boolIsANegotiation,
			 boolLeftOverNotRan,
			 boolConnected;
		long long llStartTimeInMillis;
		bool boolNotWouldBlock;

		long long llSendMsgs,			
				  llReceivedMsgs;
		int nTimeToLateInMillis,
			nBackupQueueMsgLimit;
		bool boolMsgsSync,
			 boolRemoveLateMsgs;
		
		int nTimeToCheckActInMillis;
		long long llLastActOrCheckInMillis;

		void SendNegotiation();
		
		char* EncryptMsg(string strMsg, int* pnMsgRetLen);
		int DecryptMsg(char * pcharMsg, int nMsgLen);
		string DecryptMsg(string strMsg);
		void ClearLeftOverEncryptMsg();

		char* AddTracking(char* pcharMsg, int* pnRetMsgLen);	
		void MoveSentMsgToBackup(char* pcharMsg, int nMsgLen);
		void SendReplay(bool boolIsNotCheck);

		MsgInfo* pmsiListStoredReceived,
			   * pmsiListBackupSent;

		PeerToPeerClientInfo* ppciListNegotiate,
										/* List of Duplicate Connections of "Peer To Peer" Clients to be Negotiate */
							* ppciNextInfo;

	public:

		PeerToPeerClientInfo(SOCKET socSetClient, 
							 string strSetHomeIPAddress = "", 
							 string strSetPeerIPAddress = "", 
							 char* pcharSetEncryptKey = NULL, 
							 char* pcharSetEncryptIV = NULL, 
							 char* pcharSetDecryptKey = NULL, 
							 char* pcharSetDecryptIV = NULL);
		
		void Send(string strMsg, bool boolTrack);
		void Send(char* pcharMsg, int nMsgLen, bool boolTrack);
		int Receive(char* pcharReturn);
		string GetMsgString();

		bool CanEncrypt();
		char* GetEncryptKey();
		char* GetEncryptIV();
		void SetLeftOverEncryptMsg(char* pSetLeftOverMsgPart, int nSetLeftOverMsgLen);

		void StartNegotiation(PeerToPeerClientInfo* ppciNegotiateInfo);
		bool CheckNegotiation();
		void DoNegotiation();

		void Sync(bool boolResync);
		long long GetStartTimeInMillis();

		bool Connected();
		bool NotWouldBlock();
		void ClearWouldBlock();
		string GetPeerIPAddress();

		void SetReceivedMsgLateLimit(int nTimeInMillisecs);
		void SetReceivedDropLateMsgs(bool boolDropLateMsgs);
		void SetReceivedCheckTimeLimit(int nTimeInMillis);
		void SetBackupQueueLimit(int nNewLimit);

		void SetNextClientInfo(PeerToPeerClientInfo* ppciSetNextInfo);
		PeerToPeerClientInfo* GetNextClientInfo();
		
		void CloseClient();
};

/* Information on Client and Server Communications */
struct ClientServerInfo {

	static const int BUFFERSIZE = 8192,
									/* Amount to Hold in Buffer for Send to and
									   Receiving from Server */
					 UDPBUFFERSIZE = 512,
									/* Amount to Hold in Buffer for Send to and
									   Receiving UDP Messages from Server */
					 MSGLENSIZE = 10,
									/* Length of Message Size Numbers */
					 ENCRYPTKEYSIZE = 32,
					 ENCRYPTIVSIZE = 16,
									/* Size of Encryption Key and IV Block */
					 SSLCHECKTIMEOUTINMILLIS = 100;
									/* Amount of Time to Wait for SSL Socket to be Available */
	
	ClientServerInfo() {

		Setup();
	}

	void Setup() {

		boolConnected = false;
		boolRunConnection = true;
		hmuxLock = CreateMutex(NULL, false, L"RevCommClient");
		pmsiListToSend = NULL;
		pmsiListReceived = NULL;
		pmsiListStoredToSend = NULL;
		pmsiListStoredReceived = NULL;
		pmsiListBackupSent = NULL;
		strMsgPartIndicate = "!*+#";
		strMsgStartIndicate = "%=&>";
		strMsgEndIndicate = "<@^$";
		charMsgFiller = '\0';
		nMsgIndicatorLen = 4;
		boolPeerToPeerClient = false;
		boolPeerToPeerServer = false;
		strPeerToPeerIP = "";
		nPeerToPeerPort = 59232;
		ppciListClients = NULL;
		nQueueMsgLimit = 2000;
		boolMsgDropped = false;	
		llSendMsgs = 0,			
		llReceivedMsgs = 0;
		nTimeToLateInMillis = 30;
		boolRemoveLateMsgs = false;
		boolMsgsSync = true;
		nTimeToCheckActInMillis = 30;
		llLastActOrCheckInMillis = time(NULL);
		pcharLeftOver = NULL;
		nLeftOverLength = 0;
		boolInGroupSession = false;
		boolGroupSessionHost = false;

		pcharServerHostNameIP = (char*)string("localhost").c_str();
		nServerPort = 59234;
		boolNotServerSet = true;
		socServerConn = NULL;
		socUDPConn = NULL;
		pbioSecureCon = NULL;
		pbioSecureUDPCon = NULL;
		pcharSSLClientKeyName = NULL;
		nUDPInfoSize = 0;
		psctxAccessor = NULL;
		psctxUDPAccess = NULL;
		socPeerToPeer = NULL;	
		boolPeerToPeerEncrypt = false;	
		pcharPeerToPeerDecryptKey = NULL;
		pcharPeerToPeerDecryptIV = NULL;
	}

	bool Connect()  {

		return Connect(false);
	}

	bool Connect(bool boolSetUseSSL)  {

		boolNotServerSet = false;
		return Connect((char *)string("").c_str(), 0, 0, boolSetUseSSL);
	}

	bool Connect(const char* pSetServerHostNameIP, int nSetServerPort, int nSetServerSSLPort, bool boolSetUseSSL) {

		WSADATA wsdWSAInfo;				/* Information Returned from WSA Setup */
		addrinfo* paiList = NULL,
				* paiSelected = NULL,
				aiInfo;					/* List of Host Addresses, Information on Finding Those Addresses, and
										   Holder for Selected Information */
		bool boolConnectStarted = false;/* Indicator That Connection was Started */
		u_long ulMode = 1;				/* Mode for Turning on Non-Blocking in Socket */
		const char charNoDelay = '1';	/* Setting for "No Delay" Option */

		try {

			/* If Client is Not Already Connected to a Server, User Will be Warned to Disconnect First */
			if (!boolConnected) {

				if (!boolRunConnection) {

					Setup();
				}

				if (boolNotServerSet) {

					pcharServerHostNameIP = (char *)pSetServerHostNameIP;
					nServerPort = nSetServerPort;
					boolUseSSL = boolSetUseSSL;
					boolNotServerSet = false;
				}

				if (WSAStartup(MAKEWORD(2, 2), &wsdWSAInfo) == 0) {

					if (boolSetUseSSL) {

						psctxAccessor = SSL_CTX_new(TLS_client_method());
						boolConnectStarted = boolConnected = (pbioSecureCon = ConnectSecure(pcharServerHostNameIP,
																 							IntToString(nSetServerPort),
																							psctxAccessor)) != NULL;
					}
					else {

						ZeroMemory(&aiInfo, sizeof(aiInfo));

						aiInfo.ai_family = AF_UNSPEC;
						aiInfo.ai_socktype = SOCK_STREAM;
						aiInfo.ai_protocol = IPPROTO_TCP;

						if (getaddrinfo(pcharServerHostNameIP, (char*)IntToString(nServerPort).c_str(), &aiInfo, &paiList) == 0) {

							paiSelected = paiList;

							if (paiSelected == NULL) {

								AddLogErrorMsg("During client-server connection for server, getting host address list information failed.");
							}

							while (paiSelected != NULL) {

								socServerConn = socket(paiSelected->ai_family,
													   paiSelected->ai_socktype,
													   paiSelected->ai_protocol);

								if (socServerConn != INVALID_SOCKET) {

									setsockopt(socServerConn, IPPROTO_TCP, TCP_NODELAY, &charNoDelay, sizeof(charNoDelay));

									if (connect(socServerConn, paiSelected->ai_addr, paiSelected->ai_addrlen) == 0) {

										if (ioctlsocket(socServerConn, FIONBIO, &ulMode) != 0) {

											AddLogErrorMsg("During client-server connection for server, setting non-blocking on socket failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
										}

										paiSelected = NULL;
										boolConnectStarted = true;
										boolConnected = true;
									}
									else {

										paiSelected = paiSelected->ai_next;

										AddLogErrorMsg("During client-server connection for server, connecting to server failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
										closesocket(socServerConn);
										socServerConn = NULL;
									}
								}
								else {

									paiSelected = paiSelected->ai_next;
									socServerConn = NULL;
									AddLogErrorMsg("During client-server connection for server, setting up setting up socket for connecting to server failed, connecting to server failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
								}
							}
						}
						else {

							AddLogErrorMsg("During client-server connection for server, getting possible address information failed.");
						}
					}

					if (boolConnected && !AddSendMsg("GETSTREAMFILELIST")) {

						csiOpInfo.AddLogErrorMsg("During client-server connection for server, sending message 'GETSTREAMFILELIST' failed.");
					}
				}
				else {

					AddLogErrorMsg("During client-server connection for server, network access setup failed.");
				}
			}
			else {

				AddLogErrorMsg("During client-server connection for server, client was already connected to server. Connection must be closed first.");
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During client-server connection for server, an exception occurred.", exError.what());
		}

		return boolConnectStarted;
	}

	/* Activates Local Server */
	void ActivateServer() {

		StartServer(nServerPort, false);
	}

	/* Activates Local Server */
	void ActivateServer(int nSetServerPort) {

		StartServer(nSetServerPort, false);
	}
	
	/* Runs Sender and Receiver for Client Messages for Server */
	void Communicate() {
		
		const char MSGFILLERCHAR = charMsgFiller;	
										/* Message Filler Character */
		int nReceivedAmount = 1,		/* Amount Received from Server */
			nUDPAmount = 1,				/* Amount Received from Server Through UDP */
//		char acharMsgReceived[BUFFERSIZE + 1],
										/* Buffer for Receiving Message */
//		     acharUDPMsg[nUDPMaxSize + 1];
										/* Buffer for Receiving UDP Messages */
			nQueueCount = DebugReceivedQueueCount(),
										/* Queue Count */
			nSendErrorCode = 0;			/* Send Error Code */
		pollfd apfdSSLReadCheck[1];		/* Struct Used for Read Ready SSL Socket */
		int nSSLReadCheckResp = 0;		/* Response to SSL Socket Read Check */

		apfdSSLReadCheck[0].events = POLLIN;
	
		try {

			if (socServerConn != NULL || pbioSecureCon != NULL) {

				if (boolRunConnection) {

					ProcessReplay();

					while (boolRunConnection && 
						   nReceivedAmount > 0 && 
						   nSendErrorCode != WSAEWOULDBLOCK && 
						   nQueueCount < nQueueMsgLimit) {

						char acharMsgReceived[BUFFERSIZE + 1];
						memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
					
						if (boolUseSSL) {

							apfdSSLReadCheck[0].fd = BIO_get_fd(pbioSecureCon, NULL);

							nSSLReadCheckResp = WSAPoll(apfdSSLReadCheck, 1, SSLCHECKTIMEOUTINMILLIS);
							
							if (nSSLReadCheckResp > 0) {

								nReceivedAmount = BIO_read(pbioSecureCon, acharMsgReceived, BUFFERSIZE);
							}
							else {

								nReceivedAmount = 0;
							}
						}
						else {

							nReceivedAmount = recv(socServerConn, acharMsgReceived, BUFFERSIZE, 0);
						}

						nSendErrorCode = WSAGetLastError();

						if (boolUseSSL && pbioSecureUDPCon != NULL) {

							apfdSSLReadCheck[0].fd = BIO_get_fd(pbioSecureUDPCon, NULL);

							nSSLReadCheckResp = WSAPoll(apfdSSLReadCheck, 1, SSLCHECKTIMEOUTINMILLIS);

							if (nSSLReadCheckResp > 0) {

								nReceivedAmount = BIO_read(pbioSecureUDPCon, acharMsgReceived, BUFFERSIZE);
							}
							else {

								nReceivedAmount = 0;
							}
						}
						else if (socUDPConn != NULL && 
								 (nSendErrorCode == 0 || 
								  nSendErrorCode == WSAEWOULDBLOCK)) {

							/* If Message Comes from UDP Connection, Add to TCP Message If it Exists */
							
							char acharUDPMsg[UDPBUFFERSIZE + 1];
							memset(acharUDPMsg, MSGFILLERCHAR, UDPBUFFERSIZE + 1);
					
							nUDPAmount = recvfrom(socUDPConn, 
													acharUDPMsg, 
													UDPBUFFERSIZE, 
													0,
													psaiUDPInfo,
													&nUDPInfoSize);

							nSendErrorCode = WSAGetLastError();

							if (nUDPAmount > 0) {

								llLastActOrCheckInMillis = time(NULL);
								AddReceivedMsg(acharUDPMsg, nUDPAmount);
								nQueueCount = DebugReceivedQueueCount();
							}
							else if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {

								AddLogErrorMsg("During running server message sender and receiver, receiving message through UDP from server failed. Error code: " + 
												IntToString(nSendErrorCode));
							}
						}

						if (nReceivedAmount > 0) {
							
							llLastActOrCheckInMillis = time(NULL);
							AddReceivedMsg(acharMsgReceived, nReceivedAmount);
							nQueueCount = DebugReceivedQueueCount();
						}
						else if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {
					
							if (boolUseSSL) {

								AddLogErrorMsg("During running server message sender and receiver, receiving message from server failed through SSL connection. Error code: " +
												IntToString(nSendErrorCode));
							}
							else {

								AddLogErrorMsg("During running server message sender and receiver, receiving message from server failed. Error code: " + IntToString(nSendErrorCode));
							}

							boolRunConnection = false;
						}
					}

					if (boolRunConnection) {

						boolRunConnection = SendMsg();

						if (boolRunConnection) {
							
							if (!boolMsgsSync) {
							
								RepairReceivedMsg(NULL, 0);
							}

							AddStoredMsgs();
							CheckQueueLimitChange();
							CheckLastActivity();
							CheckGroupActivity();
							ProcessPing();
							CheckUDPSwitch();
						}
					}
				}
				else {
			
					Close();
				}
			}
			else {

				AddLogErrorMsg("During running server message sender and receiver, connecting to server failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
				boolRunConnection = false;
			}

			if (!boolRunConnection) {
		
				Close();
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During running server message sender and receiver, an exception occurred.", exError.what());
			boolRunConnection = false;
		}
	}
	
	/* Runs Sender and Receiver for Client Messages for "Peer To Peer" Communications */
	void PeerToPeerCommunicate() {
	
		const char MSGFILLERCHAR = charMsgFiller;	
										/* Message Filler Character */
		SOCKET socNewClient = NULL;		/* Socket for New "Peer To Peer" Client */
		PeerToPeerClientInfo* ppciSelected = NULL,
										/* Selected "Peer To Peer" Client Information */
							* ppciDisconnected = NULL,
										/* Disconnected "Peer To Peer" Client Information */
							* ppciPrevious = NULL;
										/* Previously Selected "Peer To Peer" Client Information */
		int nQueueCount = DebugReceivedQueueCount(),
										/* Queue Count */
			nSendErrorCode = 0,			/* Send Error Code */
//		char* pcharMsg = new char[BUFFERSIZE + 1];			
										/* Received Message */ 
			nMsgLength = 0;				/* Length of Received Message */
		bool boolNotCompleted = false;	/* Indicator That Processing is Not Complete */
		sockaddr_in saiPeerToPeerInfo;	/* Connecting Client Information */
		int nPeerToPeerInfoSize = sizeof(saiPeerToPeerInfo);
										/* Size of Connecting Client Information */
/*		string strHomeIPAddress = "",	/* Client's IP Address */
/*			   strPeerIPAddress = "";	/* Current IP Address */
//			   astrParams[1] = { "" };	/* Message Parameters for Getting Client Encryption Information */
//		MsgInfo* pmsiMsg = NULL;		/* Selected Message Information and Peviously Selected */
//		char* pcharSetEncryptKey = NULL,
//			* pcharSetEncryptIV = NULL; /* Encryption Key and IV Block */
//		bool boolIPNotFound = true;		/* Indicator That New Client was not Already Connected */

		try {

			CheckPeerToPeerDisconnect();

			/* Do Communications with Clients, 
			   Move Through Client and Retrieve Any Messages Including Getting Completed Messages, and
			   Send Current Message */
			
			DoPeerToPeerNegotiation();

			/* Cycle Through and Receive Everything from Clients */
			ppciSelected = ppciListClients;

			while (ppciSelected != NULL) {

				if (!ppciSelected -> Connected()) {

					if (ppciPrevious != NULL) {
					
						ppciPrevious -> SetNextClientInfo(ppciSelected -> GetNextClientInfo());
					}
					else {
					
						ppciListClients = ppciSelected -> GetNextClientInfo();
					}

					ppciDisconnected = ppciSelected;
				}

				if (ppciDisconnected == NULL) {

					ppciPrevious = ppciSelected;
					ppciSelected = ppciSelected -> GetNextClientInfo();
				}
				else {

					ppciSelected = ppciSelected -> GetNextClientInfo();
					ppciDisconnected -> CloseClient();

					delete ppciDisconnected;
					ppciDisconnected = NULL;
				}
			}

			ppciSelected = ppciListClients;

			while (ppciSelected != NULL && 
				   nQueueCount < nQueueMsgLimit) {
				
				char* pcharMsg = new char[BUFFERSIZE + 1];

				memset(pcharMsg, MSGFILLERCHAR, BUFFERSIZE + 1);

				nMsgLength = ppciSelected -> Receive(pcharMsg);

				if (nMsgLength > 0) { 

					AddReceivedMsg(pcharMsg, nMsgLength);
					nQueueCount = DebugReceivedQueueCount();
					boolNotCompleted = true;
				}

				delete pcharMsg;
				pcharMsg = NULL;

				ppciSelected = ppciSelected -> GetNextClientInfo();

				/* If Reached the End of Client List and Not All Client Messages Processed, Start Over */
				if (ppciSelected == NULL && boolNotCompleted) {

					ppciSelected = ppciListClients;
					boolNotCompleted = false;
				}
			}

			/* Clear Any Block Indicators */
			ppciSelected = ppciListClients;

			while (ppciSelected != NULL) {
						
				ppciSelected -> ClearWouldBlock();
				ppciSelected = ppciSelected -> GetNextClientInfo();
			}

			/* Check for Server Messages to Connect to "Peer To Peer" Server as Client */
			CheckStartPeerToPeerConnect();

			/* If "Peer To Peer" Server Setup, Check for New Client's Connecting, 
				Else Check If Main Server Requires It */
			if (boolPeerToPeerServer) {
			 
				/* If a New Client is Connecting, Added to List of Clients or Add as First Client */
				socNewClient = accept(socPeerToPeer, (SOCKADDR *)&saiPeerToPeerInfo, &nPeerToPeerInfoSize);
				nSendErrorCode = WSAGetLastError();

				if (socNewClient != INVALID_SOCKET) {

					string strPeerIPAddress = string(inet_ntoa(saiPeerToPeerInfo.sin_addr)),
						   strHomeIPAddress = "",
						   astrParams[1] = { strPeerIPAddress };
					bool boolIPNotFound = true;	
					

					/* Check If Existing Encryption Infromation Exists */
					MsgInfo* pmsiMsg = DequeueReceivedMsg("PEERTOPEERENCRYPT", astrParams, 1);
					char* pcharSetEncryptKey = NULL,
						* pcharSetEncryptIV = NULL;

					if (pmsiMsg != NULL) {
					
						pcharSetEncryptKey = pmsiMsg -> GetSegment(2);
						pcharSetEncryptIV = pmsiMsg -> GetSegment(3);
					}

					ppciSelected = ppciListClients;
					ppciPrevious = NULL;

					/* Check If Connection Already Exists, Get Previously Connected One for Negotiation Check */
					while (ppciSelected != NULL && boolIPNotFound) {

						if (ppciSelected -> GetPeerIPAddress() == strPeerIPAddress) {

							ppciPrevious = ppciSelected;
							boolIPNotFound = false;

							/* If No Encryption Information Exists for Duplicate Connection, Get Encryption Information from Previous */
							if (pmsiMsg == NULL && ppciSelected -> CanEncrypt()) {
					
								pcharSetEncryptKey = ppciSelected -> GetEncryptKey();
								pcharSetEncryptIV = ppciSelected -> GetEncryptIV();
							}
						}
							
						ppciSelected = ppciSelected -> GetNextClientInfo();
					}

					if (boolIPNotFound) {

						ppciSelected = ppciListClients;	

						if (ppciSelected != NULL) {

							while (ppciSelected -> GetNextClientInfo() != NULL) {

								ppciSelected = ppciSelected -> GetNextClientInfo();
							}

							/* Add Connection with Possible Encryption */
							ppciSelected -> SetNextClientInfo(new PeerToPeerClientInfo(socNewClient, 
																					   strHomeIPAddress,
																					   strPeerIPAddress,
																					   pcharSetEncryptKey, 
																					   pcharSetEncryptIV,
																					   pcharPeerToPeerDecryptKey,
																					   pcharPeerToPeerDecryptIV));
						}
						else {
					
							/* Else This is First Connection and Possibly Has Encryption */
							ppciListClients = new PeerToPeerClientInfo(socNewClient, 
																	   strHomeIPAddress,
																	   strPeerIPAddress,
																	   pcharSetEncryptKey, 
																	   pcharSetEncryptIV,
																	   pcharPeerToPeerDecryptKey,
																	   pcharPeerToPeerDecryptIV);
						}
					}
					else {

						/* Set as Start of List with Possible Encryption */
						ppciPrevious -> StartNegotiation(new PeerToPeerClientInfo(socNewClient, 
																				  strHomeIPAddress,
																				  strPeerIPAddress,
																				  pcharSetEncryptKey, 
																				  pcharSetEncryptIV,
																				  pcharPeerToPeerDecryptKey,
																				  pcharPeerToPeerDecryptIV));
					}

					boolPeerToPeerClient = true;
				}
				else if (nSendErrorCode != WSAEWOULDBLOCK) {
				
					AddLogErrorMsg("During running 'Peer To Peer' client message sender and receiver, accepting 'Peer to Peer' clients failed. WSA error code: " + IntToString(nSendErrorCode) + ".");
				}
			}
			else {

				CheckStartPeerToPeerServer();
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During running 'Peer To Peer' client message sender and receiver, an exception occurred.", exError.what());
			boolRunConnection = false;
		}
	}

	/* Processes MsgReplay Message */
	void ProcessReplay() {

		MsgInfo* pmsiMsg = DequeueReceivedMsg("MSGREPLAY"),		
									/* Message for Getting Message Replay */
			   * pmsiSelect = pmsiListBackupSent; 
									/* Selected Message Information Record */
 // 		int nSeqReplayNum = 0,		/* Message Sequence Number for Start of Replay */
		int nSendLen = 0;			/* Length of Total Message to Send */
		char* pcharWholeSend = NULL;/* Total Message to be Sent */
//		bool boolNoError = true;	/* Indicator That There was Not a Valid Error */
		int nUDPPartLen = 0,		/* Length of Part of Message Being Sent Using UDP */
			nSendErrorCode = 0,		/* Error Code from Sends */
			nCounter = 0;			/* Counter for Loop */
		pollfd apfdSSLWriteCheck[1];/* Struct Used for Write Ready SSL Socket */
		int nSSLWriteCheckResp = 0;	/* Response to SSL Socket Write Check */
		string strErrorMsg = "";	/* Error Message */

		try {

			if (pmsiMsg != NULL) {
			
				int nSeqReplayNum = atoi(MsgInfo::FindSegmentInString(pmsiMsg -> GetMsgString(), 1));

				while (pmsiSelect != NULL) {
				
					if (pmsiSelect -> GetMetaDataSeqNum() >= nSeqReplayNum) {
					
						if (pcharWholeSend != NULL) {
						
							pcharWholeSend = MsgInfo::AppendString(pcharWholeSend, nSendLen, pmsiSelect -> GetMsgArray(), pmsiSelect -> Length());
						}
						else {
						
							pcharWholeSend = pmsiSelect -> GetMsgArray();
						}
						
						nSendLen += pmsiSelect->Length();
					}

					pmsiSelect = pmsiSelect -> GetNextMsgInfo();
				}

				if (nSendLen > 0) {

					apfdSSLWriteCheck[0].events = POLLOUT;

					if (socUDPConn == NULL) {

						if (boolUseSSL) {

							apfdSSLWriteCheck[0].fd = BIO_get_fd(pbioSecureCon, NULL);

							nSSLWriteCheckResp = WSAPoll(apfdSSLWriteCheck, 1, SSLCHECKTIMEOUTINMILLIS);

							if (nSSLWriteCheckResp > 0) {

								if (BIO_write(pbioSecureCon, pcharWholeSend, nSendLen) <= 0) {

									strErrorMsg = "During sending replay messages, send replay messages using SSL failed.";
								}
							}
							else if (nSSLWriteCheckResp == SOCKET_ERROR) {

								strErrorMsg = "During sending replay messages, socket error occurred.";
							}
						}
						else if (send(socServerConn, pcharWholeSend, nSendLen, 0) == SOCKET_ERROR) {

							strErrorMsg = "During sending replay messages, send replay messages failed.";
						}
					}
					else if (boolUseSSL) {

						apfdSSLWriteCheck[0].fd = BIO_get_fd(pbioSecureUDPCon, NULL);

						nSSLWriteCheckResp = WSAPoll(apfdSSLWriteCheck, 1, SSLCHECKTIMEOUTINMILLIS);

						if (nSSLWriteCheckResp > 0) {

							if (BIO_write(pbioSecureUDPCon, pcharWholeSend, nSendLen) <= 0) {

								strErrorMsg = "During sending replay messages, sending messages using UDP to server failed using SSL.";
							}
						}
						else if (nSSLWriteCheckResp == SOCKET_ERROR) {

							strErrorMsg = "During sending replay messages, socket error occurred using UDP and SSL.";
						}
					}
					else {

						bool boolNoError = true;

						/* Else Do UDP Send, Break up Message into Smaller Peices for Send */
						for (nCounter = 0; nCounter < nSendLen && boolNoError; nCounter += UDPBUFFERSIZE) {

							if (nSendLen - nCounter >= UDPBUFFERSIZE) {

								nUDPPartLen = UDPBUFFERSIZE;
							}
							else {

								nUDPPartLen = nSendLen - nCounter;
							}

							if (sendto(socUDPConn,
								pcharWholeSend + nCounter,
								nUDPPartLen,
								0,
								psaiUDPInfo,
								nUDPInfoSize) != nUDPPartLen) {

								strErrorMsg = "During sending replay messages, sending messages using UDP to server failed.";
								boolNoError = false;
							}
						}
					}

					if (strErrorMsg != "") {

						nSendErrorCode = WSAGetLastError();

						if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {

							AddLogErrorMsg(strErrorMsg + " Error code : " + IntToString(nSendErrorCode));
						}
					}
				}
				else {
				
					AddLogErrorMsg("During sending replay messages, no replay messages found, replay failed.");
				}
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During sending replay messages, an exception occurred.", exError.what());
		}
	}

	/* Processes Ping Message */
	void ProcessPing() {

		MsgInfo* pmsiMsg = DequeueReceivedMsg("PINGSEND");				
									/* Message for Send Ping */
//		string strPingTime = "";	/* Time of Sent Ping */
//		string astrParams[1];		/* Holder for Ping Time to Send Back to Server */

		if (pmsiMsg != NULL) {
			
			string strPingTime = string(MsgInfo::FindSegmentInString(pmsiMsg -> GetMsgString(), 1));
			string astrParams[1] = { strPingTime.substr(0, MsgInfo::FindSegmentLengthInString(pmsiMsg -> GetMsgString(), 1)) };

			AddSendMsg("PINGRETURN", astrParams, 1, false);

			delete pmsiMsg;
			pmsiMsg = NULL;
		}
	}

	/* Start Server for Receiving "Peer To Peer" Connections 
	   Returns: True If Server was Started, Else False If Not Started or Already Running */
	bool StartPeerToPeerServer(string strIPAddress, int nPort)  {
	
		bool boolServerStarted = false;
									/* Indicator That "Peer To Peer" Server was Started */

		if (!boolPeerToPeerServer) {
		
			strPeerToPeerIP = strIPAddress;
			nPeerToPeerPort = nPort;
			StartPeerToPeerServer();
			boolServerStarted = boolPeerToPeerServer; 
		}

		return boolServerStarted;
	}
	

	/* Start Server for Receiving "Peer To Peer" Connections with Encryption using Keys
	   Returns: True If Server was Started, Else False If Not Started or Already Running */
	bool StartPeerToPeerServerEncryptedWithKeys(string strIPAddress, int nPort, char* pcharKey, char* pcharIV) {
	
		bool boolServerStarted = false;
									/* Indicator That "Peer To Peer" Server was Started */
					
		if (!boolPeerToPeerEncrypt && !boolPeerToPeerServer) {	

			boolPeerToPeerEncrypt = true;
			pcharPeerToPeerDecryptKey = new char[ENCRYPTKEYSIZE + 1];
			pcharPeerToPeerDecryptIV = new char[ENCRYPTIVSIZE + 1];

			PrepCharArrayOut(pcharKey, pcharPeerToPeerDecryptKey, ENCRYPTKEYSIZE, ENCRYPTKEYSIZE + 1);
			PrepCharArrayOut(pcharIV, pcharPeerToPeerDecryptIV, ENCRYPTIVSIZE, ENCRYPTIVSIZE + 1);

			EVP_add_cipher(EVP_aes_256_cbc());

			boolServerStarted = StartPeerToPeerServer(strIPAddress, nPort); 
		}

		return boolServerStarted;
	}
	
	/* Connect Client to "Peer To Peer" Server */
	bool StartPeerToPeerConnect(string strPeerToPeerServerIP, string strPeerToPeerServerPort, char* pcharSetEncryptKey = NULL, char* pcharSetEncryptIV = NULL) {
		
/*		const char charNoDelay = '1';	/* Setting for "No Delay" Option */
		sockaddr_in saiPeerToPeerInfo;	/* Host Server Information */
		u_long ulMode = 1;				/* Mode for Turning on Non-Blocking in Socket */
		SOCKET socServer;				/* Socket for Server Connection */
		PeerToPeerClientInfo* ppciSelected = NULL,
										/* Selected "Peer To Peer" Client Information */
							* ppciPrevious = NULL;
										/* Previously Selected "Peer To Peer" Client Information */
		bool boolClientConnected = false;
										/* Indicator That Client was Connected */
		char* pcharEncryptKey = NULL, 
			* pcharEncryptIV = NULL;	/* Encryption Key and IV Block */
/*		char* pcharEncryptSave = NULL;	/* Information for Saving Encryption Info on Failed Connection */
/*		int nEncryptSaveIndex = 0,		/* Current Index of Encryption Information to Save */
/*			nEncryptSaveLen = 0,		/* Length of Encryption Information to Save  */
/*			nMsgPartIndicateLen = strMsgPartIndicate.length();
										/* Message Part Indicator Length */
		string strHomeIPAddress = "";	/* Current IP Address */
/*			   astrParams[1] = { strPeerToPeerServerIP };
										/* Parameter to Check Client Encryption Already Registered */
		bool boolIPNotFound = true;		/* Indicator That New Client was not Already Connected */
						
		/* If Information was Found, Connect to "Peer To Peer" Server */
		if (strPeerToPeerServerIP != "" && strPeerToPeerServerPort != "") {


			ppciSelected = ppciListClients;

			while (ppciSelected != NULL && boolIPNotFound) {
			
				if (ppciSelected -> GetPeerIPAddress() == strPeerToPeerServerIP) {
							
					ppciPrevious = ppciSelected;
					boolIPNotFound = false;
				}
							
				ppciSelected = ppciSelected -> GetNextClientInfo();
			}

			socServer = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

			if (socServer != INVALID_SOCKET) {

				saiPeerToPeerInfo.sin_family = AF_INET;
				saiPeerToPeerInfo.sin_port = htons(atoi(strPeerToPeerServerPort.c_str()));
 				saiPeerToPeerInfo.sin_addr.S_un.S_addr = inet_addr(strPeerToPeerServerIP.c_str());

				if (pcharSetEncryptKey != NULL && pcharSetEncryptIV != NULL) {
		
					pcharEncryptKey = new char[ENCRYPTKEYSIZE + 1], 
					pcharEncryptIV = new char[ENCRYPTIVSIZE + 1];

					PrepCharArrayOut(pcharSetEncryptKey, pcharEncryptKey, ENCRYPTKEYSIZE, ENCRYPTKEYSIZE + 1);
					PrepCharArrayOut(pcharSetEncryptIV, pcharEncryptIV, ENCRYPTIVSIZE, ENCRYPTIVSIZE + 1);
				}
				
				const char charNoDelay = '1';
				setsockopt(socServerConn, IPPROTO_TCP, TCP_NODELAY, &charNoDelay, sizeof(charNoDelay));

				if (connect(socServer, (SOCKADDR *)&saiPeerToPeerInfo, sizeof(saiPeerToPeerInfo)) == 0) {

					try {

						ppciSelected = ppciListClients;

						if (ppciSelected != NULL) {

							while (ppciSelected -> GetNextClientInfo() != NULL) {
						
								ppciSelected = ppciSelected -> GetNextClientInfo();
							}

							if (boolIPNotFound) {

								ppciSelected -> SetNextClientInfo(new PeerToPeerClientInfo(socServer, 
																						   strHomeIPAddress,
																						   strPeerToPeerServerIP, 
																						   pcharEncryptKey, 
																						   pcharEncryptIV, 
																						   pcharPeerToPeerDecryptKey, 
																						   pcharPeerToPeerDecryptIV));
							}
							else {
							
								ppciPrevious -> StartNegotiation(new PeerToPeerClientInfo(socServer, 
																						  strHomeIPAddress,
																						  strPeerToPeerServerIP, 
																						  pcharEncryptKey, 
																						  pcharEncryptIV, 
																						  pcharPeerToPeerDecryptKey, 
																						  pcharPeerToPeerDecryptIV));
							}
						}
						else {
								
							ppciListClients = new PeerToPeerClientInfo(socServer, 
																	   strHomeIPAddress,
																	   strPeerToPeerServerIP, 
																	   pcharEncryptKey, 
																	   pcharEncryptIV, 
																	   pcharPeerToPeerDecryptKey, 
																	   pcharPeerToPeerDecryptIV);

							ppciListClients -> SetReceivedMsgLateLimit(nTimeToLateInMillis);
							ppciListClients -> SetReceivedDropLateMsgs(boolRemoveLateMsgs);
							ppciListClients -> SetReceivedCheckTimeLimit(nTimeToCheckActInMillis);
							ppciListClients -> SetBackupQueueLimit(nQueueMsgLimit);
						}

						boolPeerToPeerClient = true;
						boolClientConnected = true;
					}
					catch (exception& exError) {

						AddLogErrorMsg("During connecting to 'Peer To Peer' server, an exception occurred.", exError.what());
					}
				}
				else {
					
					AddLogErrorMsg("During connecting to 'Peer To Peer' server, connecting to server failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
					closesocket(socServer);
					socServer = NULL;
					
					/* If Encryption Information Doesn't Already Exist for this Client, Store for Later Connection */
					string astrParams[1] = { strPeerToPeerServerIP };

					if (pcharEncryptKey != NULL && pcharEncryptIV != NULL && FindReceivedMsg("PEERTOPEERENCRYPT", astrParams, 1) == NULL) {

						int nEncryptSaveIndex = 0,
							nMsgPartIndicateLen = strMsgPartIndicate.length(),
							nEncryptSaveLen = strMsgStartIndicate.length() + strMsgEndIndicate.length() + nMsgPartIndicateLen * 3 + strPeerToPeerServerIP.length() + ENCRYPTKEYSIZE + ENCRYPTIVSIZE + 18;
						char* pcharEncryptSave = new char[nEncryptSaveLen];

						PrepStringOut(strMsgStartIndicate, pcharEncryptSave, nEncryptSaveLen);
						nEncryptSaveIndex = strMsgStartIndicate.length();

						PrepStringOut("PEERTOPEERENCRYPT", pcharEncryptSave + nEncryptSaveIndex, nEncryptSaveLen);
						nEncryptSaveIndex += 17;

						PrepStringOut(strMsgPartIndicate, pcharEncryptSave + nEncryptSaveIndex, nEncryptSaveLen);
						nEncryptSaveIndex += nMsgPartIndicateLen;

						PrepStringOut(strPeerToPeerServerIP, pcharEncryptSave + nEncryptSaveIndex, nEncryptSaveLen);
						nEncryptSaveIndex += strPeerToPeerServerIP.length();

						PrepStringOut(strMsgPartIndicate, pcharEncryptSave + nEncryptSaveIndex, nEncryptSaveLen);
						nEncryptSaveIndex += nMsgPartIndicateLen;

						PrepCharArrayOut(pcharEncryptKey, pcharEncryptSave + nEncryptSaveIndex, ENCRYPTKEYSIZE, nEncryptSaveLen);
						nEncryptSaveIndex += ENCRYPTKEYSIZE;

						PrepStringOut(strMsgPartIndicate, pcharEncryptSave + nEncryptSaveIndex, nEncryptSaveLen);
						nEncryptSaveIndex += nMsgPartIndicateLen;

						PrepCharArrayOut(pcharEncryptIV, pcharEncryptSave + nEncryptSaveIndex, ENCRYPTIVSIZE, nEncryptSaveLen);
						nEncryptSaveIndex += ENCRYPTIVSIZE;

						PrepStringOut(strMsgEndIndicate, pcharEncryptSave + nEncryptSaveIndex, nEncryptSaveLen);
						nEncryptSaveIndex += strMsgEndIndicate.length();

						AddReceivedMsg(pcharEncryptSave, nEncryptSaveLen);
					}
				}
			}
			else {
							
				socServer = NULL;
				AddLogErrorMsg("During connecting to 'Peer To Peer' server, setting up setting up socket for connecting to server failed, connecting to server failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
			}
		}			
		else {
		
			AddLogErrorMsg("During connecting to 'Peer To Peer' server, invalid server IP or port sent.");
		}

		return boolClientConnected;
	}


	/* Add New Message to Be Sent to List of Waiting Messages to Be Sent 
	   Returns True If Message was Successfully Stored, Else False */
	bool AddSendMsg(string strMsgTypeName, string astrMsgParams[], int nMsgParamsLen, bool boolTrack) {

		string strMsg = strMsgTypeName;	/* Message to be Sent */
		int nMsgLen = 0;				/* Length of Message Parameters and 
					 					   Length of Message After Adding End Characters to Message */
		bool boolMsgAdded = false;		/* Indicator That Message was Added */
		int nCounter = 0; 				/* Counter for Loop */

		try {
			
			if (boolConnected) {

				if (boolTrack) {

					strMsg += "-" + to_string(++llSendMsgs);
				}

				/* Part Together Message to be Sent with Indicators for Parameters and Added the End Afterwards */
				for (nCounter = 0; nCounter < nMsgParamsLen; nCounter++) {
	
					 strMsg += strMsgPartIndicate + astrMsgParams[nCounter];
				}
	
				boolMsgAdded = EnqueueSendMsg(strMsgStartIndicate + strMsg + strMsgEndIndicate);
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During adding 'Send To' message of type, '" + strMsgTypeName + "', an exception occurred.", exError.what());
		}

		return boolMsgAdded;
	}

	bool AddSendMsg(string strMsgTypeName, string astrMsgParams[], int nMsgParamsLen) {
	
		return AddSendMsg(strMsgTypeName, astrMsgParams, nMsgParamsLen, true);
	}

	bool AddSendMsg(string strMsgTypeName) { 

		string astrEmpty[1] = {""};		/* Empty Array for Passing No Parameters */

		return AddSendMsg(strMsgTypeName, astrEmpty, 0);
	}

	/* Adds Stored Messages to Queues If Available */
	void AddStoredMsgs() {

		try {

			if (pmsiListStoredReceived == NULL && pmsiListStoredToSend == NULL && boolMsgDropped) {
			
				AddLogErrorMsg("During adding stored messages, discovered that messages were dropped while store queues were full or message was incomplete.");
				boolMsgDropped = false;
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During adding stored messages, an exception occurred.", exError.what());
		}
	}

	/* Sends Message to "Peer To Peer" Clients */
	bool SendPeerToPeerMsg(string strMsgTypeName, string astrMsgParams[], int nMsgParamsLen) {

		string strMsg = strMsgTypeName;	
									/* Message to be Sent */
		char* pcharConvertMsg = NULL;		
									/* Converted Version of Message That is Possibly Encrypted */
		int nMsgLen = 0;			/* Length of Message */
		PeerToPeerClientInfo* ppciSelected = NULL;
									/* Selected "Peer To Peer" Client Information */
		bool boolNoError = true;	/* Indicator That There was Not a Valid Error */
		int nCounter = 0; 			/* Counter for Loop */

		try {
					
			/* If Using "Peer To Peer" Communications, Move Through Client and Send Current Message */
			if (boolPeerToPeerClient) {
			
				/* Part Together Message to be Sent with Indicators for Parameters and Added the End Afterwards */
				for (nCounter = 0; nCounter < nMsgParamsLen; nCounter++) {
	
					 strMsg += strMsgPartIndicate + astrMsgParams[nCounter];
				}

				strMsg = strMsgStartIndicate + strMsg + strMsgEndIndicate;

				pcharConvertMsg = new char[strMsg.length() + 1];
				nMsgLen = PrepStringOut(strMsg, pcharConvertMsg, strMsg.length() + 1);

				ppciSelected = ppciListClients;

				while (ppciSelected != NULL) {

					if (nMsgLen <= BUFFERSIZE) {
							
						ppciSelected -> Send(pcharConvertMsg, nMsgLen, true);
					}
					else {

						for (nCounter = 0; nCounter < nMsgLen; nCounter += BUFFERSIZE) {

							if ((nMsgLen - nCounter) >= BUFFERSIZE) {
								
								ppciSelected -> Send(pcharConvertMsg + nCounter, BUFFERSIZE, true);
							}
							else {

								ppciSelected -> Send(pcharConvertMsg + nCounter, nMsgLen - nCounter, true);
							}
						}
					}

					ppciSelected = ppciSelected -> GetNextClientInfo();
				}
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During sending 'Peer To Peer' message, an exception occurred.", exError.what());
			boolNoError = false;
		}

		return boolNoError;
	}

	/* Finds Received Message By Message Type and Message Criteria List
	   Return: Message String, Else Empty String If Not Found or Fails */
	MsgInfo* FindReceivedMsg(string strMsgTypeName, string astrMsgCrit[], int nMsgCritLen) {
		
		MsgInfo* pmsiReturn = NULL; /* Message to be Returned */

		try {

			pmsiReturn = FindReceivedMsg(strMsgTypeName, astrMsgCrit, nMsgCritLen, false);
		}
		catch (exception& exError) {
			
			AddLogErrorMsg("During finding message type, '" + strMsgTypeName + "' for retrieval, getting search critiera failed.", exError.what());
		}

		return pmsiReturn;
	}

	MsgInfo* FindReceivedMsg(string strMsgTypeName) { 
		
		string astrParams[1] = { "" };
									/* Message Parameters */

		return FindReceivedMsg(strMsgTypeName, astrParams, 0, false); 
	}

	/* Finds Received Message By Message Type with Criteria List Match
	   Return: Message and Removes it from List of Received Messages, Else Empty String If Not Found or Fails */
	MsgInfo* DequeueReceivedMsg(string strMsgTypeName, string astrMsgCrit[], int nMsgCritLen) {

		return FindReceivedMsg(strMsgTypeName, astrMsgCrit, nMsgCritLen, true); 
	}

	MsgInfo* DequeueReceivedMsg(string strMsgTypeName) { 

		string astrParams[1] = { "" };
									/* Message Parameters */

		return FindReceivedMsg(strMsgTypeName, astrParams, 0, true); 
	}

	/* Dequeues Stored Message By Message Type with an Optional Search String
	   Return: Message and Removes it from List of Stored Messages, else NULL */
	MsgInfo* DequeueStoredMsg(string strMsgTypeName, string astrMsgCrit[], int nMsgCritLen) {
		
		MsgInfo* pmsiSelect = NULL;		/* Selected Message Information */
		bool boolCritMatch = true,		/* Indicator That the Message  */
			 boolNotEndFound = true;	/* Indicator That End of Message was Not Found */
		MsgInfo* pmsiPrevious = NULL,
			   * pmsiStart = NULL,
			   * pmsiEnd = NULL;		/* Previous, Start and End of Message Information */
		int nCounter = 0;				/* Counter for Loop */

		try {
	
			/* Cycle Through List of Stored Messages */
			pmsiPrevious = pmsiSelect = pmsiListStoredReceived;		

			while (pmsiSelect != NULL && boolNotEndFound) {

				if (pmsiStart == NULL) {
					
					for (nCounter = 0; nCounter < nMsgCritLen && boolCritMatch; nCounter++) {

						boolCritMatch = MsgInfo::FindInString(pmsiSelect -> GetSegment(nCounter + 1), astrMsgCrit[nCounter], pmsiSelect -> GetSegmentLength(nCounter + 1)) == 0;
					}

					if (boolCritMatch && MsgInfo::FindInString(pmsiSelect -> GetSegment(0), strMsgTypeName, pmsiSelect -> GetSegmentLength(0)) == 0) {

						/* The First Part of the Message was Found, Grab the Message Information That it as in,
							Check for the End of the Message in the Message Information */
						pmsiStart = pmsiSelect; 
		
						if (pmsiSelect -> IsComplete()) {
					
							/* End of Message was Found in the Same Message Information, Grab the Message Information it was in, Set Indicator */
							pmsiEnd = pmsiSelect;
							boolNotEndFound = false;
						}
					}
					
					boolCritMatch = true;
				}
				else if (pmsiSelect -> HasEnd()) {

					pmsiEnd = pmsiSelect;
					boolNotEndFound = false;	
				}

				/* Grab the Currently Selected Message Information, 
				   Until the Message Information Containing the Start of the Message is Found, So the Previous One Will Already be Found */
				if (pmsiStart == NULL) {
			
					pmsiPrevious = pmsiSelect;
				}

				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}
		
			/* If Beginning and End of Message was Found, Remove the Processed Message Information If Directed To */
			if (pmsiStart != NULL && pmsiEnd != NULL) {

				/* If It is the First Message Information, Make the One After the End the First, 
					Else Connect the Previous to the Next One */
				if (pmsiListStoredReceived == pmsiStart) {
						
					pmsiListStoredReceived = pmsiEnd -> GetNextMsgInfo();
				}
				else {
						
					pmsiPrevious -> SetNextMsgInfo(pmsiEnd -> GetNextMsgInfo());
				}

				pmsiEnd -> SetNextMsgInfo(NULL);
			}
			else {
					
				pmsiStart = NULL;
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During dequeuing stored message type, '" + strMsgTypeName + "', an exception occurred.", exError.what());
		}
	
		return pmsiStart;
	}

	/* Dequeues Stored Message By Message Type by Segment Number
	   Return: Message and Removes it from List of Stored Messages, else NULL */
	MsgInfo* DequeueStoredMsg(int nSegNumSelect) {
		
		MsgInfo* pmsiSelect = NULL;		/* Selected Message Information */
		bool boolNotEndFound = true;	/* Indicator That End of Message was Not Found */
		MsgInfo* pmsiPrevious = NULL,
			   * pmsiStart = NULL,
			   * pmsiEnd = NULL;		/* Previous, Start and End of Message Information */

		try {
	
			/* Cycle Through List of Stored Messages */
			pmsiPrevious = pmsiSelect = pmsiListStoredReceived;	

			while (pmsiSelect != NULL && boolNotEndFound) {

				if (pmsiStart == NULL) {

					if (pmsiSelect -> GetMetaDataSeqNum() == nSegNumSelect && pmsiSelect -> HasStart()) { 

						/* The First Part of the Message was Found, Grab the Message Information That it as in,
						   Check for the End of the Message in the Message Information */
						pmsiStart = pmsiSelect; 
		
						if (pmsiSelect -> IsComplete()) {
					
							/* End of Message was Found in the Same Message Information, 
							   Grab the Message Information it was in, Set Indicator */
							pmsiEnd = pmsiSelect;
							boolNotEndFound = false;
						}
					}
				}
				else if (pmsiSelect -> HasEnd()) {

					pmsiEnd = pmsiSelect;
					boolNotEndFound = false;	
				}

				/* Grab the Currently Selected Message Information, 
				   Until the Message Information Containing the Start of the Message is Found, So the Previous One Will Already be Found */
				if (pmsiStart == NULL) {
			
					pmsiPrevious = pmsiSelect;
				}

				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}
		
			/* If Beginning and End of Message was Found, Remove the Processed Message Information If Directed To */
			if (pmsiStart != NULL && pmsiEnd != NULL) {

				/* If It is the First Message Information, Make the One After the End the First, 
				   Else Connect the Previous to the Next One */
				if (pmsiListStoredReceived == pmsiStart) {
						
					pmsiListStoredReceived = pmsiEnd -> GetNextMsgInfo();
				}
				else {
						
					pmsiPrevious -> SetNextMsgInfo(pmsiEnd -> GetNextMsgInfo());
				}

				pmsiEnd -> SetNextMsgInfo(NULL);
			}
			else {
					
				pmsiStart = NULL;
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During dequeuing stored message segment number, '" + IntToString(nSegNumSelect) + "', an exception occurred.", exError.what());
		}
	
		return pmsiStart;
	}

	/* Finds Received Message By Message Type with an Optional Search String and Deletes Them */
	bool ClearReceivedMsg(string strMsgTypeName, string astrMsgCrit[], int nMsgCritLen) {
		
		bool boolSuccess = false;	/* Indicator That Deletion was Successful */

		try {

			while (FindReceivedMsg(strMsgTypeName,  astrMsgCrit, nMsgCritLen, true) != NULL) {} 

			boolSuccess = true;
		}
		catch (exception& exError) {
			
			AddLogErrorMsg("During finding message type, '" + strMsgTypeName + "' for deletion, exception occurred.", exError.what());
		}

		return boolSuccess;
	}

	bool ClearReceivedMsg(string strMsgTypeName) {
			
		string astrEmpty[1] = {""};		/* Empty Array for Passing No Parameters */

		return ClearReceivedMsg(strMsgTypeName, astrEmpty, 0);
	}

	/* Processes File from Stream */
	MsgInfo* ProcessFile(string astrMsgCrit[], int nMsgCritLen) {
	
		const string MSGPARTINDICATOR = strMsgPartIndicate;
		const int MSGPARTINDICATORLEN = MSGPARTINDICATOR.length();
									/* Message Part Indicator and its Length */
		MsgInfo* pmsiStart = NULL,
			   * pmsiSelect = NULL,
			   * pmsiPrevious = NULL;	/* File Starting, Selected and Previously Selected Information */
		char* pcharFileDesign = NULL,	
									/* File Designation */
			* pcharFilePath = NULL, /* File Path */
			* pcharFilePathCheck = NULL,
			* pcharFileDesignCheck = NULL;	
									/* File Path Name and Designation to Test for in Message Parts */
		int nFileDesignLen = 0,		/* Length of File Designation */
			nFilePathLen = 0,		/* Length of File Path */
			nFileLen = 0,			/* Length of File */
			nFileDesignCheckLen = 0,/* Length of File Designation */
			nFilePathCheckLen = 0,	/* Length of File Path */
			nMsgNumTest = 0,		/* File Part Number to Test */
			nMsgNumLast = 0;		/* Last File Part Number to Test */
		string strErrorMsg = "";	/* Error Message */
		bool boolContinue = true;	/* Indicator to Continue Processing File */
		
		try {

			/* If the Beginning and End of the Expected Message Exists */
			if (FindReceivedMsg("FILESTART", astrMsgCrit, nMsgCritLen, false) != NULL &&
				FindReceivedMsg("FILEEND", astrMsgCrit, nMsgCritLen, false) != NULL) {
				
				pmsiStart = FindReceivedMsg("FILESTART", astrMsgCrit, nMsgCritLen, true);

				pcharFileDesign = pmsiStart -> GetSegment(1);
				nFileDesignLen = pmsiStart -> GetSegmentLength(1);

				pcharFilePath = pmsiStart -> GetSegment(2);	
				nFilePathLen = pmsiStart -> GetSegmentLength(2);

				nFileLen = atoi(pmsiStart -> GetSegment(3));

				pmsiPrevious = pmsiStart;

				while (pmsiPrevious -> GetNextMsgInfo() != NULL) {
				
					pmsiPrevious = pmsiPrevious -> GetNextMsgInfo();
				}

				pmsiSelect = FindReceivedMsg("FILEPART", astrMsgCrit, nMsgCritLen, true);

				while (boolContinue && pmsiSelect != NULL) {
						
					pcharFileDesignCheck = pmsiSelect -> GetSegment(1);
					nFileDesignCheckLen = pmsiSelect -> GetSegmentLength(1);

					pcharFilePathCheck = pmsiSelect -> GetSegment(2);	
					nFilePathCheckLen = pmsiSelect -> GetSegmentLength(2);

					nMsgNumTest = atoi(pmsiSelect -> GetSegment(3));

					if (MsgInfo::FindInString(pcharFileDesign, pcharFileDesignCheck, nFileDesignLen, nFileDesignCheckLen) >= 0 &&
						MsgInfo::FindInString(pcharFilePath, pcharFilePathCheck, nFilePathLen, nFilePathCheckLen) >= 0 && 
						nMsgNumTest > nMsgNumLast && 
						nFileLen > nMsgNumTest) {

						pmsiPrevious -> SetNextMsgInfo(pmsiSelect);
						pmsiPrevious = pmsiSelect;
						nMsgNumLast = nMsgNumTest;

						while (pmsiPrevious -> GetNextMsgInfo() != NULL) {
				
							pmsiPrevious = pmsiPrevious -> GetNextMsgInfo();
						}
						
						pmsiSelect = FindReceivedMsg("FILEPART", astrMsgCrit, nMsgCritLen, true);
					}
					else {

						strErrorMsg = "During processing file from stream, getting file parts failed due out of order messages. File type: '" + string(pcharFileDesign) + "' messages deleted. " + IntToString(nMsgNumTest) + " " + IntToString(nMsgNumLast) + " " + IntToString(nFileLen);
						boolContinue = false;
					}
				}

				if (boolContinue) {
					
					pmsiSelect = FindReceivedMsg("FILEEND", astrMsgCrit, nMsgCritLen, true);

					while (boolContinue && pmsiSelect != NULL) {
						
						pcharFileDesignCheck = pmsiSelect -> GetSegment(1);
						nFileDesignCheckLen = pmsiSelect -> GetSegmentLength(1);

						pcharFilePathCheck = pmsiSelect -> GetSegment(2);	
						nFilePathCheckLen = pmsiSelect -> GetSegmentLength(2);

						nMsgNumTest = atoi(pmsiSelect -> GetSegment(3));

						if (MsgInfo::FindInString(pcharFileDesign, pcharFileDesignCheck, nFileDesignLen, nFileDesignCheckLen) >= 0 &&
							MsgInfo::FindInString(pcharFilePath, pcharFilePathCheck, nFilePathLen, nFilePathCheckLen) >= 0 && 
							nMsgNumTest >= nMsgNumLast && 
							nFileLen >= nMsgNumTest) {

							pmsiPrevious -> SetNextMsgInfo(pmsiSelect);
							pmsiPrevious = pmsiSelect;
							nMsgNumLast = nMsgNumTest;

							while (pmsiPrevious -> GetNextMsgInfo() != NULL) {
				
								pmsiPrevious = pmsiPrevious -> GetNextMsgInfo();
							}
						
							pmsiSelect = FindReceivedMsg("FILEEND", astrMsgCrit, nMsgCritLen, true);
						}
						else {

							strErrorMsg = "During processing file from stream, getting file ends failed due out of order messages. File type: '" + string(pcharFileDesign) + "' messages deleted.";
							boolContinue = false;
						}
					}
				}

				if (!boolContinue) {

					AddLogErrorMsg(strErrorMsg);
					ClearFile(astrMsgCrit, nMsgCritLen);
				}
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During processing file from stream, an exception occurred.", exError.what());
			ClearFile(astrMsgCrit, nMsgCritLen);
		}

		return pmsiStart;
	}

	/* Length of Name and Path of File in Stream
	   Returns: Length of File Name and Path, Else 0 */
	int GetFilePathLength(string astrMsgCrit[], int nMsgCritLen) {
	
		MsgInfo* pmsiSelect = NULL;	/* Selected Message Information */
		int nFilePathLen = 0;		/* File's Path and Name from Message Information */
		
		try {

			/* If the Beginning and End of the Expected Message Exists */
			if ((pmsiSelect = FindReceivedMsg("FILESTART", astrMsgCrit, nMsgCritLen, false)) != NULL) {

				nFilePathLen = pmsiSelect -> GetSegmentLength(2);
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During getting name and path of file from stream, an exception occurred.", exError.what());
		}

		return nFilePathLen;
	}

	/* Checks If File is Fully in Stream
	   Returns: Length of File Contents If Indicating That File is Fully in Stream, Else 0 */
	int CheckFile(string strFileDesign) {
	
		const string MSGFILESTART = "FILESTART",
					 MSGFILEPART = "FILEPART",
					 MSGFILEEND = "FILEEND";
									/* Start, Part, and End File Message Beginnings */
		char* pcharSentFileSize = NULL;
									/* Sent Length of File from Starting Message */
		int nFileLen = 0,			/* Length of File Contents */
			nCurrentLen = 0;		/* Length of Currently Found File Contents */
		MsgInfo* pmsiSelect = pmsiListReceived;		
									/* Selected Message Information */
		bool boolMsgHasNoEnd = false,
									/* Indicator That Current Message Does Not Have an Ending */
			 boolStartFound = false,
			 boolEndFound = false;	/* Indicator That Start and End of File was Found */

		try {

			while (pmsiSelect != NULL) {
			
				if (MsgInfo::FindInString(pmsiSelect -> GetSegment(1), strFileDesign, pmsiSelect -> GetSegmentLength(1) == 0)) {

					if (MsgInfo::FindInString(pmsiSelect -> GetSegment(0), MSGFILESTART, pmsiSelect -> GetSegmentLength(0)) == 0) {
				
						nCurrentLen += pmsiSelect -> GetSegmentLength(4);
						pcharSentFileSize = pmsiSelect -> GetSegment(3);
						boolStartFound = true;
					
						boolMsgHasNoEnd = !pmsiSelect -> HasEnd();
					}
					else if (MsgInfo::FindInString(pmsiSelect -> GetSegment(0), MSGFILEPART, pmsiSelect -> GetSegmentLength(0)) == 0) {
				
						nCurrentLen += pmsiSelect -> GetSegmentLength(4);
					
						boolMsgHasNoEnd = !pmsiSelect -> HasEnd();
					}
					else if (MsgInfo::FindInString(pmsiSelect -> GetSegment(0), MSGFILEEND, pmsiSelect -> GetSegmentLength(0)) == 0) {
				
						nCurrentLen += pmsiSelect -> GetSegmentLength(4);
						boolEndFound = true;
					
						boolMsgHasNoEnd = !pmsiSelect -> HasEnd();
					}
					else if (boolMsgHasNoEnd) {
				
						nCurrentLen += pmsiSelect -> GetSegmentLength(1);
						boolMsgHasNoEnd = !pmsiSelect -> HasEnd();
					}
				}

				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}

			if (boolStartFound && boolEndFound) {
			
				if (pcharSentFileSize != NULL && atoi(pcharSentFileSize) == nCurrentLen) {

					nFileLen = nCurrentLen;
				}
				else if (pcharSentFileSize != NULL) {
				
					AddLogErrorMsg("During checking if file is in stream, invalid file size of " + 
								    IntToString(nCurrentLen) + " out of expected total of " + string(pcharSentFileSize) + 
								   " was found. File should be removed from stream as download failure.");
				}
				else {
				
					AddLogErrorMsg("During checking if file is in stream, invalid file with messing size information was found. File should be removed from stream as download failure.");
				}
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During checking if file is in stream, an exception occurred.", exError.what());
		}
		
		return nFileLen;
	}

	/* Clears File from Stream */
	void ClearFile(string astrParams[], int nMsgCritLen) {

		try {

			while (DequeueReceivedMsg("FILESTART", astrParams, nMsgCritLen) != NULL) {}
			while (DequeueReceivedMsg("FILEPART", astrParams, nMsgCritLen) != NULL) {}
			while (DequeueReceivedMsg("FILEEND", astrParams, nMsgCritLen) != NULL) {}
			while (DequeueStoredMsg("FILESTART", astrParams, nMsgCritLen) != NULL) {}
			while (DequeueStoredMsg("FILEPART", astrParams, nMsgCritLen) != NULL) {}
			while (DequeueStoredMsg("FILEEND", astrParams, nMsgCritLen) != NULL) {}
		}	
		catch (exception& exError) {
		
			AddLogErrorMsg("During removing downloaded file, an exception occurred.", exError.what(), true);
		}
	}

	/* Clears File from Stream */
	void ClearFile(string strFileDesign) {
		
		string astrParams[1] = { strFileDesign };
									/* Parameter for File Design to Delete */

		 ClearFile(astrParams, 1);
	}
	
	/* Adds Error Message to Log */
	void AddLogErrorMsg(string strErrorMsg) {

		AddErrorMsg("LOGERRORMSG", strErrorMsg);
	}	

	/* Adds Error Message to Log with Possible Thread Locking */
	void AddLogErrorMsg(string strErrorMsg, bool boolLockThread) {
	
		if (boolLockThread) {
		
			if (!(hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0)) {

				boolLockThread = false;
				AddLogErrorMsg("During sending error message to log, locking thread failed.");
			}
		}

		AddLogErrorMsg(strErrorMsg);

		if (boolLockThread) {
		
			if (!ReleaseMutex(hmuxLock)) {
			
				AddLogErrorMsg("During sending error message to log, unlocking thread failed.");
			}
		}
	}
	
	/* Adds Error Message to Log with Added Exception Information */
	void AddLogErrorMsg(string strErrorMsg, const char* pcharExceptInfo) {

		string strExceptInfo = "No exception information sent.";
									/* Exception Information */

		if (pcharExceptInfo != NULL) {
		
			strExceptInfo = string(pcharExceptInfo);
		}

		AddErrorMsg("LOGERRORMSG", strErrorMsg + " Exception: " + strExceptInfo);
	}	

	/* Adds Error Message to Log with Exception Information and Possible Thread Locking */
	void AddLogErrorMsg(string strErrorMsg, const char* pcharExceptInfo, bool boolLockThread) {

		string strExceptInfo = "No exception information sent.";
									/* Exception Information */
	
		if (boolLockThread) {
		
			if (!(hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0)) {

				boolLockThread = false;
				AddLogErrorMsg("During sending error message to log, locking thread failed.");
			}
		}

		AddLogErrorMsg(strErrorMsg + " Exception: " + strExceptInfo);

		if (boolLockThread) {
		
			if (!ReleaseMutex(hmuxLock)) {
			
				AddLogErrorMsg("During sending error message to log, unlocking thread failed.");
			}
		}
	}
	
	/* Adds Error Message to Display */
	void AddDisplayErrorMsg(string strErrorMsg) {

		AddErrorMsg("DISPLAYERRORMSG", strErrorMsg);
	}
	
	/* Adds Error Message to Display with Possible Thread Locking */
	void AddDisplayErrorMsg(string strErrorMsg, bool boolLockThread) {
	
		if (boolLockThread) {
		
			if (!(hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0)) {

				boolLockThread = false;
				AddLogErrorMsg("During sending error message to display, locking thread failed.");
			}
		}

		AddErrorMsg("DISPLAYERRORMSG", strErrorMsg);

		if (boolLockThread) {
		
			if (!ReleaseMutex(hmuxLock)) {
			
				AddLogErrorMsg("During sending error message to display, unlocking thread failed.");
			}
		}
	}
	
	/* Adds Error Message to Log and Display
	   Returns: True If Successfully Added, Else False  */
	void AddReplacementErrorMsg(string strLogErrorMsg, string strDisplayErrorMsg) {
	 
		AddErrorMsg("LOGERRORMSG", strLogErrorMsg);
		AddErrorMsg("DISPLAYERRORMSG", strDisplayErrorMsg);
	}

	/* Closes Client Server Connection */
	void Close() {

		MsgInfo* pmsiHolder = NULL;			/* Holder for Deleting Messages */	

		try {

			ClosePeerToPeer();

			if (boolUseSSL) {

				if (psctxAccessor != NULL) {

					SSL_CTX_free(psctxAccessor);
				}

				if (pbioSecureCon != NULL) {

					BIO_free_all(pbioSecureCon);
				}

				if (psctxUDPAccess != NULL) {

					SSL_CTX_free(psctxUDPAccess);
				}

				if (pbioSecureUDPCon != NULL) {

					BIO_free_all(pbioSecureUDPCon);
				}
			}

			if (socServerConn != NULL) {

				shutdown(socServerConn, SD_BOTH);
				closesocket(socServerConn);
			}

			if (socUDPConn != NULL) {

				shutdown(socUDPConn, SD_BOTH);
				closesocket(socUDPConn);
			}
					
			if (pcharPeerToPeerDecryptKey != NULL && pcharPeerToPeerDecryptIV != NULL) {
				
				OPENSSL_cleanse(pcharPeerToPeerDecryptKey, ENCRYPTKEYSIZE);
				OPENSSL_cleanse(pcharPeerToPeerDecryptIV, ENCRYPTIVSIZE);
			}

			boolInGroupSession = false;
			boolGroupSessionHost = false;
			boolRunConnection = false;

			WSACleanup();

			boolNotServerSet = true;
			boolConnected = false;

			while (pmsiListToSend != NULL) {

				pmsiHolder = pmsiListToSend;
				pmsiListToSend = pmsiListToSend -> GetNextMsgInfo();

				if (pmsiHolder != NULL) {

					delete pmsiHolder;
					pmsiHolder = NULL;
				}
			}

			while (pmsiListReceived != NULL) {

				pmsiHolder = pmsiListReceived;
				pmsiListReceived = pmsiListReceived -> GetNextMsgInfo();

				if (pmsiHolder != NULL) {

					delete pmsiHolder;
					pmsiHolder = NULL;
				}
			}

			while (pmsiListStoredToSend != NULL) {

				pmsiHolder = pmsiListStoredToSend;
				pmsiListStoredToSend = pmsiListStoredToSend -> GetNextMsgInfo();

				if (pmsiHolder != NULL) {

					delete pmsiHolder;
					pmsiHolder = NULL;
				}
			}

			while (pmsiListStoredReceived != NULL) {

				pmsiHolder = pmsiListStoredReceived;
				pmsiListStoredReceived = pmsiListStoredReceived -> GetNextMsgInfo();

				if (pmsiHolder != NULL) {

					delete pmsiHolder;
					pmsiHolder = NULL;
				}
			}

			while (pmsiListBackupSent != NULL) {

				pmsiHolder = pmsiListBackupSent;
				pmsiListBackupSent = pmsiListBackupSent -> GetNextMsgInfo();
					
				if (pmsiHolder != NULL) {
					
					delete pmsiHolder;
					pmsiHolder = NULL;
				}
			}

			ReleaseMutex(hmuxLock);
			CloseHandle(hmuxLock);
			hmuxLock = NULL;
		}
		catch (exception& exError) {

			if (hmuxLock != NULL) {

				ReleaseMutex(hmuxLock);
				hmuxLock = NULL;
			}

			AddLogErrorMsg("During client shutdown, an exception occurred.", exError.what());
		}
	}

	/* Closes "Peer To Peer" Server and Client Connections */
	void ClosePeerToPeer() {

		PeerToPeerClientInfo* ppciSelected = NULL;
									/* Selected "Peer To Peer" Client Information */
		
		try {
							
			boolPeerToPeerClient = false;
			boolPeerToPeerServer = false;
				
			/* Close "Peer To Peer" Server Socket, and 
				Go Through Clients Closing Connections */
			if (socPeerToPeer != NULL) {

				shutdown(socPeerToPeer, SD_BOTH);
				closesocket(socPeerToPeer);
			}

			ppciSelected = ppciListClients;

			while (ppciSelected != NULL) {
						
				ppciSelected -> CloseClient();

				ppciSelected = ppciSelected -> GetNextClientInfo();
			}

			while (ppciListClients != NULL) {
						
				ppciSelected = ppciListClients;
				ppciListClients = ppciListClients -> GetNextClientInfo();

				if (ppciSelected != NULL) {
					
					delete ppciSelected;
					ppciSelected = NULL;
				}
			}
		}
		catch (exception& exError) {

			AddLogErrorMsg("During client 'Peer To Peer' shutdown, an exception occurred.", exError.what());
		}
	}

	/* Prepares Char Array to Pass as Pointer to Remotely Called Function */
	int PrepCharArrayOut(char* pcharValue, char* pcharValueOut, int nValueLen, int nOutLen) {

		if (nValueLen > nOutLen) {

			nValueLen = nOutLen;
		}

		memset(pcharValueOut, '\0', nOutLen);
		memcpy(pcharValueOut, pcharValue, nValueLen);

		return nValueLen;
	}

	/* Prepares String to Pass as Pointer to Remotely Called Function */
	int PrepStringOut(string strValue, char* pcharValueOut, int nOutLen) {

		return PrepCharArrayOut((char *)strValue.c_str(), pcharValueOut, strValue.length(), nOutLen);
	}

	string IntToString(int nValue) {
		
		return to_string((long long) nValue);
	}

	string BoolToString(bool boolValue) {
		
		string strValue = "true";	/* String Value for Indicator Value */

		if (!boolValue) {
		
			strValue = "false";
		}

		return strValue;
	}

	bool SetMsgPartIndicator(string strSetMsgPartIndicate) {
	
		bool boolAccepted = false;	/* Indicator That Change was Accepted */

		if (strMsgStartIndicate != strSetMsgPartIndicate &&
			strMsgEndIndicate != strSetMsgPartIndicate &&
			nMsgIndicatorLen == strSetMsgPartIndicate.length()) {
		
			strMsgPartIndicate = strSetMsgPartIndicate;
			boolAccepted = true;
		}
		else {
		
			AddLogErrorMsg("During setting message part indicators, indicator was the same as another or an invalid length. Setting failed.");
		}

		return boolAccepted;
	}

	bool SetMsgStartIndicator(string strSetMsgStartIndicate) {
	
		bool boolAccepted = false;	/* Indicator That Change was Accepted */

		if (strMsgPartIndicate != strSetMsgStartIndicate &&
			strMsgEndIndicate != strSetMsgStartIndicate &&
			nMsgIndicatorLen == strSetMsgStartIndicate.length()) {

			strMsgStartIndicate = strSetMsgStartIndicate;
			boolAccepted = true;
		}
		else {
		
			AddLogErrorMsg("During setting message start indicators, indicator was the same as another or an invalid length. Setting failed.");
		}

		return boolAccepted;
	}

	bool SetMsgEndIndicator(string strSetMsgEndIndicate) {
	
		bool boolAccepted = false;	/* Indicator That Change was Accepted */

		if (strMsgStartIndicate != strSetMsgEndIndicate &&
			strMsgPartIndicate != strSetMsgEndIndicate &&
			nMsgIndicatorLen == strSetMsgEndIndicate.length()) {

			strMsgEndIndicate = strSetMsgEndIndicate;
			boolAccepted = true;
		}
		else {
		
			AddLogErrorMsg("During setting message end indicators, indicator was the same as another or an invalid length. Setting failed.");
		}

		return boolAccepted;
	}

	void SetMsgFiller(char charSetMsgFiller) {

		charMsgFiller = charSetMsgFiller;
	}

	bool SetMsgIndicatorLen(int nSetMsgIndicatorLen) {
	
		bool boolAccepted = false;	/* Indicator That Change was Accepted */

		if (nSetMsgIndicatorLen > 0) {
		
			nMsgIndicatorLen = nSetMsgIndicatorLen;
			boolAccepted = true;
		}
		else {
		
			AddLogErrorMsg("During setting message indicator length, indicator length can not be set to zero. Setting failed.");
		}

		return boolAccepted;
	}

	string GetMsgPartIndicator() {
	
		return strMsgPartIndicate;
	}
	
	string GetMsgStartIndicator() {

		return strMsgStartIndicate;
	}

	string GetMsgEndIndicator() {

		return strMsgEndIndicate;
	}

	char GetMsgFiller() {

		return charMsgFiller;
	}

	bool IsConnected() {
	
		return boolConnected;
	}

	bool HasPeerToPeerServer() {
	
		return boolPeerToPeerServer;
	}

	bool HasPeerToPeerClients() {
	
		return boolPeerToPeerClient;
	}

	/* Indicator That User is in Server Session Group */
	bool IsInSessionGroup() {
		
		return boolInGroupSession;
	}

	/* Indicator That User is Host of Server Session Group */
	bool IsSessionGroupHost() {
		
		return boolGroupSessionHost;
	}

	/* Sets Queue Limit Value */
	void SetQueueLimit(int nNewLimit) {
	
		PeerToPeerClientInfo* ppciSelected = ppciListClients;

		nQueueMsgLimit = nNewLimit;

		while (ppciSelected != NULL) {
						
			ppciSelected -> SetBackupQueueLimit(nNewLimit);

			ppciSelected = ppciSelected -> GetNextClientInfo();
		}
	}

	/* Sets Amount of Time Until Message is Considered Late */
	void SetMsgLateLimit(int nTimeInMillisecs) {
	
		PeerToPeerClientInfo* ppciSelected = ppciListClients;

		nTimeToLateInMillis = nTimeInMillisecs;

		while (ppciSelected != NULL) {
						
			ppciSelected -> SetReceivedMsgLateLimit(nTimeInMillisecs);

			ppciSelected = ppciSelected -> GetNextClientInfo();
		}
	}

	/* Sets Indicator to Drop Messages That are Considered Late */
	void SetDropLateMsgs(bool boolDropLateMsgs) {

		PeerToPeerClientInfo* ppciSelected = ppciListClients;

		boolRemoveLateMsgs = boolDropLateMsgs;

		while (ppciSelected != NULL) {
						
			ppciSelected -> SetReceivedDropLateMsgs(boolDropLateMsgs);

			ppciSelected = ppciSelected -> GetNextClientInfo();
		}
	}

	/* Sets Amount of Time Without Message Activity Before Polling Server */
	void SetActivityCheckTimeLimit(int nTimeInMillis) {

		PeerToPeerClientInfo* ppciSelected = ppciListClients;

		nTimeToCheckActInMillis = nTimeInMillis;

		while (ppciSelected != NULL) {
						
			ppciSelected -> SetReceivedCheckTimeLimit(nTimeInMillis);

			ppciSelected = ppciSelected -> GetNextClientInfo();
		}
	}

	/* Sets SSL Client File Name */
	void SetSSLClientKeyName(char* pcharSetSSLClientKeyName) {

		pcharSSLClientKeyName = pcharSetSSLClientKeyName;
	}

	HANDLE ThreadLocker() {
	
		return hmuxLock;
	}

	char* DebugReceived(int nMsgIndex) {

		return DebugMaskPeerToPeerIP(GetMsg(pmsiListReceived, nMsgIndex), GetMsgLen(pmsiListReceived, nMsgIndex));
	}

	char* DebugToSend(int nMsgIndex) {
		
		return DebugMaskPeerToPeerIP(GetMsg(pmsiListToSend, nMsgIndex), GetMsgLen(pmsiListToSend, nMsgIndex));
	}

	char* DebugReceivedStored(int nMsgIndex) {

		return DebugMaskPeerToPeerIP(GetMsg(pmsiListStoredReceived, nMsgIndex), GetMsgLen(pmsiListStoredReceived, nMsgIndex));
	}

	char* DebugToSendStored(int nMsgIndex) {

		return DebugMaskPeerToPeerIP(GetMsg(pmsiListStoredToSend, nMsgIndex), GetMsgLen(pmsiListStoredToSend, nMsgIndex));
	}

	string DebugBackupSeqs() {
		
		MsgInfo* pmsiSelect = pmsiListBackupSent;
									/* Selected Message Information */
		string strDebugMsg = "";	/* Debug Message */

		while (pmsiSelect != NULL) {

			if (strDebugMsg != "") {

				strDebugMsg += ", ";
			}
				
			strDebugMsg += to_string(pmsiSelect -> GetMetaDataSeqNum());
			pmsiSelect = pmsiSelect -> GetNextMsgInfo();
		}

		if (strDebugMsg == "") {

			strDebugMsg = "No messages in backup.";
		}

		return strDebugMsg;
	}

	int DebugSendQueueCount() {
	
		MsgInfo* pmsiSelect = pmsiListToSend;
		int nToSend = 0;

		if (pmsiSelect != NULL) {
				
			nToSend++;

			while (pmsiSelect -> GetNextMsgInfo() != NULL) {
					
				nToSend++;
				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}
		}

		return nToSend;
	}
	
	int DebugReceivedQueueCount() {
		
		MsgInfo* pmsiSelect = pmsiListReceived;
		int nReceived = 0;

		if (pmsiSelect != NULL) {

			nReceived++;

			while (pmsiSelect -> GetNextMsgInfo() != NULL) {

				nReceived++;
				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}
		}

		return nReceived;
	}

	int DebugReceivedStoredQueueCount() {
		
		MsgInfo* pmsiSelect = pmsiListStoredReceived;
		int nReceived = 0;

		if (pmsiSelect != NULL){

			nReceived++;

			while (pmsiSelect -> GetNextMsgInfo() != NULL) {

				nReceived++;
				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}
		}

		return nReceived;
	}

	int DebugSendStoredQueueCount() {
	
		MsgInfo* pmsiSelect = pmsiListStoredToSend;
		int nToSend = 0;

		if (pmsiSelect != NULL) {
				
			nToSend++;

			while (pmsiSelect -> GetNextMsgInfo() != NULL) {
					
				nToSend++;
				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}
		}

		return nToSend;
	}

	int DebugReceivedMsgLength(int nMsgIndex) {
	
		return GetMsgLen(pmsiListReceived, nMsgIndex);
	}

	int DebugSendMsgLength(int nMsgIndex) {
	
		return GetMsgLen(pmsiListToSend, nMsgIndex);
	}
	
	int DebugReceivedStoredMsgLength(int nMsgIndex) {
	
		return GetMsgLen(pmsiListStoredReceived, nMsgIndex);
	}

	int DebugSendStoredMsgLength(int nMsgIndex) {
	
		return GetMsgLen(pmsiListStoredToSend, nMsgIndex);
	}

	~ClientServerInfo() {

		Close();
	}

	private: 

		char* pcharServerHostNameIP;	/* Server Host Name or IP Address */
		int nServerPort;				/* Port for Accessing Server */
		bool boolUseSSL,				/* Indicator to Use SSL */
			 boolNotServerSet;			/* Indicator That Server Settings Have Not Been Set */
		SOCKET socServerConn,			/* Server Connection */
			   socUDPConn;				/* Alternative Server UDP Connection */
		BIO *pbioSecureCon,				/* SSL Connection */
			*pbioSecureUDPCon;			/* UDP SSL Connection */
		char *pcharSSLClientKeyName;	/* SSL Client Key Name */
		SOCKADDR* psaiUDPInfo;			/* UDP Connection Address Information */	
		int nUDPInfoSize;				/* Size of UDP Connection Address Information */
		SSL_CTX* psctxAccessor,			/* TLS/SSL Accessor for Connector */
			   * psctxUDPAccess;		/* UDP DTLS Accessor for Connector */
		string strMsgPartIndicate,
			   strMsgStartIndicate,
			   strMsgEndIndicate;		/* Indicator for Parts of Messages and
										   Start and End of Message */
		char charMsgFiller;				/* Filler for End Message */
		int nMsgIndicatorLen;			/* Maximum Length of Message Indicator */
		bool boolConnected,				/* Indicator That Client is Connected to Server */
			 boolRunConnection;			/* Indicator to Keep Connection Open */
		HANDLE hmuxLock;				/* Handle to Thread Locking */
		MsgInfo* pmsiListToSend,
			   * pmsiListReceived,		/* List of Information on Messages from Server and
								  		   Message to be Sent to Server */
			   * pmsiListStoredToSend,
			   * pmsiListStoredReceived,
			   * pmsiListBackupSent;	/* List of Information on Messages Stored from Server and
								  		   Message to be Sent to Server to be Added to Queues When They Are Available and
										   Backup of Sent Messages */
		bool boolPeerToPeerClient,		/* Indicator That "Peer To Peer" Client Connections Being Used */
			 boolPeerToPeerServer;		/* Indicator That "Peer To Peer" Server is Up */		
		string strPeerToPeerIP;			/* IP Address for Client "Peer To Peer" Server */
		int nPeerToPeerPort;			/* Default Port for Client "Peer To Peer" Server */
		bool boolPeerToPeerEncrypt;		/* Indicator to Encrypt Client "Peer To Peer" Server Communication */
		char* pcharPeerToPeerDecryptKey,/* Decryption Key for Client "Peer To Peer" Server Communication */
			* pcharPeerToPeerDecryptIV;	/* Decryption IV Block for Client "Peer To Peer" Server Communication */
		SOCKET socPeerToPeer;			/* "Peer To Peer" Server Socket */
		PeerToPeerClientInfo* ppciListClients;
										/* List of Connected "Peer To Peer" Clients */
		int nQueueMsgLimit;				/* Limit of Messages in the Queues */
		bool boolMsgDropped;			/* Indicator That Message was Dropped */
		long long llSendMsgs,			/* Number of Sent Messages */
				  llReceivedMsgs;		/* Number of Received Messages */
		int nTimeToLateInMillis,		/* Amount of Time Til a Message is Late */
			nTimeToCheckActInMillis;	/* Amount of Time Til Check Activity If no Messages Were Received */
		bool boolRemoveLateMsgs,		/* Indicator to Invalidate Late Messages */
			 boolMsgsSync;				/* Indicator That Messages are in Sync with Server */
		long long llLastActOrCheckInMillis;
										/* Last Time Message Activity or Check was Done */
		char* pcharLeftOver;			/* Leftover Peices from the Last Message */
		int nLeftOverLength;			/* Length of Leftover Peices from the Last Message */
		bool boolInGroupSession,		/* Indicator That User is Server Session Group */
			 boolGroupSessionHost;		/* Indicator That User is Host of Server Session Group */

		BIO* ConnectSecure(const string strHostName, const string strPort, SSL_CTX* psctxSetAccess) {

			BIO* pbioCon = NULL;		/* SSL Connection */
			SSL* psslCon = NULL;		/* Underlying SSL Connection */
			X509* px509CertCheck = NULL;/* Certificate for Check */
			bool boolSetup = false;

			SSL_CTX_set_verify(psctxSetAccess, SSL_VERIFY_PEER, NULL);

			SSL_CTX_set_verify_depth(psctxSetAccess, 4);

			SSL_CTX_set_options(psctxSetAccess,
				SSL_OP_NO_SSLv2 | SSL_OP_NO_SSLv3 | SSL_OP_NO_TLSv1 | SSL_OP_NO_TLSv1_1 | SSL_OP_NO_DTLSv1 | SSL_OP_NO_COMPRESSION);

			if (pcharSSLClientKeyName != NULL && SSL_CTX_load_verify_locations(psctxSetAccess, pcharSSLClientKeyName, NULL)) {

				if ((pbioCon = BIO_new_ssl_connect(psctxSetAccess)) != NULL) {

					if (BIO_set_conn_hostname(pbioCon, (char*)(strHostName + ":" + strPort).c_str())) {

						BIO_get_ssl(pbioCon, &psslCon);

						if (psslCon != NULL) {

							if (BIO_do_connect(pbioCon)) {

								if (BIO_do_handshake(pbioCon)) {

									if ((px509CertCheck = SSL_get_peer_certificate(psslCon)) != NULL) {

										X509_free(px509CertCheck);

										if (SSL_get_verify_result(psslCon) == X509_V_OK) {

											boolSetup = true;
										}
										else {

											AddLogErrorMsg("During setting up SSL, confirming SSL connection failed.");
										}
									}
									else {

										AddLogErrorMsg("During setting up SSL, getting certificate for check failed.");
									}
								}
								else {

									AddLogErrorMsg("During setting up SSL, doing handshake failed.");
								}
							}
							else {

								AddLogErrorMsg("During setting up SSL, connection failed.");
							}
						}
						else {

							AddLogErrorMsg("During setting up SSL, getting SSL connection failed.");
						}
					}
					else {

						AddLogErrorMsg("During setting up SSL, setting up connection host name failed.");
					}
				}
				else {

					AddLogErrorMsg("During setting up SSL, setting up connection failed.");
				}
			}
			else {

				AddLogErrorMsg("During setting up SSL, loading client certificate failed or was not set.");
			}

			if (!boolSetup) {

				pbioCon = NULL;
			}

			return pbioCon;
		}

		/* Uses System Command Line to Start Server */
		void StartServer(int nUseServerPort, bool boolUseServerSSLPort) {

			system(("start RevCommServer " + string(pcharServerHostNameIP) + " " + IntToString(nUseServerPort)).c_str());
		}

		/* Start Server for Receiving "Peer To Peer" Connections */
		void StartPeerToPeerServer()  {

			addrinfo aiPeerToPeerInfo;		/* "Peer To Peer" Server Setup Information */
			int nPeerToPeerInfoSize;		/* "Peer To Peer" Server Setup Information */
			addrinfo* paiPeerToPeerSettings = NULL;
											/* Settings for "Peer To Peer" Server */
			u_long ulMode = 1;				/* Mode for Turning on Non-Blocking in Socket */

			try {
			
				/* If Client is Not Already Connected to a Server */
				if (!boolPeerToPeerServer) {

					nPeerToPeerInfoSize = sizeof(aiPeerToPeerInfo);
					ZeroMemory(&aiPeerToPeerInfo, nPeerToPeerInfoSize);

					aiPeerToPeerInfo.ai_family = AF_INET;
					aiPeerToPeerInfo.ai_socktype = SOCK_STREAM;
					aiPeerToPeerInfo.ai_protocol = IPPROTO_TCP;
					aiPeerToPeerInfo.ai_flags = AI_PASSIVE;
				 
					socPeerToPeer = socket(AF_UNSPEC, SOCK_STREAM, IPPROTO_TCP);

					if (socPeerToPeer != INVALID_SOCKET) {

						if (getaddrinfo(strPeerToPeerIP.c_str(), (char *)IntToString(nPeerToPeerPort).c_str(), &aiPeerToPeerInfo, &paiPeerToPeerSettings) == 0) {
						
							if (ioctlsocket(socPeerToPeer, FIONBIO, &ulMode) == NO_ERROR) {
							
								if (bind(socPeerToPeer, paiPeerToPeerSettings -> ai_addr, (int)paiPeerToPeerSettings -> ai_addrlen) == 0) {
						
									if (listen(socPeerToPeer, SOMAXCONN) != SOCKET_ERROR) {
									
										boolPeerToPeerServer = true;
									}
									else {
				
										closesocket(socPeerToPeer);
										AddLogErrorMsg("During setting up for 'Peer to Peer' connections, starting listening mode failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
									}
								}
								else {

									closesocket(socPeerToPeer);
									AddLogErrorMsg("During setting up 'Peer to Peer' server, binding to socket failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
								}
							}
							else {

								closesocket(socPeerToPeer);
								AddLogErrorMsg("During setting up 'Peer to Peer' server, setting socket to non-blocking failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
							}
						}
						else {

							closesocket(socPeerToPeer);
							AddLogErrorMsg("During setting up 'Peer to Peer' server, getting socket address information. WSA error code: " + IntToString(WSAGetLastError()) + ".");
						}
					}
					else {

						AddLogErrorMsg("During setting up 'Peer to Peer' server, setting up socket failed. WSA error code: " + IntToString(WSAGetLastError()) + ".");
					}
				}	
				else {
					
					AddLogErrorMsg("During setting up 'Peer to Peer' server, client not connected to main server or 'Peer to Peer' server already started.");
				}
			}
			catch (exception& exError) {

				AddLogErrorMsg("During setting up 'Peer to Peer' server, an exception occurred.", exError.what());
			}
		}

		/* Checks For and Processes Messages to for Switching to UDP Communciations */
		void CheckUDPSwitch() {

			MsgInfo* pmsiMsg = DequeueReceivedMsg("UDPSWITCHNOTICE");				
											/* Returned Message */
/*			char* pcharPort = pmsiMsg -> GetSegment(1);
											/* UDP Port */
/*				* pcharMsgSize = pmsiMsg -> GetSegment(2);
											/* Maximum Size of UDP Messages */
/* 			SOCKET socUDPConn;				/* Socket Connection ID */
/*			sockaddr_in saiSetUDPInfo;		/* Address for UDP Connection */
/*			string astrParams[1] = { "false" };			
											/* Holder for Information Being Send Back to Server */
/*			u_long ulMode = 1;				/* Mode for Turning on Non-Blocking in Socket */
/*			unsigned int unMaxMsgSize = nUDPMaxSize;
											/* Maximum Size for UDP Messages */
/*			int nUnsignIntSize = sizeof(unsigned int);
											/* Size of Maximum Size for UDP Messages */

			if (pmsiMsg != NULL) {

				char* pcharPort = pmsiMsg -> GetSegment(1),
					* pcharMsgSize = pmsiMsg -> GetSegment(2);
				string astrParams[1] = { "false" };
				u_long ulMode = 1;
				unsigned int unMaxMsgSize = UDPBUFFERSIZE;
				int nUnsignIntSize = sizeof(unsigned int);

				if (pcharPort != NULL && 
					pcharMsgSize != NULL && 
					string(pcharPort) != "" && 
					string(pcharMsgSize) != "") {

					if (boolUseSSL) {

						psctxUDPAccess = SSL_CTX_new(DTLS_client_method());
					
						if ((pbioSecureUDPCon = ConnectSecure(pcharServerHostNameIP,
															  IntToString(nServerPort),
															  psctxUDPAccess)) != NULL) {

							astrParams[0] = "true";
						}
					}
					else {

						socUDPConn = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

						if (socUDPConn != INVALID_SOCKET) {

							sockaddr_in saiSetUDPInfo;
							saiSetUDPInfo.sin_family = AF_INET;
							saiSetUDPInfo.sin_port = htons(atoi(pcharPort));
 							saiSetUDPInfo.sin_addr.S_un.S_addr = inet_addr(pcharServerHostNameIP);

							psaiUDPInfo = (SOCKADDR *)&saiSetUDPInfo;
							nUDPInfoSize = sizeof(saiSetUDPInfo);
						
							if (getsockopt(socUDPConn, 
										   SOL_SOCKET, 
										   SO_MAX_MSG_SIZE, 
										   (char *)&unMaxMsgSize, 
										   &nUnsignIntSize) == 0) {

								if (unMaxMsgSize >= UDPBUFFERSIZE) {

									if (bind(socUDPConn, psaiUDPInfo, nUDPInfoSize) == 0) {

										if (ioctlsocket(socUDPConn, FIONBIO, &ulMode) != 0) {
									
											AddLogErrorMsg("During processing message 'UDPSWITCHNOTICE', setting non-blocking on socket failed. WSA error code: " + 
														   IntToString(WSAGetLastError()) + ".");
										}

										astrParams[0] = "true";
									}
									else {

										AddLogErrorMsg("During processing message 'UDPSWITCHNOTICE', connecting to server failed. WSA error code: " + 
													   IntToString(WSAGetLastError()) + ".");
										socUDPConn = NULL;
									}
								}
								else {

									AddLogErrorMsg("During processing message 'UDPSWITCHNOTICE', UDP socket maximum size of " + IntToString(unMaxMsgSize) + 
												   " smaller than set size of " + IntToString(UDPBUFFERSIZE) + ".");
									socUDPConn = NULL;
								}
							}
							else {

								AddLogErrorMsg("During processing message 'UDPSWITCHNOTICE', UDP socket maximum size could not be collected.");
								socUDPConn = NULL;
							}
						}
						else {

							AddLogErrorMsg("During processing message 'UDPSWITCHNOTICE', setting up setting up UDP socket failed. WSA error code: " + 
										   IntToString(WSAGetLastError()) + ".");
							socUDPConn = NULL;
						}
					}
				}
				else {
				
					AddLogErrorMsg("Processing message 'UDPSWITCHNOTICE' failed due to invalid message. Message: '" + pmsiMsg -> GetMsgString() + "'.");
				}

				AddSendMsg("UDPSWITCHCONFIRM", astrParams, 1, true);

				delete pmsiMsg;
				pmsiMsg = NULL;
			}
		}

		/* Checks For and Processes Messages to Start Client "Peer To Peer" Server */
		void CheckStartPeerToPeerServer() {

			MsgInfo* pmsiMsg = DequeueReceivedMsg("PEERTOPEERSTART");				
											/* Returned Message */
/*			char* pcharIP = pmsiMsg -> GetSegment(1);
											/* IP Address */
/*				* pcharPort = pmsiMsg -> GetSegment(2);
											/* IP Port */
/*				* pcharSetEncryptKey = pmsiMsg -> GetSegment(3),
											/* Holder for Encryption Key */
/*				* pcharSetEncryptIV = pmsiMsg -> GetSegment(4);
											/* Holder for Encryption IV Block */
			bool boolNoEncrypt = true;		/* Indicator to Not Use Encryption */

			if (pmsiMsg != NULL && !boolPeerToPeerServer) {

				char* pcharIP = pmsiMsg -> GetSegment(1),
					* pcharPort = pmsiMsg -> GetSegment(2);

				if (pcharIP != NULL && pcharPort != NULL && string(pcharIP) != "" && string(pcharPort) != "") {

					char* pcharSetEncryptKey = pmsiMsg -> GetSegment(3),
						* pcharSetEncryptIV = pmsiMsg -> GetSegment(4);
					
					if (pcharSetEncryptKey != NULL && pcharSetEncryptIV != NULL) {
						
						StartPeerToPeerServerEncryptedWithKeys(string(pcharIP), atoi(pcharPort), pcharSetEncryptKey, pcharSetEncryptIV);
						boolNoEncrypt = false;
					}					
				}
				else {
				
					AddLogErrorMsg("Processing message 'PEERTOPEERSTART' failed due to invalid message. Message: '" + pmsiMsg -> GetMsgString() + "'.");
				}

				if (boolNoEncrypt) {

					strPeerToPeerIP = string(pcharIP);
					nPeerToPeerPort = atoi(pcharPort);	
					StartPeerToPeerServer();
				}

				delete pmsiMsg;
				pmsiMsg = NULL;
			}
			else if (boolPeerToPeerServer) {
		 
				AddLogErrorMsg("Processing message 'PEERTOPEERSTART' failed due 'Peer To Peer' server connection already alive.");
			}
		}

		/* Checks For and Processes Messages to Connect Client to "Peer To Peer" Server */
		void CheckStartPeerToPeerConnect() {
		
			MsgInfo* pmsiMsg = DequeueReceivedMsg("PEERTOPEERCONNECT");				
											/* Returned Message */
			char* pcharIP = NULL,			/* IP Address for "Peer To Peer" Server to Connect to */
				* pcharPort = NULL,			/* Port for "Peer To Peer" Server to Connect to */
				* pcharEncryptKey = NULL,
				* pcharEncryptIV = NULL;	/* Encryption Key and IV Block */

			if (pmsiMsg != NULL) {

				pcharIP = pmsiMsg -> GetSegment(1);
				pcharPort = pmsiMsg -> GetSegment(2);

				/* If Information was Found, Connect to "Peer To Peer" Server */
				if (pcharIP != NULL && pcharPort != NULL && string(pcharIP) != "" && string(pcharPort) != "") {
					
					pcharEncryptKey = pmsiMsg -> GetSegment(3);
					pcharEncryptIV = pmsiMsg -> GetSegment(4);

					if (pcharEncryptKey != NULL && pcharEncryptIV != NULL) {

						StartPeerToPeerConnect(string(pcharIP), string(pcharPort), pcharEncryptKey, pcharEncryptIV);
					}
					else {

						StartPeerToPeerConnect(string(pcharIP), string(pcharPort));
					}
				}			
				else {
		
					AddLogErrorMsg("Processing message 'PEERTOPEERCONNECT' failed due to invalid message. Message: '" + pmsiMsg -> GetMsgString() + "'.");
				}

				delete pmsiMsg;
				pmsiMsg = NULL;
			}
		}

		/* Checks For and Processes Messages to Disconnect "Peer To Peer" Server and Clients */
		void CheckPeerToPeerDisconnect() {

			MsgInfo* pmsiMsg = DequeueReceivedMsg("PEERTOPEERDISCONNECT");				
											/* Returned Message */

			if (pmsiMsg != NULL) {

				ClosePeerToPeer();

				delete pmsiMsg;
				pmsiMsg = NULL;
			}
		}

		/* Checks Group Join and Left Messages */
		void CheckGroupActivity() {

			MsgInfo* pmsiMsg = DequeueReceivedMsg("GROUPJOINED");				
											/* Returned Group Joined Message */
//			long long llStartMsgNum = pmsiMsg -> GetMetaDataSeqNum(),
//					  llEndMsgNum = 0;		/* Sequence Numbers for Starting and Ending Group Message */

			if (pmsiMsg != NULL) {

				/* Check If Get Start Group Session Message */
				long long llStartMsgNum = pmsiMsg -> GetMetaDataSeqNum();

				delete pmsiMsg;
				pmsiMsg = NULL;

				/* If No End Group Message Found, Then User is a Group Session */
				if ((pmsiMsg = DequeueReceivedMsg("GROUPEXITED")) == NULL) {

					boolInGroupSession = true;
				}
				else {
				
					/* Else If Both Start and End Message Found, Find Which One Came Last, Set Indicator */
					long long llEndMsgNum = pmsiMsg -> GetMetaDataSeqNum();

					delete pmsiMsg;
					pmsiMsg = NULL;

					if (llStartMsgNum < llEndMsgNum) {
					
						boolInGroupSession = false;
						boolGroupSessionHost = false;
					}
					else if (llStartMsgNum > llEndMsgNum) {
					
						boolInGroupSession = true;
					}
					else {

						AddLogErrorMsg("During checking group activity, start and end of group message came in an improbable sequence.");			
					}
				}
			}
			else if ((pmsiMsg = DequeueReceivedMsg("GROUPEXITED")) != NULL) {

				/* Got End of Group Session Message */
				boolInGroupSession = false;
				boolGroupSessionHost = false;
				delete pmsiMsg;
				pmsiMsg = NULL;
			}

			if (boolInGroupSession &&
				(pmsiMsg = DequeueReceivedMsg("HOSTNOTICE")) != NULL) {
			
				/* Got Group Session Host Indicator Message */
				boolGroupSessionHost = true;
				delete pmsiMsg;
				pmsiMsg = NULL;
			}
		}

		void DoPeerToPeerNegotiation() {

			MsgInfo* pmsiMsg = NULL;		/* Returned Message */
//			PeerToPeerClientInfo* ppciSelected = NULL;
											/* Selected "Peer To Peer" Client Information */
//			string strMsg = "",				/* Message String */
//				   strIPAddress = "";		/* IP Address */

			pmsiMsg = DequeueReceivedMsg("PEERTOPEERNEGOTIATE");	

			if (pmsiMsg == NULL) {
			
				PeerToPeerClientInfo* ppciSelected = ppciListClients;

				while (ppciSelected != NULL) {
					
					ppciSelected -> CheckNegotiation();
					ppciSelected = ppciSelected -> GetNextClientInfo();
				}
			}
			else {

				string strMsg = pmsiMsg -> GetMsgString(),
					   strIPAddress = string(MsgInfo::FindSegmentInString(strMsg, 1));

				PeerToPeerClientInfo* ppciSelected = ppciListClients;

				while (ppciSelected != NULL) {
					
					if (ppciSelected -> GetPeerIPAddress() == strIPAddress) {

						ppciSelected -> DoNegotiation();
					}

					ppciSelected = ppciSelected -> GetNextClientInfo();
				}
			}
		}

		/* Adds Send Message
		   Returns: True If Successful, Else False  */
		bool EnqueueSendMsg(string strMsg) {
		
			const int MSGENDINDICATORLEN = strMsgEndIndicate.length();
										/* Length of End Indicator */
			int nMsgLen = strMsg.length(),	
										/* Length of Message */
				nNewMsgStartIndex = -1,
				nNewMsgEndIndex = -1;	/* New Message's Starting and Ending Index */
			bool boolSuccess = false;	/* Indicator That Adding Message was Successful */
			string strMsgSelect = "";	/* Selected Part of Message */
			MsgInfo* pmsiSelect = NULL,	/* Selected Message Information Record */
				   * pmsiStore = NULL;	/* Selected Store Message Information Record */
			int nQueueStoreCount = DebugSendStoredQueueCount(),
				nQueueCount = DebugSendQueueCount(),
										/* Queue Store and Regular Queue Count */
				nSendCount = 0,			/* Number of Sends Enqueued */ 
				nCounter = 0; 			/* Counter for Loop */

			try {
				
				nNewMsgStartIndex = MsgInfo::FindStringStartIndex(strMsg);
				nNewMsgEndIndex = MsgInfo::FindStringEndIndex(strMsg);

				if (nNewMsgStartIndex >= 0 && nNewMsgEndIndex >= 0) {

					nCounter = nNewMsgStartIndex;
					nNewMsgEndIndex += (MSGENDINDICATORLEN - 1);

					pmsiSelect = pmsiListToSend;
					pmsiStore = pmsiListStoredToSend;

					if (pmsiSelect != NULL) {

						while (pmsiSelect -> GetNextMsgInfo() != NULL) {

							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
						}
					}
						
					if (pmsiStore != NULL) {
							
						while (pmsiStore -> GetNextMsgInfo() != NULL) {

							pmsiStore = pmsiStore -> GetNextMsgInfo();
						}
					}
					
					while (nCounter < nMsgLen && nSendCount < nQueueMsgLimit) {

						/* If Message Doesn't Fit in the Buffer, Grab a Buffer Size Piece of It */
						if (nNewMsgEndIndex - nCounter <= BUFFERSIZE) {
							
							strMsgSelect = strMsg.substr(nCounter, (nNewMsgEndIndex - nCounter) + 1);
							nCounter += nNewMsgEndIndex - nCounter;
						}
						else if (nCounter + BUFFERSIZE <= nMsgLen) {
						
							strMsgSelect = strMsg.substr(nCounter, BUFFERSIZE);
							nCounter += BUFFERSIZE;
						}
						else {
							
							strMsgSelect = strMsg.substr(nCounter);
							nCounter = nMsgLen - nCounter;
						}

						if (pmsiListToSend != NULL) {

							/* If the Last Message was the Complete Message, and the New One Has a Starting Indicator
							   Put New Message in its Own, Else Add to Last Message */
							if ((pmsiSelect -> IsComplete() || pmsiSelect -> HasEnd()) && nQueueCount < nQueueMsgLimit) {

								pmsiSelect -> SetNextMsgInfo(new MsgInfo(strMsgSelect));
								pmsiSelect = pmsiSelect -> GetNextMsgInfo();
								boolSuccess = true;
							}
							else if (pmsiListStoredToSend != NULL) {
							
								if (nQueueStoreCount < nQueueMsgLimit) {

									/* Else Send Queue Has an Incomplete Message, If the Last Message in the Sending Store Queue
									   was the Complete Message, and the New One Has a Starting Indicator
									   Put New Message in its Own, Else Add to Last Message */
									if (pmsiStore -> IsComplete() || pmsiStore -> HasEnd()) {

										pmsiStore -> SetNextMsgInfo(new MsgInfo(strMsgSelect));
										pmsiStore = pmsiStore -> GetNextMsgInfo();
										boolSuccess = true;
									}
									else {

										/* Else the Last Message in the Store is Incomplete, Note that the Message was Dropped */
										AddLogErrorMsg("During adding message to send, message being put into store queue failed due to queue have an incomplete message at the end.");
									}
								}
								else {
								
									boolMsgDropped = true;
								}
							}
							else {
							
								/* Else the Sending Store Queue is Empty, and the Message Has a Starting Indicator, Put at Start of Queue */
								pmsiStore = pmsiListStoredToSend = new MsgInfo(strMsgSelect);
								boolSuccess = true;
							}
						}
						else {
							
							/* Else the Sending Queue is Empty, and the Message Has a Starting Indicator, Put at Start of Queue */
							pmsiSelect = pmsiListToSend = new MsgInfo(strMsgSelect);
							boolSuccess = true;
						}

						if (boolSuccess) {

							if (nCounter < nMsgLen) {

								nNewMsgStartIndex = MsgInfo::FindStringStartIndex(strMsg.substr(nCounter));
								nNewMsgEndIndex = MsgInfo::FindStringEndIndex(strMsg.substr(nCounter));

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

					AddLogErrorMsg("During adding message to send, could not add message due invalid format from missing start or end indicator. Message: '" + strMsg + "'.");
					boolSuccess = false;
				}
			}
			catch (exception& exError) {
				
				AddLogErrorMsg("During adding message to send, an exception occurred.", exError.what());
				boolSuccess = false;
			}

			return boolSuccess;
		}

		/* Adds Received Message
		   Returns: True If Successful, Else False  */
		void AddReceivedMsg(char* pcharMsg, int nMsgLen) {
	
			const char MSGFILLERCHAR = charMsgFiller;	
										/* Message Filler Character */
			const int MSGSTARTINDICATORLEN = strMsgStartIndicate.length(),
					  MSGENDINDICATORLEN = strMsgEndIndicate.length();
										/* Length of Message Start and End Indicator */
			int nMsgStartIndex = 0,		
				nMsgEndIndex = 0,		/* Starting and Ending Index of Message */
				nIndexLen = 0;			/* Length Between Index */
			MsgInfo* pmsiSelect = NULL,	/* Selected Message Information Record */
				   * pmsiPrevCheck = NULL,
										/* Previous to Starting Message Information for Last Queue Message */
				   * pmsiCheck = NULL;	/* Checked Message Information for Last Queue Message */
			bool boolNoStart = false,	/* Indicator That Message Has No Start Index */
				 boolCompleted = false; /* Indicator That Selected Message Completed */
			char charSelectText = NULL;	/* Selected Text for Checks */
			int nCounter = 0;			/* Counter for Loop */
						
			try {

				if (pcharMsg != NULL && nMsgLen > 0 && string(pcharMsg) != "") {
									
					pmsiSelect = pmsiListReceived;

					if (pmsiSelect != NULL) {

						while (pmsiSelect -> GetNextMsgInfo() != NULL) {

							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
						}

						if (nLeftOverLength > 0) {
						
							pcharMsg = MsgInfo::AppendString(pcharLeftOver, nLeftOverLength, pcharMsg, nMsgLen);
							nMsgLen += nLeftOverLength;

							pcharLeftOver = NULL;
							nLeftOverLength = 0;
						}
					}
						
					/* Find If Initial Message Exists */
					nMsgStartIndex = MsgInfo::FindStringStartIndex(pcharMsg, nMsgLen);
					nMsgEndIndex = MsgInfo::FindStringEndIndex(pcharMsg, nMsgLen);

					/* If it Has No Start, Get All of Message  */
					if (nMsgStartIndex < 0 || (nMsgEndIndex > 0 && nMsgStartIndex >= nMsgEndIndex)) {

						nMsgStartIndex = 0;
						boolNoStart = true;

						for (nCounter = 0; nCounter < nMsgLen; nCounter++) { 
                                
							charSelectText = *(pcharMsg + nCounter);

							if (charSelectText == MSGFILLERCHAR || charSelectText == '\0') {

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
						if (pmsiSelect != NULL) {

							boolCompleted = pmsiSelect -> HasEnd();

							if ((boolCompleted && !boolNoStart) || (!boolCompleted && boolNoStart)) {

								pmsiSelect -> SetNextMsgInfo(ValidateCreateMsg(pcharMsg + nMsgStartIndex, nIndexLen));
							}
							else if (!boolCompleted && !boolNoStart) {

								RepairReceivedMsg(pcharMsg + nMsgStartIndex, nIndexLen);
							}
							else if (boolCompleted && boolNoStart) {

								if (boolMsgsSync) {

									AddLogErrorMsg("During adding received message to queue, new message arrived without start indicator. Dropped message: '" + string(pcharMsg + nMsgStartIndex) + "'. Last message: '" + pmsiSelect -> GetMsgString() + "'.");								
								}
								else {
								
									RepairReceivedMsg(pcharMsg + nMsgStartIndex, nIndexLen);
								}
							}
							else {

								AddLogErrorMsg("During adding received message to queue, new message got inconsistant information. Dropped message: '" + string(pcharMsg + nMsgStartIndex) + "'. Last message: '" + pmsiSelect -> GetMsgString() + "'.");								
							}
						}
						else if (!boolNoStart) {
				
							/* Else Received Queue is Empty, Add Message */
							pmsiListReceived = ValidateCreateMsg(pcharMsg + nMsgStartIndex, nIndexLen);
						}
						else {
							
							AddLogErrorMsg("During adding received message to queue, new message with no start indicator. Dropped message: '" + string(pcharMsg + nMsgStartIndex) + "'.");								
						}
					} 

					nMsgLen -= (nMsgEndIndex + 1); 

					if (nMsgLen > 0) {
						
						if (nMsgLen > strMsgStartIndicate.length()) {
						
							AddReceivedMsg(pcharMsg + (nMsgEndIndex + 1), nMsgLen);
						}
						else {
						
							pcharLeftOver = new char[nMsgLen];
							nLeftOverLength = PrepCharArrayOut(pcharMsg + (nMsgEndIndex + 1), pcharLeftOver, nMsgLen, nMsgLen);
						}
					}
				}
			}
			catch (exception& exError) {
			
				AddLogErrorMsg("During adding received message to queue, an exception occurred. Message: " + string(pcharMsg), exError.what());
			}
		}

		void AddReceivedMsg(string strMsg) {
		
			AddReceivedMsg((char *)strMsg.c_str(), strMsg.length());
		}

		/* Stores Send Message */
		void StoreSendMsg(string strMsg) {
		
			const int MSGENDINDICATORLEN = strMsgEndIndicate.length();
										/* Length of Message End Indicator */
			int nMsgLen = strMsg.length(),	
										/* Length of Message */
				nNewMsgStartIndex = -1,
				nNewMsgEndIndex = -1,	/* New Message's Starting and Ending Index */
				nIndexLen = 0;			/* Length Between Index */
			string strMsgSelect = "";	/* Selected Part of Message */
			MsgInfo* pmsiSelect = NULL;	/* Selected Message Information Record */
			int nCounter = 0; 			/* Counter for Loop */

			try {

				nNewMsgStartIndex = MsgInfo::FindStringStartIndex(strMsg);
				nNewMsgEndIndex = MsgInfo::FindStringEndIndex(strMsg);

				if (nNewMsgStartIndex >= 0 && nNewMsgEndIndex >= 0) {
						
					nCounter = nNewMsgStartIndex;
					nNewMsgEndIndex += (MSGENDINDICATORLEN - 1);

					pmsiSelect = pmsiListStoredToSend;

					if (pmsiSelect != NULL) {

						while (pmsiSelect -> GetNextMsgInfo() != NULL) {

							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
						}
					}
					
					while (nCounter < nMsgLen) {
						
						if (nCounter > 0) {
						
							nIndexLen = nNewMsgEndIndex - nCounter;
						}
						else {
						
							nIndexLen = nNewMsgEndIndex + 1;
						}

						/* If Message Doesn't Fit in the Buffer, Grab a Buffer Size Piece of It */
						if (nIndexLen <= BUFFERSIZE) {
							
							strMsgSelect = strMsg.substr(nCounter, nIndexLen);
							nCounter += nIndexLen;
						}
						else if (nCounter + BUFFERSIZE <= nMsgLen) {
						
							strMsgSelect = strMsg.substr(nCounter, BUFFERSIZE);
							nCounter += BUFFERSIZE;
						}
						else {
							
							strMsgSelect = strMsg.substr(nCounter);
							nCounter = nMsgLen - nCounter;
						}

						if (pmsiSelect != NULL) {

							pmsiSelect -> SetNextMsgInfo(new MsgInfo(strMsgSelect));
							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
						}
						else {
							
							/* Else the Sending Queue is Empty, and the Message Has a Starting Indicator, Put at Start of Queue */
							pmsiSelect = pmsiListStoredToSend = new MsgInfo(strMsgSelect);
						}

						if (nCounter < nMsgLen) {

							nNewMsgStartIndex = MsgInfo::FindStringStartIndex(strMsg.substr(nCounter));
							nNewMsgEndIndex = MsgInfo::FindStringEndIndex(strMsg.substr(nCounter));

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

					AddLogErrorMsg("During storing message to send, could not add message due invalid format from missing start or end indicator. Message: '" + strMsg + "'.");
				}
			}
			catch (exception& exError) {
				
				AddLogErrorMsg("During storing message to send, an exception occurred.", exError.what());
			}
		}

		/* Stores Received Message for Later Addition to the Queue */
		void StoreReceivedMsg(char* pcharMsg, int nMsgLen) {

			const int MSGSTARTINDICATORLEN = strMsgStartIndicate.length(),
					  MSGENDINDICATORLEN = strMsgEndIndicate.length();
										/* Length of Message Start and End Indicator */
			int nMsgStartIndex = 0,		
				nMsgEndIndex = 0,		/* Starting and Ending Index of Message */
				nIndexLen = 0;			/* Length Between Index */
			MsgInfo* pmsiSelect = NULL,	/* Selected Message Information Record */
				   * pmsiEnd = NULL,	/* Ending Message Information Record of the Last Complete Message */
				   * pmsiStart = NULL;  /* Starting Message Information Record of the Imcomplete Message */
			bool boolHasStart = true,
				 boolHasEnd = true;		/* Indicator That Message Has Start and End Index */
			int nQueueCount = DebugReceivedStoredQueueCount(),
										/* Queue Count */
				nStoreCount = 0;		/* Count of Messages Being Stored */
						
			try {

				if (pcharMsg != NULL) {

					/* Find If Initial Message Exists */
					nMsgStartIndex = MsgInfo::FindStringStartIndex(pcharMsg, nMsgLen);
					nMsgEndIndex = MsgInfo::FindStringEndIndex(pcharMsg, nMsgLen);
					
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

						pmsiSelect = pmsiListStoredReceived;

						while (pmsiSelect != NULL && pmsiSelect -> GetNextMsgInfo() != NULL) {

							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
						}

						/* If Other Messages in Received Queue and the Last Message in Queue is Complete, Add New Message */
						if (pmsiSelect != NULL) {
							
							/* If the Last Message is Not Complete, and a New One Arrives, 
							   Remove the Incomplete Message */
							if (boolHasStart && !pmsiSelect -> HasEnd()) {
							
								if (pmsiSelect != pmsiListStoredReceived) {

									pmsiSelect = pmsiListStoredReceived;
								
									while (pmsiSelect -> GetNextMsgInfo() != NULL) {

										if (pmsiSelect -> HasEnd()) { 

											pmsiEnd = pmsiSelect;
										}

										pmsiSelect = pmsiSelect -> GetNextMsgInfo();
									}

									if (pmsiEnd != NULL) {
									
										pmsiSelect = pmsiEnd;
										pmsiStart = pmsiSelect -> GetNextMsgInfo();
										pmsiSelect -> SetNextMsgInfo(NULL);

										while (pmsiStart != NULL) {
										
											pmsiEnd = pmsiStart;
											pmsiStart = pmsiStart -> GetNextMsgInfo();

											delete pmsiEnd;
											pmsiEnd = NULL;
										}
									}
								}
								else {
								
									delete pmsiListStoredReceived;
									pmsiSelect = pmsiListStoredReceived = NULL;
								}

								boolMsgDropped = true;
							}

							if (pmsiSelect != NULL) {

								if (boolHasStart == pmsiSelect -> HasEnd() && 
									nQueueCount + nStoreCount < nQueueMsgLimit) {
										
									pmsiSelect -> SetNextMsgInfo(new MsgInfo(pcharMsg + nMsgStartIndex, nIndexLen));
									pmsiSelect = pmsiSelect -> GetNextMsgInfo();
									nStoreCount++;
								}
								else {
								
									boolMsgDropped = true;
								}
							}
							else if (boolHasStart) {

								/* Else Queue is Empty After Removing Incomplete Message, Add New Message */
								pmsiSelect = pmsiListStoredReceived = new MsgInfo(pcharMsg + nMsgStartIndex, nIndexLen);
								nStoreCount++;
							}
							else {
							
								boolMsgDropped = true;
							}
						}
						else {
				
							/* Else Queue is Empty, Add Message */
							pmsiSelect = pmsiListStoredReceived = new MsgInfo(pcharMsg + nMsgStartIndex, nIndexLen);
							nStoreCount++;
						}

						if (nMsgLen - (nMsgEndIndex + 1) > 0) {

							StoreReceivedMsg(pcharMsg + (nMsgEndIndex + 1), nMsgLen - (nMsgEndIndex + 1));
						}
					}
				}
			}
			catch (exception& exError) {
			
				AddLogErrorMsg("During storing received message to queue, an exception occurred.", exError.what());
			}
		}

		void StoreReceivedMsg(string strMsg) {
		
			StoreReceivedMsg((char *)strMsg.c_str(), strMsg.length());
		}

		/* Moves Current "To Send" Queue to Backup That are Setup for Tracking, Else Deletes It */
		void MoveSentMsgsToBackup() {
		
			MsgInfo* pmsiSelect = NULL,
									/* Selected Message Information Record */
				   * pmsiForBackup = NULL,
				   * pmsiBackupSelect = NULL;
									/* Messages Selected for Backup and Selected Backup Message Information Record */
			long long llSeqNum = 0; /* Message Sequence Number */
			int nQueueCount = 0;	/* Count of Messages in Backup Queue */
			
			try {

				/* Cycle Through Send Queue for Those Message Being Tracked, and Put in a Holder Queue, Before Removing from Send Queue */
				while (pmsiListToSend != NULL) {

					pmsiSelect = pmsiListToSend;
					pmsiListToSend = pmsiListToSend -> GetNextMsgInfo();

					/* If Message is Being Tracked */ 
					if (pmsiSelect -> GetMetaDataSeqNum() > 0) { 
									
						/* If First to be Check or In Sequence, Put in Holder Queue */
						if (llSeqNum == 0 || llSeqNum + 1 == pmsiSelect -> GetMetaDataSeqNum()) {
					
							llSeqNum = pmsiSelect -> GetMetaDataSeqNum();

							if (pmsiForBackup != NULL) {
						
								pmsiBackupSelect -> SetNextMsgInfo(pmsiSelect);
								pmsiBackupSelect = pmsiBackupSelect -> GetNextMsgInfo();
							}
							else {
						
								pmsiForBackup = pmsiBackupSelect = pmsiSelect;
							}

							pmsiBackupSelect -> SetNextMsgInfo(NULL);
							nQueueCount++;
						}
						else if (llSeqNum < pmsiSelect -> GetMetaDataSeqNum()) {

							/* Else Found a Missing Message from Send Queue, Clear Holder Queue, and Put in Next After Missing */
							AddLogErrorMsg("During storing backup messages, messages missing from send queue. Dropping any messages before missing ones.");

							pmsiForBackup = pmsiBackupSelect;

							while (pmsiForBackup != NULL) {

								pmsiBackupSelect = pmsiForBackup;
								pmsiForBackup = pmsiForBackup -> GetNextMsgInfo();

								delete pmsiBackupSelect;
								pmsiBackupSelect = NULL;
							}
							
							pmsiForBackup = pmsiBackupSelect = pmsiSelect;
							pmsiBackupSelect -> SetNextMsgInfo(NULL);
							nQueueCount = 1;
						}
						else {
						
							/* Else Found a Old Message from Send Queue Already Passed Sequence in Holder, Do Not Add to Holder Queue */
							AddLogErrorMsg("During storing backup messages, old message found in send queue. Dropping message.");

							delete pmsiSelect;
							pmsiSelect = NULL;
						}
					}
					else {

						delete pmsiSelect;
						pmsiSelect = NULL;
					}
				}

				/* If Holder Queue Has Messages */
				if (pmsiForBackup != NULL) {

					if (pmsiListBackupSent != NULL) {

						pmsiSelect = pmsiListBackupSent;

						while (pmsiSelect -> GetNextMsgInfo() != NULL) {
				
							nQueueCount++;
							pmsiSelect = pmsiSelect -> GetNextMsgInfo();	
						}

						/* If Holder Has Next in Sequence, Add to End of Backup Queue */
						if (pmsiSelect -> GetMetaDataSeqNum() + 1 == pmsiForBackup -> GetMetaDataSeqNum()) {
					
							pmsiSelect -> SetNextMsgInfo(pmsiForBackup);
							nQueueCount++;
						}
						else if (pmsiSelect -> GetMetaDataSeqNum() < pmsiForBackup -> GetMetaDataSeqNum()) {
						
							/* Else Found Old Messages from Backup Queue, Clear Backup Queue, and Replace with Holder Queue */
							AddLogErrorMsg("During storing backup messages, messages missing from backup queue. Dropping current messages from backup queue.");

							while (pmsiListBackupSent != NULL) {

								pmsiSelect = pmsiListBackupSent;
								pmsiListBackupSent = pmsiListBackupSent -> GetNextMsgInfo();

								delete pmsiSelect;
								pmsiSelect = NULL;
								nQueueCount--;
							}
							
							pmsiListBackupSent = pmsiForBackup;
						}
						else {
						
							/* Else Found Old Messages Holder Queue, Not Adding to Backup Queue */
							AddLogErrorMsg("During storing backup messages, messages being put into backup queue are old. Backup failed.");
						}
					}
					else {
			
						pmsiListBackupSent = pmsiForBackup;
					}
				}

				/* Remove Oldest Messages from the Backup Queue When Over Limit */
				while (pmsiListBackupSent != NULL && nQueueCount > nQueueMsgLimit) {

					pmsiSelect = pmsiListBackupSent;
					pmsiListBackupSent = pmsiListBackupSent -> GetNextMsgInfo();

					delete pmsiSelect;
					pmsiSelect = NULL;
					nQueueCount--;
				}
			}
			catch (exception& exError) {

				AddLogErrorMsg("During storing backup messages, an exception occurred.", exError.what());
			}
		}

		/* Dequeues and Sends Next Message to Server
		   Returns: Message String, Else Blank String */
		bool SendMsg() {
	
			char* pcharWholeSend = NULL;/* Total Message to be Sent */
			MsgInfo* pmsiSelect = NULL;	/* Selected Message Information */
			pollfd apfdSSLWriteCheck[1];/* Struct Used for Write Ready SSL Socket */
			bool boolNoError = true,	/* Indicator That There was Not a Valid Error */
				 boolUDPDoBackup = true;/* Indicator That UDP Send Finished So Dequeue and Backup Can Occur */
			int nSendLen = 0,			/* Length of Total Message to Send */
				nUDPPartLen = 0,		/* Length of Part of Message Being Sent Using UDP */	
				nSSLWriteCheckResp = 0,	/* Response to SSL Socket Write Check */
				nSendErrorCode = 0,		/* Error Code from Sends */
				nCounter = 0;			/* Counter for Loop */
			string strErrorMsg = "";	/* Error Message */

			try {
					
				if (pmsiListToSend != NULL) {

					pmsiSelect = pmsiListToSend;

					while (pmsiSelect != NULL) {

						if (pcharWholeSend != NULL) {

							pcharWholeSend = MsgInfo::AppendString(pcharWholeSend, nSendLen, pmsiSelect -> GetMsgArray(), pmsiSelect -> Length());
						}
						else {
						
							pcharWholeSend = pmsiSelect -> GetMsgArray();
						}

						nSendLen += pmsiSelect->Length();
						pmsiSelect = pmsiSelect -> GetNextMsgInfo();
					}

					if (socUDPConn == NULL) {
					
						if (boolUseSSL) {

							apfdSSLWriteCheck[0].fd = BIO_get_fd(pbioSecureCon, NULL);
							apfdSSLWriteCheck[0].events = POLLOUT;

							nSSLWriteCheckResp = WSAPoll(apfdSSLWriteCheck, 1, SSLCHECKTIMEOUTINMILLIS);

							if (nSSLWriteCheckResp > 0) {

								if (BIO_write(pbioSecureCon, pcharWholeSend, nSendLen) > 0) {

									MoveSentMsgsToBackup();
								}
								else {

									strErrorMsg = "During dequeuing and sending message, sending messages to server failed using SSL.";
									boolNoError = false;
								}
							}
							else if (nSSLWriteCheckResp == SOCKET_ERROR) {

								strErrorMsg = "During dequeuing and sending message, sending messages to server failed.";
								boolNoError = false;
							}
						}
						else if (send(socServerConn, pcharWholeSend, nSendLen, 0) != SOCKET_ERROR) {

							MoveSentMsgsToBackup();
						}
						else {
						
							strErrorMsg = "During dequeuing and sending message, sending messages to server failed.";
							boolNoError = false;
						}
					}
					else {

						if (boolUseSSL) {

							apfdSSLWriteCheck[0].fd = BIO_get_fd(pbioSecureUDPCon, NULL);
							apfdSSLWriteCheck[0].events = POLLOUT;

							nSSLWriteCheckResp = WSAPoll(apfdSSLWriteCheck, 1, SSLCHECKTIMEOUTINMILLIS);

							if (nSSLWriteCheckResp > 0) {

								if (BIO_write(pbioSecureUDPCon, pcharWholeSend, nSendLen) > 0) {
							
									MoveSentMsgsToBackup();
								} 
								else  {

									strErrorMsg = "During dequeuing and sending message, sending messages using UDP to server failed using SSL.";
									boolNoError = false;
								}
							}
							else if (nSSLWriteCheckResp == SOCKET_ERROR) {
								
								strErrorMsg = "During dequeuing and sending message, socket polling failed during sending messages using UDP to server using SSL.";
								boolNoError = false;
							}
						}
						else {

							/* Else Do UDP Send, Break up Message into Smaller Peices for Send */
							for (nCounter = 0; nCounter < nSendLen && boolNoError && boolUDPDoBackup; nCounter += UDPBUFFERSIZE) {

								if (nSendLen >= UDPBUFFERSIZE) {

									nUDPPartLen = UDPBUFFERSIZE;
								}
								else {

									nUDPPartLen = nSendLen - nCounter;
								}

								if (sendto(socUDPConn,
									pcharWholeSend + nCounter,
									nUDPPartLen,
									0,
									psaiUDPInfo,
									nUDPInfoSize) != nUDPPartLen) {

									boolUDPDoBackup = false;

									strErrorMsg = "During dequeuing and sending message, sending messages to server failed using UDP.";
									boolNoError = false;
								}
							}
						}

						if (boolUDPDoBackup) {
						
							MoveSentMsgsToBackup();
						}
					}

					/* Possible Error or Temporary Socket Blocked */
					if (!boolNoError) {

						nSendErrorCode = WSAGetLastError();

						if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {

							AddLogErrorMsg(strErrorMsg + " Error code : " + IntToString(nSendErrorCode));
						}
						else {

							/* Else Not an Actual Socket Error */
							boolNoError = true;
						}
					}
				}
			}
			catch (exception& exError) {

				AddLogErrorMsg("During dequeuing and sending message, an exception occurred.", exError.what());
				boolNoError = false;
			}

			return boolNoError;
		}

		/* Finds Received Message By Message Type with an Optional Search String and Can Delete Message
		   Return: Message and Removes it from List of Received Messages If Directed to, Else Empty String If Not Found or Fails */
		MsgInfo* FindReceivedMsg(string strMsgTypeName, string astrMsgCrit[], int nMsgCritLen, bool boolDelete) {
		
			const string MSGSTARTINDICATOR = strMsgStartIndicate,
						 MSGPARTINDICATOR = strMsgPartIndicate;
						// SEARCHINMSG = MSGSTARTINDICATOR + strMsgTypeName + MSGPARTINDICATOR + strMsgCritSearchString;
											/* Starting and Part Indicator of Message and Part of Message to Search For */
			const int MSGSTARTLEN = MSGSTARTINDICATOR.length(),
					  MSGPARTLEN = MSGPARTINDICATOR.length();
											/* Length of Start and Part Start of Message Indicator */
					 // SEARCHINMSGLEN = SEARCHINMSG.length();
											/* Length of Part of Message to Search For */
			MsgInfo* pmsiSelect = NULL;		/* Selected Message Information */
			string strMsgCritSearchString = "";		
											/* Segments of the Message Being Checked Against Multiple Message Information */ 
			bool boolCritMatch = true,		/* Indicator That Criteria Matches */
				 boolNotEndFound = true;	/* Indicator That End of Message was Not Found */
			MsgInfo* pmsiPrevious = NULL,
				   * pmsiStart = NULL,
				   * pmsiEnd = NULL;		/* Previous, Start and End of Message Information */
			char* pcharCheckMsg = NULL;		/* Collected Message for Checking Multiple Message Information */
			int nCheckMsgLen = 0,			/* Length of Collected Message */
				nMetaDataIndex = -1;		/* Index in Metadata in Message Being Checked Across Multiple INformation */
			MsgInfo* pmsiCheck = NULL,		/* Message That Check was Started With */
				   * pmsiPrevCheck = NULL;	/* Message That was Previously Check was Started With */
			int nCounter = 0;				/* Counter for Loop */

			try {

				if (strMsgTypeName != "STREAMFILE") {
					
					/* Cycle Through List of Received Messages */
					pmsiPrevious = pmsiSelect = pmsiListReceived;

					for (nCounter = 0; nCounter < nMsgCritLen; nCounter++) {

						strMsgCritSearchString += astrMsgCrit[nCounter];
						
						if (nCounter + 1 < nMsgCritLen) {
							
							strMsgCritSearchString += MSGPARTINDICATOR;
						}
					}

					const string SEARCHINMSG = MSGSTARTINDICATOR + strMsgTypeName + MSGPARTINDICATOR + strMsgCritSearchString;
					const int SEARCHINMSGLEN = SEARCHINMSG.length();

					while (pmsiSelect != NULL && boolNotEndFound) {

						if (pmsiStart == NULL) {

							for (nCounter = 0; nCounter < nMsgCritLen && boolCritMatch; nCounter++) {

								boolCritMatch = strcmp(pmsiSelect->GetSegment(nCounter + 1),(char *)astrMsgCrit[nCounter].c_str()) == 0;
								//boolCritMatch = MsgInfo::FindInString(pmsiSelect -> GetSegment(nCounter + 1), astrMsgCrit[nCounter], pmsiSelect -> GetSegmentLength(nCounter + 1)) == 0;
							}

							if (boolCritMatch && MsgInfo::FindInString(pmsiSelect -> GetSegment(0), strMsgTypeName, pmsiSelect -> GetSegmentLength(0)) == 0) {

								/* The First Part of the Message was Found, Grab the Message Information That it as in,
								   Check for the End of the Message in the Message Information */
								pmsiStart = pmsiSelect; 
		
								if (pmsiSelect -> IsComplete()) {
					
									/* End of Message was Found in the Same Message Information, Grab the Message Information it was in, Set Indicator */
									pmsiEnd = pmsiSelect;
									boolNotEndFound = false;
								}
							}
							else if (pmsiSelect -> HasStart() || nCheckMsgLen > 0) {
							
								if (pmsiSelect -> HasStart()) {
								
									nMetaDataIndex = MsgInfo::FindMetaDataInString(pmsiSelect -> GetMsgArray(), pmsiSelect -> Length());

									if (nMetaDataIndex >= 0) {
										
										nCheckMsgLen = pmsiSelect -> Length() - (pmsiSelect -> GetSegmentLength(0) - (nMetaDataIndex + 1));
										pcharCheckMsg = new char[nCheckMsgLen];

										memset(pcharCheckMsg, GetMsgFiller(), nCheckMsgLen);
										memcpy(pcharCheckMsg, (char *)MSGSTARTINDICATOR.c_str(), MSGSTARTLEN);
										memcpy(pcharCheckMsg, pmsiSelect -> GetSegment(0), nMetaDataIndex);
										memcpy(pcharCheckMsg, 
											   pmsiSelect -> GetMsgArray() + MSGSTARTLEN + pmsiSelect -> GetSegmentLength(0), 
											   pmsiSelect -> Length() - (MSGSTARTLEN + pmsiSelect -> GetSegmentLength(0)));
									}
									else {
									
										pcharCheckMsg = pmsiSelect -> GetMsgArray(); 
										nCheckMsgLen = pmsiSelect -> Length();
									}

									pmsiCheck = pmsiSelect;
									pmsiPrevCheck = pmsiPrevious;
								}
								else {

									pcharCheckMsg = MsgInfo::AppendString(pcharCheckMsg, nCheckMsgLen, pmsiSelect -> GetMsgArray(), pmsiSelect -> Length());
									nCheckMsgLen += pmsiSelect -> Length();

									if (MsgInfo::FindInString(pcharCheckMsg, SEARCHINMSG, nCheckMsgLen) > -1) {
																
										pmsiStart = pmsiCheck; 
										pmsiPrevious = pmsiPrevCheck;

										if (pmsiSelect -> HasEnd()) {

											pmsiEnd = pmsiSelect;
											boolNotEndFound = false;	
										}

										pcharCheckMsg = NULL;
										nCheckMsgLen = 0;
										pmsiCheck = NULL;
										pmsiPrevCheck = NULL;
									}
									else if (nCheckMsgLen >= SEARCHINMSGLEN) {

										pcharCheckMsg = NULL;
										nCheckMsgLen = 0;
										pmsiCheck = NULL;
										pmsiPrevCheck = NULL;
									}
								}
							}

							boolCritMatch = true;
						}
						else if (pmsiSelect -> HasEnd()) {

							pmsiEnd = pmsiSelect;
							boolNotEndFound = false;	
						}

						/* Grab the Currently Selected Message Information, 
							Until the Message Information Containing the Start of the Message is Found, So the Previous One Will Already be Found */
						if (pmsiStart == NULL) {
			
							pmsiPrevious = pmsiSelect;
						}

						pmsiSelect = pmsiSelect -> GetNextMsgInfo();
					}
		
					/* If Beginning and End of Message was Found, Remove the Processed Message Information If Directed To */
					if (pmsiStart != NULL && pmsiEnd != NULL) {
		
						if (boolDelete) {

							/* If It is the First Message Information, Make the One After the End the First, 
								Else Connect the Previous to the Next One */
							if (pmsiListReceived == pmsiStart) {
						
								pmsiListReceived = pmsiEnd -> GetNextMsgInfo();
							}
							else {
						
								pmsiPrevious -> SetNextMsgInfo(pmsiEnd -> GetNextMsgInfo());
							}

							pmsiEnd -> SetNextMsgInfo(NULL);
						}
					}
					else {
					
						pmsiStart = NULL;
					}
				}
				else {
	
					pmsiStart = ProcessFile(astrMsgCrit, nMsgCritLen);
				}
			}
			catch (exception& exError) {

				AddLogErrorMsg("During finding message type, '" + strMsgTypeName + "', an exception occurred.", exError.what());
			}
	
			return pmsiStart;
		}

		/* Returns the Selected Message by Index Number from the Starting Queue Message 
		   Returns Message or Blank String */
		char* GetMsg(MsgInfo* pmsiStart, int nMsgIndex) {

			MsgInfo* pmsiSelect = pmsiStart;	
										/* Selected Message Information Record */
			char* pcharMsg = NULL;		/* Selected Message */
			int nCounter = 0;			/* Counter for Loop */
	
			try {

				if (pmsiSelect != NULL) {

					if (nMsgIndex > 0) {

						while (pmsiSelect -> GetNextMsgInfo() != NULL) {

							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
							nCounter++;

							if (nCounter == nMsgIndex) {

								pcharMsg = pmsiSelect -> GetMsgArray();
								break;
							}
						}
					}
					else {
				
						pcharMsg = pmsiSelect -> GetMsgArray(); 
					}
				}
			}
			catch (exception& exError) {

				AddLogErrorMsg("During getting message, an exception occurred.", exError.what());
			}

			return pcharMsg;
		}

		/* Returns the Length of Selected Message by Index Number from the Starting Queue Message 
		   Returns Length of Message or 0 */
		int GetMsgLen(MsgInfo* pmsiStart, int nMsgIndex) {

			MsgInfo* pmsiSelect = pmsiStart;	
										/* Selected Message Information Record */
			int nMsgLen = 0,			/* Selected Message's Length */
				nCounter = 0;			/* Counter for Loop */
	
			try {

				if (pmsiSelect != NULL) {

					if (nMsgIndex > 0) {

						while (pmsiSelect -> GetNextMsgInfo() != NULL) {

							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
							nCounter++;

							if (nCounter == nMsgIndex) {

								nMsgLen = pmsiSelect -> Length();
								break;
							}
						}
					}
					else {
				
						nMsgLen = pmsiSelect -> Length();
					}
				}
			}
			catch (exception& exError) {

				AddLogErrorMsg("During getting message's length, an exception occurred.", exError.what());
			}

			return nMsgLen;
		}

		/* Send Error Message to Server and Client App */
		void AddErrorMsg(string strErrorMsgType, string strErrorMsg) {
			
			const string MSGSTARTINDICATOR = strMsgStartIndicate,
						 MSGPARTINDICATOR = strMsgPartIndicate,
						 MSGENDINDICATOR = strMsgEndIndicate;
										/* Message Start, Part and End Indicator */
			const int MSGSTARTINDICATORLEN = MSGSTARTINDICATOR.length(),
					  MSGPARTINDICATORLEN = MSGPARTINDICATOR.length(),
					  MSGENDINDICATORLEN = MSGENDINDICATOR.length(),
										/* Length of Message Start, Part and End Indicator */
					  MSGRECEIVEDADDLEN = MSGSTARTINDICATORLEN + MSGPARTINDICATORLEN + MSGENDINDICATORLEN + strErrorMsgType.length(),
										/* Additional Message Information for Receiving */
					  MSGSENDINGADDLEN = MSGSTARTINDICATORLEN + MSGPARTINDICATORLEN + MSGENDINDICATORLEN + 11;
										/* Additional Message Information for Sending */
			int nIndex = 0;

			try {

				if (MsgInfo::FindInString(strErrorMsg, MSGSTARTINDICATOR + "LOGERRORMSG") < 0 && 
					MsgInfo::FindInString(strErrorMsg, MSGSTARTINDICATOR + "DISPLAYERRORMSG") < 0 && 
					MsgInfo::FindInString(strErrorMsg, MSGSTARTINDICATOR + "CLIENTERROR") < 0) {
					
					while ((nIndex = MsgInfo::FindStringStartIndex(strErrorMsg)) >= 0) {
			
						strErrorMsg.replace(nIndex, MSGSTARTINDICATORLEN, " ");
					}

					while ((nIndex = MsgInfo::FindInString(strErrorMsg, MSGPARTINDICATOR)) >= 0) {
			
						strErrorMsg.replace(nIndex, MSGPARTINDICATORLEN, " ");
					}

					while ((nIndex = MsgInfo::FindStringEndIndex(strErrorMsg)) >= 0) {
			
						strErrorMsg.replace(nIndex, MSGENDINDICATORLEN, " ");
					}
			
					if (strErrorMsg.length() + MSGRECEIVEDADDLEN <= BUFFERSIZE) {
			
						StoreReceivedMsg(MSGSTARTINDICATOR + strErrorMsgType + MSGPARTINDICATOR + strErrorMsg + MSGENDINDICATOR);
					}
					else {
					
						StoreReceivedMsg(MSGSTARTINDICATOR + strErrorMsgType + MSGPARTINDICATOR + strErrorMsg.substr(0, BUFFERSIZE - MSGRECEIVEDADDLEN - 4) + "..." + MSGENDINDICATOR);
					}

					/* If it is a Display Error Message, Don't Send to Server */
					if (strErrorMsgType != "DISPLAYERRORMSG") {
			
						if (strErrorMsg.length() + MSGSENDINGADDLEN <= BUFFERSIZE) {
			
							StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + strErrorMsg + MSGENDINDICATOR);
						}
						else {
					
							StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + strErrorMsg.substr(0, BUFFERSIZE - MSGSENDINGADDLEN - 4) + "..." + MSGENDINDICATOR);
						}
					}
				}
				else {

					StoreReceivedMsg(MSGSTARTINDICATOR + strErrorMsgType + MSGPARTINDICATOR + "Sending error message failed." + MSGENDINDICATOR);

					if (strErrorMsgType != "DISPLAYERRORMSG") {

						StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + "Sending error message failed." + MSGENDINDICATOR);
					}
				}
			}
			catch (exception& exError) {

				StoreReceivedMsg(MSGSTARTINDICATOR + strErrorMsgType + MSGPARTINDICATOR + "During sending out errors, exception occurred." + MSGENDINDICATOR);

				if (strErrorMsgType != "DISPLAYERRORMSG") {

					StoreSendMsg(MSGSTARTINDICATOR + "CLIENTERROR" + MSGPARTINDICATOR + "During sending out errors, exception occurred." + MSGENDINDICATOR);
				}
			}
		}

		/* Checks For and Processes Changes to Queue Limit Value */
		void CheckQueueLimitChange() {
		
			MsgInfo* pmsiMsg = DequeueReceivedMsg("SETQUEUELIMIT");				
											/* Returned Message */
			string strNewQueueLimit = "";	/* New Queue Limit */
	
			if (pmsiMsg != NULL) {

				strNewQueueLimit = pmsiMsg -> GetSegment(1);

				if (strNewQueueLimit != "") {

					nQueueMsgLimit = atoi(strNewQueueLimit.c_str());
				}			
				else {
		
					AddLogErrorMsg("Processing message 'SETQUEUELIMIT' failed due to invalid message. Message: '" + pmsiMsg -> GetMsgString() + "'.");
				}

				delete pmsiMsg;
				pmsiMsg = NULL;
			}
		}

		/* Validates If Message is in Sequence or Late, If Valid, Return Message Information, Else Null */
		MsgInfo* ValidateCreateMsg(char* pcharMsg, int nMsgLen) {
		
			MsgInfo* pmsiCheck = NULL,
				   * pmsiSelect = pmsiListReceived;
									/* Check for Valid Message Information, 
									   Starting Message Information Being Checked, and 
									   Previous Message Information to Start,
									   Selected Message Information */
			long long llSeqNum = 0,	/* Message Sequence Number */
					  llTimeInMillis = 0;
									/* Message Time Sent in Milliseconds */
//			string astrParams[1] = { llReceivedMsgs + 1 };
									/* Message Parameters Adding Replay */
			bool boolInvalid = false,	
									/* Indicator That Message is Invalid */
				 boolNoSyncBypass = true;
									/* Indicator to Not Bypass Message Out of Sync Indicator When 
									   Appropriate Message Information Record Arrives 
									   While Out of Sync */

			try {

				if (pcharMsg != NULL && nMsgLen > 0) {

					pmsiCheck = new MsgInfo(pcharMsg, nMsgLen);

					if (pmsiCheck -> HasStart()) {

						llSeqNum = pmsiCheck -> GetMetaDataSeqNum();
						llTimeInMillis = pmsiCheck -> GetMetaDataTime();
					}

					/* If Metadata Exists */ 
					if (llSeqNum > 0) {

						/* If Message is in Sequence */
						if (llReceivedMsgs + 1 == llSeqNum) {
						
							llReceivedMsgs++;

							if (boolMsgsSync) {

								if (boolRemoveLateMsgs && llTimeInMillis > 0) {

									boolInvalid = time(NULL) - llTimeInMillis >= nTimeToLateInMillis;
								}
							}
							else {
							
								/* Appropriate Message Arrived While Out of Sync, Set Bypass to Add to Received Queue */
								boolNoSyncBypass = false;
							}
						}
						else if (llReceivedMsgs + 1 < llSeqNum) {
						
							RepairReceivedMsg(pcharMsg, nMsgLen);
						}
						else {

							boolInvalid = true;
						}
					}
					else if (!boolMsgsSync) {
							
						RepairReceivedMsg(pcharMsg, nMsgLen);
					}

					if (pmsiCheck != NULL && ((!boolMsgsSync && boolNoSyncBypass) || boolInvalid)) {

						delete pmsiCheck;
						pmsiCheck = NULL;
					}
				}
			}
			catch (exception& exError) {
			
				AddLogErrorMsg("During validating message, an exception occurred.", exError.what());
			}

			return pmsiCheck;
		}

		/* Call Next Received Message or Last Incomplete Message After Removing It from Queue */
		void RepairReceivedMsg(char* pcharMsg, int nMsgLen) {
		
			MsgInfo* pmsiNew = NULL,
				   * pmsiStart = NULL,
				   * pmsiPrev = NULL,
				   * pmsiSelect = pmsiListReceived;
			string astrParams[1] = { to_string(llReceivedMsgs + 1) };

			/* Find the Last, Beginning of the Last, and 
			   the Previous to the Beginning of the Last Message Information Record */
			while (pmsiSelect != NULL && pmsiSelect -> GetNextMsgInfo() != NULL) {
				
				if (pmsiSelect -> GetNextMsgInfo() -> HasStart()) {
					
					pmsiPrev = pmsiSelect;
					pmsiStart = pmsiSelect -> GetNextMsgInfo();
				}

				pmsiSelect = pmsiSelect -> GetNextMsgInfo();
			}

			if (pcharMsg != NULL && nMsgLen > 0) {

				pmsiNew = new MsgInfo(pcharMsg, nMsgLen);
			}

			/* If Messages are Currently in Sync, Check Send Message is First Out of Sync */
			if (boolMsgsSync) {

				if (pmsiNew != NULL && 
					llReceivedMsgs < pmsiNew -> GetMetaDataSeqNum()) {
				
					/* If Last Message Not Complete, Remove and Delete its Information Records */
					if (pmsiSelect != NULL && !pmsiSelect -> HasEnd()) {

						astrParams[0] = to_string(llReceivedMsgs);
						llReceivedMsgs--;

						if (pmsiSelect == pmsiListReceived) {
			 						
							pmsiStart = pmsiListReceived;
							pmsiListReceived = NULL;
						}

						if (pmsiPrev != NULL) {
								
							pmsiPrev -> SetNextMsgInfo(NULL);
						}

						while (pmsiStart != NULL) {
				
							pmsiSelect = pmsiStart;
							pmsiStart = pmsiStart -> GetNextMsgInfo();
									
							delete pmsiSelect;
							pmsiSelect = NULL;
						}
					}
					
					/* Send to Get Replay to Get Back Into Sync, While Switching to Out of Sync Mode */
					if (!AddSendMsg("MSGREPLAY", astrParams, 1, false)) {
							
						AddLogErrorMsg("During resequencing of received messages, sending message 'MSGREPLAY' for message sequence number, " + astrParams[0] + ", failed.");
					}

					boolMsgsSync = false;
				}
			}
			else {
			
				/* Dequeue Any Stored Message into the Main Queue That Are Next to be In Sync, Register it */
				pmsiPrev = pmsiSelect;

				while ((pmsiSelect = DequeueStoredMsg(llReceivedMsgs + 1)) != NULL) {
							
					pmsiPrev -> SetNextMsgInfo(pmsiSelect);

					while (pmsiPrev -> GetNextMsgInfo() != NULL) {
									
						pmsiPrev = pmsiPrev -> GetNextMsgInfo();
					}

					llReceivedMsgs++;
				}

				/* If No More Stored Message Waiting to be Put in Queue to Go Back in Sync, Change Mode */
				if (pmsiListStoredReceived != NULL) {
								
					pmsiSelect = pmsiListStoredReceived;

					while (pmsiSelect != NULL) {
							
						/* If Message Found Where Still Out of Sync, Exit, and Keep in Out of Sync Mode */
						if (llReceivedMsgs < pmsiSelect -> GetMetaDataSeqNum()) {

							pmsiSelect = NULL;
							boolMsgsSync = false;
						}
						else {
									
							/* Else Assume the End, and No Out of Sync Message Found, Go Back to In Sync Mode */
							boolMsgsSync = true;
							pmsiSelect = pmsiSelect -> GetNextMsgInfo();
						}
					}
				}
				else {
				
					boolMsgsSync = true;
				}
			}

			/* If In Out of Sync Mode, Store Message to One Main Queue When Back in Sync */
			if (!boolMsgsSync && 
				pmsiNew != NULL &&
				(pmsiNew -> GetMetaDataSeqNum() <= 0 ||
				 llReceivedMsgs < pmsiNew->GetMetaDataSeqNum())) {

				StoreReceivedMsg(pcharMsg, nMsgLen);
			}
		}

		/* Check If Message Activity Has Occurred Within Time Limit Else Send Check */
		void CheckLastActivity() {
		
//			string astrParams[1] = { llReceivedMsgs + 1 };
									/* Message Parameters Adding to Message Check */

			if (time(NULL) - llLastActOrCheckInMillis >= nTimeToCheckActInMillis) {
						
				string astrParams[1] = { to_string(llReceivedMsgs + 1) };

				if (!AddSendMsg("MSGCHECK", astrParams, 1, false)) {
							
					AddLogErrorMsg("Sending message 'MSGCHECK' for message activity check, " + astrParams[0] + ", failed.");
				}

				llLastActOrCheckInMillis = time(NULL);
			}
		}

		/* Removes IP Address from Debug Messages */
		char* DebugMaskPeerToPeerIP(char* pcharMsg, int nMsgLen) {
		
			int nIPLength = 0;		/* Length of Segment to Remove IP Addresses */
			//nIPIndex = MsgInfo::FindInString(pcharMsg, MsgInfo::FindSegmentInString(strMsg, 1), nMsgLen, nIPLength),
									/* Index of Segment to Remove IP Addresses */
			//nCounter = 0;			/* Counter for Loop */
		
			if (pcharMsg != NULL) {

				if (MsgInfo::FindInString(pcharMsg, "PEERTOPEER", nMsgLen) >= 0 && 
					(nIPLength = MsgInfo::FindSegmentLengthInString(pcharMsg, 1, nMsgLen)) > 0) {

					int nIPIndex = MsgInfo::FindInString(pcharMsg, MsgInfo::FindSegmentInString(pcharMsg, 1, nMsgLen), nMsgLen, nIPLength),
						nCounter = 0;

					if (nIPIndex >= 0) {
				
						for (nCounter = nIPIndex; nCounter < nIPLength; nCounter++) {
					
							*(pcharMsg + nCounter) = '#';
						}
					}
				}
			}

			return pcharMsg;
		}

} csiOpInfo;

MsgInfo::MsgInfo(char* pcharSetMsg, int nSetMsgLen) {
	
	try {

		SetupInfo(pcharSetMsg, nSetMsgLen);
	}
	catch (exception& exError) { 

		throw exError;
	}
}

MsgInfo::MsgInfo(string strSetMsg) {
	
	try {

		SetupInfo((char *)strSetMsg.c_str(), strSetMsg.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

void MsgInfo::SetupInfo(char* pcharSetMsg, int nSetMsgLen) {

	int nMsgLength = 0;				/* Buffer Size */
	
	try {

		boolHasStart = false;
		boolHasEnd = false;
	
		if (nSetMsgLen <= csiOpInfo.BUFFERSIZE) {
	
			nMsgLength = csiOpInfo.BUFFERSIZE;
		}
		else {

			nMsgLength = nSetMsgLen;
		}

		pcharMsg = new char[nMsgLength + 1];
		memset(pcharMsg, csiOpInfo.GetMsgFiller(), nMsgLength + 1);
		memcpy(pcharMsg, pcharSetMsg, nSetMsgLen);
		pcharMsg[nSetMsgLen] = '\0';

		nLength = nMsgLength;
	
		nStartIndex = MsgInfo::FindStringStartIndex(pcharSetMsg, nSetMsgLen);
		nEndIndex = MsgInfo::FindStringEndIndex(pcharSetMsg, nSetMsgLen);

		if (nEndIndex >= 0 && nStartIndex >= 0 && nStartIndex < nEndIndex) {
			
			nEndIndex += (csiOpInfo.GetMsgEndIndicator().length() - 1);

			nLength = (nEndIndex - nStartIndex) + 1;

			boolHasStart = true;
			boolHasEnd = true;
		}
		else if (nEndIndex >= 0) {
		
			nEndIndex += (csiOpInfo.GetMsgEndIndicator().length() - 1);
			nStartIndex = 0;
			nLength = nEndIndex + 1;
			boolHasEnd = true;
		}
		else if (nStartIndex >= 0) {
		
			nEndIndex = nSetMsgLen - 1;

			if (nStartIndex > 0) {

				nLength = nSetMsgLen - (nStartIndex + 1);
			}
			else {
			
				nLength = nSetMsgLen;
			}

			boolHasStart = true;
		}
		else {
		
			nEndIndex = nSetMsgLen - 1;
			nStartIndex = 0;
			nLength = nSetMsgLen;
		}

		pmsiNextInfo = NULL;
	}
	catch (exception& exError) { 

		throw exError;
	}
}

int MsgInfo::FindStringStartIndex(char* pcharCheck, int nCheckLen) {

	string strMsgStartIndicate = csiOpInfo.GetMsgStartIndicator();
									/* Starting Indicator */
	int nFoundIndex = -1;			/* Found Starting Index */

	try {
	
		nFoundIndex = MsgInfo::FindInString(pcharCheck, (char *)strMsgStartIndicate.c_str(), nCheckLen, strMsgStartIndicate.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return nFoundIndex;
}

int MsgInfo::FindStringStartIndex(string strCheck) {

	try {

		return MsgInfo::FindStringStartIndex((char *)strCheck.c_str(), strCheck.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

int MsgInfo::FindStringEndIndex(char* pcharCheck, int nCheckLen) {

	string strMsgEndIndicate = csiOpInfo.GetMsgEndIndicator();
									/* Ending Indicator */
	int nFoundIndex = -1;			/* Found Ending Index */
	
	try {
		
		nFoundIndex = MsgInfo::FindInString(pcharCheck, (char *)strMsgEndIndicate.c_str(), nCheckLen, strMsgEndIndicate.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return nFoundIndex;
}

int MsgInfo::FindStringEndIndex(string strCheck) {

	try {

		return MsgInfo::FindStringEndIndex((char *)strCheck.c_str(), strCheck.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

/* Finds Starting Index of Matching "Find" String Within "Check" String, Else Returns -1 */
int MsgInfo::FindInString(char* pcharCheck, char* pcharFind, int nCheckLen, int nFindLen) {

	bool boolNotFound = true;		/* Indicator That String Was Not Found */
	int nFoundIndex = -1,			/* Starting Index */
		nCounter = 0;				/* Counter for Loop */

	try {

		if (nCheckLen >= nFindLen) {

			for (nCounter = 0; nCounter < nCheckLen && boolNotFound; nCounter++) {
			
				/* If the First Character of "Find" String Found, 
				   Set Index of Found String to Currently Selected Index of "Check" String */
				if (nFoundIndex >= 0) {

					if (nFindLen >= (nCounter - nFoundIndex) + 1) {

						if (*(pcharCheck + nCounter) != *(pcharFind + (nCounter - nFoundIndex))) {
					
							nCounter = nFoundIndex;
							nFoundIndex = -1;
						}
						else if (nFindLen <= (nCounter - nFoundIndex) + 1) {

							boolNotFound = false;
						}
					}
					else {

						boolNotFound = false;
					}
				}
				else if ((nFindLen - 1) <= nCheckLen - (nCounter + 1)) {

					if (*(pcharCheck + nCounter) == *pcharFind &&
						*(pcharCheck + nCounter + (nFindLen - 1)) == *(pcharFind + (nFindLen - 1))) {
				
						/* Else If the First Character of "Find" String Found, 
						   Set Index of Found String to Currently Selected Index of "Check" String */
						nFoundIndex = nCounter;
					}
				}
			}

			if (boolNotFound) {
			
				nFoundIndex = -1;
			}
		}
	}
	catch (exception& exError) { 

		throw exError;
	}

	return nFoundIndex;
}

int MsgInfo::FindInString(char* pcharCheck, string strFind, int nCheckLen) {

	try {

		return MsgInfo::FindInString(pcharCheck, (char *)strFind.c_str(), nCheckLen, strFind.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

int MsgInfo::FindInString(string strCheck, string strFind) {

	try {

		return MsgInfo::FindInString((char *)strCheck.c_str(), (char *)strFind.c_str(), strCheck.length(), strFind.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

char* MsgInfo::FindSegmentInString(char* pcharCheck, int nSegNum, int nCheckLen) {

	const string MSGPARTINDICATOR = csiOpInfo.GetMsgPartIndicator();
									/* Message Part Indicator */
	const int MSGPARTINDICATORLEN = MSGPARTINDICATOR.length(),
									/* Length of Message Part Indicator */
			  MSGENDINDICATORLEN = csiOpInfo.GetMsgEndIndicator().length();
									/* Length of Message End Indicator */
	int nPartStartIndex = 0,		/* Index of the Start of the Selected Part */
		nPartLen = 0,				/* Length of the Selected Part */
		nCoveredIndex = 0,			/* Last Index of Covered Message */
		nCounter = 0;				/* Counter for Loop */
	char* pcharSegment = NULL;		/* Found Message Segment */
	
	try {

		nPartStartIndex = MsgInfo::FindInString(pcharCheck, MSGPARTINDICATOR, nCheckLen);
		nPartLen = MsgInfo::FindSegmentLengthInString(pcharCheck, nSegNum, nCheckLen);
	
		pcharSegment = new char[nPartLen + 1];		
		memset(pcharSegment, csiOpInfo.GetMsgFiller(), nPartLen + 1);

		if (nPartLen > 0) {

			if (nPartStartIndex >= 0) {
	
				nCoveredIndex = nPartStartIndex + MSGPARTINDICATORLEN;

				/* If Any Segment Outside of Start, Else If Starting Message Type Segment, Else Return Nothing */
				if (nSegNum > 0) {

					for (nCounter = 1; nCounter < nSegNum && nPartStartIndex >= 0; nCounter++) {
	
						nPartStartIndex = MsgInfo::FindInString(pcharCheck + nCoveredIndex, MSGPARTINDICATOR, nCheckLen - (nCoveredIndex + 1));

						if (nPartStartIndex >= 0) {
					
							nCoveredIndex += nPartStartIndex + MSGPARTINDICATORLEN;
						}
					}

					memcpy(pcharSegment, pcharCheck + nCoveredIndex, nPartLen);
				}
			}
			else {

				memcpy(pcharSegment, pcharCheck, nPartLen);
			}

			if (nSegNum == 0) {
	
				nPartStartIndex = MsgInfo::FindInString(pcharCheck, csiOpInfo.GetMsgStartIndicator(), nCheckLen);

				if (nPartStartIndex >= 0) {
			
					nCoveredIndex = nPartStartIndex + csiOpInfo.GetMsgStartIndicator().length();
				}

				memcpy(pcharSegment, pcharCheck + nCoveredIndex, nPartLen);
			}
		}

		*(pcharSegment + nPartLen) = '\0';
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return pcharSegment;
}

char* MsgInfo::FindSegmentInString(string strCheck, int nSegNum) {

	try {

		return MsgInfo::FindSegmentInString((char *)strCheck.c_str(), nSegNum, strCheck.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

int MsgInfo::FindSegmentLengthInString(char* pcharCheck, int nSegNum, int nCheckLen) {

	const string MSGPARTINDICATOR = csiOpInfo.GetMsgPartIndicator();
									/* Message Part Indicator */
	const int MSGPARTINDICATORLEN = MSGPARTINDICATOR.length(),
									/* Length of Message Part Indicator */
			  MSGENDINDICATORLEN = csiOpInfo.GetMsgEndIndicator().length();
									/* Length of Message End Indicator */
	int nSegmentLen = 0,			/* Length of Message Segment */
		nNextStartIndex = 0,		/* Index of the Start of the Next Part or End */
		nRemainingLen = nCheckLen,	/* Remaining of the Message Length */
		nCoveredIndex = 0,			/* Last Index of Covered Message */
		nCounter = 0;				/* Counter for Loop */

	try {

		if (pcharCheck != NULL) {

			if (nSegNum > 0) {

				for (nCounter = 0; nCounter < nSegNum && nNextStartIndex >= 0; nCounter++) {

					nNextStartIndex = MsgInfo::FindInString(pcharCheck + nCoveredIndex, MSGPARTINDICATOR, nRemainingLen);

					if (nNextStartIndex >= 0) {

						nCoveredIndex += nNextStartIndex + MSGPARTINDICATORLEN;
						nRemainingLen = nCheckLen - nCoveredIndex;
					}
				}

				if (nNextStartIndex >= 0) {
		
					nNextStartIndex = MsgInfo::FindInString(pcharCheck + nCoveredIndex, MSGPARTINDICATOR, nRemainingLen);

					if (nNextStartIndex >= 0) {
			
						nSegmentLen = nNextStartIndex;
					}
				}
			
				if (nNextStartIndex < 0) {

					nNextStartIndex = MsgInfo::FindInString(pcharCheck + nCoveredIndex, csiOpInfo.GetMsgEndIndicator(), nRemainingLen);

					if (nNextStartIndex >= 0) {
				
						nSegmentLen = nNextStartIndex;
					}
					else {
				
						nSegmentLen = nCheckLen - nCoveredIndex;
					}
				}
			}
			else if (nSegNum == 0) {
	
				nNextStartIndex = MsgInfo::FindInString(pcharCheck, csiOpInfo.GetMsgStartIndicator(), nCheckLen);

				if (nNextStartIndex >= 0) {
		
					nCoveredIndex = nNextStartIndex + csiOpInfo.GetMsgStartIndicator().length();
					nNextStartIndex = MsgInfo::FindInString(pcharCheck + nCoveredIndex, MSGPARTINDICATOR, nCheckLen - nCoveredIndex);
			
					if (nNextStartIndex >= 0) {

						nSegmentLen = nNextStartIndex;
					}
					else {

						nNextStartIndex = MsgInfo::FindInString(pcharCheck + nCoveredIndex, csiOpInfo.GetMsgEndIndicator(), nCheckLen - nCoveredIndex);
					
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
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return nSegmentLen;
}

int MsgInfo::FindSegmentLengthInString(string strCheck, int nSegNum) {

	try {

		return MsgInfo::FindSegmentLengthInString((char *)strCheck.c_str(), nSegNum, strCheck.length());
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

char* MsgInfo::AppendString(char* pcharMainMsg, int nMainLen, char* pcharAppendString, int nAppendLen, bool boolAddEnd) {
	
	int nMsgLength = nMainLen + nAppendLen;				
									/* Buffer Size */
	char* pcharNewMsg = new char[nMsgLength + 1];
									/* Holder for New Message */

	try {
		
		memset(pcharNewMsg, csiOpInfo.GetMsgFiller(), nMsgLength + 1);

		if (boolAddEnd) {
	
			pcharNewMsg[nMainLen + nAppendLen] = '\0';
		}

		if (nMainLen > 0) {

			memcpy(pcharNewMsg, pcharMainMsg, nMainLen);

			if (nAppendLen > 0) {

				memcpy(pcharNewMsg + nMainLen, pcharAppendString, nAppendLen);
			}
		}
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return pcharNewMsg;
}

char* MsgInfo::AppendString(string strMainMsg, string strAppendString) {

	try {

		return AppendString((char *)strMainMsg.c_str(), strMainMsg.length(), (char *)strAppendString.c_str(), strAppendString.length(), false);
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

char* MsgInfo::AppendString(char* pcharMainMsg, int nMainLen, char* pcharAppendString, int nAppendLen) { 

	try {

		return AppendString(pcharMainMsg, nMainLen, pcharAppendString, nAppendLen, true);
	}
	catch (exception& exError) { 
	
		throw exError;
	}
}

/* Gets Message Metadata Sequence Number */
long long MsgInfo::GetMetaDataSeqNum() {

	return MsgInfo::GetMetaDataSeqNum(pcharMsg, nLength);
}

long long MsgInfo::GetMetaDataSeqNum(char* pcharCheck, int nCheckLen) {
	
	char* pstrMsgType = NULL;		/* Message Type Name */
	int nMsgTypeLen = 0,
		nNumIndex = 0,
		nNumLen = 0;
	long long llSeqNum = 0;			/* Length of Message Type and Possible Metadata and 
									   Index of the Message Sequence Number and Length and Sequence Number */

	try {
		
		/* If Metadata Exists */
		if ((pstrMsgType = MsgInfo::FindSegmentInString(pcharCheck, 0, nCheckLen)) != NULL && 
			(nMsgTypeLen = MsgInfo::FindSegmentLengthInString(pcharCheck, 0, nCheckLen)) > 0 && 
			(nNumIndex = MsgInfo::FindInString(pstrMsgType, string("-"), nMsgTypeLen) + 1) > 0) {

			/* Check If There is More Metadata, If Not, Get the Rest of the String as the Sequence Number */ 
			if (nMsgTypeLen - (nNumIndex + 1) > 0 && 
				(nNumLen = MsgInfo::FindInString(pstrMsgType + nNumIndex, string("-"), nMsgTypeLen - (nNumIndex + 1))) <= 0) {
			
				nNumLen = nMsgTypeLen - nNumIndex;
			}

			llSeqNum = atoi(string(pstrMsgType).substr(nNumIndex, nNumLen).c_str());
		}
	}
	catch (exception& exError) {
			
		throw exError;
	}

	return llSeqNum;
}

long long MsgInfo::GetMetaDataSeqNum(string strCheck) {

	return MsgInfo::GetMetaDataSeqNum((char *)strCheck.c_str(), strCheck.length());
}

/* Gets Message Metadata Time Sent */
long long MsgInfo::GetMetaDataTime() {

	return MsgInfo::GetMetaDataTime(pcharMsg, nLength);
}

long long MsgInfo::GetMetaDataTime(char* pcharCheck, int nCheckLen) {
	
	char* pstrMsgType = NULL;
							/* Message Type Name */
	int nMsgTypeLen = 0,
		nNumIndex = 0,
		nTimeIndex = 0,
		nTimeLen = 0;		/* Length of Message Type and Possible Metadata and 
							   Index of the Message Sequence Number, Time,  and Time Length */
	long long llTimeInMillis = 0;
							/* Time Sent in Milliseconds */

	try {

		/* If Metadata Exists, Get to Time Index */
		if ((pstrMsgType = MsgInfo::FindSegmentInString(pcharCheck, 0, nCheckLen)) != NULL && 
			(nMsgTypeLen = MsgInfo::FindSegmentLengthInString(pcharCheck, 0, nCheckLen)) > 0 && 
			(nNumIndex = MsgInfo::FindInString(pstrMsgType, string("-"), nMsgTypeLen) + 1) > 0 && 
			nMsgTypeLen - (nNumIndex + 1) > 0 && 
			(nTimeIndex = MsgInfo::FindInString(pstrMsgType + nNumIndex, string("-"), nMsgTypeLen - (nNumIndex + 1))) > 0) {
			
			/* Check If There is More Metadata, If Not, Get the Rest of the String as the Time */ 
			if (nMsgTypeLen - ((nNumIndex + 1) + (nTimeIndex + 1)) > 0 && 
				(nTimeLen = MsgInfo::FindInString(pstrMsgType + nTimeIndex + (nNumIndex + 1), string("-"), nMsgTypeLen - ((nNumIndex + 1) + (nTimeIndex + 1)))) <= 0) {
			
				nTimeLen = nMsgTypeLen - nTimeIndex;
			}

			llTimeInMillis = atoi(string(pstrMsgType).substr(nTimeIndex + (nNumIndex + 1), nTimeLen).c_str());
		}
	}
	catch (exception& exError) {
			
		throw exError;
	}

	return llTimeInMillis;
}

long long MsgInfo::GetMetaDataTime(string strCheck) {

	return MsgInfo::GetMetaDataTime((char *)strCheck.c_str(), strCheck.length());
}

/* Find If Metadata is in String, Returns Index, Else -1 If Not Found */
int MsgInfo::FindMetaDataInString(char* pcharMsg, int nMsgLen) {
	
	char* pstrMsgType = NULL;
							/* Message Type Name */
	int nMsgTypeLen = 0,
		nNumIndex = -1;		/* Length of Message Type and Possible Metadata and 
							   Index of the Message Sequence Number */

	try {

		/* If Metadata Exists, Get to Time Index */
		if ((pstrMsgType = MsgInfo::FindSegmentInString(pcharMsg, 0, nMsgLen)) != NULL && 
			(nMsgTypeLen = MsgInfo::FindSegmentLengthInString(pcharMsg, 0, nMsgLen)) > 0 && 
			MsgInfo::FindInString(pstrMsgType, string("-"), nMsgTypeLen) > 0) {

			nNumIndex = MsgInfo::FindInString(pcharMsg, string("-"), nMsgLen);
		}
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return nNumIndex;
}

char* MsgInfo::GetSegment(int nSegNum) {

	char* pcharSegment = NULL;		/* Found Message Segment */

	try {

		pcharSegment = MsgInfo::FindSegmentInString(pcharMsg + nStartIndex, nSegNum, nLength);
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return pcharSegment;
}

int MsgInfo::GetSegmentLength(int nSegNum) {
	
	int nSegmentLen = 0;			/* Length of Message Segment */

	try {

		nSegmentLen = MsgInfo::FindSegmentLengthInString(pcharMsg + nStartIndex, nSegNum, nLength);
	}
	catch (exception& exError) { 
	
		throw exError;
	}

	return nSegmentLen;
}

char* MsgInfo::GetMsgArray() {

	return pcharMsg + nStartIndex;
}

string MsgInfo::GetMsgString() {

	string strMsg = "";

	if (pcharMsg != NULL) {
	
		strMsg = string(pcharMsg + nStartIndex);
	}

	return strMsg;
}

void MsgInfo::SetNextMsgInfo(MsgInfo* pmsiSetNextInfo) {

	pmsiNextInfo = pmsiSetNextInfo;
}

MsgInfo* MsgInfo::GetNextMsgInfo() {

	return pmsiNextInfo;
}

int MsgInfo::Length() {

	return nLength;
}

/* If Message Has Been Completed */
bool MsgInfo::IsComplete() {

	return boolHasStart && boolHasEnd;
}

/* If Start of Message Indicator in Message */
bool MsgInfo::HasStart() {

	return boolHasStart;
}

/* If End of Message Indicator in Message */
bool MsgInfo::HasEnd() {

	return boolHasEnd;
}

int MsgInfo::GetStartIndex() {

	return nStartIndex;
}

int MsgInfo::GetEndIndex() {
	
	return nEndIndex;
}

PeerToPeerClientInfo::PeerToPeerClientInfo(SOCKET socSetClient, 
										   string strSetHomeIPAddress,
										   string strSetPeerIPAddress,  
										   char* pcharSetEncryptKey, 
										   char* pcharSetEncryptIV, 
										   char* pcharSetDecryptKey, 
										   char* pcharSetDecryptIV) {
			
	u_long ulMode = 1;				/* Mode for Turning on Non-Blocking in Socket */
	sockaddr_in saiConnectInfo;		/* Connection Information */
	int nConnectInfoSize = sizeof(saiConnectInfo);
									/* Size of Connection Information */

	socClient = socSetClient;
	strHomeIPAddress = strSetHomeIPAddress;
	strPeerIPAddress = strSetPeerIPAddress;
	pcharEncryptKey = pcharSetEncryptKey;
	pcharEncryptIV = pcharSetEncryptIV;
	pcharDecryptKey = pcharSetDecryptKey;
	pcharDecryptIV = pcharSetDecryptIV;
	pcharLeftOverMsgPart = NULL;
	nLeftOverMsgLen = 0;
	boolIsNegotiating = false;
	boolIsANegotiation = true;
	boolLeftOverNotRan = true;
	boolNotWouldBlock = true;
	boolConnected = true;
	llSendMsgs = 0,			
	llReceivedMsgs = 0;
	nTimeToLateInMillis = 30;
	boolMsgsSync = true;
	boolRemoveLateMsgs = false;
	nTimeToCheckActInMillis = 30;
	llLastActOrCheckInMillis = time(NULL);

	if (pcharSetEncryptKey != NULL && 
		pcharSetEncryptIV != NULL && 
		pcharSetDecryptKey != NULL && 
		pcharSetDecryptIV != NULL) {
		
		boolHasEncryptInfo = true;
	}
	else {
	
		boolHasEncryptInfo = false;
	}

	if (strSetHomeIPAddress == "") {
	
		if (getsockname(socSetClient, (SOCKADDR *)&saiConnectInfo, &nConnectInfoSize) == 0) {
		
			strHomeIPAddress = string(inet_ntoa(saiConnectInfo.sin_addr));
		}
		else {

			csiOpInfo.AddLogErrorMsg("During setting up client 'Peer To Peer' information, getting Home IP address failed.");
		}
	}

	if (strSetPeerIPAddress == "") {
	
		if (getpeername(socSetClient, (SOCKADDR *)&saiConnectInfo, &nConnectInfoSize) == 0) {
		
			strPeerIPAddress = string(inet_ntoa(saiConnectInfo.sin_addr));
		}
		else {

			csiOpInfo.AddLogErrorMsg("During setting up client 'Peer To Peer' information, getting Peer IP address failed.");
		}
	}

	if (ioctlsocket(socClient, FIONBIO, &ulMode) != NO_ERROR) {
	
		csiOpInfo.AddLogErrorMsg("During setting up client 'Peer To Peer' information, setting non-blocking mode failed.");			
	}

	Sync(false);
	
	pmsiListStoredReceived = NULL;
	pmsiListBackupSent = NULL;
	ppciListNegotiate = NULL;
	ppciNextInfo = NULL;
}

void PeerToPeerClientInfo::Send(char* pcharMsg, int nMsgLen, bool boolTrack) {

	int nSendErrorCode = 0;			/* Send Error Code */

	try {
		
		if (boolConnected) {

			if (socClient != INVALID_SOCKET) {

				if (boolTrack) {
				
					pcharMsg = AddTracking(pcharMsg, &nMsgLen);
				}

				pcharMsg = EncryptMsg(pcharMsg, &nMsgLen);

				if (send(socClient, pcharMsg, nMsgLen, 0) != SOCKET_ERROR) {

					MoveSentMsgToBackup(pcharMsg, nMsgLen);
				}
				else{

					/* Else If There is a Valid Error */
					nSendErrorCode = WSAGetLastError();
								
					if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {

						if (!boolIsNegotiating && !boolIsANegotiation) {

							csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message sender, sending messages failed. Error code: " + csiOpInfo.IntToString(nSendErrorCode));
						}

						CloseClient();
					}
				}
			}
			else {

				csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message sender, connecting to client failed. WSA error code: " + csiOpInfo.IntToString(WSAGetLastError()) + ".");
			}
		}
	}
	catch (exception& exError) {

		csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message sender, an exception occurred.", exError.what());
	}
}

void PeerToPeerClientInfo::Send(string strMsg, bool boolTrack) {

	Send((char *)strMsg.c_str(), strMsg.length(), boolTrack);
}

int PeerToPeerClientInfo::Receive(char* pcharReturn) {

	const int BUFFERSIZE = csiOpInfo.BUFFERSIZE;
									/* Buffer Size */
	const char MSGFILLERCHAR = csiOpInfo.GetMsgFiller();
									/* Filler Character */
	int nReceivedAmount = 0;		/* Amount Received from Server */
//	char acharMsgReceived[csiOpInfo.BUFFERSIZE];
									/* Buffer for Receiving Message */
	string strNewMsg = "",			/* New Message */
		   strMsgType = "";			/* Message Type */
	int nSendErrorCode = 0;			/* Send Error Code */
//	long long llSeqNum = 0,			/* Message Sequence Number */
//			  llTimeInMillis = 0;	/* Time in Milliseconds */
//	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate;
									/* Selected Negotating "Peer To Peer" Client Information */
//						* ppciPrevious = NULL,
									/* Previously Selected Negotating "Peer To Peer" Client Information */
//						* ppciDisconnected = NULL;
									/* Disconnected Selected Negotating "Peer To Peer" Client Information */
//	MsgInfo* pmsiSelect = pmsiListBackupSent;
									/* Selected Message Information Record */
	bool boolIsSync = false;		/* Indicator That a Sync Message Arrived */
//	bool boolResyncing = (string(MsgInfo::FindSegmentInString(strNewMsg, 2)) == "1"),
//		 boolPreviousSynced = (atoi(MsgInfo::FindSegmentInString(strNewMsg, 3)) == llStartTimeInMillis);
									/* Indicator That Negotiator is Resyncing and If They were Previous In Sync */

	try {

		if (boolConnected && boolNotWouldBlock) {

			if (socClient != INVALID_SOCKET) {

				char acharMsgReceived[BUFFERSIZE + 1];
				memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
				
				nReceivedAmount = recv(socClient, &acharMsgReceived[0], BUFFERSIZE - nLeftOverMsgLen, 0);
				nSendErrorCode = WSAGetLastError();

				if (nReceivedAmount > 0) {

					nReceivedAmount = DecryptMsg(&acharMsgReceived[0], 
												 nReceivedAmount);
					boolLeftOverNotRan = true;
					
					if (nReceivedAmount > 0 && (strNewMsg = string(acharMsgReceived)) != "") {

						strMsgType = string(MsgInfo::FindSegmentInString(strNewMsg, 0));

						if (strMsgType != "NEGOTIATESYNC" && strMsgType != "MSGREPLAY" && strMsgType != "MSGCHECK") {

							long long llSeqNum = MsgInfo::GetMetaDataSeqNum(&acharMsgReceived[0], nReceivedAmount);

							if (llSeqNum == 0 || llReceivedMsgs + 1 == llSeqNum) {
							
								long long llTimeInMillis = MsgInfo::GetMetaDataTime(&acharMsgReceived[0], nReceivedAmount);

								if (llTimeInMillis == 0 || !(time(NULL) - llTimeInMillis >= nTimeToLateInMillis && boolRemoveLateMsgs)) {
								
									memcpy(pcharReturn, &acharMsgReceived[0], nReceivedAmount);
								}

								llReceivedMsgs++;
							}
							else if (boolMsgsSync) {

								SendReplay(true);
								boolMsgsSync = false;
							}
							
							if (!boolMsgsSync) {

								if (pmsiListStoredReceived != NULL) {
								
									pmsiListStoredReceived = new MsgInfo(&acharMsgReceived[0], nReceivedAmount);
								}
								else {

									MsgInfo* pmsiSelect = pmsiListStoredReceived;
								
									while (pmsiSelect -> GetNextMsgInfo() != NULL) {
									
										pmsiSelect = pmsiSelect -> GetNextMsgInfo();
									}

									pmsiSelect -> SetNextMsgInfo(new MsgInfo(&acharMsgReceived[0], nReceivedAmount));
								}

								memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
								nReceivedAmount = 0;
							}
						}
						else if (strMsgType == "MSGREPLAY" || strMsgType == "MSGCHECK") {
						
							long long llSeqNum = atoi(MsgInfo::FindSegmentInString(strNewMsg, 1));
							MsgInfo* pmsiSelect = pmsiListBackupSent;

							while (pmsiSelect != NULL) {

								if (llSeqNum <= pmsiSelect -> GetMetaDataSeqNum()) {

									if (send(socClient, pmsiSelect -> GetMsgArray(), pmsiSelect -> Length(), 0) != SOCKET_ERROR) {
									
										pmsiSelect = pmsiSelect -> GetNextMsgInfo();	
									}
									else {

										nSendErrorCode = WSAGetLastError();
								
										if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {

											if (!boolIsNegotiating && !boolIsANegotiation) {

												csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, sending replayed message, #" + to_string(pmsiSelect -> GetMetaDataSeqNum()) + ", failed for message type, '" + strMsgType + "'. Error code: " + csiOpInfo.IntToString(nSendErrorCode));
											}
										}

										pmsiSelect = NULL;
									}
								}
								else {
								
									pmsiSelect = pmsiSelect -> GetNextMsgInfo();
								}
							}
							
							memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
							nReceivedAmount = 0;
						}
						else {

							long long llTimeInMillis = atoi(MsgInfo::FindSegmentInString(strNewMsg, 1));
							bool boolResyncing = (string(MsgInfo::FindSegmentInString(strNewMsg, 2)) == "1"),
								 boolPreviousSynced = (atoi(MsgInfo::FindSegmentInString(strNewMsg, 3)) == llStartTimeInMillis);
								
							if (llTimeInMillis < llStartTimeInMillis || (boolResyncing && boolPreviousSynced)) {
								
								llStartTimeInMillis = llTimeInMillis;
							}
							else if (llTimeInMillis > llStartTimeInMillis && boolResyncing && !boolPreviousSynced) {
						
								Sync(false);
								SendNegotiation();
							}
							
							memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
							nReceivedAmount = 0;
						}

						llLastActOrCheckInMillis = time(NULL);
					}
				}									
				else if (nReceivedAmount <= 0) {

					boolNotWouldBlock = nSendErrorCode != WSAEWOULDBLOCK;

					if (boolNotWouldBlock && nReceivedAmount != 0) {
				
						if (!boolIsNegotiating && !boolIsANegotiation) {

							csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, receiving message from client failed. WSA error code: " + csiOpInfo.IntToString(nSendErrorCode) + ".");
						}

						CloseClient();
					}

					if (nLeftOverMsgLen > 0 && boolLeftOverNotRan) {
						
						nReceivedAmount = nLeftOverMsgLen;
						memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
						memcpy(&acharMsgReceived[0], pcharLeftOverMsgPart, nReceivedAmount);
						
						ClearLeftOverEncryptMsg();

						nReceivedAmount = DecryptMsg(&acharMsgReceived[0], 
													 nReceivedAmount);
						
						if (nReceivedAmount > 0) {

							memcpy(pcharReturn, &acharMsgReceived[0], nReceivedAmount);
						}
						else {

							boolLeftOverNotRan = false;
						}
					}
					else {

						/* Else Processing Connections Being Negotiated */
						PeerToPeerClientInfo* ppciSelected = ppciListNegotiate,
											* ppciPrevious = NULL,
											* ppciDisconnected = NULL;

						while (ppciSelected != NULL && nReceivedAmount <= 0) {
			
							if (ppciSelected -> Connected()) {
		
								nReceivedAmount = ppciSelected -> Receive(acharMsgReceived);

								if (nReceivedAmount > 0) {
								
									memcpy(pcharReturn, &acharMsgReceived[0], nReceivedAmount);
								}
								
								nSendErrorCode = WSAGetLastError();

								if (nSendErrorCode != WSAEWOULDBLOCK && nReceivedAmount != 0) {
				
									ppciDisconnected = ppciSelected;
								}
							}
							else {
						
								ppciDisconnected = ppciSelected;
							}

							if (ppciDisconnected == NULL) {

								ppciPrevious = ppciSelected;
								ppciSelected = ppciSelected -> GetNextClientInfo();
							}
							else {

								if (ppciPrevious != NULL) {
					
									ppciPrevious -> SetNextClientInfo(ppciSelected -> GetNextClientInfo());
								}
								else {
							
									ppciListNegotiate = ppciSelected -> GetNextClientInfo();
								}

								ppciSelected = ppciSelected -> GetNextClientInfo();
								ppciDisconnected -> CloseClient();

								delete ppciDisconnected;
								ppciDisconnected = NULL;
							}
						}

						/* Clear Any Block Indicators for Next Time */
						ppciSelected = ppciListNegotiate;

						while (ppciSelected != NULL) {
						
							ppciSelected -> ClearWouldBlock();
							ppciSelected = ppciSelected -> GetNextClientInfo();
						}
					}
					
					/* If Wait for Message Activity Has Gone Passed Limit, Send Message Check */
					if (time(NULL) - llLastActOrCheckInMillis >= nTimeToCheckActInMillis) {

						SendReplay(false);

						llLastActOrCheckInMillis = time(NULL);
					}
				}

				if (!boolMsgsSync && nReceivedAmount <= 0) {
				
					MsgInfo* pmsiSelect = pmsiListStoredReceived;
								
					if (pmsiSelect != NULL) {
						
						long long llSeqNum = pmsiSelect -> GetMetaDataSeqNum();

						if (llSeqNum == 0 || llReceivedMsgs + 1 == llSeqNum) {
						
							nReceivedAmount = pmsiSelect -> Length();
							memset(acharMsgReceived, MSGFILLERCHAR, BUFFERSIZE + 1);
							memcpy(&acharMsgReceived[0], pmsiSelect -> GetMsgArray(), nReceivedAmount);

							pmsiListStoredReceived = pmsiListStoredReceived -> GetNextMsgInfo();

							delete pmsiSelect;
							pmsiSelect = NULL;
						}
						else if (llSeqNum <= llReceivedMsgs) { 

							pmsiListStoredReceived = pmsiListStoredReceived -> GetNextMsgInfo();

							delete pmsiSelect;
							pmsiSelect = NULL;
						}
					}

					if (pmsiListStoredReceived == NULL) {
					
						boolMsgsSync = true;
					}
				}
			}
			else {
			
				csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, connecting to client failed. WSA error code: " + csiOpInfo.IntToString(WSAGetLastError()) + ".");
			}
		}
	}
	catch (exception& exError) {

		csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, an exception occurred.", exError.what());
	}

	return nReceivedAmount;
}

/* Adds Tracking to a Message, and Return New Message */
char* PeerToPeerClientInfo::AddTracking(char* pcharMsg, int* pnRetMsgLen) {

	string strTrackingData = "";	/* Tracking Data to Add */
	char* pcharNewMsg = NULL;		/* Message with Added Tracking */
	int nMsgLen = *(pnRetMsgLen),
		nNewMsgLen = 0,
		nMsgTypeLen = 0,
		nMsgDataLen = 0,
		nStartIndex = 0;			/* Message Length, Length of Message with Adding Tracking and it Length and
									   Message Type Length and Start Index of Message */

	try {

		nMsgTypeLen = MsgInfo::FindSegmentLengthInString(pcharMsg, 0, nMsgLen);
		nStartIndex = MsgInfo::FindStringStartIndex(pcharMsg, nMsgLen);

		if (nMsgTypeLen > 0 && nStartIndex >= 0) {
		
			strTrackingData = "-" + to_string(time(NULL)) + "-" + to_string(++llSendMsgs);
			nMsgDataLen = strTrackingData.length();

			pcharNewMsg = new char[nMsgLen + nMsgDataLen + 1];
			memset(pcharNewMsg, csiOpInfo.GetMsgFiller(), nMsgLen + nMsgDataLen + 1);

			if (nStartIndex > 0) {

				nNewMsgLen = (nStartIndex + 1) + csiOpInfo.GetMsgStartIndicator().length() + nMsgTypeLen;
			}
			else {
				
				nNewMsgLen = csiOpInfo.GetMsgStartIndicator().length() + nMsgTypeLen;
			}
				
			memcpy(pcharNewMsg, pcharMsg, nNewMsgLen);
			memcpy(pcharNewMsg + nNewMsgLen, (char *)strTrackingData.c_str(), nMsgDataLen);
			memcpy(pcharNewMsg + nNewMsgLen + nMsgDataLen, pcharMsg + nNewMsgLen, nMsgLen - nNewMsgLen);

			*(pnRetMsgLen) += nMsgDataLen;
		}
	}
	catch (exception& exError) {
	
		csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' adding of tracking to message, an exception occurred.", exError.what());	
	}

	return pcharNewMsg;
}

/* Puts a Message in Backup Queue */
void PeerToPeerClientInfo::MoveSentMsgToBackup(char* pcharMsg, int nMsgLen) {
	
	MsgInfo* pmsiSelect = pmsiListBackupSent;
							/* Selected Message Information Record */
	long long llSeqNum = 0; /* Message Sequence Number */
	int nQueueCount = 0;	/* Count of Messages in Backup Queue */
			
	try {
					
		if ((llSeqNum = MsgInfo::GetMetaDataSeqNum(pcharMsg, nMsgLen) > 0)) {

			if (pmsiSelect != NULL) {

				while (pmsiSelect -> GetNextMsgInfo() != NULL) {
				
					nQueueCount++;
					pmsiSelect = pmsiSelect -> GetNextMsgInfo();	
				}

				if (pmsiSelect -> GetMetaDataSeqNum() + 1 == llSeqNum) {
				
					pmsiSelect -> SetNextMsgInfo(new MsgInfo(pcharMsg, nMsgLen));
				}
				else if (llSeqNum > pmsiSelect -> GetMetaDataSeqNum()) {

					/* Else Found Old Messages from Backup Queue, Clear Backup Queue, and Replace with Holder Queue */
					csiOpInfo.AddLogErrorMsg("During storing peer-to-peer backup messages, messages missing from backup queue. Dropping current messages from backup queue.");

					while (pmsiListBackupSent != NULL) {

						pmsiSelect = pmsiListBackupSent;
						pmsiListBackupSent = pmsiListBackupSent -> GetNextMsgInfo();

						delete pmsiSelect;
						pmsiSelect = NULL;
						nQueueCount--;
					}
							
					pmsiListBackupSent = new MsgInfo(pcharMsg, nMsgLen);
				}
				else {
						
					/* Else Found Old Messages Holder Queue, Not Adding to Backup Queue */
					csiOpInfo.AddLogErrorMsg("During storing peer-to-peer backup messages, messages being put into backup queue are old. Backup failed.");
				}
			}
			else {
			
				pmsiListBackupSent = new MsgInfo(pcharMsg, nMsgLen);
			}
		}

		/* Remove Oldest Messages from the Backup Queue When Over Limit */
		while (pmsiListBackupSent != NULL && nQueueCount > nBackupQueueMsgLimit) {

			pmsiSelect = pmsiListBackupSent;
			pmsiListBackupSent = pmsiListBackupSent -> GetNextMsgInfo();

			delete pmsiSelect;
			pmsiSelect = NULL;
			nQueueCount--;
		}
	}
	catch (exception& exError) {

		csiOpInfo.AddLogErrorMsg("During storing backup messages, an exception occurred.", exError.what());
	}
}

/* Do Message Replay*/
void PeerToPeerClientInfo::SendReplay(bool boolIsNotCheck) {

	string strMsg = "";				/* Replay Message */
//	int nSendErrorCode = WSAGetLastError();			
									/* Send Error Code */
	
	if (socClient != INVALID_SOCKET) {

		if (boolIsNotCheck) {
	
			strMsg = csiOpInfo.GetMsgStartIndicator() + "MSGREPLAY" + csiOpInfo.GetMsgPartIndicator() + to_string(llReceivedMsgs + 1) + csiOpInfo.GetMsgEndIndicator();
		}
		else {
	
			strMsg = csiOpInfo.GetMsgStartIndicator() + "MSGCHECK" + csiOpInfo.GetMsgPartIndicator() + to_string(llReceivedMsgs + 1) + csiOpInfo.GetMsgEndIndicator();
		}

		if (send(socClient, (char *)strMsg.c_str(), strMsg.length(), 0) == SOCKET_ERROR) {

			int nSendErrorCode = WSAGetLastError();
								
			if (nSendErrorCode != WSAEWOULDBLOCK && nSendErrorCode != 0) {

				if (!boolIsNegotiating && !boolIsANegotiation) {
					
					if (boolIsNotCheck) {

						csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, sending replay message failed. Error code: " + csiOpInfo.IntToString(nSendErrorCode));
					}
					else {
					
						csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' message receiver, sending check message failed. Error code: " + csiOpInfo.IntToString(nSendErrorCode));
					}
				}

				CloseClient();
			}
		}
	}
}

/* Encrypts Peer To Peer Message */
char* PeerToPeerClientInfo::EncryptMsg(string strMsg, int* pnMsgRetLen) {
			
	const char MSGFILLERCHAR = csiOpInfo.GetMsgFiller();		
								/* Filler Character for Messages */
	const int ENCRYPTIVSIZE = csiOpInfo.ENCRYPTIVSIZE;
								/* Size of Encryption Block */
	char* pcharMsg = NULL;		/* Holder for Message */
/*			* pcharAlteredMsg = NULL;
								/* Encrypted/Decrypted Message */
	int nMsgLen = 0,			/* Message Length */
		nMsgOrigLen = 0;		/* Original Length of Message */
//			nMsgOrigResizeLen = 0,  /* Original Resize Length of String During Encryption */
//			nMsgAddedResizeLen = 0, /* Newly Added Resize Length of String During Encryption */

	try {
			
		*(pnMsgRetLen) = nMsgLen;

		strMsg.insert(0, (ENCRYPTIVSIZE * 2 + (ENCRYPTIVSIZE - (strMsg.length() % ENCRYPTIVSIZE))), ' ');

		nMsgLen = nMsgOrigLen = strMsg.length();

		if (boolHasEncryptInfo && nMsgLen > 0) {
						
			char* pcharAlteredMsg = NULL;

			pcharMsg = (char *)strMsg.c_str();

			EVP_CIPHER_CTX *peccCrypter = EVP_CIPHER_CTX_new();
			
			if (EVP_EncryptInit_ex(peccCrypter, 
								   EVP_aes_256_cbc(), 
								   NULL,
								   (const unsigned char *)pcharEncryptKey,	
								   (const unsigned char *)pcharEncryptIV) != -1) {
				
				int nMsgOrigResizeLen = nMsgLen + csiOpInfo.ENCRYPTIVSIZE;
						
				pcharAlteredMsg = new char[nMsgOrigResizeLen];

				memset(pcharAlteredMsg, MSGFILLERCHAR, nMsgOrigResizeLen);

				if (EVP_EncryptUpdate(peccCrypter, 
									  (unsigned char *)pcharAlteredMsg, 
									  &nMsgOrigResizeLen,
									  (const unsigned char *)pcharMsg,	
									  nMsgLen) != -1) {

					int nMsgAddedResizeLen = nMsgLen - nMsgOrigResizeLen;
						
					if (EVP_EncryptFinal_ex(peccCrypter, 
											(unsigned char *)pcharAlteredMsg + nMsgOrigResizeLen, 
											&nMsgAddedResizeLen) != -1) {
							
						nMsgLen = nMsgOrigResizeLen + nMsgAddedResizeLen;

						memset(pcharMsg, MSGFILLERCHAR, nMsgOrigLen);
						memcpy(pcharMsg, pcharAlteredMsg, nMsgLen);

						*(pnMsgRetLen) = nMsgLen;
					}
					else {
					
						csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' encryption operation, finishing string for encryption failed.");
					}
				}
				else {
				
					csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' encryption operation, updating string for encryption failed.");	
				}
			}
			else {
			
				csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' encryption operation, encryption initialization failed.");								
			}

			EVP_CIPHER_CTX_free(peccCrypter);
		}
	}
	catch (exception& exError) {

		csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' encryption operation, an exception occurred.", exError.what());
	}

	return pcharMsg;
}
	
/* Decrypts Peer To Peer Message */
int PeerToPeerClientInfo::DecryptMsg(char* pcharMsg, int nMsgLen) {
			
	const char MSGFILLERCHAR = csiOpInfo.GetMsgFiller();		
								/* Filler Character for Messages */
	const int ENCRYPTIVSIZE = csiOpInfo.ENCRYPTIVSIZE,
								/* Encryption Block Size */
			  MSGENDINDICATORLEN = csiOpInfo.GetMsgEndIndicator().length();
								/* Length of Message End Indicator */
/*		char* pcharOrigMsg = new char[nMsgOrigLen];
								/* Decrypted Original Message During Processing */
/*			* pcharAlteredMsg = new char[nMsgOrigLen];
								/* Decrypted Message During Processing */
//			* pcharStoreMsg = new char[nMsgOrigLen];
								/* Decrypted Final Message */
//		char* pcharExtraMsg;	/* Left Over From Encryption Failure to Reprocess */
/*		int nMsgOrigResizeLen = nMsgLen,  
									/* Original Resize Length of String During Encryption */
//			nMsgLastEndIndex = 0,	/* Index of a Message's Last End Indicator That Has Been Found */
//			nMsgCurrentEndIndex = 0,/* Index of a Message's Current End Indicator That Has Been Found From Last */
//			nMsgStartIndex = 0,		/* Index of a Message's Start Indicator That Has Been Found From Last End */
//			nMsgCheckLen = 0,		/* Length of Encrypted Message to be Checked */
//			nMsgAddedResizeLen = 0, /* Newly Added Resize Length of String During Encryption */
//			nMsgExtraLen = 0,		/* Left Over from Failed Encryption to be Reprocessed */
//			nCounter = 0;			/* Counter for Loop */

	try {

		if (boolHasEncryptInfo && nMsgLen > 0) {


			EVP_CIPHER_CTX *peccCrypter = EVP_CIPHER_CTX_new();

			if (EVP_DecryptInit_ex(peccCrypter, 
								   EVP_aes_256_cbc(), 
								   NULL,
								   (const unsigned char *)pcharDecryptKey,	
								   (const unsigned char *)pcharDecryptIV) != -1) {
																						  
				if (EVP_CIPHER_CTX_set_padding(peccCrypter, 0) != -1) {

					int nMsgOrigLen = nMsgLen + nLeftOverMsgLen,
						nMsgOrigResizeLen = nMsgOrigLen,
						nMsgLastEndIndex = 0,
						nMsgCurrentEndIndex = 0,
						nMsgStartIndex = 0,
						nMsgCheckLen = 0,
						nMsgAddedResizeLen = 0,
						nCounter = 0;

					char *pcharOrigMsg = new char[nMsgOrigLen],
						 *pcharAlteredMsg = new char[nMsgOrigLen],
						 *pcharStoreMsg = new char[nMsgOrigLen];
						
					memset(pcharOrigMsg, MSGFILLERCHAR, nMsgOrigLen);
					memset(pcharAlteredMsg, MSGFILLERCHAR, nMsgOrigLen);
					memset(pcharStoreMsg, MSGFILLERCHAR, nMsgOrigLen);

					if (nLeftOverMsgLen > 0) {
						
						memcpy(pcharOrigMsg, pcharLeftOverMsgPart, nLeftOverMsgLen);
					}

					memcpy(pcharOrigMsg + nLeftOverMsgLen, pcharMsg, nMsgLen);
					ClearLeftOverEncryptMsg();

					nMsgLen = 0;

					for (nCounter = 0; nCounter < nMsgOrigLen; nCounter++) {
					
						nMsgCheckLen = (nCounter - nMsgLastEndIndex) + 1;

						if (EVP_DecryptUpdate(peccCrypter,
					 						  (unsigned char *)pcharAlteredMsg, 
											  &nMsgOrigResizeLen,
											  (const unsigned char *)pcharOrigMsg + nMsgLastEndIndex,	
											  nMsgCheckLen) != -1) {
																							  
							if ((nMsgCurrentEndIndex = MsgInfo::FindStringEndIndex(pcharAlteredMsg, nMsgOrigResizeLen)) >= 0) {

								if ((nMsgStartIndex = MsgInfo::FindStringStartIndex(pcharAlteredMsg, nMsgOrigResizeLen)) < 0 || nMsgStartIndex >= nMsgCurrentEndIndex) {
								 
									nMsgStartIndex = 0;
								}
								
								nMsgCurrentEndIndex += (MSGENDINDICATORLEN - 1);

								nMsgAddedResizeLen = (ENCRYPTIVSIZE - (((nMsgCurrentEndIndex - nMsgStartIndex) + 1) % ENCRYPTIVSIZE));

								if (nMsgOrigLen - (nCounter + 1) < nMsgAddedResizeLen) {
								
									nMsgAddedResizeLen = nMsgOrigLen - (nCounter + 1);
								}

								if (EVP_DecryptFinal_ex(peccCrypter,
														(unsigned char *)pcharAlteredMsg + nMsgCurrentEndIndex, 
														&nMsgAddedResizeLen) != -1) {

									memcpy(pcharStoreMsg + nMsgLen, pcharAlteredMsg + nMsgStartIndex, (nMsgCurrentEndIndex - nMsgStartIndex) + 1);
									memset(pcharAlteredMsg, MSGFILLERCHAR, nMsgOrigLen);

									nMsgLastEndIndex += nMsgCurrentEndIndex;
									nMsgLen += (nMsgCurrentEndIndex - nMsgStartIndex) + 1;
								}
								else {
					
									csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, finishing string for decryption failed.");
								}
							}
						}
						else {
				
							csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, updating string for decryption failed.");	
						}
					}
			
					int nMsgExtraLen = nMsgOrigLen;

					if (nMsgLastEndIndex > 0) {

						nMsgExtraLen = nMsgOrigLen - (nMsgLastEndIndex + 1);
					}

					if (nMsgExtraLen > ENCRYPTIVSIZE) {

						if (nMsgExtraLen < csiOpInfo.BUFFERSIZE) {				

							nLeftOverMsgLen = nMsgExtraLen;
							pcharLeftOverMsgPart = new char[nMsgExtraLen];

							memset(pcharLeftOverMsgPart, MSGFILLERCHAR, nMsgExtraLen);

							if (nMsgLastEndIndex > 0) {

								memcpy(pcharLeftOverMsgPart, pcharOrigMsg + (nMsgLastEndIndex + 1), nMsgExtraLen);
							}
							else {
							
								memcpy(pcharLeftOverMsgPart, pcharOrigMsg, nMsgExtraLen);
							}
						}
						else {
				
							csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, left over message holder from decryption reached the buffer limit, dumping content.");

							ClearLeftOverEncryptMsg();
						}
					}
					
					memset(pcharMsg, MSGFILLERCHAR, csiOpInfo.BUFFERSIZE + 1);

					if (nMsgLen > 0) {

						memcpy(pcharMsg, pcharStoreMsg, nMsgLen);	
					}
				}
				else {
				
					csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, setting message padding for decryption failed.");	
				}
			}
			else {
			
				csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, decryption initialization failed.");								
			}
					
			EVP_CIPHER_CTX_free(peccCrypter);
		}
	}
	catch (exception& exError) {

		csiOpInfo.AddLogErrorMsg("During client 'Peer to Peer' decryption operation, an exception occurred.", exError.what());
	}

	return nMsgLen;
}
	
/* Decrypts Peer To Peer Message as String */
string PeerToPeerClientInfo::DecryptMsg(string strMsg) {
	
	char* pcharMsg = (char *)strMsg.c_str();

	DecryptMsg(pcharMsg, strMsg.length());

	return string(pcharMsg);
}

void PeerToPeerClientInfo::SetLeftOverEncryptMsg(char* pSetLeftOverMsgPart, int nSetLeftOverMsgLen) {

	if (pSetLeftOverMsgPart != NULL && nSetLeftOverMsgLen > 0) {

		ClearLeftOverEncryptMsg();

		pcharLeftOverMsgPart = new char[nSetLeftOverMsgLen];

		nLeftOverMsgLen = csiOpInfo.PrepCharArrayOut(pSetLeftOverMsgPart, pcharLeftOverMsgPart, nSetLeftOverMsgLen, nSetLeftOverMsgLen);
	}
}

void PeerToPeerClientInfo::ClearLeftOverEncryptMsg() {

	pcharLeftOverMsgPart = NULL;
	nLeftOverMsgLen = 0;
}

void PeerToPeerClientInfo::StartNegotiation(PeerToPeerClientInfo* ppciNegotiateInfo) {

//	PeerToPeerClientInfo* ppciSelected = NULL,
									/* Selected Negotating "Peer To Peer" Client Information */
//  bool boolNegotiateEncypted = ppciNegotiateInfo -> CanEncrypt();
									/* Indicator That Negotating "Peer To Peer" Client Information is Encrypted */

	if (ppciNegotiateInfo -> Connected()) {

		bool boolNegotiateEncrypted = ppciNegotiateInfo -> CanEncrypt();

		if ((boolHasEncryptInfo == boolNegotiateEncrypted) || (!boolHasEncryptInfo && boolNegotiateEncrypted)) {

			if (ppciListNegotiate == NULL) {

				ppciListNegotiate = ppciNegotiateInfo;
			}
			else {
		
				PeerToPeerClientInfo* ppciSelected = ppciListNegotiate;

				while (ppciSelected -> GetNextClientInfo() != NULL) {
				
					ppciSelected = ppciSelected -> GetNextClientInfo();
				}

				ppciSelected -> SetNextClientInfo(ppciNegotiateInfo);
			}

			ppciNegotiateInfo -> SetReceivedMsgLateLimit(nTimeToLateInMillis);
			ppciNegotiateInfo -> SetReceivedDropLateMsgs(boolRemoveLateMsgs);
			ppciNegotiateInfo -> SetReceivedCheckTimeLimit(nTimeToCheckActInMillis);
			ppciNegotiateInfo -> SetBackupQueueLimit(nBackupQueueMsgLimit);

			if (!boolHasEncryptInfo && boolNegotiateEncrypted) {
			
				CloseClient();
			}
			
			boolIsNegotiating = false;
		}
		else {
		
			ppciNegotiateInfo -> CloseClient();
			delete ppciNegotiateInfo;
			ppciNegotiateInfo = NULL;

			csiOpInfo.AddLogErrorMsg("During starting client 'Peer To Peer' negotations, adding new one for " + strPeerIPAddress + " since new doesn't have encryption when old one does.");
		}
	}
}

bool PeerToPeerClientInfo::CheckNegotiation() {
		
	boolIsANegotiation = false;

	if (!boolIsNegotiating && ppciListNegotiate != NULL) {
	
		SendNegotiation();
		boolIsNegotiating = true;
	}
	else if (ppciListNegotiate == NULL) {
	
		boolIsNegotiating = false;
	}

	return boolIsNegotiating;
}

void PeerToPeerClientInfo::SendNegotiation() {
	
	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate,
									/* Selected Negotiating Connection */
						* ppciPrevious = NULL,
									/* Previously Selected Negotating "Peer To Peer" Client Information */
						* ppciDisconnected = NULL;
									/* Disconnected Selected Negotating "Peer To Peer" Client Information */
	string strMsg = csiOpInfo.GetMsgStartIndicator() + "PEERTOPEERNEGOTIATE" + 
					csiOpInfo.GetMsgPartIndicator() + strHomeIPAddress + 
					csiOpInfo.GetMsgEndIndicator();
									/* Negotiation Message */

	while (ppciSelected != NULL) {

		if (ppciSelected -> Connected()) {

			ppciSelected -> Send(strMsg, false);
		}
		else {
						
			ppciDisconnected = ppciSelected;
		}

		if (ppciDisconnected == NULL) {

			ppciPrevious = ppciSelected;
			ppciSelected = ppciSelected -> GetNextClientInfo();
		}
		else {

			if (ppciPrevious != NULL) {
					
				ppciPrevious -> SetNextClientInfo(ppciSelected -> GetNextClientInfo());
			}
			else {
							
				ppciListNegotiate = ppciSelected -> GetNextClientInfo();
			}

			ppciSelected = ppciSelected -> GetNextClientInfo();
			ppciDisconnected -> CloseClient();

			delete ppciDisconnected;
			ppciDisconnected = NULL;
		}
	}
}

void PeerToPeerClientInfo::DoNegotiation() {

	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate,
									/* Selected Negotating "Peer To Peer" Client Information */
						* ppciPrevious = NULL,
									/* Previously Selected Negotating "Peer To Peer" Client Information */
						* ppciDisconnected = NULL;
									/* Disconnected Selected Negotating "Peer To Peer" Client Information */

	try {
		
		while (ppciSelected != NULL) {

			if (ppciSelected -> Connected()) {

				if (ppciSelected -> GetStartTimeInMillis() > llStartTimeInMillis) {
					
					SendNegotiation();
				}
				else if (ppciSelected -> GetStartTimeInMillis() == llStartTimeInMillis) {

					Sync(true);
					ppciSelected -> Sync(true);
					SendNegotiation();
				}
				else {
		
					CloseClient();
				}
			}
			else {
		
				ppciDisconnected = ppciSelected;
			}

			if (ppciDisconnected == NULL) {

				ppciPrevious = ppciSelected;
				ppciSelected = ppciSelected -> GetNextClientInfo();
			}
			else {

				if (ppciPrevious == NULL) {
								
					ppciListNegotiate = ppciSelected -> GetNextClientInfo();
				}
				else {
								
					ppciPrevious -> SetNextClientInfo(ppciSelected -> GetNextClientInfo());
				}

				ppciDisconnected -> CloseClient();

				delete ppciDisconnected;
				ppciDisconnected = NULL;
			}
		}
	}
	catch (exception& exError) {

		csiOpInfo.AddLogErrorMsg("During running client 'Peer To Peer' negotations, an exception occurred.", exError.what());
	}
}

void PeerToPeerClientInfo::Sync(bool boolResync) {
	
	const string MSGPARTINDICATOR = csiOpInfo.GetMsgPartIndicator();
									/* Message Part Indicator */
	long long llPreviousTimeInMillis = llStartTimeInMillis;
									/* Previous Time In Milliseconds */
	string strResyncInfo = "0";		/* Information on Sync Being a Resync, Defaults to 0 as False */

	if (boolResync) {

		llStartTimeInMillis = time(NULL) + rand() % 100;
		strResyncInfo = "1";
	}

	Send(csiOpInfo.GetMsgStartIndicator() + "NEGOTIATESYNC" + 
		 MSGPARTINDICATOR + to_string(llStartTimeInMillis) + 
		 MSGPARTINDICATOR + strResyncInfo + 
		 MSGPARTINDICATOR + to_string(llPreviousTimeInMillis) + 
		 csiOpInfo.GetMsgEndIndicator(), false);
}

long long PeerToPeerClientInfo::GetStartTimeInMillis() {

	return llStartTimeInMillis;
}

bool PeerToPeerClientInfo::CanEncrypt() {

	return boolHasEncryptInfo;
}

char* PeerToPeerClientInfo::GetEncryptKey() {
	
	return pcharEncryptKey;
}

char* PeerToPeerClientInfo::GetEncryptIV() {

	return pcharEncryptIV;
}

string PeerToPeerClientInfo::GetPeerIPAddress() {

	return strPeerIPAddress;
}

bool PeerToPeerClientInfo::Connected() {

	return boolConnected;
}

bool PeerToPeerClientInfo::NotWouldBlock() {

	return boolNotWouldBlock;
}

void PeerToPeerClientInfo::ClearWouldBlock() {

	boolNotWouldBlock = true;
}

void PeerToPeerClientInfo::SetReceivedMsgLateLimit(int nTimeInMillisecs) {

	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate;
											/* Selected "Peer To Peer" Client Information */

	nTimeToLateInMillis = nTimeInMillisecs;

	while (ppciSelected != NULL) {
						
		ppciSelected -> SetReceivedMsgLateLimit(nTimeInMillisecs);

		ppciSelected = ppciSelected -> GetNextClientInfo();
	}
}

void PeerToPeerClientInfo::SetReceivedDropLateMsgs(bool boolDropLateMsgs) {

	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate;
											/* Selected "Peer To Peer" Client Information */

	boolRemoveLateMsgs = boolDropLateMsgs;

	while (ppciSelected != NULL) {
						
		ppciSelected -> SetReceivedDropLateMsgs(boolDropLateMsgs);

		ppciSelected = ppciSelected -> GetNextClientInfo();
	}
}

void PeerToPeerClientInfo::SetReceivedCheckTimeLimit(int nTimeInMillis) {

	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate;
											/* Selected "Peer To Peer" Client Information */

	nTimeToCheckActInMillis = nTimeInMillis;

	while (ppciSelected != NULL) {
						
		ppciSelected -> SetReceivedCheckTimeLimit(nTimeInMillis);

		ppciSelected = ppciSelected -> GetNextClientInfo();
	}
}

/* Sets Queue Limit Value */
void PeerToPeerClientInfo::SetBackupQueueLimit(int nNewLimit) {
		
	PeerToPeerClientInfo* ppciSelected = ppciListNegotiate;
											/* Selected "Peer To Peer" Client Information */

	nBackupQueueMsgLimit = nNewLimit;

	while (ppciSelected != NULL) {
						
		ppciSelected -> SetBackupQueueLimit(nNewLimit);

		ppciSelected = ppciSelected -> GetNextClientInfo();
	}
}

void PeerToPeerClientInfo::SetNextClientInfo(PeerToPeerClientInfo* ppciSetNextInfo) {
	
	ppciSetNextInfo -> SetReceivedMsgLateLimit(nTimeToLateInMillis);
	ppciSetNextInfo -> SetReceivedDropLateMsgs(boolRemoveLateMsgs);
	ppciSetNextInfo -> SetReceivedCheckTimeLimit(nTimeToCheckActInMillis);
	ppciSetNextInfo -> SetBackupQueueLimit(nBackupQueueMsgLimit);

	ppciNextInfo = ppciSetNextInfo;
}

PeerToPeerClientInfo* PeerToPeerClientInfo::GetNextClientInfo() {

	return ppciNextInfo;
}

//void PeerToPeerClientInfo::CloseClient(bool boolSendClose) {
void PeerToPeerClientInfo::CloseClient() {

	PeerToPeerClientInfo* ppciSelected = NULL,
									/* Selected Negotating "Peer To Peer" Client Information */
						* ppciNegotiateWinner = NULL;
									/* Selected Negotating "Peer To Peer" Client Information Winner */
	MsgInfo* pmsiHolder = NULL;		/* Holder for Deleting Messages */

	while (ppciListNegotiate != NULL) {
		
		ppciSelected = ppciListNegotiate;
		ppciListNegotiate = ppciSelected -> GetNextClientInfo();

		if (ppciSelected -> Connected()) {
			
			ppciSelected -> SetNextClientInfo(NULL);

			if (ppciNegotiateWinner == NULL) { 

				ppciNegotiateWinner = ppciSelected;
			}
			else {
									
				ppciNegotiateWinner -> StartNegotiation(ppciSelected);
			}
		}
		else {
		
			delete ppciSelected;
			ppciSelected = NULL;
		}
	}

	if (ppciNegotiateWinner != NULL) {

		ppciNegotiateWinner -> SetNextClientInfo(ppciNextInfo);
		ppciNegotiateWinner -> SetLeftOverEncryptMsg(pcharLeftOverMsgPart, nLeftOverMsgLen);

		ppciNextInfo = ppciNegotiateWinner;
	}
	
	while (pmsiListBackupSent != NULL) {

		pmsiHolder = pmsiListBackupSent;
		pmsiListBackupSent = pmsiListBackupSent -> GetNextMsgInfo();
					
		if (pmsiHolder != NULL) {
					
			delete pmsiHolder;
			pmsiHolder = NULL;
		}
	}
	
	while (pmsiListStoredReceived != NULL) {

		pmsiHolder = pmsiListStoredReceived;
		pmsiListBackupSent = pmsiListStoredReceived -> GetNextMsgInfo();
					
		if (pmsiHolder != NULL) {
					
			delete pmsiHolder;
			pmsiHolder = NULL;
		}
	}

	if (socClient != NULL) {

		shutdown(socClient, SD_BOTH);
		closesocket(socClient);

		socClient = NULL;
	}

	boolConnected = false;
}
								
/* Connects to Server Using Default Settings */
bool Activate() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Connection was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			boolSuccess = csiOpInfo.Connect();
	
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During creating default server connection, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During creating default server connection, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During creating default server connection, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}
								
/* Connects to Server Using Default Settings With SSL */
bool ActivateUsingSSL(char *pcharSetSSLClientKeyName) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Connection was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			csiOpInfo.SetSSLClientKeyName(pcharSetSSLClientKeyName);
			boolSuccess = csiOpInfo.Connect(true);
	
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During creating default server connection using SSL, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During creating default server connection using SSL, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During creating default server connection using SSL, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Connects to Server Using User Settings */
bool ActivateByHostPort(char* pcharSetServerHostNameIP, int nSetServerPort)  {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock == NULL || (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0)) {
	
			boolSuccess = csiOpInfo.Connect(pcharSetServerHostNameIP, nSetServerPort, nSetServerPort, false);
	
			if (hmuxLock != NULL && !ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating server connection by host and port, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating server connection by host and port, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		if (hmuxLock != NULL) {

			ReleaseMutex(hmuxLock);
		}

		csiOpInfo.AddLogErrorMsg("During activating server connection by host and port, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Connects to Server Using User Settings Using SSL */
bool ActivateByHostPortUsingSSL(char* pcharSetServerHostNameIP, int nSetServerPort, char *pcharSetSSLClientKeyName) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock == NULL || (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0)) {

			csiOpInfo.SetSSLClientKeyName(pcharSetSSLClientKeyName);
			boolSuccess = csiOpInfo.Connect(pcharSetServerHostNameIP, nSetServerPort, nSetServerPort, true);

			if (hmuxLock != NULL && !ReleaseMutex(hmuxLock)) {

				csiOpInfo.AddLogErrorMsg("During activating server connection by host and port using SSL, unlocking thread failed.");
			}
		}
		else {

			csiOpInfo.AddLogErrorMsg("During activating server connection by host and port using SSL, locking thread failed.");
		}
	}
	catch (exception& exError) {

		if (hmuxLock != NULL) {

			ReleaseMutex(hmuxLock);
		}

		csiOpInfo.AddLogErrorMsg("During activating server connection by host and port using SSL, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Starts up and Connects to Server Using Default Settings */
bool ActivateWithServer()  {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			csiOpInfo.ActivateServer(); 
			boolSuccess = csiOpInfo.Connect();
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating server and default connection, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating server and default connection, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating server and default connection, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Starts up and Connects to Server Using Default Settings Using SSL */
bool ActivateWithServerUsingSSL(char *pcharSetSSLClientKeyName)  {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			csiOpInfo.ActivateServer();
			csiOpInfo.SetSSLClientKeyName(pcharSetSSLClientKeyName);
			boolSuccess = csiOpInfo.Connect(true);
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating server and default connection using SSL, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating server and default connection using SSL, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating server and default connection using SSL, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Starts up and Connects to Server Using User Settings */
bool ActivateWithServerByPort(int nSetServerPort)  {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			csiOpInfo.ActivateServer(nSetServerPort);
			boolSuccess = csiOpInfo.Connect();	
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating server and connection by port, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating server and connection by port, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating server and connection by port, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Starts up and Connects to Server Using User Settings Using SSL */
bool ActivateWithServerByPortUsingSSL(int nSetServerPort, char* pcharSetSSLClientKeyName) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.ActivateServer(nSetServerPort);
			csiOpInfo.SetSSLClientKeyName(pcharSetSSLClientKeyName);
			boolSuccess = csiOpInfo.Connect(true);

			if (!ReleaseMutex(hmuxLock)) {

				csiOpInfo.AddLogErrorMsg("During activating server and connection by port using SSL, unlocking thread failed.");
			}
		}
		else {

			csiOpInfo.AddLogErrorMsg("During activating server and connection by port using SSL, locking thread failed.");
		}
	}
	catch (exception& exError) {

		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating server and connection by port using SSL, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool StartPeerToPeerServer(char* pcharHostName, int nPort) {
		
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			boolSuccess = csiOpInfo.StartPeerToPeerServer(string(pcharHostName), nPort);			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating peer to peer server, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating peer to peer server, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating peer to peer server, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool StartPeerToPeerServerEncryptedWithKeys(char* pcharHostName, int nPort, char* pcharEncryptKey, char* pcharEncryptIV, int nEncryptKeySize, int nEncryptIVSize) {
		
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (csiOpInfo.ENCRYPTKEYSIZE <= nEncryptKeySize) {
			
				if (csiOpInfo.ENCRYPTIVSIZE <= nEncryptIVSize) {
			
					boolSuccess = csiOpInfo.StartPeerToPeerServerEncryptedWithKeys(string(pcharHostName), nPort, pcharEncryptKey, pcharEncryptIV);			
				}
				else {
			
					csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' server using set encryption, encryption IV block size is smaller than required length.");
				}
			}
			else {
			
				csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' server using set encryption, encryption key size is smaller than required length.");
			}		
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' server using set encryption, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' server using set encryption, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating peer to peer server using set encryption, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}


bool StartPeerToPeerConnect(char* pcharHostName, int nPort) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			boolSuccess = csiOpInfo.StartPeerToPeerConnect(string(pcharHostName), csiOpInfo.IntToString(nPort));			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool StartPeerToPeerConnectEncryptedWithKeys(char* pcharHostName, int nPort, char* pcharEncryptKey, char* pcharEncryptIV, int nEncryptKeySize, int nEncryptIVSize) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Activation was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (csiOpInfo.ENCRYPTKEYSIZE <= nEncryptKeySize) {
			
				if (csiOpInfo.ENCRYPTIVSIZE <= nEncryptIVSize) {

					boolSuccess = csiOpInfo.StartPeerToPeerConnect(string(pcharHostName), csiOpInfo.IntToString(nPort), pcharEncryptKey, pcharEncryptIV);			
				}
				else {
			
					csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection using set encryption, encryption IV block size is smaller than required length.");
				}
			}
			else {
			
				csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connectionusing set encryption, encryption key size is smaller than required length.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection using set encryption, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection using set encryption, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During activating 'Peer to Peer' client connection using set encryption, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}


void Communicate() {
		
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			csiOpInfo.Communicate();
						
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During connection processing, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During connection processing, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During connection processing, an exception occurred.", exError.what(), true);
	}
}

void PeerToPeerCommunicate() {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			csiOpInfo.PeerToPeerCommunicate();		
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During 'Peer to Peer' connection processing, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During 'Peer to Peer' connection processing, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During 'Peer to Peer' connection processing, an exception occurred.", exError.what(), true);
	}
}

/* Starts Stream */
void StartStream(int nNewTransID, char* pcharHostName, int nPort) {
			
	string astrParams[3] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName), csiOpInfo.IntToString(nPort) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("STARTSTREAM", astrParams, 3)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTSTREAM' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + " failed.");
			}				
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTSTREAM' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTSTREAM' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTSTREAM' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", an exception occurred.", exError.what(), true);
	}
}			
		
/* Starts Setup to Send Asynchronous HTTP POST Messages */
void StartHTTPPostAsyncWithHostPort(int nNewTransID, char* pcharHostName, int nPort) {
			
	string astrParams[3] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName), csiOpInfo.IntToString(nPort) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("STARTHTTPPOSTASYNC", astrParams, 3)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + " failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", unlocking thread failed.");
			}		
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", an exception occurred.", exError.what(), true);
	}
}			
		
/* Starts Setup to Send Asynchronous HTTP POST Messages Through Server Post 80 */
void StartHTTPPostAsyncWithHost(int nNewTransID, char* pcharHostName) {
			
	string astrParams[2] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("STARTHTTPPOSTASYNC", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "' failed.");
			}				
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', an exception occurred.", exError.what(), true);
	}
}			
		
/* Starts Setup to Send Synchronous HTTP POST Messages */
void StartHTTPPostSyncWithHostPort(int nNewTransID, char* pcharHostName, int nPort) {
	
	string astrParams[3] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName), csiOpInfo.IntToString(nPort) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("STARTHTTPPOSTSYNC", astrParams, 3)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + " failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", unlocking thread failed.");
			}		
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", an exception occurred.", exError.what(), true);
	}
}	
		
/* Starts Setup to Send Synchronous HTTP POST Messages Through Server Post 80 */
void StartHTTPPostSyncWithHost(int nNewTransID, char* pcharHostName) {
			
	string astrParams[2] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("STARTHTTPPOSTSYNC", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "' failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPPOSTSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', an exception occurred.", exError.what(), true);
	}
}	
		
/* Starts Setup to Send Asynchronous HTTP GET Messages */
void StartHTTPGetASyncWithHostPort(int nNewTransID, char* pcharHostName, int nPort) {

	string astrParams[3] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName), csiOpInfo.IntToString(nPort) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			if (!csiOpInfo.AddSendMsg("STARTHTTPGETASYNC", astrParams, 3)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + " failed.");
			}		
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", an exception occurred.", exError.what(), true);
	}
}
		
/* Starts Setup to Send Asynchronous HTTP GET Messages Through Server Default Port */
void StartHTTPGetASyncWithHost(int nNewTransID, char* pcharHostName) {

	string astrParams[2] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
						
			if (!csiOpInfo.AddSendMsg("STARTHTTPGETASYNC", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "'  failed.");
			}		
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + ", locking thread failed.");
		}		
		
		if (!ReleaseMutex(hmuxLock)) {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + ", unlocking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETASYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + ", an exception occurred.", exError.what(), true);
	}
}
		
/* Starts Setup to Send Synchronous HTTP GET Messages */
void StartHTTPGetSyncWithHostPort(int nNewTransID, char* pcharHostName, int nPort) {

	string astrParams[3] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName), csiOpInfo.IntToString(nPort) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			if (!csiOpInfo.AddSendMsg("STARTHTTPGETSYNC", astrParams, 3)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + " failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', host: '" + string(pcharHostName) + "', port: " + csiOpInfo.IntToString(nPort) + ", an exception occurred.", exError.what(), true);
	}
}
		
/* Starts Setup to Send Synchronous HTTP GET Messages Through Server Default Post */
void StartHTTPGetSyncWithHost(int nNewTransID, char* pcharHostName) {

	string astrParams[2] = { csiOpInfo.IntToString(nNewTransID), string(pcharHostName) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
						
			if (!csiOpInfo.AddSendMsg("STARTHTTPGETSYNC", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "' failed.");
			}		
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'STARTHTTPGETSYNC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "' and host: '" + string(pcharHostName) + "', an exception occurred.", exError.what(), true);
	}
}		
		
/* Sends Direct Message to Server with a Message Designation */
void SendDirectMsgWithDesign(char* pcharMsg, char* pcharDesign) {

	string astrParams[2] = { string(pcharDesign), string(pcharMsg) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
								
			if (!csiOpInfo.AddSendMsg("DIRECTMSG", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG', '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "' failed.");
			}	
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG', '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "', unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG', '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG', '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "', an exception occurred.", exError.what(), true);
	}
}

/* Sends Direct Message to Server with No Message Designation */
void SendDirectMsg(char* pcharMsg) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			SendDirectMsgWithDesign(pcharMsg, (char*)string("").c_str());
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' without designation, unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' without designation, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' without designation, an exception occurred.", exError.what(), true);
	}
}
		
/* Sends Direct Message to Server with a Message Designation to "Peer To Peer" Clients */
void SendDirectMsgPeerToPeerWithDesign(char* pcharMsg, char* pcharDesign) {

	string astrParams[2] = { string(pcharDesign), string(pcharMsg) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.SendPeerToPeerMsg("DIRECTMSG", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "' failed.");
			}		
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "', unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, '" + string(pcharDesign) + "' and message: '" + string(pcharMsg) + "', an exception occurred.", exError.what(), true);
	}
}

/* Sends Direct Message to Server with No Message Designation to "Peer To Peer" Clients */
void SendDirectMsgPeerToPeer(char* pcharMsg) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			SendDirectMsgPeerToPeerWithDesign(pcharMsg, (char*)string("").c_str());
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, message: '" + string(pcharMsg) + "', unlocking thread failed.");
			}			
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, message: '" + string(pcharMsg) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'DIRECTMSG' to 'Peer To Peer' clients, message: '" + string(pcharMsg) + "', an exception occurred.", exError.what(), true);
	}
}
		
/* Adds to Queue of Messages to be Sent Through Stream (Will Not Store Messages That Have Stream Reserved Characters) */
void AddStreamMsg(int nTransID, char* pcharMsg) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), string(pcharMsg) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
								
			if (!csiOpInfo.AddSendMsg("ADDSTREAMMSG", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'ADDSTREAMMSG' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and message: '" + string(pcharMsg) + "' failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'ADDSTREAMMSG' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and message: '" + string(pcharMsg) + "', unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'ADDSTREAMMSG' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and message: '" + string(pcharMsg) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'ADDSTREAMMSG' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and message: '" + string(pcharMsg) + "', an exception occurred.", exError.what(), true);
	}
}

/* Registers Data Process */
void RegisterDataProcess(int nNewTransID, char* pcharDataDesign) {
			
	string astrParams[2] = { csiOpInfo.IntToString(nNewTransID), string(pcharDataDesign) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("REGDATAEXEC", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "' failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', unlocking thread failed.");
			}		
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', an exception occurred.", exError.what(), true);
	}
}

/* Registers Data Process with Parameters or Adds Params to Existing Data Process Transaction */
void RegisterDataProcessWithParams(int nNewTransID, char* pcharDataDesign, char* pcharParamName, char* pcharParamValue) {
			
	string astrParams[4] = { csiOpInfo.IntToString(nNewTransID), string(pcharDataDesign), string(pcharParamName), string(pcharParamValue) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("REGDATAEXEC", astrParams, 4)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', parameter name: '" + string(pcharParamName) + "', and value: '" + string(pcharParamValue) + "' failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', parameter name: '" + string(pcharParamName) + "', and value: '" + string(pcharParamValue) + "', unlocking thread failed.");
			}		
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', parameter name: '" + string(pcharParamName) + "', and value: '" + string(pcharParamValue) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'REGDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nNewTransID) + "', data designation: '" + string(pcharDataDesign) + "', parameter name: '" + string(pcharParamName) + "', and value: '" + string(pcharParamValue) + "', an exception occurred.", exError.what(), true);
	}
}

/* Send Stored HTTP Message */
void SendHTTP(int nTransID, int nNewRespID) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), csiOpInfo.IntToString(nNewRespID) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
		
			if (!csiOpInfo.AddSendMsg("SENDHTTP", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'SENDHTTP' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' with response ID: '" + csiOpInfo.IntToString(nNewRespID) + "' failed.");
			}		
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'SENDHTTP' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' with response ID: '" + csiOpInfo.IntToString(nNewRespID) + "', unlocking thread failed.");
			}	
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'SENDHTTP' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' with response ID: '" + csiOpInfo.IntToString(nNewRespID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'SENDHTTP' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' with response ID: '" + csiOpInfo.IntToString(nNewRespID) + "', an exception occurred.", exError.what(), true);
	}
}

/* Send Data Process */
void SendDataProcess(int nTransID, int nNewRespID, char* pcharDataDesign, bool boolAsync) {

	string astrParams[4] = { csiOpInfo.IntToString(nTransID), csiOpInfo.IntToString(nNewRespID), string(pcharDataDesign), csiOpInfo.BoolToString(boolAsync) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("PROCESSDATAEXEC", astrParams, 4)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'PROCESSDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID, '" + csiOpInfo.IntToString(nNewRespID) + "' failed.");
			}			
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'PROCESSDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID, '" + csiOpInfo.IntToString(nNewRespID) + "', unlocking thread failed.");
			}		
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'PROCESSDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID, '" + csiOpInfo.IntToString(nNewRespID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'PROCESSDATAEXEC' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID, '" + csiOpInfo.IntToString(nNewRespID) + "', an exception occurred.", exError.what(), true);
	}
} 

/* Gets Response Returned from Send Message, Before Deleting it from Response Queue and
   Outputs Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */
void GetHTTPResponse(int nTransID, int nRespID, char* pcharRetMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[2] = { csiOpInfo.IntToString(nTransID), csiOpInfo.IntToString(nRespID) };
									/* Message Parameters */
	MsgInfo* pmsiMsg = NULL,
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			pmsiMsg = csiOpInfo.DequeueReceivedMsg("HTTPRESPONSE", astrParams, 2);

			while (pmsiMsg != NULL) {
			
				strMsg += pmsiMsg -> GetMsgString();
				pmsiPrevious = pmsiMsg;
				pmsiMsg = pmsiMsg -> GetNextMsgInfo();

				delete pmsiPrevious;
				pmsiPrevious = NULL;
			}

			csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 3), 
																					 pcharRetMsg, 
																					 MsgInfo::FindSegmentLengthInString(strMsg, 3),
																					 atoi(pcharRetMsgLen))), 
									pcharRetMsgLen, 
									csiOpInfo.MSGLENSIZE);
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Processing message 'HTTPRESPONSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
										 csiOpInfo.IntToString(nRespID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Processing message 'HTTPRESPONSE' with transaction ID: '" + 
									 csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
									 csiOpInfo.IntToString(nRespID) + "', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Processing message 'HTTPRESPONSE' with transaction ID: '" + 
								 csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
								 csiOpInfo.IntToString(nRespID) + "', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Checks If Gets Response Returned from Send Message, 
   Outputs Length of Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */
int CheckHTTPResponse(int nTransID, int nRespID) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[2] = { csiOpInfo.IntToString(nTransID), csiOpInfo.IntToString(nRespID) };
									/* Message Parameters */	
	MsgInfo* pmsiMsg = NULL;		/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */
	bool boolEndNotFound = true;	/* Indicator That End of Message was Found */
	int nRespLen = 0;				/* Length of Response */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("HTTPRESPONSE", astrParams, 2);

			while (pmsiMsg != NULL && boolEndNotFound) {
			
				strMsg += pmsiMsg -> GetMsgString();

				if (!pmsiMsg -> HasEnd()) {
				
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();
				}
				else {
				
					boolEndNotFound = false;
				}
			}

			if (!boolEndNotFound) {

				nRespLen = MsgInfo::FindSegmentLengthInString(strMsg, 3);

				/* If No Message in Response, Set 1 to Indicate Arrival */
				if (nRespLen == 0) {
				
					nRespLen = 1;
				}
			}
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Checking for message 'HTTPRESPONSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
										 csiOpInfo.IntToString(nRespID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Checking for message 'HTTPRESPONSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Checking for message 'HTTPRESPONSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}

	return nRespLen;
}

/* Gets Response Returned from Send Message, Before Deleting it from Response Queue and
   Outputs Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */
void GetDataProcessResponse(int nTransID, int nRespID, char* pcharRetMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[2] = { csiOpInfo.IntToString(nTransID), csiOpInfo.IntToString(nRespID) };
									/* Message Parameters */
	MsgInfo* pmsiMsg = NULL,
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			pmsiMsg = csiOpInfo.DequeueReceivedMsg("DATAEXECRETURN", astrParams, 2);

			while (pmsiMsg != NULL) {
			
				strMsg += pmsiMsg -> GetMsgString();
				pmsiPrevious = pmsiMsg;
				pmsiMsg = pmsiMsg -> GetNextMsgInfo();

				delete pmsiPrevious;
				pmsiPrevious = NULL;
			}

			csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 3), 
																					 pcharRetMsg, 
																					 MsgInfo::FindSegmentLengthInString(strMsg, 3),
																					 atoi(pcharRetMsgLen))), 
									pcharRetMsgLen, 
									csiOpInfo.MSGLENSIZE);
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Processing message 'DATAEXECRETURN' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
										 csiOpInfo.IntToString(nRespID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Processing message 'DATAEXECRETURN' with transaction ID: '" + 
									 csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
									 csiOpInfo.IntToString(nRespID) + "', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Processing message 'DATAEXECRETURN' with transaction ID: '" + 
								 csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
								 csiOpInfo.IntToString(nRespID) + "', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Checks If Gets Response Returned from Data Process Return Message, 
   Outputs Length of Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */
int CheckDataProcessResponse(int nTransID, int nRespID) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[2] = { csiOpInfo.IntToString(nTransID), csiOpInfo.IntToString(nRespID) };
									/* Message Parameters */	
	MsgInfo* pmsiMsg = NULL;		/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */
	bool boolEndNotFound = true;	/* Indicator That End of Message was Found */
	int nRespLen = 0;				/* Length of Response */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("DATAEXECRETURN", astrParams, 2);

			while (pmsiMsg != NULL && boolEndNotFound) {
			
				strMsg += pmsiMsg -> GetMsgString();

				if (!pmsiMsg -> HasEnd()) {
				
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();
				}
				else {
				
					boolEndNotFound = false;
				}
			}

			if (!boolEndNotFound) {

				nRespLen = MsgInfo::FindSegmentLengthInString(strMsg, 3);

				/* If No Message in Response, Set 1 to Indicate Arrival */
				if (nRespLen == 0) {
				
					nRespLen = 1;
				}
			}
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Checking for message 'DATAEXECRETURN' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
										 csiOpInfo.IntToString(nRespID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Checking for message 'DATAEXECRETURN' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
									 csiOpInfo.IntToString(nRespID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Checking for message 'DATAEXECRETURN' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and response ID: '" + 
								 csiOpInfo.IntToString(nRespID) + "', an exception occurred.", exError.what(), true);
	}

	return nRespLen;
}

/* Gets a Waiting Message from Stream, Before Deleting it from the Wait Queue and
   Outputs Message as a String If It Exists, Else Blank String */
void GetStreamMsg(int nTransID, char* pcharRetMsg, char* pcharRetMsgLen) {
	
	int nSegCount = 0;				/* Count of Message Segments */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[1] = { csiOpInfo.IntToString(nTransID) };
									/* Message Parameters */
	MsgInfo* pmsiMsg = NULL,
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			pmsiMsg = csiOpInfo.DequeueReceivedMsg("STREAMMSG", astrParams, 1);			

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();
					pmsiPrevious = pmsiMsg;
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();

					delete pmsiPrevious;
					pmsiPrevious = NULL;
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 2), 
																						 pcharRetMsg, 
																						 MsgInfo::FindSegmentLengthInString(strMsg, 2),
																						 atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}
			else {
						
				csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
			}
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During processing message 'STREAMMSG', message: '" + strMsg + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During processing message 'STREAMMSG', message: '" + strMsg + "', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During processing message 'STREAMMSG', message: '" + strMsg + "', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Gets Next Waiting Message from Any Stream, Before Deleting it from the Wait Queue and
   Outputs Message as a String If It Exists, Else Blank String */
void GetStreamMsgNext(char* pcharRetMsg, char* pcharRetMsgLen) {
	
	int nSegCount = 0;				/* Count of Message Segments */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiMsg = NULL,
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.DequeueReceivedMsg("STREAMMSG");

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();
					pmsiPrevious = pmsiMsg;
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();

					delete pmsiPrevious;
					pmsiPrevious = NULL;
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 2), 
																						 pcharRetMsg, 
																					     MsgInfo::FindSegmentLengthInString(strMsg, 2),
																					     atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}
			else {
			
				csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
			}
		
			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During processing next message 'STREAMMSG', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During processing next message 'STREAMMSG', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During processing next message 'STREAMMSG', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Gets If Stream Message is Ready
   Returns: Length of Stream Message Successfully, Else 0 */
int CheckStreamMsgReady() {

	int nMsgLen = 0;				/* Length of Message */	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiMsg = NULL;		/* Selected Message Information */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("STREAMMSG");	

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();

					if (!pmsiMsg -> HasEnd()) {
				
						pmsiMsg = pmsiMsg -> GetNextMsgInfo();
					}
					else {
				
						pmsiMsg = NULL;
					}
				}

				nMsgLen = MsgInfo::FindSegmentLengthInString(strMsg, 2);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During finding if a stream messsage exists, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During finding if a stream messsage exists, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During finding if a stream messsage exists, an exception occurred.", exError.what(), true);
	}

	return nMsgLen;
}

/* Gets If Stream Message of a Specific Designation is Ready
   Returns: Length of Stream Message Successfully, Else 0 */
int CheckStreamMsgByIDReady(int nTransID) {

	string astrParams[1] = { csiOpInfo.IntToString(nTransID) };
									/* Message Parameters */
	int nMsgLen = 0;				/* Length of Message */	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiMsg = NULL;		/* Selected Message Information */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("STREAMMSG", astrParams, 1);	

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();

					if (!pmsiMsg -> HasEnd()) {
				
						pmsiMsg = pmsiMsg -> GetNextMsgInfo();
					}
					else {
				
						pmsiMsg = NULL;
					}
				}

				nMsgLen = MsgInfo::FindSegmentLengthInString(strMsg, 2);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During finding if a stream messsage, by transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', exists, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During finding if a stream messsage, by transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', exists, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During finding if a stream messsage, by transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', exists, an exception occurred.", exError.what(), true);
	}

	return nMsgLen;
}

/* Clears Stream Messages, and
   Outputs True If Deletion Successful, Else False */
bool ClearStreamMsgs() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */	
	bool boolSuccess = false;		/* Indicator That Deletion was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			boolSuccess = csiOpInfo.ClearReceivedMsg("STREAMMSG");	

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Clearing messages: 'STREAMMSG', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Clearing messages: 'STREAMMSG', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Clearing messages: 'STREAMMSG', an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Clears Direct Message of a Designation, and
   Outputs True If Deletion Successful, Else False */
bool ClearStreamMsgsByIDReady(int nTransID) {

	string astrParams[1] = { csiOpInfo.IntToString(nTransID) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */	
	bool boolSuccess = false;		/* Indicator That Deletion was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			boolSuccess = csiOpInfo.ClearReceivedMsg("STREAMMSG", astrParams, 1);	

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Clearing messages: 'STREAMMSG', by transaction ID: " + csiOpInfo.IntToString(nTransID) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Clearing messages: 'STREAMMSG', by transaction ID: " + csiOpInfo.IntToString(nTransID) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Clearing messages: 'STREAMMSG', by transaction ID: " + csiOpInfo.IntToString(nTransID) + ", an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Starts Downloaded File from Server */
void GetStreamFile(char* pcharFileDesign) {

	string astrParams[1] = { string(pcharFileDesign) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
				
			if (!csiOpInfo.AddSendMsg("GETSTREAMFILE", astrParams, 1)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'GETSTREAMFILE' with file designation: '" + string(pcharFileDesign) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'GETSTREAMFILE' with file designation: '" + string(pcharFileDesign) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'GETSTREAMFILE' with file designation: '" + string(pcharFileDesign) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'GETSTREAMFILE' with file designation: '" + string(pcharFileDesign) + "', an exception occurred.", exError.what(), true);
	}
}

/* Gets If File was Downloaded from Stream and Outputs File to Destination Directory, Before File is Removed
   Returns: File Contents and True If Successful, Else Blank String and False */
bool CheckStreamFileDownload(char* pcharFileDesign, char* pcharRetFilePath, int nFilePathLen, char* pcharRetFileContents) {
		
	string astrParams[1] = { string(pcharFileDesign) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiFile = NULL,		/* List of File Messages */
		   * pmsiPrevious = NULL;	/* Previously Selected Message Information */
	char* pcharFileCollect = NULL,	/* Collector for File Contents */
		* pcharMsgSelect = NULL;	/* Selected Message File Contents */
	int nFileLen = 0,				/* Length of File */
		nCurrentLen = 0,			/* Length of Currently Selected Contents */
		nMsgLen = 0,				/* Selected Message File Contents Length */
		nCounter = 0;				/* Counter for Loop */
	bool boolSuccess = false,		/* Indicator That Output to File was Successful */
		 boolMsgHasEnd = true;		/* Indicator That Previously Selected Message Had an End */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nFileLen = csiOpInfo.CheckFile(string(pcharFileDesign));

			if (nFileLen > 0) {

				pmsiFile = csiOpInfo.DequeueReceivedMsg("STREAMFILE", astrParams, 1);	
		
				if (pmsiFile != NULL) {

					if (nFilePathLen <= pmsiFile -> GetSegmentLength(2)) {

						csiOpInfo.PrepCharArrayOut(pmsiFile -> GetSegment(2), pcharRetFilePath, nFilePathLen, nFilePathLen);
		
						pcharFileCollect = new char[nFileLen];

						while (pmsiFile != NULL) {
				
							if (nFileLen > nCurrentLen) {
								
								if (boolMsgHasEnd) {
				
									nMsgLen = pmsiFile -> GetSegmentLength(4);
								}
								else {
				
									nMsgLen = pmsiFile -> GetSegmentLength(1);
								}

								if (nMsgLen > 0) {
								
									if (boolMsgHasEnd) {
				
										pcharMsgSelect = pmsiFile -> GetSegment(4);
									}
									else {
				
										pcharMsgSelect = pmsiFile -> GetSegment(1);
									}
									
									if (pcharMsgSelect != NULL) {

										for (nCounter = 0; nCounter < nMsgLen; nCounter++) {
	
											pcharFileCollect[nCurrentLen + nCounter] = pcharMsgSelect[nCounter];
										}

										nCurrentLen += nMsgLen;
									}
								}

								boolMsgHasEnd = pmsiFile -> HasEnd();
							}

							pmsiPrevious = pmsiFile;
							pmsiFile = pmsiFile -> GetNextMsgInfo();

							delete pmsiPrevious;
							pmsiPrevious = NULL;
						}

						if (nFileLen == nCurrentLen) {

							csiOpInfo.PrepCharArrayOut(pcharFileCollect, pcharRetFileContents, nCurrentLen, nCurrentLen);					
							boolSuccess = true;
						}
						else {
						
							csiOpInfo.AddLogErrorMsg("During parsing message 'STREAMFILE' with file designation: '" + string(pcharFileDesign) + 
													 "', only " + csiOpInfo.IntToString(nCurrentLen) + " of " + csiOpInfo.IntToString(nFileLen) + 
													 " file size was found. File will be removed from stream as failure.");
							csiOpInfo.ClearFile(astrParams, 1);
						}
					}
					else {
					
						csiOpInfo.AddLogErrorMsg("During parsing message 'STREAMFILE' with file designation: '" + string(pcharFileDesign) + "', failure occurred due to filepath being too long.");
						csiOpInfo.ClearFile(astrParams, 1);
					}
				}
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During parsing message 'STREAMFILE' with file designation: '" + string(pcharFileDesign) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During parsing message 'STREAMFILE' with file designation: '" + string(pcharFileDesign) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During parsing message 'STREAMFILE' with file designation: '" + string(pcharFileDesign) + "', an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Gets If File is Ready for Downloaded from Stream
   Returns: Length of File Successfully, Else 0 */
int CheckStreamFileReady(char* pcharFileDesign) {

	string astrParams[1] = { string(pcharFileDesign) };
									/* Message Parameters */
	int nFileLen = 0;				/* Length of File */	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nFileLen = csiOpInfo.CheckFile(string(pcharFileDesign));

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During finding if file with designation: '" + string(pcharFileDesign) + "' was downloaded, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During finding if file with designation: '" + string(pcharFileDesign) + "' was downloaded locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During finding if file with designation: '" + string(pcharFileDesign) + "' was downloaded, an exception occurred.", exError.what(), true);
	}

	return nFileLen;
}

/* Gets Length of Path and Name of Strea File
   Returns:  Length of Path and Name of Strea File If Successful, Else 0 */
int GetStreamFilePathLength(char* pcharFileDesign) {

	string astrParams[1] = { string(pcharFileDesign) };
									/* Message Parameters */
	int nFilePathLen = 0;			/* Length of the Path and Name of File */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nFilePathLen = csiOpInfo.GetFilePathLength(astrParams, 1);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During length of path for downloaded file with designation: '" + string(pcharFileDesign) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During length of path for downloaded file with designation: '" + string(pcharFileDesign) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During length of path for downloaded file with designation: '" + string(pcharFileDesign) + "', an exception occurred.", exError.what(), true);
	}

	return nFilePathLen;
}

/* Clears Stream File That Has Been Loaded into Memory */
void ClearStreamFileDownload(char* pcharFileDesign) {

	string astrParams[1] = { string(pcharFileDesign) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.ClearFile(astrParams, 1);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During removing downloaded file with designation: '" + string(pcharFileDesign) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During removing downloaded file with designation: '" + string(pcharFileDesign) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During removing downloaded file with designation: '" + string(pcharFileDesign) + "', an exception occurred.", exError.what(), true);
	}
}

/* Gets List of Downloadable Files */
void GetStreamFileList(char* pcharRetMsg, char* pcharRetMsgLen) {
		
	int nSegCount = 0;				/* Count of Message Segments */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiMsg = NULL;		/* Message Information */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("STREAMFILELIST");

			if (pmsiMsg != NULL) {
				
				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();

					if (!pmsiMsg -> HasEnd()) {
				
						pmsiMsg = pmsiMsg -> GetNextMsgInfo();
					}
					else {
				
						pmsiMsg = NULL;
					}
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 1), 
																						 pcharRetMsg, 
																						 MsgInfo::FindSegmentLengthInString(strMsg, 1),
																						 atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Processing message 'STREAMFILELIST', message: '" + strMsg + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Processing message 'STREAMFILELIST', message: '" + strMsg + "', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Processing message 'STREAMFILELIST', message: '" + strMsg + "', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Gets Specific Direct Message, Before Deleting it from the Wait Queue and
   Outputs Message as a String If It Exists, Else Blank String */
void GetDirectMsg(char* pcharDesign, char* pcharRetMsg, char* pcharRetMsgLen) {
	
	string astrParams[1] = { string(pcharDesign) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */	
	MsgInfo* pmsiMsg = NULL,
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			pmsiMsg = csiOpInfo.DequeueReceivedMsg("DIRECTMSG", astrParams, 1);	

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();
					pmsiPrevious = pmsiMsg;
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();

					delete pmsiPrevious;
					pmsiPrevious = NULL;
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 2), 
																						 pcharRetMsg, 
																						 MsgInfo::FindSegmentLengthInString(strMsg, 2),
																						 atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During processing next message: 'DIRECTMSG', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During processing next message: 'DIRECTMSG', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During processing next message: 'DIRECTMSG', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Gets Next Direct Message, Before Deleting it from the Wait Queue and
   Outputs Message as a String If It Exists, Else Blank String */
void GetDirectMsgNext(char* pcharRetMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */	
	MsgInfo* pmsiMsg = NULL,
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			pmsiMsg = csiOpInfo.DequeueReceivedMsg("DIRECTMSG");	

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();
					pmsiPrevious = pmsiMsg;
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();

					delete pmsiPrevious;
					pmsiPrevious = NULL;
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 2), 
																						 pcharRetMsg, 
																						 MsgInfo::FindSegmentLengthInString(strMsg, 2),
																						 atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}
			else {
			
				csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During processing next message: 'DIRECTMSG', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During processing next message: 'DIRECTMSG', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During processing next message: 'DIRECTMSG', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Gets If Direct Message is Ready
   Returns: Length of Direct Message Successfully, Else 0 */
int CheckDirectMsgsReady() {

	string astrParams[1] = { "" };	/* Message Parameters */
	int nMsgLen = 0;				/* Length of Message */	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiMsg = NULL;		/* Selected Message Information */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("DIRECTMSG", astrParams, 0);	

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();

					if (!pmsiMsg -> HasEnd()) {
				
						pmsiMsg = pmsiMsg -> GetNextMsgInfo();
					}
					else {
				
						pmsiMsg = NULL;
					}
				}

				nMsgLen = MsgInfo::FindSegmentLengthInString(strMsg, 2);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During finding if a direct messsage exists, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During finding if a direct messsage exists, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During finding if a direct messsage exists, an exception occurred.", exError.what(), true);
	}

	return nMsgLen;
}

/* Gets If Direct Message of a Specific Designation is Ready
   Returns: Length of Direct Message Successfully, Else 0 */
int CheckDirectMsgsWithDesignReady(char* pcharFileDesign) {

	string astrParams[1] = { string(pcharFileDesign) };
									/* Message Parameters */
	int nMsgLen = 0;				/* Length of Message */	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	MsgInfo* pmsiMsg = NULL;		/* Selected Message Information */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.FindReceivedMsg("DIRECTMSG", astrParams, 1);	
											
			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();

					if (!pmsiMsg -> HasEnd()) {
				
						pmsiMsg = pmsiMsg -> GetNextMsgInfo();
					}
					else {
				
						pmsiMsg = NULL;
					}
				}

				nMsgLen = MsgInfo::FindSegmentLengthInString(strMsg, 2);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During finding if a direct messsage with designation: '" + string(pcharFileDesign) + "' exists, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During finding if a direct messsage with designation: '" + string(pcharFileDesign) + "' exists, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During finding if a direct messsage with designation: '" + string(pcharFileDesign) + "' exists, an exception occurred.", exError.what(), true);
	}

	return nMsgLen;
}

/* Clears Direct Message, and
   Outputs True If Deletion Successful, Else False */
bool ClearDirectMsgs() {
	
	string astrParams[1] = {""};	/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */	
	bool boolSuccess = false;		/* Indicator That Deletion was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			boolSuccess = csiOpInfo.ClearReceivedMsg("DIRECTMSG", astrParams, 0);	

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Clearing messages: 'DIRECTMSG', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Clearing messages: 'DIRECTMSG', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Clearing messages: 'DIRECTMSG', an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Clears Direct Message of a Designation, and
   Outputs True If Deletion Successful, Else False */
bool ClearDirectMsgsWithDesign(char* pcharDesign) {
	
	string astrParams[1] = { string(pcharDesign) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */	
	bool boolSuccess = false;		/* Indicator That Deletion was Successful */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			boolSuccess = csiOpInfo.ClearReceivedMsg("DIRECTMSG", astrParams, 1);	

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Clearing messages: 'DIRECTMSG' with designation, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Clearing messages: 'DIRECTMSG' with designation, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Clearing messages: 'DIRECTMSG' with designation, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

/* Sets Processing Page for HTTP POST and GET Transmissions */
void SetHTTPProcessPage(int nTransID, char* pcharProcessPathPage) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), string(pcharProcessPathPage) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
				
			if (!csiOpInfo.AddSendMsg("SETHTTPPROCESSPAGE", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'SETHTTPPROCESSPAGE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and HTTP processing page: '" + string(pcharProcessPathPage) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'SETHTTPPROCESSPAGE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and HTTP processing page: '" + string(pcharProcessPathPage) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'SETHTTPPROCESSPAGE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and HTTP processing page: '" + string(pcharProcessPathPage) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'SETHTTPPROCESSPAGE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and HTTP processing page: '" + string(pcharProcessPathPage) + "', an exception occurred.", exError.what(), true);
	}
}

/* Adds a Variable and its Value to the Next Message Being Sent Through HTTP Transmission */
void AddHTTPMsgData(int nTransID, char* pcharVarName, char* pcharValue) {
	
	string astrParams[3] = { csiOpInfo.IntToString(nTransID), string(pcharVarName), string(pcharValue) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
				
			if (!csiOpInfo.AddSendMsg("ADDHTTPMSGDATA", astrParams, 3)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'ADDHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', variable name: '" + string(pcharVarName) + "', variable value: '" + string(pcharValue) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'ADDHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', variable name: '" + string(pcharVarName) + "', variable value: '" + string(pcharValue) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'ADDHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', variable name: '" + string(pcharVarName) + "', variable value: '" + string(pcharValue) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'ADDHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', variable name: '" + string(pcharVarName) + "', variable value: '" + string(pcharValue) + "', an exception occurred.", exError.what(), true);
	}
}        
		
/* Clears Next Message Being Sent Through HTTP Transmission */
void ClearHTTPMsgData(int nTransID) {

	string astrParams[1] = { csiOpInfo.IntToString(nTransID) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("CLEARHTTPMSGDATA", astrParams, 1)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'CLEARHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'CLEARHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'CLEARHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'CLEARHTTPMSGDATA' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}
		
/* Send Message to Use SSL for an HTTP Transmission */
void UseHTTPSSL(int nTransID, bool boolUseSSL) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), csiOpInfo.BoolToString(boolUseSSL) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			if (!csiOpInfo.AddSendMsg("USESSL", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'USESSL' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'USESSL' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'USESSL' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'USESSL' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}

/* Closes Specified Transmission */
void Close(int nTransID) {
	
	string astrParams[1] = { csiOpInfo.IntToString(nTransID) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (!csiOpInfo.AddSendMsg("CLOSE", astrParams, 1)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'CLOSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'CLOSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'CLOSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'CLOSE' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}

/* Gets Log Error Message Before Clearing its Information and
   Outputs Error Message as a String If It Exists, Else Blank String */
void GetLogError(char* pcharRetMsg, char* pcharRetMsgLen) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[1] = {""};	/* Message Parameters */
	MsgInfo* pmsiMsg = NULL,		
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			pmsiMsg = csiOpInfo.DequeueStoredMsg("LOGERRORMSG", astrParams, 0);
			
			if (pmsiMsg == NULL) {
						
				pmsiMsg = csiOpInfo.DequeueReceivedMsg("LOGERRORMSG");
			}

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();
					pmsiPrevious = pmsiMsg;
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();

					delete pmsiPrevious;
					pmsiPrevious = NULL;
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 1), 
																						 pcharRetMsg, 
																						 MsgInfo::FindSegmentLengthInString(strMsg, 1),
																						 atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}
			else {
				
				csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During processing log error message', message: '" + strMsg + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During processing log error message, message: '" + strMsg + "', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During processing log error message, message: '" + strMsg + "', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Gets Log Error Message Before Clearing its Information and 
   Outputs Error Message as a String If It Exists, Else Blank String */
void GetDisplayError(char* pcharRetMsg, char* pcharRetMsgLen) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	string astrParams[1] = {""};	/* Message Parameters */
	MsgInfo* pmsiMsg = NULL,		
		   * pmsiPrevious = NULL;	/* Selected Message Information and Peviously Selected */
	string strMsg = "";				/* Returned Message */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			pmsiMsg = csiOpInfo.DequeueStoredMsg("DISPLAYERRORMSG", astrParams, 0);

			if (pmsiMsg == NULL) {
						
				pmsiMsg = csiOpInfo.DequeueReceivedMsg("DISPLAYERRORMSG");
			}

			if (pmsiMsg != NULL) {

				while (pmsiMsg != NULL) {
			
					strMsg += pmsiMsg -> GetMsgString();
					pmsiPrevious = pmsiMsg;
					pmsiMsg = pmsiMsg -> GetNextMsgInfo();

					delete pmsiPrevious;
					pmsiPrevious = NULL;
				}

				csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(MsgInfo::FindSegmentInString(strMsg, 1), 
																						 pcharRetMsg, 
																						 MsgInfo::FindSegmentLengthInString(strMsg, 1),
																						 atoi(pcharRetMsgLen))), 
										pcharRetMsgLen, 
										csiOpInfo.MSGLENSIZE);
			}
			else {
			
				csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During processing display error message, message: '" + strMsg + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During processing display error message, message: '" + strMsg + "', locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During processing display error message, message: '" + strMsg + "', an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

/* Sets Message Part Character for Stream (Defaults to '!*+#') */
void SetStreamMsgSeparator(int nTransID, char* pcharStreamMsgSeparator) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), string(pcharStreamMsgSeparator) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (!csiOpInfo.AddSendMsg("SETSTREAMMSGSEPARATOR", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSEPARATOR' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSEPARATOR' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSEPARATOR' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSEPARATOR' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}

/* Sets Message Start Character for Stream (Defaults to '%-&>') */
void SetStreamMsgStart(int nTransID, char* pcharStreamMsgStart) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), string(pcharStreamMsgStart) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (!csiOpInfo.AddSendMsg("SETSTREAMMSGSTART", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSTART' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSTART' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSTART' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGSTART' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}

/* Sets Message End Character for Stream (Defaults to '<@^$') */
void SetStreamMsgEnd(int nTransID, char* pcharStreamMsgEnd) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), string(pcharStreamMsgEnd) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (!csiOpInfo.AddSendMsg("SETSTREAMMSGEND", astrParams, 2)) {
	
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGEND' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGEND' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGEND' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGEND' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}

/* Sets Message Filler Character for Stream (Defaults to '\0') */
void SetStreamMsgFiller(int nTransID, char* pcharStreamMsgFiller) {

	string astrParams[2] = { csiOpInfo.IntToString(nTransID), string(pcharStreamMsgFiller) };
									/* Message Parameters */
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
	
			if (string(pcharStreamMsgFiller).length() == 1) {

				if (!csiOpInfo.AddSendMsg("SETSTREAMMSGFILLER", astrParams, 2)) {
	
					csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGFILLER' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' failed.");
				}
			}
			else {
	
				csiOpInfo.AddReplacementErrorMsg("Sending message 'SETSTREAMMSGFILLER' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "' and stream message filler character: '" + 
												 string(pcharStreamMsgFiller) + "' failed due to being bigger than 1 character.", "Warning: Invalid setting, change was ignored.");
			}

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGFILLER' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGFILLER' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("Sending message 'SETSTREAMMSGFILLER' with transaction ID: '" + csiOpInfo.IntToString(nTransID) + "', an exception occurred.", exError.what(), true);
	}
}

void DisconnectPeerToPeer() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.ClosePeerToPeer();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During closing 'Peer to Peer' connection, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During closing 'Peer to Peer' connection, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During closing 'Peer to Peer' connection, an exception occurred.", exError.what(), true);
	}
}

void Disconnect() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.Close();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During disconnecting, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During disconnecting, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During disconnecting, an exception occurred.", exError.what(), true);
	}
}

bool SetMsgStartIndicator(char* pcharSetMsgStartIndicate) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.SetMsgStartIndicator(string(pcharSetMsgStartIndicate));

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting message starting indicator, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting message starting indicator, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting message starting indicator, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool SetMsgPartIndicator(char* pcharSetMsgPartIndicate) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.SetMsgPartIndicator(string(pcharSetMsgPartIndicate));

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting message part indicator, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting message part indicator, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting message part indicator, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool SetMsgEndIndicator(char* pcharSetMsgEndIndicate) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.SetMsgEndIndicator(string(pcharSetMsgEndIndicate));

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting message end indicator, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting message end indicator, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting message end indicator, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool SetMsgIndicatorLen(int nSetMsgIndicatorLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.SetMsgIndicatorLen(nSetMsgIndicatorLen);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting message indicator length, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting message indicator length, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting message indicator length, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

void SetMsgFiller(char charSetMsgFiller) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			csiOpInfo.SetMsgFiller(charSetMsgFiller);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting message filler character, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting message filler character, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting message filler character, an exception occurred.", exError.what(), true);
	}
}

void SetQueueLimit(int nNewLimit) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			csiOpInfo.SetQueueLimit(nNewLimit);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting queue limit, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting queue limit, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting queue limit, an exception occurred.", exError.what(), true);
	}
}

void SetMsgLateLimit(int nTimeInMillisecs) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			csiOpInfo.SetMsgLateLimit(nTimeInMillisecs);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting message late limit, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting message late limit, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting message late limit, an exception occurred.", exError.what(), true);
	}
}

void SetDropLateMsgs(bool boolDropLateMsgs) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			csiOpInfo.SetDropLateMsgs(boolDropLateMsgs);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting indicator to drop message late messages, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting indicator to drop message late messages, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting indicator to drop message late messages, an exception occurred.", exError.what(), true);
	}
}

void SetActivityCheckTimeLimit(int nTimeInMillis) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {
			
			csiOpInfo.SetActivityCheckTimeLimit(nTimeInMillis);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During setting amount of time without message activity before polling server, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During setting amount of time without message activity before polling server, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During setting amount of time without message activity before polling server, an exception occurred.", exError.what(), true);
	}
}

bool IsConnected() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.IsConnected();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During finding if connected, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During finding if connected, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During finding if connected, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool HasPeerToPeerServer() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.HasPeerToPeerServer();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During if 'Peer to Peer' server active, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During if 'Peer to Peer' server active, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During if 'Peer to Peer' server active, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool HasPeerToPeerClients() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolSuccess = false;		/* Indicator That Message Starting Indicator was Set */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolSuccess = csiOpInfo.HasPeerToPeerClients();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During if 'Peer to Peer' connections are active, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During if 'Peer to Peer' connections are active, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During if 'Peer to Peer' connections are active, an exception occurred.", exError.what(), true);
	}

	return boolSuccess;
}

bool IsInSessionGroup() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolInGroup = false;		/* Indicator That User is in Server Session Group */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolInGroup = csiOpInfo.IsInSessionGroup();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During checking if in server session group, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During checking if in server session group, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During checking if in server session group, an exception occurred.", exError.what(), true);
	}

	return boolInGroup;
}

bool IsSessionGroupHost() {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	bool boolIsHost = false;		/* Indicator That User is Host of Server Session Group */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			boolIsHost = csiOpInfo.IsSessionGroupHost();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During checking if user is host of server session group, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During checking if user is host of server session group, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During checking if user is host of server session group, an exception occurred.", exError.what(), true);
	}

	return boolIsHost;
}

void DebugReceived(int nMsgIndex, char* pcharMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(csiOpInfo.DebugReceived(nMsgIndex), 
																					 pcharMsg, 
																					 csiOpInfo.DebugReceivedMsgLength(nMsgIndex),
																					 atoi(pcharRetMsgLen))), 
									pcharRetMsgLen, 
									csiOpInfo.MSGLENSIZE);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

void DebugToSend(int nMsgIndex, char* pcharMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(csiOpInfo.DebugToSend(nMsgIndex), 
																					 pcharMsg, 
																					 csiOpInfo.DebugSendMsgLength(nMsgIndex),
																					 atoi(pcharRetMsgLen))), 
									pcharRetMsgLen, 
									csiOpInfo.MSGLENSIZE);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

void DebugReceivedStored(int nMsgIndex, char* pcharMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	
	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(csiOpInfo.DebugReceivedStored(nMsgIndex), 
																					 pcharMsg, 
																					 csiOpInfo.DebugReceivedStoredMsgLength(nMsgIndex),
																					 atoi(pcharRetMsgLen))), 
									pcharRetMsgLen, 
									csiOpInfo.MSGLENSIZE);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging stored received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging stored received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging stored received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

void DebugToSendStored(int nMsgIndex, char* pcharMsg, char* pcharRetMsgLen) {
	
	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			csiOpInfo.PrepStringOut(csiOpInfo.IntToString(csiOpInfo.PrepCharArrayOut(csiOpInfo.DebugToSendStored(nMsgIndex), 
																					 pcharMsg, 
																					 csiOpInfo.DebugSendStoredMsgLength(nMsgIndex),
																					 atoi(pcharRetMsgLen))), 
									pcharRetMsgLen, 
									csiOpInfo.MSGLENSIZE);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging stored sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging stored sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
			csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging stored sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
		csiOpInfo.PrepStringOut("0", pcharRetMsgLen, csiOpInfo.MSGLENSIZE);
	}
}

int DebugReceivedQueueCount() {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugReceivedQueueCount();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging count of messages in received queue, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging count of messages in received queue, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging count of messages in received queue, an exception occurred.", exError.what(), true);
	}

	return nCount;
}

int DebugSendQueueCount() {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugSendQueueCount();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging count of messages in sending queue, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging count of messages in sending queue, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging count of messages in sending queue, an exception occurred.", exError.what(), true);
	}

	return nCount;
}

int DebugReceivedStoredQueueCount() {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugReceivedStoredQueueCount();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging count of messages in received stored queue, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging count of messages in received stored queue, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging count of messages in receivedq ueue, an exception occurred.", exError.what(), true);
	}

	return nCount;
}

int DebugSendStoredQueueCount() {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugSendStoredQueueCount();

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging count of messages in stored sending queue, unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging count of messages in stored sending queue, locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging count of messages in stored sending queue, an exception occurred.", exError.what(), true);
	}

	return nCount;
}
	
int DebugReceivedMsgLength(int nMsgIndex) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugReceivedMsgLength(nMsgIndex);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging getting length of received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging getting length of received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging getting length of received message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
	}

	return nCount;
}

int DebugSendMsgLength(int nMsgIndex) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugSendMsgLength(nMsgIndex);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging getting length of sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging getting length of sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging getting length of sending message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
	}

	return nCount;
}
	
int DebugReceivedStoredMsgLength(int nMsgIndex) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugReceivedStoredMsgLength(nMsgIndex);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging getting length of received stored message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging getting length of received stored message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging getting length of received stored message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
	}

	return nCount;
}

int DebugSendStoredMsgLength(int nMsgIndex) {

	HANDLE hmuxLock = csiOpInfo.ThreadLocker();
									/* Locker for Thread */
	int nCount = 0;					/* Count of Messages in Sending Queue */

	try {

		if (hmuxLock != NULL && WaitForSingleObject(hmuxLock, INFINITE) == WAIT_OBJECT_0) {

			nCount = csiOpInfo.DebugSendStoredMsgLength(nMsgIndex);

			if (!ReleaseMutex(hmuxLock)) {
			
				csiOpInfo.AddLogErrorMsg("During debugging getting length of sending stored message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", unlocking thread failed.");
			}
		}
		else {
			
			csiOpInfo.AddLogErrorMsg("During debugging getting length of sending stored message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", locking thread failed.");
		}
	}
	catch (exception& exError) {
		
		ReleaseMutex(hmuxLock);
		csiOpInfo.AddLogErrorMsg("During debugging getting length of sending stored message for index, " + csiOpInfo.IntToString(nMsgIndex) + ", an exception occurred.", exError.what(), true);
	}

	return nCount;
}
