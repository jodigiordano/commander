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

if(!get_player_exists($world_username))
{
    print close_database_connection_with_error("player not found");
    return;
}

$world_id = get_world_id_by_username($world_username);


//==================================================
// Close the connection
//==================================================
close_database_connection();


//==================================================
// Return files to download
//==================================================
print get_message("WorldId", $world_id);


?>