import re, os
from django import template

register = template.Library()

@register.filter
def specialtags( text ):
	for match in re.findall('\[s3\](.*?)\[/s3\]', text):
		text = text.replace('[s3]'+ match +'[/s3]', '/static_media/' + match)
	for match in re.findall('\[yt\](.*?)\[/yt\]', text):
		#YouTubeURL = 'http://www.youtube.com/v/' + match + '?version=3&amp;rel=0'
		#text = text.replace('[yt]'+ match +'[/yt]', '<object type="application/x-shockwave-flash" data="' + YouTubeURL + '" width="600" height="375"><param name="movie" value="' + YouTubeURL + '"/><param name="wmode" value="transparent"/></object>')
		text = text.replace('[yt]'+ match +'[/yt]', '<iframe width="600" height="375" src="http://www.youtube.com/embed/' + match + '" style="border:0px"></iframe>')
	for match in re.findall('\[av\](.*?)\[/av\]', text):
		text = text.replace('[av]' + match + '[/av]', 
			'<object type="application/x-shockwave-flash" data="/static_media/jwplayer.swf" width="600" height="338">' +
				'<param name="movie" value="/static_media/jwplayer.swf"/>' +
				'<param name="wmode" value="transparent"/>' +
				'<param name="allowfullscreen" value="true"/>' +
				'<param name="allowscriptaccess" value="true"/>' +
				'<param name="flashvars" value="' +
					'controlbar.position=over' +
					'&file=/static_media/video/' + match + '.mp4' +
					'&image=/static_media/video/' + match + '.png' +
				'"/>' +
			'</object>'
		)
	return text.replace('http://cdn.alanedwardes.com', '/static_media')
