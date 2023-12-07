# RevCommProcessor.rb - Client Communication Processing Script for RevCommSuite API
#
# MIT License
#
# Copyright (c) 2023 RevComGaming
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

module RevCommProcessor
  
     private 
     
          BUFFERSIZE = 8192              # Size of Message Buffer
          MAXBUFFERSIZEDIGITS = 10       # Maximum Number of Digits in Buffer Size
          ENCRYPTKEYSIZE = 32            # Size of Encryption Key
          ENCRYPTIVBLOCKSIZE = 16        # Size of Encryption IV Block
          @thdCommunicate = nil          # Communication Thread
          @thdPeerToPeerCommunicate = nil# "Peer To Peer" Communication Listener Thread
          @thdAysncDataProcessor = nil   # Processing Asynchronous Data Processing Returns Thread
          @mtxLock = Mutex.new           # Lock for Thread
          @w32StartStream = Win32API.new('RevCommClient32', 'StartStream', 'IPI', 'V')
                                         # Starts Stream 
          @w32StartHTTPPostAsyncWithHostPort = Win32API.new('RevCommClient32', 'StartHTTPPostAsyncWithHostPort', 'IPI', 'V')
                                         # Starts Setup to Send Asynchronous HTTP POST Messages 
          @w32StartHTTPPostAsyncWithHost = Win32API.new('RevCommClient32', 'StartHTTPPostAsyncWithHost', 'IP', 'V')
                                         # Starts Setup to Send Asynchronous HTTP POST Messages Through Server Post 80
          @w32StartHTTPPostSyncWithHostPort = Win32API.new('RevCommClient32', 'StartHTTPPostSyncWithHostPort', 'IPI', 'V')
                                         # Starts Setup to Send Synchronous HTTP POST Messages 
          @w32StartHTTPPostSyncWithHost = Win32API.new('RevCommClient32', 'StartHTTPPostSyncWithHost', 'IP', 'V')
                                         # Starts Setup to Send Synchronous HTTP POST Messages Through Server Post 80 
          @w32StartHTTPGetASyncWithHostPort = Win32API.new('RevCommClient32', 'StartHTTPGetASyncWithHostPort', 'IPI', 'V')
                                         # Starts Setup to Send Synchronous HTTP POST Messages Through Server Post 80 
          @w32StartHTTPGetASyncWithHost = Win32API.new('RevCommClient32', 'StartHTTPGetASyncWithHost', 'IP', 'V')
                                         # Starts Setup to Send Asynchronous HTTP GET Messages Through Server Default Port
          @w32StartHTTPGetSyncWithHostPort = Win32API.new('RevCommClient32', 'StartHTTPGetSyncWithHostPort', 'IPI', 'V')
                                         # Starts Setup to Send Synchronous HTTP GET Messages
          @w32StartHTTPGetSyncWithHost = Win32API.new('RevCommClient32', 'StartHTTPGetSyncWithHost', 'IP', 'V')
                                         # Starts Setup to Send Synchronous HTTP GET Messages Through Server Default Post
          @w32SendDirectMsg = Win32API.new('RevCommClient32', 'SendDirectMsg', 'P', 'V')
                                         # Sends Direct Messages to Server
          @w32SendDirectMsgWithDesign = Win32API.new('RevCommClient32', 'SendDirectMsgWithDesign', 'PP', 'V')
                                         # Sends Direct Messages with Designation to Server 
          @w32SendDirectMsgPeerToPeer = Win32API.new('RevCommClient32', 'SendDirectMsgPeerToPeer', 'P', 'V')
                                         # Sends Direct Messages to "Peer To Peer" Clients
          @w32SendDirectMsgPeerToPeerWithDesign = Win32API.new('RevCommClient32', 'SendDirectMsgPeerToPeerWithDesign', 'PP', 'V')
                                         # Sends Direct Messages with Designation to "Peer To Peer" Clients
          @w32HasPeerToPeerClients = Win32API.new('RevCommClient32', 'HasPeerToPeerClients', 'V', 'I')
                                         # Finds If "Peer To Peer" Clients are Connected
          @w32AddStreamMsg = Win32API.new('RevCommClient32', 'AddStreamMsg', 'IP', 'V')
                                         # Adds to Queue of Messages to be Sent Through Stream (Will Not Store Messages That Have Stream Reserved Characters)
          @w32RegisterDataProcess = Win32API.new('RevCommClient32', 'RegisterDataProcess', 'IP', 'V')
                                         # Register Data Process for Execution
          @w32RegisterDataProcessWithParams = Win32API.new('RevCommClient32', 'RegisterDataProcessWithParams', 'IPPP', 'V')
                                         # Register Data Process for Execution with Parameters or Add Parameters to Existing One
          @w32SendDataProcess = Win32API.new('RevCommClient32', 'SendDataProcess', 'IIPI', 'V')
                                         # Sends Data Process Execution Message
          @w32GetDataProcessResponse = Win32API.new('RevCommClient32', 'GetDataProcessResponse', 'IIPP', 'V')
                                         # Gets Response Returned from Data Process Execution Message, Before Deleting it from Response Queue and
                                         # Outputs Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */
          @w32CheckDataProcessResponse = Win32API.new('RevCommClient32', 'CheckDataProcessResponse', 'II', 'I')
                                         # Check If Response Returned from Data Process Execution Message, Before Deleting it from Response Queue and
                                         # Outputs Length of Response Message Indicated by the Response ID and Communication Transmission ID If Exists or Has Values, Else Returns 0 */
          @w32SendHTTP = Win32API.new('RevCommClient32', 'SendHTTP', 'II', 'V')
                                         # Send Stored HTTP Message
          @w32GetHTTPResponse = Win32API.new('RevCommClient32', 'GetHTTPResponse', 'IIPP', 'V')
                                         # Gets Response Returned from Send Message, Before Deleting it from Response Queue and
                                         # Outputs Response Message as a String Indicated by the Response ID and Communication Transmission ID is Valid, Else Blank String */
          @w32CheckHTTPResponse = Win32API.new('RevCommClient32', 'CheckHTTPResponse', 'II', 'I')
                                         # Check If Response Returned from Send Message, Before Deleting it from Response Queue and
                                         # Outputs Length of Response Message Indicated by the Response ID and Communication Transmission ID If Exists or Has Values, Else Returns 0 */
          @w32GetStreamMsg = Win32API.new('RevCommClient32', 'GetStreamMsg', 'IPP', 'V')
                                         # Gets a Waiting Message from Stream, Before Deleting it from the Wait Queue and
                                         # Outputs Message as a String If It Exists, Else Blank String */
          @w32GetStreamMsgNext = Win32API.new('RevCommClient32', 'GetStreamMsgNext', 'PP', 'V')
                                         # Gets Next Waiting Message from Any Stream, Before Deleting it from the Wait Queue and
                                         # Outputs Message as a String If It Exists, Else Blank String */
          @w32CheckStreamMsgReady = Win32API.new('RevCommClient32', 'CheckStreamMsgReady', 'V', 'I')
                                        # Checks for Stream Messages on Client
          @w32CheckStreamMsgByIDReady = Win32API.new('RevCommClient32', 'CheckStreamMsgByIDReady', 'I', 'I')
                                        # Checks for Stream Messages with Specified Designation on Client
          @w32ClearStreamMsgs = Win32API.new('RevCommClient32', 'ClearStreamMsgs', 'V', 'I')
                                        # Clears Stream Messages on Client
          @w32ClearStreamMsgsByIDReady = Win32API.new('RevCommClient32', 'ClearStreamMsgsByIDReady', 'I', 'I')
                                        # Sends Stream Messages with Designation on Client 
          @w32GetDirectMsg = Win32API.new('RevCommClient32', 'GetDirectMsg', 'PPP', 'V')
                                        # Gets a Specified Waiting Direct Message, Before Deleting it from the Wait Queue and
                                        # Outputs Message as a String If It Exists, Else Blank String */
          @w32GetDirectMsgNext = Win32API.new('RevCommClient32', 'GetDirectMsgNext', 'PP', 'V')
                                        # Gets a Waiting Direct Message, Before Deleting it from the Wait Queue and
                                        # Outputs Message as a String If It Exists, Else Blank String */
          @w32CheckDirectMsgsReady = Win32API.new('RevCommClient32', 'CheckDirectMsgsReady', 'V', 'I')
                                        # Checks for Direct Messages on Client
          @w32CheckDirectMsgsWithDesignReady = Win32API.new('RevCommClient32', 'CheckDirectMsgsWithDesignReady', 'P', 'I')
                                        # Checks for Direct Messages with Specified Designation on Client
          @w32ClearDirectMsgs = Win32API.new('RevCommClient32', 'ClearDirectMsgs', 'V', 'I')
                                        # Clears Direct Messages on Client
          @w32ClearDirectMsgsWithDesign = Win32API.new('RevCommClient32', 'ClearDirectMsgsWithDesign', 'P', 'I')
                                        # Sends Direct Messages with Designation on Client
          @w32GetStreamFile = Win32API.new('RevCommClient32', 'GetStreamFile', 'P', 'V')
                                         # Gets Downloaded File from Stream and Outputs File to Destination Directory, Before File is Removed
          @w32CheckStreamFileDownload = Win32API.new('RevCommClient32', 'CheckStreamFileDownload', 'PPIP', 'I')
                                         # Gets If File was Downloaded from Stream and Returns It, Before File is Removed
          @w32CheckStreamFileReady = Win32API.new('RevCommClient32', 'CheckStreamFileReady', 'P', 'I')
                                         # Finds If File is Ready to be Downloaded from Stream and Returns File Length If Found
          @w32GetStreamFilePathLength = Win32API.new('RevCommClient32', 'GetStreamFilePathLength', 'P', 'I')
                                         # Finds Length of File's Path and Name for Retrieval from File Information If File is in Stream
          @w32ClearStreamFileDownload = Win32API.new('RevCommClient32', 'ClearStreamFileDownload', 'P', 'V')
                                         # Removes a Downloaded File from Stream
          @w32GetStreamFileList = Win32API.new('RevCommClient32', 'GetStreamFileList', 'PP', 'V')
                                         # Gets List of Downloadable Files */
          @w32SetHTTPProcessPage = Win32API.new('RevCommClient32', 'SetHTTPProcessPage', 'IP', 'V')
                                         # Sets Processing Page for HTTP POST and GET Transmissions 
          @w32AddHTTPMsgData = Win32API.new('RevCommClient32', 'AddHTTPMsgData', 'IPP', 'V')
                                         # Adds a Variable and its Value to the Next Message Being Sent Through HTTP Transmission */
          @w32ClearHTTPMsgData = Win32API.new('RevCommClient32', 'ClearHTTPMsgData', 'I', 'V')
                                         # Clears Next Message Being Sent Through HTTP Transmission 
          @w32UseHTTPSSL = Win32API.new('RevCommClient32', 'UseHTTPSSL', 'II', 'V')
                                         # Sends Indicator Message to Use SSL for HTTP Message
          @w32Close = Win32API.new('RevCommClient32', 'Close', 'I', 'V')
                                         # Closes Specified Transmission 
          @w32GetLogError = Win32API.new('RevCommClient32', 'GetLogError', 'PP', 'V')
                                         # Gets Log Error Message Before Clearing its Information and
                                         # Outputs Error Message as a String If It Exists, Else Blank String 
          @w32GetDisplayError = Win32API.new('RevCommClient32', 'GetDisplayError', 'PP', 'V')
                                         # Gets Log Error Message Before Clearing its Information and 
                                         # Outputs Error Message as a String If It Exists, Else Blank String 
          @w32SetStreamMsgSeparator = Win32API.new('RevCommClient32', 'SetStreamMsgSeparator', 'IP', 'V')
                                         # Sets Message Part Character for Stream (Defaults to '|||')
          @w32SetStreamMsgEnd = Win32API.new('RevCommClient32', 'SetStreamMsgEnd', 'IP', 'V')
                                         # Sets Message End Character for Stream (Defaults to '|||*')
          @w32SetStreamMsgFiller = Win32API.new('RevCommClient32', 'SetStreamMsgFiller', 'IP', 'V')
                                         # Sets Message Filler Character for Stream (Defaults to '\0') 
          @w32Disconnect = Win32API.new('RevCommClient32', 'Disconnect', 'V', 'V')
                                         # Disconnects Client from Server
          @w32DisconnectPeerToPeer = Win32API.new('RevCommClient32', 'DisconnectPeerToPeer', 'V', 'V')
                                         # Disconnects "Peer To Peer" Server and Clients
          @w32SetMsgPartIndicator = Win32API.new('RevCommClient32', 'SetMsgPartIndicator', 'P', 'I')
                                         # Sets Message Part Character for Server Messages (Defaults to '||||')
          @w32SetMsgStartIndicator = Win32API.new('RevCommClient32', 'SetMsgStartIndicator', 'P', 'I')
                                         # Sets Message Start Character for Server (Defaults to '*|||')
          @w32SetMsgEndIndicator = Win32API.new('RevCommClient32', 'SetMsgEndIndicator', 'P', 'I')
                                         # Sets Message End Character for Server (Defaults to '|||*')
          @w32SetMsgFiller = Win32API.new('RevCommClient32', 'SetMsgFiller', 'P', 'V')
                                         # Sets Message Filler Character for Server Messages (Defaults to '\0')
          @w32SetMsgIndicatorLen = Win32API.new('RevCommClient32', 'SetMsgIndicatorLen', 'P', 'I')
                                        # Sets Message Indicator Character Length for Server (Defaults is 4)
          @w32IsConnected = Win32API.new('RevCommClient32', 'IsConnected', 'V', 'I')
                                         # Finds If Client is Connected to Server
