FROM mcr.microsoft.com/dotnet/runtime:8.0

ARG TARGETPLATFORM

ADD build/${TARGETPLATFORM} /opt/aeblog

WORKDIR /opt/aeblog

ENTRYPOINT ["/opt/aeblog/Ae.Blog"]
