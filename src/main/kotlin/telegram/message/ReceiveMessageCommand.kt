package de.p4ddy.facezoombot.telegram.message

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.telegram.model.User

class ReceiveMessageCommand(
    val chatId : Long,
    val message: String,
    val user: User,
    val chatType: String
) : Command()