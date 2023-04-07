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
            if(error) $('#errorModal').modal('show');
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
        color: white;
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
          height:116px;
      }
      @media screen and (max-width: 999px) /*800x600*/
      {
        .card{
          margin-top:220px;
          margin-bottom: 20px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (max-height : 469px) /*1280x600*/
      {
        .card{
          margin-top:220px;
          margin-bottom: 20px;
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
                    Connect database fail.
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
        $userType = $_POST["userType"];

        if($userType == "user")
        {
            header("location:LogicalLab.php");
        }
        else
        {
          header("location:AdminLogin.php");
          exit();
        }
      }
    ?>
    <div style="position: absolute; left: 50%; margin-top:70px;">
        <div style="position: relative; left: -50%;">
          <center><img src="img/Logo.png"></img></center>
        </div>
    </div>
    <div class="login vertical-center">
      <div class="container-fluid ">
        <div class="row justify-content-center">
          <div class="col-md-5 col-lg-4 col-xl-3">    <!-- col-md-3 -->
            <div class="card text-black bg-white">
              <ul class="list-group list-group-flush">
                <li class="list-group-item bg-white">
                    <h5>Select User</h5>
                    <div id="recommend">
                        <p>
                            Select a user type to select the appropriate webpage for you.
                        </p>
                    </div>
                </li>
              </ul>
              <div class="card-body">
                <form method="post" action=<?php echo htmlspecialchars($_SERVER["PHP_SELF"]);?>>
                  <div class="form-row">
                    <div class="form-group col-md-12">
                      <label for="Type" >User Type</label>
                      <select class="form-control" id="Type" name="userType">
                        <option value="user">User</option>
                        <option value="admin">Admin</option>
                      </select>
                    </div>
                  </div>
                  <br>
                  <center>
                    <div class="form-row">
                      <div class="form-group col-md-12">
                        <button type="submit" name="Submit" class="btn btn-success">Select</button>
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
  </body>
</html>
