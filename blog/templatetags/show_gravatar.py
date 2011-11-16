import os, urllib, hashlib
from django import template
register = template.Library()

@register.simple_tag
def show_gravatar(email, size=48):
	return 'http://www.gravatar.com/avatar/' + hashlib.md5(email).hexdigest() + '.png?s=' + str(size) + '&amp;r=g&amp;d=monsterid'