#          @w32Debug = Win32API.new('RevCommClient32', 'Debug', 'P', 'I')
          @w32DebugReceived = Win32API.new('RevCommClient32', 'DebugReceived', 'IPP', 'V')
                                        # Debug for Getting Message from Receiving Queue by Message Index
          @w32DebugToSend = Win32API.new('RevCommClient32', 'DebugToSend', 'IPP', 'V')
                                        # Debug for Getting Message from Sending Queue by Message Index
          @w32DebugReceivedStored = Win32API.new('RevCommClient32', 'DebugReceivedStored', 'IPP', 'V')
                                        # Debug for Getting Message from Receiving Stored Queue by Message Index
          @w32DebugToSendStored = Win32API.new('RevCommClient32', 'DebugToSendStored', 'IPP', 'V')
                                        # Debug for Getting Message from Sending Stored Queue by Message Index
          @w32DebugSendQueueCount = Win32API.new('RevCommClient32', 'DebugSendQueueCount', 'V', 'I')
                                        # Debug for Count of Message in Sending Queue
          @w32DebugReceivedQueueCount = Win32API.new('RevCommClient32', 'DebugReceivedQueueCount', 'V', 'I')
                                         # Debug for Count of Message in Receiving Queue
          @w32DebugSendStoredQueueCount = Win32API.new('RevCommClient32', 'DebugSendStoredQueueCount', 'V', 'I')
                                        # Debug for Count of Message in Sending Stored Queue
          @w32DebugReceivedStoredQueueCount = Win32API.new('RevCommClient32', 'DebugReceivedStoredQueueCount', 'V', 'I')
                                         # Debug for Count of Message in Receiving Stored Queue
          @w32DebugSendMsgLength = Win32API.new('RevCommClient32', 'DebugSendMsgLength', 'I', 'I')
                                         # Debug for Length of Selected Message in Sending Queue
          @w32DebugReceivedMsgLength = Win32API.new('RevCommClient32', 'DebugReceivedMsgLength', 'I', 'I')
                                         # Debug for Length of Selected Message in Receiving Queue
          @w32DebugSendStoredMsgLength = Win32API.new('RevCommClient32', 'DebugSendStoredMsgLength', 'I', 'I')
                                         # Debug for Length of Selected Message in Sending Stored Queue
          @w32DebugReceivedStoredMsgLength = Win32API.new('RevCommClient32', 'DebugReceivedStoredMsgLength', 'I', 'I')
                                         # Debug for Length of Selected Message in Receiving Stored Queue
          @w32SetQueueLimit = Win32API.new('RevCommClient32', 'SetQueueLimit', 'I', 'V')
                                         # Sets Length of Message Queue
          @w32SetMsgLateLimit = Win32API.new('RevCommClient32', 'SetMsgLateLimit', 'I', 'V')
                                         # Sets Message Viability Time Limit in Milliseconds
          @w32SetDropLateMsgs = Win32API.new('RevCommClient32', 'SetDropLateMsgs', 'I', 'V')
                                         # Sets Indicator to Drop Late Messages
          @w32SetActivityCheckTimeLimit = Win32API.new('RevCommClient32', 'SetActivityCheckTimeLimit', 'I', 'V')
                                         # Sets Length of Time Between Receipt of Clients and/or Server Messages Before Check is Send
          @w32IsInSessionGroup = Win32API.new('RevCommClient32', 'IsInSessionGroup', 'V', 'I')
                                         # Gets Indicator That User is In Server Group Session 
          @w32IsSessionGroupHost = Win32API.new('RevCommClient32', 'IsSessionGroupHost', 'V', 'I')
                                         # Gets Indicator That User is Host of Server Group Session 
          @hhRegObjects = Hash.new       # Holder for Registered Objects That Can be Updated Through Communications
          @hhSendStorage = {'HTTP' => [], 'STREAMCLIENT' => {}, 'STREAMRAW' => [], 'DIRECTCLIENT' => {}, 'DATAPROCESS' => {}}
                                         # Storage for Sending Messages to Server
          @hhSendFuncs = {'HTTP' => {}, 'DATAPROCESS' => {}, 'DIRECTCLIENT' => []}
                                         # Storage for Functions to Call When Receiving Response Messages to Server
          @hhReceivers = {'HTTP' => {}, 'DATAPROCESS' => {}}              
                                         # Holds Threads for Receiving Responses for HTTP Transmissions and Data Processes
          @hhSharedObjects = {}          # Objects Shared Between Session Group Members
          @hhDataMaps = {}               # Data Map Information
          @anUsedIDs = []                # Used IDs
          @astrAutoRetDirectMsgDesigns = []
                                         # List of Direct Message Designations to do Auto Retrieval for
          @ahHashUpdates = []            # List of Hashs to Update
          @fAutoRetLimitInMillis = 1000  # Automated Retrieval for HTTP Transmission or Data Process Time Limit in Milliseconds
          @boolAutoRetProcessCmd = false # Indicator to Process Automated Retrieval Client Messages
          @boolAutoRetEndTrans = false   # Indicator to Close HTTP Transmission or Delete Data Process After Automated Retrieval  
          @boolDebug = false             # Indicator To Do Debugging
     
          # Processes Incoming Communications 
          def self.ProcessIncoming(strJSONMsg) 
          
