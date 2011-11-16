import datetime, re, cgi
from datetime import date
from django import template
register = template.Library()

@register.filter
def paragraphize(Body):
    Body = Body.replace("\n\r","\n\n")
    CodeTags = ['code','pre']
    for Tag in CodeTags:
        Escape = re.findall('(?s)<'+Tag+'>(.+?)</'+Tag+'>',Body)
        for Code in Escape:
            if re.search('<|>', Code):
                Body = Body.replace(Code,cgi.escape(Code))
    Text = Body.split("\n\n")
    Result = ''
    Allow = ['img', 'a', 'code', 'b', 'em', 'small', 'strong']
    Disallow = ['h3', 'ul', 'ol', 'li', 'pre', 'blockquote', 'object', 'div']
    for T in Text:
        Clear = False
        for A in Allow:
            if re.search('<'+A+'.*?>', T) and not Clear:
                Clear = True
        for D in Disallow:
            if re.search('<'+D+'.*?>', T):
                Clear = False
        if T and (Clear or not re.search('<', T)):
            Result += "\n" + '<p>' + T.lstrip("\n").rstrip("\r\n") + '</p>' + "\n"
        else:
            Result += T
    return Result
	
paragraphize.is_safe = True