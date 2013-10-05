from django.dispatch import receiver
from django.db.models.signals import post_save, post_delete
from django.db import models
import datetime

content_type_choices = (
	('draft', 'Draft'),
	('published', 'Published')
)

class Post(models.Model):
	title = models.CharField(max_length=100)
	slug = models.SlugField(max_length=75)
	body = models.TextField()
	slideshow = models.CharField(max_length=1024, blank=True)
	published = models.DateTimeField(auto_now_add=True)
	type = models.CharField(max_length=255, choices=content_type_choices)
	comments = models.IntegerField(default=0)
	def get_absolute_url(self):
		return "/posts/%s/" % self.slug
	def is_old(self):
		now = datetime.datetime.now()
		if self.published < datetime.datetime(now.year - 2, now.month, now.day):
			return True
		return False
	def __unicode__(self):
		return self.title
		
class Page(models.Model):
	title = models.CharField(max_length=100)
	slug = models.SlugField(max_length=75)
	body = models.TextField()
	sidebar = models.TextField()
	published = models.DateTimeField(auto_now_add=True)
	type = models.CharField(max_length=255, choices=content_type_choices)
	def get_absolute_url(self):
		return "/%s/" % self.slug
	def __unicode__(self):
		return self.title

class Comment(models.Model):
	name = models.CharField(max_length = 100)
	email = models.EmailField(blank = True)
	ip = models.GenericIPAddressField(blank = True)
	url = models.URLField(blank = True)
	body = models.TextField()
	published = models.DateTimeField(auto_now_add = True)
	is_admin = models.BooleanField()
	post = models.ForeignKey('Post')
	def get_absolute_url(self):
		return "/posts/%s/#comment-%i" % (self.post.slug, self.id)
	def __unicode__(self):
		return self.name
		
class Screenshot(models.Model):
	image = models.FileField(upload_to='uploads/portfolio')
	name = models.CharField(max_length = 100, blank = True)
	published = models.DateTimeField(auto_now_add = True)
	description = models.TextField(blank = True)
	
class Skill(models.Model):
	name = models.CharField(max_length = 100)
	def get_absolute_url(self):
		return "/portfolio/skill/%s/" % self.id
	def __unicode__(self):
		return self.name

class Portfolio(models.Model):
	name = models.CharField(max_length = 100)
	published = models.DateField(blank = True, null = True)
	summary = models.CharField(max_length = 512)
	type = models.CharField(max_length=255, choices=content_type_choices)
	description = models.TextField(blank = True)
	preview = models.TextField(blank = True)
	banner = models.FileField(upload_to='uploads/portfolio', blank = True)
	screenshots = models.ManyToManyField(Screenshot, blank = True)
	skills = models.ManyToManyField(Skill, blank = True)
	def get_absolute_url(self):
		return "/portfolio/item/%s/" % self.id

@receiver(post_save, sender=Comment)
@receiver(post_delete, sender=Comment)
def update_comment_count(signal, sender, instance, **kwargs):
	instance.post.comments = Comment.objects.all().filter(post=instance.post).count()
	instance.post.save()