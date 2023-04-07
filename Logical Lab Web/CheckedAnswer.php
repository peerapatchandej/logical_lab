<?php
    session_start();
    ob_start();
    require_once('ConnectDB.php');
    
    $user_id = mysqli_real_escape_string($conn,$_POST["user_id"]);
    $send_time = mysqli_real_escape_string($conn,$_POST["send_time"]);
    
    $sql = "SELECT Admin_ID,Name,Lastname FROM admins WHERE Email = '".$_SESSION['username']."'";
    $result = mysqli_query($conn,$sql) or mysqli_error();
    
    if($result)
    {
        if(mysqli_num_rows($result) > 0)
        {
            if($row = mysqli_fetch_assoc($result))
            {
                $admin_name = $row['Name']." ".$row['Lastname'];
                $sql = "UPDATE answers
                SET Checked = 1,Admin_ID = '".$row['Admin_ID']."',Admin_Name = '".$admin_name."'
                WHERE User_ID = '$user_id' AND Send_Time = '$send_time'";
                $result = mysqli_query($conn,$sql) or mysqli_error();
                
                if($result)
                {
                    echo "Checked success.";
                }
            }
        }
    }
    
    
    mysqli_close($conn);
?>
