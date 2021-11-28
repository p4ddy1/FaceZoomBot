package de.p4ddy.facezoombot.facezoom

import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.core.transport.TransportBase
import de.p4ddy.facezoombot.facezoom.entity.Face
import de.p4ddy.facezoombot.facezoom.repository.FaceRepository
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.picture.repository.PhotoRepository
import mu.KotlinLogging
import java.util.*

private val logger = KotlinLogging.logger {}
class ZoomFacesHandler(
    private val photoRepository: PhotoRepository,
    private val faceZoomer: FaceZoomer,
    private val faceRepository: FaceRepository,
    private val transport: TransportBase,
    private val telegramBotApi: TelegramBotApi
    ) : Handler<ZoomFacesCommand> {
    override suspend fun handle(command: ZoomFacesCommand) {
        val picture = photoRepository.loadById(command.photoId)
        if (picture == null) {
            logger.error { "Could not load picture with id ${command.photoId} from database" }
            return
        }

        val faceList = faceZoomer.zoom(picture.data)
        val faceCount = faceList.count()

        for (face in faceList) {
            val faceEntity = Face(photoId = command.photoId, data = face, createdAt = Date())
            faceRepository.persist(faceEntity)
        }

        logger.info { "Found $faceCount Faces in photo ${command.photoId} from chat ${command.chatId}" }

        if (faceCount == 0) {
            this.sendNoFaceFoundMessage(command)
            this.photoRepository.delete(command.photoId)
        } else {
            this.transport.send(SendFacesCommand(command.photoId, command.chatId, command.user))
        }
    }

    private fun sendNoFaceFoundMessage(command: ZoomFacesCommand) {
        if (command.chatType == "private") {
            this.telegramBotApi.bot.sendMessage(
                command.chatId,
                "Sorry, I was not able to find any faces in your picture."
            )
        }
    }
}