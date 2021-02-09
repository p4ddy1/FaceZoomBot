package de.p4ddy.facezoombot.telegram.picture

import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi

class ReceivePhotosHandler(private val telegramBotApi: TelegramBotApi) : Handler<ReceivePhotosCommand> {
    override suspend fun handle(command: ReceivePhotosCommand) {
        val file = telegramBotApi.bot.downloadFileBytes(command.photoList[0].fileId)
    }
}