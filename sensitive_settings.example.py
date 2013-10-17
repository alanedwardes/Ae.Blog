import os, datetime

ADMINS = (
	('Alan Edwardes', 'my@email.address'),
)

MANAGERS = ADMINS

DATABASES = {
	'default': {
		'ENGINE': 'django.db.backends.', #Add 'postgresql_psycopg2', 'postgresql', 'mysql', 'sqlite3' or 'oracle'.
		'NAME': '',						 #Or path to database file if using sqlite3.
		'USER': '',						 #Not used with sqlite3.
		'PASSWORD': '',					 #Not used with sqlite3.
		'HOST': '',						 #Set to empty string for localhost. Not used with sqlite3.
		'PORT': '',						 #Set to empty string for default. Not used with sqlite3.
	}
}

BIRTH_DATE = datetime.datetime(yyyy, mm, dd)

SECRET_KEY = 'secret key'

EMAIL_FIELD_NAME = 'anti_spam_email_field_name'
NAME_FIELD_NAME = 'anti_spam_name_field_name'
SUBJECT_FIELD_NAME = 'anti_spam_subject_field_name'
URL_FIELD_NAME = 'anti_spam_url_field_name'
BODY_FIELD_NAME = 'anti_spam_comment_field_name'

ALLOWED_HOSTS = ['example.com']