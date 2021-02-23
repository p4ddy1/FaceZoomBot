package de.p4ddy.facezoombot.telegram.message

import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.database.mongodb.MongoDbClientProvider
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi

class ReceiveMessageHandler(private val telegramClient: TelegramBotApi) : Handler<ReceiveMessageCommand> {
    override suspend fun handle(command: ReceiveMessageCommand) {
        val helpMessage = "Hey. I'm the FaceZoomBot. Send me your photos and I will zoom on all faces, if I can find any. \n" +
                "You can also add me to a group chat."

        if (command.chatType == "private")
        {
            if (command.message == "/start" || command.message == "/help") {
                telegramClient.bot.sendMessage(
                    command.chatId,
                    helpMessage
                )
                return
            }

            telegramClient.bot.sendMessage(
                command.chatId,
                "Type /help for more info"
            )
        }
    }
}