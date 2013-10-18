﻿from blog.models import *
from django.contrib import admin

class PostAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['title', 'body']
	date_hierarchy = 'published'
	list_display = ('title', 'published', 'comments', 'type')
	fields = ('title', 'slug', 'type', 'slideshow', 'body')

admin.site.register(Post, PostAdmin)

class PageAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['title', 'body']
	date_hierarchy = 'published'
	list_display = ('title', 'published', 'type')
	fields = ('title', 'slug', 'type', 'body', 'sidebar')

admin.site.register(Page, PageAdmin)

class CommentAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['name', 'body']
	date_hierarchy = 'published'
	list_display = ('name', 'email', 'published', 'post')

admin.site.register(Comment, CommentAdmin)

class PortfolioAdmin(admin.ModelAdmin):
	ordering = ['-published']
	list_filter = ['published']
	search_fields = ['name', 'summary']
	date_hierarchy = 'published'
	list_display = ('name', 'summary', 'published')
	
class RedirectAdmin(admin.ModelAdmin):
    list_display = ('base', 'redirect', 'is_recursive')
	
admin.site.register(Portfolio, PortfolioAdmin)
admin.site.register(Redirect, RedirectAdmin)
admin.site.register(Screenshot)
admin.site.register(Skill)
