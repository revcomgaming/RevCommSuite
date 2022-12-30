<?php
/* RevCommMain.php - Web Server Communications Script for RevCommSuite API

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

include_once 'RevCommConfig.php';

class cCommunications {
	
	private $strJSGlobalMsgsDesig = 'GLOBALS';
								/* The Designation for the JSON Output Messages for Updates to 
								   Global Javascript Variables and Function Calls */
	private $aJSONMsgs = Array();
								/* List of Output Messages in the Form of JSON Objects */
	private $aJSONMsgCurrent = Array();
								/* The Output Messages in the Form of a JSON Object That is Currently Being Created */
	function __construct() {
	
		global $CONFIG;
				
		if (!isset($CONFIG) || !$CONFIG -> host_settings_loaded) {
				
			echo "Error: Configuration module required for use of Communications module. Communications initialization failed.";
			exit();
		}

		/* If Store Configuration Object for Reloading */
		if ($CONFIG -> store_app_info) {
		
			$_SESSION['COMM'] = &$this;
		}
	}
	
	/* Sets up the Beginning of Form */
	public static function FormStart($strActPageName, $strFormName = '', $strFormAttr = '') {
		
		global $CONFIG;
		$strSiteURL = $CONFIG -> url;
									/* Site's URL */
		$aSiteFormInfo;				/* Site's Form Information */
		$nFormID = rand();			/* ID for the Form */
		$nFormMethodAttr = 0;		/* Position of the Form's "Method" Attribute in the Attributes If It's There */
		
		/* Setup to Store Site's Information, Especially the Form Information */
		if (empty($_SESSION[$strSiteURL])) {
			
			$_SESSION[$strSiteURL] = array('FORMS' => array());
		}
		
		if (empty($_SESSION[$strSiteURL]['FORMS'])) {
			
			$_SESSION[$strSiteURL]['FORMS'] = array();
		}
		
		$aSiteFormInfo = &$_SESSION[$strSiteURL]['FORMS'];
		
		/* Check ID, until an Unused One is Found */
		foreach ($aSiteFormInfo as $aFormInfo) {
			
			if ($nFormID == $aFormInfo['ID']) {
				
				$nFormID = rand();
				reset($aSiteFormInfo);
			}
		}
		
		/* If the Form Name was not Set */
		if ($strFormName == '') {
			
			$strFormName = 'frmCommForm_' . $nFormID;
		}
		
		$strFormAttr = str_replace('\'', '"', $strFormAttr);
		$nFormMethodAttr = stripos($strFormAttr, 'method');
		
		if ($nFormMethodAttr !== false) {
			
			$strFormAttr = str_replace($strFormAttr, substr($strFormAttr, $nFormMethodAttr, stripos($strFormAttr, '"', $nFormMethodAttr + 8)), $strFormAttr);
		}
		
		$aSiteFormInfo[] = array('ID' => $nFormID, 
								 'NAME' => $strFormName);
		
		return '<form name="' . $strFormName . '" action="' . $strActPageName . '" ' . $strFormAttr . ' method="post">
					<input type="hidden" name="ID" value="' . $nFormID . '">';
	}
	
	/* Sets up the End of the Form */
	public static function FormEnd($strSubmitBtnType = 'LINK', 
								   $strSubmitBtnText = 'SUBMIT',
								   $strButtonClassList = '', 
								   $boolUseWaitImage = true, 
								   $strWaitImgAltMsg = 'Please wait...') {
		
		$strFormEnd = '';			/* End of HTML Form */
		$strSubmitBtnHTMLClass = 'divCommFormButtonClass';
									/* HTML Class of Layer Containing Submit Button */
		
		/* Submit Button Type */
		switch (strtoupper($strSubmitBtnType)) {
			
			case 'LINK': {
				
				$strFormEnd .= '	<div class="' . $strSubmitBtnHTMLClass . ' ' . $strButtonClassList . '">
										<a href="#" name="aCommFormLink" title="' . $strSubmitBtnText . '">
											<span class="spCommFormText">' .
												$strSubmitBtnText .
										   '</span>
										</a>
									</div>';
				break;
			}
			case 'INPUT': {
				
				$strFormEnd .= '	<div class="' . $strSubmitBtnHTMLClass . ' ' . $strButtonClassList . '">
										<input type="button" name="inpCommFormButton" value="' . $strSubmitBtnText . '" />
									</div>';
				break;
			}
			case 'BUTTON': {
				
				$strFormEnd .= '	<div class="' . $strSubmitBtnHTMLClass . ' ' . $strButtonClassList . '">
										<button type="submit" name="btnCommFormButton">' . $strSubmitBtnText . '</button>
									</div>';
				break;
			}
		}
		
		if ($boolUseWaitImage) {
			
			$strFormEnd .= '	<div class="divCommFormWaitImgClass" style="display: none">
									<img name="imgCommFormWaitImg" src="" alt="' . $strWaitImgAltMsg . '" />
								</div>';
		}
		
		return $strFormEnd . '</form>';
	}
	
	/* Validates If Form is Authenitic */
	public static function FormValidate($strSentFormID = '') {
		
		global $CONFIG;
		$boolValidated = false;		/* Indicator That Form was Invalid */
		
		/* If the User Has Not Sent an ID, Get it from the Form POST */
		if (empty($strSentFormID)) {
			
			if (!empty($_POST['ID'])) {
				
				$strSentFormID = $_POST['ID'];
			}
		}
		
		if (!empty($strSentFormID)) {
			
			foreach ($_SESSION[$CONFIG -> url]['FORMS'] as $aFormInfo) {
				
				if ($strSentFormID == $aFormInfo['ID']) {
					
					$boolValidated = true;
					break;
				}
			}
		}
		
		/* Return True If Form was Validated, Else False */
		return $boolValidated;
	}
	
	/* Setup to Store a New JSON Output Message */
	public function JSONMsgCreate($strJSObjDesig = '') {
		
		$aJSONMsgCurrent = &$this -> aJSONMsgCurrent;
		
		if (empty($strJSObjDesig)) {
			
			$strJSObjDesig = $this -> strJSGlobalMsgsDesig;
		}
		
		$aJSONMsgCurrent = array('DESIGNATION' => $strJSObjDesig,
								 'VARUPDATES' => array(),
								 'FUNCCALLS' => array());
	}
	
	/* Ends Storage of Selected JSON Output Message */
	public function JSONMsgSaveClose() {
		
		$aJSONMsgCurrent = &$this -> aJSONMsgCurrent;
		$boolConfirmComplete = false;
									/* Indicator That the Operation was Done */
		
		if (!empty($aJSONMsgCurrent)) {
			
			$this -> aJSONMsgs[] = $aJSONMsgCurrent;
			$aJSONMsgCurrent = array();
			
			$boolConfirmComplete = true;
		}
		
		/* Return True That Message Saved and Closed, Else False */
		return $boolConfirmComplete;
	}
	
	/* Store Javascript Variable Update Information for Sending in the Next Output Message */
	public function JSONMsgJSVarUpdate($strJSVarName, $mxJSVarValue) {
		
		$boolConfirmComplete = false;
									/* Indicator That the Operation was Done */
		
		if (!empty($this -> aJSONMsgCurrent)) {
			
			$this -> aJSONMsgCurrent['VARUPDATES'][] = array('NAME' => $strJSVarName, 'VALUE' => $mxJSVarValue);
			
			$boolConfirmComplete = true;
		}
		
		/* Return True That Javascript Variable Update was Stored, Else False */
		return $boolConfirmComplete;
	}
	
	/* Store Javascript Function Calls Information for Sending in the Next Output Message */
	public function JSONMsgJSFuncCall($strJSFuncName, $mxJSFuncParm = '', $boolIsNotParamList = true) {
		
		$aJSONMsgCurrent = &$this -> aJSONMsgCurrent;
		$aJSFuncCalls;				/* Javascript Function Calls in JSON Output Message Being Put Together */
		$boolConfirmComplete = false;
									/* Indicator That the Operation was Done */
		
		if (!empty($aJSONMsgCurrent)) {
			
			$aJSFuncCalls = &$aJSONMsgCurrent['FUNCCALLS'];
			
			if (empty($mxJSFuncParm)) {
				
				$mxJSFuncParm = array();
			}
			else if ($boolIsNotParamList) {
				
				$mxJSFuncParm = array($mxJSFuncParm);
			}
			
			$aJSFuncCalls[] = array('NAME' => $strJSFuncName, 'PARAMS' => $mxJSFuncParm);
						
			$boolConfirmComplete = true;
		}
		
		/* Return True That Javascript Function Call was Stored, Else False */
		return $boolConfirmComplete;
	}
	
	/* Adds Parameter to First Stored Javascript Function Call Information Found for Sending in the Next Output Message */
	public function JSONMsgJSFuncAddParam($strJSFuncName, $mxJSFuncParm, $nParamIndex = null) {
		
		$aJSONMsgCurrent = &$this -> aJSONMsgCurrent;
		$aJSFuncCall;				/* The Selected Javascript Function Call in JSON Output Message Being Put Together */
		$boolConfirmComplete = false;
									/* Indicator That the Operation was Done */
		
		if (!empty($aJSONMsgCurrent)) {
			
			if (!empty($aJSONMsgCurrent['FUNCCALLS'])) {
				
				$aJSFuncCall = &$aJSONMsgCurrent['FUNCCALLS'];
				
				foreach ($aJSFuncCall as $nFuncIndex => $aFuncCallInfo) {
					
					if ($aFuncCallInfo['NAME'] == $strJSFuncName) {
						
						if (!is_numeric($nParamIndex)) {
							
							$nParamIndex = count($aFuncCallInfo['PARAMS']);
						}
						
						array_splice($this -> aJSONMsgCurrent['FUNCCALLS'][$nFuncIndex]['PARAMS'], $nParamIndex, 0, $mxJSFuncParm);
						
						$boolConfirmComplete = true;
						break;
					}
				}
			}
		}
		
		/* Return True That Javascript Function Call Param was Added, Else False */
		return $boolConfirmComplete;
	}
	
	/* Clear All Messages and Send Error to be Outputted Through Javascript in JSON Output Message */
	public function JSONMsgError($strErrorMsg) {
		
		$this -> JSONMsgSaveClose();
		$this -> JSONMsgCreate('ERROR');
		$this -> JSONMsgJSFuncCall('Show', $strErrorMsg);
		$this -> JSONMsgSaveClose();
	}
	
	/* Clears All Store Messages Including the One Currently Being Created */
	public function JSONMsgsClear() {
		
		$this -> aJSONMsgCurrent = array();
		$this -> aJSONMsgs = array();
	}
	
	/* Outputs Stored Messages as Javascript Array of JSON Objects, and Clear Them Out */
	public function JSONMsgsOutput($boolPrintOut = true) {
				
		$aJSONMsgs = &$this -> aJSONMsgs;
		$strMsg = '';				/* Outputted Message */
		$boolAddComma = false;		/* Indicator to Add Comma to End Output of the Previously Selected JSON Object */
		
		/* If Any Messages Output Them, and Clear the Queue */
		if (count($aJSONMsgs) > 0) {
			
			$strMsg = '[';

			foreach ($aJSONMsgs as $aJSONMsgSelect) {
				
				if ($boolAddComma) {
					
					$strMsg .= ',';
				}
				
				$strMsg .= json_encode($aJSONMsgSelect);
				
				$boolAddComma = true;
			}
			
			$strMsg .= ']';
			
			if ($boolPrintOut) {

				echo $strMsg;
			}
			
			$this -> JSONMsgsClear();
		}
		
		return $strMsg;
	}
	
	/* Creates Server Command "SENDMSGUSER" from Stored JSON Message Before Clearing its Stored Information */
	public function CreateSendMsgUser($nClientID) {
	    
	    global $CONFIG;
	    
	    return $CONFIG -> GetMsgStartIndicator() . 'SENDMSGUSER' . $CONFIG -> GetMsgPartIndicator() . $nClientID . 
	           $CONFIG -> GetMsgPartIndicator() . $this -> JSONMsgsOutput(false) . $CONFIG -> GetMsgEndIndicator();
	}
	
	/* Log Responses to Third Party Interactions */
	public function LogResponse($strRespFileName, $strRespMessage) {
		
		global $CONFIG;

		$CONFIG -> LogResponse($strRespFileName, $strRespMessage);
	}

	/* Logs Error  */
	public function LogCommError($strLogMessage) {
	
		global $CONFIG;

		$CONFIG -> LogError($strLogMessage);
		$this -> JSONMsgError($strLogMessage);
	}
	
	/* Logs Error (Depreciated) */
	public function LogError($strLogFileName, $strLogMessage) {
		
		$this -> LogCommError($strLogMessage);
	}
	
	/* Logs Error Message to Generic Daily File (Depreciated) */
	public function LogGenericError($strLogMessage) {

		global $CONFIG;

		$CONFIG -> LogError($strLogMessage);
	}
}

/* Create Object If It is not Already Stored in Session */
if (isset($_SESSION['COMM'])) {

	$COMM = &$_SESSION['COMM'];
}
else {

	$COMM = new cCommunications();
}
?>