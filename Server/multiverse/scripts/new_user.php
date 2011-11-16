<?php

require("database.php");
require("input.php");


//==================================================
// Verify infos
//==================================================
if(!filter_var($email, FILTER_VALIDATE_EMAIL))
{
    print close_database_connection_with_error("email not valid");
    return;
}

if(strlen($username) < 4 || strlen($username) > 200)
{
    print close_database_connection_with_error("username length");
    return;
}

if(strlen($password) < 4 || strlen($password) > 200)
{
    print close_database_connection_with_error("password length");
    return;
}


//==================================================
//Verify that the username is unique
//==================================================
if (get_player_exists($username))
{
    print close_database_connection_with_error("username already taken");
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