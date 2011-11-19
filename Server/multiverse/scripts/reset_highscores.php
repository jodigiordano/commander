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


//==================================================
// Reset highscores
//==================================================
reset_highscores($world);


//==================================================
// Close the connection and send result
//==================================================
close_database_connection();

print get_ok_message();
?>