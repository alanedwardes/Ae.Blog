import datetime, re, cgi
from random import choice
from datetime import date
from django import template
register = template.Library()

@register.tag
def random_item(parser, token):
	tag_name, str = token.split_contents()
	return RandomItemNode(str)

class RandomItemNode(template.Node):
	def __init__(self, str):
		self.str = str
	def render(self, context):
		context['random_item'] = choice(self.str.split('|'))
		return ''