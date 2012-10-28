from datetime import datetime
from dateutil.relativedelta import relativedelta
from django import template
register = template.Library()

@register.filter(name='timesince_years')
def timesince_years(value, compare=False):
	if not compare:
		return datetime.now().year - value.year
	else:
		return value.year - compare.year