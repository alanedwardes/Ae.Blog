import os
from django.conf import settings
from django import template
register = template.Library()

@register.simple_tag
def media_include(res):
	mod = os.path.getmtime(os.path.join(settings.MEDIA_ROOT, res))
	
	return str.format("{0}{1}?last_modified={2}", settings.PUBLIC_MEDIA_ROOT, res, mod)