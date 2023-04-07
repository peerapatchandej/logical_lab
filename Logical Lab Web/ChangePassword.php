<?php
  session_start();
  ob_start();
  require_once('PHPMailer/PHPMailerAutoload.php');
  require_once("CheckLastUpdate.php");
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
      @media screen and (max-width: 999px) /*800x600*/
      {
        .card{
          margin-top:100px;
          margin-bottom:20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (max-height : 469px) /*1280x600*/
      {
        .card{
          margin-top:100px;
          margin-bottom : 20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 470px) and (max-height:589px) /*1280x720*/
      {
        .card{
          margin-top:100px;
          margin-bottom : 20px;
        }
      }
    </style>
  </head>
  <body>
    <?php
      //form change password
      $OldClass = "form-control";
      $NewClass = "form-control";
      $ConfirmClass = "form-control";

      if(isset($_POST['Submit']))
      {
        $OldPassword = mysqli_real_escape_string($conn,$_POST["OldPassword"]);
        $NewPassword = mysqli_real_escape_string($conn,$_POST['NewPassword']);
        $ConfirmPassword = mysqli_real_escape_string($conn,$_POST['ConfirmPassword']);

        if($OldPassword == "")
        {
          $OldClass = "form-control is-invalid";
          $OldError = "OldEmpty";
        }
        else if($OldPassword != "")
        {
          $sql = "SELECT Password FROM admins WHERE Email = '".$_SESSION['username']."'";
          $result = mysqli_query($conn,$sql) or mysqli_error();

          if($result)
          {
            if(mysqli_num_rows($result) > 0)
            {
              if($row = mysqli_fetch_assoc($result))
              {
                $EncodePassword = md5($OldPassword);
                
                if($row['Password'] != $EncodePassword) $OldError = "OldInvalid";
                else if($NewPassword == "")
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
                             WHERE Email='".$_SESSION['username']."'";
                      $result = mysqli_query($conn,$sql) or mysqli_error();

                      $OldPassword = "";
                      $NewPassword = "";
                      $ConfirmPassword = "";
                      $isValid = true;
                    }
                  }
                }
              }
            }
          }
        }
      }
      
      //Get name and last name
      $sql = "SELECT Name,Lastname,Role_ID FROM admins WHERE Email = '".$_SESSION['username']."'";
      $result = mysqli_query($conn,$sql) or mysqli_error();

      if($result)
      {
          if(mysqli_num_rows($result) > 0)
          {
              if($row = mysqli_fetch_assoc($result))
              {
                  $nav_name = $row['Name'];
                  $nav_lastname = $row['Lastname'];
                  
                  if($row['Role_ID']==1) $isAdmin = true;
              }
          }
      }
    ?>
    
    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg navbar-light bg-light fixed-top">
      <img src="img/Icon.png" width="32px" height="38px" class="d-inline-block navbar-brand"></img>
      <span id="brandText" class="mr-3">Logical Lab</span>
      <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbar" aria-controls="navbar" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbar">
        <ul class="navbar-nav mr-auto">
        <?php if(isset($isAdmin) && $isAdmin){ ?>  
          <li class="nav-item">
            <a class="nav-link" href="AdminManage.php">Examiner Management</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" href="LogicalLab.php">Play Logical Lab</a>
          </li>
        <?php 
            } 
            else{
        ?>
          <li class="nav-item">
            <a class="nav-link" href="LogicalLab.php">Play Logical Lab</a>
          </li>
        <?php }?>
        </ul>
        <span class="navbar-text ml-auto"><?php echo $nav_name." ".$nav_lastname;?></span>
        <ul class="navbar-nav ">
          <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></a>
            <div class="dropdown-menu dropdown-menu-right" aria-labelledby="navbarDropdownMenuLink">
              <a class="dropdown-item" href="EditProfile.php" >Edit Profile</a>
              <a class="dropdown-item" href="ChangePassword.php" >Change Password</a>
              <a class="dropdown-item" href="SignOut.php">Sign out</a>
            </div>
          </li>
        </ul>
      </div>
    </nav>
    
    <div class="login vertical-center">
      <div class="container-fluid ">
        <div class="row justify-content-center">
          <div class="col-md-5 col-lg-4 col-xl-3">
            <div class="card text-black bg-white">
              <ul class="list-group list-group-flush">
                <li class="list-group-item bg-white">
                    <h5>Set a new password</h5>
                    <div id="recommend">
                        <p>
                            Set a new password for your account (Passwords must be at least 6 characters long).
                        </p>
                    </div>
                </li>
              </ul>
              <div class="card-body">
                <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <?php
                        if(isset($isValid) && $isValid)
                        {
                          echo "<div class='col-md-12 alert alert-success' role='alert'><center>Password changed successfully.</center></div>";
                          /*echo "<div class='col-md-12 alert alert-success' role='alert'>Password changed successfully. Page will redirect.</div>";
                          
                          if(isset($isAdmin) && $isAdmin) header("refresh:2;url=AdminManage.php");
                          else header("refresh:2;url=LogicalLabAdmin.php");*/
                        }
                      ?>
                      <label for="OldPassword" >Old Password</label>
                      <input type="password" class="<?php echo $OldClass;?>" id="OldPassword" name="OldPassword"
                      value="<?php if(isset($OldPassword)){echo $OldPassword;} ?>">
                      <?php
                        if(isset($OldError) && $OldError == "OldEmpty")
                          echo "<div class='feedback text-danger'> Please enter your old password.</div>";
                        else if(isset($OldError) && $OldError == "OldInvalid")
                          echo "<div class='feedback text-danger'> Password wrong, please try again.</div>";
                      ?>
                    </div>
                  </div>
                  <div class="form-row">
                    <div class="form-group col-md-12">
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
                        <button type="submit" name="Submit" class="btn btn-success">Change Password</button>
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

    <?php mysqli_close($conn);?>
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js" integrity="sha384-vFJXuSJphROIrBnz7yo7oB41mKfc8JzQZiCq4NCceLEaO4IHwicKwpJf9c9IpFgh" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js" integrity="sha384-alpBpkh1PFOepccYVYDB4do5UnbKysX5WZXm3XxPqe5iKTfUKjNkCk9SaVuEZflJ" crossorigin="anonymous"></script>
  </body>
</html>
