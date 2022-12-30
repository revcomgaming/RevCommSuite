<?php
/* RevCommData.php - Web Server Data Script for RevCommSuite API

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

/* Database Interaction Class */
class cDataInteract {
	
	private $dbConnect;				/* Database Connection Object */

	function __construct() {
		
		global $CONFIG;
		$dbConnect;					/* Data Connection */
		$objData;					/* Database Connection Information */
		$boolError = false;			/* Indicator That Error Has Occurred */
		
		try {
			
			if (isset($CONFIG) && $CONFIG-> host_settings_loaded) {
				
				try {
					
					$objData = $CONFIG -> database;
					
					/* If Database Connection was Made */
					$dbConnect = new PDO($objData -> type . ':host=' . $objData -> server . ';dbname=' . $objData -> name,
							 			 $objData -> username, 
										 $objData -> password);
						
					if (!$dbConnect -> setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION)) {
						
						$this -> WarningCheck('Error occurred during database connection.',
											  'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $CONFIG -> hostname .
											  '", database connection settings updated failed.');
					}
					
					$this -> dbConnect = &$dbConnect;
				}
				catch (PDOException $peError) {
						
					$this -> WarningCheck('Exception Occurred During Connecting to Database.',
										  'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $CONFIG -> hostname .
										  '", database connection failed. Exception: ' . $peError -> getMessage());
					$boolError = true;
				}
					
				$this -> CredentialClear();

				/* If Store Configuration Object for Reloading */
				if ($CONFIG -> store_app_info) {
				
					$_SESSION['DATA'] = &$this;
				}
			}
			else {
				
				echo "Error: Configuration module required for use of DataInteract module. DataInteract initialization failed.";
				exit();
			}
		}
		catch (Exception $exError) {
			
			/* Connecting to Database Failed, Set Indicator, and Inform Admin by E-mail */
			$boolError = true;
				
			$this -> WarningCheck('Exception Occurred During Connecting to Database.', 
				      			  'User at IP: "' .  $_SERVER['REMOTE_ADDR'] . '", Host Name: "' . $CONFIG -> hostname . 
				      			  '", database connection failed. Exception: ' . $exError -> getMessage());
		}
		
