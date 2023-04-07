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
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js" integrity="sha384-vFJXuSJphROIrBnz7yo7oB41mKfc8JzQZiCq4NCceLEaO4IHwicKwpJf9c9IpFgh" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js" integrity="sha384-alpBpkh1PFOepccYVYDB4do5UnbKysX5WZXm3XxPqe5iKTfUKjNkCk9SaVuEZflJ" crossorigin="anonymous"></script>
    
    <script type="text/javascript">
        $(document).ready(function(){
            var error = '<?=$_GET['Error']?>';
            if(error == "LoginError") $('#errorModal').modal('show');
        });
    </script>
    
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
      .card{
          margin-top:170px;
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
    <!-- Modal connect error -->
    <div class="modal fade" id="errorModal" tabindex="-1" role="dialog" aria-labelledby="ModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title text-white" id="ModalLabel">Error</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    Someone else has logged in with the account you want to log in to. You can check your email for a new password reset.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>  
      
    <?php
      if(isset($_POST['Submit']))
      {
        require_once("ConnectDB.php");
        $email = mysqli_real_escape_string($conn,$_POST["inputEmail"]);
        $password = mysqli_real_escape_string($conn,$_POST['inputPassword']);

        if($email == "" || $password == "") $Error= "InputEmpty";
        else if(!filter_var($email, FILTER_VALIDATE_EMAIL))
          $Error= "InputIncorrect";
        else
        {
          $EncodePassword = md5($password);
          
          $sql = "SELECT Email FROM admins WHERE Email = '$email' AND Password = '$EncodePassword'";
          $result = mysqli_query($conn,$sql) or mysqli_error();

          if($result)
          {
            if(mysqli_num_rows($result) > 0)
            {
              if($row = mysqli_fetch_assoc($result))
              {
                date_default_timezone_set("Asia/Bangkok");
                $datetime = date('Y-m-d H:i:s');

                $sql = "UPDATE admins
                SET Last_Update = '$datetime'
                WHERE Email = '".$row['Email']."'";
                $result = mysqli_query($conn,$sql) or mysqli_error();

                if($result)
                {
                  $_SESSION['username'] = $row['Email'];
                  
                  $sql = "SELECT Last_Update,Role_ID FROM admins WHERE Email = '".$_SESSION['username']."'";
                  $result = mysqli_query($conn,$sql) or mysqli_error();

                  if($result)
                  {
                    if(mysqli_num_rows($result) > 0)
                    {
                      if($row = mysqli_fetch_assoc($result))
                      {
                          $_SESSION['lastUpdate'] = $row['Last_Update'];
                          
                          if($row['Role_ID'] == 1) header("Location:AdminManage.php");
                          else header("Location:LogicalLab.php");
                          exit();
                      }
                    }
                  }
                }
              }
            }
            else $Error= "InputIncorrect";
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
                    <h5>Sign-in</h5>
                    <div id="recommend">
                        <p>
                            Sign in to manage admin information.
                        </p>
                    </div>
                </li>
              </ul>
              <div class="card-body">
                <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <?php
                        if(isset($Error) && $Error == "InputEmpty")
                        {
                          echo "<div class='col-md-12 alert alert-danger' role='alert'> Please enter your email and password. </div>";
                        }
                        else if(isset($Error) && $Error == "InputIncorrect")
                        {
                          echo "<div class='col-md-12 alert alert-danger' role='alert'> Invalid email or password. </div>";
                        }
                      ?>
                      <label for="inputEmail">Email</label>
                      <input type="text" class="form-control" id="inputEmail" name="inputEmail" value="<?php if(isset($email)){echo $email;} ?>">
                    </div>
                  </div>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <label for="inputPassword">Password</label>
                      <input type="password" class="form-control" id="inputPassword" name="inputPassword" value="<?php if(isset($password)){echo $password;} ?>">
                    </div>
                  </div>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <button type="submit" name="Submit" class="btn btn-success mr-3">Sign in</button>
                      <a id="forgotpass" href = "ForgotPassword.php">Forgot password</a>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
