<?php
/* RevCommConfig.php - Web Server Configuration Script for RevCommSuite API

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
 SOFTWARE. */

error_reporting(E_ERROR);

class cConfig {
	
	function __construct() {

		$objThis = &$this;
		$strDirectName = __DIR__;	/* Name of Directory for Configuration Files */
		$strHostName = '';			/* Server's HostName */
		$aSettings;					/* Holder for Settings from Session */
		$objConfigDirect;			/* Access to Directory for Configuration Files */
		$strFileName = '';			/* Name of a Selected File in the Directory for Configuration Files */
		$boolConfigNotFound = true; /* Indicator That Site's Configuration File was Not Found */
		$boolConfigRedirectFound = false; 
									/* Indicator That Site's Configuration File with Redirect was Found */
		$strMsgStartIndictator = '+???';
		                            /* Server Command Message Starting Indicator */
		$strMsgPartIndictator = '???+';
		                            /* Server Command Message Part Starting Indicator */
		$strMsgEndIndictator = '+??+';
		                            /* Server Command Message Ending Indicator */
		$strMsgTableName = 'revcomm_msg_out';
		                            /* Server Command Message Database Storage Table */
		$strMsgIDColumnName = 'revcomm_msg_out_id';
		                            /* Server Command Message ID Database Column Name in the Storage Table */
		$strMsgValueColumnName = 'msg';
		                            /* Server Command Message Databae Column Name in the Storage Table Where Message is Stored */
		$objMessagingInfo;          /* Configuration Messaging Settings */
		
		session_start();
		
		/* If Host Name is Available, Get it, Else Get it from IP Address */
		if (!empty($_SERVER['REMOTE_HOST'])) {
				
			$strHostName = $_SERVER['REMOTE_HOST'];
		}
		else {
				
			$strHostName = gethostname();
		}
		
		$objThis -> hostname = $strHostName;
		$objThis -> host_settings_loaded = false;
		
		$objThis -> LoadSettings();
		
		try {

			/* Get Access to the Directory Containing the XML Configuration Files, and
			   Get the One for This Site by Looking at the "URL" Setting Within It  */
			if (($objConfigDirect = opendir($strDirectName)) !== false) {
			 	
			 	/* Cycle Through All of the Files in the Directory Looking for the XML Configuration Files */
				while ($boolConfigNotFound && ($strFileName = readdir($objConfigDirect)) !== false) {
			
					if ((strrpos($strFileName, '.xml') + 4) == strlen($strFileName)) {
					
						$boolConfigNotFound = $objThis -> LoadXMLSettings(simplexml_load_file($strDirectName . $objThis -> SystemSlashReplace('\\') . $strFileName), $objThis);
						
						if (isset($objThis -> redirect)) {
							
							/* If Site's Config File was Found with Redirect was Found, Store That For Later, and 
							   Look for Other Site Config Files in Another Directory */
							if (!$boolConfigNotFound) {
							
								$boolConfigRedirectFound = true;
							}
							
							$boolConfigNotFound = true;
							
							closedir($objConfigDirect);
							
							$strDirectName = $objThis -> SystemSlashReplace($objThis -> redirect);
							unset($objThis -> redirect);
							
							if (($objConfigDirect = $objThis -> DirectoryAboveSearch($strDirectName)) === false) {
							
								/* Could Not Gain Access to the Configuraton Redirect File Directory */
								$objThis -> LogError('Error occurred during loading configuration file. ' .
								'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $strHostName .
								'", finding or accessing configuration file redirected directory for host site failed.');
								break;
							}
						}
					}
				}
					
				closedir($objConfigDirect);
				
				if ($boolConfigRedirectFound) {
						
					$boolConfigNotFound = false;
				}
				
				if (!$boolConfigNotFound) {
				    
				    if (!empty($objThis -> messaging)) {
				        
				        $objMessagingInfo = $objThis -> messaging;
    				    
				        if (!empty($objMessagingInfo -> msg_start_identicator)) {
    				        
				            $strMsgStartIndictator = $objMessagingInfo -> msg_start_identicator;
    				    }
    				    
    				    if (!empty($objMessagingInfo -> msg_part_identicator)) {
    				        
    				        $strMsgPartIndictator = $objMessagingInfo -> msg_part_identicator;
    				    }
    				    
    				    if (!empty($objMessagingInfo -> msg_end_identicator)) {
    				        
    				        $strMsgEndIndictator = $objMessagingInfo -> msg_end_identicator;
    				    }
    				    
    				    if (!empty($objMessagingInfo -> msg_table_name)) {
    				        
    				        $strMsgTableName = $objMessagingInfo -> msg_table_name;
    				    }
    				    
    				    if (!empty($objMessagingInfo -> msg_id_field_name)) {
    				        
    				        $strMsgIDColumnName = $objMessagingInfo -> msg_id_field_name;
    				    }
    				    
    				    if (!empty($objMessagingInfo -> msg_value_field_name)) {
    				        
    				        $strMsgValueColumnName = $objMessagingInfo -> msg_value_field_name;
    				    }
				    }
					
					/* If Store Configuration Object for Reloading */
					if ($objThis -> store_app_info) {
						
						$_SESSION['CONFIG'] = &$objThis;
					}
						
					if ($objThis -> use_ssl && empty($_SERVER['HTTPS'])) {
						
						if (str_contains($objThis -> url, 'http://')) {
			
							header('Location: ' . str_replace('http://', 'https://', $objThis -> url));
						}
						else {
			
							header('Location: https://' . $objThis -> url);
						}
						
						exit();
					}
					
					$objThis -> config_file_path = $strDirectName . $objThis -> SystemSlashReplace('\\') . $strFileName;
					$objThis -> host_settings_loaded = true;
				}
				else {
					
					/* Else Could Not Find Configuration File */
					$objThis -> LogError('Error occurred during loading configuration file. ' .
										 'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $strHostName .
										 '", finding or accessing configuration file for host site failed.');
				}
			}
			else {
			
				/* Else Could Not Gain Access to the Configuraton File Directory */
				$objThis -> LogError('Error occurred during loading configuration file. ' .
									 'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $strHostName .
									 '", finding or accessing configuration file directory for host site failed.');
			}	
		}
		catch (Exception $exError) {
			
			$objThis -> LogError('Error occurred during loading configuration file. ' .
								 'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $strHostName .
								 '", exception occurred. Exception: ' . $exError -> getMessage());
		}
	}
	
