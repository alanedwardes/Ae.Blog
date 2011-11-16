from django import template
register = template.Library()

@register.simple_tag(takes_context=True)
def login(context):
	user = context['user']
	if not user.is_authenticated():
		return '<a id="login-link" href="/admin/">Login</a>'
	else:
		return '<a id="admin-link" href="/admin/">Admin</a>'