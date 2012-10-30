ae =
{
	util:
	{
		http:
		{
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
						success(ae.util.http.temp[id]);
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
		},
		event: function(element, event, callback)
		{
			// If we didn't supply a valid element, exit.
			if (!element) return;
			// If the W3C way...
			if (element.addEventListener)
			{
				element.addEventListener(event, callback, false);
			}
			// If the Microsoft way
			else if (element.attachEvent)
			{
				element.attachEvent('on' + event, callback);
			}
		},
		element: function(tag, attributes, appendTo)
		{
			var el = document.createElement(tag);
			for (index in attributes)
			{
				var type = typeof attributes[index];
				if (type === "object")
				{
					for (index2 in attributes[index])
					{
						el[index][index2] = attributes[index][index2];
					}
				}
				else
				{
					el[index] = attributes[index];
				}
			}
			if (appendTo) appendTo.appendChild(el);
			return el;
		}
	},
	print_mail: function()
	{
		var mt = 'mailto';
		var ae = 'alanedwardes';
		var pe = '\u002e';
		var cm = 'com';
		var at = '\u0040';
		var co = 'alan';
		return '<a href="' + mt + ':' + co + at + ae + pe + cm + '">' + co + at + ae + pe + cm + '</a>';
	},
	konami: function()
	{
		// http://mattkirman.com/2009/05/11/how-to-recreate-the-konami-code-in-javascript/
		var keys = [], konami = '38,38,40,40,37,39,37,39,66,65';
		ae.util.event(window, 'keydown', function(e){
			keys.push(e.keyCode);
			if (keys.join().indexOf(konami) >= 0)
			{
				ae.util.element('div', {
					id: 'tcial'
				}, document.body).appendChild(
					ae.util.element('div', {
						id: 'tciali'
					})
				);
				keys = [];
			}
		});
	},
	search: {
		searchTimeout: false,
		query: function(queryString)
		{
			clearTimeout(ae.search.searchTimeout);
			queryString = queryString.replace(/[^A-Za-z0-9 ]/g, '');
			if (queryString)
			{
				ae.search.searchTimeout = setTimeout(function(){
					ae.util.http.getJSON('/json/search?q=' + queryString, function(results){
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
	},
	lightbox:
	{
		find: function()
		{
			var linkElements = document.getElementsByTagName('a');
			for (var i = 0; i < linkElements.length; i++)
			{
				var link = linkElements[i];
				if (link.href.match(/(png|jpeg|jpg|gif|bmp)/i) && !link.className.match(/lightboxlink/gi))
				{
					ae.lightbox.addEventToLink(link);
				}
			}
		},
		addEventToLink: function(link)
		{
			link.className += ' lightboxlink';
			ae.util.event(link, 'mousedown', function(e){
				ae.lightbox.show(link);
			});
		},
		show: function(link)
		{
			this.close();
			
			// Container div
			var container = ae.util.element('div', {
				id: 'lightbox-container',
				onclick: ae.lightbox.close
			}, document.body);
			
			// Tint
			ae.util.element('div', {
				id: 'tint',
				onclick: ae.lightbox.close
			}, document.body);
			
			// Loading
			var loading = ae.util.element('div', {
				id: 'loading',
				innerHTML: 'LOADING',
				onclick: ae.lightbox.close
			}, container);
			
			// Image
			var image = ae.util.element('img', {
				onerror: function(e)
				{
					if (confirm('Error loading "' + link.href + '".\n\nDo you want to try and open it normally?'))
					{
						document.location = link.href;
					}
					else
					{
						ae.lightbox.close();
					}
				},
				onload: function(e)
				{
					loading.parentNode.removeChild(loading);
					
					var preview = ae.util.element('div', {
						id: 'preview',
						title: 'Click to close',
						style:
						{
							marginLeft: (-image.width  / 2) + 'px',
							marginTop:  (-image.height / 2) + 'px'
						}
					}, container);
					
					preview.appendChild(image);
				},
				onclick: ae.lightbox.close,
				src: link.href
			}, container);
		},
		close: function()
		{
			var container = document.getElementById('lightbox-container');
			if (container)
			{
				container.parentNode.removeChild(container);
			}
			
			var tint = document.getElementById('tint');
			if (tint)
			{
				tint.parentNode.removeChild(tint);
			}
		}
	},
	init: function(e)
	{
		ae.konami();
		ae.lightbox.find();
	}
}

ae.util.event(window, 'load', ae.init);

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