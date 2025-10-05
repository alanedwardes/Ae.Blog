FROM mcr.microsoft.com/dotnet/aspnet:8.0

ARG TARGETPLATFORM

ADD build/${TARGETPLATFORM} /opt/aeblog

WORKDIR /opt/aeblog

ENTRYPOINT ["/opt/aeblog/Ae.Blog"]
