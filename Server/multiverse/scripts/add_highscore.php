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
$world    = $_GET["world"];
$level    = $_GET["level"];
$score    = $_GET["score"];


//==================================================
// Sanitize
//==================================================
$username = sanitize($username);
$password = sanitize($password);
$world = sanitize($world);
$level = sanitize($level);
$score = sanitize($score);


//==================================================
// Transform
//==================================================
$world = intval($world);
$level = intval($level);
$score = intval($score);


//==================================================
// Verifications
//==================================================
if (!verify_player_credentials($username, $password))
    return close_database_connection_with_error("credentials");

if (!get_world_exists($world))
    return close_database_connection_with_error("world_not_found");

//todo: verify that the level exists in the world folder


//==================================================
// Add highscore
//==================================================
add_highscore($world, $level, $username, $score);


//==================================================
// Close the connection
//==================================================
close_database_connection();


//==================================================
// Return answer
//==================================================
return get_ok_message();
?>