		/* If an Error Has Occurred and Showing Errors, Inform User */
		if ($boolError) {
			
			if ($CONFIG -> database -> connection_warn) {
				
				echo '<script language="javascript" type="text/javascript">
				
						   alert("Could not establish connection to server' . $strUserNotifyMessage . '. Please refresh page to try again.");  
	
					   </script>';
			}
								    	  
			/* Stop Script Execution */
			exit();				
		}
	}
	
	/* Query Database Using Safe Parameter for Passed Values */
	function QuerySafeParam($strStatement, $mxParams, &$arsInfo) {
	
		$dbConnect = &$this -> dbConnect;
		$psResults;					/* Database Results */
		$drRow;						/* Data Record from Results */
		$nResult = 0;				/* Number of Rows Returned by Query */
			
		try {
			
			$psResults = $dbConnect -> prepare($strStatement);
				
			if (!is_object($mxParams) && !is_array($mxParams)) {
			
				$mxParams = [$mxParams];
			}
				
			$psResults -> execute($mxParams);
	
			/* Get Results and Number of Rows */
			$nResult = $psResults -> rowCount();
			$arsInfo = array();
				
			while ($drRow = $psResults -> fetchObject()) {
	
				$arsInfo[] = $drRow;
			}
		}
		catch (PDOException $peError) {
				
			/* Throw Exception to Function Caller */
			throw $peError;
		}
	
		/* Return Number of Rows Returned by Query */
		return $nResult;
	}
	
	/* Query Database and Return Array */
	function QuerySafeParamArray($strStatement, $mxParams, &$aInfo) {
	
		$dbConnect = &$this -> dbConnect;
		$psResults;					/* Database Results */
		$nResult = 0;				/* Number of Rows Returned by Query */
			
		try {
			
			$psResults = $dbConnect -> prepare($strStatement);
				
			if (!is_object($mxParams) && !is_array($mxParams)) {
			
				$mxParams = [$mxParams];
			}
				
			$psResults -> execute($mxParams);
	
			/* Get Results and Number of Rows */
			$nResult = $psResults -> rowCount();
			$aInfo = $psResults -> fetchAll();
		}
		catch (PDOException $peError) {
				
			/* Throw Exception to Function Caller */
			throw $peError;
		}
	
		return $nResult;
	}
	
	/* Query Database */
	function Query($strStatement, &$arsInfo) {
		
		$dbConnect = &$this -> dbConnect;
		$psResults;					/* Database Results */
		$drRow;						/* Data Record from Results */
		$nResult = 0;				/* Number of Rows Returned by Query */
			
		try {
			
			$psResults = $dbConnect -> query($strStatement);

			/* Get Results and Number of Rows */
			$nResult = $psResults -> rowCount();
			$arsInfo = array();
			
			while ($drRow = $psResults -> fetchObject()) {
				
				$arsInfo[] = $drRow;
			}
		}
		catch (PDOException $peError) {
			
			/* Throw Exception to Function Caller */
			throw $peError;
		}

		/* Return Number of Rows Returned by Query */
		return $nResult;
	}
	
	/* Query Database and Return Array */
	function QueryArray($strStatement, &$aInfo) {

		$dbConnect = &$this -> dbConnect;
		$psResults;					/* Database Results */
		$nResult = 0;				/* Number of Rows Returned by Query */
			
		try {
			
			$psResults = $dbConnect -> query($strStatement);

			/* Get Results and Number of Rows */
			$nResult = $psResults -> rowCount();
			$aInfo = $psResults -> fetchAll();
		}
		catch (PDOException $peError) {
			
			/* Throw Exception to Function Caller */
			throw $peError;
		}
		
		return $nResult;
	}
	
	/* Execute Database Statement */
	function Execute($strStatement, $boolUpdate = false) {
		
		$dbConnect = &$this -> dbConnect;
		$nResult = 0;			/* Number of Rows Affected by Statement or ID for Inserted Information */

		try {
			
			if (($nResult = $dbConnect -> exec($strStatement))) {
				
				/* If the Statement was Executed Successfully, and it was an not Update Statement */
				if (!$boolUpdate) {
					
					/* Get the ID of the Newly Created Record to be Returned by the Function */
					$nResult = $dbConnect -> lastInsertId();
				}
			}
			else {
				
				/* Else Return -1 for Error */
				$nResult = -1;
			}
		}
		catch (PODException $peError) {
			
			/* Throw Exception to Function Caller */
			throw $peError;
		}

		/* Return, If an Update Database Statement was Successfully Executed, the Number of Affected Rows,  or If an Insert Database Statement is
		   Successfully Executed, it will be the ID of Newly Created Record, or -1 If Failure */
		return $nResult;
	}

	/* Execute Database Statement Using Safe Parameter for Passed Values */
	function ExecuteSafeParam($strStatement, $mxParams, $boolUpdate = false) {
	
		$dbConnect = &$this -> dbConnect;
		$nResult = 0;			/* Number of Rows Affected by Statement or ID for Inserted Information */
	
		try {
			
			$psResults = $dbConnect -> prepare($strStatement);
				
			if (!is_object($mxParams) && !is_array($mxParams)) {
			
				$mxParams = [$mxParams];
			}
				
			if ($psResults -> execute($mxParams)) {
	
				/* If the Statement was Executed Successfully, and it was an not Update Statement */
				if (!$boolUpdate) {
						
					/* Get the ID of the Newly Created Record to be Returned by the Function */
					$nResult = $dbConnect -> lastInsertId();
				}
				else {
					
					/* Get Number of Rows Updated */
					$nResult = $psResults -> rowCount();
				}
			}
			else {
	
				/* Else Return -1 for Error */
				$nResult = -1;
			}
		}
		catch (PODException $peError) {
				
			/* Throw Exception to Function Caller */
			throw $peError;
		}
	
		/* Return, If an Update Database Statement was Successfully Executed, the Number of Affected Rows,  or If an Insert Database Statement is
		   Successfully Executed, it will be the ID of Newly Created Record, or -1 If Failure */
		return $nResult;
	}
	
	/* Starts Database Transaction */
	function TransactionStart() {
		
		$dbConnect = &$this -> dbConnect;
		$boolTransStart = false;	/* Indicator That Database Transaction was Started */
		
		try {
			
			/* Execute Statement to Stop Auto-Comment Database Statements */
			$boolTransStart = $dbConnect -> beginTransaction();
		}
		catch (PODException $peError) {
			
			/* Rethrow Exception to Function Caller */
			throw $peError; 
		}
		
		/* Return True If Database Transaction was Started, Else False */
		return $boolTransStart;
	}

	/* Ends Database Transaction by Committing Pending Database Statements or Rolling Them Back */
	function TransactionEnd($boolCommit) {
		
		$dbConnect = &$this -> dbConnect;
		$boolTransEnd = false;		/* Indicator That Database Transaction was Ended */
		
		try {
			
			/* If Committing Pending Database Statements or Rolling Them Back, Set Database Statment */
			if ($boolCommit) {
				
				$boolTransEnd = $dbConnect -> commit();
			}
			else {
				
				$boolTransEnd = $dbConnect -> rollBack();
			}
		}
		catch (PODException $poError) {
			
			/* Rethrow Exception to Function Caller */
			throw $poError;
		}
		
		/* Return True If Database Transaction was Ended, Else False */
		return $boolTransEnd;
	}
	
	/* Push Command to Server for Processing */
	function PushServerCommand($strCommandMsg) {
	   
	    global $CONFIG;
	    
	    $this -> Execute('INSERT INTO ' . $CONFIG -> GetMsgTableName() . ' (' . $CONFIG -> GetMsgValueColumnName() . ') VALUES (\'' . $strCommandMsg . '\')');
	    global $CONFIG;
	}
	
	/* Check If Warning Should be Sent Out, and If Anything Fails, Ignore it */
	private function WarningCheck($strWarnTitle, $strWarnMsg) {
		
		global $CONFIG;
		$boolSendMsg = false;		/* Indicator to Send Warning */
		
		try {
			
			if (isset($CONFIG -> warning_last_sent)) {
				
				if ((time() - $CONFIG -> warning_last_sent) / 60 > (int)$CONFIG -> database -> warn_interval_mins) {
					
					$boolSendMsg = true;
				}
			}
			else {
				
				$boolSendMsg = true;
				$CONFIG -> warning_last_sent = time();
			}
			
			if ($boolSendMsg && !empty($CONFIG -> email)) {
				
				mail($CONFIG -> email, $CONFIG -> url . ': ' . $strWarnTitle, $strWarnMsg);
			}
			
			$CONFIG -> LogMsg($strWarnMsg);
		}
		catch(Exception $exError){

			$CONFIG -> LogError("During checking for sending warning message, '" . $strWarnMsg . "', an exception occurred. Exception: " . $exError -> getMessage());
		}
	}

	public function StringEscape($strInput) {

		$strQuotedString = $this -> dbConnect -> quote($strInput);
		return substr($strQuotedString, 1, strlen($strQuotedString) - 2);
	}
	
	/* Clears Database Credentials If Specified by Configuration Settings */
	public function CredentialClear() {
		
		global $CONFIG;
		$objData = &$CONFIG -> database;
									/* Configuration Database */
		
		if ($objData -> credential_remove) {
			
			unset($objData -> database);
			unset($objData -> username);
			unset($objData -> password);
		}
	}
	
	function __destruct() {
		
		$this -> dbConnect = null;
	}
}

/* Create Object If It is not Already Stored in Session */
if (isset($_SESSION['DATA'])) {

	$DATA = &$_SESSION['DATA'];
}
else {

	$DATA = new cDataInteract();
}
?>