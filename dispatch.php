<?php
require_once 'config.php';
require_once 'carbo/loader.php';
require_once 'RedBeanPHP/rb.phar';
require_once 'models.php';
require_once 'views.php';

$router = new Carbo\Routing\CachedRouter(new Carbo\Caching\ApcCache(CACHE_KEY));

$authenticator = new Carbo\Auth\MutliFactorArrayAuthenticator($auth_credentials, new Carbo\Sessions\PHPSessionHandler('aeblog'));

$connection = [
	'dbname' => DB_NAME,
	'user' => DB_USER,
	'password' => DB_PASS,
	'host' => DB_HOST,
	'driver' => 'pdo_mysql'
];

Carbo\Routing\RouteMap::map($router, [
	['r^/s/(?P<dropbox_screenshot>.*)/$', 'Carbo\Views\PermanentRedirectView', 'https://dl.dropboxusercontent.com/u/1903330/wc/%s.png'],
	['/estranged', 'Carbo\Views\PermanentRedirectView', 'http://www.iamestranged.com/'],
	['r^/estranged/(?P<path>.*)$', 'Carbo\Views\PermanentRedirectView', 'http://www.iamestranged.com/%s'],
	['/', 'HomeView', 'templates/index.html'],
	['/archive/', 'ArchiveView', 'templates/archive.html'],
	['r^/posts/(?P<slug>.*)/$', 'SingleView', 'templates/single.html'],
	['/portfolio/', 'PortfolioView', 'templates/portfolio.html'],
	['r^/portfolio/item/(?P<portfolio_id>.*)/$', 'PortfolioSingleView', 'templates/portfolio_single.html'],
	['r^/portfolio/skill/(?P<skill_id>.*)/$', 'PortfolioSkillView', 'templates/portfolio.html'],
	['/contact/', 'TemplateView', 'templates/contact.html'],
	['r^/admin/', 'Carbo\Extensions\Admin\AdminRouter', $authenticator, $connection],
	[Carbo\Http\Code::NotFound, 'TemplateView', 'templates/404.html']
]);

$router->despatch();