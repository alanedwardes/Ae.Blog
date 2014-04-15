<?php
R::setup(sprintf('mysql:host=%s;dbname=%s', DB_HOST, DB_NAME), DB_USER, DB_PASS);
R::debug(false);
R::freeze(true);

use AeFramework as ae;

class TemplateView extends AeFramework\TwigView
{
	function body($template_data = array())
	{
		return parent::body(array_merge(array(
			'template_name' => $this->template
		), $template_data));
	}
	
	function expire()
	{
		return 60 * 60 * 24;
	}
}

class HomeView extends TemplateView
{
	function getJson($url)
	{
		if (!$data = file_get_contents($url))
			return array();
		
		if (!$data = json_decode($data, true))
			return array();
		
		return $data;
	}

	function body()
	{
		return parent::body(array(
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
	function body()
	{
		return parent::body(array(
			'posts' => R::getAll('SELECT title, published, slug FROM post WHERE is_published ORDER BY published DESC')
		));
	}
}

class SingleView extends TemplateView
{
	private $post;

	function map($params = array())
	{
		$this->post = R::findOne('post', 'slug LIKE ? AND is_published', [$params['slug']]);
	}

	function body()
	{
		return parent::body(array(
			'post' => $this->post,
			'is_single' => true
		));
	}
}

class PortfolioView extends TemplateView
{
	function body()
	{
		return parent::body(array(
			'all_skills' => R::findAll('skill'),
			'portfolios' => R::findAll('portfolio')
		));
	}
}

class PortfolioSkillView extends TemplateView
{
	private $skill;

	function map($params = array())
	{
		$this->skill = R::load('skill', $params['skill_id']);
	}

	function body()
	{
		return parent::body(array(
			'skill' => $this->skill,
			'portfolios' => $this->skill->sharedPortfolioList
		));
	}
}

class PortfolioSingleView extends TemplateView
{
	private $portfolio;

	function map($params = array())
	{
		$this->portfolio = R::load('portfolio', $params['portfolio_id']);
	}

	function body()
	{
		return parent::body(array(
			'portfolio_item' => $this->portfolio,
			'portfolio_item_skills' => $this->portfolio->sharedSkillList,
			'portfolio_item_screenshots' => $this->portfolio->sharedScreenshotList
		));
	}
}

class ContactView extends TemplateView
{

}

class NotFoundView extends TemplateView
{
	function code()
	{
		return ae\HttpCode::NotFound;
	}
}