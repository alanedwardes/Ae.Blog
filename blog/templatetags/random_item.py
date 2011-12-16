import datetime, re, cgi
from random import choice
from datetime import date
from django import template
register = template.Library()

@register.tag
def random_item(parser, token):
	tag_name, str, var = token.split_contents()
	return RandomItemNode(str, var)

class RandomItemNode(template.Node):
	def __init__(self, str, var):
		self.choices = str.replace('"', '').split('|')
		self.var = var
	def render(self, context):
		context[self.var] = choice(self.choices)
		return ''