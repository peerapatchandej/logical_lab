<?php
    require_once('ConnectDBInGame.php');
    
    $email = mysqli_real_escape_string($conn,$_POST['email']);
    $level = mysqli_real_escape_string($conn,$_POST['level']);
    
    if($level == "1"){
        $sql = "SELECT Email FROM users WHERE Email = '$email' AND Level_1 = 1";
    }
    else if($level == "2"){
        $sql = "SELECT Email FROM users WHERE Email = '$email' AND Level_2 = 1";
    }
    else if($level == "3"){
        $sql = "SELECT Email FROM users WHERE Email = '$email' AND Level_3 = 1";
    }
    $result = mysqli_query($conn,$sql) or mysqli_error();
    
    if($result){
        if(mysqli_num_rows($result) > 0){
            echo "Found user data in database.";
        }
        else{
            $sql = "SELECT Email FROM users WHERE Email = '$email'";
            $result = mysqli_query($conn,$sql) or mysqli_error();
            
            if($result){
                if(mysqli_num_rows($result) > 0){
                    echo "Update user data in database.";
                }
                else{
                    echo "Not found user in database.";
                }
            }
        } 
    }
    
    mysqli_close($conn);
?>