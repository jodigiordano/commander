<?php

//==================================================
// Connect to database
//==================================================
require("database.php");


//==================================================
// Get infos
//==================================================
$username = $_GET["username"];
$password = $_GET["password"];
$email    = $_GET["email"];


//==================================================
// Sanitize
//==================================================
$username = sanitize($username);
$password = sanitize($password);
$email    = sanitize($email);


//==================================================
// Verify infos
//==================================================
if(!filter_var($email, FILTER_VALIDATE_EMAIL))
{
    print close_database_connection_with_error("email is not valid");
    return;
}

if(strlen($username) < 4 || strlen($username) > 200)
{
    print close_database_connection_with_error("username must be between 4 and 40.");
    return;
}

if(strlen($password) < 4 || strlen($password) > 200)
{
    print close_database_connection_with_error("password must be between 4 and 40.");
    return;
}


//==================================================
//Verify that the username is unique
//==================================================
if (get_player_exists($username))
{
    print close_database_connection_with_error("username already taken.");
    return;
}


//==================================================
//Save the infos in the database
//==================================================
$world_id = add_player($username, $password, $email);


//==================================================
// Close the connection and send result
//==================================================
close_database_connection();

print get_message("NewPlayer", $world_id);

?>