	/* Add or Update Existing Setting Within Configuration with Ability to Store Temporarily Local or in Session */
	public function Settings($strSettingName, $mxSettingValue, $boolLocalStorage = true) {
		
		if ($boolLocalStorage) {
			
			$this -> {$strSettingName} = $mxSettingValue;
		}
		else {
			
			$_SESSION['SETTINGS'][$strSettingName] = $mxSettingValue;
			$this -> {$strSettingName} = &$_SESSION['SETTINGS'][$strSettingName];
		}
	}

	/* Get Setting Within Configuration */
	public function SettingGet($strSettingName) {
		
		return $this -> {$strSettingName};
	}

	/* Remove Settings Within Configuration */
	public function SettingRemove($strSettingName) {
		
		$boolUnset = false;			/* Indicator That the Value was Unset */
		
		if (isset($_SESSION['SETTINGS'][$strSettingName])) {
			
			unset($_SESSION['SETTINGS'][$strSettingName]);
		}
		
		if (isset($this -> {$strSettingName})) {
			
			unset($this -> {$strSettingName});
			$boolUnset = true;
		}
		
		return $boolUnset;
	}
	
	/* Loads Session Settings into an Object */
	private function LoadSettings() {

		/* If Settings Have Been Stored in Session, Add to Object's Variables, Else Create Session Storage Array */
		if (isset($_SESSION['SETTINGS'])) {
		
			$aSettings = &$_SESSION['SETTINGS'];
		
			foreach ($aSettings as $strSettingName => $mxSettingValue) {
					
				$this -> {$strSettingName} = &$aSettings[$strSettingName];
			}
		}
		else {
		
			$_SESSION['SETTINGS'] = array();
		}
	}
	
