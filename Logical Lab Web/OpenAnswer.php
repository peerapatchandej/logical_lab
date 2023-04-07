<?php
    session_start();
    ob_start();
    require_once('ConnectDBInGame.php');
    
    $user_id = mysqli_real_escape_string($conn,$_POST["user_id"]);
    $Level = mysqli_real_escape_string($conn,$_POST["level"]);
    
    //Find last date time 
    $sql = "SELECT MAX(Send_Time) as Send_Time FROM answers WHERE User_ID = $user_id AND Level = '$Level'";
    $result = mysqli_query($conn,$sql) or mysqli_error();;
    
    if($result)
    {
        if(mysqli_num_rows($result) > 0)
        {
            while($row = mysqli_fetch_assoc($result))
            {
                $sendtime = $row['Send_Time'];
            }
            
            //Find answer data
            $sql = "SELECT CmdName,CmdId,CmdType,CmdSleep,CmdSpeed,CmdConId,CmdInnerId,CmdPort,CmdConType,CmdConValue,CmdRange,
                         CmdLoopId,CmdLoopType,CmdLoopCount,CmdTmpLoopCount,CmdEndType,CmdSource,CmdDes,CmdEndIfDes
                  FROM answers WHERE User_ID = $user_id AND Level = '$Level' AND Send_Time = '$sendtime';
                  GROUP BY Send_Time";
            $result = mysqli_query($conn,$sql) or mysqli_error();;
            
            if($result)
            {
                if(mysqli_num_rows($result) > 0)
                {
                    echo "Answer";
                    while($row = mysqli_fetch_assoc($result))
                    {
                      echo $row['CmdName'].";".$row['CmdId'].";".$row['CmdType'].";".$row['CmdSleep'].";".$row['CmdSpeed'].";".
                           $row['CmdConId'].";".$row['CmdInnerId'].";".$row['CmdPort'].";".$row['CmdConType'].";".$row['CmdConValue'].";".
                           $row['CmdRange'].";".$row['CmdLoopId'].";".$row['CmdLoopType'].";".$row['CmdLoopCount'].";".$row['CmdTmpLoopCount'].";".
                           $row['CmdEndType'].";".$row['CmdSource'].";".$row['CmdDes'].";".$row['CmdEndIfDes']."\n";
                    }
                }
                else
                {
                    echo "No data.";
                }
            }
        }
        else
        {
            echo "No data.";
        }
    }
    
    
?>
