<?php
/* RevCommProxy.php - Web Server Script Inclusion Script for RevCommSuite API
 
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

	/* $objConfigInfo = $CONFIG -> processing; 
									/* Processing Configuration Information */
	/* $strScriptFolderPath = "Scripts"; 
									/* Path of Folder Containing Scripts */
	$aScriptIncludeInfo = [];  		/* List of Information on What Script Files in the "Script" Folder to Include */
	$boolUseMain = false;			/* Indicator to Use Main Module */
	$boolIncludeAll = false;		/* Indicator to Include All Files in the "Script" Folder */
	$objScriptDirect;				/* Access to Directory for Script Files */
	$strErrorMsg = '';				/* Error Message */

	include_once 'RevCommConfig.php';

	if (!isset($CONFIG)) {
			
		echo "Error: Configuration module required for use of Proxy module. Processing initialization failed.";
		exit();
	}

	$objConfigInfo = $CONFIG -> processing; 
	
	if (!isset($objConfigInfo -> use_data) || $objConfigInfo -> use_data) {

		include_once 'RevCommData.php';

		if (!isset($DATA)) {
				
			echo "Error: Data module required for use of Proxy module. Processing initialization failed.";
			exit();
		}
	}

	if (!isset($objConfigInfo -> use_main) || $objConfigInfo -> use_main) {
	
		include_once 'RevCommMain.php';

		if (isset($COMM)) {

			$boolUseMain = true;
		}
		else {	

			echo "Error: Communication module required for use of Proxy module. Processing initialization failed.";
			exit();
		}
	}

	$strScriptFolderPath = $CONFIG -> SystemSlashReplace("Scripts/");

	if (!file_exists($strScriptFolderPath)) {

		if (!mkdir($strScriptFolderPath)) {

			LogProxyError('During proxy processing, creating Proxy script folder, "' . $strScriptFolderPath . 
						  '", failed. Processing initialization failed.');
			
			exit();
		}
	}

	if (isset($objConfigInfo -> proxy -> include_scripts)) {

		$aScriptIncludeInfo = explode(",", strtolower($objConfigInfo -> proxy -> include_scripts));

		foreach ($aScriptIncludeInfo as $strFileSelect) {

			if ($strFileSelect == "*") { 

				$boolIncludeAll = true;
				break;
			}
		}
	}

	try {

		/* Get Access to the Directory Containing the PHP Script Files  */
		if (($objScriptDirect = opendir($strScriptFolderPath)) !== false) {

			/* Cycle Through All of the Files in the Directory Looking for the PHP Script Files */
			while (($strFileName = readdir($objScriptDirect)) !== false) {
		
				if (($boolIncludeAll || in_array(strtolower($strFileName), $aScriptIncludeInfo)) && 
					(strrpos($strFileName, '.php') + 4) == strlen($strFileName))  {
			
					include_once $strScriptFolderPath . $strFileName;
				}
			}
		
			closedir($objScriptDirect);
		}
		else {

			LogProxyError('During proxy processing, opening script\'s directory path "' . $strScriptFolderPath . 
						  '" folder failed.');
		}
	}
	catch (Exception $exError) {

		LogProxyError('During proxy processing, while including scripts from the "' . $strScriptFolderPath . 
					  '" folder, an exception occurred. Exception: ' . $exError -> getMessage());
	}

	if (!empty($_POST['RevCommProxyPage'])) {
		
		try {
			
			include_once $strScriptFolderPath . $_POST['RevCommProxyPage'];
		}
		catch (Exception $exError) {

			LogProxyError('During proxy processing, including post page, "' . $_POST['RevCommProxyPage'] . '", failed. Exception: ' . $exError -> getMessage());
		}
	}

	function LogProxyError($strMsg) {

		global $COMM;
		global $boolUseMain;

		if ($boolUseMain) {

			$COMM -> LogGenericError($strMsg);
		}
		else {

			echo $strMsg;
		}
	}
?>