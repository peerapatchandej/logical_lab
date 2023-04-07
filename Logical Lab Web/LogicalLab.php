<?php
    session_start();
    ob_start();
    require_once("ConnectDB.php");
    require_once('PHPMailer/PHPMailerAutoload.php');
    
    if(isset($_SESSION['username']) && isset($_SESSION['lastUpdate']))
    {
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
            else
            {
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
                          
                          if($row['Role_ID'] == 1) $isAdmin = true;
                        }
                    }
                }
            }
        }
    }
?>
<!DOCTYPE html>
<html lang="en-us">
  <head>
    <meta charset="utf-8">
    <!--<meta http-equiv="Content-Type" content="text/html; charset=utf-8">-->
    <meta name="viewport" http-equiv="Content-Type" content="width=device-width, initial-scale=1, shrink-to-fit=no, text/html">
    <title>Unity WebGL Player | Logical Lab</title>
    <link rel="shortcut icon" href="TemplateData/favicon.ico">
    <link rel="stylesheet" href="TemplateData/style.css">
    <script src="TemplateData/UnityProgress.js"></script>  
    <script src="Build/UnityLoader.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css" integrity="sha384-PsH8R72JQ3SOdhVi3uxftmaW6Vc51MKb0q5P2rRUpPvrszuE4W1povHYgTpBfshb" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.2.1.slim.min.js" integrity="sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.12.3/umd/popper.min.js" integrity="sha384-vFJXuSJphROIrBnz7yo7oB41mKfc8JzQZiCq4NCceLEaO4IHwicKwpJf9c9IpFgh" crossorigin="anonymous"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/js/bootstrap.min.js" integrity="sha384-alpBpkh1PFOepccYVYDB4do5UnbKysX5WZXm3XxPqe5iKTfUKjNkCk9SaVuEZflJ" crossorigin="anonymous"></script>
    
    <script>
      var gameInstance = UnityLoader.instantiate("gameContainer", "Build/New folder (2).json", {onProgress: UnityProgress});
    </script>
    <script type="text/javascript">
        function SignInOverlap(){
            window.location.href = "https://logicallabcoop.000webhostapp.com/AdminLogin.php?Error=LoginError";
        }
    </script>
    <style>
      body{
        font-family: sans-serif;
        background-color: #414141;
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
      #brandText{
          font-size: 1.2em;
          color: #448ccb;
      }
      #credit{
          color: white;
          font-size: 0.7em;
      }
      #credit a{
          color : gray;
      }
      
      @media screen and (max-width: 999px) /*800x600*/
      {
        .webgl-content{
            margin-top:180px;
            margin-left : 250px;
        }
      }
      @media screen and (min-width: 1000px) and (max-width: 1022px) and (max-height: 637px) /*1024x768*/
      {
        .webgl-content{
            margin-top:95px;
            margin-left : 140px;
        }
      }
      
      @media screen and (min-width: 1023px) and (max-width: 1024px) and (max-height: 869px) /*1280x1024*/
      {
        .webgl-content{
            margin-top:80px;
            margin-left : 140px;
        }
      }
      @media screen and (min-width: 1025px) and (max-width: 1120px) /*1400x1050*/
      {
        .webgl-content{
            margin-top:70px;
            margin-left : 90px;
        }
      }
      @media screen and (min-width: 1121px) and (max-width: 1152px) and (max-height: 733px) /*1152x864*/
      {
        .webgl-content{
            margin-top:55px;
            margin-left : 75px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (max-height : 469px) /*1280x600*/
      {
        .webgl-content{
            margin-top:180px;
            margin-left : 10px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 470px) and (max-height:589px) /*1280x720*/
      {
        .webgl-content{
            margin-top:120px;
            margin-left : 10px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 590px) and (max-height:637px) /*1280x768*/
      {
        .webgl-content{
            margin-top:100px;
            margin-left : 10px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 638px) and (max-height:669px) /*1280x800*/
      {
        .webgl-content{
            margin-top:90px;
            margin-left : 10px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 670px) and (max-height:829px) /*1280x960*/
      {
        .webgl-content{
            margin-top:0px;
             margin-left : 0px;
        }
      }
      @media screen and (min-width: 1201px) and (max-width: 1280px) and (min-height : 830px) and (max-height:893px)
      {
        .webgl-content{
            margin-top:0px;
        }
      }
      @media screen and (min-width: 1281px) and (max-width : 1344px) /*1680x1050*/
      {
        .webgl-content{
            margin-top:50px;
        }
      }
      @media screen and (min-width: 1345px) and (max-width : 1358px) and (max-height:637px) /*1360x768*/
      {
        .webgl-content{
            margin-top:85px;
        }
      }
      @media screen and (min-width: 1359px) and (max-width : 1364px) and (max-height:637px) /*1366x768*/
      {
        .webgl-content{
            margin-top:85px;
        }
      }
      @media screen and (min-width: 1365px) and (max-width : 1438px) and (max-height:769px)
      {
        .webgl-content{
            margin-top:30px;
        }
      }
      @media screen and (min-width: 1439px) and (max-width : 1598px) and (max-height:769px)
      {
        .webgl-content{
            margin-top:30px;
        }
      }
    </style>
  </head>
  <body>
    <?php
      //Get name and last name
      if(isset($_SESSION['username']) && isset($_SESSION['lastUpdate']))
      {
          //Nav bar
          echo "<nav class='navbar navbar-expand-lg navbar-light bg-light fixed-top'>";
          echo "<img src='img/Icon.png' width='32px' height='38px' class='d-inline-block navbar-brand'></img>";
          echo "<span id='brandText' class='mr-3'>Logical Lab</span>";
          echo "<button class='navbar-toggler' type='button' data-toggle='collapse' data-target='#navbar' aria-controls='navbar' aria-expanded='false' aria-label='Toggle navigation'>
            <span class='navbar-toggler-icon'></span>
          </button>";
          echo "<div class='collapse navbar-collapse' id='navbar'>";
          echo "<ul class='navbar-nav mr-auto'>";
          if(isset($isAdmin) && $isAdmin){  
              echo "<li class='nav-item'>";
                echo "<a class='nav-link' href='AdminManage.php'>Admin Management</a>";
              echo "</li>";
              echo "<li class='nav-item'>";
                echo "<a class='nav-link active' href='LogicalLab.php'>Play Logical Lab</a>";
              echo "</li>";
           }
           else{ 
              echo "<li class='nav-item'>";
                echo "<a class='nav-link active' href='LogicalLab.php'>Play Logical Lab</a>";
              echo "</li>";
           }
           echo "</ul>";
            echo "<span class='navbar-text ml-auto'>".$nav_name." ".$nav_lastname."</span>";
            echo "<ul class='navbar-nav' >";
              echo "<li class='nav-item dropdown'>";
                echo "<a class='nav-link dropdown-toggle' href='#' id='navbarDropdownMenuLink' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></a>";
                echo "<div class='dropdown-menu dropdown-menu-right' aria-labelledby='navbarDropdownMenuLink'>";
                  echo "<a class='dropdown-item' href='EditProfile.php' >Edit Profile</a>";
                  echo "<a class='dropdown-item' href='ChangePassword.php' >Change Password</a>";
                  echo "<a class='dropdown-item' href='SignOut.php'>Sign out</a>";
                echo "</div>";
              echo "</li>";
            echo "</ul>";
          echo "</div>";
        echo "</nav>";
      }
      else{
        echo "<a href='index.php'><img src='img/back.png' width='40px' height='40px' style='margin-left:20px; margin-top:20px;'></img></a>";
      }
    ?>
    
    <div class="webgl-content">
        <div id="gameContainer" style="width: 1280px; height: 600px"></div>
        <footer>
            <div id="credit">
                Icons made by 
                <a href="https://www.flaticon.com/authors/google" title="Google">Google</a> ,
                <a href="https://www.flaticon.com/authors/smashicons" title="Smashicons">Smashicons</a> ,
                <a href="https://www.flaticon.com/authors/dave-gandy" title="Dave Gandy">Dave Gandy</a> ,
                <a href="http://www.freepik.com" title="Freepik">Freepik</a> ,
                <a href="https://www.flaticon.com/authors/those-icons" title="Those Icons">Those Icons</a> ,
                <a href="https://www.flaticon.com/authors/creaticca-creative-agency" title="Creaticca Creative Agency">Creaticca Creative Agency</a> ,
                <a href="https://www.flaticon.com/authors/chanut-is-industries" title="Chanut is Industries">Chanut is Industries</a>
                from 
                <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a> is licensed by 
                <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a>
                <br>
                Sounds made by
                <a href="http://www.freesfx.co.uk">http://www.freesfx.co.uk</a><br>
                This Logical Lab uses these sounds from freesound:
                Access Denied Click by <a href="https://freesound.org/people/suntemple/">suntemple</a> ( https://freesound.org/people/suntemple/ )
            </div>
        </footer>
    </div>
    <?php
        function base64url_encode($data) {
            return rtrim(strtr(base64_encode($data), '+/', '-_'), '=');
        }
    ?>
  </body>
</html>