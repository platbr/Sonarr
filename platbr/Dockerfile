FROM ubuntu:xenial as builder

ENV DEBIAN_FRONTEND noninteractive
ARG MONO_VERSION=5.20
ARG MONO_URL=stable-xenial/snapshots/$MONO_VERSION

RUN apt-get update && apt-get -y install curl dirmngr apt-transport-https lsb-release ca-certificates && \
    curl -sL https://deb.nodesource.com/setup_10.x | bash && \
    rm -rf /var/lib/apt/lists/*

RUN curl -sL https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add - && \
    echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list && \
    apt-get update && apt-get -y install yarn && \
    rm -rf /var/lib/apt/lists/*

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb http://download.mono-project.com/repo/debian $MONO_URL main" > /etc/apt/sources.list.d/mono-official-stable.list && \
    apt-get update && apt-get install -y \ 
        tofrodos tzdata \
        mono-complete \
        sqlite3 mediainfo \
        && rm -rf /var/lib/apt/lists/*

WORKDIR /build

COPY . /build
ARG BUILD_NUMBER="3.10.4.001"
ARG BRANCH="platbr"

RUN msbuild /t:restore src/Sonarr.sln
RUN ./build.sh


FROM lsiobase/mono:LTS
ARG BUILD_NUMBER="3.10.4.001"
ARG BRANCH="platbr"
LABEL maintainer="platbr"

RUN mkdir -p /app/sonarr/bin
COPY --from=builder /build/_output_linux /app/sonarr/bin

RUN echo "UpdateMethod=docker\nBranch=${BRANCH}\nPackageVersion=${BUILD_NUMBER}\nPackageAuthor=platbr" > /app/sonarr/package_info && \
    rm -rf /app/sonarr/bin/Sonarr.Update

# add local files
COPY platbr/docker-root/ /

# ports and volumes
EXPOSE 8989
VOLUME /config
