/*  Snowfall jquery plugin
	Version 1.5 Oct 5th 2011
	Added collecting snow! Uses the canvas element to collect snow. In order to initialize snow collection use the following
	
	$(document).snowfall({collection : 'element'});

    element = any valid jquery selector.

	The plugin then creates a canvas above every element that matches the selector, and collects the snow. If there are a varrying amount of elements the 
	flakes get assigned a random one on start they will collide.

	Version 1.4 Dec 8th 2010
	Fixed issues (I hope) with scroll bars flickering due to snow going over the edge of the screen. 
	Added round snowflakes via css, will not work for any version of IE. - Thanks to Luke Barker of http://www.infinite-eye.com/
	Added shadows as an option via css again will not work with IE. The idea behind shadows, is to show flakes on lighter colored web sites - Thanks Yutt
 
	Version 1.3.1 Nov 25th 2010
	Updated script that caused flakes not to show at all if plugin was initialized with no options, also added the fixes that Han Bongers suggested 
	
	Developed by Jason Brown for any bugs or questions email me at loktar69@hotmail
	info on the plugin is located on Somethinghitme.com
	
	values for snow options are
	
	flakeCount,
	flakeColor,
	flakeIndex,
	minSize,
	maxSize,
	minSpeed,
	maxSpeed,
	round, 		true or false, makes the snowflakes rounded if the browser supports it.
	shadow		true or false, gives the snowflakes a shadow if the browser supports it.
	
	Example Usage :
	$(document).snowfall({flakeCount : 100, maxSpeed : 10});
	
	-or-
	
	$('#element').snowfall({flakeCount : 800, maxSpeed : 5, maxSize : 5});
	
	-or with defaults-
	
	$(document).snowfall();
	
	- To clear -
	$('#element').snowfall('clear');
*/

