from django import template

register = template.Library()

@register.filter()
def htmlentities(s):
    return s.encode('ascii', 'xmlcharrefreplace')