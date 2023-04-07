<?php
  session_start();
  ob_start();

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
      img{
          width:768px;
          height:152px;
      }
      .card{
          margin-top:170px;
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
      @media screen and (min-width: 1023px) and (max-width: 1024px) and (max-height: 869px) /*1280x1024*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1025px) and (max-width: 1120px) /*1400x1050*/
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
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 590px) and (max-height:637px) /*1280x768*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 638px) and (max-height:669px) /*1280x800*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1281px) and (max-width : 1344px) /*1680x1050*/
      {
        .card{
          margin-top:220px;
        }
      }
      @media screen and (min-width: 1345px) and (max-width : 1358px) and (max-height:637px) /*1360x768*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1359px) and (max-width : 1364px) and (max-height:637px) /*1366x768*/
      {
        .card{
          margin-top:260px;
          margin-bottom:20px;
        }
      }
    </style>
  </head>
  <body>
    <?php
      if(isset($_GET['Email']))
      {
        $Email = base64url_decode($_GET['Email']);
      }
      
      //form change password
      $NewClass = "form-control";
      $ConfirmClass = "form-control";

      if(isset($_POST['Submit']))
      {
        require_once("ConnectDB.php");
        $NewPassword = mysqli_real_escape_string($conn,$_POST['NewPassword']);
        $ConfirmPassword = mysqli_real_escape_string($conn,$_POST['ConfirmPassword']);

        if($NewPassword == "")
        {
          $NewClass = "form-control is-invalid";
          $NewError = "NewEmpty";
        }
        else if($NewPassword != "")
        {
          if(strlen($NewPassword) < 6)
          {
            $NewClass = "form-control is-invalid";
            $NewError = "NewInvalid";
          }
          else if($ConfirmPassword == "")
          {
            $ConfirmClass = "form-control is-invalid";
            $ConfirmError = "ConfirmEmpty";
          }
          else if($ConfirmPassword != "")
          {
            if($NewPassword != $ConfirmPassword)
            {
              $ConfirmClass = "form-control is-invalid";
              $ConfirmError = "ConfirmInvalid";
            }
            else
            {
              $EncodePassword = md5($NewPassword);
            
              $sql = "UPDATE admins
                     SET Password = '$EncodePassword'
                     WHERE Email='$Email'";
              $result = mysqli_query($conn,$sql) or mysqli_error();

              $NewPassword = "";
              $ConfirmPassword = "";
              $isValid = true;
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
                    <h5>Set a new password</h5>
                    <div id="recommend">
                        <p>
                            Set a new password for your account (Passwords must be at least 6 characters long).<br>
                        </p>
                    </div>
                </li>
              </ul>
              <div class="card-body">
                <?php
                    if(isset($_GET['Email'])){
                        $url = base64url_encode($Email);
                    }
                ?>
                <form method="post" action=<?php if(isset($_GET['Email'])) echo htmlspecialchars($_SERVER["PHP_SELF"])."?Email=$url";?>>
                  <input type="hidden" name="Email" value="<?php if(isset($_GET['Email'])) echo $Email;?>">
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <?php
                        if(isset($isValid) && $isValid)
                        {
                          echo "<div class='col-md-12 alert alert-success' role='alert'>Password reset successfully. Page will redirect.</div>";
                          header("refresh:1;url=AdminLogin.php");
                        }
                      ?>
                      <label for="NewPassword" >New Password</label>
                      <input type="password" class="<?php echo $NewClass;?>" id="NewPassword" name="NewPassword"
                      value="<?php if(isset($NewPassword)){echo $NewPassword;} ?>">
                      <?php
                        if(isset($NewError) && $NewError == "NewEmpty")
                          echo "<div class='feedback text-danger'> Please enter your new password.</div>";
                        else if(isset($NewError) && $NewError == "NewInvalid")
                          echo "<div class='feedback text-danger'> Passwords must be at least 6 characters long.</div>";
                      ?>
                    </div>
                  </div>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <label for="ConfirmPassword" >Confirm Password</label>
                      <input type="password" class="<?php echo $ConfirmClass;?>" id="ConfirmPassword" name="ConfirmPassword"
                      value="<?php if(isset($ConfirmPassword)){echo $ConfirmPassword;} ?>">
                      <?php
                        if(isset($ConfirmError) && $ConfirmError == "ConfirmEmpty")
                          echo "<div class='feedback text-danger'> Please enter your password to confirm.</div>";
                        else if(isset($ConfirmError) && $ConfirmError == "ConfirmInvalid")
                          echo "<div class='feedback text-danger'> Password don't match.</div>";
                      ?>
                    </div>
                  </div><br>
                  <center>
                    <div class="form-row">
                      <div class="form-group col-md-12">
                        <button type="submit" name="Submit" class="btn btn-success">Reset Password</button>
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
        function base64url_decode($data)
        {
            return base64_decode(str_pad(strtr($data, '-_', '+/'), strlen($data) % 4, '=', STR_PAD_RIGHT));
        }
    ?>
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js" integrity="sha384-vFJXuSJphROIrBnz7yo7oB41mKfc8JzQZiCq4NCceLEaO4IHwicKwpJf9c9IpFgh" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js" integrity="sha384-alpBpkh1PFOepccYVYDB4do5UnbKysX5WZXm3XxPqe5iKTfUKjNkCk9SaVuEZflJ" crossorigin="anonymous"></script>
  </body>
</html>
