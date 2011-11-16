from blog.models import *
from django.contrib import admin

class PostAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['title', 'body']
	date_hierarchy = 'published'
	list_display = ('title', 'published', 'comments')
	fields = ('title', 'slug', 'body', 'type', 'slideshow')

admin.site.register(Post, PostAdmin)

class CommentAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['name', 'body']
	date_hierarchy = 'published'
	list_display = ('name', 'email', 'published', 'post')

admin.site.register(Comment, CommentAdmin)

class ProjectAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['title', 'body']
	date_hierarchy = 'published'
	list_display = ('title', 'published')
	fields = ('title', 'url', 'banner', 'body', 'skills')
	
admin.site.register(Project, ProjectAdmin)

class ChangeAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['body']
	date_hierarchy = 'published'
	
admin.site.register(Change, ChangeAdmin)