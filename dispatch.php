<?php
require_once 'config.php';
require_once 'carbo/loader.php';
require_once 'RedBeanPHP/rb.phar';
require_once 'models.php';
require_once 'views.php';

$router = new Carbo\Routing\Router;

$session = new Carbo\Sessions\PHPSessionHandler('aeblog');
$authenticator = new Carbo\Auth\MutliFactorArrayAuthenticator($auth_credentials, $session);

$connection = [
	'dbname' => DB_NAME,
	'user' => DB_USER,
	'password' => DB_PASS,
	'host' => DB_HOST,
	'port' => DB_PORT,
	'driver' => 'pdo_mysql'
];

$stats_connection = [
	'dbname' => DB_NAME,
	'user' => DB_USER,
	'password' => DB_PASS,
	'host' => DB_HOST,
	'port' => DB_PORT,
	'driver' => 'pdo_mysql',
	'prefix' => 'ae'
];

Carbo\Mapping\Map::create($router, [
	['r^/s/(?P<dropbox_screenshot>.*)/$', 'Carbo\Views\PermanentRedirectView', 'https://dl.dropboxusercontent.com/u/1903330/wc/%s.png'],
	['/estranged', 'Carbo\Views\PermanentRedirectView', 'http://www.iamestranged.com/'],
	['r^/estranged/(?P<path>.*)$', 'Carbo\Views\PermanentRedirectView', 'http://www.iamestranged.com/%s' . (isset($_SERVER['QUERY_STRING']) ? '?'. $_SERVER['QUERY_STRING'] : '')],
	['/', 'HomeView', 'index.html'],
	['r^/ext/(?P<url>.*)$', 'ExternalImagesView'],
	['/archive/', 'ArchiveView', 'archive.html'],
	['r^/posts/(?P<slug>.*)/$', 'SingleView', 'single.html'],
	['/portfolio/', 'PortfolioView', 'portfolio.html'],
	['r^/portfolio/item/(?P<portfolio_id>.*)/$', 'PortfolioSingleView', 'portfolio_single.html'],
	['r^/portfolio/skill/(?P<skill_id>.*)/$', 'PortfolioSkillView', 'portfolio.html'],
	['/contact/', 'TemplateView', 'contact.html'],
	['r^/favicon/(?P<color>[A-Fa-z0-9]+).png$', 'FaviconView'],
	['r^/admin/', 'Carbo\Extensions\Admin\AdminRouter', $authenticator, $connection, $stats_connection],
	[Carbo\Http\Code::NotFound, 'TemplateView', '404.html']
]);

$router->despatch();

Carbo\Extensions\Statistics\Collector::collect($stats_connection, $session->id());