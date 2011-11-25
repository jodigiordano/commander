<?php

require("response_builder.php");

//==================================================
// Database infos
//==================================================
//$sql_username      = "epheme5_root";
//$sql_password      = "C0mmand3rJ0d1";
//$sql_database      = "epheme5_commander";
$sql_username      = "root";
$sql_password      = "";
$sql_database      = "commander";

$sql_hostname      = "localhost";
$sql_table_players = "players";
$sql_table_worlds  = "worlds";
$sql_table_scores  = "scores";
$sql_table_ratings = "ratings";


//==================================================
// Connect to the database
//==================================================

$handle = mysql_connect($sql_hostname, $sql_username, $sql_password);

if ($handle == FALSE)
    exit_with_error("server down");

mysql_select_db($sql_database, $handle);

if (mysql_errno($handle) != 0)
    exit_with_error("server down");


//==================================================
// Data functions
//==================================================

function get_player($username)
{
    global $sql_table_players;

    $result = execute_request("SELECT * FROM $sql_table_players WHERE username='$username'");

    return mysql_num_rows($result) == 0 ? null : mysql_fetch_array($result);
}


function get_player_exists($username)
{
    return get_player($username) != null;
}


function add_player($username, $password, $email)
{
    global $handle, $sql_table_players, $sql_table_worlds;

    execute_request("INSERT INTO $sql_table_players (username, password, email) VALUES ('$username', '$password', '$email')");
    
    $id = mysql_insert_id($handle);
    
    execute_request("INSERT INTO $sql_table_worlds (id) VALUES ('$id')");

    return $id;
}


function verify_player_credentials($username, $password)
{
    $player = get_player($username);
    
    return $player != null && strcmp($player["password"], $password) == 0;
}


function get_world_id_by_username($username)
{
    $player = get_player($username);

    return $player == null ? -1 : $player["id"];
}


function get_world_by_username($username)
{
    return get_world(get_world_id_by_username($username));
}


function get_world($id)
{
    global $sql_table_worlds;

    $result = execute_request("SELECT * FROM $sql_table_worlds WHERE id='$id'");

    return mysql_num_rows($result) == 0 ? null : mysql_fetch_array($result);
}


function get_world_exists($id)
{
    return get_world($id) != null;
}


function get_world_last_update_timestamp($id)
{
    global $sql_table_worlds;

    $world = get_world($id);
    
    return $world["last_update"];
}


function reset_highscores($world_id)
{
    global $sql_table_scores;

    execute_request("DELETE FROM $sql_table_scores WHERE world_id='$world_id'");
}


function get_world_highscores($world_id)
{
    global $sql_table_scores;

    return execute_request("SELECT * FROM $sql_table_scores WHERE world_id='$world_id'");
}


function get_level_highscores($world_id, $level_id)
{
    global $sql_table_scores;
    
    return execute_request("SELECT * FROM $sql_table_scores WHERE world_id='$world_id' AND level_id='$level_id'");
}


function add_highscore($world_id, $level_id, $username, $score)
{
    global $sql_table_scores;

    $results = get_level_highscores($world_id, $level_id);
    
    $scores_count = mysql_num_rows($results);
    $lowest = PHP_INT_MAX;
    $exists = FALSE;
    
    while ($row = mysql_fetch_array($results))
    {
        $db_username = $row["username"];
        $db_score = intval($row["score"]);
    
        // don't insert the highscore twice
        if (strcmp($db_username, $username) == 0 && $db_score == $score)
        {
            $exists = TRUE;
            break;
        }
        
        if ($db_score < $lowest)
            $lowest = $db_score;
    }
    
    
    if ($exists == TRUE)
        return;

    $request_add =
        "INSERT INTO $sql_table_scores (world_id, level_id, username, score) " .
        "VALUES ('$world_id', '$level_id', '$username', '$score')";
        
    $request_remove =
        "DELETE FROM $sql_table_scores WHERE " .
        "world_id='$world_id' AND level_id='$level_id' AND score='$lowest'";        

    if ($scores_count >= 10 && $score > $lowest)
    {
        execute_request($request_remove);
        execute_request($request_add);
    }
    
    else if ($scores_count < 10)
    {
        execute_request($request_add);
    }
}


//==================================================
// Helpers
//==================================================

function execute_request($request)
{
    global $handle;

    $result = mysql_query($request, $handle);

    if (mysql_errno($handle) != 0)
        close_database_connection_with_error("nice try.");

    return $result;
}


function close_database_connection()
{
    global $handle;

    if ($handle == FALSE)
        return;

    mysql_close($handle);
}


function close_database_connection_with_error($error)
{
    close_database_connection();
    
    return get_error_message($error);
}


function close_database_connection_with_ok()
{
    close_database_connection();
    
    return get_ok_message();
}


function sanitize($data)
{
    global $handle;

    return mysql_real_escape_string($data, $handle);
}

?>