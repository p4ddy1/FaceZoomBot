package de.p4ddy.facezoombot.facezoom

import com.github.kotlintelegrambot.entities.TelegramFile
import com.github.kotlintelegrambot.entities.inputmedia.GroupableMedia
import com.github.kotlintelegrambot.entities.inputmedia.InputMediaPhoto
import com.github.kotlintelegrambot.entities.inputmedia.MediaGroup
import de.p4ddy.facezoombot.core.command.Handler
import de.p4ddy.facezoombot.facezoom.repository.FaceRepository
import de.p4ddy.facezoombot.telegram.api.TelegramBotApi
import de.p4ddy.facezoombot.telegram.picture.repository.PhotoRepository
import java.io.File
import java.nio.file.Files
import kotlin.io.path.Path

class SendFacesHandler(
    private val faceRepository: FaceRepository,
    private val photoRepository: PhotoRepository,
    private val telegramBotApi: TelegramBotApi
    ): Handler<SendFacesCommand> {
    override suspend fun handle(command: SendFacesCommand) {
        val faces = this.faceRepository.loadByPhotoId(command.photoId)

        val faceMediaList = mutableListOf<GroupableMedia>()
        val fileList = mutableListOf<File>()

        for (face in faces) {
            //TODO: Dont save to disk
            val tmpFileName = "/tmp/${face.key}.png"
            File(tmpFileName).writeBytes(face.data)
            val file = File(tmpFileName)
            faceMediaList.add(InputMediaPhoto(TelegramFile.ByFile(file)))
            fileList.add(file)
        }

        //TODO: Check for failed call. Everywhere where telegram api is called
        telegramBotApi.bot.sendMediaGroup(command.chatId, MediaGroup.from(*faceMediaList.toTypedArray()))

        for (file in fileList) {
            file.delete()
        }
    }
}