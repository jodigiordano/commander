<?php

//==================================================
// Connect to database
//==================================================
require("database.php");


//==================================================
// Get infos
//==================================================
$username  = $_GET["username"];
$password  = $_GET["password"];
$file_path = $_GET["file"];


//==================================================
// Sanitize
//==================================================
$username  = sanitize($username);
$password  = sanitize($password);
$file_path = sanitize($file_path);


//==================================================
// Verifications
//==================================================

if(!verify_player_credentials($username, $password))
{
    print close_database_connection_with_error("credentials");
    return;
}


//==================================================
// Build file to download
//==================================================
$file_path = "../worlds/" . $file_path;

if (!file_exists($file_path))
{
    print close_database_connection_with_error("file not found");
    return;
}

$file = fopen($file_path, "r");
$output = fread($file, filesize($file_path));
fclose($file);


//==================================================
// Close the connection
//==================================================
close_database_connection();


//==================================================
// Return files to download
//==================================================
print get_message("LoadFile", $output);


?>