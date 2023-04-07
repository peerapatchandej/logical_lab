<?php
    require_once('ConnectDBInGame.php');
    
    $name = mysqli_real_escape_string($conn,$_POST['name']);
    $surname = mysqli_real_escape_string($conn,$_POST['surname']);
    $email = mysqli_real_escape_string($conn,$_POST['email']);
    /*$level = mysqli_real_escape_string($conn,$_POST['level']);*/
    
    
    $sql = "UPDATE users
         SET Name = '$name',Surname = '$surname'
         WHERE Email = '$email'";
    $result = mysqli_query($conn,$sql) or mysqli_error();
    
    if($result)
    {
        echo "Update user data complete.";
    }
    
    mysqli_close($conn);
?>