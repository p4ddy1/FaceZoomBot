package de.p4ddy.facezoombot.telegram.picture

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.telegram.model.Photo
import de.p4ddy.facezoombot.telegram.model.User

class ReceivePhotosCommand(
    val chatId: Long,
    val user: User,
    val photoList: List<Photo>
    ) : Command()