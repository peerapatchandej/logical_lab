<?php
    session_start();
    ob_start();
    require_once('PHPMailer/PHPMailerAutoload.php');
    require_once("ConnectDBInGame.php");
    
    if(isset($_SESSION['username']) && isset($_SESSION['lastUpdate']))
    {
        $sql = "SELECT Last_Update FROM admins WHERE Email = '".$_SESSION['username']."' 
        AND Last_Update = '".$_SESSION['lastUpdate']."'";
        $result = mysqli_query($conn,$sql) or mysqli_error();
        
        if($result)
        {
            if(mysqli_num_rows($result) <= 0)
            {
                echo "Signed in overlapping.";
            }
            else
            {
                echo "Have Session.";
            }
        }
    }
    else
    {
        echo "No have session.";
    }
?>