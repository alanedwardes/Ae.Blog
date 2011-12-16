var searchTimeout;
function searchFor(query){
	clearTimeout(searchTimeout)
	if(query){
		return searchTimeout = setTimeout(function(){
			$.getJSON('/json/search?q=' + query, function(results){
				output = '';
				if(results.length){
					output += '<h2>Results for "' + query + '"</h2>';
					output += '<ul>'
					$.each(results, function(i, result){
						output += '<li>'
							output += '<a href="' + result.slug + '">' + result.title + '</a>'
							output += '<a href="' + result.slug + '#comments" class="commentcount">' + result.comments + '</a>'
						output += '</li>'
					});
					output += '</ul>'
				}else{
					output += '<h2>No results for ' + query + '</h2>'
					output += '<ul>'
						output += '<li>Try to use simple search terms</li>'
						output += '<li>Can\'t find what you\'re looking for? Try <a href="http://www.google.co.uk/search?q=' + query + '&sitesearch=alan.edward.es">searching this site for "' + query + '" on Google</a></li>'
					output += '</ul>'
				}
				$('#searchresults').html(output);
			});
		}, 50)
	}else{
		$('#searchresults').html('');
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
			marginTop: '-=' + ($('#img').height() - 200),
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

var currentColour = 0;
var colours = ['046380', 'A20D1E', '6F8C07', '955918', '800463'];
var current = {};

function doColour(fr, to){
	current = fr;
	function an(a, b){
		if(a == b){
			return a;
		}else if(a < b){
			return a+20;
		}else{
			return a-20;
		}
	}
	
	function rgbtostr(c){
		return 'rgb(' + c.r + ',' + c.g + ',' + c.b + ')';
	}

	currentInterval = setInterval(function(){
		if(current.r == to.r && current.g == to.g && current.b == to.b){
			clearInterval(currentInterval);
			currentColour++;
			if(!colours[currentColour]){
				currentColour = 0;
			}
			return doColour(current, hexToRGB(colours[currentColour]));
		}
	
		current.r = an(current.r, to.r);
		current.g = an(current.g, to.g);
		current.b = an(current.b, to.b);
		
		//$('.dbg, label.error, #hdr, .save, #ldg').css('background-color', rgbtostr(current));
		//$('.dbd, input.text:focus, textarea:focus, input.error, textarea.error, #tbg, #ftr, #bdy.shw, .admin .metawrap, .save, .tri h2, #ftr a').css('border-color', rgbtostr(current));
		//$('.dco, a').css('color', rgbtostr(current));
	}, 250000000000);
}

$(document).ready(function(){
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
		if(confirm('Error loading "' + URL + '".\n\nDo you want to try and open it normally?')){
			document.location = URL;
		}else{
			return fadeAndRemove(container);
		}
	}).load(function(){
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