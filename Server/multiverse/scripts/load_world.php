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