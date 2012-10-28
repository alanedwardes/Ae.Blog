from django import template
from django.conf import settings
register = template.Library()

@register.simple_tag
def version():
	return open(settings.BLOG_ROOT + 'VERSION', 'r').read().replace('\n', '')