(function($){
	$.snowfall = function(element, options){
		var	defaults = {
				flakeCount : 35,
				flakeColor : '#ffffff',
				flakeIndex: 999999,
				minSize : 1,
				maxSize : 2,
				minSpeed : 1,
				maxSpeed : 5,
				round : false,
				shadow : false,
				collection : false,
				collectionHeight : 40
			},
			options = $.extend(defaults, options),
			random = function random(min, max){
				return Math.round(min + Math.random()*(max-min)); 
			};

			$(element).data("snowfall", this);			

			// Snow flake object
			function Flake(_x, _y, _size, _speed, _id)
			{
				// Flake properties
				this.id = _id; 
				this.x  = _x;
				this.y  = _y;
				this.size = _size;
				this.speed = _speed;
				this.step = 0;
				this.stepSize = random(1,10) / 100;

				if(options.collection){
					this.target = canvasCollection[random(0,canvasCollection.length-1)];
				}

				var flakeMarkup = $(document.createElement("div")).attr({'class': 'snowfall-flakes', 'id' : 'flake-' + this.id}).css({'width' : this.size, 'height' : this.size, 'background' : options.flakeColor, 'position' : 'absolute', 'top' : this.y, 'left' : this.x, 'fontSize' : 0, 'zIndex' : options.flakeIndex});

				if($(element).get(0).tagName === $(document).get(0).tagName){
					$('body').append(flakeMarkup);
					element = $('body');
				}else{
					$(element).append(flakeMarkup);
				}

				this.element = document.getElementById('flake-' + this.id);

				// Update function, used to update the snow flakes, and checks current snowflake against bounds
				this.update = function(){
					this.y += this.speed;

					if(this.y > (elHeight) - (this.size  + 6)){
						this.reset();
					}

					this.element.style.top = this.y + 'px';
					this.element.style.left = this.x + 'px';

					this.step += this.stepSize;
					this.x += Math.cos(this.step);

					// Pileup check
					if(options.collection){
						if(this.x > this.target.x && this.x < this.target.width + this.target.x && this.y > this.target.y && this.y < this.target.height + this.target.y){
							var ctx = this.target.element.getContext("2d"),
								curX = this.x - this.target.x,
								curY = this.y - this.target.y,
								colData = this.target.colData;

								if(colData[parseInt(curX)][parseInt(curY+this.speed+this.size)] !== undefined || curY+this.speed+this.size > this.target.height){
									if(curY+this.speed+this.size > this.target.height){
										while(curY+this.speed+this.size > this.target.height && this.speed > 0){
											this.speed *= .5;
										}

										ctx.fillStyle = "#fff";

										if(colData[parseInt(curX)][parseInt(curY+this.speed+this.size)] == undefined){
											colData[parseInt(curX)][parseInt(curY+this.speed+this.size)] = 1;
											ctx.fillRect(curX, (curY)+this.speed+this.size, this.size, this.size);
										}else{
											colData[parseInt(curX)][parseInt(curY+this.speed)] = 1;
											ctx.fillRect(curX, curY+this.speed, this.size, this.size);
										}
										this.reset();
									}else{
										// flow to the sides
										this.speed = 1;
										this.stepSize = 0;

										if(parseInt(curX)+1 < this.target.width && colData[parseInt(curX)+1][parseInt(curY)+1] == undefined ){
											// go left
											this.x++;
										}else if(parseInt(curX)-1 > 0 && colData[parseInt(curX)-1][parseInt(curY)+1] == undefined ){
											// go right
											this.x--;
										}else{
											//stop
											ctx.fillStyle = "#fff";
											ctx.fillRect(curX, curY, this.size, this.size);
											colData[parseInt(curX)][parseInt(curY)] = 1;
											this.reset();
										}
									}
								}
						}
					}

					if(this.x > (elWidth) - widthOffset || this.x < widthOffset){
						this.reset();
					}
				}

				// Resets the snowflake once it reaches one of the bounds set
				this.reset = function(){
					this.y = 0;
					this.x = random(widthOffset, elWidth - widthOffset);
					this.stepSize = random(1,10) / 100;
					this.size = random((options.minSize * 100), (options.maxSize * 100)) / 100;
					this.speed = random(options.minSpeed, options.maxSpeed);
				}
			}

			// Private vars
			var flakes = [],
				flakeId = 0,
				i = 0,
				elHeight = $(element).height(),
				elWidth = $(element).width(),
				widthOffset = 0,
				snowTimeout = 0;

			// Collection Piece ******************************
			if(options.collection !== false){
				var testElem = document.createElement('canvas');
				if(!!(testElem.getContext && testElem.getContext('2d'))){
					var canvasCollection = [],
						elements = $(options.collection),
						collectionHeight = options.collectionHeight;

					for(var i =0; i < elements.length; i++){
							var bounds = elements[i].getBoundingClientRect(),
								canvas = document.createElement('canvas'),
								collisionData = [];

							if(bounds.top-collectionHeight > 0){									
								document.body.appendChild(canvas);
								canvas.style.position = 'absolute';
								canvas.height = collectionHeight;
								canvas.width = bounds.width;
								canvas.style.left = bounds.left;
								canvas.style.top = bounds.top-collectionHeight;

								for(var w = 0; w < bounds.width; w++){
									collisionData[w] = [];
								}

								canvasCollection.push({element :canvas, x : bounds.left, y : bounds.top-collectionHeight, width : bounds.width, height: collectionHeight, colData : collisionData});
							}
					}
				}else{
					// Canvas element isnt supported
					options.collection = false;
				}
			}
			// ************************************************

			// This will reduce the horizontal scroll bar from displaying, when the effect is applied to the whole page
			if($(element).get(0).tagName === $(document).get(0).tagName){
				widthOffset = 25;
			}

			// Bind the window resize event so we can get the innerHeight again
			$(window).bind("resize", function(){  
				elHeight = $(element).height();
				elWidth = $(element).width();
			}); 


			// initialize the flakes
			for(i = 0; i < options.flakeCount; i+=1){
				flakeId = flakes.length;
				flakes.push(new Flake(random(widthOffset,elWidth - widthOffset), random(0, elHeight), random((options.minSize * 100), (options.maxSize * 100)) / 100, random(options.minSpeed, options.maxSpeed), flakeId));
			}

			// This adds the style to make the snowflakes round via border radius property 
			if(options.round){
				$('.snowfall-flakes').css({'-moz-border-radius' : options.maxSize, '-webkit-border-radius' : options.maxSize, 'border-radius' : options.maxSize});
			}

			// This adds shadows just below the snowflake so they pop a bit on lighter colored web pages
			if(options.shadow){
				$('.snowfall-flakes').css({'-moz-box-shadow' : '1px 1px 1px #555', '-webkit-box-shadow' : '1px 1px 1px #555', 'box-shadow' : '1px 1px 1px #555'});
			}

			// this controls flow of the updating snow
			function snow(){
				for( i = 0; i < flakes.length; i += 1){
					flakes[i].update();
				}

				snowTimeout = setTimeout(function(){snow()}, 30);
			}

			snow();

		// Public Methods

		// clears the snowflakes
		this.clear = function(){
						$(element).children('.snowfall-flakes').remove();
						flakes = [];
						clearTimeout(snowTimeout);
					};
	};

	// Initialize the options and the plugin
	$.fn.snowfall = function(options){
		if(typeof(options) == "object" || options == undefined){		
				 return this.each(function(i){
					(new $.snowfall(this, options)); 
				});	
		}else if (typeof(options) == "string") {
			return this.each(function(i){
				var snow = $(this).data('snowfall');
				if(snow){
					snow.clear();
				}
			});
		}
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
	if(typeof bgImg == "undefined"){
		$(document).snowfall({
			minSize : 3,
			maxSize : 6,
			minSpeed : 2,
			maxSpeed : 4,
			flakeCount : 50,
			round : true,
			shadow: true
		});
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