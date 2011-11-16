import datetime
from django import template
register = template.Library()

@register.simple_tag
def age():
	return str(datetime.datetime.now().year-1992)