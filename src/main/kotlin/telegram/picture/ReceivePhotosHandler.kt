package de.p4ddy.facezoombot.telegram.picture

import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.picture.entity.Picture
import de.p4ddy.facezoombot.telegram.picture.repository.PictureRepository
import mu.KotlinLogging

private val logger = KotlinLogging.logger {}
class ReceivePhotosHandler(
    private val telegramBotApi: TelegramBotApi,
    private val pictureRepository: PictureRepository
    ) : Handler<ReceivePhotosCommand> {
    override suspend fun handle(command: ReceivePhotosCommand) {
        val user = command.user

        for (photo in command.photoList) {
            logger.info { "Photo ${photo.fileId} received! ChatId: ${user.id}. (${user.username}, ${user.firstName} ${user.lastName})" }

            val photoFile = telegramBotApi.bot.downloadFileBytes(photo.fileId)
            if (photoFile == null) {
                logger.error { "Could not download photo file for ${photo.fileId}" }
                continue
            }

            val picture = Picture(data = photoFile, chatId = user.id, fileId = photo.fileId)
            this.pictureRepository.persist(picture)
        }
    }
}