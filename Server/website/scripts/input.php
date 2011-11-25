<?php

$username       = isset($_GET["username"])       ? sanitize($_GET["username"])        : "";
$world_username = isset($_GET["world_username"]) ? sanitize($_GET["world_username"])  : "";
$password       = isset($_GET["password"])       ? sanitize($_GET["password"])        : "";
$email          = isset($_GET["email"])          ? sanitize($_GET["email"])           : "";
$file_path      = isset($_GET["file"])           ? sanitize($_GET["file"])            : "";

$world     = isset($_GET["world"])    ? intval(sanitize($_GET["world"])) : -1;
$level     = isset($_GET["level"])    ? intval(sanitize($_GET["level"])) : -1;
$score     = isset($_GET["score"])    ? intval(sanitize($_GET["score"])) : -1;

?>