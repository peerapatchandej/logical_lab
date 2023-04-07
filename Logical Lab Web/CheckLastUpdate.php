<?php
    if(isset($_SESSION['username']) && isset($_SESSION['lastUpdate']))
    {
        require_once("ConnectDB.php");
        $sql = "SELECT Last_Update FROM admins WHERE Email = '".$_SESSION['username']."' 
        AND Last_Update = '".$_SESSION['lastUpdate']."'";
        $result = mysqli_query($conn,$sql) or mysqli_error();
        
        if($result)
        {
            if(mysqli_num_rows($result) <= 0)
            {
                $sql = "UPDATE admins
                        SET Last_Update = '0000-00-00 00:00:00'
                        WHERE Email='".$_SESSION['username']."'";
                $result = mysqli_query($conn,$sql) or mysqli_error();
                
                $mail = new PHPMailer;

                $mail->isSMTP();                                   // Set mailer to use SMTP
                $mail->Host = 'smtp.gmail.com';                    // Specify main and backup SMTP servers
                $mail->SMTPAuth = true;                            // Enable SMTP authentication
                $mail->Username = 'coop.logicallab@gmail.com';     // SMTP username
                $mail->Password = '0993239096';                 // SMTP password
                $mail->SMTPSecure = 'tls';                         // Enable TLS encryption, `ssl` also accepted
                $mail->Port = 587;                                 // TCP port to connect to

                $mail->setFrom('coop.logicallab@gmail.com', 'Logical Lab');
                $mail->addReplyTo('coop.logicallab@gmail.com', 'Logical Lab');
                $mail->addAddress($_SESSION['username']);   // Add a recipient
                $mail->isHTML(true);  // Set email format to HTML

                $url = base64url_encode($_SESSION['username']);
                
                $bodyContent = "<p>Hi, Someone else has signed in with the account you want to sign in to. Please click the link below to reset password.</p><br>";
                $bodyContent .= "<a href='https://logicallabcoop.000webhostapp.com/ResetPassword.php?Email=$url'>https://logicallabcoop.000webhostapp.com/ResetPassword.php?Email=$url</a>";
                $subject = 'Someone else signed in with your account';
                $mail->Subject = '=?utf-8?B?'.base64_encode($subject).'?=';
                $mail->Body    = $bodyContent;
                $mail->send();
                
                session_unset();
                session_destroy();
                header("Location:AdminLogin.php?Error=LoginError");
                mysqli_close($conn);
                exit();
            }
        }
    }
    else
    {
        session_unset();
        session_destroy();
        header("Location:AdminLogin.php");
        exit();
    }
    
    function base64url_encode($data) {
            return rtrim(strtr(base64_encode($data), '+/', '-_'), '=');
    }
?>
