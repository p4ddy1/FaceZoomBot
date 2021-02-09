package de.p4ddy.facezoombot.telegram.message

import de.p4ddy.facezoombot.core.command.Handler

class ReceiveMessageHandler : Handler<ReceiveMessageCommand> {
    override suspend fun handle(command: ReceiveMessageCommand) {
        println(command.message)
    }
}