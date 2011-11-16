<?php

require("database.php");
require("input.php");


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


$player_world = get_world_id_by_username($username);

if ($world != $player_world)
{
    print close_database_connection_with_error("credentials");
    return;
}


if (!is_uploaded_file($_FILES["file"]["tmp_name"]))
{
    print close_database_connection_with_error("file not uploaded");
    return;
}


//==================================================
// Get the file and put it in the right directory
//==================================================

$upload_dir = "../worlds/world" . $world . "/";
$upload_file = $upload_dir . "world" . $world . ".zip";

if (!file_exists($upload_dir))
    mkdir($upload_dir);


$moved = move_uploaded_file($_FILES["file"]["tmp_name"], $upload_file);

if ($moved == FALSE)
{
    print close_database_connection_with_error("file not uploaded");
    return;
}


//==================================================
// Close the connection
//==================================================
close_database_connection();


//==================================================
// Return files to download
//==================================================
print get_message("SaveWorld", $output);


?>