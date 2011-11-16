<?php

require("database.php");
require("input.php");


//==================================================
// Verify the credentials
//==================================================
if (!verify_player_credentials($username, $password))
{
    print close_database_connection_with_error("credentials");
    return;
}


//==================================================
// Get the world ID
//==================================================
$world_id = get_world_id_by_username($username);


//==================================================
// Close the connection and send result
//==================================================
close_database_connection();

print get_message("Login", $world_id);

?>