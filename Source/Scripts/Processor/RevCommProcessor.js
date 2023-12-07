/* RevCommProcessor.js - Client Communication Processing Script for RevCommSuite API

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

var COMM = {
		
	boolIdle: true, 	/* Indicator That No Communications Are Being Transmitted */
	aQueue: [],			/* List of Communications to be Transmitted */
	aObjReg: [],		/* List of Registered Objects for Updating by Response Messages */
	fnResponse: null,	/* Response Function */
	fnError: null,		/* Error Function */
	objWorker: null,	/* Web Worker for Server and Peer-to-Peer Communications */
	objSendStorage: {HTTP: [], STREAMCLIENT: {}, STREAMRAW: [], DIRECTCLIENT: {}, DATAPROCESS: {}},
                        /* Storage for Sending Messages to Server */
    objSendFuncs: {HTTP: {}, DATAPROCESS: {}, DIRECTCLIENT: []},
    					/* Storage for Functions to Call When Receiving Response Messages to Server */ 
    aReceivedMsgs: [],	/* Holder for Unprocessed Received Messages */
    aobjDataMaps: {},	/* List of Information on Data Maps */
    anUsedIDs: [],		/* Used IDs */
	boolDebug: false,	/* Indicator to do Debug Logging */
	Init: function() {
		
		this.aObjReg["GLOBAL"] = window;
		this.aObjReg["MAIN"] = this;
		this.aObjReg["LOGGER"] = LOGGER;
		
		var aCommFormButtons = document.querySelectorAll('div.divCommFormButtonClass'),
									/* Communications Form Submit Buttons */
			nCommFormButtonCount = aCommFormButtons.length,
									/* Count of Communications Form Submit Buttons */
			nButtonCounter = 0;		/* Counter for Submit Button Loop */
			/*var aCommFormInputs = document.querySelectorAll('a[name="aCommFormLink"], input[name="inpCommFormButton"]'),
									/* Communications Form Submit Inputs */
			/*	  nCommFormInputCount = aCommFormInputs.length,
									/* Count of Communications Form Submit Inputs */
			/*	  nFormInputCounter = 0;		
			 						/* Counter for Submit Input Loop */
			
		for (nButtonCounter = 0; nButtonCounter < nCommFormButtonCount; nButtonCounter++) {
		
			aCommFormInputs = aCommFormButtons[nButtonCounter].querySelectorAll('a[name="aCommFormLink"], input[name="inpCommFormButton"]');
			nCommFormInputCount = aCommFormInputs.length;
			nFormInputCounter = 0;			
			
			for (nFormInputCounter = 0; nFormInputCounter < nCommFormInputCount; nFormInputCounter++) {
			
				aCommFormInputs[nFormInputCounter].addEventListener('click', function(eEvent) {
				
					eEvent.preventDefault();
					
					var objThis = this;
					/*var objSelected = objThis.parentNode,
													/* Selected Object */
					/*	  aLayerWaitLayerList = [], /* Layers Containing the Waiting Image Layer */
					/*	  nLayerWaitLayerCount = 0, /* Count of Layers Containing the Waiting Image Layer */
					/*	  aButtonForms = [],		/* List of Forms Containing the Submit Button */
					/*	  nFormCount = 0;			/* Count of List of Forms Containing the Submit Button */
					/*	  objFormSelect;			/* Selected Form */
					/*	  aFormInputs;				/* Selected Form's Input Fields */
					/*    nFormInputCount = 0;		/* Count of Selected Form's Input Fields */
					/*	  objFormInput;				/* Selected Input of the Selected Form's ID */
					/*	  boolFormIDFound = false;	/* Indicator That The Selected Form Has an Input for Its ID */
					/*	  strInputName = '';		/* Name of the Selected Form's Selected Input */
					/*	  strTransParams = '';		/* String of Parameters Collected for the Selected Form's 
										   			   Input Boxes for Transmission */
					/*	  nCounter = 0;				/* Counter for Loop */
					/*	  nInputCounter = 0;		/* Counter for Selected Form's Inputs Loop */
					
					if (!objThis.getAttribute('disabled')) {
					
						var objSelected = objThis.parentNode,
							aLayerWaitLayerList = [],
							nLayerWaitLayerCount = 0,
							aButtonForms = [],
							nFormCount = 0,
							objFormSelect = null,
							aFormInputs = [],
							nFormInputCount = 0,
							objFormInput = null,
							boolFormIDFound = false,
							strInputName = '',
							strTransParams = '',
							nCounter = 0,
							nInputCounter = 0;	
						
						while (objSelected.parentNode && (objSelected.tagName.trim().toLowerCase() != 'div' || 
														  objSelected.className.trim().toLowerCase() != 'divCommFormButtonClass'.toLowerCase())) {
						
							objSelected = objSelected.parentNode;
						}
					
						if (objSelected) {
							
							aLayerWaitLayerList = objSelected.querySelectorAll('div.divCommFormWaitImgClass');
							nLayerWaitLayerCount = aLayerWaitLayerList.length;
							
							while (objSelected.parentNode) {
								
								objSelected = objSelected.parentNode;
								
								if (objSelected.tagName && objSelected.tagName.trim().toLowerCase() == 'form') {
								
									aButtonForms.push(objSelected);
									nFormCount++;
								}
							}
							
							if (nFormCount) {
							
								/* Disable the Submit Button, and Show its Waiting Image If its There */
								objThis.setAttribute('disabled', 'true');
								
								for (nCounter = 0; nCounter < nLayerWaitLayerCount; nCounter++) {
								
									aLayerWaitLayerList[nCounter].show();
								}
								
								/* Find the Correct Parent Form */
								for (nCounter = 0; nCounter < nFormCount; nCounter++) {
									
									objFormSelect = aButtonForms[nCounter];
									aFormInputs = objFormSelect.querySelectorAll('input');
									nFormInputCount = aFormInputs.length;
									
									if (nFormInputCount) {
											
										/* Cycle Through the Selected Form's Input Box to Get its Values for Transmission Parameters, 
										   But Only Send If it Has a ID Input Field to Prove that the Selected Form is the Correct One */
										for (nInputCounter = 0; nInputCounter < nFormInputCount; nInputCounter++) {
										
											objFormInput = aFormInputs[nInputCounter];
											
											switch (objFormInput.getAttribute('type')) {
								
												case 'text': {}
												case 'hidden': {}
												case 'button': {}
												case 'image': {}
												case 'password': {}
												case 'submit': {
													
													if (strTransParams != '') {
														
														strTransParams += '&';
													}
								
													strInputName = COMM.GetInputNameID(objFormInput);
													strTransParams += strInputName + '=' + objFormInput.value;
													
													/* If the Selected Form's Has an Input for Its ID */
													if (strInputName == 'ID') {
														
														boolFormIDFound = true;
													}
													
													break;
												}
												case 'checkbox': {}
												case 'radio': {
													
													if (objFormInput.getAttribute('checked')) {
														
														if (strTransParams != '') {
															
															strTransParams += '&';
														}
								
														strTransParams += COMM.GetInputNameID(objFormInput) + '=' + objFormInput.value;
													}
													
													break;
												}
												default: {}
											}
										}
										
										/* If the Selected Form Has Been Confirmed as the Correct One, Do Transmission, and 
										   Exit Searching Through Forms */
										if (boolFormIDFound) {
								
											/* Find Any Selects in the Form, and Add Them */
											aFormInputs = objFormSelect.querySelectorAll('select');
											nFormInputCount = aFormInputs.length;
												
											if (nFormInputCount) {
												
												for (nInputCounter = 0; nInputCounter < nFormInputCount; nInputCounter++) {
								
													if (strTransParams != '') {
														
														strTransParams += '&';
													}
													
													objFormInput = aFormInputs[nInputCounter];
													strTransParams += COMM.GetInputNameID(objFormInput) + '=' + objFormInput.value;
												}
											}
											
											COMM.SubmitMsg(objFormSelect.getAttribute('action'), strTransParams, COMM.GetInputNameID(objFormSelect));
											nCounter = nFormCount;		
										}
									}
								}
							}
						}
					}
				});
			}
		}
		
		this.SetMsgProcessFunc();
	},
	Process: null,		/* Function for Processing Messages */
	ClientMsgSend: function(aData) {
		
		if (this.IsConnected()) {
			
			this.objWorker.postMessage(aData);
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send messages.');
		}
	},
	Connect: function(strHostNameIP, nPort, boolSetUseSSL) {
		
//		var aScriptElements = document.getElementsByTagName("script"),
									/* List of Script Elements */
//			nScriptCount = aScriptElements.length,
									/* List of Script Elements */
//			strScriptURL = "",		/* Script's Source URL */
//			nStartIndex = 0,		/* Starting Index of URL */
//			nCounter = 0;			/* Counter for Loop */
		
		if (!this.IsConnected()) {
			
			if (window.Worker && window.crypto && window.WebSocket) {
				
				try {
					
					var aScriptElements = document.getElementsByTagName("script"),
						nScriptCount = aScriptElements.length,
						strScriptURL = "",
						nStartIndex = -1,
						nCounter = 0;
	
					/* Find URL for Current Script to Get Path to Client */
					for (nCounter = 0; nCounter < nScriptCount && nStartIndex <= 0; nCounter++) {
						
						strScriptURL = aScriptElements[nCounter].src;
						nStartIndex = strScriptURL.indexOf('RevCommProcessor.js');
						
						if (nStartIndex < 0) {
							
							nStartIndex = strScriptURL.indexOf('RevCommProcessor.min.js');
						}
						
						if (nStartIndex >= 0) {
							
							strScriptURL = strScriptURL.substr(0, nStartIndex);
							
							if (strScriptURL.length && strScriptURL.lastIndexOf("/") != strScriptURL.length - 1) {
								
								strScriptURL += "/";
							}
						}
					}

					try {

						this.objWorker = new Worker(strScriptURL + 'RevCommClient.js');
					}
					catch(exError) {

						this.objWorker = new Worker(strScriptURL + 'RevCommClient.min.js');
					}
					
					this.objWorker.onmessage = function(evMessage) {
				
						if (evMessage.data) {
							
							COMM.StoreReceivedMsg(evMessage.data);
						}
					}

					this.ClientMsgSend(['Connect', strHostNameIP, nPort, boolSetUseSSL]);
					
					window.requestFileSystem = window.requestFileSystem || window.webkitRequestFileSystem;
				}
				catch(exError) {
					
					LOGGER.Log('Starting client failed. Exception: ' + exError.message);
				}
			}
			else {
				
				LOGGER.Log('Browser does not support Web Workers or Web Crypto or Web Sockets, unable communicate with directly with servers.');
			}
		}
		else {

			LOGGER.Log('Client web worker was already setup.');
		}
	},
	ConnectWithSSL: function(strHostNameIP, nPort) {
		
		this.Connect(strHostNameIP, nPort, true);
	},
	StartHTTPPost: function(nNewTransID, strHostNameIP, nPort, boolAsync) {

        var boolStarted = false;  	/* Indicator That Storage was Started */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewTransID)) {
				
				if (typeof(strHostNameIP) == 'string' && strHostNameIP.trim() != "") {

		            if (!this.ValidateTransID(nNewTransID)) {

		            	this.ClientMsgSend(['StartHTTPPost', nNewTransID, strHostNameIP, nPort, boolAsync]);		
		                this.objSendStorage['HTTP'].push(nNewTransID);
		                boolStarted = true;
		            }
					else {

						LOGGER.Log('Starting HTTP Post communication failed, transmission ID: ' + nNewTransID + ' already exists.');
					}
				}
				else {

					LOGGER.Log('Starting HTTP Post communication failed, hostname/IP is not set.');
				}
			}
			else {

				LOGGER.Log('Starting HTTP Post communication failed, new transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to start server HTTP Post communication.');
		}
        
        return boolStarted;
	},
	StartHTTPGet: function(nNewTransID, strHostNameIP, nPort, boolAsync) {

        var boolStarted = false;  	/* Indicator That Storage was Started */
        
		if (this.IsConnected()) {

			if (Number.isInteger(nNewTransID)) {
				
				if (typeof(strHostNameIP) == 'string' && strHostNameIP.trim() != "") {

		            if (!this.ValidateTransID(nNewTransID)) {

		            	this.ClientMsgSend(['StartHTTPGet', nNewTransID, strHostNameIP, nPort, boolAsync]);	
		                this.objSendStorage['HTTP'].push(nNewTransID);
		                boolStarted = true;
		            }
					else {

						LOGGER.Log('Starting HTTP Get communication failed, transmission ID: ' + nNewTransID + ' already exists.');
					}
				}
				else {

					LOGGER.Log('Starting HTTP Get communication failed, hostname/IP is not set.');
				}
			}
			else {

				LOGGER.Log('Starting HTTP Get communication failed, new transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to start server HTTP Get communication.');
		}
        
        return boolStarted;
	},
	StartStreamClient: function(nNewTransID, strHostNameIP, nPort) {
		
        var boolStarted = false  /* Indicator That Storage was Started */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewTransID)) {
				
				if (typeof(strHostNameIP) == 'string' && strHostNameIP.trim() != "") {
					
					if (Number.isInteger(nPort)) {
			              
						if (!this.ValidateTransID(nNewTransID)) {
						
							this.objSendStorage['STREAMCLIENT'][nNewTransID] = [];
							this.StartStream(nNewTransID, strHostNameIP, nPort);
							boolStarted = true;
						}
						else {

							LOGGER.Log('Starting stream for client messages failed, transmission ID: ' + nNewTransID + ' already exists.');
						}
					}
					else {

						LOGGER.Log('Starting stream for client messages failed, port is not set.');
					}
				}
				else {

					LOGGER.Log('Starting stream for client messages failed, hostname/IP is not set.');
				}
			}
			else {

				LOGGER.Log('Starting stream for client messages failed, new transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to start stream for client messages.');
		}
        
        return boolStarted;
	},
	StartStreamRaw: function(nNewTransID, strHostNameIP, nPort) {
		
        var boolStarted = false  /* Indicator That Storage was Started */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewTransID)) {
				
				if (typeof(strHostNameIP) == 'string' && strHostNameIP.trim() != "") {
					
					if (Number.isInteger(nPort)) {
			              
						if (!this.ValidateTransID(nNewTransID)) {
						
							this.objSendStorage['STREAMRAW'].push(nNewTransID);
							this.StartStream(nNewTransID, strHostNameIP, nPort);
							boolStarted = true;
						}
						else {

							LOGGER.Log('Starting stream for raw messages failed, transmission ID: ' + nNewTransID + ' already exists.');
						}
					}
					else {

						LOGGER.Log('Starting stream for raw messages failed, port is not set.');
					}
				}
				else {

					LOGGER.Log('Starting stream for raw messages failed, hostname/IP is not set.');
				}
			}
			else {

				LOGGER.Log('Starting stream for raw messages failed, new transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to start stream for raw messages.');
		}
        
        return boolStarted;
	},
	StartStream: function(nNewTransID, strHostNameIP, nPort) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewTransID)) {
				
				if (typeof(strHostNameIP) == 'string' && strHostNameIP.trim() != "") {
					
					if (Number.isInteger(nPort)) {
			
						this.ClientMsgSend(['StartStream', nNewTransID, strHostNameIP, nPort]);
					}
					else {

						LOGGER.Log('Starting stream for raw messages failed, port is not set.');
					}
				}
				else {

					LOGGER.Log('Starting stream for raw messages failed, hostname/IP is not set.');
				}
			}
			else {

				LOGGER.Log('Starting stream for raw messages failed, new transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to start stream for raw messages.');
		}
	},
	StartDataProcess: function(nNewTransID, strDataDesign) {
        
        var boolStarted = false;  	/* Indicator That Storage was Started */
		
		if (this.IsConnected()) {
			
			if (typeof(strDataDesign) == 'string' && strDataDesign.trim() != "") {
	
				if (!this.ValidateTransID(nNewTransID)) {
	          
					this.objSendStorage['DATAPROCESS'][nNewTransID] = {DESIGNATION: strDataDesign, 
	                                                            	   PARAMS: []};
					boolStarted = true;
				}
				else {

					LOGGER.Log('Starting data process failed, transmission ID: ' + nNewTransID + ' already exists.');
				}
			}
			else {

				LOGGER.Log('Starting data process failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to start a data process.');
		}
        
        return boolStarted;
	},
	StreamClientAddVar: function(nTransID, strRegObjDesign, strVarName, mxVarValue) {

//		var aobjStreamClientStorage = this.objSendStorage["STREAMCLIENT"],
									/* Storage for Stream Client Message */
//			nStoreLen = aobjStreamClientStorage.length,			
									/* Length of the Array */
//    		nCounter = 0;			/* Counter for Loop */
        var boolStored = false;   	/* Indicator That Update was Stored */

		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strRegObjDesign) == 'string' && strRegObjDesign.trim() != "") {
					
					if (typeof(strVarName) == 'string' && strVarName.trim() != "") {
						
						if (typeof(mxVarValue) != 'undefined' && mxVarValue != null) {
							
							var aobjStreamClientStorage = this.objSendStorage["STREAMCLIENT"],
									/* Storage for Stream Client Message */
								nStoreLen = 0,			
									/* Length of the Array */
						        nCounter = 0;			
									/* Counter for Loop */

							/* Only Update If the Variable Name is a String and the Value Is Not a Hash or Contain One */
							if (typeof(strVarName) == 'string' && 
								mxVarValue &&
								(typeof(mxVarValue) != 'object' || 
								 Array.isArray(mxVarValue)) && 
								!this.ArrayHasObject(mxParams)) {
				                
				               if (aobjStreamClientStorage[nTransID]) {

						        	 nStoreLen = aobjStreamClientStorage.length;

					                  /* Go Through Each Set of Updates to Find If Any are for the Specified Object Designation */
						        	 for (nCounter = 0; nCounter < nStoreLen && !boolStored; nCounter++) {
					
						        		 if (aobjStreamClientStorage[nCounter]["DESIGNATION"] == strRegObjDesign) {
   
						        			 aobjStreamClientStorage[nCounter]["VARUPDATES"].push({NAME: strVarName, VALUE: mxVarValue});
				                             boolStored = true;
						        		 }
						             }      
				               }
				               else {
				                    
				            	   aobjStreamClientStorage[nTransID] = [{DESIGNATION: strRegObjDesign,
				                                                         VARUPDATES: [{NAME: strVarName, VALUE: mxVarValue}],
				                                                         FUNCCALLS: []}];
				                   boolStored = true;
				               }
							}
							else {

								LOGGER.Log('Adding variable to stream for client messages failed, invalid value sent.');
							}
						}
						else {

							LOGGER.Log('Adding variable to stream for client messages failed, variable value is not set.');
						}
					}
					else {

						LOGGER.Log('Adding variable to stream for client messages failed, variable name is not set.');
					}
				}
				else {

					LOGGER.Log('Adding variable to stream for client messages failed, designation is not set.');
				}
			}
			else {

				LOGGER.Log('Adding variable to stream for client messages failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to adding variable to stream for client messages.');
		}
        
        return boolStored;
	},
	StreamClientAddFuncCall: function(nTransID, strRegObjDesign, strFuncName, mxParams) {

//		var aobjStreamClientStorage = this.objSendStorage["STREAMCLIENT"],
									/* Storage for Stream Client Message */
//			nStoreLen = aobjStreamClientStorage.length,			
									/* Length of the Array */
//    		nCounter = 0;			/* Counter for Loop */
        var boolStored = false;   	/* Indicator That Update was Stored */
        
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strRegObjDesign) == 'string' && strRegObjDesign.trim() != "") {
					
					if (typeof(strFuncName) == 'string' && strFuncName.trim() != "") {
							
						var aobjStreamClientStorage = this.objSendStorage["STREAMCLIENT"],
								/* Storage for Stream Client Message */
							nStoreLen = 0,			
								/* Length of the Array */
					        nCounter = 0;			
								/* Counter for Loop */

						/* Only Update If the Variable Name is a String and the Value Is Not a Hash or Contain One */
						if (typeof(strVarName) == 'string' && 
							mxVarValue &&
							(typeof(mxVarValue) != 'object' || 
							 Array.isArray(mxVarValue)) && 
							!this.ArrayHasObject(mxParams)) {
							
		                   if (mxParams && !Array.isArray(mxParams)) {
			                       
		                         mxParams = [mxParams];    
		                   }
			                
			               if (aobjStreamClientStorage[nTransID]) {

					        	 nStoreLen = aobjStreamClientStorage.length;

				                  /* Go Through Each Set of Updates to Find If Any are for the Specified Object Designation */
					        	 for (nCounter = 0; nCounter < nStoreLen && !boolStored; nCounter++) {
				
					        		 if (aobjStreamClientStorage[nCounter]["DESIGNATION"] == strRegObjDesign) {
  
						        		 aobjStreamClientStorage[nCounter]["FUNCCALLS"].push({NAME: strFuncName, PARAMS: mxParams});
			                             boolStored = true;
					        		 }
					             }      
			               }
			               else {
			                    
			            	   aobjStreamClientStorage[nTransID] = [{DESIGNATION: strRegObjDesign,
			                                                         VARUPDATES: [],
			                                                         FUNCCALLS: [{NAME: strFuncName, PARAMS: mxParams}]}];
			                   boolStored = true;
			               }
						}
						else {

							LOGGER.Log('Adding function call to stream for client messages failed, invalid value sent.');
						}
					}
					else {

						LOGGER.Log('Adding function call to stream for client messages failed, function name is not set.');
					}
				}
				else {

					LOGGER.Log('Adding function call to stream for client messages failed, designation is not set.');
				}
			}
			else {

				LOGGER.Log('Adding function call to stream for client messages failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to adding function call to stream for client messages.');
		}

        return boolStored;
	},
    AddStreamMsg: function(nTransID, strMsg) {

		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strMsg) == 'string' && strMsg.trim() != "") {				
					
					this.ClientMsgSend(['AddStreamMsg', nTransID, strMsg]);
				}
				else {

					LOGGER.Log('Adding stream message failed, message is not set.');
				}
			}
			else {

				LOGGER.Log('Adding stream message failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to add stream message.');
		}
    
	},
	DirectClientMsgAddVar: function(strRegObjDesign, strVarName, mxVarValue) {

        var boolStored = false;   	/* Indicator That Update was Stored */
		
		if (this.IsConnected()) {
			
			if (typeof(strRegObjDesign) == 'string' && strRegObjDesign.trim() != "") {
				
				if (typeof(strVarName) == 'string' && strVarName.trim() != "") {
					
					if (typeof(mxVarValue) != 'undefined' && mxVarValue != null) {

						/* Only Update If the Variable Name is a String and the Value Is Not a Hash or Contain One */
						if (typeof(strVarName) == 'string' && 
							mxVarValue &&
							(typeof(mxVarValue) != 'object' || 
							 Array.isArray(mxVarValue)) && 
							!this.ArrayHasObject(mxVarValue)) {

							if (this.objSendStorage["DIRECTCLIENT"]["DESIGNATION"] == strRegObjDesign) {
							
								 this.objSendStorage["DIRECTCLIENT"]["VARUPDATES"].push({NAME: strVarName, VALUE: mxVarValue});
			                }
			                else {
			                    
			            	   	 this.objSendStorage["DIRECTCLIENT"] = {DESIGNATION: strRegObjDesign,
			                                                         	VARUPDATES: [{NAME: strVarName, VALUE: mxVarValue}],
			                                                         	FUNCCALLS: []};
			                }
			                   
			                boolStored = true;
						}
						else {

							LOGGER.Log('Adding variable for direct client messages failed, function name is not set.');
						}
					}
					else {

						LOGGER.Log('Adding variable for direct client messages failed, variable value is not set.');
					}
				}
				else {

					LOGGER.Log('Adding variable for direct client messages failed, variable name is not set.');
				}
			}
			else {

				LOGGER.Log('Adding variable for direct client messages failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to adding variable for direct client messages.');
		}
		
		return boolStored;
	},
	DirectClientMsgAddFuncCall: function(strRegObjDesign, strFuncName, mxParams) {
		
        var boolStored = false;   	/* Indicator That Update was Stored */
        
		if (this.IsConnected()) {

			if (typeof(strRegObjDesign) == 'string' && strRegObjDesign.trim() != "") {
				
				if (typeof(strFuncName) == 'string' && strFuncName.trim() != "") {

					/* Only Update If the Variable Name is a String and the Value Is Not a Hash or Contain One */
					if (typeof(mxParams) == 'string' && 
						mxParams &&
						(typeof(mxParams) != 'object' || 
						 Array.isArray(mxParams)) && 
						!this.ArrayHasObject(mxParams)) {
						
	                   if (mxParams && !Array.isArray(mxParams)) {
		                       
	                       mxParams = [mxParams];    
	                   }
		                
		               if (this.objSendStorage["DIRECTCLIENT"]["DESIGNATION"] == strRegObjDesign) {

		            	   this.objSendStorage["DIRECTCLIENT"]["FUNCCALLS"].push({NAME: strFuncName, PARAMS: mxParams});
		               }
		               else {
		                    
		            	   this.objSendStorage["DIRECTCLIENT"] = {DESIGNATION: strRegObjDesign,
		                                                          VARUPDATES: [],
		                                                          FUNCCALLS: [{NAME: strFuncName, PARAMS: mxParams}]};
		               }
		                   
		               boolStored = true;
					}
					else {

						LOGGER.Log('Adding function call to direct messages failed, parameter is not set.');
					}
				}
				else {

					LOGGER.Log('Adding function call to direct messages failed, function name is not set.');
				}
			}
			else {

				LOGGER.Log('Adding function call to direct messages failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to adding function call for direct client messages.');
		}
		
		return boolStored;
	},
	AddDataProcessParams: function(nTransID, strParamName, mxParamValue) {

		var boolStoraged = false;  	/* Indicator That Parameter was Stored */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
					
				if (typeof(mxParamValue) != 'undefined' && mxParamValue != null) {
				
					if (typeof(mxParamValue) == 'boolean') { 
						
						if (mxParamValue) {
						
							mxParamValue = 1;	
						}
						else {
						
							mxParamValue = 0;	
						}
					}
					else {
						
						mxParamValue = mxParamValue.toString();
					}	
				}
				else {
					
					mxParamValue = '';
				}

				if (this.objSendStorage['DATAPROCESS'][nTransID]) {
			  
					this.objSendStorage['DATAPROCESS'][nTransID]['PARAMS'].push({NAME: strParamName, VALUE: mxParamValue});
					boolStoraged = true 
				}
				else {

					LOGGER.Log('Adding adding parameter for a data process failed, transaction ID, "' + nTransID + '" was not found.');
				}
			}
			else {

				LOGGER.Log('Adding adding parameter for a data process failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to adding parameter for a data process.');
		}
		
		return boolStoraged;
	},
	AddDataMapFunc: function(strDesign, objSource, strFuncName, strDataProcessDesign, strDataParamName) {
		
/*		var strOrgFuncCode = objSource[strFuncName].toString(),
									/* Data Map Function as String */
/*			strOrgFuncAugList = strOrgFuncCode.substring(strOrgFuncCode.indexOf('(') + 1, 
														 strOrgFuncCode.indexOf(')'))
											  .split(",")
											  .map(strAug => strAug.trim())
											  .join("','"); */
									/* List of Data Map Function's Augments for Use In Replacing Function */
									
		if (typeof(strDesign) == 'string' && strDesign.trim() != '') {		
		
			this.ClearDataMap(strDesign);
		
			if (typeof(objSource) == 'object') {
						
				if (typeof(strFuncName) == 'string' && strFuncName.trim() != '' && objSource.hasOwnProperty(strFuncName)) { 
			
					var strOrgFuncCode = objSource[strFuncName].toString(),
						strOrgFuncAugList = strOrgFuncCode.substring(strOrgFuncCode.indexOf('(') + 1, 
																	 strOrgFuncCode.indexOf(')'))
														  .split(",")
														  .map(strAug => strAug.trim())
														  .join('","');
						
					if (strOrgFuncAugList != '') {
						
						strOrgFuncAugList = '"' + strOrgFuncAugList + '",';
					}

					objSource['revcom_' + strFuncName] = new Function('return new Function(' + 
																	  strOrgFuncAugList + '"' + 
																	  strOrgFuncCode.substring(strOrgFuncCode.indexOf('{') + 1, 
																	  						   strOrgFuncCode.length - 2)
																	  				.replace(/\r?\n|\r/g, "") + '");')()
															.bind(objSource);

					objSource[strFuncName] = new Function('																							\
																																					\
						var mxValue = this.revcom_' + strFuncName + '.apply(this, arguments);														\
																																					\
						if (COMM.UpdateDataMapVal("' + strDesign + '", mxValue)) {																	\
																																					\
							var nMsgID = COMM.GetUniqueID();																						\
																																					\
							if (COMM.StartDataProcess(nMsgID, "' + strDataProcessDesign + '")) {													\
																																					\
								if (!COMM.AddDataProcessParams(nMsgID, "' + strDataParamName + '", mxValue)) {										\
																																					\
									LOGGER.Log("Processing data map, designation: \'' + strDesign + '\', adding parameter failed.");				\
								}																													\
																																					\
								if (!COMM.SendDataProcess(nMsgID, COMM.GetUniqueID())) {															\
																																					\
									LOGGER.Log("Processing data map, designation: \'' + strDesign + '\', send failed.");							\
								}																													\
							}																														\
							else {																													\
																																					\
								LOGGER.Log("Processing data map, designation: \'' + strDesign + '\', start failed.");								\
							}																														\
						}																															\
																																					\
						return mxValue;').bind(objSource);
						
					this.aobjDataMaps[strDesign] = {
						'TYPE': 0,
						'SOURCE': objSource,
						'METHOD': strFuncName,
						'DATAPROCESS': strDataProcessDesign,
						'PARAM': strDataParamName,
						'VALUE': null
					};
				}
				else {
	
					LOGGER.Log('Adding function call for a data map, designation: "' + strDesign + '", failed, invalid information was sent.');
				}	
			}
			else {
	
				LOGGER.Log('Adding function call for a data map, designation: "' + strDesign + '", failed, function name is not set.');
			}
		}
		else {

			LOGGER.Log('Adding function call for a data map failed, designation is not set.');
		}
	},
	AddDataMapVar: function(strDesign, objSource, strVarName, strDataProcessDesign, strDataParamName) {
		
		if (typeof(strDesign) == 'string' && strDesign.trim() != '') {		
		
			this.ClearDataMap(strDesign);
		
			if (typeof(objSource) == 'object') {
						
				if (typeof(strVarName) == 'string' && strVarName.trim() != "" && objSource.hasOwnProperty(strVarName)) { 
			
					Object.defineProperty(objSource, strVarName, {
						
						set(mxValue) {
							
							this.value = mxValue;
							
							if (COMM.UpdateDataMapVal(strDesign, mxValue)) {
							
								var nMsgID = COMM.GetUniqueID();
			
								if (COMM.StartDataProcess(nMsgID, strDataProcessDesign)) {
										
									if (!COMM.AddDataProcessParams(nMsgID, strDataParamName, mxValue)) {
				
										LOGGER.Log('Processing data map, designation: "' + strDesign + '", adding parameter failed.');
									}
									
									if (!COMM.SendDataProcess(nMsgID, COMM.GetUniqueID())) {
				
										LOGGER.Log('Processing data map, designation: "' + strDesign + '", send failed.');
									}
								}
								else {
									
									LOGGER.Log('Processing data map, designation: "' + strDesign + '", start failed.');
								}
							}
						},
						get() {
							
							return this.value;
						}
					});
						
					this.aobjDataMaps[strDesign] = {
						'TYPE': 1,
						'SOURCE': objSource,
						'METHOD': strVarName,
						'DATAPROCESS': strDataProcessDesign,
						'PARAM': strDataParamName,
						'VALUE': objSource[strVarName]
					};
				}
				else {
	
					LOGGER.Log('Adding variable to a data map, designation: "' + strDesign + '", failed, invalid information was sent.');
				}	
			}
			else {
	
				LOGGER.Log('Adding variable to a data map, designation: "' + strDesign + '", failed, variable name is not set.');
			}
		}
		else {

			LOGGER.Log('Adding variable to a data map failed, designation is not set.');
		}
	},
	SetHTTPResponseFuncs: function(nTransID, objDestination, strFuncName) {
		
        var boolSet = false;        /* Indicator That Parameter was Stored */
        
		if (this.IsConnected()) {
		
			if (Number.isInteger(nTransID)) {
				
				if (typeof(objDestination) == 'object') {
					
					if (typeof(strFuncName) == 'string' && strFuncName.trim() != "" && objDestination.hasOwnProperty(strFuncName)) {

			            if (objDestination && 
			                strFuncName && 
			                strFuncName != "") {

			                  if (!this.objSendFuncs['HTTP'][nTransID]) {
			                       
			                	  this.objSendFuncs['HTTP'][nTransID] = [];
			                  }
			                  
			                  this.objSendFuncs['HTTP'][nTransID].push({OBJECT: objDestination,
			                                                       		METHOD: strFuncName});
			                  boolSet = true;
						}
						else {

							LOGGER.Log('Adding function call for a data process failed, invalid information was sent.');
						}	
					}
					else {

						LOGGER.Log('Adding function call for a data process failed, function name is not set.');
					}
				}
				else {

					LOGGER.Log('Adding function call for a data process failed, destination object is not set.');
				}
			}
			else {

				LOGGER.Log('Adding function call for a data process failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to add function to responses HTTP communications.');
		}
		
        return boolSet;
	},
	SetDirectClientMsgFuncs: function(objDestination, strFuncName, strDesign) {

        var boolSet = false;        /* Indicator That Parameter was Stored */
        
		if (this.IsConnected()) {
			
			if (typeof(objDestination) == 'object') {
				
				if (typeof(strFuncName) == 'string' && strFuncName.trim() != "" && objDestination.hasOwnProperty(strFuncName)) {

					if (objDestination && 
		                strFuncName && 
		                strFuncName != "") {
		                    
						this.objSendFuncs['DIRECTCLIENT'].push({OBJECT: objDestination,
		                                                   		METHOD: strFuncName,
		                                                   		DESIGNATION: strDesign});
		                boolSet = true;
		            }
					else {

						LOGGER.Log('Set function call for response from direct client messages failed, invalid information was sent.');
					}
				}
				else {

					LOGGER.Log('Set function call for response from direct client messages failed, function name is not set.');
				}
			}
			else {

				LOGGER.Log('Set function call for response from direct client messages failed, destination object is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set function for execution on response from direct client messages.');
		}
		
		return boolSet;
	},
	SetDataProcessResponseFuncs: function(nTransID, objDestination, strFuncName) {

        var boolSet = false;        /* Indicator That Parameter was Stored */
        
		if (this.IsConnected()) {
		
			if (Number.isInteger(nTransID)) {
			
				if (typeof(objDestination) == 'object') {
				
					if (typeof(strFuncName) == 'string' && strFuncName.trim() != "") {
			               
						if (objDestination && 
							strFuncName && 
							strFuncName != "") {
		          
		                    if (!this.objSendFuncs['DATAPROCESS'][nTransID]) {
		                         
		                    	this.objSendFuncs['DATAPROCESS'][nTransID] = [];
		                    }
		                    
		                    this.objSendFuncs['DATAPROCESS'][nTransID].push({OBJECT: objDestination,
		                                                                	 METHOD: strFuncName});
			                boolSet = true;
						}
						else {

							LOGGER.Log('Set data process response function call failed, invalid information was sent.');
						}		
					}
					else {

						LOGGER.Log('Set data process response function call failed, function name is not set.');
					}
				}
				else {

					LOGGER.Log('Set data process response function call failed, destination object is not set.');
				}
			}
			else {

				LOGGER.Log('Set data process response function call failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set function for execution on response from data process.');
		}
        
        return boolSet;
	},
	UpdateDataMapVal: function (strDesign, mxValue) {
		
		var boolUpdated = false;
		
		if (strDesign && 
			strDesign != "") {
				
			if (this.aobjDataMaps.hasOwnProperty(strDesign) &&
				this.aobjDataMaps[strDesign]['VALUE'] != mxValue) {
			
				this.aobjDataMaps[strDesign]['VALUE'] = mxValue;
				boolUpdated = true;	
			}
		}
		
		return boolUpdated;
	},
	SendStreamClientMsg: function(nTransID) {

        var boolSend = false;     	/* Indicator That Message was Sent */
        
		if (this.IsConnected()) {
		
			if (Number.isInteger(nTransID)) {

				if (this.objSendStorage['STREAMCLIENT'][nTransID]) {
	            
	            	if (JSON) {

		            	this.AddStreamMsg(nTransID, JSON.stringify(this.objSendStorage['STREAMCLIENT'][nTransID]));
		            	this.objSendStorage['STREAMCLIENT'] = Object.keys(this.objSendStorage['STREAMCLIENT'])
		            												.reduce(function(retValue, mxIndex) {
			
							if (mxIndex != nTransID) {
								
								retValue[mxIndex] = this.objSendStorage['STREAMCLIENT'][mxIndex];
							}
							
							return retValue;
						}, {}); 
	    	              
	        	    	boolSend = true;
	            	}
	            	else {

						LOGGER.Log('Browser does not support native JSON, sending stream client message failed.');	
					}
				}
				else {

					LOGGER.Log('Sending stream client message failed, transaction ID, "' + nTransID + '" was not found.');
				}
			}
			else {

				LOGGER.Log('Sending stream client message failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send storaged stream client message.');
		}
        
        return boolSend;
	},
	SendStreamRawMsg: function(nTransID, strMsg) {
		
		var boolSend = false;     	/* Indicator That Message was Sent */
		
		if (this.IsConnected()) {
		
			if (Number.isInteger(nTransID)) {
				
				if (typeof(strMsg) == 'string' && strMsg.trim() != "") {
					
					if (this.objSendStorage['STREAMRAW'][nTransID]) {
		            
						this.AddStreamMsg(nTransID, strMsg);
		                boolSend = true
					}
					else {

						LOGGER.Log('Sending stream raw message failed, transaction ID, "' + nTransID + '" was not found.');
					}
				}
				else {

					LOGGER.Log('Sending stream raw message failed, message is not set.');
				}
			}
			else {

				LOGGER.Log('Sending stream raw message failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send stream raw message.');
		}
            
        return boolSend;
	},
	SendDirectClientMsg: function(strMsgDesign) {
	
        var boolSend = false     	/* Indicator That Message was Sent */
		
		if (this.IsConnected()) {
            
            if (this.objSendStorage['DIRECTCLIENT']) {
                
                if (JSON) {
	
					if (!strMsgDesign) {
						
						strMsgDesign = '';
					}
	
	                this.SendDirectRawMsg(JSON.stringify([this.objSendStorage['DIRECTCLIENT']]), strMsgDesign);    
	                this.ClearDirectClientMsgDesign();
	                boolSend = true;
				}
				else {
					
					LOGGER.Log('Browser does not support native JSON, sending direct client message failed.');	
				}
            }
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send storaged direct client messages.');
		}
        
        return boolSend;
	},
	SendDirectRawMsg: function(strMsg, strMsgDesign) {
		
		if (this.IsConnected()) {

			if (typeof(strMsg) == 'string' && strMsg.trim() != "") {
	
				if (!strMsgDesign) {
					
					strMsgDesign = '';
				}
			
				this.ClientMsgSend(['SendDirectRawMsg', strMsg, strMsgDesign]);		
			}
			else {

				LOGGER.Log('Sending direct raw message failed, message is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send direct raw messages.');
		}
	},
	SendHTTP: function(nTransID, nNewRespID, boolAutoRetrieval) {

        var boolSend = false;     	/* Indicator That Message was Sent */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (Number.isInteger(nNewRespID)) {
		              
		            if (this.objSendStorage['HTTP'][nTransID]) {

		            	this.ClientMsgSend(['SendHTTP', nTransID, nNewRespID, boolAutoRetrieval]);
		            	boolSend = true;
		            }
					else {
	
						LOGGER.Log('Sending HTTP message failed, transmission ID: ' + nTransID + ' does not exist.');
					}
				}
				else {

					LOGGER.Log('Sending HTTP message failed, new response ID is not set.');
				}
			}
			else {

				LOGGER.Log('Sending HTTP message failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send storaged HTTP message.');
		}
		
		return boolSend;
	},
	SendDataProcess: function(nTransID, nNewRespID, boolAsync, boolAutoRetrieval) {
		
		var boolSend = false;     	/* Indicator That Message was Sent */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (Number.isInteger(nNewRespID)) {
					
					if (typeof(boolAsync) != 'boolean') {
						
						boolAsync = true;
					}
					
					if (typeof(boolAutoRetrieval) != 'boolean') {
						
						boolAutoRetrieval = true;
					}
				  
				  	if (this.objSendStorage['DATAPROCESS'][nTransID]) {
				  
				  		this.ClientMsgSend(['SendDataProcess', 
		  									nTransID, 
		  									nNewRespID, 
		  									boolAsync, 
		  									boolAutoRetrieval,
		  									this.objSendStorage['DATAPROCESS'][nTransID]['DESIGNATION'],
		  									this.objSendStorage['DATAPROCESS'][nTransID]['PARAMS']]);
				  		boolSend = true;
				  	}
				  	else {

						LOGGER.Log('Sending data process information failed, transmission ID was invalid.');
				  	}
				}
				else {

					LOGGER.Log('Sending data process information failed, new response ID is not set.');
				}
			}
			else {

				LOGGER.Log('Sending data process information failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to send storaged data process information.');
		}
			  
		return boolSend;
	},
	ChangeDirectClientMsgDesign: function(strRegObjDesign) {
		
		var boolChanged = false;  	/* Indicator That Update was Stored */
		
		if (this.IsConnected()) {

			if (typeof(strRegObjDesign) == 'string' && strRegObjDesign.trim() != "") {
                 
				this.objSendStorage["DIRECTCLIENT"]["DESIGNATION"] = strRegObjDesign
	            boolChanged = true	
			}
			else {

				LOGGER.Log('Sending direct raw message failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to change designation for selected direct client message.');
		}
		 
		return boolChanged;
	},
	ClearDirectClientMsgDesign: function() {
		
		this.objSendStorage["DIRECTCLIENT"] = {};
	},
	GetHTTPResponse: function(nTransID, nRespID, boolProcessCmd) {
	
		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (Number.isInteger(nRespID)) {

					if ((objMsgSelect = this.GetReceivedMsg("HTTP", [{NAME: "TRANSID", VALUE: nTransID}, 
																	 {NAME: "RESPID", VALUE: nRespID}]))) {

						if ((strMsg = objMsgSelect.MESSAGE) != '') {
							
							if (!objMsgSelect.AUTOPROCESS) {
								
								if (boolProcessCmd) {
									
									this.Process(strMsg);
								}
								
								this.RunSendFuncs('HTTP', nTransID, strMsg);
							}
							
							if (objMsgSelect.AUTODELETE || boolDeleteTrans) {
								
								this.TranClose(nTransID, true);
							}
						}
					}
				}
				else {

					LOGGER.Log('Getting response from a sent HTTP message failed, new response ID is not set.');
				}
			}
			else {

				LOGGER.Log('Getting response from a sent HTTP message failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get response from a sent HTTP message.');
		}
		
		return strMsg;
	},
	GetDataProcessResponse: function(nTransID, nRespID, boolDeleteTrans, boolProcessCmd) {
		
		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (Number.isInteger(nRespID)) {
					
					if ((objMsgSelect = this.GetReceivedMsg("DATAPROCESS", [{NAME: "TRANSID", VALUE: nTransID}, 
																			{NAME: "RESPID", VALUE: nRespID}]))) {

						if ((strMsg = objMsgSelect.MESSAGE) != '') {
							
							if (!objMsgSelect.AUTOPROCESS) {
								
								if (boolProcessCmd) {
									
									this.Process(strMsg);
								}
								
								this.RunSendFuncs('DATAPROCESS', nTransID, strMsg);
							}
							
							if (objMsgSelect.AUTODELETE || boolDeleteTrans) {
								
								this.TranClose(nTransID, true);
							}
						}
					}
				}
				else {

					LOGGER.Log('Getting response from a sent data process failed, new response ID is not set.');
				}
			}
			else {

				LOGGER.Log('Getting response from a sent data process failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get response from a sent data process.');
		}
		
		return strMsg;
	},
	GetStreamMsg: function(nTransID, boolProcessCmd) {

		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */
			
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				this.ClientMsgSend(['GetStreamMsg', nTransID, boolProcessCmd]);
				
				if ((objMsgSelect = this.GetReceivedMsg("STREAMMSG", [{NAME: "TRANSID", VALUE: nTransID}]))) {

					if ((strMsg = objMsgSelect.MESSAGE) != '' && !objMsgSelect.AUTOPROCESS && boolProcessCmd) {
									
						this.Process(strMsg);
					}
				}
			}
			else {

				LOGGER.Log('Getting message from stream failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get message from stream.');
		}
		
		return strMsg;
	},
	GetStreamMsgNext: function(boolProcessCmd) {

		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['GetStreamMsgNext', boolProcessCmd]);

			if ((objMsgSelect = this.GetReceivedMsg("STREAMMSG"))) {

				if ((strMsg = objMsgSelect.MESSAGE) != '' && !objMsgSelect.AUTOPROCESS && boolProcessCmd) {
								
					this.Process(strMsg);
				}
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get next message from next available stream.');
		}
	},
	GetDirectMsg: function(strDesign, boolProcessCmd) {
		
		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */

		if (this.IsConnected()) {

			if (typeof(strDesign) == 'string' && strDesign.trim() != "") {
			
				this.ClientMsgSend(['GetDirectMsg', strDesign, boolProcessCmd]);
				
				if ((objMsgSelect = this.GetReceivedMsg("DIRECTMSG", [{NAME: "DESIGNATION", VALUE: strDesign}]))) {

					if ((strMsg = objMsgSelect.MESSAGE) != '' && !objMsgSelect.AUTOPROCESS && boolProcessCmd) {
									
						this.Process(strMsg);
					}
				}
			}
			else {

				LOGGER.Log('Getting direct message failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get a direct message.');
		}
	},
	GetDirectMsgNext: function(boolProcessCmd) {

		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['GetDirectMsgNext', boolProcessCmd]);		
			
			if ((objMsgSelect = this.GetReceivedMsg("DIRECTMSG"))) {

				if ((strMsg = objMsgSelect.MESSAGE) != '' && !objMsgSelect.AUTOPROCESS && boolProcessCmd) {
								
					this.Process(strMsg);
				}
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get next direct message.');
		}
	},
	ClearStreamMsgs: function(nTransID) {
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['ClearStreamMsgs', nTransID]);
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to clear stream messages.');
		}
	},
	ClearDirectMsgs: function(strDesign) {
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['ClearDirectMsgs', strDesign]);
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to clear direct messages.');
		}
	},
	ClearDataMap: function(strDesign) {
		
		if (typeof(strDesign) == 'string' && strDesign.trim() != "" && this.aobjDataMaps[strDesign]) {
		
			var objDataMap = this.aobjDataMaps[strDesign];
			
			if (objDataMap['TYPE'] == 0) {
				
				objDataMap['SOURCE'][objDataMap['METHOD']] = objDataMap['SOURCE']['revcom_' + objDataMap['METHOD']].bind(objSource);
				delete objDataMap['SOURCE']['revcom_' + objDataMap['METHOD']];
			}
			else if (objDataMap['TYPE'] == 1) {
			
				Object.defineProperty(objDataMap['SOURCE'], strVarName, {
					
					set(mxValue) {
						
						this.value = mxValue;
					}
				});
			}
			
			delete this.aobjDataMaps[strDesign];
		} 
	},
	GetReceivedMsg: function(strMsgType, aCheckParams, boolReadOnly) {
	
		var aMsgList = this.aReceivedMsgs,
									/* List of Messages */
			objMsgSelect = null,	/* Selected Message */
			objParamSelect,			/* Selected Check Paramter */
			nMsgTotal = aMsgList.length,
									/* Count of Messages */
			nParamTotal = 0,		/* Count of Parameters */
			boolParamsMatch = false,/* Indicator That Parameter Checks all Matched */
			nMsgCounter = 0,		/* Counter for Messages Loop */
			nParamCounter = 0;		/* Counter for Parameter Loop */
	
		if (typeof(strMsgType) == 'string' && strMsgType != '') {
			
			if (typeof(aCheckParams) == 'object' && Array.isArray(aCheckParams)) {
				
				nParamTotal = aCheckParams.length;

				for (nParamCounter = 0; nParamCounter < nParamTotal; nParamCounter++) {
					
					objParamSelect = aCheckParams[nParamCounter];
					
					if (typeof(objParamSelect) != 'object' || 
						!objParamSelect.hasOwnProperty('NAME') || 
						!objParamSelect.hasOwnProperty('VALUE')) {
						
						aCheckParams.splice(nParamCounter, 1);
						nParamCounter--; 
						nParamTotal--;
					}
				}
				
				nParamTotal = aCheckParams.length;
			}
			else {
				
				aCheckParams = [];
			}
			
			for (nMsgCounter = 0; nMsgCounter < nMsgTotal; nMsgCounter++) {
				
				objMsgSelect = aMsgList[nMsgCounter];
			
				if (objMsgSelect.TYPE == strMsgType) {
					
					boolParamsMatch = true;

					for (nParamCounter = 0; nParamCounter < nParamTotal && boolParamsMatch; nParamCounter++) {
						
						boolParamsMatch = objMsgSelect[aCheckParams[nParamCounter].NAME] == aCheckParams[nParamCounter].VALUE;
					}
					
					if (boolParamsMatch) {
					
						if (!boolReadOnly) {

							aMsgList.splice(nMsgCounter, 1);
						}
						
						nMsgCounter = nMsgTotal;
					}
				}
				
				if (!boolParamsMatch) {
					
					objMsgSelect = null;
				}
			}
		}
		else {
			
			LOGGER.Log('Getting received message from queue failed, invalid message type sent.');
		}
		
		return objMsgSelect;
	},
	GetAvailableFileList: function() {

		var strMsg = '',			/* Response Message */
			objMsgSelect;			/* Selected Message */
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['GetAvailableFileList']);
		
			if ((objMsgSelect = this.GetReceivedMsg("STREAMFILELIST"))) {

				strMsg = objMsgSelect.MESSAGE;
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get list of available files to download.');
		}
		
		return strMsg;
	},
	RunSendFuncs: function(strType, nTransID, mxValue) {
		
//		var aList = this.objSendFuncs[strType][nTransID],
//			nListCount = aList.length,
//			nCounter = 0;			/* List of Messages, Selected Message, Message Count, Counter for Loop */
		
		if (this.objSendFuncs[strType][nTransID]) {
			
			var aList = this.objSendFuncs[strType][nTransID],
				nListCount = aList.length,
				nCounter = 0;
				
			if (!Array.isArray(mxValue)) {
				
				mxValue = [mxValue];
			}
			
			for (nCounter = 0; nCounter < nListCount; nCounter++) {
			
				aList[nCounter].OBJECT[aList[nCounter].METHOD].apply(aList[nCounter].OBJECT, mxValue);
			}
		}
	},
	SetHTTPProcessPage: function(nTransID, strPageURL) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strPageURL) == 'string' && strPageURL.trim() != "") {
			
					this.ClientMsgSend(['SetHTTPProcessPage', nTransID, strPageURL]);
				}
				else {

					LOGGER.Log('Setting processing page for HTTP communication failed, processing page URL is not set.');
				}
			}
			else {

				LOGGER.Log('Setting processing page for HTTP communication failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set HTTP communication\'s process page.');
		}
	},
	AddHTTPMsgData: function(nTransID, strVarName, strValue) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strVarName) == 'string' && strVarName.trim() != "") {
					
					if (typeof(strValue) != 'undefined' && strValue != null) {
			
						this.ClientMsgSend(['AddHTTPMsgData', nTransID, strVarName, strValue]);
					}
					else {

						LOGGER.Log('Adding data to a HTTP communication failed, variable value is not set.');
					}
				}
				else {

					LOGGER.Log('Adding data to a HTTP communication failed, variable name is not set.');
				}
			}
			else {

				LOGGER.Log('Adding data to a HTTP communication failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to add data to a HTTP communication.');
		}
	},
	ClearHTTPMsgData: function(nTransID) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
			
				this.ClientMsgSend(['ClearHTTPMsgData', nTransID]);
			}
			else {

				LOGGER.Log('Clearing data from a HTTP communication failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to clear data from a HTTP communication.');
		}
	},
	UseHTTPSSL: function(nTransID, boolUseSSL) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
			
				this.ClientMsgSend(['UseHTTPSSL', nTransID, boolUseSSL]);
			}
			else {

				LOGGER.Log('Changing indicator to use of HTTP SSL failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to change indicator to use of HTTP SSL.');
		}
	},
	TranClose: function(nTransID, boolReleaseID) {
		
        var boolClosed = false;  	/* Indicator That Transmission was Closed */
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
	              
	            if (this.ValidateTransID(nTransID)) {
					
					this.ClientMsgSend(['TranClose', nTransID]);	
	              
	                if (this.objSendStorage['STREAMCLIENT'][nTransID]) {
	                        
	                	delete this.objSendStorage['STREAMCLIENT'][nTransID];
	                }  
	                else if (this.objSendStorage['STREAMRAW'][nTransID]) {
	                  
	                	delete this.objSendStorage['STREAMRAW'][nTransID];
	                }
	                else if (this.objSendStorage['HTTP'][nTransID]) {
	        
	                	delete this.objSendStorage['HTTP'][nTransID];
	                   
	                    if (this.objSendFuncs['HTTP'][nTransID]) {
	                           
	                    	delete this.objSendFuncs['HTTP'][nTransID];
	                    }

	                    this.ClientMsgSend(['ReceiverClose', 'HTTP', nTransID]); 
	                }
	                else if (this.objSendStorage['DATAPROCESS'][nTransID]) {
	        
	                	delete this.objSendStorage['DATAPROCESS'][nTransID];
	                   
	                    if (this.objSendFuncs['DATAPROCESS'][nTransID]) {
	                           
	                    	delete this.objSendFuncs['DATAPROCESS'][nTransID];
	                    }

	                    this.ClientMsgSend(['ReceiverClose', 'DATAPROCESS', nTransID]); 
	                }
	                  
	                boolClosed = true
	            }

	            if (boolReleaseID) {
	            	
	            	this.ReleaseUniqueID(nTransID);
	            }
			}
			else {

				LOGGER.Log('Closing a transaction failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to close a transaction.');
		}

        return boolClosed;
	},
	SetStreamTranMsgSeparatorChar: function(nTransID, strMsgSeparatorChars) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strMsgSeparatorChars) == 'string' && strMsgSeparatorChars.trim() != "") {
			
					this.ClientMsgSend(['SetStreamTranMsgSeparatorChar', nTransID, strMsgSeparatorChars]);				
				}
				else {

					LOGGER.Log('Setting part separator character for stream messages failed, message part characters is not set.');
				}
			}
			else {

				LOGGER.Log('Setting part separator character for stream messages failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set part separator character for stream messages.');
		}
	},
	SetStreamTranEndChars: function(nTransID, strMsgEndChars) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(strMsgEndChars) == 'string' && strMsgEndChars.trim() != "") {
					
					this.ClientMsgSend(['SetStreamTranEndChars', nTransID, strMsgEndChars]);
				}
				else {

					LOGGER.Log('Setting end separator character for stream messages failed, message part characters is not set.');
				}
			}
			else {

				LOGGER.Log('Setting end separator character for stream messages failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set end indicator character for stream messages.');
		}
	},
	SetStreamTranFillerChar: function(nTransID, charMsgFillerChar) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nTransID)) {
				
				if (typeof(charMsgFillerChar) == 'string' && charMsgFillerChar.trim() != "") {
					
					this.ClientMsgSend(['SetStreamTranFillerChar', nTransID, charMsgFillerChar]);
				}
				else {

					LOGGER.Log('Setting filler character for stream messages failed, message part characters is not set.');
				}
			}
			else {

				LOGGER.Log('Setting filler character for stream messages failed, transmission ID is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set filler character for stream messages.');
		}
	},
	GetLogError: function() {
		
		var objMsgSelect;			/* Selected Message */
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['GetLogError']);	
				
			if ((objMsgSelect = this.GetReceivedMsg("LOGERRORMSG"))) {

				if (objMsgSelect.MESSAGE != '') {
								
					LOGGER.Log(objMsgSelect.MESSAGE);
				}
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get log messages.');
		}
	},
	GetDisplayError: function() {
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['GetDisplayError']);
				
			if ((objMsgSelect = this.GetReceivedMsg("DISPLAYERRORMSG"))) {

				if (objMsgSelect.MESSAGE != '') {
								
					LOGGER.Log(objMsgSelect.MESSAGE, true);
				}
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to get display messages.');
		}
	},
	GetUniqueID: function() {

        var nID = Math.floor(Math.random() * Math.floor(999999));          
                            		/* New ID */
                            
        while (this.anUsedIDs.includes(nID)) {
        
        	nID = Math.floor(Math.random() * Math.floor(999999)); 
        }
        
        this.anUsedIDs.push(nID);
        
        return nID;
	},
	ReleaseUniqueID: function(nID) {
      
    	if (this.anUsedIDs.includes(nID)) {
       
    		this.anUsedIDs.splice(this.anUsedIDs.indexOf(nTransID), 1);
    	}
	},
	AddAutoRetDirectMsgDesigns: function(strDesign) {
		
		if (this.IsConnected()) {
			
			if (typeof(strDesign) == 'string' && strDesign.trim() != "") {
			
				this.ClientMsgSend(['AddAutoRetDirectMsgDesigns', strDesign]);
			}
			else {

				LOGGER.Log('Adding designation of direct messages responses to automatically execute failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to add desingation of direct messages responses to automatically execute.');
		}
	},
	RemoveAutoRetDirectMsgDesigns: function(strDesign) {
		
		if (this.IsConnected()) {
			
			if (typeof(strDesign) == 'string' && strDesign.trim() != "") {
			
				this.ClientMsgSend(['RemoveAutoRetDirectMsgDesigns', strDesign]);
			}
			else {

				LOGGER.Log('Removing designation of direct messages responses to automatically execute failed, designation is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to remove desingation of direct messages responses to automatically execute.');
		}
	},
	SetServerMsgSeparatorChar: function(strMsgSeparatorChars) {
		
		if (this.IsConnected()) {
			
			if (typeof(strMsgSeparatorChars) == 'string' && strMsgSeparatorChars.trim() != "") {
			
				this.ClientMsgSend(['SetMsgPartIndicator', strMsgSeparatorChars]);
			}
			else {

				LOGGER.Log('Setting part separator character for server messages failed, message part characters is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set part separator character for server messages.');
		}
	},
	SetServerStartChars: function(strMsgStartChars) {
		
		if (this.IsConnected()) {
			
			if (typeof(strMsgStartChars) == 'string' && strMsgStartChars.trim() != "") {
			
				this.ClientMsgSend(['SetMsgStartIndicator', strMsgStartChars]);
			}
			else {

				LOGGER.Log('Setting start separator character for server messages failed, message start characters is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set start indicator character for server messages.');
		}
	},
	SetServerEndChars: function(strMsgEndChars) {
		
		if (this.IsConnected()) {
			
			if (typeof(strMsgEndChars) == 'string' && strMsgEndChars.trim() != "") {
			
				this.ClientMsgSend(['SetMsgEndIndicator', strMsgEndChars]);
			}
			else {

				LOGGER.Log('Setting end separator character for server messages failed, message end characters is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set end indicator character for server messages.');
		}
	},
	SetServerFillerChar: function(charMsgFillerChar) {
		
		if (this.IsConnected()) {
			
			if (typeof(charMsgFillerChar) == 'string' && charMsgFillerChar.trim() != "") {
			
				this.ClientMsgSend(['SetMsgFiller', charMsgFillerChar]);
			}
			else {

				LOGGER.Log('Setting filler character for server messages failed, message filler characters is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set filler character for server messages.');
		}
	},
	SetMsgIndicatorLen: function(nSetMsgLen) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nSetMsgLen) && nSetMsgLen) {
			
				this.ClientMsgSend(['SetMsgIndicatorLen', nSetMsgLen]);
			}
			else {

				LOGGER.Log('Setting number of indicator characters for server messages failed, length is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set number of indicator characters for server messages.');
		}
	},
	Disconnect: function() {
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['Close']);
			this.objWorker.terminate();
			this.objWorker = null;
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to disconnect from server.');
		}
	},
	SetQueueLimit: function(nNewLimit) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewLimit)) {
			
				this.ClientMsgSend(['SetQueueLimit', nNewLimit]);
			}
			else {

				LOGGER.Log('Setting limit to queue for server messages failed, limit is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set limit to queue for server messages.');
		}
	},
	SetMsgLateLimit: function(nNewLimit) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewLimit)) {
			
				this.ClientMsgSend(['SetMsgLateLimit', nNewLimit]);
			}
			else {

				LOGGER.Log('Setting time limit to queue for server messages failed, time limit is not set.');
			}
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set time limit for queued server messages.');
		}
	},
	SetDropLateMsgs: function(boolDrop) {
		
		if (this.IsConnected()) {
			
			this.ClientMsgSend(['SetDropLateMsgs', boolDrop]);
		}
		else {

			LOGGER.Log('Client web worker not setup, unable to set indicator to drop late server messages.');
		}
	},
	SetActivityCheckTimeLimit: function(nNewLimit) {
		
		if (this.IsConnected()) {

			if (Number.isInteger(nNewLimit)) {
			
				this.ClientMsgSend(['SetActivityCheckTimeLimit', nNewLimit]);
			}
			else {

				LOGGER.Log('Setting activity check time limit to queue for server messages failed, time limit is not set.');
			}
		}
		else {
 
			LOGGER.Log('Client web worker not setup, unable to set length of time before checking with server for messages.');
		}
	},
	AutoRetLimitInMillis: function(fSetAutoRetLimitInMillis) {
		
		if (this.IsConnected()) {

			if (typeof(fSetAutoRetLimitInMillis) == 'number') {
			
				this.ClientMsgSend(['AutoRetLimitInMillis', fSetAutoRetLimitInMillis]);
			}
			else {

				LOGGER.Log('Setting length of time to wait for data process or HTTP communcation response failed, wait time length is not set.');
			}
		}
		else {
 
			LOGGER.Log('Client web worker not setup, unable to set length of time to wait for data process or HTTP communcation response.');
		}
	},
	AutoRetProcessCmd: function(boolSetAutoRetProcessCmd) {
		
		if (this.IsConnected()) {

			if (typeof(boolSetAutoRetProcessCmd) == 'boolean') {
			
				this.ClientMsgSend(['AutoRetProcessCmd', boolSetAutoRetProcessCmd]);
			}
			else {

				LOGGER.Log('Setting identicator to auto-executed processed client messages failed, identicator is not set.');
			}
		}
		else {
 
			LOGGER.Log('Client web worker not setup, unable to set indicator to automatically execute responses to data processes or HTTP communcations.');
		}
	},
	AutoRetEndTrans: function(boolSetAutoRetEndTrans) {
		
		if (this.IsConnected()) {

			if (typeof(boolSetAutoRetEndTrans) == 'boolean') {
			
				this.ClientMsgSend(['AutoRetEndTrans', boolSetAutoRetEndTrans]);
			}
			else {

				LOGGER.Log('Setting identicator to remove auto-executed transactions failed, identicator is not set.');
			}
		}
		else {
 
			LOGGER.Log('Client web worker not setup, unable to set indicator to automatically close HTTP transmissions or delete data process after automated retrieval.');
		}
	},	
	SubmitMsg: function(strSubmitPage, 
						mxParams, 
						strFormName, 
						strMethod, 
						boolUseProxy, 
						boolSendAsync,
						funcRawProcess) {
		
		var strParamString = 'Date=' + new Date().getTime(),	
									/* String of Parameters to Send */
			nParamCount = 0,		/* Parameter Count List */
			boolSubmit = true,		/* Indicator That Message Can be Submitted */
			nCounter = 0;			/* Counter for Loop */
		
		if (JSON) {
		
			if (strMethod) {
				
				strMethod = strMethod.toUpperCase();
			}
			else {
				
				strMethod = 'POST';
			}
	
			switch (typeof(mxParams)) {
				
				case 'object': {
				
					for (var strKey in mxParams) {
						
						strParamString += '&' + encodeURIComponent(strKey) + "=" + encodeURIComponent(mxParams[strKey]);
					}
					
					break;
				}			
				case 'array': {
	
					nParamCount = mxParams.length;
					
					for (nCounter = 0; nCounter < nParamCount; nCounter++) {
						
						strParamString += '&' + nCounter.toString() + "=" + encodeURIComponent(mxParams[nCounter]);
					}
					
					break;
				}
				case 'string': {
	
					if (mxParams.indexOf('?') < 0) {
						
						if (mxParams != '') {
							
							strParamString += '&' + encodeURIComponent(mxParams);
						}
					}
					else {
	
						LOGGER.Log('Invalid submission parameter, submission failed.');
						boolSubmit = false;
					}
					
					break;
				}
				default: {
	
					LOGGER.Log('Invalid submission parameter, submission failed.');	
					boolSubmit = false;			
					break;
				}
			}
			
			if (typeof(strFormName) != 'string') {
				
				strFormName = '';
			}
			
			if (typeof(boolUseProxy) != 'boolean') {
				
				boolUseProxy = false;
			}
			
			if (typeof(boolSendAsync) != 'boolean') {
				
				boolSendAsync = false;
			}
			
			if (boolSubmit) {
	
				if (strMethod == 'GET') {
					
					strParamString = '?' + strParamString;
				}
				
				this.aQueue[this.aQueue.length] = {
													strForm: strFormName,
													strPage: strSubmitPage, 
													strParams: strParamString,
													strType: strMethod,
													boolProxy: boolUseProxy,
													boolAsync: boolSendAsync,
													funcProcess: funcRawProcess
												  };
			}
			
			/* If No Transmissions Are Being Sent, Send Current One */
			if (this.boolIdle) {
				
				this.SendMsg();
			}
		}
		else {
			
			LOGGER.Log('Browser does not support native JSON, submission failed.');	
		}
	},					/* Store Communication for Transmission */
	SendMsg: function() {
		
		var objAJAX = null;				/* AJAX Object */
/*		var aAJAXVersion = ['MSXML2.XmlHttp.6.0',
		                    'MSXML2.XmlHttp.5.0',
		                    'MSXML2.XmlHttp.4.0',
		                    'MSXML2.XmlHttp.3.0',
		                    'MSXML2.XmlHttp.2.0',
		                    'Microsoft.XmlHttp'];
										/* List of Alternative AJAX Object */
		/*var objSubmitInfo;			/* Object Containing Information for the Selected Submit Message */
		/*var strParameters = "";		/* Parameter String to Send to Submission Page */
		/*var nCounter = 0;				/* Counter for Loop */
		
		/* If Any Messages, Send the Next One */
		if (this.aQueue.length > 0) {
			
			this.boolIdle = false;
			
			if (typeof XMLHttpRequest !== 'undefined') {
				
				objAJAX = new XMLHttpRequest();
			}
			else {
				
				var aAJAXVersion = ['MSXML2.XmlHttp.6.0',
				                    'MSXML2.XmlHttp.5.0',
				                    'MSXML2.XmlHttp.4.0',
				                    'MSXML2.XmlHttp.3.0',
				                    'MSXML2.XmlHttp.2.0',
				                    'Microsoft.XmlHttp'];
				
				for (var nCounter = 0; nCounter < 6; nCounter++) {
					
					try {
						
						objAJAX = new ActiveXObject(aAJAXVersion[nCounter]);
						nCounter = 6;
					}
					catch(exError) {
						
						objAJAX = null;
					}
				}
			}
			
			if (objAJAX) {

				var objSubmitInfo = this.aQueue.shift();
				var strParameters = objSubmitInfo.strParams;
				
				if (objSubmitInfo.boolUseProxy) {
					
					strParameters += '&RevCommProxyPage=' + encodeURIComponent(objSubmitInfo.strPage);
					objSubmitInfo.strPage = 'RevCommProxy.php';
				}
				
				objAJAX.open(objSubmitInfo.strType, objSubmitInfo.strPage, objSubmitInfo.boolAsync);

				if (typeof(objSubmitInfo.funcProcess) == 'function') {
					
					objAJAX.onreadystatechange = objSubmitInfo.funcProcess;
				}
				else if (typeof(this.fnResponse) != 'function') {
					
					objAJAX.onreadystatechange = new Function("																												\
																																											\
						if (this.readyState == 4) {																															\
																																											\
							if (this.status == 200) {																														\
																																											\
								if (JSON) {																																	\
																																											\
									COMM.Process(this.responseText);																									\
								}																																			\
								else {																																		\
																																											\
									/* Error Occurred */																													\
									LOGGER.Log('Browser does not support native JSON, processing returned message failed.');												\
								} 																																			\
							}																																				\
							else {																																			\
																																											\
								LOGGER.Log('Error: ' + strRespJSONData);																									\
							}																																				\
										 																																	\
							var strFormName = '" + objSubmitInfo.strForm + "';																								\
								 	/* Name of Submitting Form */																											\
							/*var aFormLayers = document.querySelectorAll('form[name=\"' + strFormName + '\"] div.divCommFormButtonClass');									\
							 		/* Layer Containing Form Inputs */																										\
							/*var nFormLayerCount = aFormLayers.length;																										\
							 		/* Count of Layer Containing Form Inputs */																								\
							/*var aFormInputs = aFormLayers[nFormCounter].querySelectorAll('a[name=\"aCommFormLink\"], input[name=\"inpCommFormButton\"]');					\
							 		/* Form Inputs */																														\
							/*var nFormChildCount = aFormInputs.length;																										\
							 		/* Count of Form Child Element */																										\
							/*var objLayerParent; */																														\
							 		/* Form Layer Parent */																													\
							/*var aLayerWaitLayerList; */																													\
							 		/* Form Waiting Image Layer */																											\
							/*var nFormCounter = 0;																															\
							    	/* Counter for Layer Loop */																											\
							/*var nChildCounter = 0;																														\
							    /* Counter for Child Loop */																												\
							 																																				\
							if (strFormName != '') {																														\
																																											\
								var aFormLayers = document.querySelectorAll('form[name=\"' + strFormName + '\"] div.divCommFormButtonClass'),								\
									nFormLayerCount = 0,																													\
									aFormInputs,																															\
									nFormChildCount = 0,																													\
									nFormCounter = 0,																														\
									nChildCounter = 0;																														\
																																											\
								if (aFormLayers) {																															\
																																											\
									nFormLayerCount = aFormLayers.length;																									\
								}																																			\
																																											\
								for (nFormCounter = 0; nFormCounter < nFormLayerCount; nFormCounter++) {																	\
																																											\
									aFormInputs = aFormLayers[nFormCounter].querySelectorAll('a[name=\"aCommFormLink\"], input[name=\"inpCommFormButton\"]');				\
																																											\
									if (aFormInputs) {																														\
																																											\
										nFormChildCount = aFormInputs.length;																								\
																																											\
										for (nChildCounter = 0; nChildCounter < nFormChildCount; nChildCounter++) {															\
																																											\
											aFormInputs[nChildCounter].removeAttribute('disabled');																			\
										}																																	\
									}																																		\
																																											\
									/* Re-Enable Origination Form Submit Button, Hide Wait Button */																		\
									objLayerParent = aFormLayers[nFormCounter].parentNode;																					\
																																											\
									if (objLayerParent) {																													\
																																											\
										aLayerWaitLayerList = objLayerParent.querySelectorAll('div.divCommFormWaitImgClass');												\
																																											\
										if (aLayerWaitLayerList) {																											\
																																											\
											nFormChildCount = aLayerWaitLayerList.length;																					\
																																											\
											for (nChildCounter = 0; nChildCounter < nFormChildCount; nChildCounter++) {														\
																																											\
												aLayerWaitLayerList[nChildCounter].style.display = 'none';																	\
											}																																\
										}																																	\
									}																																		\
								}																																			\
							}																																				\
						}																																					\
																																											\
						COMM.Idle(true);																																	\
						COMM.SendMsg();");
				}
				else {

					objAJAX.onreadystatechange = function() {
						
						if (this.readyState == 4) {																															
																																											
							if (this.status == 200) {
								
								if (typeof(COMM.fnResponse) == 'function') {

									COMM.fnResponse.call(this, this.responseText);
								}
								else { 
																													
									LOGGER.Log('Invalid response function, calling response function failed');	
								}
							}
							else if (typeof(COMM.fnError) == 'function') {

								COMM.fnError.call(this, this.responseText);
							}
							else {

								LOGGER.Log('Error: ' + this.responseText);
							}

							COMM.Idle(true);
							COMM.SendMsg();
						}
					}
				}
				
				objAJAX.send();
			}
			else {

				LOGGER.Log('Creating AJAX object failed, send failed.');	
			}
		}
	},					/* Transmit Stored Communications */
	RegisterObject: function(strDesignation, objObject) {
		
		var boolObjRegistered = false;
									/* Indicator That the Object was Registered */
		
		if (typeof(objObject) == 'object') {
			
			this.aObjReg[strDesignation] = objObject;
			boolObjRegistered = true;
		}
		
		/* Return True If Object was Registered, Else False */
		return boolObjRegistered;
	},
	SetResponseFunc: function(fnSet) {
		
		if (typeof(fnSet) == 'function') {
			
			this.fnResponse = fnSet;
		}
		else {
				
			this.fnResponse = null;
		}
	},
	SetErrorFunc: function(fnSet) {
		
		if (typeof(fnSet) == 'function') {
			
			this.fnError = fnSet;
		}
		else {
				
			this.fnError = null;
		}
	},
	SetMsgProcessFunc: function(fnSet) {
		
		if (typeof(fnSet) == 'function') {
			
			this.Process = fnSet;
		}
		else {
				
			this.Process = function(mxJSONData) {

				/*var aRespMsgs; 			/* Information on Response Messages from Processing Page */																			
				/*var nMsgTotal = 0;		/* Count of Response Messages */																									
				/*var nMsgCounter = 0;		/* Counter for Messages Loop */																										
				/*var aMsgSelect;			/* Selected Message Information */																										
				/*var strMsgDesign = '';	/* The Designation of the Selected Message */																							
				/*var aInfoSelect;			/* Selected Information List */																										
				/*var nInfoTotal = 0;		/* Count of Response Messages Selected Information List */																				
				/*var nCounter = 0;			/* Counter for Loop */		
						
				if (mxJSONData && (typeof(mxJSONData) == 'string' && mxJSONData.trim() != '')) {																															
					
					if (JSON) {
						
						try {																																
						
							var aRespMsgs = mxJSONData,																					
								nMsgTotal = 0,		
								aRegister = this.aObjReg,
								aMsgSelect,																													
								aInfoSelect,		
								objRegSelect,			
								objProcessSelect,																							
								nInfoTotal = 0,																												
								nMsgCounter = 0,																											
								nCounter = 0;	
																																		
							if (this.boolDebug) { 																								
		
								if (typeof(mxJSONData) == 'string') {
	
									LOGGER.Log('Processed: ' + mxJSONData);
								}
								else {
									
									LOGGER.Log('Processed: ' + JSON.stringify(mxJSONData));
								}																						
							}	
							
							if (typeof(aRespMsgs) == 'string') {
								
								aRespMsgs = JSON.parse(aRespMsgs);
							}
							
							nMsgTotal = aRespMsgs.length;																								
	
							/* Process Messages */																											
							for (nMsgCounter = 0; nMsgCounter < nMsgTotal; nMsgCounter++) {																	
								
								/* Get Selected Message's Information */																					
								aMsgSelect = aRespMsgs[nMsgCounter];	

								objRegSelect = aRegister[aMsgSelect.DESIGNATION];
								
								if (!objRegSelect) {
								
									objRegSelect = aRegister['GLOBAL'];	
								}																		
						
								/* Get and Process Selected Message's Variable Updates */																	
								aInfoSelect = aMsgSelect.VARUPDATES;
								
								if (aInfoSelect) {
																			
									nInfoTotal = aInfoSelect.length;																				
							
									for (nCounter = 0; nCounter < nInfoTotal; nCounter++) {		
										
										objProcessSelect = aInfoSelect[nCounter];
						
										if (objRegSelect.hasOwnProperty(objProcessSelect.NAME)) {
											
											/* Do a Designated Object Variable Update */																			
											objRegSelect[objProcessSelect.NAME] = objProcessSelect.VALUE;
										}									
									}
								}																														
						
								/* Get and Process Selected Message's Function Calls */																		
								aInfoSelect = aMsgSelect.FUNCCALLS;	
								
								if (aInfoSelect) {			
																												
									nInfoTotal = aInfoSelect.length;																							
							
									for (nCounter = 0; nCounter < nInfoTotal; nCounter++) {		
																		
										objProcessSelect = aInfoSelect[nCounter];

										if (objRegSelect.hasOwnProperty(objProcessSelect.NAME)) {
										
											/* Do a Global Function Call */																							
											objRegSelect[objProcessSelect.NAME].apply(objRegSelect, objProcessSelect.PARAMS);
										}	
									}	
								}	
							}																																
						}																																	
						catch (exError) {																													
						
							/* Error Occurred */																											
							LOGGER.Log('Error: Processing returned message failed. Exception: ' + exError.message);											
						}	
					}
					else {
						
						LOGGER.Log('Browser does not support native JSON, processing returned message failed.');	
					}																																
				}
			}
		}
	},
	SetWaitImg: function(strImgPath, strAltMsg) {
		
		var aWaitImg = document.querySelectorAll('div.divCommFormWaitImgClass img[name="imgCommFormWaitImg"]'),
							/* Waiting Images */
			nWaitImgCount = aWaitImg.length,
							/* Count of Waiting Images */
			nCounter = 0;	/* Counter for Loop */
		
		/* Set the New Image Path, and Alternative Message If Sent */
		for (nCounter = 0; nCounter < nWaitImgCount; nCounter++) {
			
			aWaitImg[nCounter].attr('src', strImgPath);
			
			if (strAltMsg) {

				aWaitImg[nCounter].attr('alt', strAltMsg);
			}
		}
	},
	StoreReceivedMsg: function(mxMsg) {

		if (JSON) {
			
			if (typeof(mxMsg) == 'string') {
				
				mxMsg = JSON.parse(mxMsg);
			}		
		
			if (mxMsg.hasOwnProperty("AUTOPROCESS") && 
				mxMsg.AUTOPROCESS) {
	
				this.Process(mxMsg.MESSAGE);
							
				if (mxMsg.hasOwnProperty("TYPE") && 
					mxMsg.hasOwnProperty("TRANSID")) {
	
					this.RunSendFuncs(mxMsg.TYPE, mxMsg.TRANSID, mxMsg.MESSAGE);
				}
			}	
		
			if (mxMsg.hasOwnProperty("AUTODELETE") && 
				mxMsg.AUTODELETE) {
							
				if (mxMsg.hasOwnProperty("TYPE") && 
					mxMsg.hasOwnProperty("TRANSID")) {
					
					this.TranClose(mxMsg.TRANSID, true);
				}
				else {
	
					LOGGER.Log('During storing received message, deletion failed due to missing type or transaction ID.');
				}				
			}
			
			if (mxMsg.TYPE != "ERROR") {

				this.aReceivedMsgs.push(mxMsg);
			}
			else {
				
				LOGGER.Log(mxMsg.MESSAGE, true);
			}
		}
		else {
			
			LOGGER.Log('Browser does not support native JSON, storing received message failed.');
		}
	},
	GetInputNameID: function(objInput) {
		
		var strInputName = objInput.getAttribute('id');
							/* Name of the Input */
		
		if (!strInputName) {
			
			strInputName = objInput.getAttribute('name');
		}
		
		return strInputName;
	},
	Idle: function(boolIsIdle) {
          
		if (typeof(boolIsIdle) == 'boolean') {
			
			if (boolIsIdle) {
				
				this.boolIdle = true;
			}
			else {
				
				this.boolIdle = false;
			}
		}
		
		return this.boolIdle;
	},
	IsConnected: function() {
          
		return (this.objWorker !== null && typeof(this.objWorker) !== 'undefined');
	},
	ArrayHasObject: function(mxCheck) {
       
		var nCheckLen = 0,			/* Length of the Array to Check */
        	boolHasObject = false, 	/* Indicator That Array Has Object */
			nCounter = 0;			/* Counter for Loop */
         
         if (mxCheck && Array.isArray(mxCheck)) {

        	 nCheckLen = mxCheck.length;
        	 
        	 for (nCounter = 0; nCounter < nCheckLen && !boolHasObject; nCounter++) {
               
        		 if (typeof(mxCheck[nCounter]) == 'object' && 
        			 Array.isArray(mxCheck[nCounter])) {
                      
        			 boolHasObject = true;
        		 }
              }           
		}
              
         return boolHasObject;
	},
	ValidateTransID: function(nNewTransID) {

      return this.objSendStorage['STREAMCLIENT'][nNewTransID] || 
    		 this.objSendStorage['STREAMRAW'][nNewTransID] ||
    		 this.objSendStorage['HTTP'][nNewTransID] ||
    		 this.objSendStorage['DATAPROCESS'][nNewTransID];

	},
	DebugActivate: function(boolOn) {
          
		if (boolOn) {

			this.boolDebug = true;
		}
		else {
		
			this.boolDebug = false;
		}
		
		this.ClientMsgSend(['DebugActivate', this.boolDebug]);
	},
	DebugMode: function() {
          
        return this.boolDebug;
	}
};