	/* Loads XML Settings into an Object, and Can Check if it is for This Site  */
	private function LoadXMLSettings($objXMLInfo, &$objSetStore, $boolURLVerify = true) {
	
		$boolConfigNotFound = true;	/* Indicator That Settings for This Site Has Not Been Found */
		/*$strURL = '';				/* URL to Compare */
		/*$aURLCompareList = Array(); /* List of Possible URL from Main One to Compare Against Site */
	
		if (is_object($objXMLInfo)) {

			if ($boolURLVerify) {
				
				/* Read the All of the Current Level Values in the XML File as Settings, Until it is Confirmed */
				foreach ($objXMLInfo as $strSettingName => $objSettingValue) {
					
					if (strtolower($strSettingName) == 'url') {
						
						if (stripos($_SERVER['SERVER_NAME'], (string)$objSettingValue) >= 0) {
						
							/* Confirm If the Configuration File for This Site Through Possible URLs */
							$strURL = (string)$objSettingValue;
						
							$aURLCompareList = Array($strURL,
													'www.' . $strURL,
													'http://' . $strURL,
													'https://' . $strURL,
													'http://www.' . $strURL,
													'https://www.' . $strURL);
						
							if (in_array($_SERVER['SERVER_NAME'], $aURLCompareList)) {
						
								$objSetStore -> url = $strURL;
								$boolConfigNotFound = false;
								break;
							}
						}
					}
					else if (strtolower($strSettingName) == 'redirect' && !isset($objSetStore -> redirect)) {
						
						$objSetStore -> redirect = (string)$objSettingValue;
					}
				}
			}

			/* If Verifying URL And Configuration File Found Or Not Verfying URL,
			Read the All of the Values in the XML File as Settings */
			if (($boolURLVerify && !$boolConfigNotFound) || !$boolURLVerify) {
				
				foreach ($objXMLInfo as $strSettingName => $objSettingValue) {
			
					if (strtolower($objSettingValue) != 'true') {
				
						if (strtolower($objSettingValue) != 'false') {
		
							if (!is_numeric($objSettingValue)) {
									
								if (!count($objSettingValue -> children())) {
									
									$objSetStore -> {$strSettingName} = (string)$objSettingValue;
								}
								else {
								    
								    $objSetStore -> {$strSettingName} = new stdClass();
									$this -> LoadXMLSettings($objSettingValue, $objSetStore -> {$strSettingName}, false);
								}
							}
							else {
									
								$objSetStore -> {$strSettingName} = (int)$objSettingValue;
							}
						}
						else {
									
							$objSetStore -> {$strSettingName} = false;
						}
					}
					else {
									
						$objSetStore -> {$strSettingName} = true;
					}
				}
			}
		}
		
		/* Return False If the Settings for this Site Has Been Found When Verifying URL, Else True */
	 	return $boolConfigNotFound;
	}

	/* Search for Directory on Higher Level */
	private function DirectoryAboveSearch(&$strDirectName, $nMaxLevel = 10) {

		$objThis = &$this;
		$objCurrentDirect;			/* Access to Selected Directory */
		$strCurrentLevel = '..';	/* Path of Current Directory Level */
					/* Current Count of Levels */
		$strFileName = '';			/* Name of a Selected File in the Selected Directory */
		$boolDirectNotFound = true;	/* Indicator That the Directory was not Found */
		
		while ($boolDirectNotFound && 
			   ($objCurrentDirect = opendir($strCurrentLevel)) !== false &&
			   $nLevelCount < $nMaxLevel) {

			if ($strCurrentLevel == '..') {

				$strCurrentLevel .= $objThis -> SystemSlashReplace('/');
			}
			
			while ($boolDirectNotFound && ($strFileName = readdir($objCurrentDirect)) !== false) {

				if (is_dir($strCurrentLevel . $strFileName) && $strFileName == $strDirectName) {

					$strDirectName = $strCurrentLevel . $strFileName;
					$objCurrentDirect = opendir($strDirectName);
					$boolDirectNotFound = false;
				}
			}
				
			$strCurrentLevel .= '..' . $objThis -> SystemSlashReplace('/');
			$nLevelCount++;
		}

		return $objCurrentDirect;
	}
	
	public function GetMsgStartIndicator() {
	    
	    return $strMsgStartIndictator;
	}
	
	public function GetMsgPartIndicator() {
	    
	    return $strMsgPartIndictator;
	}
	
	public function GetMsgEndIndicator() {
	    
	    return $strMsgEndIndictator;
	}
	
	public function GetMsgTableName() {
	    
	    return $strMsgTableName;
	}
	
