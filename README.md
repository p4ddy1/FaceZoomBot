# FaceZoomBot

This is a Telegram Bot which you can send images of people, and it will send only their enlarged faces back. 
If you want to try it send a message to: @face_zoom_bot on Telegram. 
Why? Just for fun!
You can start multiple instances of this bot on many systems,
as long as they can connect to the same RabbitMQ broker and MongoDB.

## Requirements

* Kotlin
* OpenCV with Java Support
* RabbitMQ
* MongoDB

## Running

* Install ![OpenCV for Java](https://opencv-java-tutorials.readthedocs.io/en/latest/01-installing-opencv-for-java.html)
* Create a config.toml based on the config.toml.example
* Run `./gradlew run --args="-c /path/to/config.toml --telegram"` to start as Telegram Bot API Handler
* Run `./gradlew run --args="-c /path/to/config.toml --consumer"` to start as consumer
* The consumers will do the processing of the received pictures using OpenCV. You can start as many as you want.

If you want a complete environment to run the bot look at the Dockerfile in the docker folder.
Just run `./gradlew build` and copy the tar file in the docker folder. Then run `docker build`.