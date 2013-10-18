from django.http import HttpResponseRedirect
from blog.models import Redirect

class RedirectMiddleware(object):
	def deslash(self, url):
		return url.strip('/')

	def process_response(self, request, response):
		if response.status_code != 404:
			return response
		
		request_directory = self.deslash(request.path)
		request_fullpath = self.deslash(request.get_full_path())
		
		for redirect in Redirect.objects.all():
			base = self.deslash(redirect.base)
			new_path = request_fullpath[len(base):]
			
			if base == request_directory:
				return HttpResponseRedirect(redirect.redirect + new_path)
			
			if redirect.is_recursive and request_fullpath[:len(base)] == base:
				return HttpResponseRedirect(redirect.redirect + new_path)
		
		return response