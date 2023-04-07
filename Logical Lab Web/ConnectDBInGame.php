<?php
    ob_start();
    $servername = "localhost";
    $server_username = "id3753999_logicallab";
    $server_password = "logicallab";
    $dbName = "id3753999_logicallab";
    
    try
    {
        $conn = new mysqli($servername,$server_username,$server_password,$dbName);
        
        if (mysqli_connect_error())	
        {
 		    throw new Exception("Can't connect database.");
        }
    }
    catch (Exception $e)
    {
        //echo $e->getMessage();
    }
    
?>
