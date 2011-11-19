<?php

require("database.php");
require("input.php");

$xml = new SimpleXMLElement("<HighScores/>");

if (get_world_exists($world))
{
    $scores = $xml->addChild("Scores");

    $data = get_world_highscores($world);
    
    while ($row = mysql_fetch_array($data))
    {
        $level_id = $row["level_id"];
        $player = $row["username"];
        $score = $row["score"];

        $level = $scores->xpath("Level[@id=$level_id]");
        $level = count($level) == 0 ? null : $level[0];

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
}

close_database_connection();

print get_message("HighScores", $xml->asXML());

?>