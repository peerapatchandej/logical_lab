<?php
    session_start();
    require_once("ConnectDBInGame.php");
    
    $Level = mysqli_real_escape_string($conn,$_POST["level"]);
    
    $sql = "SELECT users.User_ID,Name,Surname,MAX(answers.Send_Time) as SendTime,Checked
         FROM users INNER JOIN answers
         ON users.User_ID = answers.User_ID 
         WHERE answers.Send_Time = (SELECT MAX(answers.Send_Time) FROM answers WHERE users.User_ID = answers.User_ID AND answers.Level = '$Level')
         GROUP BY users.User_ID , answers.Send_Time";
         
    $result = mysqli_query($conn,$sql) or mysqli_error();
    
    if($result)
    {
        if(mysqli_num_rows($result) > 0)
        {
            echo "Answer";
            while($row = mysqli_fetch_assoc($result))
            {
              echo $row['User_ID'].";".$row['Name'].";".$row['Surname'].";".$row['SendTime'].";".$row['Checked']."\n";
            }
        }
        else{
            echo "No data.";
        }
    }
    
    mysqli_close($conn);
?>
