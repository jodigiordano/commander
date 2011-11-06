<?php

require("database.php");
require("input.php");


if (!get_world_exists($world))
    exit_with_error("world_not_found");

$data = get_world_highscores($world);

$xml = new SimpleXMLElement("<HighScores/>");
$scores = $xml->addChild("Scores");

while ($row = mysql_fetch_array($data))
{
    $level_id = $row["level_id"];
    $player = $row["username"];
    $score = $row["score"];

    $level = $scores->xpath("Level[@id=$level_id]");
    $level = $level[0];

    if ($level == null)
    {
        $level = $scores->addChild("Level");
        $level->addAttribute("id", $level_id);
        $level->addChild("Scores");
    }
    
    $level = $level->Scores;
    
    $entry = $level->addChild("Score");
    $entry->addAttribute("player", $player);
    $entry->addAttribute("score", $score);
}

close_database_connection();

return get_message("HighScores", $xml->asXML());

?>