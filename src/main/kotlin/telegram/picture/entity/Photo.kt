package de.p4ddy.facezoombot.telegram.picture.entity

import org.bson.codecs.pojo.annotations.BsonId
import org.litote.kmongo.Id
import org.litote.kmongo.newId

class Photo(
    @BsonId val key: Id<Photo> = newId(),
    val chatId: Long,
    val fileId: String,
    val data: ByteArray
)