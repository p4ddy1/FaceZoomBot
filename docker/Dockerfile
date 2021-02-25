FROM alpine:3.13

RUN apk add --no-cache openjdk8 openjdk8-jre-base cmake apache-ant gcc g++ make wget unzip linux-headers openblas openblas-dev \
        libjpeg-turbo libjpeg-turbo-dev libpng libpng-dev gstreamer gstreamer-dev && \
    wget https://github.com/opencv/opencv/archive/4.4.0.zip -O opencv.zip && \
    unzip opencv.zip && \
    mv opencv-4.4.0 opencv && \
    rm opencv.zip && \
    mkdir -p /opencv/build && \
    mkdir -p /usr/local/opencv && \
    cd /opencv/build && \
    cmake \
        -D CMAKE_BUILD_TYPE=RELEASE \
        # Configure Paths
        -D CMAKE_INSTALL_PREFIX=/usr/local/opencv \
        -D JAVA_AWT_INCLUDE_PATH=/usr/lib/jvm/default-jvm/include \
        -D JAVA_AWT_LIBRARY=/usr/lib/jvm/default-jvm/include/jawt.h \
        -D JAVA_INCLUDE_PATH=/usr/lib/jvm/default-jvm/include \
        -D JAVA_INCLUDE_PATH2=/usr/lib/jvm/default-jvm/include/linux \
        -D JAVA_JVM_LIBRARY=/usr/lib/jvm/default-jvm/include/jni.h \
        # Disable not needed stuff
        -D BUILD_PERF_TESTS=NO \
        -D BUILD_SHARED_LIBS=NO \
        -D BUILD_TESTS=NO \
        -D BUILD_opencv_python=NO \
        -D BUILD_IPP_IW=NO \
        -D BUILD_ITT=NO \
        -D BUILD_OPENEXR=NO \
        -D BUILD_TIFF=NO \
        -D BUILD_TIFF=NO \
        -D BUILD_WEBP=NO \
        -D BUILD_opencv_dnn=NO \
        -D BUILD_opencv_gapi=NO \
        -D BUILD_opencv_highgui=NO \
        -D BUILD_opencv_ml=NO \
        -D BUILD_opencv_photo=NO \
        -D BUILD_opencv_stitching=NO \
        -D BUILD_opencv_ts=NO \
        -D BUILD_opencv_video=NO \
        -D BUILD_opencv_videoio=NO \
        # Build Java Bindings
        -D BUILD_JAVA=YES  \
        -D BUILD_opencv_java=YES \
        -D BUILD_opencv_java_bindings_gen=YES .. && \
    make -j`grep -c '^processor' /proc/cpuinfo` && \
    make install && \
    # Cleanup
    cd / && \
    rm -rf /opencv && \
    apk del --purge cmake apache-ant gcc g++ make wget unzip openblas-dev libjpeg-turbo-dev libpng-dev openjdk8 linux-headers

COPY facezoombot.tar /

RUN tar -xf /facezoombot.tar -C / && \
    rm /facezoombot.tar

ENV FACEZOOMBOT_OPTS="-Djava.library.path=/usr/local/opencv/share/java/opencv4"
ENV OPENCV_FACECASCADECLASSIFIERPATH="/usr/local/opencv/share/opencv4/haarcascades/haarcascade_frontalface_alt.xml"


ENTRYPOINT ["/facezoombot/bin/facezoombot"]
