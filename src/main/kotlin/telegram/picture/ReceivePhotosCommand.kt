package de.p4ddy.facezoombot.telegram.picture

import de.p4ddy.facezoombot.core.command.Command
import de.p4ddy.facezoombot.telegram.model.Photo

class ReceivePhotosCommand(val chatId: Long, val photoList: List<Photo>) : Command()