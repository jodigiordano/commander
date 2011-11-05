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


//==================================================
// Sanitize
//==================================================
$username = sanitize($username);
$password = sanitize($password);
$world = sanitize($world);


//==================================================
// Transform
//==================================================
$world = intval($world);


//==================================================
// Verifications
//==================================================

if(!verify_player_credentials($username, $password))
{
    print close_database_connection_with_error("credentials");
    return;
}

if(!get_world_exists($world))
{
    print close_database_connection_with_error("world not found");
    return;
}

//==================================================
// Build files list to download
//==================================================
$output = "";

foreach (glob("../worlds/world" . $world . "/*") as $filename) {
    $output = $output . basename($filename) . ",";
}


//==================================================
// Close the connection
//==================================================
close_database_connection();


//==================================================
// Return files to download
//==================================================
print get_message("LoadWorld", $output);


?>