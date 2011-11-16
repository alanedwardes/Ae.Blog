from blog.models import Comment
from django import template

register = template.Library()

@register.tag
def recent_comments(parser, token):
		return RecentCommentsNode()

class RecentCommentsNode(template.Node):
	def render(self, context):
		context['recent_comments'] = Comment.objects.all().order_by('-published').filter(is_admin=False)[:5]
		return ''