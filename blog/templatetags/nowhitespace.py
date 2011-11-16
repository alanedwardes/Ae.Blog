import re
from django import template

register = template.Library()

@register.tag
def nowhitespace(parser, token):
    nodelist = parser.parse(('endnowhitespace',))
    parser.delete_first_token()
    return StripWhiteSpaceNode(nodelist)

class StripWhiteSpaceNode(template.Node):
    def __init__(self, nodelist):
        self.nodelist = nodelist
    def render(self, context):
        output = self.nodelist.render(context)
        return re.sub(r'[\t\n\r\f\v]+', '', output)