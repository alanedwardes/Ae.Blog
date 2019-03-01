FROM mcr.microsoft.com/dotnet/core/sdk

LABEL "com.github.actions.name"="Hello World"
LABEL "com.github.actions.description"="Write arguments to the standard output"
LABEL "com.github.actions.icon"="mic"
LABEL "com.github.actions.color"="purple"

ADD entrypoint.sh /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