/* Error Processing Object */
var LOGGER = {

	boolShowAll: false,				/* Indicator to Show All Errors */ 
	Log: function(strMessage, boolShowError) {
		
		if (this.boolShowAll && boolShowError) {
		
			var aErrorLayer = document.querySelectorAll('div.divErrorMsgClass'),
										/* Layer for Containing Error Message */
				nErrorLayerCount = aErrorLayer.length,
										/* Count of Error Layers */
	/*			objLayer = document.createElement('div'),
										/* Created Error Layer */
	/*			objBody = document.body,/* Document's Body */
				nCounter = 0;			/* Counter for Loop */
			
			if (nErrorLayerCount) {
				
				for (nCounter = 0; nCounter < nErrorLayerCount; nCounter++) {
				
					aErrorLayer[nCounter].innerHTML = strMessage;
				}
			}
			else {
				
				var objLayer = document.createElement('div'),
					objBody = document.body;
				
				objLayer.className = 'divErrorMsgClass';
				objLayer.appendChild(document.createTextNode(strMessage));
				
				if (objBody.firstChild) {
					
					objBody.insertBefore(objLayer, objBody.firstChild);
				}
				else {
	
					objBody.appendChild(objLayer);
				}
			}
		}
		
		if (console) {
			
			console.log(strMessage);
		}
		
		if (boolShowError) {
			
			alert(strMessage);
		}
	},
	Suppress: function(boolSuppressErrors) {
		
		if (boolSuppressErrors) {
			
			this.boolShowAll = false;
		}
		else {
			
			this.boolShowAll = true;
		}
	}
};

COMM.Init();