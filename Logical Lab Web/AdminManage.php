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
    
    <script type="text/javascript">
        $(function(){
          $('#filter').on('change', function () {
              var url = $(this).val();
              if (url) window.location = url;
              return false;
          });
        });
    </script>
    
    <style>
      body{
        width : 100%;
        width : 100vw;
        font-family: sans-serif;
        background-color: #448ccb;
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
      .table-responsive{
        max-height:500px;
      }
      .feedback{
        font-size: 13px;
      }
      #brandText{
          font-size: 1.2em;
          color: #448ccb;
      }
      @media screen and (max-width: 999px) /*800x600*/
      {
        .tabledata{
          margin-top:120px;
        }
      }
      @media screen and (min-width: 1000px)
      {
        .col-lg-7{
          margin-top:120px;
          margin-left : 30px;
        }
        .card{
            margin-top:120px;
            margin-left : 60px;
        }
      }
    </style>
  </head>
  <body>
    <?php
      //Get post data
      //input add admin
      if(isset($_POST["inputName"])) $name = mysqli_real_escape_string($conn,$_POST["inputName"]);
      if(isset($_POST["inputLastname"])) $lastname = mysqli_real_escape_string($conn,$_POST["inputLastname"]);
      if(isset($_POST["inputEmail"])) $email = mysqli_real_escape_string($conn,$_POST["inputEmail"]);

      //input change password
      if(isset($_POST["OldPassword"])) $OldPassword = mysqli_real_escape_string($conn,$_POST['OldPassword']);
      if(isset($_POST["NewPassword"])) $NewPassword = mysqli_real_escape_string($conn,$_POST['NewPassword']);
      if(isset($_POST["ConfirmPassword"])) $ConfirmPassword = mysqli_real_escape_string($conn,$_POST['ConfirmPassword']);


      //Default form class
      //form add admin
      $NameClass = "form-control";
      $LastnameClass= "form-control";
      $EmailClass = "form-control";

      //form change password
      $OldClass = "form-control";
      $NewClass = "form-control";
      $ConfirmClass = "form-control";

      //Validate input data
      if(isset($_POST['Submit']))
      {
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
        else $EmailValid = true;

        if($NameValid && $LastnameValid && $EmailValid)
        {
          //Random Password
          $seed = str_split('abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789');
          shuffle($seed);
          $password = '';
          foreach (array_rand($seed, 6) as $k) $password .= $seed[$k];

          //check email in database
          $sql = "SELECT Email FROM admins WHERE Email = '$email'";
          $result = mysqli_query($conn,$sql) or mysqli_error();

          if($result)
          {
            if(mysqli_num_rows($result) > 0)
            {
              $EmailClass = "form-control is-invalid";
              $EmailError= "EmailInvalid";
              $EmailValid = false;
            }
            else
            {
              //insert data
              date_default_timezone_set("Asia/Bangkok");
              $datetime = date('Y-m-d H:i:s');
              
              $EncodePassword = md5($password);
                
              $sql = "INSERT INTO admins (Name,Lastname,Email,Password,Create_Date,Role_ID)
                      VALUES ('$name','$lastname','$email','$EncodePassword','$datetime',2)";
              $result = mysqli_query($conn,$sql) or mysqli_error();

              if($result)
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
                $mail->addAddress($email);   // Add a recipient
                $mail->isHTML(true);  // Set email format to HTML

                $bodyContent = "<p>Hi ".$name." ".$lastname."</p><br>";
                $bodyContent .= "<p>You are now a Logical Lab game admin. Here is your password ".$password." <br>( You can change your password at the Change Password menu. )</p>";
                $subject = 'The administrator password of Logical Lab';
                $mail->Subject = '=?utf-8?B?'.base64_encode($subject).'?=';
                $mail->Body    = $bodyContent;
                $mail->send();

                $name = "";
                $lastname = "";
                $email = "";
              }
            }
          }
        }
      }
      else if(isset($_POST['Delete']))
      {
        $deleteId = mysqli_real_escape_string($conn,$_POST['delete_id']);
        
        $sql = "DELETE FROM admins WHERE Admin_ID = $deleteId";
        $result = mysqli_query($conn,$sql) or mysqli_error();
      }
      else if(isset($_POST['Search']))
      {
          $searchVal = mysqli_real_escape_string($conn,$_POST['search_val']);
      }
      
      //Get name and last name
      $sql = "SELECT Name,Lastname FROM admins WHERE Email = '".$_SESSION['username']."'";
      $result = mysqli_query($conn,$sql) or mysqli_error();

      if($result)
      {
          if(mysqli_num_rows($result) > 0)
          {
              if($row = mysqli_fetch_assoc($result))
              {
                  $nav_name = $row['Name'];
                  $nav_lastname = $row['Lastname'];
              }
          }
      }
    ?>

    <!-- Navbar -->
    <nav class="navbar navbar-expand-lg navbar-light bg-light fixed-top">
      <img src="img/Icon.png" width="32px" height="38px" class="d-inline-block navbar-brand"></img>
      <span id="brandText" class="mr-3">Logical Lab</span>
      <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbar" aria-controls="navbarTogglerDemo02" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbar">
        <ul class="navbar-nav mr-auto"> <!--mr-auto ml-4-->
          <li class="nav-item active">
            <a class="nav-link" href="AdminManage.php">Examiner Management <span class="sr-only">(current)</span></a>
          </li>
          <li class="nav-item"> <!--ml-2-->
            <a class="nav-link" href="LogicalLab.php">Play Logical Lab</a>
          </li>
        </ul>
        <span class="navbar-text"><?php echo $nav_name." ".$nav_lastname;?></span>
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

    <div class="container-fluid ">
      <div class="row">
          <div class="col-md-12 col-lg-7 tabledata">
            <!-- Filter -->
            <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                <div class="form-row align-items-center">
                  <div class="col-auto">
                    <select class="custom-select mb-2 mr-sm-2 mb-sm-0" id="filter">
                        <option value="" selected>Choose filter</option>
                        <option value="https://logicallabcoop.000webhostapp.com/AdminManage.php?Filter=AToZ">A - Z</option>
                        <option value="https://logicallabcoop.000webhostapp.com/AdminManage.php?Filter=ZToA">Z - A</option>
                        <option value="https://logicallabcoop.000webhostapp.com/AdminManage.php?Filter=NewToOld">Newest - Oldest</option>
                        <option value="https://logicallabcoop.000webhostapp.com/AdminManage.php?Filter=OldToNew">Oldest - Newest</option>
                    </select>
                  </div>
                  <div class="col-auto">
                    <div class="input-group mb-2 mb-sm-0">
                      <input type="text" class="form-control" name="search_val" placeholder="Search...">
                      <span class="input-group-btn">
                        <button class="btn btn-secondary" type="submit" name="Search">Search</button>
                      </span>
                    </div>
                  </div>
                </div>
            </form><br>
            
            <!-- Table Data -->  
            <div class="panel panel-default">
              <div class="panel-body table-responsive">
                <table class="table table-light table-hover table-bordered">
                  <thead class="thead-light">
                    <tr>
                      <th scope="col" class="text-dark">#</th>
                      <th scope="col" class="text-dark">Name</th>
                      <th scope="col" class="text-dark">Last name</th>
                      <th scope="col" class="text-dark">Email</th>
                      <th scope="col" class="text-dark">Delete Admin</th>
                    </tr>
                  </thead>
                  <tbody>
                    <?php
                      if(isset($_GET['Filter']))
                      {
                        if($_GET['Filter'] == "AToZ")
                        {
                            $sql = "SELECT Admin_ID,Name,Lastname,Email,Role_ID FROM admins ORDER BY Name ASC";
                        }
                        else if($_GET['Filter'] == "ZToA")
                        {
                            $sql = "SELECT Admin_ID,Name,Lastname,Email,Role_ID FROM admins ORDER BY Name DESC";
                        }
                        else if($_GET['Filter'] == "NewToOld")
                        {
                            $sql = "SELECT Admin_ID,Name,Lastname,Email,Role_ID FROM admins ORDER BY Admin_ID DESC";
                        }
                        else if($_GET['Filter'] == "OldToNew")
                        {
                            $sql = "SELECT Admin_ID,Name,Lastname,Email,Role_ID FROM admins ORDER BY Admin_ID ASC";
                        }
                      }
                      else if(isset($_POST['search_val']))
                      {
                          $sql = "SELECT Admin_ID,Name,Lastname,Email,Role_ID FROM admins 
                          WHERE Admin_ID LIKE '%$searchVal%' OR Name LIKE '%$searchVal%' OR 
                          Lastname LIKE '%$searchVal%' OR Email LIKE '%$searchVal%'";
                      }
                      else
                      {
                        $sql = "SELECT Admin_ID,Name,Lastname,Email,Role_ID FROM admins";
                      }
                      
                      $result = mysqli_query($conn,$sql) or mysqli_error();
                      
                      if($result)
                      {
                        if(mysqli_num_rows($result) > 0)
                        {
                          while($row = mysqli_fetch_assoc($result))
                          {
                    ?>
                    <tr>
                      <th scope="row"><?php echo $row['Admin_ID'];?></th>
                      <td><?php echo $row['Name'];?></td>
                      <td><?php echo $row['Lastname'];?></td>
                      <td><?php echo $row['Email'];?></td>
                      <td>
                          <?php
                            if($row['Role_ID'] != 1){
                              echo "<a href='#delete".$row['Admin_ID']."' data-toggle='modal'>
                                <center><button type='button' class='btn btn-danger'> Delete </button></center>
                              </a>";
                            }
                            else{
                              echo "<center><button type='button' class='btn btn-danger' disabled> Delete </button></center>";
                            }
                          ?>
                      </td>
                      
                      <!-- Modal confirm delete -->
                      <div class="modal fade" id="delete<?php echo $row['Admin_ID'];?>" tabindex="-1" role="dialog" aria-labelledby="ModalLabel" aria-hidden="true">
                        <div class="modal-dialog" role="document">
                          <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                            <div class="modal-content">
                              <div class="modal-header bg-danger">
                                <h5 class="modal-title text-white" id="ModalLabel">Delete Admin User</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                  <span aria-hidden="true">&times;</span>
                                </button>
                              </div>
                              <div class="modal-body">
                                <input type="hidden" name="delete_id" value="<?php echo $row['Admin_ID']; ?>">
                                <?php echo $row['Admin_ID']; ?>
                                Are you sure to delete <strong><?php echo $row['Name']." ".$row['Lastname']?></strong><?php echo " ?" ?>
                              </div>
                              <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                                <button type="submit" name="Delete" class="btn btn-primary">Delete</button>
                              </div>
                            </div>
                          </form>
                        </div>
                      </div>
                    </tr>
                    <?php
                          }
                        }
                      }
                    ?>
                    <!--<tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>
                    <tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>
                    <tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>
                    <tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>
                    <tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>
                    <tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>
                    <tr><td>test</td><td>test</td><td>test</td><td>test</td></tr>-->
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          
          <!-- Form add admin -->
          <div class="col-md-12 col-lg-4">
              <div class="card text-black bg-white">
                  <div class="card-header "><h3>Add Examiner</h3></div>
                  <div class="card-body">
                      <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                        <div class="form-row">
                          <div class="form-group col-md-6">
                            <label for="inputName">Name</label>
                            <input type="text" id="inputName" name = "inputName" class="<?php echo $NameClass;?>"
                            value="<?php if(isset($name)) echo $name;?>">
                            <?php
                              if(isset($NameError) && $NameError == "NameEmpty")
                                echo "<div class='feedback text-danger'> Please enter your name.</div>";
                              else if(isset($NameError) && $NameError == "NameIncorrect")
                                echo "<div class='feedback text-danger'>Please enter your English name.</div>";
                            ?>
                          </div>
                          <div class="form-group col-md-6">
                            <label for="inputLastname">Last name</label>
                            <input type="text" id="inputLastname" name = "inputLastname" 
                            class="<?php echo $LastnameClass;?>" value="<?php if(isset($lastname)) echo $lastname;?>">
                            <?php
                              if(isset($LastnameError) && $LastnameError == "LastnameEmpty")
                                echo "<div class='feedback text-danger'> Please enter your last name.</div>";
                              else if(isset($LastnameError) && $LastnameError == "LastnameIncorrect")
                                echo "<div class='feedback text-danger'>Please enter your English last name.</div>";
                            ?>
                          </div>
                        </div>
                        <div class="form-row" name="emailFrom">
                          <div class="form-group col-md-12">
                            <label for="inputEmail">Email</label>
                            <div class="input-group">
                              <input type="text" id="inputEmail" name = "inputEmail" class="<?php echo $EmailClass;?>"
                              value="<?php if(isset($email)) echo $email;?>">
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
                        </div>
                        <br>
                        <button type="submit" class="btn btn-success" name = "Submit">Add Data</button>
                        <button type="reset" class="btn btn-danger" name = "Clear">Clear Data</button>
                      </form>
                  </div>
              </div>
          </div>
      </div>
    </div>
    <?php 
        mysqli_close($conn);
    ?>
  </body>
</html>
