from django.conf.urls import *
from blog.feeds import *
from django.conf import settings
from django.contrib import admin
from django.utils.encoding import iri_to_uri
admin.autodiscover()

urlpatterns = patterns('',
	(r'^$', 'blog.views.index'),
	(r'^favicon.ico$', 'django.views.static.serve', {
		'document_root': settings.MEDIA_ROOT,
		'path': 'favicon.ico'
	}),
	(r'^robots.txt$', 'blog.views.robots'),
	(r'^sitemap.xml$', 'blog.views.sitemap'),
	(r'^feeds/posts/$', PostFeed()),
	(r'^feeds/comments/(?P<slug>.*)/$', CommentFeed()),
	(r'^feeds/comments/$', CommentFeed()),
	(r'^json/(?P<method>.*)$', 'blog.views.json'),
	(r'^archive/$', 'blog.views.archive'),
	(r'^random/$', 'blog.views.random'),
	(r'^contact/$', 'blog.views.contact'),
	(r'^posts/(?P<post_slug>.*)/$', 'blog.views.single'),
	(r'^s/(?P<file>.*)/$', 'blog.views.shot'),
	(r'^pure/(?P<post_slug>.*)/$', 'blog.views.pure'),
	(r'^media/(?P<path>.*)$', 'django.views.static.serve', {
		'document_root': settings.MEDIA_ROOT,
	}),
	(r'^admin/', include(admin.site.urls)),
	(r'^(?P<page_slug>.*)/$', 'blog.views.page'),
)