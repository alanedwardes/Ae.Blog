<?php
R::setup(sprintf('mysql:host=%s;dbname=%s', DB_HOST, DB_NAME), DB_USER, DB_PASS);
R::debug(false);

use AeFramework\Http as Http;

class TemplateView extends AeFramework\Views\TemplateView implements AeFramework\Views\ICacheable
{
	function response($template_data = [])
	{
		return parent::response(array_merge(array(
			'template_name' => $this->template,
			'path' => $_SERVER['REQUEST_URI']
		), $template_data));
	}
	
	function expire()
	{
		return 60 * 60 * 24;
	}
	
	function hash()
	{
	
	}
}

class HomeView extends TemplateView
{
	function getJson($url)
	{
		if (!$data = file_get_contents($url))
			return [];
		
		if (!$data = json_decode($data, true))
			return [];
		
		return $data;
	}

	function response()
	{
		return parent::response(array(
			'posts' => R::getAll('SELECT title, published, slug FROM post WHERE is_published ORDER BY published DESC LIMIT 4'),
			'featured_portfolios' => R::getAll('SELECT * FROM portfolio WHERE featured'),
			'twitter_data' => @$this->getJson('http://jsonpcache.alanedwardes.com/?resource=twitter_ae_timeline&arguments=2'),
			'lastfm_data' => @$this->getJson('http://jsonpcache.alanedwardes.com/?resource=lastfm_ae_albums&arguments=12')['topalbums']['album'],
			'steamgames_data' => @$this->getJson('http://jsonpcache.alanedwardes.com/?resource=steam_ae_games')['mostPlayedGames']['mostPlayedGame'],
			'mapmyrun_data' => @$this->getJson('http://jsonpcache.alanedwardes.com/?resource=mapmyfitness_runs')['result']['output']['workouts']
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
			'all_skills' => R::findAll('skill'),
			'portfolios' => R::find('portfolio', 'type = "published" ORDER BY published DESC')
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
			'portfolios' => $this->skill->sharedPortfolioList
		));
	}
}

class PortfolioSingleView extends TemplateView
{
	private $portfolio;

	function request($verb, array $params = [])
	{
		$this->portfolio = R::findOne('portfolio', 'id = ? AND type = "published"', [$params['portfolio_id']]);
		
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