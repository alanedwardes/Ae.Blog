<?php
use AeFramework as ae;

require_once 'config.php';
require_once 'AeFramework/loader.php';
require_once 'RedBeanPHP/rb.phar';
require_once 'models.php';
require_once 'views.php';

$router = new ae\Routing\CachedRouter(new ae\Caching\ApcCache(CACHE_KEY));

ae\Routing\RouteMap::map($router, [
	['/estranged', 'AeFramework\PermanentRedirectView', 'http://www.iamestranged.com/'],
	['r^/estranged/(?P<path>.*)$', 'AeFramework\PermanentRedirectView', 'http://www.iamestranged.com/%s'],
	['/', 'HomeView', 'templates/index.html'],
	['/archive/', 'ArchiveView', 'templates/archive.html'],
	['r^/posts/(?P<slug>.*)/$', 'SingleView', 'templates/single.html'],
	['/portfolio/', 'PortfolioView', 'templates/portfolio.html'],
	['r^/portfolio/item/(?P<portfolio_id>.*)/$', 'PortfolioSingleView', 'templates/portfolio_single.html'],
	['r^/portfolio/skill/(?P<skill_id>.*)/$', 'PortfolioSkillView', 'templates/portfolio.html'],
	['/contact/', 'TemplateView', 'templates/contact.html'],
	['r^/admin/', 'AeFramework\Extensions\Admin\AdminRouter', new ae\ArrayAuthenticator($auth_credentials)],
	[ae\HttpCode::NotFound, 'TemplateView', 'templates/404.html']
]);

echo $router->despatch();