from django.contrib.syndication.views import Feed
from django.shortcuts import get_object_or_404
from django.utils.html import escape
from blog.models import Post, Comment
from blog.templatetags.specialtags import specialtags
from blog.templatetags.paragraphize import paragraphize

class PostFeed(Feed):
	title = 'Alan Edwardes'
	link = '/'
	description = 'Posts from the blog of Alan Edwardes'

	def items(self):
		return Post.objects.filter(type='published').order_by('-published')[:25]

	def item_title(self, item):
		return item.title

	def item_description(self, item):
		return paragraphize(specialtags(item.body))

	def item_pubdate(self, item):
		return item.published

class CommentFeed(Feed):
	title = 'Alan Edwardes: Comments'
	link = '/'
	description = 'Comments from the blog of Alan Edwardes'

	def get_object(self, request, slug=None):
		if slug:
			return get_object_or_404(Post, slug=slug)
		else:
			return None

	def items(self, post):
		if post:
			return Comment.objects.filter(post=post).order_by('-published')[:25]
		else:
			return Comment.objects.order_by('-published')[:25]

	def item_title(self, item):
		return item.name

	def item_description(self, item):
		return paragraphize(escape(item.body))

	def item_pubdate(self, item):
		return item.published