FROM microsoft/aspnet

COPY src/AeBlog AeBlog/
WORKDIR /AeBlog

RUN ["dnu", "restore"]

EXPOSE 5000

ENTRYPOINT ["dnx", "run"]