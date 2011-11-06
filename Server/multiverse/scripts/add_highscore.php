<?php

require("database.php");
require("input.php");


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