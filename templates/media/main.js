(function($) {
	if(!document.defaultView || !document.defaultView.getComputedStyle){ // IE6-IE8
		var oldCurCSS = $.curCSS;
		$.curCSS = function(elem, name, force){
			if(name === 'background-position'){
				name = 'backgroundPosition';
			}
			if(name !== 'backgroundPosition' || !elem.currentStyle || elem.currentStyle[ name ]){
				return oldCurCSS.apply(this, arguments);
			}
			var style = elem.style;
			if ( !force && style && style[ name ] ){
				return style[ name ];
			}
			return oldCurCSS(elem, 'backgroundPositionX', force) +' '+ oldCurCSS(elem, 'backgroundPositionY', force);
		};
	}
	
	var oldAnim = $.fn.animate;
	$.fn.animate = function(prop){
		if('background-position' in prop){
			prop.backgroundPosition = prop['background-position'];
			delete prop['background-position'];
		}
		if('backgroundPosition' in prop){
			prop.backgroundPosition = '('+ prop.backgroundPosition;
		}
		return oldAnim.apply(this, arguments);
	};
	
	function toArray(strg){
		strg = strg.replace(/left|top/g,'0px');
		strg = strg.replace(/right|bottom/g,'100%');
		strg = strg.replace(/([0-9\.]+)(\s|\)|$)/g,"$1px$2");
		var res = strg.match(/(-?[0-9\.]+)(px|\%|em|pt)\s(-?[0-9\.]+)(px|\%|em|pt)/);
		return [parseFloat(res[1],10),res[2],parseFloat(res[3],10),res[4]];
	}
	
	$.fx.step. backgroundPosition = function(fx) {
		if (!fx.bgPosReady) {
			var start = $.curCSS(fx.elem,'backgroundPosition');
			if(!start){//FF2 no inline-style fallback
				start = '0px 0px';
			}
			
			start = toArray(start);
			fx.start = [start[0],start[2]];
			var end = toArray(fx.end);
			fx.end = [end[0],end[2]];
			
			fx.unit = [end[1],end[3]];
			fx.bgPosReady = true;
		}
		//return;
		var nowPosX = [];
		nowPosX[0] = ((fx.end[0] - fx.start[0]) * fx.pos) + fx.start[0] + fx.unit[0];
		nowPosX[1] = ((fx.end[1] - fx.start[1]) * fx.pos) + fx.start[1] + fx.unit[1];           
		fx.elem.style.backgroundPosition = nowPosX[0]+' '+nowPosX[1];

	};
})(jQuery);

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

var anDur = 20000;
var curIm;
function replaceBg(){
	base = '/static_media/';
	curIm = new Image()
	curIm.onload = function(){
		$('#slideshow').html('<a href="' + base + bgImg[curBg] + '"><img id="ssi" width="100%" src="' + base + bgImg[curBg] + '"/></a>');
		searchLinksForLightbox();
		$('#tbg').css({
			'background-image': 'url(' + base + bgImg[curBg] + ')',
			'background-position': '0px 0px'
		});
		$('#tbg #ldg').stop().fadeOut(anDur / 10);
		$('#tbg').animate({
			backgroundPosition: '(0px -' + (curIm.height - 300) + 'px)'
		}, {
			duration: anDur,
			easing: 'linear',
			complete: function(){
				return replaceBg();
			}
		});
		setTimeout(function(){
			$('#tbg #ldg').css('opacity', 1).fadeIn(anDur / 10, function(){
				$('#tbg').css('background-image', '');
			});
		}, anDur - (anDur / 10));
		if(bgImg[curBg+1]){
			curBg++
		}else{
			curBg = 0
		}
	}
	curIm.src = base + bgImg[curBg]
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