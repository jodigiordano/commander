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

//todo: verify that the level exists in the world folder


//==================================================
// Add highscore
//==================================================
add_highscore($world, $level, $username, $score);


//==================================================
// Close the connection and send result
//==================================================
close_database_connection();

print get_ok_message();
?>