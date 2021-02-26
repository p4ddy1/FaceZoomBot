package de.p4ddy.facezoombot.telegram.picture

import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.facezoom.ZoomFacesCommand
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.picture.entity.Photo
import de.p4ddy.facezoombot.telegram.picture.repository.PhotoRepository
import mu.KotlinLogging

private val logger = KotlinLogging.logger {}
class ReceivePhotosHandler(
    private val telegramBotApi: TelegramBotApi,
    private val photoRepository: PhotoRepository,
    private val transport: TransportBase
    ) : Handler<ReceivePhotosCommand> {
    override suspend fun handle(command: ReceivePhotosCommand) {
        val picture = this.storePicture(command) ?: return

        val zoomFacesCommand = ZoomFacesCommand(picture.key.toString(), command.chatId, command.user, command.chatType)
        transport.send(zoomFacesCommand)
    }

    private suspend fun storePicture(command: ReceivePhotosCommand): Photo? {
        val user = command.user
        val photo = command.photoList.lastOrNull()

        if (photo == null) {
            logger.error { "No photo received" }
            return null
        }

        logger.info { "Photo ${photo.fileId} received! ChatId: ${command.chatId} (${command.chatTitle}) User: ${user.id}. (${user.username}, ${user.firstName} ${user.lastName})" }

        val photoFile = telegramBotApi.bot.downloadFileBytes(photo.fileId)
        if (photoFile == null) {
            logger.error { "Could not download photo file for ${photo.fileId}" }
            return null
        }

        val picture = Photo(data = photoFile, chatId = command.chatId, fileId = photo.fileId)
        this.photoRepository.persist(picture)

        return picture
    }
}