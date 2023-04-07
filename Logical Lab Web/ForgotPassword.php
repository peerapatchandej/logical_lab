<?php
  session_start();
  ob_start();
  require_once('PHPMailer/PHPMailerAutoload.php');
  
  if(isset($_SESSION['username']) && isset($_SESSION['lastUpdate']))
  {
    require_once("ConnectDB.php");
    $sql = "SELECT Role_ID FROM admins WHERE Email = '".$_SESSION['username']."' 
    AND Last_Update = '".$_SESSION['lastUpdate']."'";
    $result = mysqli_query($conn,$sql) or mysqli_error();

    if($result)
    {
        if(mysqli_num_rows($result) > 0)
        {
            if($row = mysqli_fetch_assoc($result)){
                if($row['Role_ID'] == 1) header("Location:AdminManage.php");
                else header("Location:LogicalLab.php");
                mysqli_close($conn);
                exit();
            }
        }
        else
        {
            session_unset();
            session_destroy();
        }
    }
  }
?>
<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css" integrity="sha384-PsH8R72JQ3SOdhVi3uxftmaW6Vc51MKb0q5P2rRUpPvrszuE4W1povHYgTpBfshb" crossorigin="anonymous">
    <style>
      body{
        font-family: sans-serif;
        background-color: #448ccb;
      }
      .vertical-center {
        min-height: 100%;
        min-height: 100vh;
        display: flex;
        align-items: center;
      }
      a{
        color: black;
        background-color: transparent;
        text-decoration: none;
      }
      a:hover {
        background-color: transparent;
        text-decoration: none;
      }
      #recommend , .alert , .feedback{
        font-size: 13px;
      }
      #brandText{
          font-size: 1.2em;
          color: #448ccb;
      }
      .card{
          margin-top:170px;
      }
      img{
          width:768px;
          height:152px;
      }
      @media screen and (max-width: 999px) /*800x600*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1000px) and (max-width: 1022px) and (max-height: 637px) /*1024x768*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (max-height : 469px) /*1280x600*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 470px) and (max-height:589px) /*1280x720*/
      {
        .card{
          margin-top:250px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 590px) and (max-height:637px) /*1280x768*/
      {
        .card{
          margin-top:220px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 638px) and (max-height:669px) /*1280x800*/
      {
        .card{
          margin-top:220px;
        }
      }
      @media screen and (min-width: 1345px) and (max-width : 1358px) and (max-height:637px) /*1360x768*/
      {
        .card{
          margin-top:220px;
        }
      }
      @media screen and (min-width: 1359px) and (max-width : 1364px) and (max-height:637px) /*1366x768*/
      {
        .card{
          margin-top:220px;
        }
      }
    </style>
  </head>
  <body>
    <?php
      //form change password
      $EmailClass = "form-control";

      if(isset($_POST['Submit']))
      {
        require_once("ConnectDB.php");  
        $Email = mysqli_real_escape_string($conn,$_POST["Email"]);

        if($Email == "")
        {
          $EmailClass = "form-control is-invalid";
          $EmailError = "EmailEmpty";
        }
        else
        {
          if(!filter_var($Email, FILTER_VALIDATE_EMAIL))
          {
            $EmailClass = "form-control is-invalid";
            $EmailError= "EmailIncorrect";
          }
          else
          {
            $sql = "SELECT Email FROM admins WHERE Email = '$Email'";
            $result = mysqli_query($conn,$sql) or mysqli_error();

            if($result)
            {
              if(mysqli_num_rows($result) > 0)
              {
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
                $mail->addAddress($Email);   // Add a recipient
                $mail->isHTML(true);  // Set email format to HTML

                $url = base64url_encode($Email);
                
                $bodyContent = "<p>Hi, do you want to reset your password?</p><br>";
                $bodyContent .= "<p>Someone (hopefully it is you) asked us to reset your account password. Please click the link below to proceed. If you did not request this password reset. You can skip this email!</p><br>";
                $bodyContent .= "<a href='https://logicallabcoop.000webhostapp.com/ResetPassword.php?Email=$url'>https://logicallabcoop.000webhostapp.com/ResetPassword.php?Email=$url</a>";
                $subject = 'Forgot Password (Logical Lab)';
                $mail->Subject = '=?utf-8?B?'.base64_encode($subject).'?=';
                $mail->Body    = $bodyContent;
                $mail->send();

                $Email = "";
                $isSend = true;
              }
              else
              {
                $EmailClass = "form-control is-invalid";
                $EmailError= "EmailInvalid";
              }
            }
          }
        }
        mysqli_close($conn);
      }
    ?>
    <div style="position: absolute; left: 50%; margin-top:70px;">
        <div style="position: relative; left: -50%;">
          <center><img src="img/LogoAdmin.png"></img></center>
        </div>
    </div>
    <div class="login vertical-center">
      <div class="container-fluid ">
        <div class="row justify-content-center">
          <div class="col-md-5 col-lg-4 col-xl-3">    <!-- col-md-3 -->
            <div class="card text-black bg-white">
              <ul class="list-group list-group-flush">
                <li class="list-group-item bg-white">
                    <h5>Forgot Password</h5>
                    <div id="recommend">
                        <p>
                            Enter the email address associated with your account to receive an email for resetting 
                            your password.
                        </p>
                    </div>
                </li>
              </ul>
              <div class="card-body">
                <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <?php
                        if(isset($isSend) && $isSend)
                        {
                          echo "<div class='col-md-12 alert alert-success' role='alert'> Email sent successfully. Please see your email.</div>";
                        }
                      ?>
                      <label for="Email" >Email</label>
                      <input type="text" class="<?php echo $EmailClass;?>" id="Email" name="Email"
                      value="<?php if(isset($Email)){echo $Email;} ?>">
                      <?php
                        if(isset($EmailError) && $EmailError == "EmailEmpty")
                          echo "<div class='feedback text-danger'>Please enter your email address.</div>";
                        else if(isset($EmailError) && $EmailError == "EmailIncorrect")
                          echo "<div class='feedback text-danger'>Email must be in the format: your@email.com.</div>";
                        else if(isset($EmailError) && $EmailError == "EmailInvalid")
                          echo "<div class='feedback text-danger'>This email was not found in the database.</div>";
                      ?>
                    </div>
                  </div>
                  <br>
                  <center>
                    <div class="form-row">
                      <div class="form-group col-md-12">
                        <button type="submit" name="Submit" class="btn btn-success">Send</button>
                      </div>
                    </div>
                  </center>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <?php 
        function base64url_encode($data) {
            return rtrim(strtr(base64_encode($data), '+/', '-_'), '=');
        }
    ?>
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js" integrity="sha384-vFJXuSJphROIrBnz7yo7oB41mKfc8JzQZiCq4NCceLEaO4IHwicKwpJf9c9IpFgh" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js" integrity="sha384-alpBpkh1PFOepccYVYDB4do5UnbKysX5WZXm3XxPqe5iKTfUKjNkCk9SaVuEZflJ" crossorigin="anonymous"></script>
  </body>
</html>
