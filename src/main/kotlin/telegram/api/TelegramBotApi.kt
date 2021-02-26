package de.p4ddy.facezoombot.telegram.api

import com.github.kotlintelegrambot.bot
import com.github.kotlintelegrambot.dispatch
import com.github.kotlintelegrambot.dispatcher.photos
import com.github.kotlintelegrambot.dispatcher.text
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.telegram.message.ReceiveMessageCommand
import de.p4ddy.facezoombot.telegram.model.Photo
import de.p4ddy.facezoombot.telegram.model.User
import de.p4ddy.facezoombot.telegram.picture.ReceivePhotosCommand
import mu.KotlinLogging

private val logger = KotlinLogging.logger {}
class TelegramBotApi(botSettings: TelegramBotSettings, transport: TransportBase) {
    val bot = bot {
        token = botSettings.apiToken
        dispatch {
            text {
                val text = update.message?.text
                val user = update.message?.from
                val chatId = update.message?.chat?.id
                val chatType = update.message?.chat?.type

                if (text == null || user == null || chatId == null || chatType == null) {
                    logger.error { "Could not handle message because text or user is null" }
                    return@text
                }

                val command = ReceiveMessageCommand(
                    chatId,
                    text,
                    User.fromTelegramUser(user),
                    chatType
                )

                transport.send(command)
            }

            photos {
                val photos = update.message?.photo
                val user = update.message?.from
                val chatId = update.message?.chat?.id
                val chatType = update.message?.chat?.type

                if (photos == null || user == null || chatId == null || chatType == null) {
                    logger.error { "Could not handle photo update because data is missing" }
                    return@photos
                }

                val photoList = photos.map { Photo.fromTelegramPhoto(it) }
                val command = ReceivePhotosCommand(
                    chatId,
                    User.fromTelegramUser(user),
                    photoList,
                    chatType,
                    update.message?.chat?.title
                )
                transport.send(command)
            }
        }
    }
}