<?php
    require_once('ConnectDBInGame.php');

    $name = mysqli_real_escape_string($conn,$_POST['name']);
    $surname = mysqli_real_escape_string($conn,$_POST['surname']);
    $email = mysqli_real_escape_string($conn,$_POST['email']);
    /*$level = mysqli_real_escape_string($conn,$_POST['level']);*/
    
    date_default_timezone_set("Asia/Bangkok");
    $datetime = date('Y-m-d H:i:s');
    
    /*if($level == "1")
    {
        $sql = "INSERT INTO users (Name,Surname,Email,Create_Date,Level_1)
                VALUES ('$name','$surname','$email','$datetime',1)";
    }
    else if($level == "2")
    {
        $sql = "INSERT INTO users (Name,Surname,Email,Create_Date,Level_2)
                VALUES ('$name','$surname','$email','$datetime',1)";
    }
    else if($level == "3")
    {
        $sql = "INSERT INTO users (Name,Surname,Email,Create_Date,Level_3)
                VALUES ('$name','$surname','$email','$datetime',1)";
    }*/
    
    $sql = "INSERT INTO users (Name,Surname,Email,Create_Date)
                VALUES ('$name','$surname','$email','$datetime')";
    
    $result = mysqli_query($conn,$sql) or mysqli_error();
    
    if($result)
    {
        echo "Insert user data complete.";
    }
    
    mysqli_close($conn);
?>
