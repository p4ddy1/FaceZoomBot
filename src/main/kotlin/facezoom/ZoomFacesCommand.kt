package de.p4ddy.facezoombot.facezoom

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.telegram.model.User

class ZoomFacesCommand(
    val photoId: String,
    val chatId: Long,
    val user: User,
    val chatType: String
) : Command()