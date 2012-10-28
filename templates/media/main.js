HTTP = {
	temp: {},
	factory: function()
	{
		var requestObjects = [
			function () { return new XMLHttpRequest(); },
			function () { return new ActiveXObject("Msxml2.XMLHTTP"); },
			function () { return new ActiveXObject("Msxml3.XMLHTTP"); },
			function () { return new ActiveXObject("Microsoft.XMLHTTP"); }
		];
		for (var i = 0; i < requestObjects.length; i++)
		{
			try
			{
				return requestObjects[i]();
				this.factory = requestObjects[i]();
			}
			catch (e)
			{
				// Try another
			}
		}
	},
	getJSON: function(url, success)
	{
		this.get(url, function(data){
			try
			{
				success(JSON.parse(data));
			}
			catch(e)
			{
				var id = new Date().getTime();
				eval('HTTP.temp = [' + id + '] = ' + data);
				success(HTTP.temp[id]);
			}
		});
	},
	get: function(url, success)
	{
		var http = this.factory();
		http.open("GET", url, true);
		
		http.onreadystatechange = function(e){
			if (http.readyState == 4)
			{
				if (http.status == 200)
				{
					success(http.responseText);
				}
			}
			return false;
		}
		
		http.send();
	}
}

var search = {
	searchTimeout: false,
	query: function(queryString)
	{
		clearTimeout(search.searchTimeout);
		queryString = queryString.replace(/[^A-Za-z0-9 ]/g, '');
		if (queryString)
		{
			search.searchTimeout = setTimeout(function(){
				HTTP.getJSON('/json/search?q=' + queryString, function(results){
					var output = [];
					if (results.length)
					{
						output.push('<h2>Results for "' + queryString + '"</h2>');
						output.push('<ul>');
						for (i in results)
						{
							output.push('<li>');
							var result = results[i];
							output.push('<a href="' + result.slug + '">' + result.title + '</a>');
							output.push('<a href="' + result.slug + '#comments" class="commentcount">' + result.comments + '</a>');
							output.push('</li>');
						}
						output.push('</ul>');
					}
					else
					{
						output.push('<h2>No results for ' + queryString + '</h2>');
						output.push('<ul>');
							output.push('<li>Try to use simple search terms</li>');
							output.push('<li>Can\'t find what you\'re looking for? Try <a href="http://www.google.co.uk/search?q=' + queryString + '&sitesearch=alan.edward.es">searching this site for "' + queryString + '" on Google</a></li>');
						output.push('</ul>');
					}
					document.getElementById('searchresults').innerHTML = output.join('');
				});
			}, 50);
		}
		else
		{
			document.getElementById('searchresults').innerHTML = '';
		}
	}
}

var anDur = 25000;
var curIm;
var changeTimeout;
function replaceBg(){
	$('#slideshow').fadeTo(0.7);
	$('#img').remove();
	base = '/static_media/';
	curIm = new Image()
	curIm.onload = function(){
		$('#slideshow').fadeTo(1).html('<a href="' + base + bgImg[curBg] + '"><img id="ssi" width="100%" src="' + base + bgImg[curBg] + '"/></a>');
		searchLinksForLightbox();
		//$('#tbg #ldg').stop().fadeOut(anDur / 10);
		$(curIm).hide().attr('id', 'img');
		$('#tbg').append(curIm);
		$('#img').stop().fadeIn(anDur / 10).animate({
			marginTop: '-=' + ($('#img').height() - 200)
		}, {
			queue: false,
			duration: anDur,
			easing: 'linear'
		});
		
		clearTimeout(changeTimeout);
		changeTimeout = setTimeout(function(){
			$(curIm).fadeOut(anDur / 10, function(){
				return nextBG();
			});
			
		}, anDur/2);
	}
	curIm.src = base + bgImg[curBg]
	
	return false; // don't go to #
}

function nextBG(){
	if(bgImg[curBg+1]){
		curBg++
	}else{
		curBg = 0
	}
	return replaceBg();
}

function lastBG(){
	if(bgImg[curBg-1]){
		curBg--
	}else{
		curBg = bgImg.length - 1
	}
	return replaceBg();
}

// http://www.admixweb.com/2010/08/24/javascript-tip-get-a-random-number-between-two-integers/
function randomFromTo(from, to){
	return Math.floor(Math.random() * (to - from + 1) + from);
}

// http://mattkirman.com/2009/05/11/how-to-recreate-the-konami-code-in-javascript/
var keys = [],konami = '38,38,40,40,37,39,37,39,66,65';
$(window).keydown(function(e){
	keys.push(e.keyCode);
	if(keys.toString().indexOf(konami) >= 0){
		$(document.createElement('div')).attr('id', 'tcial').append(function(){
			return $(document.createElement('div')).attr('id', 'tciali');
		}).click(function(){
			$(this).fadeOut('slow', function(){
				$(this).remove();
			})
		}).appendTo('body').fadeIn();
		keys = [];
	};
});

