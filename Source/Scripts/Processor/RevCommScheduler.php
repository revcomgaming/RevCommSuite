<?php
/* RevCommCron.php - Web Server Scheduler Management Script for RevCommSuite API

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
include_once 'RevCommData.php';

/* Scheduler Class */
class cScheduler {
    
    private $aEvents = Array();
                                    /* List of Database Event Designations */
    
    function __construct() {
        
        global $CONFIG;
        global $DATA;
        $aEvents = &$this -> aEvents;
                                    /* List of Database Event Designations */
        $strScriptURL = "http://";	/* URL of this Script */
        $strScriptFolderPath = "Cron/";	
                                    /* Path of Folder Containing Scripts for Running as Cron Jobs */
        $aJobInfo;                  /* Holder for Scheduler Job Information */
        $nDelay = 0;                /* Amount of Start Delay in Milliseconds */
        $strDataStatement = "";     /* Database Statement for Setting Up Events */
        $dtCurrentDate = new DateTime();
                                    /* Current Time Information */
        
        try {
            
            if (!isset($CONFIG) || !$CONFIG -> host_settings_loaded) {
                
                echo "Error: Configuration module required for use of Scheduler module. Scheduler initialization failed.";
                exit();
            }
            
            if (!isset($DATA)) {
                
                echo "Error: Data module required for use of Scheduler module. Scheduler initialization failed.";
                exit();
            }
            
            $aJobInfo = $CONFIG -> processing -> scheduler;
            
            if (count($aJobInfo)) {
                
                try {
                    
                    $DATA -> Execute("SET GLOBAL event_scheduler = ON;");
                }
                catch (Exception $exError) {
                    
                    $CONFIG -> LogError("During setting up scheduler events, turning on event scheduler failed. Exception: " . $exError -> getMessage());
                }
            }
            
            foreach ($aJobInfo as $objJobInfo) {
                
                $aEvents[] = $objJobInfo -> designation;
                
                if ($objJobInfo -> enabled) {
                    
                    if (isset($objJobInfo -> designation) && 
                        isset($objJobInfo -> interval) &&
                        isset($objJobInfo -> interval_type) &&
                        isset($objJobInfo -> data_statement) && 
                        !empty($objJobInfo -> designation) &&
                        (!empty($objJobInfo -> interval) || 
                         (int)$objJobInfo -> interval >= 0) &&
                        !empty($objJobInfo -> interval_type) &&
                        !empty($objJobInfo -> data_statement)) {
                            
                        $nDelay = 0;
                        
                        if (isset($objJobInfo -> delay) && (int)$objJobInfo -> delay > 0) {
                            
                            $nDelay = (int)$objJobInfo -> delay;
                        }
                        
                        $strDataStatement = "CREATE EVENT IF NOT EXISTS " . $objJobInfo -> designation . " " .
                                            "ON SCHEDULE ";
                        
                        if (strtolower($objJobInfo -> interval_type) != "once") {
                            
                            $strDataStatement .= "EVERY " . $objJobInfo -> interval . " " . $objJobInfo -> interval_type . " " .
                                                 "STARTS DATE_ADD(NOW(), INTERVAL " . $nDelay . " MICROSECOND) " .
                                                 "ON COMPLETION PRESERVE ";
                        }
                        else {
                            
                            $strDataStatement .= "AT DATE_ADD(NOW(), INTERVAL " . $nDelay . " MICROSECOND) " .
                                                 "ON COMPLETION NOT PRESERVE ";
                        }
                        
                        $strDataStatement .= "DO BEGIN " . $objJobInfo -> data_statement;
                        
                        if (substr(rtrim($strDataStatement), -1) != ";") {
                             
                            $strDataStatement = rtrim($strDataStatement) . ";";
                        }
                            
                        $DATA -> Execute($strDataStatement . " END", true);
                    }
                    else {
                        
                        $CONFIG -> LogError("During setting up scheduler events, settings are missing, designation: " . $objJobInfo -> designation . ".");
                    }
                }
                else {
                    
                    $DATA -> Execute("DROP EVENT IF EXISTS " . $objJobInfo -> designation, true);
                }
            }
            
            /* If Store Configuration Object for Reloading */
            if ($CONFIG -> store_app_info) {
                
                $_SESSION['SCHEDULER'] = &$this;
            }
        }
        catch (Exception $exError) {
            
            $CONFIG -> LogError("During setting up scheduler events, an exception occurred. Exception: " . $exError -> getMessage());
        }
    }
    
    /* Get List of Enabled Events */
    public function GetEventNames() {
        
        return $this -> aEvents;
    }
}

/* Create Object If It is not Already Stored in Session */
if (isset($_SESSION['SCHEDULER'])) {
    
    $SCHEDULER = &$_SESSION['SCHEDULER'];
}
else {
    
    $SCHEDULER = new cScheduler();
}
