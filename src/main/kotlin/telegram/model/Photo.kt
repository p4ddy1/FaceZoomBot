package de.p4ddy.facezoombot.telegram.model

import com.github.kotlintelegrambot.entities.files.PhotoSize

data class Photo(
    val fileId: String,
    val fileUniqueId: String,
    val width: Int,
    val height: Int
) {
    companion object Factory {
        fun fromTelegramPhoto(photo: PhotoSize): Photo {
            return Photo(
                photo.fileId,
                photo.fileUniqueId,
                photo.width,
                photo.height
            )
        }
    }
}
