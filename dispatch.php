<?php
use AeFramework as ae;

require_once 'config.php';
require_once 'AeFramework/loader.php';
require_once 'RedBeanPHP/rb.phar';
require_once 'models.php';
require_once 'views.php';

$router = new ae\CachedRouter(new ae\ApcCache, CACHE_KEY);

$router->route(new ae\StringMapper('/estranged', new ae\PermanentRedirectView('http://www.iamestranged.com/')));

$router->route(new ae\RegexMapper('^/estranged/(?P<path>.*)$', new ae\PermanentRedirectView('http://www.iamestranged.com/%s')));

$router->route(new ae\StringMapper('/', new HomeView('templates/index.html')));

$router->route(new ae\StringMapper('/archive/', new ArchiveView('templates/archive.html')));

$router->route(new ae\RegexMapper('^/posts/(?P<slug>.*)/$', new SingleView('templates/single.html')));

$router->route(new ae\StringMapper('/portfolio/', new PortfolioView('templates/portfolio.html')));

$router->route(new ae\RegexMapper('^/portfolio/item/(?P<portfolio_id>.*)/$', new PortfolioSingleView('templates/portfolio_single.html')));

$router->route(new ae\RegexMapper('^/portfolio/skill/(?P<skill_id>.*)/$', new PortfolioSkillView('templates/portfolio.html')));

$router->route(new ae\StringMapper('/contact/', new ContactView('templates/contact.html')));

$router->error(ae\HttpCode::NotFound, new NotFoundView('templates/404.html'));

echo $router->despatch();