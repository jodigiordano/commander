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
    print close_database_connection_with_error("world_not_found");
    return;
}


$output = get_world_last_update_timestamp($world);


//==================================================
// Close the connection
//==================================================
close_database_connection();


//==================================================
// Return files to download
//==================================================
print get_message("LastUpdate", $output);


?>