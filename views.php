<?php
R::setup(sprintf('mysql:host=%s;port=%s;dbname=%s', DB_HOST, DB_PORT, DB_NAME), DB_USER, DB_PASS);
R::debug(false);

use Carbo\Http as Http;

class FaviconView extends Carbo\Views\View
{
	private $color = [0, 0, 0];
	
	function request($verb, array $params = [])
	{
		$this->headers['Content-Type'] = 'image/png';
		$this->color = [
			base_convert(substr($params['color'], 0, 2), 16, 10),
			base_convert(substr($params['color'], 2, 2), 16, 10),
			base_convert(substr($params['color'], 4, 2), 16, 10),
		];
	}
	
	function response($template_data = [])
	{
		$ae = imagecreatetruecolor(16, 16);
		imagefill($ae, 0, 0, imagecolorallocatealpha($ae, 0, 0, 0, 127));

		imagettftext($ae, 20, 0, 0, 13, imagecolorallocate($ae, $this->color[0], $this->color[1], $this->color[2]), './assets/ebg.ttf', chr(230));

		imagesavealpha($ae, true);
		imagealphablending($ae, true);

		imagepng($ae);
		imagedestroy($ae);
	}
}

class ExternalImagesView extends Carbo\Views\View
{
	private $file = '';

	public function request($verb, array $params = [])
	{
		$this->file = '_cache' . DIRECTORY_SEPARATOR . md5($params['url']);
		if (!file_exists($this->file))
		{
			if ($content = @file_get_contents($params['url']))
			{
				$image = @imagecreatefromstring($content);
				@imagejpeg($image, $this->file, 90);
				@imagedestroy($image);
			}
		}
	}

	function response($template_data = [])
	{
		if (file_exists($this->file))
		{
			header('Content-Type: image/jpeg');
			return readfile($this->file);
		}
		else
		{
			throw new Http\CodeException(Http\Code::NotFound);
		}
	}
}

class TemplateView extends Carbo\Views\View
{
	public $template;
	protected $twig;

	public function __construct($template)
	{
		$this->template = $template;
		$loader = new \Twig_Loader_Filesystem('templates');
		$this->twig = new \Twig_Environment($loader, ['cache' => '_cache']);
		
		$links_clickable_filter = new Twig_SimpleFilter('links_clickable', function ($string) {
			return preg_replace('!(((f|ht)tp(s)?://)[-a-zA-Zа-яА-Я()0-9@:%_+.~#?&;//=]+)!i', '<a href="$1">$1</a>', $string);
		});
		$this->twig->addFilter($links_clickable_filter);
	}
	
	public function request($verb, array $params = [])
	{
		$this->headers['Content-Type'] = 'text/html';
	}

	function response($template_data = [])
	{
		return $this->twig->render($this->template, [
			'template_name' => $this->template,
			'path' => $_SERVER['REQUEST_URI']
		] + $template_data);
	}
}

class HomeView extends TemplateView
{
	function getJson($resource, $arguments = null)
	{
		$url = 'http://jsonpcache.alanedwardes.com/?resource=' . $resource . ($arguments === null ? '' : '&arguments=' . $arguments);
		
		if (!$data = file_get_contents($url))
			return [];
		
		if (!$data = json_decode($data, true))
			return [];
		
		return $data;
	}

	function response()
	{
		$run_stats = @end($this->getJson('mapmyfitness_stats')['_embedded']['stats']);
		
		return parent::response(array(
			'posts' => R::getAll('SELECT title, published, slug FROM post WHERE is_published ORDER BY published DESC LIMIT 4'),
			'featured_portfolios' => R::getAll('SELECT * FROM portfolio WHERE featured'),
			'twitter_data' => @$this->getJson('twitter_ae_timeline', 2),
			'lastfm_data' => @$this->getJson('lastfm_ae_albums', 12)['topalbums']['album'],
			'steamgames_data' => @$this->getJson('steam_ae_games')['mostPlayedGames']['mostPlayedGame'],
			'mapmyrun_data' => @array_reverse($this->getJson('mapmyfitness_runs', ($run_stats['activity_count'] - 10))['_embedded']['routes']),
			'mapmyrun_stats' => $run_stats
		));
	}
}

class ArchiveView extends TemplateView
{
	function response()
	{
		return parent::response(array(
			'posts' => R::getAll('SELECT title, published, slug FROM post WHERE is_published ORDER BY published DESC')
		));
	}
}

class SingleView extends TemplateView
{
	private $post;

	function request($verb, array $params = [])
	{
		$this->post = R::findOne('post', 'slug LIKE ? AND is_published', [$params['slug']]);
		
		if (!$this->post)
			throw new Http\CodeException(Http\Code::NotFound);
	}

	function response()
	{
		return parent::response(array(
			'post' => $this->post,
			'is_single' => true
		));
	}
}

class PortfolioView extends TemplateView
{
	function response()
	{
		return parent::response(array(
			'categories' => R::find('category', 'is_published ORDER BY ordering ASC'),
			'portfolios' => R::find('portfolio', 'is_published ORDER BY published DESC')
		));
	}
}

class PortfolioSkillView extends TemplateView
{
	private $skill;

	function request($verb, array $params = [])
	{
		$this->skill = R::findOne('skill', 'id = ?', [$params['skill_id']]);
		
		if (!$this->skill)
			throw new Http\CodeException(Http\Code::NotFound);
	}

	function response()
	{
		return parent::response(array(
			'skill' => $this->skill,
			'categories' => R::find('category', 'is_published ORDER BY ordering ASC'),
			'portfolios' => $this->skill->with('ORDER BY published DESC')->withCondition('is_published')->sharedPortfolioList
		));
	}
}

class PortfolioSingleView extends TemplateView
{
	private $portfolio;

	function request($verb, array $params = [])
	{
		$this->portfolio = R::findOne('portfolio', 'id = ? AND is_published', [$params['portfolio_id']]);
		
		if (!$this->portfolio)
			throw new Http\CodeException(Http\Code::NotFound);
	}

	function response()
	{
		return parent::response(array(
			'portfolio_item' => $this->portfolio,
			'portfolio_item_skills' => $this->portfolio->sharedSkillList,
			'portfolio_item_screenshots' => $this->portfolio->sharedScreenshotList
		));
	}
}