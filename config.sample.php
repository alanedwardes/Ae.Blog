<?php
error_reporting(E_ALL);
date_default_timezone_set('Europe/London');
set_include_path('');

define('DB_HOST', '');
define('DB_NAME', '');
define('DB_USER', '');
define('DB_PASS', '');

$auth_credentials = ['user' => ['password', 'mfa_secret']];