#               aUpdateSets = nil    # List of Updates Parsed from Communications
#               strDesignation = ""  # Designation of Object or Function for Selected Update
#               objRegObjSelect = nil# Selected Registered Object to be Updated
#               aObjUpdates = nil    # List of Updates for Designated Objects
#               aMethodUpdates = nil # List of Updates for Designated Method Calls
               
               begin
                    
                    if @boolDebug == true
                    
                         Log("Processed: " + strJSONMsg)
                    end
                    
                    if (strJSONMsg != "") 
                         
                         aUpdateSets = JSONConvert.Decode(strJSONMsg)    
                         strDesignation = "" 
                         objRegObjSelect = nil
                         
                         # Go Through Each Set of Updates
                         aUpdateSets.each { |hhUpdateSelect|
                         
                              # Get Previously Registered Object for Selected Update, and Get List of its Variables and Functions to Update
                              if @hhRegObjects.has_key?(hhUpdateSelect["DESIGNATION"]) == true
                              
                                   objRegObjSelect = @hhRegObjects[hhUpdateSelect["DESIGNATION"]]
                                   
                              else 
                         
                                   objRegObjSelect = @hhRegObjects["GLOBAL"]
                              
                              end
                              
                              if hhUpdateSelect.has_key?("VARUPDATES") == true
                              
                                   # Do Updates for Object's Variables
                                   hhUpdateSelect["VARUPDATES"].each { |hhUpdateInstr| 
                                        
                                        if objRegObjSelect.instance_variable_defined?("@" + hhUpdateInstr["NAME"]) == true
                                        
                                             objRegObjSelect.instance_variable_set("@" + hhUpdateInstr["NAME"], hhUpdateInstr["VALUE"])
                                        
                                        end
                                   }
                              
                              end
                         
                              if hhUpdateSelect.has_key?("FUNCCALLS") == true
                              
                                   # Do Updates for Object's Method Calls
                                   hhUpdateSelect["FUNCCALLS"].each { |hhUpdateInstr|
                                   
                                        if objRegObjSelect.respond_to?(hhUpdateInstr["NAME"], true) == true
                                        
                                             objRegObjSelect.method(hhUpdateInstr["NAME"]).call(*hhUpdateInstr["PARAMS"])
                                        
                                        end
                                   }
                              
                              end
                         }
                    
                    end

               rescue Exception => exError 
               
                    Log('Processing incoming messages failed. Exception: ' + exError.message + '. Backtrace: ' + exError.backtrace.join(' || '), true)
                              
               end
          
          end
          
          def self.ReceiveSharedObject(strObject)
               
               objRestore = Marshal.load(strObject)
               RegisterObject("SHAREDOBJ" + objRestore["ID"], objRestore["OBJECT"])
          end
          
          def self.GetSharedObjectsDataByClassName(strClassName, boolExcludeHostOnly = true)             
            
              strClassName = strClassName.downcase()
           
              return @hhSharedObjects.select { |objSelect|
             
                         objSelect["CLASS"].downcase() == strClassName &&
                         ((boolExcludeHostOnly == true && objSelect["HOST_ONLY"] == false) || 
                          boolExcludeHostOnly == false)
                     }
          end
          
          # Setup Shared Objects
          def self.SetupSharedObject(objSubject, strSubjectID, boolIsForHost = false, boolForHostOnly = false, strAddToSetup = "")
                  
  #             strMethod = ""
                    
               objSubject.methods(true).each { |strRawMethodInfo|
                         
                     strMethod = strRawMethodInfo.to_s()
                     
                     if strMethod != "nil?"

                          objSubject.instance_eval("alias revcom_" + strMethod + " " + strMethod + " ")
                               
                          strParamsList = (objSubject.method(strMethod).parameters.map { |aParams| aParams[1].to_s }).join(", ")
                         
                          if boolIsForHost == false
                                    
                              if boolHostOnly == true
                                   
                                   strParamsList += ", true"
                              end
                              
                               objSubject.instance_eval("def " + strMethod + "(" + strParamsList + ") 
                                   
                                                            if " + boolForHostOnly.to_s() + " == false || 
                                                               RevCommProcessor.IsSessionGroupHost() == true
                                                                 
                                                                 if @strTopLvlMethod == ''
                                   
                                                                      @strTopLvlMethod = '" + strMethod + "'
                                                                      RevCommProcessor.DirectClientMsgAddFuncCall('SHAREDOBJ" + strSubjectID + "', '" +
                                                                                                                  strMethod + "',
                                                                                                                  [" + strParamsList + "])
                                                                      RevCommProcessor.SendDirectClientMsg()
                                                                                                                   
                                                                 end
                                   
                                                                 revcom_" + strMethod + "(" + strParamsList + ")
          
                                                                 if @strTopLvlMethod == '" + strMethod + "'
                                                                 
                                                                      @strTopLvlMethod = ''
                                                                 end
                                                            end
                                                         end")
                          else
                               
                              objSubject.instance_eval("def " + strMethod + "(" + strParamsList + ", boolFromHost = false) 
                                    
                                                             if boolFromHost == false
          
                                                                 RevCommProcessor.DirectClientMsgAddFuncCall(" + strSubjectID + ", '" +
                                                                                                             strMethod + "',
                                                                                                             [" + strParamsList + "])
                                                                 RevCommProcessor.SendDirectClientMsg()
                                                                                                             
                                                             else
                                    
                                                                 revcom_" + strMethod + "(" + strParamsList + ")
                                                             end
                                                        end")
                          end
                     else
                          
                          break;
                     end
               }
          end

          # Starts Stream Registered to Transaction ID Using Hostname/IP and Port
          def self.StartStream(nNewTransID, strHostNameIP, nPort)

               if IsConnected() == true
              
                    @w32StartStream.call(nNewTransID, strHostNameIP, nPort)
               else
                    
                    Log("During starting stream, can not start due to not being connected.")
               end
          end
          
          # Adds Variable Updates for Message for Client Streams to Existing Stored Information for Sending to Server Later
          def self.StreamClientAddVar(nTransID, strRegObjDesign, strVarName, mxVarValue) 
          
               hhStreamClientStorage = @hhSendStorage["STREAMCLIENT"]
                                    # Storage for Stream Client Message
               boolStored = false   # Indicator That Update was Stored
               
               # Only Update If the Variable Name is a String and the Value Is Not a Hash or Contain One
               if (strVarName.is_a?(String) == true && mxVarValue.is_a?(Hash) == false && ArrayHasHash(mxParams) == false)
                     
                    if (hhStreamClientStorage.has_key?(nTransID) == true)
          
                         # Go Through Each Set of Updates to Find If Any are for the Specified Object Designation
                         hhStreamClientStorage[nTransID].each { |hhUpdateSelect|
                         
                              if (hhUpdateSelect["DESIGNATION"] == strRegObjDesign)
                                 
                                   hhUpdateSelect["VARUPDATES"].push({"NAME" => strVarName, "VALUE" => mxVarValue})
                                   boolStored = true
                                   break
                              end
                         }    
                         
                    else
                         
                         hhStreamClientStorage[nTransID] = [{"DESIGNATION" => strRegObjDesign,
                                                             "VARUPDATES" => [{"NAME" => strVarName, "VALUE" => mxVarValue}],
                                                             "FUNCCALLS" => []}]
                         boolStored = true
                         
                    end
          
               end
               
               return boolStored
          
          end
          
          # Adds Function Call Updates for Message for Client Streams to Existing Stored Information for Sending to Server Later
          def self.StreamClientAddFuncCall(nTransID, strRegObjDesign, strFuncName, mxParams = []) 
          
               hhStreamClientStorage = @hhSendStorage['STREAMCLIENT']
                                    # Storage for Stream Client Message
               boolStored = false   # Indicator That Function Calls Were Stored
               
               # Only Update If the Function Name is a String and the Parameters Is Not a Hash or Contain One
               if (strFuncName.is_a?(String) == true && mxParams.is_a?(Hash) == false && ArrayHasHash(mxParams) == false)
                    
                    if (mxParams.is_a?(Array) == false)
                       
                         mxParams = [mxParams]  
                         
                    end
                     
                    if (hhStreamClientStorage.has_key?(nTransID) == true)
          
                         # Go Through Each Set of Updates to Find If Any are for the Specified Object Designation
                         hhStreamClientStorage[nTransID].each { |hhUpdateSelect|
                         
                              if (hhUpdateSelect["DESIGNATION"] == strRegObjDesign)
                                 
                                   hhUpdateSelect["FUNCCALLS"].push({"NAME" => strFuncName, "PARAMS" => mxParams})
                                   boolStored = true
                                   break
                              end
                         }    
                         
                    else
                         
                         hhStreamClientStorage[nTransID] = [{"DESIGNATION" => strRegObjDesign,
                                                             "VARUPDATES" => [],
                                                             "FUNCCALLS" => [{"NAME" => strFuncName, "PARAMS" => mxParams}]}]
                         boolStored = true
                         
                    end
          
               end
               
               return boolStored
          
          end

          # Adds Message to Stream Associated to Transmission ID
          def self.AddStreamMsg(nTransID, strMsg)
               
               if IsConnected() == true
               
                    @w32AddStreamMsg.call(nTransID, strMsg)
               else
                    
                    Log("During adding stream, can not adding due to not being connected.")
               end
          end
          
          def self.RunRetFunc(hFuncInfo, strRespMsg = nil) 
                
               if hFuncInfo && hFuncInfo["OBJECT"] 
           
                    if hFuncInfo["OBJECT"].respond_to?(hFuncInfo["METHOD"], true) == true
               
                         if strRespMsg && hFuncInfo["OBJECT"].method(hFuncInfo["METHOD"]).parameters.length > 0
           
                              hFuncInfo["OBJECT"].method(hFuncInfo["METHOD"]).call(strRespMsg)
                                   
                         else 
                              
                              hFuncInfo["OBJECT"].method(hFuncInfo["METHOD"]).call()
                         end
                    else
                         
                         Log("During running response process method, '" + hFuncInfo["METHOD"] + "', does not exist.")
                    end
               end
          end
          
          # Shows Error on Screen
          def self.Show(strMsg)
          
               msgbox_p(strMsg)
          end
          
          # Finds If a Valid Array Has a Hash Value in it
          def self.ArrayHasHash(aobjCheck)
             
               boolHasHash = false; # Indicator That Array Has Hash 
               
               if (aobjCheck.is_a?(Array) == true)

                    aobjCheck.each { |objSelect|
                     
                         if (objSelect.is_a?(Hash) == true)
                            
                              boolHasHash = true;
                              break;
                                
                         end
                    }
                    
               end
                    
               return boolHasHash;
          end
          
          def self.UpdateHashSafe(hOriginal)
               
               hClone = nil              # Clone for Update
               
               if hOriginal.is_a?(Hash) == true
               
                    hClone = hOriginal.clone
                    
                    @ahHashUpdates.push({'ORIGINAL' => hOriginal,
                                         'CLONE' => hClone})
               end
               
               return hClone
          end

          def self.ValidateTransID(nNewTransID)

            return @hhSendStorage['STREAMCLIENT'].has_key?(nNewTransID) == true || 
                   @hhSendStorage['STREAMRAW'].include?(nNewTransID) == true ||
                   @hhSendStorage['HTTP'].include?(nNewTransID) == true ||
                   @hhSendStorage['DATAPROCESS'].has_key?(nNewTransID) == true
          end
          
     public
     
        def self.Connect(strHostNameIP = '', nPort = 0, boolStartServer = false, strSSLPrivKeyName = "", boolSSLConnect = false)
              
              boolConnected = IsConnected()
                                  # Indicator That Client Server Connection was Made
            
              begin

                  if boolSSLConnect == false || (boolSSLConnect == true && strSSLPrivKeyName != "")
                  
                       if boolConnected == false 
                     
                             # If Not Starting the Server Before Connecting to it
                             if boolStartServer == false
                                 
                                 # If No Hostname or IP Address was Set, Connect with Default Settings
                                 if strHostNameIP == '' 
                                   
                                       if boolSSLConnect == true

                                            if Win32API.new('RevCommClient32', 'ActivateUsingSSL', 'P', 'I').call(strSSLPrivKeyName) == 1
                                         
                                                boolConnected = true
                                              
                                            else 
                                              
                                                Log('Connecting client to server using default settings and SSL failed.', true)
                                              
                                            end

                                       elsif Win32API.new('RevCommClient32', 'Activate', 'V', 'I').call() == 1
                                              
                                           boolConnected = true
                                         
                                       else 
                                         
                                           Log('Connecting client to server using default settings failed.', true)
                                         
                                       end
                                       
                                 elsif nPort != 0
                                   
                                       if boolSSLConnect == true

                                            if Win32API.new('RevCommClient32', 'ActivateByHostPortUsingSSL', 'PIP', 'I').call(strHostNameIP, nPort, strSSLPrivKeyName) == 1
                                              
                                                boolConnected = true
                                              
                                            else
                                              
                                                Log('Connecting client to server using host, "' + strHostNameIP + '", port: ' + nPort.to_s() + ' and SSL failed.', true)
                                              
                                            end
                       
                                       elsif Win32API.new('RevCommClient32', 'ActivateByHostPort', 'PI', 'I').call(strHostNameIP, nPort) == 1
                                         
                                           boolConnected = true
                                         
                                       else
                                         
                                           Log('Connecting client to server using host, "' + strHostNameIP + '", port: ' + nPort.to_s() + ' failed.', true)
                                         
                                       end
                                   
                                 else
                                   
                                       Log('Can not connect client to server using host due to invalid settings.', true)
                   
                                 end
                                 
                             elsif nPort != 0
                                 
                                   
                                 if boolSSLConnect == true

                                      if Win32API.new('RevCommClient32', 'ActivateWithServerUsingSSL', 'P', 'I').call(strSSLPrivKeyName) == 1
                                        
                                            boolConnected = true
                                            
                                      else
                                            
                                            Log('Starting server and connecting client to it using SSL failed.', true)
                                                                  
                                      end

                                 elsif Win32API.new('RevCommClient32', 'ActivateWithServer', 'V', 'I').call() == 1
                                   
                                       boolConnected = true
                                       
                                 else
                                       
                                       Log('Starting server and connecting client to it failed.', true)
                                                             
                                 end
                                 
                             elsif boolSSLConnect == true 

                                  if Win32API.new('RevCommClient32', 'ActivateWithServerByPortUsingSSL', 'IP', 'I').call(nPort, strSSLPrivKeyName) == 1
                                            
                                      # If Port was Set, Start Server on That Port and Connect Locally
                                      boolConnected = true
                                            
                                  else
                        
                                      Log('Starting server and connecting client to it using port: ' + nPort.to_s() + ' and SSL failed.', true)
                                      
                                  end 
                                 
                             elsif Win32API.new('RevCommClient32', 'ActivateWithServerByPort', 'I', 'I').call(nPort) == 1
                                       
                                 # If Port was Set, Start Server on That Port and Connect Locally
                                 boolConnected = true
                                       
                             else
                   
                                 Log('Starting server and connecting client to it using port: ' + nPort.to_s() + ' failed.', true)
                                 
                             end                         
             
                             # If Client and Server Connection was Made, 
                             # Start up Threads for Sending and Receiving from Server and 
                             # Listen for "Peer To Peer" Connections
                             if boolConnected == true
             
                                 if @thdCommunicate == nil
                                       
                                       @thdCommunicate = Thread.new {
                                 
                                           w32Communicate = Win32API.new('RevCommClient32', 'Communicate', 'V', 'V')
                                                 # Receives and Sends Waiting Message from Server
                                           strDebugMsg = ""
                                                 # Debug Message
                                           strResponseMsg = ''         
                                                       # Returned Direct Message from Server    
                                                                         
                                           while RevCommProcessor.IsConnected() == true do
                                                                               
                                                 w32Communicate.call()
                                                 RevCommProcessor.ManageDataMapVars()
                                                 RevCommProcessor.GetLogError()
                                                 RevCommProcessor.GetDisplayError()
                                                 
                                                 if RevCommProcessor.DebugMode() == true
                                                   
                                                     strDebugMsg = RevCommProcessor.Debug()
                                                     
                                                     if strDebugMsg != ""
                                                         
                                                           RevCommProcessor.Log(strDebugMsg)
                                                     end
                                                 end
                                                
                                                 if RevCommProcessor.AutoRetProcessCmd() == true && 
                                                    RevCommProcessor.RunAutoDirectMsgByDesign() == false
                                                     
                                                    RevCommProcessor.GetDirectMsgNext(true)
                                                 end
                                                 
                                                 RevCommProcessor.RunAutoRetCleanup()
                                                 RevCommProcessor.HashUpdate()
                                                 
                                                 sleep(0.1)
                                           end
                                       }
                                 end
                                 
                                 if @thdPeerToPeerCommunicate == nil
                                       
                                       @thdPeerToPeerCommunicate = Thread.new {

                                           w32PeerToPeerCommunicate = Win32API.new('RevCommClient32', 'PeerToPeerCommunicate', 'V', 'V')
                                                 # Receives and Sends Waiting Message to "Peer To Peer" Clients
                                           strDebugMsg = ""
                                                 # Debug Message
                                                                         
                                           while RevCommProcessor.IsConnected() == true do
                                                                               
                                                 w32PeerToPeerCommunicate.call()
                                                 RevCommProcessor.GetLogError()
                                                 RevCommProcessor.GetDisplayError()

                                                 if RevCommProcessor.DebugMode() == true
                                                     
                                                     strDebugMsg = RevCommProcessor.Debug()
                                                     
                                                     if strDebugMsg != ""
                                                         
                                                           RevCommProcessor.Log(strDebugMsg)
                                                     end
                                                 end
                                                 
                                                 sleep(0.1)
                                           end
                                       }
                                       
                                 end
                             end
                             
                             ObjectSpace.define_finalizer(self, proc { |nObjectID| Disconnect() })
                                 
                       end
                    else

                         Log('Connecting client to server failed. Error: SSL connection\'s private key file name not set.', true)
                    end
                  
                  return boolConnected
              
              rescue Exception => exError 
              
                  Log('Connecting client to server failed. Exception: ' + exError.message, true)
                            
              end
          
        end

        def self.ConnectWithSSL(strSSLPrivKeyName, strHostNameIP = '', nPort = 0, boolStartServer = false)

          return Connect(strHostNameIP, nPort, boolStartServer, strSSLPrivKeyName, true)
        end
        
        # Starts "Peer To Peer" Server with Optional Encryption
        # Returns: True If Server is Started, Else False If Not Started or Already Running
        def self.StartPeerToPeerServer(strHostNameIP, nPort, strEncryptKey = "", strEncryptIV = "") 
              
#               w32StartServer = nil# Starts Peer-to-Peer Server
              boolStarted = false # Indicator That "Peer To Peer" Server was Started

              if strEncryptKey != "" && strEncryptIV != "" 

                if strEncryptKey.length >= ENCRYPTKEYSIZE && strEncryptIV.length >= ENCRYPTIVBLOCKSIZE

                  if Win32API.new('RevCommClient32', 'StartPeerToPeerServerEncryptedWithKeys', 'PIPPII', 'I').call(strHostNameIP, nPort, strEncryptKey, strEncryptIV, strEncryptKey.length, strEncryptIV.length) == 1
              
                    boolStarted = true
                  end
                else

                  Log('Starting peer-to-peer server, encryption key or block were not the correct sizes.')
                end
              else

                begin

                  w32StartServer = Win32API.new('RevCommClient32', 'StartPeerToPeerServerEncrypted', 'PI', 'I')
                
                rescue Exception => exError 
              
                    Log('Starting peer-to-peer server, encryption key and block was not set and client does not have default encryption. ' +
                        'Server will messages that are not encrypted which is not advised. Message: ' + exError.message)
                end                               
              end

              if w32StartServer && w32StartServer.call(strHostNameIP, nPort) == 1
                
                boolStarted = true

              elsif Win32API.new('RevCommClient32', 'StartPeerToPeerServer', 'PI', 'I').call(strHostNameIP, nPort) == 1

                boolStarted = true
              end
              
              return boolStarted
        end
        
        # Connects Client to a "Peer To Peer" Server with Optional Encryption
        def self.StartPeerToPeerConnect(strHostNameIP, nPort, strEncryptKey = "", strEncryptIV = "") 
              
#               w32StartConnect = nil
                                  # Starts Peer-to-Peer Connection
              boolConnected = false# Indicator That Client Connected to "Peer To Peer" Server
              
              if strEncryptKey != "" && strEncryptIV != "" 

                if strEncryptKey.length >= ENCRYPTKEYSIZE && strEncryptIV.length >= ENCRYPTIVBLOCKSIZE

                    if Win32API.new('RevCommClient32', 'StartPeerToPeerConnectEncryptedWithKeys', 'PIPPII', 'I').call(strHostNameIP, nPort, strEncryptKey, strEncryptIV, strEncryptKey.length, strEncryptIV.length) == 1
                  
                        boolConnected = true
                    end
                else

                    Log('Starting peer-to-peer connection, encryption key or block were not the correct sizes.')                  
                end

              else

                begin

                  w32StartConnect = Win32API.new('RevCommClient32', 'StartPeerToPeerConnectEncrypted', 'PI', 'I')

                rescue Exception => exError 
                
                      Log('Starting peer-to-peer connection, encryption key and block was not set and client does not have default encryption. ' +
                          'Messages send through the connection will not be encrypted which is not advised. Message: ' + exError.message)
                end   
                
                if w32StartConnect && w32StartConnect.call(strHostNameIP, nPort) == 1

                  boolConnected = true
                  
                elsif Win32API.new('RevCommClient32', 'StartPeerToPeerConnect', 'PI', 'I').call(strHostNameIP, nPort) == 1
              
                  boolConnected = true
                end
            end
              
              return boolConnected
        end
                  
        # Starts HTTP Transmission with "Port" Method Registered to Transaction ID Using Hostname/IP and Optional Port and Async or Synched
        def self.StartHTTPPost(nNewTransID, strHostNameIP, nPort = 0, boolAsync = true)
              
              boolStarted = false  # Indicator That Storage was Started

              if ValidateTransID(nNewTransID) == false
                     
                   if IsConnected() == true
            
                       if boolAsync == true
                             
                             if nPort != 0
                             
                                 @w32StartHTTPPostAsyncWithHostPort.call(nNewTransID, strHostNameIP, nPort)
                             else
                                 
                                 @w32StartHTTPPostAsyncWithHost.call(nNewTransID, strHostNameIP)
                                 
                             end
                             
                       elsif nPort != 0
                             
                             @w32StartHTTPPostSyncWithHostPort.call(nNewTransID, strHostNameIP, nPort)
                                 
                       else
                                       
                             @w32StartHTTPPostSyncWithHost.call(nNewTransID, strHostNameIP)
                             
                       end
                    
                       @hhSendStorage['HTTP'].push(nNewTransID)
                       boolStarted = true
                   else
                        
                        Log("During starting HTTP POST message, can not start due to not being connected.")
                   end
              
              end
              
              return boolStarted
              
        end
        
        # Starts HTTP Transmission with "Get" Method Registered to Transaction ID Using Hostname/IP and Optional Port and Async or Synched
        def self.StartHTTPGet(nNewTransID, strHostNameIP, nPort = 0, boolAsync = true)

              boolStarted = false  # Indicator That Storage was Started  
              
              if ValidateTransID(nNewTransID) == false
                    
                  if IsConnected() == true   
              
                       if boolAsync == true
                             
                             if nPort != 0
                             
                                 @w32StartHTTPGetASyncWithHostPort.call(nNewTransID, strHostNameIP, nPort)
                             else
                                 
                                 @w32StartHTTPGetASyncWithHost.call(nNewTransID, strHostNameIP)
                                 
                             end
                             
                       elsif nPort != 0
                             
                             @w32StartHTTPGetSyncWithHostPort.call(nNewTransID, strHostNameIP, nPort)
                                 
                       else
                                       
                             @w32StartHTTPGetSyncWithHost.call(nNewTransID, strHostNameIP)
                             
                       end
                       
                       @hhSendStorage['HTTP'].push(nNewTransID)
                       boolStarted = true
                  else
                       
                       Log("During starting HTTP GET message, can not start due to not being connected.")
                  end
              end
              
              return boolStarted
              
        end
        
        # Start Storage of Information for Sending Stream Message for Clients to Server Later
        def self.StartStreamClient(nNewTransID, strHostNameIP, nPort) 
        
              boolStarted = false  # Indicator That Storage was Started
              
              if ValidateTransID(nNewTransID) == false
              
                  @hhSendStorage['STREAMCLIENT'][nNewTransID] = []
                  StartStream(nNewTransID, strHostNameIP, nPort)
                  boolStarted = true
              
              end
              
              return boolStarted
        
        end

        # Start Storage of Information for Sending Raw Stream Message to Server Later
        def self.StartStreamRaw(nNewTransID, strHostNameIP, nPort) 
        
              boolStarted = false  # Indicator That Storage was Started
              
              if ValidateTransID(nNewTransID) == false
              
                  @hhSendStorage['STREAMRAW'].push(nNewTransID)
                  StartStream(nNewTransID, strHostNameIP, nPort)
                  boolStarted = true
              
              end
              
              return boolStarted
        
        end
        
        # Start Data Process
        def self.StartDataProcess(nNewTransID, strDataDesign) 
        
          boolStarted = false  # Indicator That Storage was Started
          
          if ValidateTransID(nNewTransID) == false
          
              @hhSendStorage['DATAPROCESS'][nNewTransID] = {"DESIGNATION" => strDataDesign, 
                                                            "PARAMS" => []}
              boolStarted = true
          
          end
          
          return boolStarted
    
        end

        # Adds Variable Updates for Client Message for Streams to Existing Stored Information for Sending to Server Later
        def self.StreamClientAddVar(nTransID, strRegObjDesign, strVarName, mxVarValue) 
        
              return StreamAddVar(nTransID, 'STREAMCLIENT', strRegObjDesign, strVarName, mxVarValue)
        end

        # Adds Function Call Updates for Client Message for Streams to Existing Stored Information for Sending to Server Later
        def self.StreamClientAddFuncCall(nTransID, strRegObjDesign, strFuncName, mxParams = []) 

              return StreamAddFuncCall(nTransID, 'STREAMCLIENT', strRegObjDesign, strFuncName, mxParams)
        end

        # Adds Variable Updates for Direct Client Message to Existing Stored Information for Sending to Server Later
        # If an Unsent Message Exists and the Designation is not that of the Existing Message, Addition will Fail
        def self.DirectClientMsgAddVar(strRegObjDesign, strVarName, mxVarValue) 
        
              boolStored = false   # Indicator That Update was Stored
              
              # Only Update If the Variable Name is a String and the Value Is Not a Hash or Contain One
              if strVarName.is_a?(String) == true && mxVarValue.is_a?(Hash) == false && ArrayHasHash(mxVarValue) == false
                    
                  if @hhSendStorage["DIRECTCLIENT"].empty? == false
        
                        if (@hhSendStorage["DIRECTCLIENT"]["DESIGNATION"] == strRegObjDesign)
                            
                            # Go Through Each Set of Updates to Find If Any are for the Specified Object Designation
                            @hhSendStorage["DIRECTCLIENT"]["VARUPDATES"].push({"NAME" => strVarName, "VALUE" => mxVarValue})
                            boolStored = true
                                  
                        end
                        
                  else
                        
                        @hhSendStorage["DIRECTCLIENT"] = {"DESIGNATION" => strRegObjDesign,
                                                          "VARUPDATES" => [{"NAME" => strVarName, "VALUE" => mxVarValue}],
                                                          "FUNCCALLS" => []}
                        boolStored = true
                        
                  end
              end
              
              return boolStored
        
        end
        
        # Adds Function Call Updates for Direct Client Message to Existing Stored Information for Sending to Server Later
        # If an Unsent Message Exists and the Designation is not that of the Existing Message, Addition will Fail
        def self.DirectClientMsgAddFuncCall(strRegObjDesign, strFuncName, mxParams = []) 
        
              boolStored = false   # Indicator That Function Calls Were Stored
              
              # Only Update If the Function Name is a String and the Parameters Is Not a Hash or Contain One
              if strFuncName.is_a?(String) == true && mxParams.is_a?(Hash) == false && ArrayHasHash(mxParams) == false
                  
                  if mxParams.is_a?(Array) == false
                      
                        mxParams = [mxParams]  
                        
                  end
                    
                  if @hhSendStorage["DIRECTCLIENT"].empty? == false
                        
                        if @hhSendStorage["DIRECTCLIENT"]["DESIGNATION"] == strRegObjDesign
                                                      
                            @hhSendStorage["DIRECTCLIENT"]["FUNCCALLS"].push({"NAME" => strFuncName, "PARAMS" => mxParams})
                            boolStored = true
                            
                        end
                        
                  else
                        
                        @hhSendStorage["DIRECTCLIENT"] = {"DESIGNATION" => strRegObjDesign,
                                                          "VARUPDATES" => [],
                                                          "FUNCCALLS" => [{"NAME" => strFuncName, "PARAMS" => mxParams}]}
                        boolStored = true
                  end
                  
              end
              
              return boolStored
        
        end
        
        # Stored Data Process Parameters By Transaction ID
        def self.AddDataProcessParams(nTransID, strParamName, mxParamValue)

              boolStoraged = false  # Indicator That Parameter was Stored
              
              if @hhSendStorage['DATAPROCESS'].has_key?(nTransID) == true
                   
                  if mxParamValue.is_a?(TrueClass) == true
                       
                       mxParamValue = "1"
                  elsif mxParamValue.is_a?(FalseClass) == true
                       
                       mxParamValue = "0"
                  elsif mxParamValue.is_a?(String) == false
                                              
                       mxParamValue = mxParamValue.to_s
                  end
              
                  @hhSendStorage['DATAPROCESS'][nTransID]['PARAMS'].push({"NAME" => strParamName, "VALUE" => mxParamValue})
                  boolStoraged = true
                  
              end
              
              return boolStoraged
              
        end
        
        # Add a Data Mapped Function
        def self.AddDataMapFunc(strDesign, 
                                objSource, 
                                strFuncName, 
                                strDataProcessDesign, 
                                strDataParamName) 
                  
#             strParamsList = (objSource.method(strFuncName).parameters.map { |aParams| aParams[1].to_s }).join(", ")
                                
             ClearDataMap(strDesign) 

             if objSource.respond_to?(strFuncName, true) == true
                  
                  strParamsList = (objSource.method(strFuncName).parameters.map { |aParams| aParams[1].to_s }).join(", ")
                  
                  objSource.instance_eval("alias revcom_" + strFuncName + " " + strFuncName + " 

                                           def " + strFuncName + "(" + strParamsList + ")
                      
                                             mxValue = revcom_" + strFuncName + "(" + strParamsList + ")
                                                  
                                             nMsgID = RevCommProcessor.GetUniqueID()     
                                             RevCommProcessor.StartDataProcess(nMsgID, '" + strDataProcessDesign + "')
                                                     
                                             if mxValue
                                   
                                                  RevCommProcessor.AddDataProcessParams(nMsgID, '" + strDataParamName + "', mxValue)
                                                     
                                             else
             
                                                  RevCommProcessor.AddDataProcessParams(nMsgID, '" + strDataParamName + "', '')
                                             end
                                             
                                                     
                                             if RevCommProcessor.SendDataProcess(nMsgID, RevCommProcessor.GetUniqueID()) == false
                                                     
                                                  RevCommProcessor.Log('Processing data map, designation: ''" + strDesign + "'', send failed.', true)
                                             end
                       
                                             return mxValue
                                           end")
              
                  @hhDataMaps[strDesign] = {"TYPE" => 0, 
                                            "OBJECT" => objSource, 
                                            "METHOD" => strFuncName, 
                                            "DATAPROCESS" => strDataProcessDesign,
                                            "PARAM" => strDataParamName,
                                            "VALUE" => nil}
             else
                  
                  Log("Setting up data map for function, designation: '" + strDesign + "', failed on finding the function, '" + 
                      strFuncName + "', accessible within object.", true)
             end                   
        end
        
        # Add a Data Mapped Variable
        def self.AddDataMapVar(strDesign, 
                               objSource, 
                               strVarName, 
                               strDataProcessDesign, 
                               strDataParamName)  
                               
             ClearDataMap(strDesign) 
                  
             objSource.instance_eval("def RevComVarCheck_" + strDesign + "_" + strVarName + "

                                          if self.instance_variable_defined?('@" + strVarName + "') == true
                                               
                                               mxValue = self.instance_variable_get('@" + strVarName + "')
                                                    
                                               if RevCommProcessor.UpdateDataMapVarVal('" + strDesign + "', mxValue) == true
     
                                                    nMsgID = RevCommProcessor.GetUniqueID()     
                              
                                                    RevCommProcessor.StartDataProcess(nMsgID, '" + strDataProcessDesign + "')
     
                                                    if !mxValue
                                            
                                                         mxValue = ''
                                                    end
                          
                                                    RevCommProcessor.AddDataProcessParams(nMsgID, '" + strDataParamName + "', mxValue)
                                                     
                                                    if RevCommProcessor.SendDataProcess(nMsgID, RevCommProcessor.GetUniqueID()) == false
                                                     
                                                         RevCommProcessor.Log('Processing data map, designation: ''" + strDesign + "'', send failed.', true)
                                                    end   
                                               end
                                          end
                                      end")
            
             @hhDataMaps[strDesign] = {"TYPE" => 1, 
                                       "OBJECT" => objSource, 
                                       "METHOD" => strVarName, 
                                       "DATAPROCESS" => strDataProcessDesign,
                                       "PARAM" => strDataParamName,
                                       "VALUE" => objSource.instance_variable_get("@" + strVarName)}
        end
        
        def self.SetHTTPResponseFuncs(nTransID, objDestination, strFuncName)

             boolSet = false        # Indicator That Parameter was Stored
             
             if objDestination && 
                strFuncName && 
                strFuncName != ""

                  if @hhSendFuncs['HTTP'].has_key?(nTransID) == false
                       
                       @hhSendFuncs['HTTP'][nTransID] = []
                  end
                  
                  @hhSendFuncs['HTTP'][nTransID].push({"OBJECT" => objDestination,
                                                       "METHOD" => strFuncName})
             end
             
             return boolSet
        end

        def self.SetDirectClientMsgFuncs(objDestination, strFuncName, strDesign = nil)
          
               boolSet = false        # Indicator That Return Function was Set
               
               if objDestination && 
                  strFuncName && 
                  strFuncName != ""
                    
                    @hhSendFuncs['DIRECTCLIENT'].push({"OBJECT" => objDestination,
                                                       "METHOD" => strFuncName,
                                                       "DESIGNATION" => strDesign})
               end
               
               return boolSet
        end
     
        def self.SetDataProcessResponseFuncs(nTransID, objDestination, strFuncName)
          
               boolSet = false        # Indicator That Return Function was Set
               
               if objDestination && 
                  strFuncName && 
                  strFuncName != ""
          
                    if @hhSendFuncs['DATAPROCESS'].has_key?(nTransID) == false
                         
                         @hhSendFuncs['DATAPROCESS'][nTransID] = []
                    end
                    
                    @hhSendFuncs['DATAPROCESS'][nTransID].push({"OBJECT" => objDestination,
                                                                "METHOD" => strFuncName})
               end
               
               return boolSet
        end
        
        # Add a Data Mapped Variable
        def self.UpdateDataMapVarVal(strDesign, 
                                     strValue)  

             boolUpdated = false    # Indicator That Change in Value Has Occurred
                                      
             if @hhDataMaps.has_key?(strDesign) == true && 
                @hhDataMaps[strDesign]["VALUE"] != strValue
           
                  @hhDataMaps[strDesign]["VALUE"] = strValue
                  boolUpdated = true
             end
         
           return boolUpdated
        end
        
        # Check if Updates on Data Mapped Variables
        def self.ManageDataMapVars

             @hhDataMaps.each { |strDesign, hDataMap|
             
                  hDataMap["OBJECT"].method("RevComVarCheck_" + strDesign + "_" + hDataMap["METHOD"]).call()
             }
          
        end
        # Sends Designated Stored Stream Client Message Before Removing it from Storage
        def self.SendStreamClientMsg(nTransID)  
        
              boolSend = false     # Indicator That Message was Sent
              
              if @hhSendStorage['STREAMCLIENT'].has_key?(nTransID) == true
            
                  AddStreamMsg(nTransID, JSONConvert.Encode(@hhSendStorage['STREAMCLIENT'][nTransID]))
                  @hhSendStorage['STREAMCLIENT'][nTransID].clear
                  
                  boolSend = true
              end
              
              return boolSend
              
        end 

        # Sends Designated Stored Raw Message Before Removing it from Storage
        def self.SendStreamRawMsg(nTransID, strMsg)  
        
              boolSend = false     # Indicator That Message was Sent
              
              if @hhSendStorage['STREAMRAW'].include?(nTransID) == true
            
                  AddStreamMsg(nTransID, strMsg)
                  boolSend = true
              end
              
              return boolSend
              
        end 
        
        # Sends Direct Client Message with Optional Designation to Server Before Removing it from Storage
        def self.SendDirectClientMsg(strMsgDesign = '', boolSendServer = true, boolSendPeerToPeer = false)
              
              boolSend = false     # Indicator That Message was Sent
              
              if @hhSendStorage['DIRECTCLIENT'].empty? == false
                  
                  SendDirectRawMsg(JSONConvert.Encode([@hhSendStorage['DIRECTCLIENT']]), strMsgDesign, boolSendServer, boolSendPeerToPeer)                    
                  ClearDirectClientMsgDesign()
                  boolSend = true
              end

              return boolSend
        end

        # Sends Direct Raw Message with Optional Designation to Server or "Peer To Peer" Clients or Both or Neither
        def self.SendDirectRawMsg(strMsg, strMsgDesign = '', boolSendServer = true, boolSendPeerToPeer = false)
              
              if boolSendServer == true
                   
                  if IsConnected() == true
                    
                       if strMsgDesign == ''
                             
                             @w32SendDirectMsg.call(strMsg)
                           
                       else 
                             
                             @w32SendDirectMsgWithDesign.call(strMsg, strMsgDesign)
                                       
                       end
                  else
                       
                       Log("During sending raw message, can not send due to not being connected.")
                  end
              end
              
              if boolSendPeerToPeer == true && @w32HasPeerToPeerClients.call() == 1
                    
                  if strMsgDesign == ''
                        
                        @w32SendDirectMsgPeerToPeer.call(strMsg)
                      
                  else 
                        
                        @w32SendDirectMsgPeerToPeerWithDesign.call(strMsg, strMsgDesign)
                                  
                  end
              end
        end
        
        # Sends Stored HTTP Transmission Message Associated with Transmission ID and Response ID
        def self.SendHTTP(nTransID, nNewRespID, boolAutoRetrieval = true)

              boolSend = false     # Indicator That Message was Sent
              
              if @hhSendStorage['HTTP'].include?(nTransID) == true
              
                  @w32SendHTTP.call(nTransID, nNewRespID)
                  boolSend = true
                   
                  if boolAutoRetrieval == true
                        
                        if !(@hhReceivers['HTTP'].has_key?(nTransID) == true && @hhReceivers['HTTP'][nTransID].has_key?(nNewRespID) == true)

                             @hhReceivers['HTTP'][nTransID] = {nNewRespID => {'RECEIVER' => nil, 'ACTIVE' => true}}
                                  
                        elsif @hhReceivers['HTTP'][nTransID].has_key?(nNewRespID) == false
                             
                             UpdateHashSafe(@hhReceivers['HTTP'][nTransID])[nNewRespID] = {'RECEIVER' => nil, 'ACTIVE' => true}
                                  
                        else
                             Log("During setting up of HTTP transmission auto retrieval for transaction ID, " + nTransID.to_s +  ", and response ID, " + nNewRespID.to_s + 
                                 ", auto retrieval was already running. Waiting until completion before setting up again")
                                  
                             @hhReceivers['HTTP'][nTransID][nNewRespID].join
                             @hhReceivers['HTTP'][nTransID].delete(nNewRespID)  
                        end
                        
                        @hhReceivers['HTTP'][nTransID][nNewRespID]['RECEIVER'] = Thread.new({'TRANSID' => nTransID,
                                                                                             'RESPID' => nNewRespID,
                                                                                             'TIMELIMIT' => @fAutoRetLimitInMillis,
                                                                                             'PROCESSCMD' => @boolAutoRetProcessCmd,
                                                                                             'ENDTRANS' => @boolAutoRetEndTrans}) { |hTransInfo|
     
                              nTransID = hTransInfo['TRANSID']
                                         # Transmission ID
                              nRespID =  hTransInfo['RESPID']
                                         # Response ID
                              nLimitInMillis = hTransInfo['TIMELIMIT']
                                         # Time Limit in Milliseconds for Retrival
                              boolProcessCmd = hTransInfo['PROCESSCMD']
                                         # Indicator to Process Response as Client Message Commands
                              boolEndTrans = hTransInfo['ENDTRANS'] 
                                         # Indicator to Close HTTP Transmission
                              boolNotFinished = true       
                                         # Indicator That Login Check is not Finished       
                              tmStart = Time.now
                                         # Start Time of Execution   
          
                              # Continously Check Until Response from Server is Received
                              while boolNotFinished == true && RevCommProcessor.IsConnected() == true && Time.now - tmStart < nLimitInMillis
                                   
                                   if RevCommProcessor.GetHTTPResponse(nTransID, nRespID, boolProcessCmd) == ''
                                        
                                        sleep(0.1)
                                             
                                   else
                                        
                                        if boolEndTrans == true
                                                                 
                                             RevCommProcessor.TranClose(nTransID)
                                        end        
                                        
                                        boolNotFinished = false
                                   end
                              end
                        }
                  end
                  
              end
              
              return boolSend
        end
        
        # Sends Stored Data Process Transmission Message Associated with Transmission ID and Response ID
        def self.SendDataProcess(nTransID, nNewRespID, boolAsync = true, boolAutoRetrieval = true) 
          
          strDesignation = @hhSendStorage['DATAPROCESS'][nTransID]['DESIGNATION']
                               # Selected Designation
          nAsyncValue = 1      # Integer Conversion Value for Indicator to Do Data Process Asychronously
          boolSend = false     # Indicator That Message was Sent
          
          if @hhSendStorage['DATAPROCESS'].has_key?(nTransID) == true
          
            if @hhSendStorage['DATAPROCESS'][nTransID]['PARAMS'].empty? == true
       
               if IsConnected() == true
              
                    @w32RegisterDataProcess.call(nTransID, strDesignation)
               else
                    
                    Log("During sending data process, can not register due to not being connected.")
               end
            elsif IsConnected() == true
                 
               @hhSendStorage['DATAPROCESS'][nTransID]['PARAMS'].each { |objParam|
 
                 @w32RegisterDataProcessWithParams.call(nTransID, strDesignation, objParam['NAME'], objParam['VALUE'])
               }
            else
               
               Log("During sending data process, can not register with parameters due to not being connected.")
            end
            
            if IsConnected() == true
                 
                 if boolAsync == false
                   
                   nAsyncValue = 0
                 end
     
                 @w32SendDataProcess.call(nTransID, nNewRespID, strDesignation, nAsyncValue)
                 
                 boolSend = true
            else
               
                 Log("During sending data process, can not send due to not being connected.")
            end    
 
            if boolAutoRetrieval == true
                 
                if @hhReceivers['DATAPROCESS'].has_key?(nTransID) == false || @hhReceivers['DATAPROCESS'][nTransID].size() == 0
                     
                     @hhReceivers['DATAPROCESS'][nTransID] = {nNewRespID => {'RECEIVER' => nil, 'ACTIVE' => true, 'TIMESTART' => Time.now}}
                          
                elsif @hhReceivers['DATAPROCESS'][nTransID].has_key?(nNewRespID) == false

                    @hhReceivers['DATAPROCESS'][nTransID][nNewRespID] = {'RECEIVER' => nil, 'ACTIVE' => true, 'TIMESTART' => Time.now}
                else
                     
                     Log("During setting up of data process auto retrieval for transaction ID, " + nTransID.to_s +  ", and response ID, " + nNewRespID.to_s + 
                         ", auto retrieval was already running.")
                end
                
                if boolAsync == true
                
                     @hhReceivers['DATAPROCESS'][nTransID][nNewRespID]['RECEIVER'] = Thread.new({'TRANSID' => nTransID,
                                                                                                 'RESPID' => nNewRespID,
                                                                                                 'TIMELIMIT' => @fAutoRetLimitInMillis,
                                                                                                 'PROCESSCMD' => @boolAutoRetProcessCmd,
                                                                                                 'ENDTRANS' => @boolAutoRetEndTrans}) { |hTransInfo|
               
                           nTransID = hTransInfo['TRANSID']
                                      # Transmission ID
                           nRespID =  hTransInfo['RESPID']
                                      # Response ID
                           nLimitInMillis = hTransInfo['TIMELIMIT']
                                      # Time Limit in Milliseconds for Retrival
                           boolProcessCmd = hTransInfo['PROCESSCMD']
                                      # Indicator to Process Response as Client Message Commands
                           boolEndTrans = hTransInfo['ENDTRANS'] 
                                      # Indicator to End Data Process
                           boolNotFinished = true       
                                      # Indicator That Login Check is not Finished
                           tmStart = Time.now
                                      # Start Time of Execution   
               
                           # Continously Check Until Response from Server is Received
                           while boolNotFinished == true && RevCommProcessor.IsConnected() == true && Time.now - tmStart < nLimitInMillis
                                
                                if RevCommProcessor.GetDataProcessResponse(nTransID, nRespID, boolEndTrans, boolProcessCmd) == ''
                                     
                                     sleep(0.1)
                                          
                                else                  
                                     
                                     boolNotFinished = false
                                end
                           end
                     }
                elsif !@thdAysncDataProcessor 
                     
                     @thdAysncDataProcessor = Thread.new { 
                         
                         # Continously Check Until Response from Server is Received
                         while RevCommProcessor.IsConnected() == true
                              
                              RevCommProcessor.DoDataProcessResponses()
                              
                              sleep(0.1)
                         end
                     }
                end               
            end
              
          end
          
          return boolSend
        end
        
        # Changes Registered Object of Existing Direct Client Message
        def self.ChangeDirectClientMsgDesign(strRegObjDesign)
              
              boolChanged = false  # Indicator That Update was Stored
                            
              # Only Update If the Variable Name is a String 
              if strVarName.is_a?(String) == true && @hhSendStorage["DIRECTCLIENT"].empty? == false
              
                  @hhSendStorage["DIRECTCLIENT"]["DESIGNATION"] = strRegObjDesign
                  boolChanged = true
                  
              end
              
              return boolChanged
        end

        # Clears Existing Direct Client Message
        def self.ClearDirectClientMsgDesign()
              
              @hhSendStorage["DIRECTCLIENT"].clear

        end
        
        # Gets Stored HTTP Transmission Message Associated with Transmission ID and Response ID
        def self.GetHTTPResponse(nTransID, nRespID, boolProcessCmd = false)
              
              nActualMsgLen = @w32CheckHTTPResponse.call(nTransID, nRespID)
                                  # Length of Waiting Message
              astrMsg = ' ' * nActualMsgLen 
                                  # Returned Message
              strMsgLen = nActualMsgLen.to_s  
                                  # Length of Return Message
              
              if nActualMsgLen > 0
                  
                  @w32GetHTTPResponse.call(nTransID, nRespID, astrMsg, strMsgLen)

                  nActualMsgLen = strMsgLen.to_i
              
                  if boolProcessCmd == true && nActualMsgLen > 0
                  
                        ProcessIncoming(astrMsg[0, nActualMsgLen])
                        
                  end
   
                  if @hhSendFuncs['HTTP'].has_key?(nTransID) == true &&
                     nActualMsgLen > 0
     
                       @hhSendFuncs['HTTP'][nTransID].each { |hFuncInfo|
                       
                            RunRetFunc(hFuncInfo, astrMsg[0, nActualMsgLen])
                       }
                  end
              end
              
              return astrMsg[0, nActualMsgLen];
        end

        # Gets Stored Data Process Message Associated with Transmission ID and Response ID
        def self.GetDataProcessResponse(nTransID, nRespID, boolDeleteTrans = false, boolProcessCmd = false)

              nActualMsgLen = @w32CheckDataProcessResponse.call(nTransID, nRespID)
                                  # Length of Waiting Message
              astrMsg = ' ' * nActualMsgLen 
                                  # Returned Message
              strMsgLen = nActualMsgLen.to_s  
                                  # Length of Return Message
              
              if nActualMsgLen > 0
                  
                  @w32GetDataProcessResponse.call(nTransID, nRespID, astrMsg, strMsgLen)
        
                  nActualMsgLen = strMsgLen.to_i
              
                  if boolProcessCmd == true && nActualMsgLen > 0
                  
                        ProcessIncoming(astrMsg[0, nActualMsgLen])
                        
                  end
        
                  if @hhSendFuncs['DATAPROCESS'].has_key?(nTransID) == true && nActualMsgLen > 0
 
                       @hhSendFuncs['DATAPROCESS'][nTransID].each { |hFuncInfo|
                       
                            RunRetFunc(hFuncInfo, astrMsg[0, nActualMsgLen])
                       }
                  end
                  
                  if boolDeleteTrans == true && @hhSendStorage['DATAPROCESS'].has_key?(nTransID) == true
     
                      @hhSendStorage['DATAPROCESS'].delete(nTransID)
                      @hhSendFuncs['DATAPROCESS'].delete(nTransID)
                  end

                  @hhReceivers['DATAPROCESS'][nTransID][nRespID]['ACTIVE'] = false
              end
              
              return astrMsg[0, nActualMsgLen];
        end

        # Gets Stored Data Process Message Associated with Transmission ID and Response ID
        def self.DoDataProcessResponses
             
             @hhReceivers['DATAPROCESS'].each { |nTransIndex, hTransInfo|

                  hTransInfo.each { |nRespIndex, hRespInfo|

                       if !hRespInfo['RECEIVER'] && 
                          hRespInfo['ACTIVE'] == true
                                
                           if Time.now - hRespInfo['TIMESTART'] < @fAutoRetLimitInMillis
                                
                              GetDataProcessResponse(nTransIndex, nRespIndex, @boolAutoRetEndTrans, @boolAutoRetProcessCmd)
                           else
                                
                              hRespInfo['ACTIVE'] = false
                           end
                       end
                  }
             } 
        end
    
        # Gets Next Message from Stream Associated with Transmission ID
        def self.GetStreamMsg(nTransID, boolProcessCmd = false)
              
              nMsgLen = @w32CheckStreamMsgsByIDReady.call(nTransID)
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen    # Returned Message
              strMsgLen = nMsgLen.to_s   # Holder Length of Return Message
              
              if nMsgLen > 0 
                  
                  @w32GetStreamMsg.call(nTransID, astrMsg, strMsgLen)
                  nMsgLen = strMsgLen.to_i
                  
                  if boolProcessCmd == true && nMsgLen > 0
                  
                        ProcessIncoming(astrMsg[0, nMsgLen])
                        
                  end
              
              end
              
              return astrMsg[0, nMsgLen];
        end
    
        # Gets Next Waiting Message from Any Stream
        def self.GetStreamMsgNext(boolProcessCmd = false)
              
              nMsgLen = @w32CheckStreamMsgsReady.call()
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen    # Returned Message
              strMsgLen = nMsgLen.to_s   # Holder of Length of Return Message

              if nMsgLen > 0
              
                  @w32CheckStreamMsgsReady.call(astrMsg, strMsgLen)
                  nMsgLen = strMsgLen.to_i
                  
                  if boolProcessCmd == true && nMsgLen > 0
                  
                        ProcessIncoming(astrMsg[0, nMsgLen])
                        
                  end
              end
              
              return astrMsg[0, nMsgLen];
        end

        # Gets Next Direct Message of a Designation
        def self.GetDirectMsg(strDesign, boolProcessCmd = false)
              
              nMsgLen = @w32CheckDirectMsgsWithDesignReady.call(strDesign)
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen    # Returned Message
              strMsgLen = nMsgLen.to_s   # Holder of Length of Return Message

              if nMsgLen > 0
                
                  @w32GetDirectMsg.call(strDesign, astrMsg, strMsgLen)
                  nMsgLen = strMsgLen.to_i
                  
                  if boolProcessCmd == true && nMsgLen > 0
                  
                        ProcessIncoming(astrMsg[0, nMsgLen])
                        
                  end
             
                  if nMsgLen > 0
                       
                       @hhSendFuncs['DIRECTCLIENT'].each { |hFuncInfo|
                       
                            if hFuncInfo["DESIGNATION"] == strDesign
                                 
                                 RunRetFunc(hFuncInfo, astrMsg[0, nMsgLen])
                            end
                       }
                  end
        
              end
              
              return astrMsg[0, nMsgLen];
        end

        # Gets Next Direct Message
        def self.GetDirectMsgNext(boolProcessCmd = false)
              
              nMsgLen = @w32CheckDirectMsgsReady.call()
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen    # Returned Message
              strMsgLen = nMsgLen.to_s   # Holder of Length of Return Message

              if nMsgLen > 0
                  
                  @w32GetDirectMsgNext.call(astrMsg, strMsgLen)
                  nMsgLen = strMsgLen.to_i
                  
                  if boolProcessCmd == true && nMsgLen > 0
                  
                        ProcessIncoming(astrMsg[0, nMsgLen])
                        
                  end
                  
                  if nMsgLen > 0
                       
                       @hhSendFuncs['DIRECTCLIENT'].each { |hFuncInfo|
                       
                            if !hFuncInfo["DESIGNATION"]
                                 
                                 RunRetFunc(hFuncInfo, astrMsg[0, nMsgLen])
                            end
                       }
                  end
             
              end
              
              return astrMsg[0, nMsgLen];
        end

        def self.GetSharedObjectByID(strID)
             
             objShared = nil
             
             @hhSharedObjects.each { |objSelect|
                  
                  if objSelect["ID"] == strID
                       
                       objShared = objSelect["OBJECT"]
                       break
                               
                  end
             }
             
             if !objShared
                  
                  @hhRegObjects.each { |strIndex, objSelect|
                       
                       if strIndex.start_with?("SHAREDOBJ") == true &&
                          strIndex == strID
                            
                            objShared = objSelect
                            break
                                    
                       end
                  }
             end
            
             return objShared
        end

        def self.GetSharedObjectByDesign(strDesign)
             
             return @hhSharedObjects[strDesign]["OBJECT"]
        end
          
        def self.GetSharedObjectsByClassName(strClassName, boolExcludeHostOnly = true)             
             
             aShared = GetSharedObjectsDataByClassName(strClassName, boolExcludeHostOnly)  
             strClassName = strClassName.downcase()
                  
             aShared += @hhRegObjects.select { |strIndex, objSelect|
                  
                  strIndex.start_with?("SHAREDOBJ") == true &&
                  objSelect["CLASS"].downcase() == strClassName
             }
            
             return aShared
        end
  
        def self.GetSharedObjectsForHostByClassName(strClassName)             
             
             return GetSharedObjectsDataByClassName(strClassName, false).select { |objSelect|
           
                         objSelect["HOST_ONLY"] == true
                    }
        end
        
        # Clears Stream Messages
        def self.ClearStreamMsgs(nTransID = nil)
              
              boolSuccessful = false# Indicator That Deletion was Successful
              
              if nTransID
                  
                  if @w32ClearStreamMsgs.call() == 1
                        
                        boolSuccessful = true                        
                  end
              else
                  
                  if @w32ClearStreamMsgsByIDReady.call(nTransID) == 1
                        
                        boolSuccessful = true       
                  end 
              end
              
              return boolSuccessful
        end
        
        # Clears Direct Messages
        def self.ClearDirectMsgs(strDesign = "")

              boolSuccessful = false# Indicator That Deletion was Successful
              
              if strDesign == ""
                  
                  if @w32ClearDirectMsgs.call() == 1
                  
                        boolSuccessful = true                        
                  end
              else
                  
                  if @w32ClearDirectMsgsWithDesign.call(strDesign) == 1
                  
                        boolSuccessful = true                        
                  end 
              end
              
              return boolSuccessful
        end
        
        # Clears a Data Map
        def self.ClearDataMap(strDesign)                    
                                       
#              hDataMap = @hhDataMaps[strDesign]
                                    # Selected Data Map
#              strRemoveFunc = "remove_method "
                                    # Setup for Removing Functions
             
              if @hhDataMaps.has_key?(strDesign) == true
 
                  hDataMap = @hhDataMaps[strDesign]  
                  strRemoveFunc = "remove_method "
                  
                  if hDataMap["TYPE"] == 0
                       
                       strRemoveFunc = ":revcom_" + hDataMap["METHOD"]
                  elsif hDataMap["TYPE"] == 1

                       strRemoveFunc = "RevComVarCheck_" + hDataMap["DESIGN"] + "_" + hDataMap["METHOD"] 
                  end
            
                  hDataMap["OBJECT"].instance_eval(strRemoveFunc)
                  @hhDataMaps.delete(strDesign)
              end
        end
    
        # Starts a File Download from Server Using its Designation
        def self.FileDownloadStart(strFileDesign)

              @w32GetStreamFile.call(strFileDesign)
        end
    
        # Checks If File Download is Completed and Saves it Using its Designation and an
        # Optional Directory Destination can be Set, Else it Downloads to Default Directory
        # If No File Designation Specified, it Saves Any File That Has Been Downloaded 
        # Returns True If Specified File were Downloaded Successfully, 
        # Else False If Failure or Specified File Has Not Completed Downloading
        def self.FileDownloadFinish(strFileDesign, strFilePathOverride = '')
 
             boolDownloaded = false     # Indicator That File or Files were Downloaded
             nFileLen = @w32CheckStreamFileReady.call(strFileDesign)
                                     # Check If File is Ready for Download, and Get Its Length
#             nFilePathLength = 0    # Length of File's Name and Path
#             astrMsg = ' ' * nFileLen# Returned File Contents
#             astrFilePathMsg = ' ' * nFilePathLength
                                     # Returned File Path in Return Message
#             strFileDownloadPath = ''   
                                     # File Path from Downloaded File
#             aFileDownloadPathList = nil
                                     # List of Peices of Download Path

             begin
                 
                 if nFileLen > 0
                      
                      nFilePathLength = @w32GetStreamFilePathLength.call(strFileDesign)
                      
                      if nFilePathLength > 0
                       
                         astrMsg = ' ' * nFileLen
                         astrFilePathMsg = ' ' * nFilePathLength
       
                         if @w32CheckStreamFileDownload.call(strFileDesign, astrFilePathMsg, nFilePathLength, astrMsg) == 1
   
                              if strFilePathOverride == ""
                                  
                                    strFilePathOverride = astrFilePathMsg[0, nFilePathLength] 
                              else
                          
                                    strFileDownloadPath = astrFilePathMsg[0, nFilePathLength]
   
                                   if strFilePathOverride.index('\\') != strFilePathOverride.length - 1
   
                                        strFilePathOverride += '\\'

                                   end

                                   aFileDownloadPathList = strFileDownloadPath.split('\\')
        
                                   if aFileDownloadPathList.length > 0
        
                                        strFilePathOverride += aFileDownloadPathList.pop
                                   end
                              end
  
                              File.chmod(0777, strFilePathOverride) rescue nil
            
                              File.open(strFilePathOverride, "w+b") do |flSave|
             
                                   flSave.write(astrMsg)
                              end
           
                              boolDownloaded = true
                         end
                      else
                   
                           Log('Downloading file, designation: ' + strFileDesign + ' failed, due to no file path length found.', true)
                      end
                 end

             rescue Exception => exError

                 Log('Downloading file, designation: ' + strFileDesign + ' failed. Message: ' + exError.message, true)
                 raise 
             end
             
             return boolDownloaded
        end
        
        def self.ClearStreamFileDownload(strFileDesign)
        
             @w32ClearStreamFileDownload.call(strFileDesign)
        end
    
        # Gets List of Files by Designations That Are Available for Download
        def self.GetAvailableFileList()
              
              astrMsg = ' ' * BUFFERSIZE # Returned Message
              strMsgLen = BUFFERSIZE.to_s.rjust(MAXBUFFERSIZEDIGITS, '0')
                                        # Length of Return Message
              
              @w32GetStreamFileList.call(astrMsg, strMsgLen)
              
              return astrMsg[0, strMsgLen.to_i()].strip
        end
        
        # Sets HTTP Transmission's Destination Page Associated with Transmission ID
        def self.SetHTTPProcessPage(nTransID, strPageURL)
              
              @w32SetHTTPProcessPage.call(nTransID, strPageURL)
        end
    
        # Adds HTTP Transmission's Variable and Value Pair for Sending to Destination Page Associated with Transmission ID
        def self.AddHTTPMsgData(nTransID, strVariableName, strValue)
              
              @w32AddHTTPMsgData.call(nTransID, strVariableName, strValue.to_s)
        end
    
        # Clears HTTP Transmission's Variable and Value Pairs for Sending to Destination Page Associated with Transmission ID
        def self.ClearHTTPMsgData(nTransID)
              
              @w32ClearHTTPMsgData.call(nTransID)
        end
        
        # Clears HTTP Transmission's Variable and Value Pairs for Sending to Destination Page Associated with Transmission ID
        def self.UseHTTPSSL(nTransID, boolUseSSL)
          
              nUseSSLValue = 1      # Integer Conversion Value for Indicator to Use SSL
              
              if boolUseSSL == false
                
                nUseSSLValue = 0
              end
              
              @w32UseHTTPSSL.call(nTransID, nUseSSLValue)
        end
    
        # Close Stream or HTTP Transmission's Associated with Transmission ID
        def self.TranClose(nTransID, boolReleaseID = false)

              boolClosed = false   # Indicator That Transmission was Closed
              
              if ValidateTransID(nTransID) == true
              
                  @w32Close.call(nTransID)
                  
                  if @hhSendStorage['STREAMCLIENT'].has_key?(nTransID) == true
                        
                      @hhSendStorage['STREAMCLIENT'][nTransID].clear
                        
                  elsif @hhSendStorage['STREAMRAW'].include?(nTransID) == true
                  
                      @hhSendStorage['STREAMRAW'].delete(nTransID)
                        
                  elsif @hhSendStorage['HTTP'].include?(nTransID) == true
        
                      @hhSendStorage['HTTP'].delete(nTransID)
                   
                      if @hhSendFuncs['HTTP'].has_key?(nTransID) == true
                           
                           @hhSendFuncs['HTTP'][nTransID].clear
                      end
                      
                      @hhReceivers['HTTP'][nTransID].each { |nIndex, hReceiver|
        
                         hReceiver['ACTIVE'] = false
                      }

                  end
                  
                  boolClosed = true
              
              end

              if boolReleaseID == true && @anUsedIDs.include?(nTransID) == true
               
                   ReleaseUniqueID(nTransID)
              end
                             
              return boolClosed 
        end

        def self.RunAutoDirectMsgByDesign
             
             boolRunning = @astrAutoRetDirectMsgDesigns.length > 0 && AutoRetProcessCmd() == true
             
             if boolRunning == true
                  
                  @astrAutoRetDirectMsgDesigns.each { |strDesign| 
                  
                       GetDirectMsg(strDesign, true)
                  }
             end
             
             return boolRunning
        end
        
        def self.RunAutoRetCleanup
             
             anTransClosed = []     # IDs Transactions to Close Receivers for 
#             boolIsClosed = true   # Indicator That a Transaction was Closed
             
             @hhReceivers['HTTP'].each { |nIndex, hReceiver| 
             
                  boolIsClosed = true
                  
                  hReceiver.each { |nRespIndex, hReceiveSelect| 
                       
                       if hReceiveSelect['ACTIVE'] == false
                       
                            hReceiveSelect['RECEIVER'].join 
                            hReceiveSelect['RECEIVER'] = nil 
                       else
                            
                            boolIsClosed = false
                       end
                  }
                  
                  if boolIsClosed == true
                  
                       anTransClosed.push(nIndex)
                  end
             }
             
             anTransClosed.each { |nIndex|
             
                  @hhReceivers['HTTP'].delete(nIndex)
             }
             
             anTransClosed = []
             
             @hhReceivers['DATAPROCESS'].each { |nIndex, hReceiver| 

                  boolIsClosed = true
                  
                  hReceiver.each { |nRespIndex, hReceiveSelect| 
                            
                       if hReceiveSelect['ACTIVE'] == false
                            
                            if hReceiveSelect['RECEIVER']
                            
                                 hReceiveSelect['RECEIVER'].join 
                                 hReceiveSelect['RECEIVER'] = nil 
                            end
                       else
                            
                            boolIsClosed = false
                       end
                  }
                  
                  if boolIsClosed == true
                  
                       anTransClosed.push(nIndex)
                  end
             }
             
             anTransClosed.each { |nIndex|
             
                  @hhReceivers['DATAPROCESS'].delete(nIndex)
             }
             
        end
        
        def self.HashUpdate
             
             @ahHashUpdates.each { |hUpdates|
             
                  if hUpdates['ORIGINAL'] != hUpdates['CLONE']
                       
                       hUpdates['ORIGINAL'].merge!(hUpdates['CLONE'])
                  end
             }
             
             @ahHashUpdates = []
        end
    
        # Changes Indicator for Parts of a Stream Transmission Message in Transmission Associated with Transmission ID
        def self.SetStreamTranMsgSeparatorChar(nTransID, strMsgSeparatorChars)
              
              @w32SetStreamMsgSeparator.call(nTransID, strMsgSeparatorChars)
        end
        
        # Changes Indicator for End of a Stream Transmission Message in Transmission Associated with Transmission ID
        def self.SetStreamTranEndChars(nTransID, strMsgEndChars)
              
              @w32SetStreamMsgEnd.call(nTransID, strMsgEndChars)
        end
        
        # Changes Extra Buffer Character Filler for a Stream Transmission Message in Transmission Associated with Transmission ID
        def self.SetStreamTranFillerChar(nTransID, charMsgFillerChar)
              
              @w32SetStreamMsgFiller.call(nTransID, charMsgFillerChar)
        end
    
        # Gets Error to be Submitted to Log
        def self.GetLogError()
              
              astrMsg = ' ' * BUFFERSIZE 
                                  # Returned Message
              strMsgLen = BUFFERSIZE.to_s.rjust(MAXBUFFERSIZEDIGITS, '0')       
                                  # Length of Return Message
              strLogMsg = ''       # Log Message
              
              @w32GetLogError.call(astrMsg, strMsgLen)
                  
              if strMsgLen.to_i > 0
              
                strLogMsg = astrMsg[0, strMsgLen.to_i]
                
                if (strLogMsg != '')
                
                      Log(strLogMsg, true)
                      
                end
              end
        end
    
        # Gets Error to be Displayed to User
        def self.GetDisplayError()
              
              astrMsg = ' ' * BUFFERSIZE 
                                  # Returned Message
              strMsgLen = BUFFERSIZE.to_s.rjust(MAXBUFFERSIZEDIGITS, '0')
                                  # Length of Return Message
              strDisplayMsg = ''   # Display Message
              
              @w32GetDisplayError.call(astrMsg, strMsgLen)
              
              if strMsgLen.to_i > 0

                strDisplayMsg = astrMsg[0, strMsgLen.to_i];
                
                if (strDisplayMsg != '')
                
                      Show(strDisplayMsg)
                      
                end
              end
        end
        
        def self.GetUniqueID

             rndGenerator = Random.new
                                  # Creates New Random ID
             nID = rndGenerator.rand(1..999999)           
                                  # New ID
                                 
             while @anUsedIDs.include?(nID) == true
             
                  nID = rndGenerator.rand(1..999999)   
                  
             end
             
             @anUsedIDs.push(nID)
             
             return nID
        end

        def self.ReleaseUniqueID(nID)
          
             if @anUsedIDs.include?(nID) == true
               
                  @anUsedIDs -= [nTransID]
                    
             end
        end
        
        def self.AddAutoRetDirectMsgDesigns(strDesign)
             
             if strDesign &&
                strDesign != "" &&
                @astrAutoRetDirectMsgDesigns.include?(strDesign) == false
                
                @astrAutoRetDirectMsgDesigns.push(strDesign)
             end
        end
        
        def self.RemoveAutoRetDirectMsgDesigns(strDesign)
             
             if strDesign &&
                strDesign != "" &&
                @astrAutoRetDirectMsgDesigns.include?(strDesign) == true
                
                @astrAutoRetDirectMsgDesigns -= [strDesign]
             end
        end
        
        # Changes Indicator for Parts of a Client/Server Message
        def self.SetServerMsgSeparatorChar(strMsgSeparatorChars)
              
              boolChanged = false; # Indicator That Character was Changed
              
              if @w32SetMsgPartIndicator.call(strMsgSeparatorChars) == 1
                  
                  boolChanged = true
                  
              end
              
              return boolChanged
        end

        # Changes Indicator for Start of a Client/Server Message
        def self.SetServerStartChars(strMsgStartChars)
              
              boolChanged = false; # Indicator That Character was Changed
                            
              if @w32SetMsgStartIndicator.call(strMsgStartChars) == 1
                  
                  boolChanged = true
                  
              end
              
              return boolChanged
        end
        
        # Changes Indicator for End of a Client/Server Message
        def self.SetServerEndChars(strMsgEndChars)
              
              boolChanged = false; # Indicator That Character was Changed
                            
              if @w32SetMsgEndIndicator.call(strMsgEndChars) == 1
                  
                  boolChanged = true
                  
              end
              
              return boolChanged
        end
        
        # Changes Extra Buffer Character Filler for a Client/Server Message
        def self.SetServerFillerChar(charMsgFillerChar)
              
              @w32SetMsgFiller.call(charMsgFillerChar)
        end   

        # Changes Extra Buffer Character Filler for a Client/Server Message
        def self.SetMsgIndicatorLen(nSetMsgLen)
        
              boolChanged = false; # Indicator That Character was Changed
                            
              if @w32SetMsgIndicatorLen.call(nSetMsgLen) == 1
                  
                  boolChanged = true
                  
              end
              
              return boolChanged
        end
        
        # Disconnect Client from Server
        def self.Disconnect()
    
              @w32Disconnect.call()
              
              if @thdCommunicate != nil
                  
                  @thdCommunicate.join()
                  @thdCommunicate = nil
              
              end
              
              if @thdPeerToPeerCommunicate != nil
              
                  @thdPeerToPeerCommunicate.join()
                  @thdPeerToPeerCommunicate = nil
              
              end
             
             if @thdAysncDataProcessor != nil
             
                  @thdAysncDataProcessor.join()
                  @thdAysncDataProcessor = nil
             
             end
        end
        
        # Discount "Peer To Peer" Server and Clients
        def self.DisconnectPeerToPeer()
    
              @w32DisconnectPeerToPeer.call()
        end
      
        # Register Objects to be Updated Through Processing Communications
        def self.RegisterObject(strDesign, objRegistrant, boolIsShared = false, boolHostOnly = false)
             
#             objHostCopy = objRegistrant.clone
             
              @hhRegObjects[strDesign] = objRegistrant
              
              if boolIsShared == true

                   @hhSharedObjects[strDesign] = {
                   
                        'ID' => objRegistrant.object_id,
                        'CLASS' => objRegistrant.class.name,
                        'OBJECT' => objRegistrant,
                        'HOST_ONLY' => boolHostOnly
                   }
                        
                   if boolHostOnly == false
     
                        DirectClientMsgAddFuncCall("SHAREDOBJ" + objRegistrant.object_id,
                                                   "ReceiveSharedObject",
                                                   Marshal.dump(@hhSharedObjects[strDesign]))
                   else
                        
                        objHostCopy = objRegistrant.clone
                        
                        SetupSharedObject(objHostCopy, strDesign, true)
                        
                        objHostCopy.Setup()

                        DirectClientMsgAddFuncCall("SHAREDOBJ" + objRegistrant.object_id,
                                                   "ReceiveSharedObject",
                                                   Marshal.dump(objHostCopy))
                   end     

                   SendDirectClientMsg() 

                   SetupSharedObject(objRegistrant, objRegistrant.object_id, false, boolHostOnly, "@strTopLvlMethod = ''")
                   objRegistrant.Setup()
              end
        end

        # Unregister Object
        def self.UnregisterObject(strDesign)
                
              if @hhRegObjects.has_key?(strDesign) == true

                   @hhRegObjects.delete(strDesign)
              end
               
             if @hhSharedObjects.has_key?(strDesign) == true

                  @hhSharedObjects.delete(strDesign)
             end
        end

        # Send Out All Shared Objects
        def self.SendSharedObjects
                
           @hhSharedObjects.each { |hObject|
                
                DirectClientMsgAddFuncCall("SHAREDOBJ" + hObject["ID"],
                                           "ReceiveSharedObject",
                                           Marshal.dump(hObject))
                SendDirectClientMsg()
           }
        end
      
        # Log to File
        def self.Log(strMsg, boolError = false)
      
              @mtxLock.lock()

              tmCurrent = Time.now # Current Time
              fLog = nil           # Log File
                      
              begin
              
                  if (File.directory?("Logs") == false)
            
                        Dir.mkdir("Logs")
            
                  end
            
                  fLog = File.open("Logs/Log-" + tmCurrent.strftime("%Y%m%d") + ".txt", 'a')
                  
                  if (boolError)
                        
                        strMsg = 'ERROR - ' + strMsg
                          
                  end
            
                  fLog.write(tmCurrent.strftime("%H:%M:%S.%L") + ": " + strMsg + "\n")
        
              rescue Exception => exError
            
                  Show("Error: Writing to log file failed. Message: " + exError.message)
            
              ensure
            
                  fLog.close unless fLog == nil
                  
              end
        
              @mtxLock.unlock()
        end
        
        def self.IsConnected()
              
              return @w32IsConnected.call() == 1
        end
        
        def self.IsInSessionGroup()
     
               return @w32IsInSessionGroup.call() == 1
        end

        def self.IsSessionGroupHost()
          
               return @w32IsSessionGroupHost.call() == 1
        end
        
        def self.SetQueueLimit(nNewLimit)

               @w32SetQueueLimit.call(nNewLimit)
        end

        def self.SetMsgLateLimit(nNewLimit)

               @w32SetMsgLateLimit.call(nNewLimit)
        end

        def self.SetDropLateMsgs(boolDrop)

          nDropValue = 1      # Integer Conversion Value for Indicator to Drop Limit Messages
          
          if boolDrop == false
            
            nDropValue = 0
          end
          
          @w32SetDropLateMsgs.call(nDropValue)
        end

        def self.SetActivityCheckTimeLimit(nNewLimit)

          @w32SetActivityCheckTimeLimit.call(nNewLimit)
        end
        
        def self.AutoRetLimitInMillis(fSetAutoRetLimitInMillis = nil)
   
             if fSetAutoRetLimitInMillis
                  
                  @fAutoRetLimitInMillis = fSetAutoRetLimitInMillis
             end
             
             return @fAutoRetLimitInMillis
        end 

        def self.AutoRetProcessCmd(boolSetAutoRetProcessCmd = nil)
          
             if boolSetAutoRetProcessCmd
                    
                 @boolAutoRetProcessCmd = boolSetAutoRetProcessCmd
             end
             
             return @boolAutoRetProcessCmd
        end 

        def self.AutoRetEndTrans(boolSetAutoRetEndTrans = nil)
            
             if boolSetAutoRetEndTrans
                      
                 @boolAutoRetEndTrans = boolSetAutoRetEndTrans
             end
             
             return @boolAutoRetEndTrans
        end 
        
        def self.Debug
              
              nMsgCount = self.DebugReceivedQueueCount
                                        # Count of Messages in Queue
              strMsg = 'Received: '     # Returned Message List
              nCounter = 0              # Counter for Loop
              
              if nMsgCount > 0
                  
                  for nCounter in 0...nMsgCount
                       
                       if nCounter > 0
                       
                            strMsg += ' | ';
                       end
                        
                        strMsg += 'R-' + (nCounter + 1).to_s + ' - ' + self.DebugReceived(nCounter)
                  end
              else
                  
                  strMsg += 'None'
              end
              
              nMsgCount = self.DebugReceivedStoredQueueCount
              strMsg += ' ----- Received - Stored: '
             
              if nMsgCount > 0
                 
                  for nCounter in 0...nMsgCount
                      
                       if nCounter > 0
                      
                            strMsg += ' | ';
                       end
                       
                        strMsg += 'RS-' + (nCounter + 1).to_s + ' - ' + self.DebugReceivedStored(nCounter)
                  end
              else
                 
                  strMsg += 'None'
              end
              
              nMsgCount = self.DebugToSendQueueCount
              strMsg += ' ----- Sending: '
              
              if nMsgCount > 0
                  
                  for nCounter in 0...nMsgCount
                      
                        if nCounter > 0
                      
                             strMsg += ' | ';
                        end
                        
                        strMsg += 'SD-' + (nCounter + 1).to_s + ' - ' + self.DebugToSend(nCounter)
                  end
              else
                  
                  strMsg += 'None'
              end
             
             nMsgCount = self.DebugToSendStoredQueueCount
             strMsg += ' ----- Sending - Stored: '
             
             if nMsgCount > 0
                 
                 for nCounter in 0...nMsgCount
                     
                       if nCounter > 0
                     
                            strMsg += ' | ';
                       end
                       
                       strMsg += 'SS' + (nCounter + 1).to_s + ' - ' + self.DebugToSendStored(nCounter)
                 end
             else
                 
                 strMsg += 'None'
             end
              
              return strMsg
              
        end
              
        def self.DebugReceivedQueueCount
              
              return @w32DebugReceivedQueueCount.call()
              
        end
        
        def self.DebugToSendQueueCount
              
              return @w32DebugSendQueueCount.call()
              
        end
      
        def self.DebugReceivedStoredQueueCount
                
               return @w32DebugReceivedStoredQueueCount.call()
                
        end
          
        def self.DebugToSendStoredQueueCount
                
               return @w32DebugSendStoredQueueCount.call()
                
        end

        # Debug for Getting Message from Receiving Queue by Message Index
        def self.DebugReceived(nMsgIndex)
              
              nMsgLen = @w32DebugReceivedMsgLength.call(nMsgIndex)           
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen   # Returned Message
              strMsgLen = nMsgLen.to_s  # Length of Return Message
              
              @w32DebugReceived.call(nMsgIndex, astrMsg, strMsgLen)
              
              return "(" + strMsgLen + ") " + astrMsg[0, strMsgLen.to_i];
              
        end

        # Debug for Getting Message from Sending Queue by Message Index
        def self.DebugToSend(nMsgIndex)
              
              nMsgLen = @w32DebugSendMsgLength.call(nMsgIndex)           
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen   # Returned Message
              strMsgLen = nMsgLen.to_s  # Length of Return Message
              
              @w32DebugToSend.call(nMsgIndex, astrMsg, strMsgLen)
              
              return "(" + strMsgLen + ") " + astrMsg[0, strMsgLen.to_i];  
              
        end

        # Debug for Getting Message from Receiving Stored Queue by Message Index
        def self.DebugReceivedStored(nMsgIndex)
                
              nMsgLen = @w32DebugReceivedStoredMsgLength.call(nMsgIndex)           
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen   # Returned Message
              strMsgLen = nMsgLen.to_s  # Length of Return Message
                
              @w32DebugReceivedStored.call(nMsgIndex, astrMsg, strMsgLen)
                
              return "(" + strMsgLen + ") " + astrMsg[0, strMsgLen.to_i];
                
        end
          
        # Debug for Getting Message from Sending Stored Queue by Message Index
        def self.DebugToSendStored(nMsgIndex)
                
              nMsgLen = @w32DebugSendStoredMsgLength.call(nMsgIndex)           
                                        # Length of Return Message
              astrMsg = ' ' * nMsgLen   # Returned Message
              strMsgLen = nMsgLen.to_s  # Length of Return Message
                
              @w32DebugToSendStored.call(nMsgIndex, astrMsg, strMsgLen)
                
              return "(" + strMsgLen + ") " + astrMsg[0, strMsgLen.to_i];  
                
        end
        
        def self.DebugActivate(boolOn = true)
              
            @boolDebug = boolOn
        end

        def self.DebugMode
              
            return @boolDebug
        end
end

# Register Global Object and Processer Itself for Server Communications
RevCommProcessor.RegisterObject("GLOBAL", self)
RevCommProcessor.RegisterObject("MAIN", RevCommProcessor)

#===============================================================================
# JSON Encoder/Decoder - Original Author: game_guy
#-------------------------------------------------------------------------------
module JSONConvert
  
     private 
     
       TOKEN_NONE = 0;
       TOKEN_CURLY_OPEN = 1;
       TOKEN_CURLY_CLOSED = 2;
       TOKEN_SQUARED_OPEN = 3;
       TOKEN_SQUARED_CLOSED = 4;
       TOKEN_COLON = 5;
       TOKEN_COMMA = 6;
       TOKEN_STRING = 7;
       TOKEN_NUMBER = 8;
       TOKEN_TRUE = 9;
       TOKEN_FALSE = 10;
       TOKEN_NULL = 11;
       TOKEN_SPACE = 12;
  
       @nIndex = 0     # Index of Message Being Converted
       @strJson = ""   # Result of Convert
       @nLength = 0    # Length of Message
            
       def self.EncodeHash(hSource)
            
          strResult = "{"
          
          hSource.each_key {|mxKey|
               
               strResult += "\"#{mxKey}\":" + self.Encode(hSource[mxKey]).to_s + ","
          }
     
          strResult[strResult.size - 1, 1] = "}"
       
          return strResult
       end
     
       def self.EncodeArray(aSource)
       
            strResult = "["
            
            aSource.each {|nSourceIndex|
            
                 strResult += self.Encode(nSourceIndex).to_s + ","
            }
       
            if (strResult.size > 1)
       
               strResult[strResult.size - 1, 1] = "]"
     
            else     
            
               strResult += "]"
            
            end
       
            return strResult
       end
     
       def self.EncodeString(strSource)
            return "\"#{strSource}\""
       end
     
       def self.EncodeInteger(nSource)
            return nSource.to_s
       end
     
       def self.EncodeBool(boolSource)
            return (boolSource.is_a?(TrueClass) ? "true" : "false")
       end
     
       def self.next_token(debug = 0)
            
            @nIndex += 1
       
            case @strJson[@nIndex - 1, 1]
       
               when '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '.'
         
                    return TOKEN_NUMBER
       
               when '{' 
         
                    return TOKEN_CURLY_OPEN
       
               when '}' 
         
                    return TOKEN_CURLY_CLOSED
       
               when '"' 
         
                    return TOKEN_STRING
               
               when ',' 
         
                    return TOKEN_COMMA
              
               when '['
         
                    return TOKEN_SQUARED_OPEN
       
               when ']'
         
                    return TOKEN_SQUARED_CLOSED
       
               when ':' 
         
                    return TOKEN_COLON
       
               when ' ' 
         
                    return TOKEN_SPACE
            end
       
            @nIndex -= 1
       
            if @strJson[@nIndex, 5] == "false"
         
                 return TOKEN_FALSE
       
            elsif @strJson[@nIndex, 4] == "true"
         
                 return TOKEN_TRUE
       
            elsif @strJson[@nIndex, 4] == "null"
         
                 return TOKEN_NULL
       
            end
       
            return TOKEN_NONE
       end
     
       def self.parse

            while true
         
                 if @nIndex >= @nLength
                 
                      break
                 end
         
                 token = self.next_token
         
                 case token
         
                    when TOKEN_NONE
           
                         return nil
         
                    when TOKEN_NUMBER
           
                         return self.ParseNumber
         
                    when TOKEN_CURLY_OPEN
           
                         return self.ParseObject
         
                    when TOKEN_STRING
           
                         return self.ParseString
         
                    when TOKEN_SQUARED_OPEN
           
                         return self.ParseArray
         
                    when TOKEN_TRUE
           
                         @nIndex += 4
                         return true
         
                    when TOKEN_FALSE
           
                         @nIndex += 5
                         return false
         
                    when TOKEN_NULL
           
                         @nIndex += 4
                         return nil
                 end
            end
       end
     
       def self.ParseObject
       
            objResult = {}
            boolNotComplete = true
       
            while boolNotComplete == true
         
                 strToken = self.next_token
         
                 if strToken == TOKEN_CURLY_CLOSED
              
                      boolNotComplete = false
                      break
                 elsif strToken == TOKEN_NONE
           
                      return nil
                 elsif strToken == TOKEN_COMMA
                 elsif strToken == TOKEN_STRING
           
                      strName = self.ParseString
                      return nil if strName == nil
                      
                      strToken = self.next_token
                      return nil if strToken != TOKEN_COLON
                      
                      nValue = self.parse
                      objResult[strName] = nValue
                 end
            end
            
            return objResult
     end
     
     def self.ParseString
       
          boolNotComplete = true
          strResult = ""
       
          while boolNotComplete == true
         
               break if @nIndex >= @nLength
         
               strChar = @strJson[@nIndex, 1]
               @nIndex += 1
         
               case strChar
         
                    when '"'
                         boolNotComplete = false
                         break
                         
                    else
                         strResult += strChar.to_s
               end
          end
       
          if boolNotComplete == true
          
               return nil
          end
       
          return strResult
     end
     
     def self.ParseNumber
       
          @nIndex -= 1
          strResult = ""
          boolNotComplete = true
       
          while boolNotComplete == true
         
               break if @nIndex >= @nLength
               strChar = @strJson[@nIndex, 1]
               @nIndex += 1
         
               case strChar
                    when "{", "}", ":", ",", "[", "]"
                    
                         @nIndex -= 1
                         boolNotComplete = false
                         break
                    when "0", "1", "2", '3', '4', '5', '6', '7', '8', '9', '.', '-'
           
                         strResult += strChar.to_s
               end
          end
       
          if (strResult.include?(".") == false)
            
               return strResult.to_i
         
          else
               
               return strResult.to_f           
          end
     end
     
     def self.ParseArray
       
          objResult = []
          boolNotComplete = true
       
          while boolNotComplete == true
         
               strToken = self.next_token(1)
         
               if strToken == TOKEN_SQUARED_CLOSED
                    
                    boolNotComplete = false
                    break
               elsif strToken == TOKEN_NONE
               
                    return nil
               elsif strToken == TOKEN_COMMA
               else
                    @nIndex -= 1
                    objResult.push(self.parse())
               end
          end
       
          return objResult
     end

  public
  
     def self.Decode(strSourceJson)
    
          @strJson = strSourceJson
          @nIndex = 0
          @nLength = @strJson.length
    
          return self.parse
     end
  
     def self.Encode(objSource)
    
          if objSource.is_a?(Hash)
      
               return self.EncodeHash(objSource)
          elsif objSource.is_a?(Array)
      
               return self.EncodeArray(objSource)
          elsif objSource.is_a?(Fixnum) || objSource.is_a?(Float)
      
               return self.EncodeInteger(objSource)
          elsif objSource.is_a?(String)
      
               return self.EncodeString(objSource)
          elsif objSource.is_a?(TrueClass) || objSource.is_a?(FalseClass)
      
               return self.EncodeBool(objSource)
          elsif objSource.is_a?(NilClass)
      
               return "null"
          end
    
          return nil
     end  
end