FROM microsoft/aspnet:1.0.0-beta8-coreclr

COPY src/AeBlog AeBlog/
WORKDIR /AeBlog

RUN ["dnu", "restore"]

EXPOSE 5000
