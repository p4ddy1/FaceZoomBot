package de.p4ddy.facezoombot.facezoom

import com.github.kotlintelegrambot.entities.ChatId
import com.github.kotlintelegrambot.entities.TelegramFile
import com.github.kotlintelegrambot.entities.inputmedia.GroupableMedia
import com.github.kotlintelegrambot.entities.inputmedia.InputMediaPhoto
import com.github.kotlintelegrambot.entities.inputmedia.MediaGroup
import com.github.kotlintelegrambot.network.fold
import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.facezoom.repository.FaceRepository
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.api.exception.RequestFailedException
import de.p4ddy.facezoombot.telegram.picture.repository.PhotoRepository
import mu.KotlinLogging
import java.io.File

private val logger = KotlinLogging.logger {}

class SendFacesHandler(
    private val faceRepository: FaceRepository,
    private val photoRepository: PhotoRepository,
    private val telegramBotApi: TelegramBotApi
) : Handler<SendFacesCommand> {
    override suspend fun handle(command: SendFacesCommand) {
        val faces = this.faceRepository.loadByPhotoId(command.photoId)

        val faceMediaList = mutableListOf<GroupableMedia>()
        val fileList = mutableListOf<File>()

        for (face in faces) {
            //TODO: Dont save to disk
            val tmpFileName = "/tmp/${face.key}-${command.chatId}.png"
            File(tmpFileName).writeBytes(face.data)
            val file = File(tmpFileName)
            faceMediaList.add(InputMediaPhoto(TelegramFile.ByFile(file)))
            fileList.add(file)
        }

        val faceCount = faceMediaList.count()

        if (faceCount == 1) {
            this.sendSinglePhoto(command.chatId, fileList)
        }

        if (faceCount > 1) {
            this.sendMultiplePhotos(command.chatId, faceMediaList)
        }

        logger.info {
            "Sent $faceCount Faces to Chat ${command.chatId} for photo ${command.photoId}. " +
            "User: ${command.user.id}. (${command.user.username}, ${command.user.firstName} ${command.user.lastName})"
        }

        this.faceRepository.deleteByPhotoId(command.photoId)
        this.photoRepository.delete(command.photoId)

        for (file in fileList) {
            file.delete()
        }
    }

    private fun sendSinglePhoto(chatId: Long, fileList: List<File>) {
        val response = this.telegramBotApi.bot.sendPhoto(ChatId.fromId(chatId), fileList[0])
        response.fold {
            throw RequestFailedException.fromResponseError(it)
        }
    }

    private fun sendMultiplePhotos(chatId: Long, faceMediaList: List<GroupableMedia>) {
        faceMediaList.chunked(10) { faceChunk ->
            val response = telegramBotApi.bot.sendMediaGroup(ChatId.fromId(chatId), MediaGroup.from(*faceChunk.toTypedArray()))
            if (response.isError) {
                throw RequestFailedException("Could not send photos")
            }
        }
    }
}