FROM mcr.microsoft.com/dotnet/runtime:6.0

ARG TARGETPLATFORM

ADD build/${TARGETPLATFORM} /opt/aeblog

VOLUME ["/data"]

WORKDIR /data

ENTRYPOINT ["/opt/aeblog/Ae.Blog"]
