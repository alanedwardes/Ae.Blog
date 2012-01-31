from django.conf.urls.defaults import *
from blog.feeds import *
from django.conf import settings
from django.contrib import admin
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
	(r'^portfolio/$', 'blog.views.portfolio'),
	(r'^about/$', 'blog.views.about'),
	#(r'^hire/$', 'blog.views.hire'), // NOT READY, NAUGHTY
	(r'^posts/(?P<post_slug>[a-z0-9-_]+)/$', 'blog.views.single'),
	(r'^s/(?P<file>[a-z0-9-_]+)/$', 'blog.views.shot'),
	(r'^pure/(?P<post_slug>[a-z0-9-_]+)/$', 'blog.views.pure'),
	(r'^media/(?P<path>.*)$', 'django.views.static.serve', {
		'document_root': settings.MEDIA_ROOT,
    }),
	(r'^media/(?P<path>.*)$', 'django.views.static.serve', {
		'document_root': settings.MEDIA_ROOT,
    }),
	(r'^static_media/(?P<path>.*)$', 'django.views.static.serve', {
		'document_root': settings.WEB_ROOT + '/static_media/',
    }),
	(r'^admin-media/(?P<path>.*)$', 'django.views.static.serve', {
		'document_root': settings.WEB_ROOT + '/djangotrunk/django/contrib/admin/media/',
    }),
    (r'^admin/', include(admin.site.urls)),
)