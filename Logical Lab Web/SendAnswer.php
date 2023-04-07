<?php
    require_once('PHPMailer/PHPMailerAutoload.php');
    require_once('ConnectDBInGame.php');

    $Level = mysqli_real_escape_string($conn,$_POST["level"]);
    $Cmd = $_POST["cmd"];
    $name = $_POST['name'];
    $surname = $_POST['surname'];
    $email = $_POST['email'];

    //Find User Email
    $sql = "SELECT User_ID,Email FROM users WHERE Email = '$email'";
    $result = mysqli_query($conn,$sql) or mysqli_error();

    if($result)
    {
        if(mysqli_num_rows($result) > 0)
        {
            while($row = mysqli_fetch_assoc($result))
            {
              $user_id = intval($row['User_ID']);
            }
        }
    }

    //Insert Answer Data
    date_default_timezone_set("Asia/Bangkok");
    $datetime = date('Y-m-d H:i:s');
    $RowCmd = explode("\n",$Cmd);
    $count = 0;

    for($i = 0; $i < count($RowCmd) - 1; $i++)
    {
        $AttrCmd = explode(";",$RowCmd[$i]);

        $sql = "INSERT INTO answers (CmdName,CmdId,CmdType,CmdSleep,CmdSpeed,CmdConId,CmdInnerId,CmdPort,CmdConType
                ,CmdConValue,CmdRange,CmdLoopId,CmdLoopType,CmdLoopCount,CmdTmpLoopCount,CmdEndType,CmdSource,
                CmdDes,CmdEndIfDes,Level,User_ID,Send_Time)
                VALUES ('".$AttrCmd[0]."','".$AttrCmd[1]."','".$AttrCmd[2]."','".$AttrCmd[3]."','".$AttrCmd[4]."','".
                        $AttrCmd[5]."','".$AttrCmd[6]."','".$AttrCmd[7]."','".$AttrCmd[8]."','".$AttrCmd[9]."','".
                        $AttrCmd[10]."','".$AttrCmd[11]."','".$AttrCmd[12]."','".$AttrCmd[13]."','".$AttrCmd[14]."','".
                        $AttrCmd[15]."','".$AttrCmd[16]."','".$AttrCmd[17]."','".$AttrCmd[18]."','$Level',
                        $user_id,'$datetime')";
        $result = mysqli_query($conn,$sql) or mysqli_error();

        if($result) $count++;
    }

    //Send Mail
    if($count == count($RowCmd) - 1)
    {
        /*$mail = new PHPMailer;

        $mail->SMTPDebug = 2;
        $mail->isSMTP();                                   // Set mailer to use SMTP
        $mail->Host = 'smtp.gmail.com';                    // Specify main and backup SMTP servers
        $mail->SMTPAuth = true;                            // Enable SMTP authentication
        $mail->Username = 'coop.logicallab@gmail.com';     // SMTP username
        $mail->Password = '0993239096';                 // SMTP password
        $mail->SMTPSecure = 'tls';                         // Enable TLS encryption, `ssl` also accepted
        $mail->Port = 587;                                 // TCP port to connect to

        $mail->setFrom('coop.logicallab@gmail.com', 'Logical Lab');
        $mail->addReplyTo('coop.logicallab@gmail.com', 'Logical Lab');
        $mail->addAddress($email);   // Add a recipient

        $mail->isHTML(true);  // Set email format to HTML

        $bodyContent = "<p>Hi ".$name." ".$surname."</p><br>";
        $bodyContent .= "<p>The admin has now received your answer. We will check the answers and reply to you as soon as possible. Thank you for applying to us.</p>";
        $subject = 'Confirmation of send the answer of Logical Lab';
        $mail->Subject = '=?utf-8?B?'.base64_encode($subject).'?=';
        $mail->Body    = $bodyContent;

        if(!$mail->send())
        {
            echo $mail->ErrorInfo;
            //echo "Send mail incomplete.";
        }
        else
        {
            echo "Send mail complete.";
        }*/
        //Update send data for user
        if($Level == "1"){
            $sql = "UPDATE users
                 SET Level_1 = 1
                 WHERE Email = '$email'";
        }
        else if($Level == "2"){
            $sql = "UPDATE users
                 SET Level_2 = 1
                 WHERE Email = '$email'";
        }
        else if($Level == "3"){
            $sql = "UPDATE users
                 SET Level_3 = 1
                 WHERE Email = '$email'";
        }

        $result = mysqli_query($conn,$sql) or mysqli_error();

        if($result) echo "Send answer complete.";
        else echo "Send answer incomplete.";
    }
    else{
        echo "Send answer incomplete.";
    }
    mysqli_close($conn);
?>
