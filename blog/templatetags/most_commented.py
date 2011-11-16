from blog.models import Post
from django import template

register = template.Library()

@register.tag
def most_commented(parser, token):
		return MostCommentedNode()

class MostCommentedNode(template.Node):
	def render(self, context):
		context['most_commented'] = Post.objects.all().filter(type='published').order_by('-comments')[:10]
		return ''