function ResizeObject(Object,Meta,ImageWidth,ImageHeight){
	Object = $(Object).get(0)
	Meta = $(Meta).get(0)
	// http://www.howtocreate.co.uk/tutorials/javascript/browserwindow
	if( typeof( window.innerWidth ) == 'number' ) {
		WindowWidth = window.innerWidth;
		WindowHeight = window.innerHeight;
	} else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {
		WindowWidth = document.documentElement.clientWidth;
		WindowHeight = document.documentElement.clientHeight;
	} else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {
		WindowWidth = document.body.clientWidth;
		WindowHeight = document.body.clientHeight;
	};
	if( ImageHeight < WindowHeight - 70 ) {
		Height = ImageHeight;
		TopMargin = Height / 2;
		borderBottom = 0;
	} else {
		Height = WindowHeight - 70;
		TopMargin = (WindowHeight - 70) / 2;
		borderBottom = 5;
	};
	if( ImageWidth < WindowWidth - 70 ) {
		Width = ImageWidth;
		LeftMargin = Width / 2;
		borderRight = 0;
	} else {
		Width = WindowWidth - 70;
		LeftMargin = (WindowWidth - 70) / 2;
		borderRight = 5;
	};
	if( Width == ImageWidth && Height == ImageHeight ) {
		Meta.innerHTML = '';
	} else {
		Meta.innerHTML = ' (cropped to <code>' + Width + 'x' + Height + '</code>)';
	};
	Object.style.borderRight = borderRight + 'px dashed #888';
	Object.style.borderBottom = borderBottom + 'px dashed #888';
	Object.style.width = Width + 'px';
	Object.style.height = Height + 'px';
	Object.style.marginLeft = '-' + LeftMargin + 'px';
	Object.style.marginTop = '-' + TopMargin + 'px';
	return Object;
};

function hexToRGB(hex){
	hex = hex.replace('#', '');
	return {
		'r': parseInt(hex.substring(0, 2), 16),
		'g': parseInt(hex.substring(2, 4), 16),
		'b': parseInt(hex.substring(4, 6), 16)
	}
}

window.requestAnimFrame = (function(){
  return  window.requestAnimationFrame       || 
		  window.webkitRequestAnimationFrame || 
		  window.mozRequestAnimationFrame    || 
		  window.oRequestAnimationFrame      || 
		  window.msRequestAnimationFrame     || 
		  function( callback ){
			window.setTimeout(callback, 1000 / 60);
		  };
})();

function an(a, b, s){
	if(a == b){
		return a;
	}else if(a < b){
		return a + s;
	}else{
		return a - s;
	}
}
	
function within(n, t, s){
	return n >= t - s && n <= t + s;
}

function rgbtostr(c){
	return 'rgb(' + c.r + ',' + c.g + ',' + c.b + ')';
}

var currentColour = 0;
var colours = ['046380', 'A20D1E', '6F8C07', '955918', '800463'];

function doColour(fr, to){
	return false;
	// Don't do the stuff below... waaay too CPU intensive.
	/*
	(function animloop(){
		if(new Date().getTime() & 2){
			return requestAnimFrame(animloop);
		}
		step = 3;		
		if(within(fr.r, to.r, step) && within(fr.g, to.g, step) && within(fr.b, fr.b, step)){
			currentColour++;
			if(!colours[currentColour]){
				currentColour = 0;
			}
			return doColour(fr, hexToRGB(colours[currentColour]));
		}else{
			requestAnimFrame(animloop);
		}
	
		fr.r = an(fr.r, to.r, step);
		fr.g = an(fr.g, to.g, step);
		fr.b = an(fr.b, to.b, step);
		
		theme = document.getElementById('theme');
		
		previous = theme.innerHTML.split('color:')[1].split(';}')[0];
		
		theme.innerHTML = theme.innerHTML.split(previous).join(rgbtostr(fr));
    })();*/
}

function action(str){
	return $.getJSON('http://actions.projects.alanedwardes.com/?callback=?', {
		'action': str
	});
}

$(document).ready(function(){
	if(typeof $().pngfix == "function"){
		$('img').pngfix();
	}

	if($('#search-box').val()){
		searchFor($('#search-box').val());
	}
	searchLinksForLightbox();
	
	if(theme == colours[0]){
		currentColour = 1;
		to = colours[1];
	}else{
		to = colours[0];
	}
	
	doColour(hexToRGB(theme), hexToRGB(to));
});

function showLightbox(e, link){
	function fadeAndRemove(object){
		$(object).fadeOut(function(){
			$(this).remove();
		});
	}

	container = $(document.createElement('div')).css('display', 'none').fadeIn();

	$(window).keydown(function(e){
		if(e.keyCode == 27){
			return fadeAndRemove(container);
		}
	});
	
	$('body').append(container);
	
	$(document.createElement('div')).attr('id', 'tint').click(function(){
		return fadeAndRemove(container);
	}).appendTo(container);
		
	loading = $(document.createElement('div')).attr('id', 'loading').html('LOADING').appendTo(container);
	
	img = new Image();
	$(img).error(function(){
		action('lightbox-error-' + link.href);
		if(confirm('Error loading "' + link.href + '".\n\nDo you want to try and open it normally?')){
			document.location = link.href;
		}else{
			return fadeAndRemove(container);
		}
	}).load(function(){
		action('lightbox-load-' + link.href);
		preview = $(document.createElement('div')).attr('id', 'preview').css('display', 'none').fadeIn(function(){
			fadeAndRemove(loading);
		});
		
		meta = $(document.createElement('div'))
			.attr('id', 'meta')
			.html('Size: <code>' + img.width + 'x' + img.height + '</code><span></span>. Click or <code>ESC</code> to close. <a href="' + link.href + '" target="blank">View Original</a>.');
		
		metalabel = $(document.createElement('span')).appendTo(meta);
		
		$(preview).append(meta).append(img).click(function(){
			return fadeAndRemove(container);
		});
		$(container).append(function(){
			return ResizeObject(preview, metalabel, img.width, img.height);
		});
		$(window).resize(function(){
			return ResizeObject(preview, metalabel, img.width, img.height);
		});
	});
	img.src = link.href;
}

function searchLinksForLightbox(){
	$('a').each(function(){
		if($(this).attr('href').match(/(png|jpeg|jpg|gif|bmp)/i) && $(this).attr('class') != 'lightboxlink'){
			$(this).mousedown(function(e){
				showLightbox(e, this);
			}).addClass('lightboxlink');
		}
	});
}