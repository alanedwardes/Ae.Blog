import os, urllib, hashlib
from django import template
register = template.Library()

@register.simple_tag
def md5(str, clean = True):
	if clean:
		return hashlib.md5(str.strip().lower()).hexdigest()
	else:
		return hashlib.md5(str).hexdigest()