	public function GetMsgIDColumnName() {
	    
	    return $strMsgIDColumnName;
	}
	
	public function GetMsgValueColumnName() {
	    
	    return $strMsgValueColumnName;
	}

	/* Logs Message */
	public function LogMsg($strLogMessage) {

		$this -> Log($strLogMessage);
	}

	/* Logs Error */
	public function LogError($strLogMessage)  {

		$this -> Log($strLogMessage, true);
	}

	/* Logs to File */
	private function Log($strLogMessage, $boolError = false) {

		$strLogDirectory = 'Logs';	/* Directory for Log Files Relative to Site Root  */
		$flLogFileAccess;			/* Access to Log File */
		$strLogFileName = date('Ymd') . '.txt';
									/* Log Filename */

		try {

			/* If the Directory Doesn't Exist, Create it */
			if (!file_exists($strLogDirectory)) {

				mkdir($strLogDirectory, 0777, true);
			}

			/* Setup Access to Log File, Write to, and Close It */
			if (($flLogFileAccess = fopen($strLogDirectory . '/' . $strLogFileName, 'a')) !== false) {

				if ($boolError) {

					$strLogMessage = 'ERROR: ' . $strLogMessage;
				}
					
				if (fwrite($flLogFileAccess, strftime('%I:%M:%S') . strstr((string)round((microtime(true) - time()), 3), '.') . ' ' .
											 strftime('%p') . ': ' . $strLogMessage . "\r\n") <= 0) {

					/* Writing Message to Log File Failed */
					$strLogMessage = '*** Error occurred during writing the following message to the log file. *** ' . $strLogMessage;
				}

				if (!fclose($flLogFileAccess)) {

					/* Closing Access to Log File Failed */
					$strLogMessage = '*** Error occurred during closing the log file after writing the following message. *** ' . $strLogMessage;
				}
			}
			else {

				/* Else Log File Access Error */
				$strLogMessage = ' *** Error occurred during accessing log file for writing the following message. *** ' . $strLogMessage;
			}
		}
		catch (Exception $exError) {

			/* Writing to Log File Failed */
			$strLogMessage = '*** Error occurred during writing the following message to the log file. Exception: ' .
							 $exError -> getMessage() . ' *** ' . $strLogMessage;
		}
	}
	
	/* Log Responses to Third Party Interactions */
	public function LogResponse($strRespFileName, $strRespMessage) {
	
		$strRespDirectory = 'responses';
									/* Responses Directory Relative from Site Root */
		$flRespFileAccess;			/* Pointer File for Saving Response File */
	
		try {
				
			/* If the Directory Doesn't Exist, Create it */
			if (!file_exists($strRespDirectory)) {
	
				mkdir($strRespDirectory, 0777, true);
			}
				
			/* Save Response File */
			if (($flRespFileAccess = fopen($strRespDirectory . $strRespFileName, 'a')) !== false) {
	
				if (fwrite($flRespFileAccess, $strRespMessage) <= 0) {
						
					/* Writing to Response File Failed */
					$this -> LogError('Error occurred during writing the response message to a file.');
				}
	
				if (!fclose($flRespFileAccess)) {
						
					/* Closing Access to File Failed */
					$this -> LogError('Error occurred during closing file for the response message.');
				}
			}
			else {
	
				/* Log File Access Error */
				$this -> LogError('Error occurred during creating file for the response message.');
			}
		}
		catch (Exception $exError) {
				
			/* Writing to Log File Failed, Inform Database of Error */
			$this -> LogError(' *** Error occurred during writing to the log file. Exception: ' . $exError -> getMessage() . ' *** ');
		}
	}

	/* Replace Slashes Based On Operation System */
	public function SystemSlashReplace($strSlashString) {

		if (!empty($PHP_OS) && strtoupper(substr($PHP_OS, 0, 3)) == 'WIN') {
			
			$strSlashString = str_replace('/', '\\', $strSlashString);
		}
		else {

			$strSlashString = str_replace('\\', '/', $strSlashString);
		}

		return $strSlashString;
	}
}

/* Create Object If It is not Already Stored in Session */
if (isset($_SESSION['CONFIG'])) {

	$CONFIG = &$_SESSION['CONFIG'];
	
	$CONFIG -> LoadSettings();
}
else {

	$CONFIG = new cConfig();
}

