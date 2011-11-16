from datetime import date
from django import template
register = template.Library()

@register.filter
def nl2li(str, size=48):
	result = ''
	for line in str.split('\n'):
		result += '<li>' + line + '</li>'
	return result