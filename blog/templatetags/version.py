from django import template
from django.conf import settings
register = template.Library()

@register.simple_tag
def version():
	return open(settings.WEB_ROOT + '/djblog/VERSION', 'r').read().replace('\n', '')