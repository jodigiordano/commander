<?php

function get_ok_message()
{
    return get_message("Ok", "");
}

function get_error_message($error)
{
    return get_message("Error", $error);
}


function get_message($type, $data)
{
    $xml = new SimpleXMLElement("<MultiverseMessage/>");
    $xml->addChild("Type", $type);
    $xml->addChild("Message", $data);

    return $xml->asXML();
}

?>