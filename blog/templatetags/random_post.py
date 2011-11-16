import datetime, re, cgi
from random import choice
from datetime import date
from blog.models import Post
from django import template
register = template.Library()

@register.tag
def random_post(parser, token):
	tag_name, current_slug = token.split_contents()
	return RandomPostNode(current_slug)

class RandomPostNode(template.Node):
	def __init__(self, current_slug):
		self.current_slug = current_slug
	def render(self, context):
		context['random_post'] = Post.objects.order_by('?').filter(type='published').exclude(slug=self.current_slug)[0]
		return ''