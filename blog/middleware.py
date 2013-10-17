from django import http
from blog.models import Redirect

class RedirectMiddleware(object):
	redirect = http.HttpResponseRedirect
	
	def deslash(self, url):
		return url.strip('/')

	def process_response(self, request, response):
		if response.status_code != 404:
			return response
		
		full_path = self.deslash(request.get_full_path())
		
		for redirect in Redirect.objects.all():
			base = self.deslash(redirect.base)
		
			if base == full_path:
				return self.redirect(redirect.redirect)
		
			if redirect.is_recursive and full_path[:len(base)] == base:
				return self.redirect(redirect.redirect)
		
		return response