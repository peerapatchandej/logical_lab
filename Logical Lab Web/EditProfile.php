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
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js" integrity="sha384-vFJXuSJphROIrBnz7yo7oB41mKfc8JzQZiCq4NCceLEaO4IHwicKwpJf9c9IpFgh" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js" integrity="sha384-alpBpkh1PFOepccYVYDB4do5UnbKysX5WZXm3XxPqe5iKTfUKjNkCk9SaVuEZflJ" crossorigin="anonymous"></script>
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
      $NameClass = "form-control";
      $LastnameClass= "form-control";
      $EmailClass = "form-control";

      if(isset($_POST['Submit']))
      {
        if(isset($_POST["inputName"])) $name = mysqli_real_escape_string($conn,$_POST["inputName"]);
        if(isset($_POST["inputLastname"])) $lastname = mysqli_real_escape_string($conn,$_POST["inputLastname"]);
        if(isset($_POST["inputEmail"])) $email = mysqli_real_escape_string($conn,$_POST["inputEmail"]);
    
        if($name == "")
        {
          $NameClass= "form-control is-invalid";
          $NameError= "NameEmpty";
          $NameValid = false;
        }
        else if(!preg_match('/^[A-Za-z]+$/', $name))
        {
          $NameClass= "form-control is-invalid";
          $NameError= "NameIncorrect";
          $NameValid = false;
        }
        else $NameValid = true;

        if($lastname == "")
        {
          $LastnameClass= "form-control is-invalid";
          $LastnameError= "LastnameEmpty";
          $LastnameValid = false;
        }
        else if(!preg_match('/^[A-Za-z]+$/', $lastname))
        {
          $LastnameClass= "form-control is-invalid";
          $LastnameError= "LastnameIncorrect";
          $LastnameValid = false;
        }
        else $LastnameValid = true;

        if($email == "")
        {
          $EmailClass = "form-control is-invalid";
          $EmailError= "EmailEmpty";
          $EmailValid = false;
        }
        else if(!filter_var($email, FILTER_VALIDATE_EMAIL))
        {
          $EmailClass = "form-control is-invalid";
          $EmailError= "EmailIncorrect";
          $EmailValid = false;
        }
        else
        { 
            $sql = "SELECT Email FROM admins WHERE Email = '$email' AND Email != '".$_SESSION['username']."'";
            $result = mysqli_query($conn,$sql) or mysqli_error();

            if($result)
            {
                if(mysqli_num_rows($result) > 0)
                {
                  $EmailClass = "form-control is-invalid";
                  $EmailError= "EmailInvalid";
                  $EmailValid = false;
                }
                else $EmailValid = true;
            }
        }
        
        if($NameValid && $LastnameValid && $EmailValid)
        {
            $sql = "UPDATE admins
                 SET Name = '$name', Lastname = '$lastname', Email = '$email'
                 WHERE Email='".$_SESSION['username']."'";
            $result = mysqli_query($conn,$sql) or mysqli_error();
            
            $_SESSION['username'] = $email;
            
            $readname = "true";
            echo "<script type='text/javascript'> ReadOnlyName(true); </script>";
            $readlastname = "true";
            echo "<script type='text/javascript'> ReadOnlyLastname(true); </script>";
            $reademail = "true";
            echo "<script type='text/javascript'> ReadOnlyEmail(true); </script>";
            
            $isValid = true;
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
    
    <!--Form-->
    <div class="login vertical-center">
      <div class="container-fluid ">
        <div class="row justify-content-center">
          <div class="col-md-5 col-lg-4 col-xl-3">
            <div class="card text-black bg-white">
              <ul class="list-group list-group-flush">
                <li class="list-group-item bg-white">
                    <h5>Edit profile</h5>
                    <div id="recommend">
                        <p>
                            Edit profile for your account.
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
                              echo "<div class='col-md-12 alert alert-success' role='alert'><center>Profile editing successfully.</center></div>";
                            }
                        ?>
                        <label for="inputName">Name</label>
                        <div class="input-group mb-2 mb-sm-0">
                            <input type="text" id="inputName" name = "inputName" class="<?php echo $NameClass;?>"
                            value=
                                "<?php 
                                    if(isset($name)) echo $name;
                                    else echo $nav_name;
                                ?>">
                        </div>    
                        <?php
                            if(isset($NameError) && $NameError == "NameEmpty")
                                echo "<div class='feedback text-danger'> Please enter your name.</div>";
                            else if(isset($NameError) && $NameError == "NameIncorrect")
                                echo "<div class='feedback text-danger'>Please enter your English name.</div>";
                        ?>
                    </div>
                  </div>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                        <label for="inputLastname">Last name</label>
                        <div class="input-group mb-2 mb-sm-0">
                            <input type="text" id="inputLastname" name = "inputLastname" 
                            class="<?php echo $LastnameClass;?>" 
                            value=
                                "<?php 
                                    if(isset($lastname)) echo $lastname;
                                    else echo $nav_lastname;
                                ?>">
                        </div>
                        <?php
                          if(isset($LastnameError) && $LastnameError == "LastnameEmpty")
                            echo "<div class='feedback text-danger'> Please enter your last name.</div>";
                          else if(isset($LastnameError) && $LastnameError == "LastnameIncorrect")
                            echo "<div class='feedback text-danger'>Please enter your English last name.</div>";
                        ?>
                    </div>
                  </div>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                        <label for="inputEmail">Email</label>
                        <div class="input-group mb-2 mb-sm-0">
                            <div class="input-group">
                              <input type="text" id="inputEmail" name = "inputEmail" class="<?php echo $EmailClass;?>"
                              value=
                                  "<?php 
                                    if(isset($email)) echo $email;
                                    else echo $_SESSION['username'];
                                  ?>">
                            </div>
                        </div>    
                        <?php
                          if(isset($EmailError) && $EmailError == "EmailEmpty")
                            echo "<div class='feedback text-danger'> Please enter your email address.</div>";
                          else if(isset($EmailError) && $EmailError == "EmailIncorrect")
                            echo "<div class='feedback text-danger'>Email must be in the format: your@email.com.</div>";
                          else if(isset($EmailError) && $EmailError == "EmailInvalid")
                            echo "<div class='feedback text-danger'>You can not use this email.</div>";
                        ?>
                    </div>
                  </div><br>
                  <center>
                    <div class="form-row">
                      <div class="form-group col-md-12">
                        <button type="submit" name="Submit" class="btn btn-success">Edit Profile</button>
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
  </body>
</html>
